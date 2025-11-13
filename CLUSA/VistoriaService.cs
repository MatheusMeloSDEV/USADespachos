using MongoDB.Driver;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CLUSA
{
    public class VistoriaService
    {
        private readonly RepositorioOrgaoAnuente _repoOrgaoAnuente;
        private readonly RepositorioVistorias _repoVistorias;
        private readonly RepositorioProcesso _repoProcesso;

        public VistoriaService(IMongoDatabase database)
        {
            _repoOrgaoAnuente = new RepositorioOrgaoAnuente();
            _repoVistorias = new RepositorioVistorias(database);
            _repoProcesso = new RepositorioProcesso();
        }

        public async Task<List<string>> SincronizarVistoriasAsync()
        {
            var listaAlteracoes = new List<string>();

            var parametrizacoesAlvo = new HashSet<string> { "EXAME FÍSICO", "CONFERÊNCIA FÍSICA", "COLETA DE AMOSTRA", "INSPEÇÃO FÍSICA" };

            var todasAsLIs = await _repoOrgaoAnuente.GetAllAsync();
            var vistoriasExistentes = (await _repoVistorias.GetAllAsync()).ToDictionary(v => v.LPCO);

            var todosProcessos = await _repoProcesso.ListarTodosAsync();
            var processosDict = todosProcessos.ToDictionary(p => p.Ref_USA);

            var lisSemParametrizacao = todasAsLIs
                .Where(li => (li.StatusLI ?? "").ToUpperInvariant() == "ENTRADA CONCLUÍDA"
                    && li.LPCO != null
                    && li.LPCO.Any(lpco =>
                        (lpco.NomeOrgao ?? "").ToUpperInvariant() == "MAPA" &&
                        string.IsNullOrEmpty(lpco.ParametrizacaoLPCO) &&
                        (lpco.MotivoExigencia ?? "").ToUpperInvariant() != "DEFERIDO" &&
                        (lpco.MotivoExigencia ?? "").ToUpperInvariant() != "CANCELADA"))
                .ToList();
                        
            foreach (var li in lisSemParametrizacao)
            {
                var primeiroLpcoMapa = li.LPCO?.FirstOrDefault(lpco =>
                    (lpco.NomeOrgao ?? "").ToUpperInvariant() == "MAPA");

                if (primeiroLpcoMapa == null)
                    continue;

                processosDict.TryGetValue(li.Ref_USA, out var processoCorrespondente);

                var vistoriaEntrada = new Vistoria
                {
                    LI = li.Numero,
                    LPCO = primeiroLpcoMapa.LPCO ?? "",
                    Importador = li.Importador,
                    Container = li.Container,
                    Conhecimento = li.Conhecimento,
                    Ref_USA = li.Ref_USA,
                    Produto = li.Produto,
                    ParametrizacaoLPCO = primeiroLpcoMapa.ParametrizacaoLPCO ?? "",
                    Terminal = processoCorrespondente?.Terminal ?? string.Empty,
                    Previsao = processoCorrespondente?.DataDeAtracacao,
                    Status = StatusVistoria.ProcessoDadoEntrada
                };

                var vistoriaExistente = await _repoVistorias.GetByLPCOAsync(vistoriaEntrada.LPCO);

                bool precisaAtualizar =
                    vistoriaExistente == null ||
                    vistoriaExistente.LI != vistoriaEntrada.LI ||
                    vistoriaExistente.Importador != vistoriaEntrada.Importador ||
                    vistoriaExistente.Container != vistoriaEntrada.Container ||
                    vistoriaExistente.Conhecimento != vistoriaEntrada.Conhecimento ||
                    vistoriaExistente.Ref_USA != vistoriaEntrada.Ref_USA ||
                    vistoriaExistente.Produto != vistoriaEntrada.Produto ||
                    vistoriaExistente.ParametrizacaoLPCO != vistoriaEntrada.ParametrizacaoLPCO ||
                    vistoriaExistente.Terminal != vistoriaEntrada.Terminal ||
                    vistoriaExistente.Previsao != vistoriaEntrada.Previsao ||
                    vistoriaExistente.Status != vistoriaEntrada.Status;

                if (precisaAtualizar)
                {
                    await _repoVistorias.UpsertAsync(vistoriaEntrada);
                    listaAlteracoes.Add($"Atualizada ou criada LI: {vistoriaEntrada.LI}");
                }
            }

            var lpsVistoria = todasAsLIs
                .SelectMany(li => li.LPCO.Select(lpco => new { LI = li, LPCO = lpco }))
                .Where(x => x.LPCO.NomeOrgao == "MAPA"
                            && !string.IsNullOrEmpty(x.LPCO.LPCO)
                            && parametrizacoesAlvo.Contains(x.LPCO.ParametrizacaoLPCO.ToUpper())
                            && x.LPCO.MotivoExigencia?.ToUpper() != "DEFERIDO"
                            && x.LPCO.MotivoExigencia?.ToUpper() != "CANCELADA");


            foreach (var item in lpsVistoria)
            {
                if (vistoriasExistentes.ContainsKey(item.LPCO.LPCO))
                {
                    continue;
                }

                processosDict.TryGetValue(item.LI.Ref_USA, out var processoCorrespondente);

                var novaVistoria = new Vistoria
                {
                    LI = item.LI.Numero,
                    LPCO = item.LPCO.LPCO,
                    Importador = item.LI.Importador,
                    Container = item.LI.Container,
                    Conhecimento = item.LI.Conhecimento,
                    Ref_USA = item.LI.Ref_USA,
                    Produto = item.LI.Produto,
                    ParametrizacaoLPCO = item.LPCO.ParametrizacaoLPCO,
                    Terminal = processoCorrespondente?.Terminal ?? string.Empty,
                    Previsao = processoCorrespondente?.DataDeAtracacao,
                    Status = StatusVistoria.AguardandoChegadaParaAgendar
                };

                await _repoVistorias.UpsertAsync(novaVistoria);
                listaAlteracoes.Add($"Nova vistoria adicionada para LI {item.LI.Numero}, LPCO: {item.LPCO.LPCO}");
            }

            var vistoriasProcessoEntrada = (await _repoVistorias.GetAllAsync())
                .Where(v => v.Status == StatusVistoria.ProcessoDadoEntrada)
                .ToList();

            foreach (var vistoria in vistoriasProcessoEntrada)
            {
                var li = todasAsLIs.FirstOrDefault(x => x.Numero == vistoria.LI);

                if (li == null || li.LPCO == null)
                    continue;

                var lpcoMapa = li.LPCO.FirstOrDefault(lpco =>
                    (lpco.NomeOrgao ?? "").ToUpperInvariant() == "MAPA");

                if (lpcoMapa != null && (lpcoMapa.MotivoExigencia?.ToUpperInvariant() == "CANCELADA"))
                {
                    continue;
                }

                if (lpcoMapa != null && !string.IsNullOrEmpty(lpcoMapa.ParametrizacaoLPCO))
                {
                    await _repoVistorias.DeleteAsync(vistoria.Id);
                    listaAlteracoes.Add($"Vistoria excluída para LI {li.Numero}");
                    processosDict.TryGetValue(li.Ref_USA, out var processoCorrespondente);
                    if (lpcoMapa.ParametrizacaoLPCO.ToUpperInvariant() != "DOCUMENTAL")
                    {
                        var novaVistoria = new Vistoria
                        {
                            LI = li.Numero,
                            LPCO = lpcoMapa.LPCO ?? "",
                            Importador = li.Importador,
                            Container = li.Container,
                            Conhecimento = li.Conhecimento,
                            Ref_USA = li.Ref_USA,
                            Produto = li.Produto,
                            ParametrizacaoLPCO = lpcoMapa.ParametrizacaoLPCO,
                            Terminal = processoCorrespondente?.Terminal ?? string.Empty,
                            Previsao = processoCorrespondente?.DataDeAtracacao,
                            Status = StatusVistoria.AguardandoChegadaParaAgendar
                        };
                        await _repoVistorias.InsertAsync(novaVistoria);
                        listaAlteracoes.Add($"Vistoria criada para LI {li.Numero}");
                    }
                }
            }
            return listaAlteracoes;
        }
    }
}

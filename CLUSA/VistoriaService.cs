using MongoDB.Driver;
using System.Collections.Generic;
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

        public async Task SincronizarVistoriasAsync()
        {
            // 1. Define as parametrizações que geram uma vistoria
            var parametrizacoesAlvo = new HashSet<string>
            {
                "EXAME FÍSICO", "CONFERÊNCIA FÍSICA",
                "COLETA DE AMOSTRA", "INSPEÇÃO FÍSICA"
            };

            // 2. Busca todos os dados necessários de uma só vez para otimizar a performance
            var todasAsLIs = await _repoOrgaoAnuente.GetAllAsync();
            var vistoriasExistentes = (await _repoVistorias.GetAllAsync()).ToDictionary(v => v.LPCO);

            // MUDANÇA: Busca todos os processos e os transforma em um dicionário para consulta rápida
            var todosProcessos = await _repoProcesso.ListarTodosAsync();
            var processosDict = todosProcessos.ToDictionary(p => p.Ref_USA);

            // 3. Encontra todos os LPCOs do MAPA que precisam de vistoria
            var lpsVistoria = todasAsLIs
                .SelectMany(li => li.LPCO.Select(lpco => new { LI = li, LPCO = lpco }))
                .Where(x =>
                    x.LPCO.NomeOrgao == "MAPA" &&
                    !string.IsNullOrEmpty(x.LPCO.LPCO) &&
                    parametrizacoesAlvo.Contains(x.LPCO.ParametrizacaoLPCO.ToUpper()) &&
                    x.LPCO.MotivoExigencia?.ToUpper() != "DEFERIDO"
                );

            // 4. Sincroniza os dados
            foreach (var item in lpsVistoria)
            {
                if (vistoriasExistentes.ContainsKey(item.LPCO.LPCO))
                {
                    continue; // Se a vistoria já existe, pula para a próxima
                }

                // MUDANÇA: Busca o processo correspondente no dicionário
                processosDict.TryGetValue(item.LI.Ref_USA, out var processoCorrespondente);

                // Se não existe, cria um novo objeto Vistoria
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
                    // MUDANÇA: Adiciona o Terminal, usando o processo que buscamos
                    Terminal = processoCorrespondente?.Terminal ?? string.Empty,
                    Status = StatusVistoria.AguardandoChegadaParaAgendar
                };

                await _repoVistorias.UpsertAsync(novaVistoria);
            }
        }
    }
}
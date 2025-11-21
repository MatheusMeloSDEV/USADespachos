using MongoDB.Driver;
using System.Linq;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

namespace CLUSA
{
    public class VistoriaService
    {
        private readonly RepositorioOrgaoAnuente _repoOrgaoAnuente;
        private readonly RepositorioVistorias _repoVistorias;
        private readonly RepositorioProcesso _repoProcesso;

        // Parametrizações que exigem vistoria física no MAPA
        private readonly HashSet<string> _parametrizacoesMapaAlvo = new()
        {
            "EXAME FÍSICO",
            "CONFERÊNCIA FÍSICA",
            "COLETA DE AMOSTRA",
            "INSPEÇÃO FÍSICA"
        };

        public VistoriaService(IMongoDatabase database)
        {
            _repoOrgaoAnuente = new RepositorioOrgaoAnuente();
            _repoVistorias = new RepositorioVistorias(database);
            _repoProcesso = new RepositorioProcesso();
        }

        public async Task<List<string>> SincronizarVistoriasAsync()
        {
            var listaAlteracoes = new List<string>();

            // 1. Carregamento de Dados
            // Traz APENAS processos ativos para otimizar
            var todosProcessos = await _repoProcesso.ListarProcessosAtivosParaStatusAsync();
            var processosDict = todosProcessos.ToDictionary(p => p.Ref_USA);

            var todasAsLIs = await _repoOrgaoAnuente.GetAllAsync();

            // Dicionário de vistorias existentes indexado pelo LPCO
            var vistoriasExistentes = (await _repoVistorias.GetAllAsync())
                                      .Where(v => !string.IsNullOrEmpty(v.LPCO))
                                      .ToDictionary(v => v.LPCO);

            // 2. Loop Único de Processamento
            foreach (var orgaoAnuente in todasAsLIs)
            {
                if (orgaoAnuente.LPCO == null || !orgaoAnuente.LPCO.Any()) continue;

                processosDict.TryGetValue(orgaoAnuente.Ref_USA, out var processoPai);

                foreach (var lpcoInfo in orgaoAnuente.LPCO)
                {
                    // Verificação de nulidade da propriedade string
                    if (string.IsNullOrEmpty(lpcoInfo.LPCO)) continue;

                    // --- ANÁLISE DA REGRA DE NEGÓCIO ---
                    // CORREÇÃO: lpcoInfo é do tipo LpcoInfo, não LPCO
                    var (deveTerVistoria, statusSugerido) = AnalisarLpco(lpcoInfo);

                    // Cenário A: O LPCO deve ter uma vistoria ativa
                    if (deveTerVistoria)
                    {
                        var novaVistoria = new Vistoria
                        {
                            // CORREÇÃO: orgaoAnuente.Numero é int, Vistoria.LI é string. Precisa do ToString()
                            LI = orgaoAnuente.Numero.ToString(),
                            LPCO = lpcoInfo.LPCO,
                            Importador = orgaoAnuente.Importador,
                            Container = orgaoAnuente.Container,
                            Conhecimento = orgaoAnuente.Conhecimento,
                            Ref_USA = orgaoAnuente.Ref_USA,
                            Produto = orgaoAnuente.Produto,
                            ParametrizacaoLPCO = lpcoInfo.ParametrizacaoLPCO ?? "",
                            Terminal = processoPai?.Terminal ?? string.Empty,
                            Previsao = processoPai?.DataDeAtracacao,
                            Status = statusSugerido
                        };

                        if (vistoriasExistentes.TryGetValue(lpcoInfo.LPCO, out var vistoriaDb))
                        {
                            // Atualização
                            bool mudouParametrizacao = vistoriaDb.ParametrizacaoLPCO != novaVistoria.ParametrizacaoLPCO;
                            novaVistoria.Id = vistoriaDb.Id;

                            if (!mudouParametrizacao)
                            {
                                novaVistoria.Status = vistoriaDb.Status;
                            }

                            if (mudouParametrizacao ||
                                vistoriaDb.Terminal != novaVistoria.Terminal ||
                                vistoriaDb.Previsao != novaVistoria.Previsao)
                            {
                                await _repoVistorias.UpsertAsync(novaVistoria);
                                listaAlteracoes.Add($"Atualizado LPCO {lpcoInfo.LPCO}");
                            }
                        }
                        else
                        {
                            // Inserção
                            await _repoVistorias.UpsertAsync(novaVistoria);
                            listaAlteracoes.Add($"Novo LPCO adicionado: {lpcoInfo.LPCO}");
                        }
                    }
                    // Cenário B: Remover se existir
                    else if (vistoriasExistentes.ContainsKey(lpcoInfo.LPCO))
                    {
                        await _repoVistorias.DeleteByLpcoAsync(lpcoInfo.LPCO);
                        listaAlteracoes.Add($"Vistoria removida: {lpcoInfo.LPCO}");
                    }
                }
            }

            return listaAlteracoes;
        }

        /// <summary>
        /// Analisa um LPCO e determina se ele deve gerar uma vistoria.
        /// CORREÇÃO: Tipo do parâmetro alterado de LPCO para LpcoInfo
        /// </summary>
        /// <summary>
        /// Analisa um LPCO e determina se ele deve gerar uma vistoria.
        /// </summary>
        private (bool DeveTerVistoria, StatusVistoria Status) AnalisarLpco(LpcoInfo lpco)
        {
            var nomeOrgao = (lpco.NomeOrgao ?? "").ToUpperInvariant();
            var motivo = (lpco.MotivoExigencia ?? "").ToUpperInvariant();
            var parametrizacao = (lpco.ParametrizacaoLPCO ?? "").ToUpperInvariant();

            // CORREÇÃO: Usamos lpco.StatusLPCO agora, pois StatusLI não existe mais no pai
            var statusLpcoNormalizado = (lpco.StatusLPCO ?? "").ToUpperInvariant();

            // 1. Regra Global: Se Deferido ou Cancelado, nunca gera vistoria
            if (motivo == "DEFERIDO" || motivo == "CANCELADA")
            {
                return (false, StatusVistoria.AguardandoChegadaParaAgendar);
            }

            // 2. REGRA ANVISA
            if (nomeOrgao.Contains("ANVISA"))
            {
                // Verifica o status no LPCO
                if ((string.IsNullOrEmpty(parametrizacao) || parametrizacao == "DOCUMENTAL")
                    && statusLpcoNormalizado == "ENTRADA CONCLUÍDA")
                {
                    return (true, StatusVistoria.ProcessoDadoEntrada);
                }

                return (false, StatusVistoria.AguardandoChegadaParaAgendar);
            }

            // 3. REGRA MAPA
            if (nomeOrgao == "MAPA")
            {
                if (!string.IsNullOrEmpty(parametrizacao) && _parametrizacoesMapaAlvo.Contains(parametrizacao))
                {
                    return (true, StatusVistoria.AguardandoChegadaParaAgendar);
                }

                // Verifica o status no LPCO
                if (string.IsNullOrEmpty(parametrizacao) && statusLpcoNormalizado == "ENTRADA CONCLUÍDA")
                {
                    return (true, StatusVistoria.ProcessoDadoEntrada);
                }

                return (false, StatusVistoria.AguardandoChegadaParaAgendar);
            }

            return (false, StatusVistoria.AguardandoChegadaParaAgendar);
        }
    }
}
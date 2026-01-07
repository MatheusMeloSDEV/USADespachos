using CLUSA.Models;
using CLUSA.Repositories;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLUSA.Services
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
            var listaLog = new List<string>();

            // 1. Carregar APENAS processos ATIVOS (Isso já filtra 90% do lixo)
            var processosAtivos = await _repoProcesso.ListarProcessosAtivosParaStatusAsync();

            if (!processosAtivos.Any()) return listaLog;

            // Cria dicionário para acesso rápido O(1)
            var processosDict = processosAtivos.ToDictionary(p => p.Ref_USA);
            var listaRefUsas = processosDict.Keys.ToList();

            // 2. Carregar LIs e Vistorias APENAS desses processos ativos (Evita Full Scan)
            // Executa em paralelo
            var taskLIs = _repoOrgaoAnuente.GetByListaRefUsaAsync(listaRefUsas);
            var taskVistorias = _repoVistorias.GetByListaRefUsaAsync(listaRefUsas);

            await Task.WhenAll(taskLIs, taskVistorias);

            var lisAtivas = taskLIs.Result;
            var vistoriasDb = taskVistorias.Result;

            // Indexar vistorias existentes por LPCO
            var vistoriasDict = vistoriasDb
                .Where(v => !string.IsNullOrEmpty(v.LPCO))
                .ToDictionary(v => v.LPCO);

            // Preparar Bulk Operations para o Mongo (Muito mais rápido que salvar 1 por 1)
            var bulkOps = new List<WriteModel<Vistoria>>();

            // Rastrear LPCOs processados para saber quais excluir depois
            var lpcosProcessados = new HashSet<string>();

            // 3. Processamento em Memória
            foreach (var orgaoAnuente in lisAtivas)
            {
                if (orgaoAnuente.LPCO == null) continue;

                processosDict.TryGetValue(orgaoAnuente.Ref_USA, out var processoPai);

                foreach (var lpcoInfo in orgaoAnuente.LPCO)
                {
                    if (string.IsNullOrEmpty(lpcoInfo.LPCO)) continue;

                    var (deveTerVistoria, statusSugerido) = AnalisarLpco(lpcoInfo);

                    if (deveTerVistoria)
                    {
                        lpcosProcessados.Add(lpcoInfo.LPCO);

                        var novaVistoria = new Vistoria
                        {
                            LI = orgaoAnuente.Numero?.ToString() ?? "",
                            LPCO = lpcoInfo.LPCO,
                            Importador = orgaoAnuente.Importador,
                            Container = orgaoAnuente.Container,
                            Conhecimento = orgaoAnuente.Conhecimento,
                            Ref_USA = orgaoAnuente.Ref_USA,
                            Produto = orgaoAnuente.Produto,
                            ParametrizacaoLPCO = lpcoInfo.ParametrizacaoLPCO ?? "",
                            Terminal = processoPai?.Terminal ?? string.Empty,
                            DataRegistroLPCO = lpcoInfo.DataRegistroLPCO,
                            Previsao = processoPai?.DataDeAtracacao,
                            Status = statusSugerido
                        };

                        if (vistoriasDict.TryGetValue(lpcoInfo.LPCO, out var vistoriaDb))
                        {
                            // --- Lógica de Atualização ---
                            bool mudouParametrizacao = vistoriaDb.ParametrizacaoLPCO != novaVistoria.ParametrizacaoLPCO;

                            // Mantém o ID e Status antigo se não mudou parametrização
                            novaVistoria.Id = vistoriaDb.Id;
                            if (!mudouParametrizacao) novaVistoria.Status = vistoriaDb.Status;
                            novaVistoria.Notas = vistoriaDb.Notas; // Mantém notas do usuário

                            // Só atualiza se algo mudou
                            if (mudouParametrizacao ||
                                vistoriaDb.Terminal != novaVistoria.Terminal ||
                                vistoriaDb.Previsao != novaVistoria.Previsao ||
                                vistoriaDb.DataRegistroLPCO != novaVistoria.DataRegistroLPCO)
                            {
                                var filter = Builders<Vistoria>.Filter.Eq(x => x.Id, vistoriaDb.Id);
                                bulkOps.Add(new ReplaceOneModel<Vistoria>(filter, novaVistoria));
                                listaLog.Add($"Atualizado: {lpcoInfo.LPCO}");
                            }
                        }
                        else
                        {
                            // --- Lógica de Inserção ---
                            bulkOps.Add(new InsertOneModel<Vistoria>(novaVistoria));
                            listaLog.Add($"Novo: {lpcoInfo.LPCO}");
                        }
                    }
                }
            }

            // 4. Identificar Vistorias para Remover (De processos ativos que não precisam mais de vistoria)
            // CUIDADO: Não remover vistorias de processos finalizados que não carregamos na etapa 1
            foreach (var vistoriaDb in vistoriasDb)
            {
                // Se a vistoria existe no DB (para um ref ativo), mas não foi processada no loop acima (regra retornou false), delete.
                if (!lpcosProcessados.Contains(vistoriaDb.LPCO))
                {
                    var filter = Builders<Vistoria>.Filter.Eq(x => x.Id, vistoriaDb.Id);
                    bulkOps.Add(new DeleteOneModel<Vistoria>(filter));
                    listaLog.Add($"Removido: {vistoriaDb.LPCO}");
                }
            }

            // 5. Executar TUDO de uma vez
            if (bulkOps.Any())
            {
                await _repoVistorias.ExecutarBulkAsync(bulkOps);
            }

            return listaLog;
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
            if (motivo == "DEFERIDO" || motivo == "CANCELADA") return (false, StatusVistoria.AguardandoChegadaParaAgendar);

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

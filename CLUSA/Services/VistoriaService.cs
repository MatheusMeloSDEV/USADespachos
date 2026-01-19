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

            // 1. Carregar APENAS processos ATIVOS
            var processosAtivos = await _repoProcesso.ListarProcessosAtivosParaStatusAsync();

            if (!processosAtivos.Any()) return listaLog;

            var processosDict = processosAtivos
                .Where(p => !string.IsNullOrEmpty(p.Ref_USA))
                .GroupBy(p => p.Ref_USA)
                .ToDictionary(g => g.Key, g => g.First());

            var listaRefUsas = processosDict.Keys.ToList();

            // 2. Carregar LIs e Vistorias
            var taskLIs = _repoOrgaoAnuente.GetByListaRefUsaAsync(listaRefUsas);
            var taskVistorias = _repoVistorias.GetByListaRefUsaAsync(listaRefUsas);

            await Task.WhenAll(taskLIs, taskVistorias);

            var lisAtivas = taskLIs.Result;
            var vistoriasDb = taskVistorias.Result;

            // BLINDAGEM 2: Cria dicionário com CHAVE ÚNICA (ignora as duplicatas na leitura para não quebrar)
            var vistoriasDict = vistoriasDb
                .Where(v => !string.IsNullOrEmpty(v.LPCO))
                .GroupBy(v => v.LPCO.Trim())
                .ToDictionary(g => g.Key, g => g.First());

            // --- NOVA LOGICA DE LIMPEZA DE DUPLICATAS ---
            // Identifica quais IDs foram "eleitos" como principais pelo GroupBy acima
            var idsPrincipais = vistoriasDict.Values.Select(v => v.Id).ToHashSet();

            // Identifica registros no banco que são cópias extras (não entraram no dicionário)
            var duplicatasParaRemover = vistoriasDb
                .Where(v => !idsPrincipais.Contains(v.Id))
                .ToList();
            // ---------------------------------------------

            var bulkOps = new List<WriteModel<Vistoria>>();

            // Já adiciona as remoções das duplicatas existentes na fila de execução
            foreach (var duplicata in duplicatasParaRemover)
            {
                var filterDup = Builders<Vistoria>.Filter.Eq(x => x.Id, duplicata.Id);
                bulkOps.Add(new DeleteOneModel<Vistoria>(filterDup));
                listaLog.Add($"Duplicata removida automaticamente: {duplicata.LPCO}");
            }

            var lpcosProcessados = new HashSet<string>();

            // 3. Processamento em Memória
            foreach (var orgaoAnuente in lisAtivas)
            {
                if (orgaoAnuente.LPCO == null) continue;

                processosDict.TryGetValue(orgaoAnuente.Ref_USA, out var processoPai);

                foreach (var lpcoInfo in orgaoAnuente.LPCO)
                {
                    if (string.IsNullOrEmpty(lpcoInfo.LPCO)) continue;

                    // --- CORREÇÃO PRINCIPAL AQUI ---
                    // Normaliza ANTES de checar duplicidade
                    string lpcoLimpo = lpcoInfo.LPCO.Trim();

                    // Verifica se o LPCO LIMPO já foi processado nesta execução
                    if (lpcosProcessados.Contains(lpcoLimpo)) continue;

                    // Adiciona o LIMPO na lista de processados
                    lpcosProcessados.Add(lpcoLimpo);
                    // -------------------------------

                    var (deveTerVistoria, statusSugerido) = AnalisarLpco(lpcoInfo);

                    if (deveTerVistoria)
                    {
                        var novaVistoria = new Vistoria
                        {
                            LI = orgaoAnuente.Numero?.ToString() ?? "",
                            LPCO = lpcoLimpo,
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

                        if (vistoriasDict.TryGetValue(lpcoLimpo, out var vistoriaDb))
                        {
                            // --- CENÁRIO: ATUALIZAÇÃO ---
                            string paramAntiga = (vistoriaDb.ParametrizacaoLPCO ?? "").Trim().ToUpper();
                            string paramNova = (novaVistoria.ParametrizacaoLPCO ?? "").Trim().ToUpper();
                            bool mudouParametrizacao = paramAntiga != paramNova;

                            novaVistoria.Id = vistoriaDb.Id;
                            novaVistoria.Notas = vistoriaDb.Notas;

                            bool statusEhInicial = vistoriaDb.Status == StatusVistoria.ProcessoDadoEntrada;
                            bool statusSugeridoEhAvancado = novaVistoria.Status != StatusVistoria.ProcessoDadoEntrada;

                            if (mudouParametrizacao || (statusEhInicial && statusSugeridoEhAvancado))
                            {
                                if (mudouParametrizacao)
                                    listaLog.Add($"Status Resetado por Parametrização ({lpcoLimpo}): {vistoriaDb.Status} -> {novaVistoria.Status}");
                                else
                                    listaLog.Add($"Correção Automática de Status ({lpcoLimpo}): {vistoriaDb.Status} -> {novaVistoria.Status}");
                            }
                            else
                            {
                                novaVistoria.Status = vistoriaDb.Status;
                            }

                            if (mudouParametrizacao ||
                                vistoriaDb.Status != novaVistoria.Status ||
                                vistoriaDb.Terminal != novaVistoria.Terminal ||
                                vistoriaDb.Previsao != novaVistoria.Previsao ||
                                vistoriaDb.DataRegistroLPCO != novaVistoria.DataRegistroLPCO)
                            {
                                var filter = Builders<Vistoria>.Filter.Eq(x => x.Id, vistoriaDb.Id);
                                bulkOps.Add(new ReplaceOneModel<Vistoria>(filter, novaVistoria));

                                if (!mudouParametrizacao && (vistoriaDb.Status == novaVistoria.Status))
                                    listaLog.Add($"Atualizado (Dados): {lpcoLimpo}");
                            }
                        }
                        else
                        {
                            // --- CENÁRIO: INSERÇÃO ---
                            bulkOps.Add(new InsertOneModel<Vistoria>(novaVistoria));
                            listaLog.Add($"Novo: {lpcoLimpo}");
                        }
                    }
                }
            }

            // 4. Identificar Vistorias para Remover (De processos ativos que não precisam mais de vistoria)
            foreach (var vistoriaDb in vistoriasDict.Values)
            {
                // Aqui também usamos o Trim() para garantir que a comparação seja justa
                if (!lpcosProcessados.Contains(vistoriaDb.LPCO.Trim()))
                {
                    var filter = Builders<Vistoria>.Filter.Eq(x => x.Id, vistoriaDb.Id);
                    bulkOps.Add(new DeleteOneModel<Vistoria>(filter));
                    listaLog.Add($"Removido (Critério): {vistoriaDb.LPCO}");
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
        /// Analisa um LPCO e determina se ele deve gerar uma vistoria e qual o Status Inicial.
        /// </summary>
        private (bool DeveTerVistoria, StatusVistoria Status) AnalisarLpco(LpcoInfo lpco)
        {
            var nomeOrgao = (lpco.NomeOrgao ?? "").ToUpperInvariant();
            var motivo = (lpco.MotivoExigencia ?? "").ToUpperInvariant();
            var parametrizacao = (lpco.ParametrizacaoLPCO ?? "").ToUpperInvariant();
            var statusLpco = (lpco.StatusLPCO ?? "").ToUpperInvariant(); // <--- Variável importante

            // 1. BLINDAGEM: Verifica se DEFERIDO aparece no Motivo OU no Status
            if (motivo == "DEFERIDO" || motivo == "CANCELADA" ||
                statusLpco == "DEFERIDO" || statusLpco == "CANCELADO") // <--- Adicione isso
            {
                return (false, StatusVistoria.AguardandoChegadaParaAgendar);
            }

            // --------------------------------------------------------
            // REGRA MAPA
            // --------------------------------------------------------
            if (nomeOrgao == "MAPA")
            {
                // CASO 1: Caiu em Canal Físico (Parametrização está na lista alvo)
                // Vai para: Aguardando Chegada (para agendar)
                if (!string.IsNullOrEmpty(parametrizacao) && _parametrizacoesMapaAlvo.Contains(parametrizacao))
                {
                    return (true, StatusVistoria.AguardandoChegadaParaAgendar);
                }

                // CASO 2: Não tem Parametrização (Vazio) e já deu Entrada
                // Vai para: Processo Dado Entrada
                if (string.IsNullOrEmpty(parametrizacao) && statusLpco == "ENTRADA CONCLUÍDA")
                {
                    return (true, StatusVistoria.ProcessoDadoEntrada);
                }

                // Qualquer outro caso do MAPA não gera vistoria por enquanto
                return (false, StatusVistoria.AguardandoChegadaParaAgendar);
            }

            // --------------------------------------------------------
            // REGRA ANVISA (Mantendo lógica similar para consistência)
            // --------------------------------------------------------
            if (nomeOrgao.Contains("ANVISA"))
            {
                if (statusLpco == "ENTRADA CONCLUÍDA")
                {
                    // Se for documental ou vazio, segue como Dado Entrada
                    if (string.IsNullOrEmpty(parametrizacao) || parametrizacao == "DOCUMENTAL")
                    {
                        return (true, StatusVistoria.ProcessoDadoEntrada);
                    }
                }
                return (false, StatusVistoria.AguardandoChegadaParaAgendar);
            }

            return (false, StatusVistoria.AguardandoChegadaParaAgendar);
        }
    }
}

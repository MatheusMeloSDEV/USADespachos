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

            // BLINDAGEM 1: Evita crash se houver Ref_USA duplicada na tabela de Processos
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

            // BLINDAGEM 2: Evita crash de chave duplicada (erro que você teve anteriormente)
            var vistoriasDict = vistoriasDb
                .Where(v => !string.IsNullOrEmpty(v.LPCO))
                .GroupBy(v => v.LPCO)
                .ToDictionary(g => g.Key, g => g.First());

            var bulkOps = new List<WriteModel<Vistoria>>();
            var lpcosProcessados = new HashSet<string>();

            // 3. Processamento em Memória
            foreach (var orgaoAnuente in lisAtivas)
            {
                if (orgaoAnuente.LPCO == null) continue;

                processosDict.TryGetValue(orgaoAnuente.Ref_USA, out var processoPai);

                foreach (var lpcoInfo in orgaoAnuente.LPCO)
                {
                    if (string.IsNullOrEmpty(lpcoInfo.LPCO)) continue;

                    // Evita processar o mesmo LPCO duas vezes na mesma execução
                    if (lpcosProcessados.Contains(lpcoInfo.LPCO)) continue;

                    var (deveTerVistoria, statusSugerido) = AnalisarLpco(lpcoInfo);

                    if (deveTerVistoria)
                    {
                        lpcosProcessados.Add(lpcoInfo.LPCO);

                        // Cria o objeto com o Status Sugerido (Base)
                        // Esse status só será usado se for uma INSERÇÃO (Novo registro)
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
                            Status = statusSugerido // Status inicial padrão
                        };

                        if (vistoriasDict.TryGetValue(lpcoInfo.LPCO, out var vistoriaDb))
                        {
                            // --- CENÁRIO: JÁ EXISTE NO BANCO ---

                            // 1. Mantém o ID original
                            novaVistoria.Id = vistoriaDb.Id;

                            // 2. AQUI ESTÁ A PROTEÇÃO:
                            // Ignoramos o 'statusSugerido' e forçamos o status que já está no banco.
                            // Assim, a sincronização nunca altera o andamento da vistoria.
                            novaVistoria.Status = vistoriaDb.Status;

                            // 3. Mantém as notas do usuário
                            novaVistoria.Notas = vistoriaDb.Notas;

                            // 4. Só atualizamos se houver mudança em dados periféricos (Terminal, Previsão, etc)
                            // ou se a parametrização mudou (o que pode ser importante).
                            if (vistoriaDb.Terminal != novaVistoria.Terminal ||
                                vistoriaDb.Previsao != novaVistoria.Previsao ||
                                vistoriaDb.ParametrizacaoLPCO != novaVistoria.ParametrizacaoLPCO ||
                                vistoriaDb.DataRegistroLPCO != novaVistoria.DataRegistroLPCO)
                            {
                                var filter = Builders<Vistoria>.Filter.Eq(x => x.Id, vistoriaDb.Id);
                                bulkOps.Add(new ReplaceOneModel<Vistoria>(filter, novaVistoria));
                                listaLog.Add($"Atualizado (Dados): {lpcoInfo.LPCO}");
                            }
                        }
                        else
                        {
                            // --- CENÁRIO: NOVO REGISTRO ---
                            // Aqui usamos o statusSugerido (Base) definido na criação do objeto
                            bulkOps.Add(new InsertOneModel<Vistoria>(novaVistoria));
                            listaLog.Add($"Novo: {lpcoInfo.LPCO}");
                        }
                    }
                }
            }

            // 4. Identificar Vistorias para Remover (De processos ativos que não precisam mais de vistoria)
            foreach (var vistoriaDb in vistoriasDict.Values)
            {
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
        /// </summary>
        private (bool DeveTerVistoria, StatusVistoria Status) AnalisarLpco(LpcoInfo lpco)
        {
            var nomeOrgao = (lpco.NomeOrgao ?? "").ToUpperInvariant();
            var motivo = (lpco.MotivoExigencia ?? "").ToUpperInvariant();
            var parametrizacao = (lpco.ParametrizacaoLPCO ?? "").ToUpperInvariant();
            var statusLpcoNormalizado = (lpco.StatusLPCO ?? "").ToUpperInvariant();

            // 1. Regra Global: Se Deferido ou Cancelado, nunca gera vistoria
            if (motivo == "DEFERIDO" || motivo == "CANCELADA")
                return (false, StatusVistoria.AguardandoChegadaParaAgendar);

            // 2. REGRA ANVISA
            if (nomeOrgao.Contains("ANVISA"))
            {
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

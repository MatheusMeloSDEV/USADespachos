using CLUSA.Models;
using CLUSA.Repositories;
using MongoDB.Bson;
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

            var processosAtivos = await _repoProcesso.ListarProcessosAtivosParaStatusAsync();
            var vistoriasDb = await _repoVistorias.GetTodasAsVistoriasDoBancoAsync();

            var bulkOps = new List<WriteModel<Vistoria>>();
            var lpcosValidosNestaRodada = new HashSet<string>();

            foreach (var proc in processosAtivos)
            {
                if (proc.LI == null) continue;

                // --- LÓGICA DE CÁLCULO DE PROGRESSO DAS LIS ---
                // 1. Conta o total de LIs reais (ignorando vazias ou "Nova LI")
                int totalLIs = proc.LI.Count(li => !string.IsNullOrWhiteSpace(li.Numero) && li.Numero != "Nova LI");

                // 2. Conta quantas já estão totalmente deferidas (olhando para os LPCOs)
                int lisDeferidas = 0;
                foreach (var liParaContar in proc.LI)
                {
                    if (liParaContar.LPCO != null && liParaContar.LPCO.Any())
                    {
                        // Verifica se TODOS os LPCOs dessa LI estão Deferidos ou Cancelados
                        bool todosDeferidos = liParaContar.LPCO.All(lpco =>
                            (lpco.MotivoExigencia ?? "").Trim().ToUpper() == "DEFERIDO" ||
                            (lpco.MotivoExigencia ?? "").Trim().ToUpper() == "CANCELADA");

                        if (todosDeferidos) lisDeferidas++;
                    }
                }

                // 3. Monta a string no formato "2/5"
                string progressoCalculado = totalLIs > 0 ? $"{lisDeferidas}/{totalLIs}" : "0/0";
                // ----------------------------------------------

                foreach (var li in proc.LI)
                {
                    if (li.LPCO == null) continue;

                    foreach (var item in li.LPCO)
                    {
                        if (string.IsNullOrWhiteSpace(item.LPCO)) continue;
                        string lpcoLimpo = item.LPCO.Trim();

                        if (ShouldCreateVistoria(item, out var statusInicial))
                        {
                            lpcosValidosNestaRodada.Add(lpcoLimpo);

                            var vistoriaNova = new Vistoria
                            {
                                Ref_USA = proc.Ref_USA,
                                LPCO = lpcoLimpo,
                                LI = li.Numero,
                                ParametrizacaoLPCO = item.ParametrizacaoLPCO,
                                DataRegistroLPCO = item.DataRegistroLPCO,
                                Importador = proc.Importador,
                                Container = proc.Container,
                                Previsao = proc.DataDeAtracacao,
                                Terminal = proc.Terminal,
                                Status = statusInicial,
                                Produto = proc.Produto,
                                Conhecimento = proc.Conhecimento,

                                // Joga o texto calculado na propriedade nova!
                                ProgressoLIs = progressoCalculado
                            };

                            var existente = vistoriasDb.FirstOrDefault(v => v.LPCO == lpcoLimpo);
                            if (existente != null)
                            {
                                bool mudouParam = (existente.ParametrizacaoLPCO ?? "") != (vistoriaNova.ParametrizacaoLPCO ?? "");

                                if (!mudouParam && existente.Status != StatusVistoria.ProcessoDadoEntrada)
                                {
                                    vistoriaNova.Status = existente.Status;
                                }

                                vistoriaNova.Id = existente.Id;
                                vistoriaNova.Notas = existente.Notas;

                                // Adicionamos a comparação do ProgressoLIs para garantir atualização na tela
                                if (mudouParam || existente.Status != vistoriaNova.Status ||
                                    existente.Previsao != vistoriaNova.Previsao || existente.ProgressoLIs != vistoriaNova.ProgressoLIs)
                                {
                                    var filter = Builders<Vistoria>.Filter.Eq(x => x.Id, existente.Id);
                                    bulkOps.Add(new ReplaceOneModel<Vistoria>(filter, vistoriaNova));
                                    listaLog.Add($"Atualizado: {lpcoLimpo}");
                                }
                            }
                            else
                            {
                                vistoriaNova.Id = ObjectId.GenerateNewId();
                                bulkOps.Add(new InsertOneModel<Vistoria>(vistoriaNova));
                                listaLog.Add($"Novo: {lpcoLimpo}");
                            }
                        }
                    }
                }
            }

            foreach (var v in vistoriasDb)
            {
                if (!lpcosValidosNestaRodada.Contains(v.LPCO.Trim()))
                {
                    var filter = Builders<Vistoria>.Filter.Eq(x => x.Id, v.Id);
                    bulkOps.Add(new DeleteOneModel<Vistoria>(filter));
                    listaLog.Add($"Removido (Limpeza): {v.LPCO}");
                }
            }

            if (bulkOps.Any())
            {
                await _repoVistorias.ExecutarBulkAsync(bulkOps);
            }

            return listaLog;
        }

        private bool ShouldCreateVistoria(LpcoInfo lpco, out StatusVistoria status)
        {
            // Status Padrão (Fallback)
            status = StatusVistoria.AguardandoChegadaParaAgendar;

            var motivo = (lpco.MotivoExigencia ?? "").Trim().ToUpper();
            var stLpco = (lpco.StatusLPCO ?? "").Trim().ToUpper();
            var param = (lpco.ParametrizacaoLPCO ?? "").Trim().ToUpper();
            var orgao = (lpco.NomeOrgao ?? "").Trim().ToUpper();

            // 1. A GUILHOTINA: Se deferido/cancelado, NÃO cria (retorna false na hora)
            if (motivo == "DEFERIDO" || motivo == "CANCELADA")
            {
                return false;
            }

            // 2. REGRA DOCUMENTAL
            if (param == "DOCUMENTAL")
            {
                status = StatusVistoria.ProcessoDadoEntrada;
                return true;
            }

            // 3. REGRA DE VISTORIA FÍSICA (Usando as parametrizações exatas)
            if (param == "EXAME FÍSICO" || param == "EXAME FISICO" ||
                param == "CONFERÊNCIA FÍSICA" || param == "CONFERENCIA FISICA" ||
                param == "COLETA DE AMOSTRA" ||
                param == "INSPEÇÃO FÍSICA" || param == "INSPECAO FISICA")
            {
                status = StatusVistoria.AguardandoChegadaParaAgendar;
                return true;
            }

            // 4. REGRA AGUARDANDO PARAMETRIZAÇÃO (MAPA e ANVISA)
            if (stLpco == "ENTRADA CONCLUÍDA" && string.IsNullOrEmpty(param))
            {
                if (orgao.Contains("MAPA") || orgao.Contains("ANVISA"))
                {
                    status = StatusVistoria.ProcessoDadoEntrada;
                    return true;
                }
            }

            // Se não se encaixou em nenhuma regra válida de criação, ignora/remove.
            return false;
        }
    }
}

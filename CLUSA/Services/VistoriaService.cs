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

            // 1. Carrega dados
            var processosAtivos = await _repoProcesso.ListarProcessosAtivosParaStatusAsync();
            var dictProcessos = processosAtivos
                .Where(p => !string.IsNullOrEmpty(p.Ref_USA))
                .GroupBy(p => p.Ref_USA)
                .ToDictionary(g => g.Key, g => g.First());

            var lisAtivas = await _repoOrgaoAnuente.GetByListaRefUsaAsync(dictProcessos.Keys.ToList());

            // CARREGA TUDO DO BANCO (Fundamental para limpar Zumbis)
            var vistoriasDb = await _repoVistorias.GetTodasAsVistoriasDoBancoAsync();

            // 2. Prepara Bulk Operations
            var bulkOps = new List<WriteModel<Vistoria>>();
            var lpcosValidosNestaRodada = new HashSet<string>();

            // 3. Processa LIs Ativas (Criação e Atualização)
            foreach (var orgao in lisAtivas)
            {
                if (orgao.LPCO == null) continue;
                dictProcessos.TryGetValue(orgao.Ref_USA, out var proc);

                foreach (var item in orgao.LPCO)
                {
                    if (string.IsNullOrWhiteSpace(item.LPCO)) continue;
                    string lpcoLimpo = item.LPCO.Trim();

                    // Aplica as mesmas regras do RepositorioProcesso
                    if (ShouldCreateVistoria(item, out var statusInicial))
                    {
                        lpcosValidosNestaRodada.Add(lpcoLimpo); // Marca como válido

                        var vistoriaNova = new Vistoria
                        {
                            Ref_USA = orgao.Ref_USA,
                            LPCO = lpcoLimpo,
                            LI = orgao.Numero,
                            ParametrizacaoLPCO = item.ParametrizacaoLPCO,
                            DataRegistroLPCO = item.DataRegistroLPCO,
                            Importador = orgao.Importador,
                            Container = orgao.Container,
                            Previsao = proc?.DataDeAtracacao,
                            Terminal = proc?.Terminal,
                            Status = statusInicial,
                            // Preenche o resto...
                            Produto = orgao.Produto,
                            Conhecimento = orgao.Conhecimento
                        };

                        var existente = vistoriasDb.FirstOrDefault(v => v.LPCO == lpcoLimpo);
                        if (existente != null)
                        {
                            // ATUALIZA
                            bool mudouParam = (existente.ParametrizacaoLPCO ?? "") != (vistoriaNova.ParametrizacaoLPCO ?? "");

                            // Se mudou parametrização, reseta status. Se não, mantém.
                            if (!mudouParam && existente.Status != StatusVistoria.ProcessoDadoEntrada)
                            {
                                vistoriaNova.Status = existente.Status;
                            }

                            vistoriaNova.Id = existente.Id;
                            vistoriaNova.Notas = existente.Notas;

                            // Compara para evitar update desnecessário
                            if (mudouParam || existente.Status != vistoriaNova.Status || existente.Previsao != vistoriaNova.Previsao)
                            {
                                var filter = Builders<Vistoria>.Filter.Eq(x => x.Id, existente.Id);
                                bulkOps.Add(new ReplaceOneModel<Vistoria>(filter, vistoriaNova));
                                listaLog.Add($"Atualizado: {lpcoLimpo}");
                            }
                        }
                        else
                        {
                            // INSERE
                            vistoriaNova.Id = ObjectId.GenerateNewId();
                            bulkOps.Add(new InsertOneModel<Vistoria>(vistoriaNova));
                            listaLog.Add($"Novo: {lpcoLimpo}");
                        }
                    }
                }
            }

            // 4. GARBAGE COLLECTOR (Remove Zumbis e Deferidos)
            // Se está no banco, mas não está na lista de 'lpcosValidosNestaRodada', é LIXO.
            // (Isso inclui processos finalizados e LPCOs que viraram Deferido)
            foreach (var v in vistoriasDb)
            {
                if (!lpcosValidosNestaRodada.Contains(v.LPCO.Trim()))
                {
                    var filter = Builders<Vistoria>.Filter.Eq(x => x.Id, v.Id);
                    bulkOps.Add(new DeleteOneModel<Vistoria>(filter));
                    listaLog.Add($"Removido (Limpeza): {v.LPCO}");
                }
            }

            // 5. Executa
            if (bulkOps.Any())
            {
                await _repoVistorias.ExecutarBulkAsync(bulkOps);
            }

            return listaLog;
        }

        private bool ShouldCreateVistoria(LpcoInfo lpco, out StatusVistoria status)
        {
            status = StatusVistoria.AguardandoChegadaParaAgendar;
            var motivo = (lpco.MotivoExigencia ?? "").ToUpper();
            var stLpco = (lpco.StatusLPCO ?? "").ToUpper();
            var param = (lpco.ParametrizacaoLPCO ?? "").ToUpper();
            var orgao = (lpco.NomeOrgao ?? "").ToUpper();

            // Regra 1: Se deferido, NÃO cria (retorna false)
            if (motivo == "DEFERIDO" || motivo == "CANCELADA" || stLpco == "DEFERIDO" || stLpco == "CANCELADO")
                return false;

            // Regra 2: MAPA
            if (orgao.Contains("MAPA"))
            {
                if (param.Contains("FÍSICA") || param.Contains("FISICA") || param.Contains("COLETA") || param.Contains("EXAME"))
                {
                    status = StatusVistoria.AguardandoChegadaParaAgendar;
                    return true;
                }
                if (string.IsNullOrEmpty(param) && stLpco == "ENTRADA CONCLUÍDA")
                {
                    status = StatusVistoria.ProcessoDadoEntrada;
                    return true;
                }
            }

            // Regra 3: ANVISA
            if (orgao.Contains("ANVISA") && stLpco == "ENTRADA CONCLUÍDA" && (string.IsNullOrEmpty(param) || param == "DOCUMENTAL"))
            {
                status = StatusVistoria.ProcessoDadoEntrada;
                return true;
            }

            return false;
        }
    }
}

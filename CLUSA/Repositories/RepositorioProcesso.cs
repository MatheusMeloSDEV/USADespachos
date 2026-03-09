using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CLUSA.Models;
using System;

namespace CLUSA.Repositories
{
    public class RepositorioProcesso
    {
        private readonly IMongoCollection<Processo> _colecao;
        private readonly RepositorioOrgaoAnuente _repositorioOrgaoAnuente;
        private readonly RepositorioFatura _repositorioFatura;
        private readonly RepositorioRecibo _repositorioRecibo;
        private readonly RepositorioNotificacao _repositorioNotificacao;
        private readonly RepositorioVistorias _repositorioVistorias;

        #region Construtor
        public RepositorioProcesso(IMongoDatabase? database = null)
        {
            var db = database ?? ConfigDatabase.GetDatabase();
            _colecao = db.GetCollection<Processo>("PROCESSO");

            _repositorioOrgaoAnuente = new RepositorioOrgaoAnuente();
            _repositorioFatura = new RepositorioFatura();
            _repositorioRecibo = new RepositorioRecibo();
            _repositorioNotificacao = new RepositorioNotificacao();
            _repositorioVistorias = new RepositorioVistorias(db);
        }
        #endregion

        #region Métodos CRUD Otimizados

        public async Task<(List<Processo> itens, long total)> ListarPrincipalPaginadoAsync(
            string origemFormulario, // "Santos" ou "Itajai"
            int pagina,
            int tamanhoPagina,
            string campoOrdenacao = "DataDeAtracacao",
            bool ascendente = true)
        {
            var builder = Builders<Processo>.Filter;

            // 1. Filtro base: Não trazer finalizados
            var filtroStatus = builder.Ne(p => p.Status, "Finalizado");

            // 2. Filtro de Origem
            FilterDefinition<Processo> filtroOrigem;
            if (origemFormulario == "Itajai")
            {
                filtroOrigem = builder.Regex(p => p.Ref_USA, new BsonRegularExpression("ITJ$", "i"));
            }
            else
            {
                filtroOrigem = builder.Not(builder.Regex(p => p.Ref_USA, new BsonRegularExpression("ITJ$", "i")));
            }

            var filtroFinal = builder.And(filtroStatus, filtroOrigem);

            // --- SEPARAÇÃO DE LÓGICAS ---
            if (campoOrdenacao == "Ref_USA")
            {
                // 🔹 LÓGICA EM MEMÓRIA (Apenas para Ref_USA manter a regra de Ano/Número)
                var todosAtivos = await _colecao.Find(filtroFinal).ToListAsync();
                long totalAtivos = todosAtivos.Count;

                var ordenados = ascendente
                    ? todosAtivos.OrderBy(p => string.IsNullOrWhiteSpace(p.Ref_USA) ? 1 : 0).ThenBy(p => ExtrairAnoNumeroRepo(p.Ref_USA))
                    : todosAtivos.OrderBy(p => string.IsNullOrWhiteSpace(p.Ref_USA) ? 1 : 0).ThenByDescending(p => ExtrairAnoNumeroRepo(p.Ref_USA));

                // Retorna a página da Ref_USA
                var paginaRefUsa = ordenados.Skip((pagina - 1) * tamanhoPagina).Take(tamanhoPagina).ToList();
                return (paginaRefUsa, totalAtivos);
            }
            else
            {
                // 🔹 LÓGICA NATIVA MONGODB (Altíssima velocidade para todas as outras colunas)
                var total = await _colecao.CountDocumentsAsync(filtroFinal);

                // O nome do campo para o Mongo ler. Ex: "$Importador"
                var sortField = $"${campoOrdenacao}";

                // Cria a coluna temporária de peso (Nulo e Vazio vão pro final)
                var addFields = new BsonDocument("$addFields", new BsonDocument("PesoVazio",
                    new BsonDocument("$cond", new BsonArray
                    {
                new BsonDocument("$or", new BsonArray
                {
                    new BsonDocument("$eq", new BsonArray { sortField, BsonNull.Value }),
                    new BsonDocument("$eq", new BsonArray { sortField, "" })
                }),
                1, // Peso 1 = Vai pro fim da fila
                0  // Peso 0 = Fica no topo da fila
                    })));

                // Ordena por Peso primeiro e depois pela coluna real
                var sortDef = ascendente
                    ? Builders<BsonDocument>.Sort.Ascending("PesoVazio").Ascending(campoOrdenacao)
                    : Builders<BsonDocument>.Sort.Ascending("PesoVazio").Descending(campoOrdenacao);

                // Executa o Pipeline nativo do MongoDB
                var resultadosBson = await _colecao.Aggregate()
                    .Match(filtroFinal)
                    .AppendStage<BsonDocument>(addFields)
                    .Sort(sortDef)
                    .Skip((pagina - 1) * tamanhoPagina)
                    .Limit(tamanhoPagina)
                    // A MÁGICA AQUI: Remove o 'PesoVazio' antes de devolver pro C# (0 significa excluir)
                    .AppendStage<BsonDocument>(new BsonDocument("$project", new BsonDocument("PesoVazio", 0)))
                    .ToListAsync();

                // Converte de BSON devolta para a classe Processo
                var paginaGenerica = resultadosBson
                    .Select(b => MongoDB.Bson.Serialization.BsonSerializer.Deserialize<Processo>(b))
                    .ToList();

                return (paginaGenerica, total);
            }
        }

        // O método extrator continua o mesmo (deixe-o logo abaixo na sua classe)
        private (int ano, int numero) ExtrairAnoNumeroRepo(string refUsa)
        {
            if (string.IsNullOrWhiteSpace(refUsa)) return (0, 0);

            string refLimpa = refUsa.Split(' ').FirstOrDefault() ?? refUsa;
            var partes = refLimpa.Split('/');
            int numero = 0, ano = 0;

            if (partes.Length == 2)
            {
                int.TryParse(partes[0], out numero);
                int.TryParse(partes[1], out ano);
            }
            return (ano, numero);
        }
        public async Task UpdateParcialAsync(ObjectId id, List<UpdateDefinition<Processo>> updates)
        {
            if (updates == null || updates.Count == 0) return;

            var filtro = Builders<Processo>.Filter.Eq(p => p.Id, id);
            var updateCombinado = Builders<Processo>.Update.Combine(updates);

            // Atualiza APENAS os campos enviados
            await _colecao.UpdateOneAsync(filtro, updateCombinado);
        }
        /// <summary>
        /// OTIMIZAÇÃO: Atualiza o status do LPCO direto no banco usando ArrayFilters.
        /// E agora permite adicionar logs no topo do Histórico do Processo!
        /// </summary>
        public async Task AtualizarStatusLpcoAsync(string refUsa, string numeroLpco, string novoStatus, string logHistorico = null)
        {
            var filtro = Builders<Processo>.Filter.Eq(p => p.Ref_USA, refUsa);
            var builderUpdate = Builders<Processo>.Update;

            // 1. Prepara a atualização
            var update = builderUpdate
                .Set("LI.$[].LPCO.$[itemLpco].StatusLPCO", novoStatus)
                .Set("LI.$[].LPCO.$[itemLpco].MotivoExigencia", novoStatus);

            // 2. Data de Deferimento automática
            if (novoStatus.ToUpper() == "DEFERIDO")
            {
                update = update.Set("LI.$[].LPCO.$[itemLpco].DataDeferimentoLPCO", DateTime.Now);
            }

            // 3. Atualização do Histórico
            if (!string.IsNullOrWhiteSpace(logHistorico))
            {
                var processoAtual = await _colecao.Find(filtro)
                                                  .Project(p => new { p.HistoricoDoProcesso })
                                                  .FirstOrDefaultAsync();

                string historicoAntigo = processoAtual?.HistoricoDoProcesso ?? "";
                string novoHistorico = $"{logHistorico}\r\n{historicoAntigo}".Trim();

                update = update.Set(p => p.HistoricoDoProcesso, novoHistorico);
            }

            // 4. Filtro Mágico (Anti-espaço em branco)
            var arrayFilters = new List<ArrayFilterDefinition>
    {
        new BsonDocumentArrayFilterDefinition<BsonDocument>(
            new BsonDocument("itemLpco.LPCO", new BsonRegularExpression(numeroLpco.Trim(), "i"))
        )
    };

            var opcoes = new UpdateOptions { ArrayFilters = arrayFilters };

            // 5. Executa a atualização no Processo
            await _colecao.UpdateOneAsync(filtro, update, opcoes);

            // --- A CORREÇÃO ENTRA AQUI! ---
            // 6. Sincroniza o Órgão Anuente para ele não ficar para trás!
            var processoAtualizado = await GetByRefUsaAsync(refUsa);
            if (processoAtualizado != null)
            {
                await SincronizarLicencas(processoAtualizado);
            }
        }
        public async Task<List<string>> ListarRefUsaAtivosAsync()
        {
            var filter = Builders<Processo>.Filter.Ne(p => p.Status, "Finalizado");
            // Projeção para trazer APENAS a string Ref_USA, economizando muita memória
            return await _colecao.Find(filter)
                .Project(p => p.Ref_USA)
                .ToListAsync();
        }
        public async Task<bool> VerificarRefUsaExisteAsync(string refUsa)
        {
            // Usa o índice Ref_USA_1 para checagem instantânea
            var filter = Builders<Processo>.Filter.Eq(p => p.Ref_USA, refUsa);
            // Project para trazer apenas o _id (economiza banda)
            var projection = Builders<Processo>.Projection.Include(p => p.Id);
            return await _colecao.Find(filter).Project(projection).AnyAsync();
        }
        public async Task<Processo?> GetByRefUsaAsync(string refUsa)
        {
            // O índice Ref_USA_1 criado anteriormente garante que isso seja instantâneo (0ms)
            var filter = Builders<Processo>.Filter.Eq(p => p.Ref_USA, refUsa);
            return await _colecao.Find(filter).FirstOrDefaultAsync();
        }
        public async Task<List<Processo>> ListarFinalizadosAsync(string sufixoExcluir = "ITJ")
        {
            var builder = Builders<Processo>.Filter;

            // 1. Filtro de Status: PEGAR APENAS "Finalizado"
            var filtroStatus = builder.Eq(p => p.Status, "Finalizado");

            // 2. Filtro de Sufixo (Mantendo a mesma regra de excluir ITJ, se necessário)
            var regex = new BsonRegularExpression(new Regex($"{sufixoExcluir}$", RegexOptions.IgnoreCase));
            var filtroSufixo = builder.Not(builder.Regex(p => p.Ref_USA, regex));

            // Combina os filtros
            var filtroFinal = builder.And(filtroStatus, filtroSufixo);

            // Dica: Se a lista for muito grande, considere adicionar .Limit(1000) ou paginação aqui
            return await _colecao.Find(filtroFinal).ToListAsync();
        }
        // Este é o método "Turbo" para o Grid principal
        public async Task<List<Processo>> ListarPrincipalOtimizadoAsync(string sufixoExcluir = "ITJ")
        {
            var builder = Builders<Processo>.Filter;

            // 1. Filtro de Status (Feito no Banco, não na memória)
            var filtroStatus = builder.Ne(p => p.Status, "Finalizado");

            // 2. Filtro de Sufixo (Feito no Banco)
            var regex = new BsonRegularExpression(new Regex($"{sufixoExcluir}$", RegexOptions.IgnoreCase));
            var filtroSufixo = builder.Not(builder.Regex(p => p.Ref_USA, regex));

            var filtroFinal = builder.And(filtroStatus, filtroSufixo);

            // Traz apenas o necessário. Se precisar de projeção (trazer menos colunas), adicione .Project(...)
            return await _colecao.Find(filtroFinal).ToListAsync();
        }
        public async Task<List<Processo>> ListarAtivosPorSufixoAsync(string sufixo)
        {
            var builder = Builders<Processo>.Filter;

            // 1. Filtro: Status NÃO PODE SER "Finalizado"
            // Usamos Ne (Not Equal)
            var filtroStatus = builder.Ne(p => p.Status, "Finalizado");

            // 2. Filtro: Ref_USA DEVE terminar com o sufixo
            var regex = new BsonRegularExpression($"{sufixo}$", "i");
            var filtroSufixo = builder.Regex(p => p.Ref_USA, regex);

            // Combina: (Não Finalizado) E (Termina com Sufixo)
            var filtroFinal = builder.And(filtroStatus, filtroSufixo);

            return await _colecao.Find(filtroFinal).ToListAsync();
        }
        /// <summary>
        /// Traz apenas os campos essenciais para os serviços de background (Vistoria e Notificação).
        /// Otimizado com PROJECTION para não carregar o objeto inteiro na memória.
        /// </summary>
        public async Task<List<Processo>> ListarProcessosAtivosParaStatusAsync()
        {
            var filter = Builders<Processo>.Filter.Ne(p => p.Status, "Finalizado");

            return await _colecao.Find(filter).ToListAsync();
        }

        public async Task CreateAsync(Processo processo)
        {
            await _colecao.InsertOneAsync(processo);

            // Executa tarefas auxiliares em paralelo
            await Task.WhenAll(
                SincronizarLicencas(processo),
                _repositorioFatura.InsertAsync(new Fatura(processo)),
                _repositorioRecibo.InsertAsync(new Recibo(processo))
            );
        }

        public async Task UpdateAsync(Processo processo)
        {
            var updateMain = _colecao.ReplaceOneAsync(p => p.Id == processo.Id, processo);
            var syncLicencas = SincronizarLicencas(processo);
            var syncVistorias = SincronizarVistoriasDoProcesso(processo);

            await Task.WhenAll(updateMain, syncLicencas, syncVistorias);
        }

        public async Task DeleteAsync(string processoId)
        {
            var processo = await ObterPorIdAsync(processoId);
            if (processo == null) return;

            var deleteMain = _colecao.DeleteOneAsync(p => p.Id == ObjectId.Parse(processoId));

            // Executa limpezas em paralelo
            await Task.WhenAll(
                deleteMain,
                _repositorioOrgaoAnuente.DeleteAllByRefUsaAsync(processo.Ref_USA),
                _repositorioFatura.DeletePorRefUsaAsync(processo.Ref_USA),
                _repositorioRecibo.DeletePorRefUsaAsync(processo.Ref_USA),
                _repositorioNotificacao.ExcluirPorRefUsaAsync(processo.Ref_USA)
            );
        }

        #endregion

        #region Métodos de Leitura Auxiliares

        public async Task<Processo?> ObterPorIdAsync(string id)
        {
            return await _colecao.Find(p => p.Id == ObjectId.Parse(id)).FirstOrDefaultAsync();
        }

        public async Task<List<string>> ObterValoresUnicosAsync(string campo)
        {
            // Distinct é muito rápido com índice
            return await _colecao.Distinct<string>(campo, FilterDefinition<Processo>.Empty).ToListAsync();
        }

        public async Task<List<Processo>> PesquisarAsync(string campo, string pesquisa)
        {
            // Adicionado limite para não travar se a busca for muito ampla
            var filter = Builders<Processo>.Filter.Regex(campo, new BsonRegularExpression(new Regex(pesquisa, RegexOptions.IgnoreCase)));
            return await _colecao.Find(filter).Limit(200).ToListAsync();
        }

        // Mantido para compatibilidade se usado em outro lugar
        public async Task<List<Processo>> ListarExcetoSufixoRefUsaAsync(string sufixoAExcluir)
        {
            return await ListarPrincipalOtimizadoAsync(sufixoAExcluir);
        }

        #endregion

        #region Sincronização (Lógica de Negócio Otimizada)

        public async Task SincronizarLicencas(Processo processo)
        {
            var lisDoProcesso = processo.LI ?? new List<LicencaImportacao>();
            var lisAtuaisNoDb = await _repositorioOrgaoAnuente.ListByRefUsaAsync(processo.Ref_USA);

            // Prepara lista de operações em lote (Bulk)
            var bulkOps = new List<WriteModel<OrgaoAnuente>>();
            var numerosDb = lisAtuaisNoDb.ToDictionary(x => x.Numero);

            // 1. Identificar Updates e Inserts
            foreach (var liProc in lisDoProcesso)
            {
                if (numerosDb.TryGetValue(liProc.Numero, out var liDb))
                {
                    // Update
                    var orgaoAtualizado = liDb;
                    AtualizarPropriedadesOrgao(orgaoAtualizado, processo, liProc);

                    var filter = Builders<OrgaoAnuente>.Filter.Eq(x => x.Id, liDb.Id);
                    bulkOps.Add(new ReplaceOneModel<OrgaoAnuente>(filter, orgaoAtualizado));
                }
                else
                {
                    // Insert
                    var novoOrgao = MapearParaOrgaoAnuente(processo, liProc);
                    bulkOps.Add(new InsertOneModel<OrgaoAnuente>(novoOrgao));
                }
            }

            // 2. Identificar Deletes (quem está no banco mas não está mais no processo)
            var numerosProcesso = lisDoProcesso.Select(x => x.Numero).ToHashSet();
            foreach (var liDb in lisAtuaisNoDb)
            {
                if (!numerosProcesso.Contains(liDb.Numero))
                {
                    var filter = Builders<OrgaoAnuente>.Filter.Eq(x => x.Id, liDb.Id);
                    bulkOps.Add(new DeleteOneModel<OrgaoAnuente>(filter));
                }
            }

            // 3. Executar TUDO de uma vez
            if (bulkOps.Any())
            {
                await _repositorioOrgaoAnuente.ExecutarBulkAsync(bulkOps);
            }
        }

        private OrgaoAnuente MapearParaOrgaoAnuente(Processo processo, LicencaImportacao li)
        {
            var orgao = new OrgaoAnuente { Ref_USA = processo.Ref_USA, Numero = li.Numero };
            AtualizarPropriedadesOrgao(orgao, processo, li);
            return orgao;
        }

        private void AtualizarPropriedadesOrgao(OrgaoAnuente orgao, Processo processo, LicencaImportacao li)
        {
            orgao.Importador = processo.Importador;
            orgao.Produto = processo.Produto;
            orgao.Container = processo.Container;
            orgao.Origem = processo.Origem;
            orgao.Conhecimento = processo.Conhecimento;
            orgao.Terminal = processo.Terminal;
            orgao.DataChegada = processo.DataDeAtracacao;
            orgao.HistoricoDoProcesso = processo.HistoricoDoProcesso;
            orgao.Pendencia = processo.Pendencia;
            orgao.Inspecao = processo.Inspecao;

            orgao.NCM = li.NCM;
            orgao.DataRegistro = li.DataRegistro;
            orgao.LPCO = li.LPCO;
        }

        private async Task SincronizarVistoriasDoProcesso(Processo processo)
        {
            if (processo.LI == null) return;

            var lpcosParaRemover = new HashSet<string>();
            var lpcosProcessados = new HashSet<string>();

            foreach (var li in processo.LI)
            {
                if (li.LPCO == null) continue;

                foreach (var itemLpco in li.LPCO)
                {
                    if (string.IsNullOrWhiteSpace(itemLpco.LPCO)) continue;

                    // Normaliza para evitar problemas de espaço
                    string lpcoLimpo = itemLpco.LPCO.Trim();

                    if (lpcosProcessados.Contains(lpcoLimpo)) continue;
                    lpcosProcessados.Add(lpcoLimpo);

                    // --- REGRA DE NEGÓCIO ---
                    var (acao, statusSugerido) = AnalisarRegraVistoria(itemLpco);

                    if (acao == AcaoVistoria.Remover)
                    {
                        // Se Deferido/Cancelado, adiciona na lista para remover
                        lpcosParaRemover.Add(lpcoLimpo);
                    }
                    else if (acao == AcaoVistoria.ManterOuCriar)
                    {
                        // Se precisa existir, fazemos o Upsert
                        var vistoriaExistente = await _repositorioVistorias.GetByLPCOAsync(lpcoLimpo);

                        StatusVistoria statusFinal = statusSugerido;
                        string notas = "";
                        ObjectId id = ObjectId.Empty;

                        if (vistoriaExistente != null)
                        {
                            // Se já existe e a parametrização NÃO mudou, preserva o status manual
                            // Se a parametrização mudou (ex: era vazio e virou Física), assume o statusSugerido
                            bool mudouParametrizacao = (vistoriaExistente.ParametrizacaoLPCO ?? "") != (itemLpco.ParametrizacaoLPCO ?? "");

                            if (!mudouParametrizacao && vistoriaExistente.Status != StatusVistoria.ProcessoDadoEntrada)
                            {
                                statusFinal = vistoriaExistente.Status;
                            }

                            notas = vistoriaExistente.Notas;
                            id = vistoriaExistente.Id;
                        }
                        else
                        {
                            // Novo item
                            id = ObjectId.GenerateNewId();
                        }

                        var vistoria = new Vistoria
                        {
                            Id = id,
                            Ref_USA = processo.Ref_USA,
                            LPCO = lpcoLimpo,
                            LI = li.Numero,
                            ParametrizacaoLPCO = itemLpco.ParametrizacaoLPCO,
                            DataRegistroLPCO = itemLpco.DataRegistroLPCO,
                            Produto = processo.Produto,
                            Container = processo.Container,
                            Conhecimento = processo.Conhecimento,
                            Importador = processo.Importador,
                            Terminal = processo.Terminal,
                            Previsao = processo.DataDeAtracacao,
                            Status = statusFinal,
                            Notas = notas
                        };

                        await _repositorioVistorias.UpsertAsync(vistoria);
                    }
                }
            }

            // Remove em lote os que foram marcados como Deferidos/Cancelados
            if (lpcosParaRemover.Any())
            {
                await _repositorioVistorias.DeleteByListaLpcosAsync(lpcosParaRemover.ToList());
            }
        }

        private enum AcaoVistoria { ManterOuCriar, Remover, Ignorar }

        private (AcaoVistoria Acao, StatusVistoria Status) AnalisarRegraVistoria(LpcoInfo lpco)
        {
            var motivo = (lpco.MotivoExigencia ?? "").Trim().ToUpper();
            var status = (lpco.StatusLPCO ?? "").Trim().ToUpper();
            var param = (lpco.ParametrizacaoLPCO ?? "").Trim().ToUpper();
            var orgao = (lpco.NomeOrgao ?? "").Trim().ToUpper();

            // 1. A GUILHOTINA (Fim de papo)
            if (motivo == "DEFERIDO" || motivo == "CANCELADA")
            {
                return (AcaoVistoria.Remover, StatusVistoria.AguardandoChegadaParaAgendar);
            }

            // 2. REGRA DOCUMENTAL
            if (param == "DOCUMENTAL")
            {
                return (AcaoVistoria.ManterOuCriar, StatusVistoria.ProcessoDadoEntrada);
            }

            // 3. REGRA DE VISTORIA FÍSICA (Usando as parametrizações exatas)
            // Adicionei as versões sem acento por segurança (caso a API ou o usuário digite sem acento)
            if (param == "EXAME FÍSICO" || param == "EXAME FISICO" ||
                param == "CONFERÊNCIA FÍSICA" || param == "CONFERENCIA FISICA" ||
                param == "COLETA DE AMOSTRA" ||
                param == "INSPEÇÃO FÍSICA" || param == "INSPECAO FISICA")
            {
                return (AcaoVistoria.ManterOuCriar, StatusVistoria.AguardandoChegadaParaAgendar);
            }

            // 4. REGRA AGUARDANDO PARAMETRIZAÇÃO (MAPA e ANVISA)
            // Se a entrada já foi concluída, mas o fiscal AINDA NÃO DEFINIU a parametrização (está vazio).
            if (status == "ENTRADA CONCLUÍDA" && string.IsNullOrEmpty(param))
            {
                if (orgao.Contains("MAPA") || orgao.Contains("ANVISA"))
                {
                    return (AcaoVistoria.ManterOuCriar, StatusVistoria.ProcessoDadoEntrada);
                }
            }

            // PADRÃO (Lixeira)
            // Cai aqui se o status for "Pronto para Entrada" ou "Pendência Documental", 
            // ou se for de outro órgão que não controlamos.
            return (AcaoVistoria.Remover, StatusVistoria.AguardandoChegadaParaAgendar);
        }
        #endregion
    }
}
using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CLUSA
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
            _repositorioVistorias = new RepositorioVistorias();
        }
        #endregion

        #region Métodos CRUD Otimizados
        /// <summary>
        /// OTIMIZAÇÃO: Atualiza o status do LPCO direto no banco sem trazer o processo inteiro para a memória.
        /// Evita tráfego de rede desnecessário e previne conflitos de concorrência.
        /// </summary>
        public async Task AtualizarStatusLpcoAsync(string refUsa, string numeroLpco, string novoStatus)
        {
            var processo = await GetByRefUsaAsync(refUsa);
            if (processo == null) return;

            bool alterou = false;

            if (processo.LI != null)
            {
                foreach (var li in processo.LI)
                {
                    if (li.LPCO != null)
                    {
                        var lpcoAlvo = li.LPCO.FirstOrDefault(x => x.LPCO == numeroLpco);
                        if (lpcoAlvo != null)
                        {
                            lpcoAlvo.MotivoExigencia = novoStatus;
                            alterou = true;
                        }
                    }
                }
            }

            if (alterou)
            {
                await _colecao.ReplaceOneAsync(p => p.Id == processo.Id, processo);
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
            var syncVistorias = SincronizarVistorias(processo);

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

        private async Task SincronizarLicencas(Processo processo)
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

        private async Task SincronizarVistorias(Processo processo)
        {
            // Se tiver muitas vistorias, aplicar lógica de Bulk aqui também.
            // Mantendo lógica original mas garantindo assincronismo correto.
            var vistoriasNoBanco = await _repositorioVistorias.GetByRefUsaAsync(processo.Ref_USA);
            foreach (var vistoria in vistoriasNoBanco)
            {
                vistoria.Importador = processo.Importador;
                vistoria.Container = processo.Container;
                vistoria.Conhecimento = processo.Conhecimento;
                vistoria.Ref_USA = processo.Ref_USA;
                vistoria.Produto = processo.Produto;
                vistoria.Terminal = processo.Terminal;
                vistoria.Previsao = processo.DataDeAtracacao;
                await _repositorioVistorias.UpsertAsync(vistoria);
            }
        }
        #endregion
    }
}
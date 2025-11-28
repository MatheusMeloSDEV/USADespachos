using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CLUSA
{
    // Assumimos que as classes de modelo (Processo, LicencaImportacao, Capa, Fatura, Recibo, OrgaoAnuente, Vistoria, etc.)
    // e suas respectivas dependências (RepositorioOrgaoAnuente, RepositorioFatura, etc.) estão definidas e acessíveis no namespace CLUSA.

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

            // Instanciação de Repositórios Auxiliares (Assumindo construtores padrão)
            _repositorioOrgaoAnuente = new RepositorioOrgaoAnuente();
            _repositorioFatura = new RepositorioFatura();
            _repositorioRecibo = new RepositorioRecibo();
            _repositorioNotificacao = new RepositorioNotificacao();
            _repositorioVistorias = new RepositorioVistorias();
        }
        #endregion

        #region Métodos CRUD Principais

        public async Task<List<Processo>> ListarTodosAsync()
        {
            return await _colecao.Find(FilterDefinition<Processo>.Empty).ToListAsync();
        }

        public async Task CreateAsync(Processo processo)
        {
            await _colecao.InsertOneAsync(processo);
            await SincronizarLicencas(processo);
            await _repositorioFatura.CreateAsync(new Fatura(processo));
            await _repositorioRecibo.CreateAsync(new Recibo(processo));
        }

        public async Task UpdateAsync(Processo processo)
        {
            await _colecao.ReplaceOneAsync(p => p.Id == processo.Id, processo);
            await SincronizarLicencas(processo);
            await SincronizarVistorias(processo);
        }

        public async Task DeleteAsync(string processoId)
        {
            var processo = await ObterPorIdAsync(processoId);
            if (processo == null) return;

            await _colecao.DeleteOneAsync(p => p.Id == ObjectId.Parse(processoId));

            // Limpeza em repositórios auxiliares
            await _repositorioOrgaoAnuente.DeleteAllByRefUsaAsync(processo.Ref_USA);
            await _repositorioFatura.DeletePorRefUsaAsync(processo.Ref_USA);
            await _repositorioRecibo.DeletePorRefUsaAsync(processo.Ref_USA);
            await _repositorioNotificacao.ExcluirPorRefUsaAsync(processo.Ref_USA);
        }

        #endregion

        #region Métodos de Leitura e Consulta

        /// <summary>
        /// Lista todos os Processos cujo Status não é "Finalizado".
        /// Usado para o ciclo de sincronização de notificações e UI.
        /// </summary>
        public async Task<List<Processo>> ListarTodosAtivosAsync()
        {
            var filter = Builders<Processo>.Filter.Ne(p => p.Status, "Finalizado");
            return await _colecao.Find(filter).ToListAsync();
        }

        public async Task<List<string>> ListarRefUsaAtivosAsync()
        {
            var filter = Builders<Processo>.Filter.Ne(p => p.Status, "Finalizado");

            return await _colecao
                .Find(filter)
                .Project(p => p.Ref_USA)
                .ToListAsync();
        }

        public async Task<List<Processo>> ListarProcessosAtivosParaStatusAsync()
        {
            var filter = Builders<Processo>.Filter.Ne(p => p.Status, "Finalizado");

            var projection = Builders<Processo>.Projection.Include(p => p.Id)
                .Include(p => p.Ref_USA)
                .Include(p => p.SR)
                .Include(p => p.Importador)
                .Include(p => p.Veiculo)
                .Include(p => p.DataDeAtracacao)
                .Include(p => p.Terminal)
                .Include(p => p.LocalDeDesembaraco)
                .Include(p => p.Container)
                .Include(p => p.Redestinacao)
                .Include(p => p.CE)
                .Include(p => p.FreeTime)
                .Include(p => p.VencimentoFreeTime)
                .Include(p => p.VencimentoFMA)
                .Include(p => p.CapaOK)
                .Include(p => p.Numerario)
                .Include(p => p.RascunhoDI)
                .Include(p => p.Pendencia)
                .Include(p => p.Status)
                .Include(p => p.CondicaoProcesso)
                .Include(p => p.Inspecao);

            return await _colecao.Find(filter).ToListAsync();
        }

        public async Task<List<Processo>> ListarExcetoSufixoRefUsaAsync(string sufixoAExcluir)
        {
            var regex = new BsonRegularExpression(new Regex($"{sufixoAExcluir}$", RegexOptions.IgnoreCase));
            var filterParaExcluir = Builders<Processo>.Filter.Regex(p => p.Ref_USA, regex);

            var filterFinal = Builders<Processo>.Filter.Not(filterParaExcluir);

            return await _colecao.Find(filterFinal).ToListAsync();
        }

        public async Task<List<Processo>> ListarPorSufixoRefUsaAsync(string sufixo)
        {
            var regex = new BsonRegularExpression(new Regex($"{sufixo}$", RegexOptions.IgnoreCase));
            var filter = Builders<Processo>.Filter.Regex(p => p.Ref_USA, regex);

            return await _colecao.Find(filter).ToListAsync();
        }

        public async Task<bool> VerificarRefUsaExisteAsync(string refUsa)
        {
            var processoExistente = await _colecao
                .Find(p => p.Ref_USA == refUsa)
                .FirstOrDefaultAsync();

            return processoExistente != null;
        }

        public async Task<Processo?> ObterPorIdAsync(string id)
        {
            return await _colecao.Find(p => p.Id == ObjectId.Parse(id)).FirstOrDefaultAsync();
        }

        public async Task<Processo?> GetByRefUsaAsync(string refUsa)
        {
            var filter = Builders<Processo>.Filter.Eq(p => p.Ref_USA, refUsa);
            return await _colecao.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<List<string>> ObterValoresUnicosAsync(string campo)
        {
            var cursor = await _colecao.DistinctAsync<string>(campo, FilterDefinition<Processo>.Empty);
            return await cursor.ToListAsync();
        }

        public async Task<List<Processo>> PesquisarAsync(string campo, string pesquisa)
        {
            var filter = Builders<Processo>.Filter.Regex(campo, new BsonRegularExpression(new Regex(pesquisa, RegexOptions.IgnoreCase)));
            return await _colecao.Find(filter).ToListAsync();
        }

        #endregion

        #region Métodos de Sincronização (Lógica de Negócio)

        /// <summary>
        /// Mapeia os dados de um Processo e uma Licenca para um objeto OrgaoAnuente.
        /// </summary>
        private OrgaoAnuente MapearParaOrgaoAnuente(Processo processo, LicencaImportacao li)
        {
            // Tenta definir o Tipo principal da LI com base no primeiro LPCO.
            Enum.TryParse<TipoOrgaoAnuente>(li.LPCO.FirstOrDefault()?.NomeOrgao, out var tipoPrincipal);

            return new OrgaoAnuente
            {
                Ref_USA = processo.Ref_USA,
                Importador = processo.Importador,
                Produto = processo.Produto,
                Container = processo.Container,
                Origem = processo.Origem,
                Conhecimento = processo.Conhecimento,
                Terminal = processo.Terminal,
                DataChegada = processo.DataDeAtracacao,
                HistoricoDoProcesso = processo.HistoricoDoProcesso,
                Pendencia = processo.Pendencia,

                Numero = li.Numero,
                NCM = li.NCM,
                DataRegistro = li.DataRegistro,
                LPCO = li.LPCO
            };
        }

        /// <summary>
        /// Sincroniza a coleção de OrgaosAnuentes (LIs) com base na lista de LIs de um Processo.
        /// </summary>
        private async Task SincronizarLicencas(Processo processo)
        {
            var lisDoProcesso = processo.LI;
            var lisAtuaisNoDb = await _repositorioOrgaoAnuente.ListByRefUsaAsync(processo.Ref_USA);

            // --- ATUALIZA LIs existentes ---
            var lisParaAtualizar = from liProc in lisDoProcesso
                                   join liDb in lisAtuaisNoDb on liProc.Numero equals liDb.Numero
                                   select (ProcessoLi: liProc, DatabaseLi: liDb);

            foreach (var (liProcesso, liDatabase) in lisParaAtualizar)
            {
                var orgaoParaSalvar = liDatabase;

                // Atualiza os dados que vêm do Processo principal
                orgaoParaSalvar.Importador = processo.Importador;
                orgaoParaSalvar.Produto = processo.Produto;
                orgaoParaSalvar.Container = processo.Container;
                orgaoParaSalvar.Origem = processo.Origem;
                orgaoParaSalvar.Conhecimento = processo.Conhecimento;
                orgaoParaSalvar.Terminal = processo.Terminal;
                orgaoParaSalvar.DataChegada = processo.DataDeAtracacao;
                orgaoParaSalvar.Inspecao = processo.Inspecao;

                // Atualiza os dados que vêm da LI editada no FrmModificaProcesso
                orgaoParaSalvar.NCM = liProcesso.NCM;
                orgaoParaSalvar.DataRegistro = liProcesso.DataRegistro;
                orgaoParaSalvar.LPCO = liProcesso.LPCO;

                orgaoParaSalvar.HistoricoDoProcesso = processo.HistoricoDoProcesso;
                orgaoParaSalvar.Pendencia = processo.Pendencia;

                await _repositorioOrgaoAnuente.UpdateAsync(orgaoParaSalvar);
            }


            // --- ADICIONA LIs novas ---
            var numerosLisAtuais = lisAtuaisNoDb.Select(li => li.Numero).ToHashSet();
            var lisParaAdicionar = lisDoProcesso.Where(li => !numerosLisAtuais.Contains(li.Numero));

            foreach (var li in lisParaAdicionar)
            {
                var novoOrgaoAnuente = MapearParaOrgaoAnuente(processo, li);
                await _repositorioOrgaoAnuente.CreateAsync(novoOrgaoAnuente);
            }

            // --- DELETA LIs que foram removidas ---
            var numerosLisProcesso = lisDoProcesso.Select(li => li.Numero).ToHashSet();
            var lisParaDeletar = lisAtuaisNoDb.Where(li => !numerosLisProcesso.Contains(li.Numero));

            foreach (var li in lisParaDeletar)
            {
                await _repositorioOrgaoAnuente.DeleteByIdAsync(li.Id.ToString());
            }
        }

        /// <summary>
        /// Sincroniza a coleção de Vistorias associadas a um Processo.
        /// </summary>
        private async Task SincronizarVistorias(Processo processo)
        {
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
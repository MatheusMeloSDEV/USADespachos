using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.RegularExpressions;

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
        // No RepositorioProcesso, crie este método se não existir
        public async Task<List<string>> ListarRefUsaAtivosAsync()
        {
            // Filtra onde Status é diferente de "Finalizado" e projeta apenas o Ref_USA
            var filter = Builders<Processo>.Filter.Ne(p => p.Status, "Finalizado");

            return await _colecao
                .Find(filter)
                .Project(p => p.Ref_USA)
                .ToListAsync();
        }
        public async Task<List<Processo>> ListarTodosAsync()
        {
            return await _colecao.Find(FilterDefinition<Processo>.Empty).ToListAsync();
        }
        public async Task<List<Processo>> ListarProcessosAtivosParaStatusAsync()
        {
            // A. FILTRO no SERVIDOR: Não traga processos finalizados.
            var filter = Builders<Processo>.Filter.Ne(p => p.Status, "Finalizado");

            // B. PROJEÇÃO: Defina quais campos são essenciais.
            // Isso reduz o volume de dados transferidos!
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
                .Include(p => p.CondicaoProcesso) // Se você usa este campo para filtering
                .Include(p => p.Inspecao)
                // Inclua qualquer outro campo usado no ProcessoHelper.AtualizarCondicaoProcesso
                ;
            return await _colecao.Find(filter).ToListAsync();
        }
        public async Task<List<Processo>> ListarExcetoSufixoRefUsaAsync(string sufixoAExcluir)
        {
            // Cria a expressão regular para encontrar o que queremos excluir
            var regex = new BsonRegularExpression(new Regex($"{sufixoAExcluir}$", RegexOptions.IgnoreCase));
            var filterParaExcluir = Builders<Processo>.Filter.Regex(p => p.Ref_USA, regex);

            // Usa o operador .Not() para inverter a lógica, trazendo tudo MENOS o que corresponde ao filtro
            var filterFinal = Builders<Processo>.Filter.Not(filterParaExcluir);

            return await _colecao.Find(filterFinal).ToListAsync();
        }
        public async Task<List<Processo>> ListarPorSufixoRefUsaAsync(string sufixo)
        {
            // Usamos uma expressão regular para encontrar documentos que terminam com o sufixo.
            // O '$' significa "fim da string" e 'IgnoreCase' ignora maiúsculas/minúsculas.
            var regex = new BsonRegularExpression(new Regex($"{sufixo}$", RegexOptions.IgnoreCase));

            var filter = Builders<Processo>.Filter.Regex(p => p.Ref_USA, regex);

            return await _colecao.Find(filter).ToListAsync();
        }
        public async Task<bool> VerificarRefUsaExisteAsync(string refUsa)
        {
            var processoExistente = await _colecao
                .Find(p => p.Ref_USA == refUsa)
                .FirstOrDefaultAsync();

            return processoExistente != null; // Retorna true se encontrou
        }


        public async Task<Processo?> ObterPorIdAsync(string id)
        {
            return await _colecao.Find(p => p.Id == ObjectId.Parse(id)).FirstOrDefaultAsync();
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

            // CORREÇÃO: Usando o novo nome do método.
            await _repositorioOrgaoAnuente.DeleteAllByRefUsaAsync(processo.Ref_USA);
            await _repositorioFatura.DeletePorRefUsaAsync(processo.Ref_USA);
            await _repositorioRecibo.DeletePorRefUsaAsync(processo.Ref_USA);
            await _repositorioNotificacao.ExcluirPorRefUsaAsync(processo.Ref_USA);
        }

        public async Task<Processo?> GetByRefUsaAsync(string refUsa)
        {
            var filter = Builders<Processo>.Filter.Eq(p => p.Ref_USA, refUsa);
            return await _colecao.Find(filter).FirstOrDefaultAsync();
        }
        // Dentro da classe RepositorioProcesso.cs

        /// <summary>
        /// Mapeia os dados de um Processo e uma Licenca para um objeto OrgaoAnuente.
        /// Esta é a "fonte da verdade" para os dados que são copiados.
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
                // Ponto de partida: o objeto que já está no banco (liDatabase).
                // Ele contém os dados que queremos preservar (Id, Inspecao, Pendencia, Status).
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

                // Agora, 'orgaoParaSalvar' é a fusão perfeita. Os dados únicos foram
                // preservados e os dados compartilhados/editados foram atualizados.
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
        /// Atualiza, insere e remove (opcional) com base no array de Vistorias do Processo.
        /// </summary>
        private async Task SincronizarVistorias(Processo processo)
        {
            // Busca todas as vistorias no banco pelo Ref_USA do processo
            var vistoriasNoBanco = await _repositorioVistorias.GetByRefUsaAsync(processo.Ref_USA);

            // Atualiza cada vistoria ligada ao Ref_USA do processo com os dados atualizados do processo
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
    }
}
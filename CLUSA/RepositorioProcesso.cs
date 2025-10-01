using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CLUSA
{
    public class RepositorioProcesso
    {
        private readonly IMongoCollection<Processo> _processos;
        private readonly RepositorioOrgaoAnuente _repositorioOrgaoAnuente;
        private readonly RepositorioFatura _repositorioFatura;
        private readonly RepositorioRecibo _repositorioRecibo;
        private readonly RepositorioNotificacao _repositorioNotificacao;

        public RepositorioProcesso()
        {
            var mongoClient = new MongoClient(ConfigDatabase.MongoConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(ConfigDatabase.MongoDatabaseName);

            _processos = mongoDatabase.GetCollection<Processo>("PROCESSO");
            _repositorioOrgaoAnuente = new RepositorioOrgaoAnuente();
            _repositorioFatura = new RepositorioFatura();
            _repositorioRecibo = new RepositorioRecibo();
            _repositorioNotificacao = new RepositorioNotificacao(mongoDatabase);
        }

        public async Task<List<Processo>> ListarTodosAsync()
        {
            return await _processos.Find(FilterDefinition<Processo>.Empty).ToListAsync();
        }
        public async Task<List<Processo>> ListarExcetoSufixoRefUsaAsync(string sufixoAExcluir)
        {
            // Cria a expressão regular para encontrar o que queremos excluir
            var regex = new BsonRegularExpression(new Regex($"{sufixoAExcluir}$", RegexOptions.IgnoreCase));
            var filterParaExcluir = Builders<Processo>.Filter.Regex(p => p.Ref_USA, regex);

            // Usa o operador .Not() para inverter a lógica, trazendo tudo MENOS o que corresponde ao filtro
            var filterFinal = Builders<Processo>.Filter.Not(filterParaExcluir);

            return await _processos.Find(filterFinal).ToListAsync();
        }
        public async Task<List<Processo>> ListarPorSufixoRefUsaAsync(string sufixo)
        {
            // Usamos uma expressão regular para encontrar documentos que terminam com o sufixo.
            // O '$' significa "fim da string" e 'IgnoreCase' ignora maiúsculas/minúsculas.
            var regex = new BsonRegularExpression(new Regex($"{sufixo}$", RegexOptions.IgnoreCase));

            var filter = Builders<Processo>.Filter.Regex(p => p.Ref_USA, regex);

            return await _processos.Find(filter).ToListAsync();
        }
        public async Task<Processo?> ObterPorIdAsync(string id)
        {
            return await _processos.Find(p => p.Id == ObjectId.Parse(id)).FirstOrDefaultAsync();
        }

        public async Task<List<string>> ObterValoresUnicosAsync(string campo)
        {
            var cursor = await _processos.DistinctAsync<string>(campo, FilterDefinition<Processo>.Empty);
            return await cursor.ToListAsync();
        }

        public async Task<List<Processo>> PesquisarAsync(string campo, string pesquisa)
        {
            var filter = Builders<Processo>.Filter.Regex(campo, new BsonRegularExpression(new Regex(pesquisa, RegexOptions.IgnoreCase)));
            return await _processos.Find(filter).ToListAsync();
        }

        public async Task CreateAsync(Processo processo)
        {
            await _processos.InsertOneAsync(processo);
            await SincronizarLicencas(processo);
            await _repositorioFatura.CreateAsync(new Fatura(processo));
            await _repositorioRecibo.CreateAsync(new Recibo(processo));
        }


        public async Task UpdateAsync(Processo processo)
        {
            await _processos.ReplaceOneAsync(p => p.Id == processo.Id, processo);
            await SincronizarLicencas(processo);
        }

        public async Task DeleteAsync(string processoId)
        {
            var processo = await ObterPorIdAsync(processoId);
            if (processo == null) return;

            await _processos.DeleteOneAsync(p => p.Id == ObjectId.Parse(processoId));

            // CORREÇÃO: Usando o novo nome do método.
            await _repositorioOrgaoAnuente.DeleteAllByRefUsaAsync(processo.Ref_USA);
            await _repositorioFatura.DeletePorRefUsaAsync(processo.Ref_USA);
            await _repositorioRecibo.DeletePorRefUsaAsync(processo.Ref_USA);
            await _repositorioNotificacao.ExcluirPorRefUsaAsync(processo.Ref_USA);
        }

        public async Task<Processo?> GetByRefUsaAsync(string refUsa)
        {
            var filter = Builders<Processo>.Filter.Eq(p => p.Ref_USA, refUsa);
            return await _processos.Find(filter).FirstOrDefaultAsync();
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
                DataChegada = processo.DataDeAtracacao,
                HistoricoDoProcesso = processo.HistoricoDoProcesso,
                Pendencia = processo.Pendencia,
                
                Numero = li.Numero,
                NCM = li.NCM,
                DataRegistro = li.DataRegistro,
                LPCO = li.LPCO,
                StatusLI = li.StatusLI,
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
    }
}
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

            foreach (var tipoOrgao in processo.OrgaosAnuentesEnvolvidos)
            {
                // CORREÇÃO: Usando o novo construtor simplificado.
                var orgaoAnuente = new OrgaoAnuente(processo.Ref_USA, tipoOrgao);
                await _repositorioOrgaoAnuente.CreateAsync(orgaoAnuente);
            }

            await _repositorioFatura.CreateAsync(new Fatura(processo));
            await _repositorioRecibo.CreateAsync(new Recibo(processo));
        }

        public async Task UpdateAsync(Processo processo)
        {
            await _processos.ReplaceOneAsync(p => p.Id == processo.Id, processo);
            await SincronizarOrgaosAnuentes(processo);
            await _repositorioFatura.UpdatePorRefUsaAsync(processo.Ref_USA, new Fatura(processo));
            await _repositorioRecibo.UpdatePorRefUsaAsync(processo.Ref_USA, new Recibo(processo));
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

        private async Task SincronizarOrgaosAnuentes(Processo processo)
        {
            var orgaosRequeridos = processo.OrgaosAnuentesEnvolvidos;
            // CORREÇÃO: Usando o novo nome do método.
            var orgaosAtuais = await _repositorioOrgaoAnuente.ListByRefUsaAsync(processo.Ref_USA);
            var tiposAtuais = orgaosAtuais.Select(o => o.Tipo).ToList();

            var tiposParaAdicionar = orgaosRequeridos.Except(tiposAtuais);
            foreach (var tipo in tiposParaAdicionar)
            {
                // CORREÇÃO: Usando o novo construtor simplificado.
                await _repositorioOrgaoAnuente.CreateAsync(new OrgaoAnuente(processo.Ref_USA, tipo));
            }

            var tiposParaRemover = tiposAtuais.Except(orgaosRequeridos);
            foreach (var tipo in tiposParaRemover)
            {
                await _repositorioOrgaoAnuente.DeleteAsync(processo.Ref_USA, tipo);
            }

            var tiposParaAtualizar = orgaosRequeridos.Intersect(tiposAtuais);
            foreach (var tipo in tiposParaAtualizar)
            {
                // CORREÇÃO: Usando o novo construtor simplificado.
                // Criamos um novo objeto OrgaoAnuente apenas com o link, pois os dados específicos
                // (Pendencia, Status, etc.) seriam atualizados em outra tela (FrmOrgaoAnuente).
                // Se o objetivo é apenas garantir que ele exista, um objeto vazio serve.
                // Se precisar copiar dados do processo, crie um construtor apropriado.
                await _repositorioOrgaoAnuente.UpdateAsync(processo.Ref_USA, tipo, new OrgaoAnuente(processo.Ref_USA, tipo));
            }
        }
    }
}
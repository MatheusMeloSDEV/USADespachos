using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CLUSA
{
    /// <summary>
    /// Gerencia as operações de banco de dados para a coleção de Órgãos Anuentes.
    /// </summary>
    public class RepositorioOrgaoAnuente
    {
        private readonly IMongoCollection<OrgaoAnuente> _colecao;

        public RepositorioOrgaoAnuente()
        {
            var client = new MongoClient(ConfigDatabase.MongoConnectionString);
            var database = client.GetDatabase(ConfigDatabase.MongoDatabaseName);
            _colecao = database.GetCollection<OrgaoAnuente>("OrgaosAnuentes");
        }

        /// <summary>
        /// Insere um novo documento de OrgaoAnuente na coleção.
        /// </summary>
        public async Task CreateAsync(OrgaoAnuente orgao)
        {
            await _colecao.InsertOneAsync(orgao);
        }

        /// <summary>
        /// Retorna uma lista com todos os documentos de OrgaoAnuente.
        /// </summary>
        public async Task<List<OrgaoAnuente>> GetAllAsync()
        {
            return await _colecao.Find(FilterDefinition<OrgaoAnuente>.Empty).ToListAsync();
        }

        /// <summary>
        /// Busca um OrgaoAnuente pelo seu ID único.
        /// </summary>
        public async Task<OrgaoAnuente?> GetByIdAsync(string id)
        {
            var filter = Builders<OrgaoAnuente>.Filter.Eq(x => x.Id, ObjectId.Parse(id));
            return await _colecao.Find(filter).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Busca um OrgaoAnuente específico pela combinação de Ref_USA e Tipo.
        /// </summary>
        public async Task<OrgaoAnuente?> GetByRefUsaAndTypeAsync(string refUsa, TipoOrgaoAnuente tipo)
        {
            var filter = Builders<OrgaoAnuente>.Filter.And(
                Builders<OrgaoAnuente>.Filter.Eq(x => x.Ref_USA, refUsa),
                Builders<OrgaoAnuente>.Filter.Eq(x => x.Tipo, tipo)
            );
            return await _colecao.Find(filter).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Lista todos os Órgãos Anuentes associados a uma mesma Ref_USA.
        /// </summary>
        public async Task<List<OrgaoAnuente>> ListByRefUsaAsync(string refUsa)
        {
            var filter = Builders<OrgaoAnuente>.Filter.Eq(x => x.Ref_USA, refUsa);
            return await _colecao.Find(filter).ToListAsync();
        }

        /// <summary>
        /// Pesquisa documentos com base em um campo e um valor de texto.
        /// </summary>
        public async Task<List<OrgaoAnuente>> SearchAsync(string field, string value)
        {
            var filter = Builders<OrgaoAnuente>.Filter.Regex(field, new BsonRegularExpression(new Regex(value, RegexOptions.IgnoreCase)));
            return await _colecao.Find(filter).ToListAsync();
        }

        /// <summary>
        /// Atualiza um OrgaoAnuente existente ou o insere caso não exista (Upsert).
        /// </summary>
        public async Task UpdateAsync(string refUsa, TipoOrgaoAnuente tipo, OrgaoAnuente entidade)
        {
            var filter = Builders<OrgaoAnuente>.Filter.And(
                Builders<OrgaoAnuente>.Filter.Eq(x => x.Ref_USA, refUsa),
                Builders<OrgaoAnuente>.Filter.Eq(x => x.Tipo, tipo)
            );

            // Tenta buscar o documento existente para preservar o ID original.
            var orgaoExistente = await _colecao.Find(filter).FirstOrDefaultAsync();
            if (orgaoExistente != null)
            {
                entidade.Id = orgaoExistente.Id;
            }

            // A opção IsUpsert = true faz a mágica:
            // Se encontrar um documento com o filtro, ele o substitui.
            // Se NÃO encontrar, ele insere a 'entidade' como um novo documento.
            await _colecao.ReplaceOneAsync(filter, entidade, new ReplaceOptions { IsUpsert = true });
        }

        /// <summary>
        /// Deleta um OrgaoAnuente específico pela combinação de Ref_USA e Tipo.
        /// </summary>
        public async Task DeleteAsync(string refUsa, TipoOrgaoAnuente tipo)
        {
            var filter = Builders<OrgaoAnuente>.Filter.And(
                Builders<OrgaoAnuente>.Filter.Eq(x => x.Ref_USA, refUsa),
                Builders<OrgaoAnuente>.Filter.Eq(x => x.Tipo, tipo)
            );
            await _colecao.DeleteOneAsync(filter);
        }

        /// <summary>
        /// Deleta todos os Órgãos Anuentes associados a uma mesma Ref_USA.
        /// </summary>
        public async Task DeleteAllByRefUsaAsync(string refUsa)
        {
            var filter = Builders<OrgaoAnuente>.Filter.Eq(x => x.Ref_USA, refUsa);
            await _colecao.DeleteManyAsync(filter);
        }
    }
}
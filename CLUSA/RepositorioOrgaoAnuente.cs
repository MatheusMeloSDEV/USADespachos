using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.RegularExpressions;

namespace CLUSA
{
    /// <summary>
    /// Gerencia as operações de banco de dados para a coleção de Órgãos Anuentes.
    /// </summary>
    public class RepositorioOrgaoAnuente
    {
        private readonly IMongoCollection<OrgaoAnuente> _colecao;

        public RepositorioOrgaoAnuente(IMongoDatabase? database = null)
        {
            var db = database ?? ConfigDatabase.GetDatabase();
            _colecao = db.GetCollection<OrgaoAnuente>("OrgaosAnuentes");
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
            if (!ObjectId.TryParse(id, out var objectId))
                return null;

            var filter = Builders<OrgaoAnuente>.Filter.Eq(x => x.Id, objectId);
            return await _colecao.Find(filter).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Busca um OrgaoAnuente específico pela combinação de Ref_USA e Tipo.
        /// </summary>
        public async Task<OrgaoAnuente?> GetByNumeroAsync(string numero)
        {
            var filter = Builders<OrgaoAnuente>.Filter.Eq(x => x.Numero, numero);
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
        public async Task UpdateAsync(OrgaoAnuente orgao)
        {
            var filter = Builders<OrgaoAnuente>.Filter.Eq(x => x.Id, orgao.Id);
            await _colecao.ReplaceOneAsync(filter, orgao);
        }

        /// <summary>
        /// Deleta um OrgaoAnuente específico pela combinação de Ref_USA e Tipo.
        /// </summary>
        public async Task DeleteByIdAsync(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId))
                return;

            var filter = Builders<OrgaoAnuente>.Filter.Eq(x => x.Id, objectId);
            await _colecao.DeleteOneAsync(filter);
        }
        public async Task DeleteAllByRefUsaAsync(string refUsa)
        {
            var filter = Builders<OrgaoAnuente>.Filter.Eq(x => x.Ref_USA, refUsa);
            await _colecao.DeleteManyAsync(filter);
        }
    }
}
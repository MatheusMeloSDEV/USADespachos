using CLUSA.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CLUSA.Repositories
{
    public class RepositorioOrgaoAnuente : RepositorioBase<OrgaoAnuente>
    {
        public RepositorioOrgaoAnuente(IMongoDatabase? database = null)
            : base("OrgaosAnuentes", database) { }

        public async Task<List<OrgaoAnuente>> GetAllAsync() => await ListarTodosAsync();

        public async Task<OrgaoAnuente?> GetByIdAsync(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId)) return null;
            return await ObterPorIdAsync(objectId);
        }

        public async Task<OrgaoAnuente?> GetByNumeroAsync(string numero)
        {
            var filter = Builders<OrgaoAnuente>.Filter.Eq(x => x.Numero, numero);
            return await _colecao.Find(filter).FirstOrDefaultAsync();
        }
        public async Task ExecutarBulkAsync(IEnumerable<WriteModel<OrgaoAnuente>> operations)
        {
            if (operations != null && operations.Any())
            {
                await _colecao.BulkWriteAsync(operations);
            }
        }
        public async Task<List<OrgaoAnuente>> ListByRefUsaAsync(string refUsa) => await GetListByRefUsaAsync(refUsa);
        public async Task<List<OrgaoAnuente>> GetListByRefUsaAsync(string refUsa)
        {
            var filter = Builders<OrgaoAnuente>.Filter.Eq(x => x.Ref_USA, refUsa);
            return await _colecao.Find(filter).ToListAsync();
        }
        public async Task<List<OrgaoAnuente>> GetByListaRefUsaAsync(IEnumerable<string> refsUsa)
        {
            var filter = Builders<OrgaoAnuente>.Filter.In(x => x.Ref_USA, refsUsa);

            return await _colecao.Find(filter).ToListAsync();
        }


        public async Task<List<OrgaoAnuente>> SearchAsync(string field, string value)
        {
            var filter = Builders<OrgaoAnuente>.Filter.Regex(field, new BsonRegularExpression(new Regex(value, RegexOptions.IgnoreCase)));
            return await _colecao.Find(filter).ToListAsync();
        }

        public async Task DeleteByIdAsync(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId)) return;
            await DeleteAsync(objectId);
        }

        public async Task DeleteAllByRefUsaAsync(string refUsa) => await DeletePorRefUsaAsync(refUsa);

    }
}

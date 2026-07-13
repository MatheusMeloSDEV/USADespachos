using CLUSA.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CLUSA.Repositories
{
    public class RepositorioVistoriaDUIMP
    {
        private readonly IMongoCollection<VistoriaDUIMP> _colecao;

        public RepositorioVistoriaDUIMP(IMongoDatabase? database = null)
        {
            var db = database ?? ConfigDatabase.GetDatabase();
            _colecao = db.GetCollection<VistoriaDUIMP>("VistoriasDUIMP");
        }

        public async Task UpsertAsync(VistoriaDUIMP item)
        {
            if (string.IsNullOrWhiteSpace(item.DUIMP)) return;

            // Tenta achar pelo DUIMP (chave única)
            var filtro = Builders<VistoriaDUIMP>.Filter.Eq(v => v.DUIMP, item.DUIMP);
            var existente = await _colecao.Find(filtro).Project(v => v.Id).FirstOrDefaultAsync();

            if (existente != ObjectId.Empty)
                item.Id = existente;
            else if (item.Id == ObjectId.Empty)
                item.Id = ObjectId.GenerateNewId();

            var filtroId = Builders<VistoriaDUIMP>.Filter.Eq(v => v.Id, item.Id);
            await _colecao.ReplaceOneAsync(filtroId, item, new ReplaceOptions { IsUpsert = true });
        }

        public async Task DeleteByDUIMPAsync(string duimp)
        {
            if (string.IsNullOrWhiteSpace(duimp)) return;
            await _colecao.DeleteOneAsync(v => v.DUIMP == duimp);
        }

        public async Task<List<VistoriaDUIMP>> GetAllAsync()
        {
            return await _colecao.Find(FilterDefinition<VistoriaDUIMP>.Empty).ToListAsync();
        }

        public async Task<VistoriaDUIMP?> GetByDUIMPAsync(string duimp)
        {
            if (string.IsNullOrWhiteSpace(duimp)) return null;
            return await _colecao.Find(v => v.DUIMP == duimp).FirstOrDefaultAsync();
        }
    }
}
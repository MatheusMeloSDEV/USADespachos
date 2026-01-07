using CLUSA.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLUSA.Repositories
{
    public class RepositorioVistorias
    {
        private readonly IMongoCollection<Vistoria> _colecao;

        public RepositorioVistorias(IMongoDatabase? database = null)
        {
            var db = database ?? ConfigDatabase.GetDatabase();
            _colecao = db.GetCollection<Vistoria>("Vistorias");
        }
        public async Task ExecutarBulkAsync(IEnumerable<WriteModel<Vistoria>> operations)
        {
            if (operations != null && operations.Any())
            {
                await _colecao.BulkWriteAsync(operations);
            }
        }
        public async Task<List<Vistoria>> GetAllAsync()
        {
            return await _colecao.Find(FilterDefinition<Vistoria>.Empty).ToListAsync();
        }
        public async Task<List<Vistoria>> GetByListaRefUsaAsync(IEnumerable<string> refsUsa)
        {
            var filter = Builders<Vistoria>.Filter.In(v => v.Ref_USA, refsUsa);
            return await _colecao.Find(filter).ToListAsync();
        }

        public async Task UpsertAsync(Vistoria vistoria)
        {
            if (vistoria.Id == ObjectId.Empty) vistoria.Id = ObjectId.GenerateNewId();

            var filter = Builders<Vistoria>.Filter.Eq(v => v.LPCO, vistoria.LPCO);

            await _colecao.ReplaceOneAsync(filter, vistoria, new ReplaceOptions { IsUpsert = true });
        }

        public async Task<List<Vistoria>> GetByRefUsaAsync(string refUsa)
        {
            var filter = Builders<Vistoria>.Filter.Eq(v => v.Ref_USA, refUsa);
            return await _colecao.Find(filter).ToListAsync();
        }

        public async Task InsertAsync(Vistoria vistoria)
        {
            if (vistoria.Id == default || vistoria.Id == ObjectId.Empty)
                vistoria.Id = MongoDB.Bson.ObjectId.GenerateNewId();
            await _colecao.InsertOneAsync(vistoria);
        }

        public async Task<Vistoria?> GetByLPCOAsync(string lpco)
        {
            var filter = Builders<Vistoria>.Filter.Eq(v => v.LPCO, lpco ?? "");
            return await _colecao.Find(filter).FirstOrDefaultAsync();
        }

        public async Task DeleteByLpcoAsync(string numeroLpco)
        {
            if (string.IsNullOrEmpty(numeroLpco)) return;
            var filter = Builders<Vistoria>.Filter.Eq(v => v.LPCO, numeroLpco);
            await _colecao.DeleteOneAsync(filter);
        }

        public async Task DeleteAsync(ObjectId id)
        {
            var filter = Builders<Vistoria>.Filter.Eq(x => x.Id, id);
            await _colecao.DeleteOneAsync(filter);
        }
    }
}

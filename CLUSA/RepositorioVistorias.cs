using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CLUSA
{
    public class RepositorioVistorias
    {
        private readonly IMongoCollection<Vistoria> _colecao;

        public RepositorioVistorias(IMongoDatabase database)
        {
            _colecao = database.GetCollection<Vistoria>("Vistorias");
        }

        public async Task<List<Vistoria>> GetAllAsync()
        {
            return await _colecao.Find(FilterDefinition<Vistoria>.Empty).ToListAsync();
        }

        public async Task UpsertAsync(Vistoria vistoria)
        {
            var filter = Builders<Vistoria>.Filter.Eq(v => v.LPCO, vistoria.LPCO);
            var existente = await _colecao.Find(filter).FirstOrDefaultAsync();
            if (existente != null)
            {
                // Atualiza mantendo o Id original
                vistoria.Id = existente.Id;
                await _colecao.ReplaceOneAsync(filter, vistoria);
            }
            else
            {
                // Insere novo
                if (vistoria.Id == default) // ou == ObjectId.Empty
                    vistoria.Id = MongoDB.Bson.ObjectId.GenerateNewId();
                await _colecao.InsertOneAsync(vistoria);
            }
        }
        public async Task<List<Vistoria>> GetByRefUsaAsync(string refUsa)
        {
            var filter = Builders<Vistoria>.Filter.Eq(v => v.Ref_USA, refUsa);
            return await _colecao.Find(filter).ToListAsync();
        }
        public async Task InsertAsync(Vistoria vistoria)
        {
            // Gera um novo Id, se não estiver definido
            if (vistoria.Id == default)
                vistoria.Id = MongoDB.Bson.ObjectId.GenerateNewId();
            await _colecao.InsertOneAsync(vistoria);
        }
        public async Task<Vistoria?> GetByLPCOAsync(string lpco)
        {
            var filter = Builders<Vistoria>.Filter.Eq(v => v.LPCO, lpco ?? "");
            return await _colecao.Find(filter).FirstOrDefaultAsync();
        }
        /// <summary>
        /// Deleta um registro de vistoria com base no número do LPCO.
        /// </summary>
        public async Task DeleteByLpcoAsync(string numeroLpco)
        {
            // Se o número do LPCO for nulo ou vazio, não faz nada.
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
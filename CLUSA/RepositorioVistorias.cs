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
            // Upsert: Atualiza se encontrar pelo LPCO, ou insere se não encontrar.
            var filter = Builders<Vistoria>.Filter.Eq(v => v.LPCO, vistoria.LPCO);
            await _colecao.ReplaceOneAsync(filter, vistoria, new ReplaceOptions { IsUpsert = true });
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
    }
}
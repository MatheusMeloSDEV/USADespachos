using MongoDB.Driver;

namespace CLUSA
{
    public class RepositorioRecibo : RepositorioBase<Recibo>
    {
        public RepositorioRecibo(IMongoDatabase? database = null)
            : base("Recibo", database)
        {
        }

        public async Task<List<Recibo>> FindRefAsync()
        {
            var filter = Builders<Recibo>.Filter.And(
                Builders<Recibo>.Filter.Ne(f => f.Ref_USA, null),
                Builders<Recibo>.Filter.Ne(f => f.Importador, null)
            );
            return await _colecao.Find(filter).ToListAsync();
        }
    }
}
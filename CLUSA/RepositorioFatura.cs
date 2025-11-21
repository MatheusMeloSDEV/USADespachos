using MongoDB.Driver;

namespace CLUSA
{
    public class RepositorioFatura : RepositorioBase<Fatura>
    {
        // ATUALIZADO: Aceita o database e repassa para o base
        public RepositorioFatura(IMongoDatabase? database = null)
            : base("Fatura", database)
        {
        }

        public async Task<List<Fatura>> FindRefAsync()
        {
            var filter = Builders<Fatura>.Filter.And(
                Builders<Fatura>.Filter.Ne(f => f.Ref_USA, null),
                Builders<Fatura>.Filter.Ne(f => f.Importador, null)
            );
            return await _colecao.Find(filter).ToListAsync();
        }
    }
}
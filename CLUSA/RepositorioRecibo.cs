using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CLUSA
{
    public class RepositorioRecibo : RepositorioBase<Recibo>
    {
        // O construtor apenas informa o nome da coleção para a classe base.
        public RepositorioRecibo() : base("Recibo") { }

        // Mantenha aqui apenas os métodos que são ESPECÍFICOS para Recibo.
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
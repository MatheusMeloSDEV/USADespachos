using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CLUSA
{
    public class RepositorioFatura : RepositorioBase<Fatura>
    {
        // O construtor apenas informa o nome da coleção para a classe base.
        public RepositorioFatura() : base("Fatura") { }

        // Mantenha aqui apenas os métodos que são ESPECÍFICOS para Fatura.
        // Por exemplo, o seu método FindRef, agora em versão async.
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
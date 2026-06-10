using CLUSA.Models;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace CLUSA.Repositories
{
    public class RepositorioCatalogo : RepositorioBase<Catalogo>
    {
        // Passa o nome exato da coleção no MongoDB para a classe base
        public RepositorioCatalogo() : base("Catalogos")
        {
        }

        // Aqui você pode adicionar métodos EXCLUSIVOS do Catálogo
        // Exemplo: Uma busca especializada por NCM
        public async Task<Catalogo?> ObterPorNcmAsync(string ncm)
        {
            var filter = Builders<Catalogo>.Filter.Eq(x => x.NCM, ncm);
            return await BuscarUmAsync(filter);
        }

        // Exemplo: Verificar se um catálogo já existe antes de adicionar
        public async Task<bool> ExisteCatalogoComNcmAsync(string ncm)
        {
            var filter = Builders<Catalogo>.Filter.Eq(x => x.NCM, ncm);
            var count = await ContarAsync(filter);
            return count > 0;
        }
    }
}
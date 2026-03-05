using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using CLUSA.Models;

namespace CLUSA.Repositories
{
    public class RepositorioLog
    {
        private readonly IMongoCollection<Log> _collection;

        public RepositorioLog()
        {
            var database = ConfigDatabase.GetDatabase();
            _collection = database.GetCollection<Log>("logs_sistema");
        }

        public async Task<List<Log>> ObterUltimosAsync(int quantidade)
        {
            var sort = Builders<Log>.Sort.Descending(x => x.DataHora);

            return await _collection.Find(_ => true)
                                    .Sort(sort)
                                    .Limit(quantidade)
                                    .ToListAsync();
        }
        // Adicionado o parâmetro 'autor'
        public async Task RegistrarLogAsync(string tipo, string autor, string mensagem,  string detalhes = null)
        {
            var log = new Log
            {
                TipoAcao = tipo,
                Autor = autor, // Vincula o autor
                Mensagem = mensagem,
                Detalhes = detalhes
            };

            await _collection.InsertOneAsync(log);
        }

        // Método para ler os logs (para exibir num Grid futuramente)
        public async Task<List<Log>> ObterTodosAsync()
        {
            // Ordena do mais recente para o mais antigo
            return await _collection.Find(_ => true)
                                    .SortByDescending(x => x.DataHora)
                                    .ToListAsync();
        }
    }
}

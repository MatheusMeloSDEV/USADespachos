using CLUSA.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLUSA.Repositories
{
    public class RepositorioVencimento
    {
        private readonly IMongoCollection<Vencimento> _collection;

        public RepositorioVencimento()
        {
            var database = ConfigDatabase.GetDatabase();
            _collection = database.GetCollection<Vencimento>("vencimentos");
        }

        public async Task AdicionarAsync(Vencimento vencimento)
        {
            await _collection.InsertOneAsync(vencimento);
        }

        public async Task<List<Vencimento>> ObterTodosAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        // --- NOVOS MÉTODOS PARA OS BOTÕES FUNCIONAREM ---

        // Necessário para preencher a tela de edição
        public async Task<Vencimento> ObterPorIdAsync(string id)
        {
            return await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        // Necessário para salvar a edição
        public async Task AtualizarAsync(Vencimento vencimento)
        {
            // Substitui o documento antigo pelo novo onde o ID for igual
            await _collection.ReplaceOneAsync(x => x.Id == vencimento.Id, vencimento);
        }

        public async Task ExcluirAsync(string id)
        {
            await _collection.DeleteOneAsync(x => x.Id == id);
        }
    }
}

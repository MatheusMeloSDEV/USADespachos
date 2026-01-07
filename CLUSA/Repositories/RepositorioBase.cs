using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using CLUSA.Interfaces;

namespace CLUSA.Repositories
{


    /// <summary>
    /// Repositório base genérico para operações CRUD no MongoDB
    /// </summary>
    public abstract class RepositorioBase<T> where T : IEntidadeBase
    {
        protected readonly IMongoCollection<T> _colecao;

        protected RepositorioBase(string nomeColecao, IMongoDatabase? database = null)
        {
            var db = database ?? ConfigDatabase.GetDatabase();
            _colecao = db.GetCollection<T>(nomeColecao);
        }

        // CREATE
        public virtual async Task InsertAsync(T entidade)
        {
            if (entidade.Id == ObjectId.Empty)
            {
                entidade.Id = ObjectId.GenerateNewId();
            }
            await _colecao.InsertOneAsync(entidade);
        }

        public virtual async Task InsertManyAsync(List<T> entidades)
        {
            foreach (var entidade in entidades)
            {
                if (entidade.Id == ObjectId.Empty)
                {
                    entidade.Id = ObjectId.GenerateNewId();
                }
            }
            await _colecao.InsertManyAsync(entidades);
        }

        // READ
        public virtual async Task<List<T>> ListarTodosAsync()
        {
            return await _colecao.Find(FilterDefinition<T>.Empty).ToListAsync();
        }

        public virtual async Task<T?> ObterPorIdAsync(ObjectId id)
        {
            var filter = Builders<T>.Filter.Eq(x => x.Id, id);
            return await _colecao.Find(filter).FirstOrDefaultAsync();
        }

        // UPDATE
        public virtual async Task UpdateAsync(T entidade)
        {
            var filter = Builders<T>.Filter.Eq(x => x.Id, entidade.Id);
            await _colecao.ReplaceOneAsync(filter, entidade);
        }

        // DELETE
        public virtual async Task DeleteAsync(ObjectId id)
        {
            var filter = Builders<T>.Filter.Eq(x => x.Id, id);
            await _colecao.DeleteOneAsync(filter);
        }

        public virtual async Task DeleteManyAsync(FilterDefinition<T> filter)
        {
            await _colecao.DeleteManyAsync(filter);
        }

        // MÉTODOS AUXILIARES ESPECÍFICOS PARA PROCESSOS
        public virtual async Task DeletePorRefUsaAsync(string refUsa)
        {
            var filter = Builders<T>.Filter.Eq("Ref_USA", refUsa);
            await _colecao.DeleteManyAsync(filter);
        }

        // CONTAGEM
        public virtual async Task<long> ContarAsync(FilterDefinition<T>? filter = null)
        {
            filter ??= FilterDefinition<T>.Empty;
            return await _colecao.CountDocumentsAsync(filter);
        }

        // BUSCA CUSTOMIZADA
        public virtual async Task<List<T>> BuscarAsync(FilterDefinition<T> filter)
        {
            return await _colecao.Find(filter).ToListAsync();
        }

        public virtual async Task<T?> BuscarUmAsync(FilterDefinition<T> filter)
        {
            return await _colecao.Find(filter).FirstOrDefaultAsync();
        }
    }
}

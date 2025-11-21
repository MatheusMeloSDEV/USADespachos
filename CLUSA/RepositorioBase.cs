using MongoDB.Bson;
using MongoDB.Driver;

namespace CLUSA
{
    /// <summary>
    /// Repositório genérico com operações CRUD comuns para coleções do MongoDB.
    /// </summary>
    /// <typeparam name="T">O tipo da entidade, que deve implementar IEntidadeBase.</typeparam>
    public abstract class RepositorioBase<T> where T : IEntidadeBase
    {
        protected readonly IMongoCollection<T> _colecao;

        protected RepositorioBase(string collectionName, IMongoDatabase? database = null)
        {
            var db = database ?? ConfigDatabase.GetDatabase();

            _colecao = db.GetCollection<T>(collectionName);
        }

        public async Task<List<T>> ListarTodosAsync()
        {
            return await _colecao.Find(FilterDefinition<T>.Empty).ToListAsync();
        }

        public async Task<T> ObterPorIdAsync(ObjectId id)
        {
            var filter = Builders<T>.Filter.Eq(doc => doc.Id, id);
            return await _colecao.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<T> ObterPorRefUSAAsync(string refUsa)
        {
            if (string.IsNullOrWhiteSpace(refUsa))
                throw new ArgumentException("Ref_USA não pode ser nula ou vazia.", nameof(refUsa));

            var filter = Builders<T>.Filter.Eq(doc => doc.Ref_USA, refUsa);
            return await _colecao.Find(filter).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(T entidade)
        {
            if (entidade == null) throw new ArgumentNullException(nameof(entidade));
            await _colecao.InsertOneAsync(entidade);
        }

        /// <summary>
        /// Substitui um documento inteiro pelo Id. É mais simples e seguro que a lista de .Set().
        /// </summary>
        public async Task UpdateAsync(T entidade)
        {
            if (entidade == null) throw new ArgumentNullException(nameof(entidade));
            var filter = Builders<T>.Filter.Eq(doc => doc.Id, entidade.Id);
            await _colecao.ReplaceOneAsync(filter, entidade);
        }

        /// <summary>
        /// Atualiza um documento buscando pela Ref_USA.
        /// </summary>
        public async Task UpdatePorRefUsaAsync(string refUsa, T entidade)
        {
            if (entidade == null) throw new ArgumentNullException(nameof(entidade));

            var original = await ObterPorRefUSAAsync(refUsa);
            if (original != null)
            {
                entidade.Id = original.Id; // Preserva o ID original do banco
            }

            var filter = Builders<T>.Filter.Eq(doc => doc.Ref_USA, refUsa);
            await _colecao.ReplaceOneAsync(filter, entidade, new ReplaceOptions { IsUpsert = true });
        }

        public async Task DeleteAsync(ObjectId id)
        {
            var filter = Builders<T>.Filter.Eq(doc => doc.Id, id);
            await _colecao.DeleteOneAsync(filter);
        }

        public async Task DeletePorRefUsaAsync(string refUsa)
        {
            var filter = Builders<T>.Filter.Eq(doc => doc.Ref_USA, refUsa);
            await _colecao.DeleteOneAsync(filter);
        }
    }
}
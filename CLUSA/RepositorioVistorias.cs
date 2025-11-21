using MongoDB.Bson;
using MongoDB.Driver;

namespace CLUSA
{
    public class RepositorioVistorias
    {
        private readonly IMongoCollection<Vistoria> _colecao;

        public RepositorioVistorias(IMongoDatabase database = null)
        {
            var db = database ?? ConfigDatabase.GetDatabase();
            _colecao = db.GetCollection<Vistoria>("Vistorias");
        }

        public async Task<List<Vistoria>> GetAllAsync()
        {
            return await _colecao.Find(FilterDefinition<Vistoria>.Empty).ToListAsync();
        }

        public async Task UpsertAsync(Vistoria vistoria)
        {
            // Filtra pelo LPCO (que é sua chave única de negócio)
            var filter = Builders<Vistoria>.Filter.Eq(v => v.LPCO, vistoria.LPCO);

            // 1. Tenta buscar o documento existente SOMENTE para pegar o _id correto
            // (Usamos projeção para trazer apenas o Id, economizando dados)
            var idExistente = await _colecao
                .Find(filter)
                .Project(v => v.Id)
                .FirstOrDefaultAsync();

            // Verifica se o ID retornado não é vazio (ObjectId.Empty é '0000...')
            if (idExistente != ObjectId.Empty)
            {
                // CRUCIAL: Sobrescreve o ID gerado no 'new Vistoria()' pelo ID que já está no banco
                vistoria.Id = idExistente;

                // Agora o ReplaceOne funciona porque o _id é idêntico
                await _colecao.ReplaceOneAsync(filter, vistoria);
            }
            else
            {
                // Se não existe, insere o novo (com o ID novo gerado no construtor)
                await _colecao.InsertOneAsync(vistoria);
            }
        }
        public async Task<List<Vistoria>> GetByRefUsaAsync(string refUsa)
        {
            var filter = Builders<Vistoria>.Filter.Eq(v => v.Ref_USA, refUsa);
            return await _colecao.Find(filter).ToListAsync();
        }
        public async Task InsertAsync(Vistoria vistoria)
        {
            // Gera um novo Id, se não estiver definido
            if (vistoria.Id == default)
                vistoria.Id = MongoDB.Bson.ObjectId.GenerateNewId();
            await _colecao.InsertOneAsync(vistoria);
        }
        public async Task<Vistoria?> GetByLPCOAsync(string lpco)
        {
            var filter = Builders<Vistoria>.Filter.Eq(v => v.LPCO, lpco ?? "");
            return await _colecao.Find(filter).FirstOrDefaultAsync();
        }
        /// <summary>
        /// Deleta um registro de vistoria com base no número do LPCO.
        /// </summary>
        public async Task DeleteByLpcoAsync(string numeroLpco)
        {
            if (string.IsNullOrEmpty(numeroLpco)) return;
            var filter = Builders<Vistoria>.Filter.Eq(v => v.LPCO, numeroLpco);
            await _colecao.DeleteOneAsync(filter);
        }

        public async Task DeleteAsync(ObjectId id)
        {
            var filter = Builders<Vistoria>.Filter.Eq(x => x.Id, id);
            await _colecao.DeleteOneAsync(filter);
        }
    }
}
using MongoDB.Bson;
using MongoDB.Driver;

namespace CLUSA
{
    public class NotifUrgente
    {
        public ObjectId Id { get; set; }
        public ObjectId UsuarioOrigemId { get; set; }
        public ObjectId UsuarioDestinoId { get; set; }
        public string Mensagem { get; set; } = string.Empty;
        public DateTime DataEnvio { get; set; }
        public bool Done { get; set; }
    }
    public class UsuarioDestinoItem
    {
        public ObjectId Id { get; set; }
        public string NomeUsuario { get; set; } = "";
    }
    public class RepositorioNotifUrgente
    {
        private readonly IMongoCollection<NotifUrgente> _colecao;

        public RepositorioNotifUrgente(IMongoDatabase database)
        {
            _colecao = database.GetCollection<NotifUrgente>("NotifUrgente");
        }

        public async Task InsertAsync(NotifUrgente notif)
        {
            await _colecao.InsertOneAsync(notif);
        }

        public async Task UpdateAsync(NotifUrgente notif)
        {
            var filter = Builders<NotifUrgente>.Filter.Eq(x => x.Id, notif.Id);
            await _colecao.ReplaceOneAsync(filter, notif);
        }

        public async Task DeleteAsync(ObjectId id)
        {
            var filter = Builders<NotifUrgente>.Filter.Eq(x => x.Id, id);
            await _colecao.DeleteOneAsync(filter);
        }

        public async Task<List<NotifUrgente>> GetByUsuarioOrigemAsync(ObjectId usuarioOrigemId)
        {
            var filter = Builders<NotifUrgente>.Filter.Eq(x => x.UsuarioOrigemId, usuarioOrigemId);
            return await _colecao.Find(filter).ToListAsync();
        }

        public async Task<List<NotifUrgente>> GetByUsuarioDestinoAsync(ObjectId usuarioDestinoId)
        {
            var filter = Builders<NotifUrgente>.Filter.Eq(x => x.UsuarioDestinoId, usuarioDestinoId);
            return await _colecao.Find(filter).ToListAsync();
        }

        public async Task<List<NotifUrgente>> GetPendentesPorUsuarioAsync(ObjectId userId)
        {
            var filter = Builders<NotifUrgente>.Filter.And(
                Builders<NotifUrgente>.Filter.Or(
                    Builders<NotifUrgente>.Filter.Eq(x => x.UsuarioOrigemId, userId),
                    Builders<NotifUrgente>.Filter.Eq(x => x.UsuarioDestinoId, userId)
                ),
                Builders<NotifUrgente>.Filter.Eq(x => x.Done, false)
            );
            return await _colecao.Find(filter).ToListAsync();
        }
    }
}

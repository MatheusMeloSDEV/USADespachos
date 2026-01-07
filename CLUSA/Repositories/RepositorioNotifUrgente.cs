using CLUSA.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLUSA.Repositories
{
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

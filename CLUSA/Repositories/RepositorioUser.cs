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
    public class RepositorioUsers
    {
        // A variável privada se chama _Users
        private readonly IMongoCollection<Users> _Users;

        public RepositorioUsers(IMongoDatabase database = null)
        {
            var db = database ?? ConfigDatabase.GetDatabase();

            _Users = db.GetCollection<Users>("Users");
        }
        public async Task<Users> GetByIdAsync(ObjectId id)
        {
            var filtro = Builders<Users>.Filter.Eq(u => u.Id, id);

            return await _Users.Find(filtro).FirstOrDefaultAsync();
        }
        public async Task<List<Users>> FindAllAsync()
        {
            var filter = Builders<Users>.Filter.Empty;
            return await _Users.Find(filter).ToListAsync();
        }

        public async Task CreateAsync(Users user)
        {
            await _Users.InsertOneAsync(user);
        }

        public async Task UpdateAsync(Users user)
        {
            var filter = Builders<Users>.Filter.Eq(u => u.Id, user.Id);
            var update = Builders<Users>.Update
                .Set(u => u.Username, user.Username)
                .Set(u => u.Password, user.Password)
                .Set(u => u.Admin, user.Admin)
                .Set(u => u.PreferenciasGrids, user.PreferenciasGrids);

            await _Users.UpdateOneAsync(filter, update);
        }

        public async Task DeleteAsync(Users user)
        {
            var filter = Builders<Users>.Filter.Eq(u => u.Id, user.Id);
            await _Users.DeleteOneAsync(filter);
        }

        public Logado Login(Users user)
        {
            Logado log = new();
            var filter = Builders<Users>.Filter.Eq(g => g.Username, user.Username);
            var usuarioEncontrado = _Users.Find(filter).FirstOrDefault();

            if (usuarioEncontrado == null)
                return log;

            if (usuarioEncontrado.Password == user.Password)
            {
                log.log = true;
                log.admin = usuarioEncontrado.Admin;
                log.Usuario = usuarioEncontrado.Username;
                log.Id = usuarioEncontrado.Id;
            }

            return log;
        }
    }
}

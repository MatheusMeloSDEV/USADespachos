using MongoDB.Driver;

namespace CLUSA
{
    public class RepositorioUsers
    {
        private readonly IMongoCollection<Users> _Users;
        private readonly IMongoClient _mongoClient;
        private readonly IMongoDatabase _mongoDatabase;

        public RepositorioUsers()
        {
            _mongoClient = new MongoClient(ConfigDatabase.MongoConnectionString);
            _mongoDatabase = _mongoClient.GetDatabase(ConfigDatabase.MongoDatabaseName);
            _Users = _mongoDatabase.GetCollection<Users>("Users");
        }

        public List<Users> ListaUsers
        {
            get
            {
                var filter = Builders<Users>.Filter.Empty;
                return _Users.Find(filter).ToList();
            }
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
                .Set(u => u.Admin, user.Admin);
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

using MongoDB.Driver;

namespace CLUSA
{
    public static class ConfigDatabase
    {
        // --- CONFIGURAÇÃO ---
        // Mude para 'false' quando quiser usar o banco de teste
        private static bool IsProducao = false;

        public static string MongoConnectionString => IsProducao
            ? "mongodb+srv://dev:dev@cluster0.cn10nzt.mongodb.net/" // Produção
            : "mongodb+srv://dev:dev@testeusa.kt5go1v.mongodb.net/"; // Desenvolvimento

        public static string MongoDatabaseName => "Trabalho";

        // --- SINGLETON (A Mágica acontece aqui) ---

        // Variáveis estáticas para segurar a conexão viva na memória
        private static MongoClient? _client;
        private static IMongoDatabase? _database;

        // Objeto para garantir segurança em ambientes multi-thread (travamento)
        private static readonly object _lock = new object();

        /// <summary>
        /// Retorna a instância ÚNICA do banco de dados.
        /// Se não existir, conecta. Se já existir, reutiliza.
        /// </summary>
        public static IMongoDatabase GetDatabase()
        {
            // Se já estiver conectado, retorna a conexão existente imediatamente
            if (_database != null) return _database;

            // Se não, bloqueia outras threads para não criar duas conexões ao mesmo tempo
            lock (_lock)
            {
                if (_database == null)
                {
                    // Cria a conexão pela PRIMEIRA e ÚNICA vez
                    _client = new MongoClient(MongoConnectionString);
                    _database = _client.GetDatabase(MongoDatabaseName);
                }
            }

            return _database;
        }
    }
}
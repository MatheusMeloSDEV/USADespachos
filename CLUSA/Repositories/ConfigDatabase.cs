using MongoDB.Driver;
using CLUSA.Services;
using CLUSA.Helpers;
using System;

namespace CLUSA.Repositories
{
    public static class ConfigDatabase
    {
        // Agora o IsProducao pode ser controlado por uma variável de ambiente ou ficar true por padrão
        private static bool IsProducao = true;

        // Lógica Híbrida: 
        // 1. Tenta ler "MONGODB_URI" da Nuvem (GitHub).
        // 2. Se for nulo, usa o EmailConfig local baseado no IsProducao.
        public static string MongoConnectionString
        {
            get
            {
                // 1. Tenta sempre ler da Nuvem primeiro
                var uriNuvem = Environment.GetEnvironmentVariable("MONGODB_URI");
                if (!string.IsNullOrEmpty(uriNuvem)) return uriNuvem;

                // 2. Se não estiver na nuvem, usa a lógica local
                #if GITHUB_ACTIONS
                    return "mongodb://localhost:27017"; 
                #else
                // No seu PC, ele vai usar o seu arquivo privado normalmente
                bool IsProducao = true;
                return IsProducao ? CLUSA.Helpers.EmailConfig.MongoUriProducao : CLUSA.Helpers.EmailConfig.MongoUriTeste;
                #endif
            }
        }

        public static string MongoDatabaseName => "Trabalho";

        private static MongoClient? _client;
        private static IMongoDatabase? _database;
        private static readonly object _lock = new object();
        public static void ConfigurarParaTeste()
        {
            IsProducao = false;
        }
        public static IMongoDatabase GetDatabase()
        {
            if (_database != null) return _database;

            lock (_lock)
            {
                if (_database == null)
                {
                    // O MongoClient agora usa a string dinâmica
                    _client = new MongoClient(MongoConnectionString);
                    _database = _client.GetDatabase(MongoDatabaseName);
                }
            }
            return _database;
        }
    }
}
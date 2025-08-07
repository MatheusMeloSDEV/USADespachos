using MongoDB.Driver;

namespace CLUSA
{
    public class RepositorioOrgaoAnuente<T> where T : class
    {
        private readonly IMongoCollection<T> _colecao;

        public RepositorioOrgaoAnuente(string collectionName)
        {
            var client = new MongoClient(ConfigDatabase.MongoConnectionString);
            var database = client.GetDatabase(ConfigDatabase.MongoDatabaseName);
            _colecao = database.GetCollection<T>(collectionName);
        }

        // Síncronos
        public List<T> ListarTodos()
        {
            return _colecao.Find(FilterDefinition<T>.Empty).ToList();
        }

        public T? ObterPorRefUsa(string refUsa)
        {
            var filter = Builders<T>.Filter.Eq("Ref_USA", refUsa);
            return _colecao.Find(filter).FirstOrDefault();
        }

        public async Task<T?> ObterPorIdAsync(string id)
        {
            var filtro = Builders<T>.Filter.Eq("Ref_USA", id);
            return await _colecao.Find(filtro).FirstOrDefaultAsync();
        }

        // Assíncronos
        public async Task<List<T>> ListarTodosAsync()
        {
            return await _colecao.Find(FilterDefinition<T>.Empty).ToListAsync();
        }

        public async Task AtualizarAsync(string refUsa, T entidade)
        {
            var filter = Builders<T>.Filter.Eq("Ref_USA", refUsa);

            // Atualiza apenas os campos alterados na coleção principal
            var atual = await _colecao.Find(filter).FirstOrDefaultAsync();
            if (atual == null)
                return;

            var tipo = typeof(T);
            var updates = new List<UpdateDefinition<T>>();
            var propriedades = new[]
            {
                "Ref_USA", "Importador", "SR", "Exportador", "Veiculo", "Produto", "Origem",
                "Li", "InspecaoIbama", "CheckInspecaoIbama", "InspecaoMapa", "CheckInspecaoMapa",
                "DataDeAtracacao", "CheckDataDeAtracacao", "DataEmbarque", "CheckDataEmbarque",
                "Amostra", "Pendencia", "StatusDoProcesso"
            };

            foreach (var prop in propriedades)
            {
                var info = tipo.GetProperty(prop);
                if (info != null)
                {
                    var valorNovo = info.GetValue(entidade);
                    var valorAtual = info.GetValue(atual);

                    if ((valorNovo == null && valorAtual != null) ||
                        (valorNovo != null && !valorNovo.Equals(valorAtual)))
                    {
                        updates.Add(Builders<T>.Update.Set(prop, valorNovo));
                    }
                }
            }

            if (updates.Count > 0)
            {
                var updateDef = Builders<T>.Update.Combine(updates);
                await _colecao.UpdateOneAsync(filter, updateDef);
            }

            // Atualiza as outras coleções relacionadas
            await AtualizarColecoesRelacionadasAsync(refUsa, entidade);
        }

        private async Task AtualizarColecoesRelacionadasAsync(string refUsa, T entidade)
        {
            var client = new MongoClient(ConfigDatabase.MongoConnectionString);
            var database = client.GetDatabase(ConfigDatabase.MongoDatabaseName);

            var colecoes = new[] { "MAPA", "ANVISA", "DECEX", "IBAMA", "IMETRO", "PROCESSO" };

            foreach (var nomeColecao in colecoes)
            {
                // Use o mesmo tipo T para todas as coleções relacionadas
                var colecao = database.GetCollection<T>(nomeColecao);
                var filter = Builders<T>.Filter.Eq("Ref_USA", refUsa);

                var updates = new List<UpdateDefinition<T>>();
                var tipo = entidade.GetType();

                var propriedades = new[]
                {
                    "Ref_USA", "Importador", "SR", "Exportador", "Veiculo", "Produto", "Origem",
                    "LI", "CheckInspecaoMapa", "DataDeAtracacao", "CheckDataDeAtracacao", "DataEmbarque",
                    "CheckDataEmbarque", "Amostra", "Pendencia", "StatusDoProcesso"
                };

                foreach (var prop in propriedades)
                {
                    var info = tipo.GetProperty(prop);
                    if (info != null)
                    {
                        var valorNovo = info.GetValue(entidade);
                        updates.Add(Builders<T>.Update.Set(prop, valorNovo));
                    }
                }

                if (updates.Count > 0)
                {
                    var updateDef = Builders<T>.Update.Combine(updates);
                    await colecao.UpdateOneAsync(filter, updateDef);
                }
            }
        }
    }
}
using MongoDB.Driver;

namespace CLUSA
{
    public class RepositorioNotificacao
    {
        private readonly IMongoCollection<Notificacao> _colecao;

        public RepositorioNotificacao(IMongoDatabase? database = null)
        {
            var db = database ?? ConfigDatabase.GetDatabase();
            _colecao = db.GetCollection<Notificacao>("Notificacao");
        }
        public async Task InsertManyAsync(List<Notificacao> notificacoes)
        {
            if (notificacoes == null || !notificacoes.Any())
            {
                return;
            }
            await _colecao.InsertManyAsync(notificacoes);
        }
        public async Task<List<Notificacao>> ObterNaoVisualizadasPorProcessosAsync(List<string> refsUsa, int limite = 500)
        {
            var filtroBuilder = Builders<Notificacao>.Filter;
            var filtro = filtroBuilder.In(n => n.RefUsa, refsUsa) & filtroBuilder.Eq(n => n.Visualizado, false);
            var sort = Builders<Notificacao>.Sort.Descending(n => n.DataCriacao);

            return await _colecao.Find(filtro)
                .Sort(sort)
                .Limit(limite)
                .ToListAsync();
        }

        public async Task ExcluirPorRefUsaAsync(string refUsa)
        {
            var filtro = Builders<Notificacao>.Filter.Eq(n => n.RefUsa, refUsa);
            await _colecao.DeleteManyAsync(filtro);
        }
        public async Task<int> ContarNaoVisualizadasAsync()
        {
            var filtro = Builders<Notificacao>.Filter.Eq(n => n.Visualizado, false);
            return (int)await _colecao.CountDocumentsAsync(filtro);
        }

        public async Task<List<Notificacao>> ObterNotificacoesNaoVisualizadasAsync(int limite = 20, int skip = 0)
        {
            var filtro = Builders<Notificacao>.Filter.Eq(n => n.Visualizado, false);
            var sort = Builders<Notificacao>.Sort.Descending(n => n.DataCriacao);

            return await _colecao.Find(filtro)
                .Sort(sort)
                .Skip(skip)
                .Limit(limite)
                .ToListAsync();
        }
        public async Task MarcarComoVisualizadoAsync(string refUsa, string mensagem)
        {
            var filtro = Builders<Notificacao>.Filter.And(
                Builders<Notificacao>.Filter.Eq(n => n.RefUsa, refUsa),
                Builders<Notificacao>.Filter.Eq(n => n.Mensagem, mensagem)
            );
            var update = Builders<Notificacao>.Update.Set(n => n.Visualizado, true);
            var resultado = await _colecao.UpdateManyAsync(filtro, update);

            if (resultado.ModifiedCount > 0)
            {
                Console.WriteLine($"Notificação do processo {refUsa} marcada como visualizada.");
            }
            else
            {
                Console.WriteLine($"Nenhuma notificação foi atualizada para o processo {refUsa}.");
            }
        }
    }
}
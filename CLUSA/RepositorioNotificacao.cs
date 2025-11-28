using MongoDB.Bson;
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
        /// <summary>
        /// Exclui todas as notificações de um processo que correspondem exatamente à mensagem fornecida.
        /// Usado para exclusão por conclusão de tarefa (ex: Redestinar).
        /// </summary>
        public async Task ExcluirPorMensagemExataAsync(string refUsa, string mensagemExata)
        {
            var filtro = Builders<Notificacao>.Filter.And(
                Builders<Notificacao>.Filter.Eq(n => n.RefUsa, refUsa),
                Builders<Notificacao>.Filter.Eq(n => n.Mensagem, mensagemExata)
            );
            await _colecao.DeleteManyAsync(filtro);
        }

        /// <summary>
        /// Exclui notificações onde o conteúdo da mensagem contém o padrão (tipo) fornecido.
        /// Usado para exclusão por tipo de vencimento (ex: "Vencimento FMA"),
        /// pois a mensagem completa ("Vencimento FMA em X dia(s)") muda com os dias.
        /// </summary>
        public async Task ExcluirPorTipoNaMensagemAsync(string refUsa, string tipoNotificacao)
        {
            // Constrói um filtro para encontrar a string do tipo em qualquer lugar da Mensagem, de forma case-insensitive ("i").
            var regex = new BsonRegularExpression($".*{tipoNotificacao}.*", "i");

            var filtro = Builders<Notificacao>.Filter.And(
                Builders<Notificacao>.Filter.Eq(n => n.RefUsa, refUsa),
                Builders<Notificacao>.Filter.Regex(n => n.Mensagem, regex)
            );
            await _colecao.DeleteManyAsync(filtro);
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
        /// <summary>
        /// Verifica se uma notificação com a mesma RefUsa e Mensagem já existe.
        /// Usado para evitar duplicatas ao re-gerar notificações.
        /// </summary>
        public async Task<bool> ExisteNotificacaoAsync(string refUsa, string mensagem)
        {
            var filtro = Builders<Notificacao>.Filter.And(
                Builders<Notificacao>.Filter.Eq(n => n.RefUsa, refUsa),
                Builders<Notificacao>.Filter.Eq(n => n.Mensagem, mensagem)
            );
            return await _colecao.Find(filtro).AnyAsync();
        }

        /// <summary>
        /// Exclui permanentemente notificações cuja data de criação é anterior à data limite.
        /// Usado para limpeza do banco (sustentabilidade).
        /// </summary>
        public async Task ExcluirNotificacoesAntigasAsync(DateTime dataLimite)
        {
            // Filtra por notificações cuja DataCriacao é menor que a dataLimite
            var filtro = Builders<Notificacao>.Filter.Lt(n => n.DataCriacao, dataLimite);
            await _colecao.DeleteManyAsync(filtro);
        }
    }
}
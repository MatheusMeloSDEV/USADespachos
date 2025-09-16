using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CLUSA
{
    public class RepositorioNotificacao
    {
        private readonly IMongoCollection<Notificacao> _colecao;

        public RepositorioNotificacao(IMongoDatabase database)
        {
            _colecao = database.GetCollection<Notificacao>("Notificacao");
        }
        public async Task<bool> ExisteNotificacaoAsync(string refUsa, string mensagem)
        {
            var filtro = Builders<Notificacao>.Filter.And(
                Builders<Notificacao>.Filter.Eq(n => n.RefUsa, refUsa),
                Builders<Notificacao>.Filter.Eq(n => n.Mensagem, mensagem)
            );
            return await _colecao.Find(filtro).AnyAsync();
        }
        public async Task ExcluirPorRefUsaAsync(string refUsa)
        {
            var filtro = Builders<Notificacao>.Filter.Eq(n => n.RefUsa, refUsa);
            await _colecao.DeleteManyAsync(filtro); 
        }
        public async Task SalvarNotificacaoAsync(Notificacao notif)
        {
            await _colecao.InsertOneAsync(notif);
        }
        public async Task<Notificacao> ObterNotificacaoPorRefUsaAsync(string refUsa)
        {
            var filtro = Builders<Notificacao>.Filter.Eq(n => n.RefUsa, refUsa);
            return await _colecao.Find(filtro).FirstOrDefaultAsync(); 
        }
        public async Task<List<Notificacao>> ObterNotificacoesNaoVisualizadasAsync()
        {
            var filtro = Builders<Notificacao>.Filter.Eq(n => n.Visualizado, false);
            return await _colecao.Find(filtro).ToListAsync(); 
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
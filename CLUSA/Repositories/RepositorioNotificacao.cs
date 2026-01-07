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
            if (notificacoes == null || !notificacoes.Any()) return;
            await _colecao.InsertManyAsync(notificacoes);
        }

        public async Task ExcluirPorRefUsaAsync(string refUsa)
        {
            var filtro = Builders<Notificacao>.Filter.Eq(n => n.RefUsa, refUsa);
            await _colecao.DeleteManyAsync(filtro);
        }

        public async Task<bool> ExisteNotificacaoAsync(string refUsa, string mensagem)
        {
            // Limit(1) garante que o banco pare de procurar assim que achar o primeiro
            var filtro = Builders<Notificacao>.Filter.And(
                Builders<Notificacao>.Filter.Eq(n => n.RefUsa, refUsa),
                Builders<Notificacao>.Filter.Eq(n => n.Mensagem, mensagem)
            );
            return await _colecao.Find(filtro).Limit(1).AnyAsync();
        }
        public async Task ExcluirPorMensagemExataAsync(string refUsa, string mensagem)
        {
            var filtro = Builders<Notificacao>.Filter.And(
                Builders<Notificacao>.Filter.Eq(n => n.RefUsa, refUsa),
                Builders<Notificacao>.Filter.Eq(n => n.Mensagem, mensagem)
            );

            await _colecao.DeleteManyAsync(filtro);
        }

        // 2. Exclui se a RefUsa bater E a mensagem contiver o texto (tipo)
        public async Task ExcluirPorTipoNaMensagemAsync(string refUsa, string trechoMensagem)
        {
            // Usa Regex para simular um "Contains" (SQL LIKE %texto%)
            // O "i" no BsonRegularExpression torna a busca Case Insensitive (ignora maiúsculas/minúsculas)
            var filtro = Builders<Notificacao>.Filter.And(
                Builders<Notificacao>.Filter.Eq(n => n.RefUsa, refUsa),
                Builders<Notificacao>.Filter.Regex(n => n.Mensagem, new BsonRegularExpression(trechoMensagem, "i"))
            );

            await _colecao.DeleteManyAsync(filtro);
        }
        public async Task ExcluirNotificacoesAntigasAsync(DateTime dataLimite)
        {
            var filtro = Builders<Notificacao>.Filter.Lt(n => n.DataCriacao, dataLimite);
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
            await _colecao.UpdateManyAsync(filtro, update);
        }
    }
}

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CLUSA
{
    #region "Notificação"
    public class Notificacao
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string? RefUsa { get; set; }
        public string? Mensagem { get; set; }
        public DateTime DataCriacao { get; set; }
        public bool Visualizado { get; set; }
    }

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
            var filtro = Builders<Notificacao>.Filter.And(
                Builders<Notificacao>.Filter.Eq(n => n.RefUsa, refUsa),
                Builders<Notificacao>.Filter.Eq(n => n.Mensagem, mensagem)
            );
            return await _colecao.Find(filtro).AnyAsync();
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

    public class GerenciadorNotificacao
    {
        private readonly RepositorioNotificacao _notificacaoRepo;

        public GerenciadorNotificacao(IMongoDatabase database)
        {
            _notificacaoRepo = new RepositorioNotificacao(database);
        }

        public async Task CriarNotificacaoSeNecessarioAsync(string refUsa, string mensagem)
        {
            if (string.IsNullOrWhiteSpace(refUsa) || string.IsNullOrWhiteSpace(mensagem)) return;

            bool existe = await _notificacaoRepo.ExisteNotificacaoAsync(refUsa, mensagem);

            if (!existe)
            {
                var novaNotificacao = new Notificacao
                {
                    RefUsa = refUsa,
                    Mensagem = mensagem,
                    DataCriacao = DateTime.Now,
                    Visualizado = false
                };
                await _notificacaoRepo.InsertManyAsync(new List<Notificacao> { novaNotificacao });
            }
        }

        public async Task ExcluirNotificacoesAntigasAsync(DateTime dataLimite)
        {
            await _notificacaoRepo.ExcluirNotificacoesAntigasAsync(dataLimite);
        }

        public async Task SincronizarNotificacoesDoProcessoAsync(Processo processo)
        {
            if (processo == null || string.IsNullOrWhiteSpace(processo.Ref_USA)) return;

            await _notificacaoRepo.ExcluirPorRefUsaAsync(processo.Ref_USA);

            if (processo.DataDeAtracacao.HasValue)
            {
                int dias = (processo.DataDeAtracacao.Value - DateTime.Today).Days;

                if (dias is >= 0 and <= 15)
                {
                    await CriarNotificacaoSeNecessarioAsync(
                        processo.Ref_USA,
                        $"Processo {processo.Ref_USA}: Dar entrada no Mapa/Anvisa"
                    );
                }

                if (dias is >= 0 and <= 5 && (processo.Redestinacao == null || processo.Redestinacao == false))
                {
                    await CriarNotificacaoSeNecessarioAsync(
                        processo.Ref_USA,
                        $"Processo {processo.Ref_USA}: Redestinar container ao terminal"
                    );
                }
            }

            if (processo.DataRegistroDI == null)
            {
                VerificarEVincularVencimento(processo, processo.VencimentoFreeTime, "FreeTime");

                VerificarEVincularVencimento(processo, processo.VencimentoFMA, "FMA");

                VerificarEVincularVencimento(processo, processo.VencimentoLI_LPCO, "LI/LPCO");
            }
        }

        private async void VerificarEVincularVencimento(Processo doc, DateTime? vencimento, string nomeExibicao)
        {
            if (!vencimento.HasValue) return;
            int dias = (vencimento.Value - DateTime.Today).Days;

            if (dias is >= 0 and <= 5)
            {
                string msg = $"Processo {doc.Ref_USA}: Vencimento {nomeExibicao} em {dias} dia(s)";
                await CriarNotificacaoSeNecessarioAsync(doc.Ref_USA, msg);
            }
        }
    }
    #endregion
    #region "notificação Urgente"
    public class NotifUrgente
    {
        public ObjectId Id { get; set; }
        public ObjectId UsuarioOrigemId { get; set; }
        public ObjectId UsuarioDestinoId { get; set; }
        public string Mensagem { get; set; } = string.Empty;
        public DateTime DataEnvio { get; set; }
        public bool Done { get; set; }
    }
    public class UsuarioDestinoItem
    {
        public ObjectId Id { get; set; }
        public string NomeUsuario { get; set; } = "";
    }
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
    #endregion
}
using CLUSA.Models;
using CLUSA.Repositories;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLUSA.Services
{
    public class NotificacaoService
    {
        private readonly RepositorioNotificacao _notificacaoRepo;

        public NotificacaoService(IMongoDatabase database)
        {
            _notificacaoRepo = new RepositorioNotificacao(database);
        }

        public async Task CriarNotificacaoSeNecessarioAsync(string refUsa, string mensagem)
        {
            if (string.IsNullOrWhiteSpace(refUsa) || string.IsNullOrWhiteSpace(mensagem)) return;

            bool existe = await _notificacaoRepo.ExisteNotificacaoAsync(refUsa, mensagem);

            if (!existe)
            {
                var nova = new Notificacao
                {
                    RefUsa = refUsa,
                    Mensagem = mensagem,
                    DataCriacao = DateTime.Now,
                    Visualizado = false
                };
                await _notificacaoRepo.InsertManyAsync(new List<Notificacao> { nova });
            }
        }

        public async Task ExcluirNotificacoesAntigasAsync(DateTime dataLimite)
        {
            await _notificacaoRepo.ExcluirNotificacoesAntigasAsync(dataLimite);
        }

        public async Task SincronizarNotificacoesDoProcessoAsync(Processo processo)
        {
            if (processo == null || string.IsNullOrWhiteSpace(processo.Ref_USA)) return;

            try
            {
                // 1. Limpa anteriores deste processo
                await _notificacaoRepo.ExcluirPorRefUsaAsync(processo.Ref_USA);

                var tasks = new List<Task>();

                // 2. Redestinação
                if (processo.DataDeAtracacao.HasValue)
                {
                    int dias = (processo.DataDeAtracacao.Value - DateTime.Today).Days;

                    // Verifica se está no prazo E se NÃO foi feita redestinação
                    if (dias >= 0 && dias <= 10 && (processo.Redestinacao == null || processo.Redestinacao == false))
                    {
                        tasks.Add(CriarNotificacaoSeNecessarioAsync(processo.Ref_USA,
                            $"Processo {processo.Ref_USA}: Redestinar container ao terminal ({dias} dias restantes)"));
                    }
                }

                // 3. Vencimentos
                if (processo.DataRegistroDI == null)
                {
                    tasks.Add(CheckVencimento(processo, processo.VencimentoFreeTime, "FreeTime"));
                    tasks.Add(CheckVencimento(processo, processo.VencimentoFMA, "FMA"));
                    tasks.Add(CheckVencimento(processo, processo.VencimentoLI_LPCO, "LI/LPCO"));
                }

                await Task.WhenAll(tasks);
            }
            catch (MongoConnectionException ex)
            {
                // Logar erro mas NÃO parar a aplicação. 
                // O Timer vai tentar de novo em alguns segundos/minutos.
                Console.WriteLine($"Erro de conexão ao sincronizar {processo.Ref_USA}: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Captura erros genéricos de Socket
                Console.WriteLine($"Erro genérico ao sincronizar {processo.Ref_USA}: {ex.Message}");
            }
        }

        private Task CheckVencimento(Processo doc, DateTime? vencimento, string nomeExibicao)
        {
            if (!vencimento.HasValue) return Task.CompletedTask;
            int dias = (vencimento.Value - DateTime.Today).Days;

            if (dias >= 0 && dias <= 5)
            {
                string msg = $"Processo {doc.Ref_USA}: Vencimento {nomeExibicao} em {dias} dia(s)";
                return CriarNotificacaoSeNecessarioAsync(doc.Ref_USA, msg);
            }
            return Task.CompletedTask;
        }
    }
}

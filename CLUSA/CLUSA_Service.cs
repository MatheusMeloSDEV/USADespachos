using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CLUSA
{
    /// <summary>
    /// Camada de Serviço (Facade) que orquestra todos os repositórios e a lógica de negócio
    /// (como o GerenciadorNotificacao) para simplificar a aplicação (UI).
    /// O FrmPrincipal deve depender apenas desta classe.
    /// </summary>
    public class CLUSA_Service
    {
        private readonly RepositorioProcesso _processoRepo;
        private readonly RepositorioNotificacao _notificacaoRepo;
        private readonly RepositorioUsers _userRepo;
        private readonly GerenciadorNotificacao _gerenciadorNotificacao;
        private readonly IMongoDatabase _database;

        public CLUSA_Service(IMongoDatabase database)
        {
            _database = database;

            _processoRepo = new RepositorioProcesso(database);
            _notificacaoRepo = new RepositorioNotificacao(database);
            _userRepo = new RepositorioUsers(database);

            _gerenciadorNotificacao = new GerenciadorNotificacao(database);
        }

        // ============================================================
        // 1. MÉTODOS DE PROCESSO
        // ============================================================

        public async Task<List<Processo>> ListarTodosProcessosAtivosAsync()
        {
            return await _processoRepo.ListarPrincipalOtimizadoAsync();
        }

        public async Task UpdateProcessoAsync(Processo processo)
        {
            await _processoRepo.UpdateAsync(processo);
            // Chama sincronização imediatamente após o salvamento
            await _gerenciadorNotificacao.SincronizarNotificacoesDoProcessoAsync(processo);
        }

        // ============================================================
        // 2. MÉTODOS DE NOTIFICAÇÃO
        // ============================================================

        public async Task SincronizarProcessosEContadorAsync(List<Processo> processosMonitorados)
        {
            foreach (var p in processosMonitorados)
            {
                await _gerenciadorNotificacao.SincronizarNotificacoesDoProcessoAsync(p);
            }
        }

        public async Task ExcluirNotificacoesAntigasAsync(DateTime dataLimite)
        {
            await _gerenciadorNotificacao.ExcluirNotificacoesAntigasAsync(dataLimite);
        }

        public async Task<int> ContarNotificacoesNaoVisualizadasAsync()
        {
            return await _notificacaoRepo.ContarNaoVisualizadasAsync();
        }

        public async Task<List<Notificacao>> ObterNotificacoesParaUIAsync(int limite, int skip)
        {
            return await _notificacaoRepo.ObterNotificacoesNaoVisualizadasAsync(limite, skip);
        }

        public async Task MarcarNotificacaoComoVisualizadaAsync(string refUsa, string mensagem)
        {
            await _notificacaoRepo.MarcarComoVisualizadoAsync(refUsa, mensagem);
        }

        // ============================================================
        // 3. MÉTODOS DE USUÁRIO
        // ============================================================

        public async Task<List<Users>> ListarTodosUsuariosAsync()
        {
            return await _userRepo.FindAllAsync();
        }

        // Você pode adicionar mais métodos aqui para OrgaoAnuente, Fatura, Recibo, etc.
    }
}
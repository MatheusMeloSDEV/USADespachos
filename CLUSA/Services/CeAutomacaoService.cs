using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CLUSA.Models;
using CLUSA.Repositories;

namespace CLUSA.Services
{
    public class CeAutomacaoService
    {
        private static readonly HttpClient _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
        private readonly RepositorioProcesso _repositorioProcesso;
        private readonly RepositorioLog _repositorioLog;
        private readonly string _baseUrl;

        public CeAutomacaoService(string? baseUrl = null, RepositorioProcesso? repoProcesso = null, RepositorioLog? repoLog = null)
        {
            _baseUrl = baseUrl ?? Environment.GetEnvironmentVariable("CE_API_URL") ?? "http://localhost:4000";
            _repositorioProcesso = repoProcesso ?? new RepositorioProcesso();
            _repositorioLog = repoLog ?? new RepositorioLog();
        }

        public async Task<StatusCeServicoResponse?> VerificarStatusApiAsync()
        {
            try
            {
                var url = $"{_baseUrl.TrimEnd('/')}/teste";
                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<StatusCeServicoResponse>();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CE_AUTOMAÇÃO] Erro ao verificar status da API: {ex.Message}");
            }

            return null;
        }

        public async Task<ResultadoSincronizacaoCe?> DispararSincronizacaoApiAsync()
        {
            try
            {
                var url = $"{_baseUrl.TrimEnd('/')}/api/sync";
                var response = await _httpClient.PostAsync(url, null);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<ResultadoSincronizacaoCe>();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CE_AUTOMAÇÃO] Erro ao disparar sincronização da API: {ex.Message}");
            }

            return null;
        }

        public async Task<RelatorioSincronizacaoCE> SincronizarProcessosComBancoAsync(string usuario = "Sistema")
        {
            var relatorio = new RelatorioSincronizacaoCE();

            try
            {
                var apiStatus = await VerificarStatusApiAsync();
                if (apiStatus == null)
                {
                    relatorio.Mensagem = "Serviço CE Automação offline ou inacessível em " + _baseUrl;
                    relatorio.Sucesso = false;
                    return relatorio;
                }

                var syncResult = await DispararSincronizacaoApiAsync();
                relatorio.JobId = syncResult?.JobId ?? string.Empty;
                relatorio.ItensProcessadosApi = syncResult?.ProcessedItems ?? 0;

                var processosPendentes = await _repositorioProcesso.ListarProcessosParaSincronizacaoCEAsync();
                relatorio.TotalProcessosAnalisados = processosPendentes.Count;

                int atualizados = 0;
                foreach (var proc in processosPendentes)
                {
                    if (string.IsNullOrWhiteSpace(proc.Ref_USA)) continue;

                    string historico = $"{DateTime.Now:dd/MM/yyyy HH:mm} - Sincronizado automaticamente via módulo CE Automação.";
                    bool alterou = await _repositorioProcesso.RegistrarVerificacaoCEAsync(proc.Ref_USA, historico);
                    if (alterou) atualizados++;
                }

                relatorio.ProcessosAtualizados = atualizados;
                relatorio.Sucesso = true;
                relatorio.Mensagem = $"Sincronização concluída com sucesso. {atualizados} processos atualizados no banco.";

                await _repositorioLog.RegistrarLogAsync(
                    "CE Automação",
                    usuario,
                    $"Sincronização de CE executada. Job: {relatorio.JobId}",
                    $"Processados: {relatorio.ProcessosAtualizados}/{relatorio.TotalProcessosAnalisados}");
            }
            catch (Exception ex)
            {
                relatorio.Sucesso = false;
                relatorio.Mensagem = $"Erro ao sincronizar processos: {ex.Message}";
                await _repositorioLog.RegistrarLogAsync("Erro CE Automação", usuario, "Falha na sincronização", ex.Message);
            }

            return relatorio;
        }
    }

    public class StatusCeServicoResponse
    {
        [JsonPropertyName("service")]
        public string Service { get; set; } = string.Empty;

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("mode")]
        public string Mode { get; set; } = string.Empty;

        [JsonPropertyName("environment")]
        public string Environment { get; set; } = string.Empty;

        [JsonPropertyName("timestamp")]
        public string Timestamp { get; set; } = string.Empty;

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;
    }

    public class ResultadoSincronizacaoCe
    {
        [JsonPropertyName("jobId")]
        public string JobId { get; set; } = string.Empty;

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("processedItems")]
        public int ProcessedItems { get; set; }

        [JsonPropertyName("timestamp")]
        public string Timestamp { get; set; } = string.Empty;
    }

    public class RelatorioSincronizacaoCE
    {
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; } = string.Empty;
        public string JobId { get; set; } = string.Empty;
        public int ItensProcessadosApi { get; set; }
        public int TotalProcessosAnalisados { get; set; }
        public int ProcessosAtualizados { get; set; }
    }
}
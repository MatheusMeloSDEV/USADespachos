using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;

namespace CLUSA
{
    public class AtualizadorGithub
    {
        private readonly string repoUrl;
        private readonly string[] extensoesAceitas;

        public event Action<string, string>? AtualizacaoDisponivel;
        public event Action<string>? DownloadConcluido;
        public event Action<string>? Erro;
        public event Action? Atualizado;

        public AtualizadorGithub(string repoUrl, params string[] extensoesAceitas)
        {
            this.repoUrl = repoUrl.TrimEnd('/');
            this.extensoesAceitas = extensoesAceitas.Length > 0 ? extensoesAceitas : new[] { ".exe" };
        }

        public async Task VerificarAtualizacaoAsync(string? versaoAtual = null)
        {
            string url = $"{repoUrl}/releases/latest";
            using var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("SeuApp/1.0");

            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                Log($"Erro ao buscar atualização: {response.StatusCode} - {response.ReasonPhrase}");
                Erro?.Invoke($"Erro ao buscar atualização: {response.StatusCode} - {response.ReasonPhrase}");
                return;
            }

            var content = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(content);
            string? versaoMaisRecente = doc.RootElement.GetProperty("tag_name").GetString();
            versaoAtual ??= System.Reflection.Assembly.GetEntryAssembly()?.GetName().Version?.ToString() ?? "0.0.0.0";

            string NormalizarVersao(string? v)
            {
                if (string.IsNullOrWhiteSpace(v)) return "";
                var partes = v.TrimStart('v', 'V').Split('-')[0].Split('.');
                return string.Join(".", partes.Take(3));
            }

            if (!string.IsNullOrEmpty(versaoMaisRecente) && NormalizarVersao(versaoMaisRecente) != NormalizarVersao(versaoAtual))
            {
                Log($"Atualização disponível: {versaoAtual} -> {versaoMaisRecente}");
                AtualizacaoDisponivel?.Invoke(versaoMaisRecente, versaoAtual);
            }
        }

        public async Task BaixarEInstalarAsync()
        {
            string url = $"{repoUrl}/releases/latest";
            using var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("SeuApp/1.0");

            var response = await client.GetStringAsync(url);
            using var doc = JsonDocument.Parse(response);
            var assets = doc.RootElement.GetProperty("assets");
            string? downloadUrl = null;
            string? nomeArquivo = null;

            foreach (var asset in assets.EnumerateArray())
            {
                var nome = asset.GetProperty("name").GetString();
                if (!string.IsNullOrEmpty(nome) && extensoesAceitas.Any(ext => nome.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
                {
                    downloadUrl = asset.GetProperty("browser_download_url").GetString();
                    nomeArquivo = nome;
                    break;
                }
            }

            if (string.IsNullOrEmpty(downloadUrl))
            {
                Log("Instalador não encontrado.");
                Erro?.Invoke("Não foi encontrado um instalador para download.");
                return;
            }

            string tempPath = Path.Combine(Path.GetTempPath(), nomeArquivo);
            using (var download = await client.GetAsync(downloadUrl))
            using (var fs = new FileStream(tempPath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await download.Content.CopyToAsync(fs);
            }

            string hash = CalcularHashSHA256(tempPath);
            Log($"Download concluído: {tempPath} | SHA256: {hash}");

            DownloadConcluido?.Invoke(tempPath);

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = tempPath,
                    Arguments = "/VERYSILENT",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                Log($"Erro ao iniciar o instalador: {ex.Message}");
                Erro?.Invoke($"Erro ao iniciar o instalador: {ex.Message}");
            }
        }

        private string CalcularHashSHA256(string filePath)
        {
            using var sha256 = SHA256.Create();
            using var stream = File.OpenRead(filePath);
            var hash = sha256.ComputeHash(stream);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }

        private void Log(string mensagem)
        {
            string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "atualizador.log");
            File.AppendAllText(logPath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {mensagem}{Environment.NewLine}");
        }

        public static bool TemConexaoInternet()
        {
            try
            {
                using var client = new HttpClient();
                var response = client.GetAsync("https://www.google.com").Result;
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}

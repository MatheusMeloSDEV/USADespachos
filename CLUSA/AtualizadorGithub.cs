using System.Diagnostics;
using System.Text.Json;

namespace CLUSA
{
    public class AtualizadorGithub
    {
        private readonly string repoUrl;
        private readonly string nomeZip;

        public event Action<string, string>? AtualizacaoDisponivel;
        public event Action<string>? DownloadConcluido;
        public event Action<string>? Erro;

        public AtualizadorGithub(string repoUrl, string nomeZip = "atualizacao.zip")
        {
            this.repoUrl = repoUrl.TrimEnd('/');
            this.nomeZip = nomeZip;
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

            static string NormalizarVersao(string? v)
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
                if (!string.IsNullOrEmpty(nome) && nome.Equals(nomeZip, StringComparison.OrdinalIgnoreCase))
                {
                    downloadUrl = asset.GetProperty("browser_download_url").GetString();
                    nomeArquivo = nome;
                    break;
                }
            }

            if (string.IsNullOrEmpty(downloadUrl))
            {
                Log("Arquivo de atualização (.zip) não encontrado.");
                Erro?.Invoke("Não foi encontrado o arquivo .zip para download.");
                return;
            }

            string tempPath = Path.Combine(Path.GetTempPath(), nomeArquivo!);
            using (var download = await client.GetAsync(downloadUrl))
            using (var fs = new FileStream(tempPath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await download.Content.CopyToAsync(fs);
            }

            DownloadConcluido?.Invoke(tempPath);

            try
            {
                string batPath = Path.Combine(Path.GetTempPath(), "update-executa.bat");
                string exeName = "Trabalho.exe";
                string destFolder = @"C:\\Program Files (x86)\\UsaDespachos"; 

                File.WriteAllText(batPath,
                    $"@echo off\r\ntaskkill /IM {exeName} /F\r\ntimeout /t 2 /nobreak\r\npowershell -Command \"Expand-Archive -Path '{tempPath}' -DestinationPath '{destFolder}' -Force\"\r\nstart \"\" \"{destFolder}\\{exeName}\"\r\n");

                Process.Start(new ProcessStartInfo
                {
                    FileName = batPath,
                    UseShellExecute = true,
                    Verb = "runas"
                });
            }
            catch (Exception ex)
            {
                Log($"Erro ao executar atualizador: {ex.Message}");
                Erro?.Invoke($"Erro ao executar atualizador: {ex.Message}");
            }
        }

        private static void Log(string mensagem)
        {
            string pastaLog = @"C:\UsaDespachos";
            Directory.CreateDirectory(pastaLog);
            string logPath = Path.Combine(pastaLog, "atualizador.log");
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

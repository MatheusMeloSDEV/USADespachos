using System.Diagnostics;
using System.Reflection;
using System.Text.Json;

namespace CLUSA
{
    public class AtualizadorGithub
    {
        private readonly string repoUrl;
        private readonly string nomeZip;

        // URLs fixas conforme seu script
        private const string UrlLibreOffice = "https://download.documentfoundation.org/libreoffice/stable/25.2.6/win/x86_64/LibreOffice_25.2.6_Win_x86-64.msi";
        private const string UrlDotNet = "https://builds.dotnet.microsoft.com/dotnet/WindowsDesktop/8.0.22/windowsdesktop-runtime-8.0.22-win-x64.exe";

        // Caminhos locais
        private const string CaminhoLibreOfficeExe = @"C:\UsaDespachos\LibreOffice\program\soffice.exe";
        private const string PastaInstalacaoLibre = @"C:\UsaDespachos\LibreOffice";

        public event Action<string, string>? AtualizacaoDisponivel;
        public event Action<string>? DownloadConcluido;
        public event Action<string>? Erro;
        public event Action<string>? StatusInstalacao;

        public AtualizadorGithub(string repoUrl, string nomeZip = "atualizacao.zip")
        {
            this.repoUrl = repoUrl.TrimEnd('/');
            this.nomeZip = nomeZip;
        }
        public async Task VerificarEInstalarDependenciasAsync()
        {
            // 1. Verificar e Instalar .NET 8
            if (!IsDotNet8Installed())
            {
                StatusInstalacao?.Invoke("O .NET 8 não foi detectado. Baixando e instalando...");
                await BaixarEInstalarDotNetAsync();
            }
            else
            {
                Log(".NET 8 já está instalado.");
            }

            // 2. Verificar e Instalar LibreOffice
            if (!File.Exists(CaminhoLibreOfficeExe))
            {
                StatusInstalacao?.Invoke("LibreOffice não encontrado. Baixando e instalando...");
                await BaixarEInstalarLibreOfficeAsync();
            }
            else
            {
                Log("LibreOffice já está instalado.");
            }
        }
        private bool IsDotNet8Installed()
        {
            try
            {
                // Executa o comando 'dotnet --list-runtimes' para verificar o que está instalado
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "dotnet",
                        Arguments = "--list-runtimes",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                // Verifica se existe o WindowsDesktop.App versão 8.x
                return output.Contains("Microsoft.WindowsDesktop.App 8.");
            }
            catch
            {
                // Se der erro (ex: dotnet nem existe no path), assume que não tem
                return false;
            }
        }

        private async Task BaixarEInstalarDotNetAsync()
        {
            try
            {
                string tempPath = Path.Combine(Path.GetTempPath(), "windowsdesktop-runtime-8.0.22-win-x64.exe");

                Log("Baixando .NET Runtime...");
                await DownloadArquivoAsync(UrlDotNet, tempPath);

                Log("Instalando .NET Runtime...");
                ExecutarInstalador(tempPath, "/install /quiet /norestart");
            }
            catch (Exception ex)
            {
                string msg = $"Erro ao instalar .NET: {ex.Message}";
                Log(msg);
                Erro?.Invoke(msg);
            }
        }

        private async Task BaixarEInstalarLibreOfficeAsync()
        {
            try
            {
                string tempPath = Path.Combine(Path.GetTempPath(), "LibreOffice_25.2.6_Win_x86-64.msi");

                // Garante que a pasta de destino existe (igual ao seu script: mkdir)
                if (!Directory.Exists(PastaInstalacaoLibre))
                {
                    Directory.CreateDirectory(PastaInstalacaoLibre);
                }

                Log("Baixando LibreOffice...");
                await DownloadArquivoAsync(UrlLibreOffice, tempPath);

                Log("Instalando LibreOffice...");
                // Argumentos para MSIEXEC conforme seu script
                string argumentos = $"/i \"{tempPath}\" INSTALLLOCATION=\"{PastaInstalacaoLibre}\" RebootYesNo=No /qn";

                // Nota: Para MSI, chamamos o msiexec.exe
                ExecutarInstalador("msiexec.exe", argumentos);
            }
            catch (Exception ex)
            {
                string msg = $"Erro ao instalar LibreOffice: {ex.Message}";
                Log(msg);
                Erro?.Invoke(msg);
            }
        }
        private async Task DownloadArquivoAsync(string url, string destino)
        {
            using var client = new HttpClient();
            // Timeout maior para arquivos grandes
            client.Timeout = TimeSpan.FromMinutes(10);

            using var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync();
            using var fileStream = new FileStream(destino, FileMode.Create, FileAccess.Write, FileShare.None);
            await stream.CopyToAsync(fileStream);
        }

        // Método auxiliar para rodar processos como Administrador
        private void ExecutarInstalador(string caminhoExecutavel, string argumentos)
        {
            var processInfo = new ProcessStartInfo
            {
                FileName = caminhoExecutavel,
                Arguments = argumentos,
                UseShellExecute = true,
                Verb = "runas", // Importante: Solicita permissão de Administrador
                WindowStyle = ProcessWindowStyle.Hidden
            };

            using var process = Process.Start(processInfo);
            process?.WaitForExit(); // Espera a instalação terminar antes de continuar
        }

        public async Task VerificarAtualizacaoAsync(string? versaoAtual = null)
        {
            // Constrói a URL da API do GitHub
            string url = $"{repoUrl}/releases/latest";

            using var client = new HttpClient();
            // User-Agent é OBRIGATÓRIO na API do GitHub
            client.DefaultRequestHeaders.UserAgent.ParseAdd("UsaDespachosApp/1.0");

            try
            {
                Log($"Verificando atualizações em: {url}");
                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    Log($"Falha ao conectar no GitHub: {response.StatusCode}");
                    return;
                }

                var content = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(content);

                // Pega a tag da versão (ex: "v1.2.3")
                string? versaoMaisRecente = doc.RootElement.GetProperty("tag_name").GetString();

                // Pega a versão local se não for passada
                versaoAtual ??= Assembly.GetEntryAssembly()?.GetName().Version?.ToString() ?? "0.0.0.0";

                Log($"Versão Local: {versaoAtual} | Versão Remota: {versaoMaisRecente}");

                if (!string.IsNullOrEmpty(versaoMaisRecente) &&
                    NormalizarVersao(versaoMaisRecente) != NormalizarVersao(versaoAtual))
                {
                    Log($"Nova atualização detectada!");
                    AtualizacaoDisponivel?.Invoke(versaoMaisRecente, versaoAtual);
                }
            }
            catch (Exception ex)
            {
                Log($"Erro ao verificar atualização: {ex.Message}");
                Erro?.Invoke($"Erro ao verificar atualização: {ex.Message}");
            }
        }
        private void ExecutarScriptAtualizacao(string zipPath)
        {
            try
            {
                string batPath = Path.Combine(Path.GetTempPath(), "update_app.bat");

                // Nome do seu executável atual para matar o processo
                string exeName = AppDomain.CurrentDomain.FriendlyName;
                if (!exeName.EndsWith(".exe")) exeName += ".exe";

                // Pasta onde o programa está rodando atualmente
                string currentFolder = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\');

                // Script BAT robusto
                string script = $@"
@echo off
echo Aguardando fechamento da aplicacao...
taskkill /IM ""{exeName}"" /F >nul 2>&1
timeout /t 2 /nobreak >nul

echo Extraindo arquivos...
powershell -Command ""Expand-Archive -Path '{zipPath}' -DestinationPath '{currentFolder}' -Force""

echo Reiniciando aplicacao...
start """" ""{Path.Combine(currentFolder, exeName)}""
del ""{batPath}""
";
                File.WriteAllText(batPath, script);

                Process.Start(new ProcessStartInfo
                {
                    FileName = batPath,
                    UseShellExecute = true,
                    CreateNoWindow = false, // Pode deixar visível pro usuário ver que está atualizando
                    Verb = "runas" // Garante permissão para sobrescrever em C:\Program Files
                });

                // Fecha a aplicação atual para permitir a sobrescrita
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                Log($"Erro ao criar script BAT: {ex.Message}");
                throw;
            }
        }
        private static string NormalizarVersao(string? v)
        {
            if (string.IsNullOrWhiteSpace(v)) return "";
            var partes = v.TrimStart('v', 'V').Split('-')[0].Split('.');
            return string.Join(".", partes.Take(3));
        }
        public async Task BaixarEInstalarAppAsync()
        {
            string url = $"{repoUrl}/releases/latest";
            using var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("UsaDespachosApp/1.0");

            try
            {
                StatusInstalacao?.Invoke("Iniciando download da atualização do sistema...");

                var response = await client.GetStringAsync(url);
                using var doc = JsonDocument.Parse(response);
                var assets = doc.RootElement.GetProperty("assets");

                string? downloadUrl = null;
                string? nomeArquivoEncontrado = null;

                // Procura o asset com o nome definido (ex: atualizacao.zip)
                foreach (var asset in assets.EnumerateArray())
                {
                    var nome = asset.GetProperty("name").GetString();
                    if (!string.IsNullOrEmpty(nome) && nome.Equals(nomeZip, StringComparison.OrdinalIgnoreCase))
                    {
                        downloadUrl = asset.GetProperty("browser_download_url").GetString();
                        nomeArquivoEncontrado = nome;
                        break;
                    }
                }

                if (string.IsNullOrEmpty(downloadUrl))
                {
                    string msg = "Arquivo de atualização não encontrado no Release do GitHub.";
                    Log(msg);
                    Erro?.Invoke(msg);
                    return;
                }

                string tempPath = Path.Combine(Path.GetTempPath(), nomeArquivoEncontrado!);

                // Baixa o ZIP
                Log($"Baixando ZIP de: {downloadUrl}");
                await DownloadArquivoAsync(downloadUrl, tempPath);

                DownloadConcluido?.Invoke(tempPath);
                StatusInstalacao?.Invoke("Download concluído. Aplicando atualização...");

                // Executa o script de atualização (BAT)
                ExecutarScriptAtualizacao(tempPath);
            }
            catch (Exception ex)
            {
                Log($"Erro ao baixar/instalar app: {ex.Message}");
                Erro?.Invoke($"Erro crítico na atualização: {ex.Message}");
            }
        }

        private static void Log(string mensagem)
        {
            try
            {
                string pastaLog = @"C:\UsaDespachos";
                Directory.CreateDirectory(pastaLog);
                string logPath = Path.Combine(pastaLog, "atualizador.log");
                File.AppendAllText(logPath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {mensagem}{Environment.NewLine}");
            }
            catch { /* Ignora falha de log */ }
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

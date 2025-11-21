using System.Diagnostics;
using System.Text;

namespace CLUSA
{
    public static class PythonRunner
    {
        // Tempo limite padrão em milissegundos (70 segundos)
        private const int DefaultTimeout = 70000;

        /// <summary>
        /// Executa o script de Follow-up do Exportador.
        /// </summary>
        public static string ExecutarExportador(string importador)
        {
            string exePath = @"C:\UsaDespachos\Exportador\dist\exportador.exe";
            string arguments = $"\"{importador}\"";
            return ExecutarScriptPdf(exePath, arguments);
        }

        /// <summary>
        /// Executa o script de Faturamento.
        /// </summary>
        public static string ExecutarFaturamento(string importador, string referencia)
        {
            string exePath = @"C:\UsaDespachos\Faturamento\dist\faturamento.exe";
            string arguments = $"\"{referencia}\" \"{importador}\"";
            return ExecutarScriptPdf(exePath, arguments);
        }

        /// <summary>
        /// Executa o script de Recibo.
        /// </summary>
        public static string ExecutarRecibo(string importador, string referencia)
        {
            string exePath = @"C:\UsaDespachos\Recibo\dist\recibo.exe";
            string arguments = $"\"{referencia}\" \"{importador}\"";
            return ExecutarScriptPdf(exePath, arguments);
        }

        // Adicionei os outros métodos para completar o exemplo
        public static string ExecutarRelatorio(string referencia)
        {
            string exePath = @"C:\UsaDespachos\Relatorio\dist\relatorio.exe";
            string arguments = $"\"{referencia}\"";
            return ExecutarScriptPdf(exePath, arguments);
        }

        public static string ExecutarCapa(string referencia)
        {
            string exePath = @"C:\UsaDespachos\Capa\dist\capa.exe";
            string arguments = $"\"{referencia}\"";
            return ExecutarScriptPdf(exePath, arguments);
        }

        /// <summary>
        /// Método central e robusto para executar um script Python,
        /// capturando saídas de forma assíncrona para evitar deadlocks.
        /// </summary>
        private static string ExecutarScriptPdf(string exeFullPath, string arguments)
        {
            try
            {
                if (!File.Exists(exeFullPath))
                    return $"Erro Crítico: Arquivo executável não encontrado em {exeFullPath}";

                var startInfo = new ProcessStartInfo
                {
                    FileName = exeFullPath,
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    StandardOutputEncoding = Encoding.UTF8,
                    StandardErrorEncoding = Encoding.UTF8
                };

                // StringBuilders para coletar a saída em tempo real
                var stdoutBuilder = new StringBuilder();
                var stderrBuilder = new StringBuilder();

                using var process = new Process { StartInfo = startInfo };

                // Configura os "ouvintes" (event handlers) para as saídas
                process.OutputDataReceived += (sender, args) =>
                {
                    if (args.Data != null) stdoutBuilder.AppendLine(args.Data);
                };
                process.ErrorDataReceived += (sender, args) =>
                {
                    if (args.Data != null) stderrBuilder.AppendLine(args.Data);
                };

                process.Start();

                // Inicia a leitura assíncrona. ESSA É A PARTE MAIS IMPORTANTE.
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                // Aguarda o término do processo com um tempo limite
                if (!process.WaitForExit(DefaultTimeout))
                {
                    try { process.Kill(); } catch { /* Ignora erros se o processo já terminou */ }
                    return "Erro de Timeout: O script demorou muito para responder e foi encerrado.";
                }

                string stdout = stdoutBuilder.ToString();
                string stderr = stderrBuilder.ToString();

                // 1. A verificação MAIS IMPORTANTE: o código de saída.
                if (process.ExitCode != 0)
                {
                    // MELHORIA: Combina stdout e stderr para não perder nenhuma informação.
                    var errorOutput = new StringBuilder();
                    errorOutput.AppendLine($"Erro na execução do script (código de saída: {process.ExitCode}):");
                    if (!string.IsNullOrWhiteSpace(stdout))
                    {
                        errorOutput.AppendLine("\n--- Saída Padrão (stdout) ---");
                        errorOutput.AppendLine(stdout.Trim());
                    }
                    if (!string.IsNullOrWhiteSpace(stderr))
                    {
                        errorOutput.AppendLine("\n--- Saída de Erro (stderr) ---");
                        errorOutput.AppendLine(stderr.Trim());
                    }
                    return errorOutput.ToString();
                }

                // 2. Se o script teve sucesso, procuramos pelo caminho do PDF.
                var pdfLine = stdout
                    .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                    .LastOrDefault(line => line.Trim().EndsWith(".pdf", StringComparison.OrdinalIgnoreCase));

                if (pdfLine != null && File.Exists(pdfLine.Trim()))
                {
                    return pdfLine.Trim(); // SUCESSO!
                }

                // 3. Caso de falha estranha: sucesso, mas sem caminho de PDF.
                return $"Erro Inesperado: O script foi executado com sucesso, mas não retornou um caminho de PDF.\n\nSaída completa:\n{stdout.Trim()}";
            }
            catch (Exception ex)
            {
                return $"Exceção na aplicação C#: {ex.Message}\n{ex.StackTrace}";
            }
        }
    }
}
using CLUSA;
using CLUSA.Services;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace Trabalho
{
    internal static class Program
    {
        [STAThread]
        static void Main(string[] args) // Voltando para 'void' síncrono
        {
            // Linhas essenciais para inicializar o WinForms corretamente.
            ApplicationConfiguration.Initialize();

            if (args != null && args.Contains("--rotina-10h"))
            {
                try
                {
                    // 1. Pegamos a logo que está AQUI no projeto Trabalho
                    var minhaLogo = Trabalho.Properties.Resources.FollowUpLogo;

                    // 2. Passamos a logo para o serviço que está LÁ na CLUSA
                    var service = new FollowUpService(minhaLogo);

                    // Esperamos o método assíncrono terminar de forma síncrona
                    service.ExecutarFluxoAutomaticoAsync("LEITESOL").GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    // Só mostra MessageBox se NÃO for a rotina automática
                    if (!Environment.CommandLine.Contains("--rotina-10h"))
                    {
                        MessageBox.Show($"Erro: {ex.Message}");
                    }

                    // Escreve no console para você ler no log do GitHub
                    Console.WriteLine($"[ERRO FATAL]: {ex.Message}");
                    throw;
                }
                return; // Encerra a aplicação após a rotina automática
            }

            // Inicia a interface gráfica normalmente na thread STA
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FrmLogin());
        }
    }
}
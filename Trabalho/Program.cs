using CLUSA;
using CLUSA.Services;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace Trabalho
{
    internal static class Program
    {
        [STAThread]
        static void Main(string[] args) 
        {
            ApplicationConfiguration.Initialize();

            if (args != null && args.Contains("--rotina-leitesol"))
            {
                try
                {
                    var minhaLogo = Trabalho.Properties.Resources.FollowUpLogo;
                    var service = new FollowUpService(minhaLogo);
                    service.ExecutarFluxoAutomaticoAsync("LEITESOL").GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    if (!Environment.CommandLine.Contains("--rotina-leitesol")){ MessageBox.Show($"Erro: {ex.Message}");}
                    Console.WriteLine($"[ERRO FATAL]: {ex.Message}");
                    throw;
                }
                return;
            }
            if (args != null && args.Contains("--rotina-casaflora"))
            {
                try
                {
                    var minhaLogo = Trabalho.Properties.Resources.FollowUpLogo;
                    var service = new FollowUpService(minhaLogo);
                    service.ExecutarFluxoAutomaticoAsync("CASA FLORA").GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    if (!Environment.CommandLine.Contains("--rotina-casaflora")){ MessageBox.Show($"Erro: {ex.Message}");}
                    Console.WriteLine($"[ERRO FATAL]: {ex.Message}");
                    throw;
                }
                return;
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FrmLogin());
        }
    }
}
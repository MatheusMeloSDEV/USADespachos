using CLUSA;
using MongoDB.Bson.Serialization;

namespace Trabalho
{
    internal static class Program
    {
        [STAThread]
        static void Main() // Mude para 'void', não 'async Task'
        {
            // Linhas essenciais para inicializar o WinForms corretamente.
            // ApplicationConfiguration.Initialize() é o método moderno para .NET 6+
            ApplicationConfiguration.Initialize();

            // Inicia a aplicação e abre o formulário de login.
            Application.Run(new FrmLogin());
        }
    }
}
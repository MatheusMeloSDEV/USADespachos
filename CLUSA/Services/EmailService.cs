using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CLUSA.Services
{
    public static class EmailService
    {
        // CONFIGURAÇÕES DO SEU E-MAIL
        private static string SmtpHost = "smtp.gmail.com";
        private static int SmtpPort = 587;
        private static string EmailRemetente = "matheusmvsj@gmail.com";
        private static string SenhaRemetente = "vuvs ybrj kcuw sgum";
        private static string EmailDestinatario = "fernando@usadespachos.com.br";

        public static async Task EnviarEmailAsync(string assunto, string corpo)
        {
            using (var smtp = new SmtpClient(SmtpHost, SmtpPort))
            {
                // A ordem importa!
                smtp.UseDefaultCredentials = false; // Desativa credenciais padrão
                smtp.Credentials = new NetworkCredential(EmailRemetente, SenhaRemetente); // Define as novas
                smtp.EnableSsl = true; // Gmail exige SSL

                var mail = new MailMessage();
                mail.From = new MailAddress(EmailRemetente, "Sistema de Vencimentos");
                mail.To.Add(EmailDestinatario);
                mail.Subject = assunto;
                mail.Body = corpo;

                await smtp.SendMailAsync(mail);
            }
        }
    }
}
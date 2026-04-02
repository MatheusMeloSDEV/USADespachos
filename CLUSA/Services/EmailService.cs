using CLUSA.Helpers;
using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CLUSA.Services
{
    public static class EmailService
    {
        // Auxiliar para decidir entre Nuvem (Environment) ou Local (EmailConfig)
        private static string GetVal(string envVar, string localVal)
        {
            var env = Environment.GetEnvironmentVariable(envVar);
            if (!string.IsNullOrEmpty(env)) return env;

            #if GITHUB_ACTIONS
                return ""; // No GitHub retorna vazio se não achar a Secret
            #else
                return localVal; // No seu PC, usa o valor que vem do EmailConfig
            #endif
        }

        // --- CONFIGURAÇÕES GERAIS (GMAIL) ---
        private const string SmtpHost = "smtp.gmail.com";
        private const int SmtpPort = 587;
        private const bool SmtpSsl = false; 

        // --- PROPRIEDADES DINÂMICAS (PERFIL 1: VENCIMENTOS) ---
        private static string Venc_Remetente => GetVal("VENC_USER", CLUSA.Helpers.EmailConfig.Venc_User);
        private static string Venc_Senha => GetVal("VENC_PASS", CLUSA.Helpers.EmailConfig.Venc_Pass);
        private static string Venc_Destinatario => GetVal("VENC_DEST", CLUSA.Helpers.EmailConfig.Venc_Dest);
        private static string Venc_Copia => GetVal("VENC_CC", CLUSA.Helpers.EmailConfig.Venc_Cc);
        private static string Venc_Bcc => GetVal("VENC_BCC", CLUSA.Helpers.EmailConfig.Venc_Bcc);

        // --- PROPRIEDADES DINÂMICAS (PERFIL 2: FOLLOW-UP) ---
        private static string Follow_Remetente => GetVal("FOLLOW_USER", CLUSA.Helpers.EmailConfig.Follow_User);
        private static string Follow_Host => GetVal("FOLLOW_HOST", CLUSA.Helpers.EmailConfig.Follow_Host);
        private static int Follow_Port => int.Parse(GetVal("FOLLOW_PORT", EmailConfig.Follow_Port.ToString()));
        private static string Follow_Senha => GetVal("FOLLOW_PASS", CLUSA.Helpers.EmailConfig.Follow_Pass);
        private static string Follow_Destinatario => GetVal("FOLLOW_DEST", CLUSA.Helpers.EmailConfig.Follow_Dest);
        private static string Follow_Cc => GetVal("FOLLOW_CC", CLUSA.Helpers.EmailConfig.Follow_Cc);
        private static string Follow_Bcc => GetVal("FOLLOW_BCC", CLUSA.Helpers.EmailConfig.Follow_Bcc);

        private static bool UseSsl => Follow_Port == 465;

        // =========================================================================
        // MÉTODOS DE ENVIO
        // =========================================================================

        public static async Task EnviarNotificacaoVencimentoAsync(string assunto, string corpo)
        {
            await EnviarEmailComMailKitAsync(
                SmtpHost, SmtpPort, SmtpSsl, "USA Despachos (Avisos)",
                Venc_Remetente, Venc_Senha, Venc_Destinatario, Venc_Copia, Venc_Bcc,
                assunto, corpo, null, null);
        }

        public static async Task EnviarFollowUpTextoAsync(string assunto, string corpo)
        {
            await EnviarEmailComMailKitAsync(
                SmtpHost, SmtpPort, SmtpSsl, "USA Despachos",
                Follow_Remetente, Follow_Senha, Follow_Destinatario, Follow_Cc, Follow_Bcc,
                assunto, corpo, null, null);
        }

        public static async Task EnviarFollowUpAsync(string assunto, string corpo, byte[] anexoPdf, string nomeArquivoPdf)
        {
            await EnviarEmailComMailKitAsync(
                Follow_Host,
                Follow_Port,
                UseSsl, // Agora ele decide se é true ou false baseado na porta
                "USA Despachos",
                Follow_Remetente, Follow_Senha, Follow_Destinatario, Follow_Cc, Follow_Bcc,
                assunto, corpo, anexoPdf, nomeArquivoPdf);
        }

        // =========================================================================
        // MOTOR DE ENVIO (PRIVADO)
        // =========================================================================
        private static async Task EnviarEmailComMailKitAsync(
            string host, int port, bool useSsl, string nomeExibicao,
            string remetente, string senha, string destinatario,
            string emailsCopia, string emailOculto, // Parâmetros atualizados
            string assunto, string corpo, byte[] anexoBytes, string nomeAnexo)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(nomeExibicao, remetente));
                message.To.Add(new MailboxAddress("", destinatario));

                // LOGICA PARA MÚLTIPLOS CC
                if (!string.IsNullOrWhiteSpace(emailsCopia))
                {
                    // Divide a string por vírgula e remove espaços extras
                    var listaCc = emailsCopia.Split(',').Select(e => e.Trim());
                    foreach (var email in listaCc)
                    {
                        if (!string.IsNullOrEmpty(email))
                            message.Cc.Add(new MailboxAddress("", email));
                    }
                }

                // LOGICA PARA BCC (Cópia Oculta)
                if (!string.IsNullOrWhiteSpace(emailOculto))
                {
                    // A mesma "mágica" da vírgula: divide, limpa os espaços e adiciona à lista
                    var listaBcc = emailOculto.Split(',').Select(e => e.Trim());

                    foreach (var email in listaBcc)
                    {
                        if (!string.IsNullOrEmpty(email))
                        {
                            message.Bcc.Add(new MailboxAddress("", email));
                        }
                    }
                }

                message.Subject = assunto;

                var bodyBuilder = new BodyBuilder { HtmlBody = corpo ?? string.Empty };

                if (anexoBytes != null && anexoBytes.Length > 0)
                    bodyBuilder.Attachments.Add(nomeAnexo, anexoBytes, ContentType.Parse("application/pdf"));

                message.Body = bodyBuilder.ToMessageBody();

                using (var client = new SmtpClient())
                {
                    // Ignora erros de certificado (comum em servidores corporativos)
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                    await client.ConnectAsync(host, port, useSsl);
                    await client.AuthenticateAsync(remetente, senha);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }
            }
            catch (Exception ex)
            {
                // Mostra erro apenas se estiver rodando com interface (Windows Forms)
                if (Environment.UserInteractive)
                {
                    MessageBox.Show($"Erro no e-mail: {ex.Message}", "Erro de E-mail", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                throw; // Lança o erro para o GitHub Actions registrar no log dele
            }
        }
    }
}
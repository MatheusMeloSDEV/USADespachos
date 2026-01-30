using System;
using System.Drawing;
using System.Windows.Forms;

namespace Trabalho
{
    // Coloque isso fora da classe FrmModificaProcesso, mas dentro do namespace Trabalho
    public class FrmDialogoSucesso : Form
    {
        public bool EnviarEmail { get; private set; }
        private CheckBox chkEnviarEmail;
        private Button btnOk;
        private Label lblMensagem;
        private PictureBox iconBox;

        public FrmDialogoSucesso(bool permitirFollowUp)
        {
            // Configuração básica do Form
            this.Text = "Sucesso";
            this.Size = new Size(400, 180);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Ícone (usando o ícone de informação do sistema)
            iconBox = new PictureBox();
            iconBox.Image = SystemIcons.Information.ToBitmap();
            iconBox.Location = new Point(20, 20);
            iconBox.Size = new Size(32, 32);
            this.Controls.Add(iconBox);

            // Mensagem
            lblMensagem = new Label();
            lblMensagem.Text = "Processo salvo com sucesso!";
            lblMensagem.AutoSize = true;
            lblMensagem.Location = new Point(70, 25);
            lblMensagem.Font = new Font("Segoe UI", 10);
            this.Controls.Add(lblMensagem);

            // CheckBox
            chkEnviarEmail = new CheckBox();
            chkEnviarEmail.Text = "Enviar Follow-Up para o importador?";
            chkEnviarEmail.AutoSize = true;
            chkEnviarEmail.Location = new Point(70, 60);
            chkEnviarEmail.Enabled = permitirFollowUp; // Só habilita se o histórico mudou

            if (!permitirFollowUp)
            {
                chkEnviarEmail.Text += " (Sem alteração no histórico)";
                chkEnviarEmail.ForeColor = Color.Gray;
            }

            this.Controls.Add(chkEnviarEmail);

            // Botão OK
            btnOk = new Button();
            btnOk.Text = "OK";
            btnOk.DialogResult = DialogResult.OK;
            btnOk.Location = new Point(280, 100);
            this.Controls.Add(btnOk);

            this.AcceptButton = btnOk;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            EnviarEmail = chkEnviarEmail.Checked;
        }
    }
}
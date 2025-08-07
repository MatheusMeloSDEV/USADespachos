using CLUSA;
using Timer = System.Windows.Forms.Timer;

namespace Trabalho
{
    public partial class FrmModificaAdmin : Form
    {
        private readonly Users _user;
        private readonly Timer _errorTimer;

        public FrmModificaAdmin(Users user)
        {
            InitializeComponent();

            _user = user ?? throw new ArgumentNullException(nameof(user));

            // Configura o Timer de erro
            _errorTimer = new Timer { Interval = 3000 };
            _errorTimer.Tick += ErrorTimer_Tick;

            // Vincula os eventos
            this.Load += FrmModificaAdmin_Load;
            btnEnviar.Click += BtnEnviar_Click;
        }

        private void FrmModificaAdmin_Load(object sender, EventArgs e)
        {
            // Ícone
            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

            // Popula os controles com os valores atuais
            txtUsername.Text = _user.Username;
            txtPassword.Text = _user.Password;
            cbAdmin.Checked = _user.Admin;
        }

        private void BtnEnviar_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs())
            {
                // Se passou na validação, atualiza o objeto e fecha com OK
                _user.Username = txtUsername.Text.Trim();
                _user.Password = txtPassword.Text;
                _user.Admin = cbAdmin.Checked;
                this.DialogResult = DialogResult.OK;
            }
        }

        /// <summary>
        /// Verifica se username e senha estão preenchidos.
        /// Destaca o fundo em vermelho e exibe mensagem caso falte.
        /// </summary>
        private bool ValidateInputs()
        {
            bool hasError = false;

            // Restaura cores e para qualquer timer anterior
            txtUsername.BackColor = Color.White;
            txtPassword.BackColor = Color.White;
            _errorTimer.Stop();

            // Verifica cada campo
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                txtUsername.BackColor = Color.MistyRose;
                hasError = true;
            }
            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                txtPassword.BackColor = Color.MistyRose;
                hasError = true;
            }

            if (hasError)
            {
                MessageBox.Show("Preencha todos os dados.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _errorTimer.Start();
            }

            return hasError;
        }

        /// <summary>
        /// Evento do timer: restaura o fundo branco após o intervalo de erro.
        /// </summary>
        private void ErrorTimer_Tick(object sender, EventArgs e)
        {
            txtUsername.BackColor = Color.White;
            txtPassword.BackColor = Color.White;
            _errorTimer.Stop();
        }
    }
}

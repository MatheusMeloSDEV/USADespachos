using CLUSA.Services;
using CLUSA.Repositories;
using CLUSA.Models;

namespace Trabalho
{
    public partial class FrmLogin : Form
    {
        private RepositorioUsers _repositorio;
        public static FrmLogin Instance { get; private set; } = null!;
        public Logado Logado { get; private set; } = null!;
        public bool Escuro { get; private set; } = false;

        private const string CaminhoArquivo = @"C:\UsaDespachos\Log.txt";

        public FrmLogin()
        {
            InitializeComponent();
            Instance = this;
            _repositorio = new RepositorioUsers();
        }

        private async void FrmLogin_Load(object sender, EventArgs e)
        {
            Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            _repositorio = new RepositorioUsers();
            await VerificarAtualizacoesAsync();
        }

        private bool CarregarLoginLocal()
        {
            try
            {
                if (File.Exists(CaminhoArquivo))
                {
                    string conteudo = File.ReadAllText(CaminhoArquivo);
                    // Supondo formato: usuario|senha
                    string[] dados = conteudo.Split('|');

                    if (dados.Length >= 2)
                    {
                        txtUsername.Text = dados[0];
                        txtPassword.Text = dados[1];
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                // Silencioso ou log de erro, para não travar a abertura
                Console.WriteLine($"Erro ao ler login local: {ex.Message}");
            }
            return false;
        }
        private void SalvarLoginLocal(string usuario, string senha)
        {
            try
            {
                // Garante que a pasta existe
                string diretorio = Path.GetDirectoryName(CaminhoArquivo)!;
                if (!Directory.Exists(diretorio))
                {
                    Directory.CreateDirectory(diretorio);
                }

                // Salva no formato: usuario|senha
                File.WriteAllText(CaminhoArquivo, $"{usuario}|{senha}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Não foi possível salvar o login localmente: {ex.Message}");
            }
        }
        private async Task VerificarAtualizacoesAsync()
        {
            var atualizador = new AtualizadorGithub(
                "https://api.github.com/repos/MatheusMeloSDEV/Trabalho",
                "atualizacao.zip"
            );

            bool atualizarAgora = false;

            atualizador.AtualizacaoDisponivel += (nova, atual) =>
            {
                var result = MessageBox.Show(
                    $"Versão atual: {atual}\nNova versão disponível: {nova}\nDeseja atualizar agora?",
                    "Atualização disponível",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    atualizarAgora = true;
                }
            };

            atualizador.DownloadConcluido += path =>
            {
                MessageBox.Show($"Download concluído: {path}\nO programa será encerrado para atualizar.");
                Application.Exit();
            };

            atualizador.Erro += msg =>
            {
                MessageBox.Show($"Erro: {msg}");
            };

            try
            {
                if (AtualizadorGithub.TemConexaoInternet())
                {
                    await atualizador.VerificarAtualizacaoAsync();
                }
                else
                {
                    MessageBox.Show("Sem conexão com a internet.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao verificar atualizações: {ex.Message}");
            }

            if (atualizarAgora)
            {
                this.Enabled = false;
                await atualizador.BaixarEInstalarAppAsync();
            }
        }
        private void BtnLogin_Click(object sender, EventArgs e)
        {
            var user = new Users
            {
                Username = txtUsername.Text,
                Password = txtPassword.Text
            };

            btnLogin.Visible = false;

            try
            {
                Logado = _repositorio.Login(user);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnLogin.Visible = true;
                return;
            }

            if (Logado.log)
            {
                if (CbLembrar.Checked)
                {
                    SalvarLoginLocal(user.Username, user.Password);
                }
                HandleSuccessfulLogin();
            }
            else
            {
                HandleLoginError();
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            if (Instance == this)
            {
                Instance = null!;
            }
        }

        private void HandleSuccessfulLogin()
        {
            HideLoginControls();
            ShowLoginFeedback();

            var menuForm = new FrmPrincipal(Logado);
            menuForm.ShowDialog();

            if (menuForm.DialogResult == DialogResult.OK)
            {
                ShowLoginScreen();
            }
        }

        private void HandleLoginError()
        {
            lblError.Visible = true;
            btnLogin.Visible = true;

            tErro.Interval = 3000;
            tErro.Tick += (s, e) =>
            {
                tErro.Stop();
                lblError.Visible = false;
            };
            tErro.Start();
        }

        private void HideLoginControls()
        {
            lblPassword.Visible = false;
            txtPassword.Visible = false;
            lblUsername.Visible = false;
            txtUsername.Visible = false;
            btnLogin.Visible = false;
            lblError.Visible = false;
        }

        private void ShowLoginFeedback()
        {
            check.Visible = true;
            lblLogado.Visible = true;

            tLogado.Interval = 3000;
            tLogado.Tick += TimerLogado_Tick;
            tLogado.Start();
        }

        private void ShowLoginScreen()
        {
            Show();

            if (!CarregarLoginLocal())
            {
                txtPassword.Clear();
                txtUsername.Clear();
            }

            CarregarLoginLocal();

            lblPassword.Visible = true;
            txtPassword.Visible = true;
            lblUsername.Visible = true;
            txtUsername.Visible = true;
            btnLogin.Visible = true;
        }

        private void TimerLogado_Tick(object? sender, EventArgs e)
        {
            if (sender == null)
            {
                // Lide com o caso em que 'sender' é nulo, se necessário.
                return;
            }

            tLogado.Stop();
            check.Visible = false;
            lblLogado.Visible = false;
            this.Hide();
        }
        private void BtnFechar_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void SetThemeColors(Color backgroundColor, Color inputBackgroundColor)
        {
            BackColor = backgroundColor;
            lblError.BackColor = backgroundColor;
            txtPassword.BackColor = inputBackgroundColor;
            txtUsername.BackColor = inputBackgroundColor;
            btnFechar.BackColor = inputBackgroundColor;
            btnLogin.BackColor = inputBackgroundColor;
        }

        private void FrmLogin_Shown(object sender, EventArgs e)
        {
            ShowLoginScreen();
        }
    }
}

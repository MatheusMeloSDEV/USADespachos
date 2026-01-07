using CLUSA.Repositories;
using CLUSA.Services;
using CLUSA.Models;
using MongoDB.Driver;
using System.Data;
using System.Drawing.Imaging;

namespace Trabalho
{
    public partial class FrmPrincipal : Form
    {
        #region Campos e Construtor

        private readonly RepositorioProcesso _repositorioProcesso;
        private readonly RepositorioNotificacao _notificacaoRepo;
        private readonly RepositorioNotifUrgente _repoNotificacoesUrgentes;
        private readonly RepositorioUsers _repositorioUsers;
        private readonly NotificacaoService _NotificacaoService;

        private readonly Logado _logadoUsuario;
        private readonly Dictionary<Type, Form> _forms = new();
        private bool _logoutPeloMenu = false;
        private readonly HashSet<TabPage> _abasJaCarregadas = new HashSet<TabPage>();

        public FrmPrincipal(Logado logadoUsuario)
        {
            InitializeComponent();
            var client = new MongoClient(ConfigDatabase.MongoConnectionString);
            var database = client.GetDatabase(ConfigDatabase.MongoDatabaseName);

            _logadoUsuario = logadoUsuario ?? throw new ArgumentNullException(nameof(logadoUsuario));

            MenuItemUsuario.Text = _logadoUsuario.Usuario;
            _repositorioProcesso = new RepositorioProcesso();
            _repositorioUsers = new RepositorioUsers(database);

            // Injeção de Dependência dos Repositórios e Gerenciador
            _notificacaoRepo = new RepositorioNotificacao(database);
            _repoNotificacoesUrgentes = new RepositorioNotifUrgente(database);
            _NotificacaoService = new NotificacaoService(database);

            // Configuração do Timer de Sincronização
            _notificacaoTimer = new System.Windows.Forms.Timer();
            _notificacaoTimer.Interval = 30000; // 30 segundos
            _notificacaoTimer.Tick += NotificacaoTimer_Tick;

            if (pictureBox1.Image != null)
            {
                pictureBox1.Image = SetImageOpacity(pictureBox1.Image, 0.2f);
            }
            panel1.Visible = true; pictureBox1.Visible = true;

            this.Shown += FrmPrincipal_Shown;
        }

        #endregion

        #region Eventos Principais do Formulário e Timer

        private async void FrmPrincipal_Shown(object? sender, EventArgs e)
        {
            await CarregarDadosProcessos();
            _notificacaoTimer.Start();
        }

        private async void FrmPrincipal_Load(object sender, EventArgs e)
        {
            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            GridColumnManager.RegistrarCatalogosPadrao();
            await PopularTableLayoutUrgentes();
        }

        private void FrmPrincipal_FormClosing(object sender, FormClosingEventArgs e)
        {
            _notificacaoTimer.Stop();

            if (!_logoutPeloMenu && e.CloseReason == CloseReason.UserClosing)
            {
                Application.Exit();
            }
        }

        private async void NotificacaoTimer_Tick(object? sender, EventArgs e)
        {
            // 1. PAUSA O TIMER IMEDIATAMENTE
            // Isso impede que ele dispare de novo se a conexão estiver lenta
            _notificacaoTimer.Stop();

            try
            {
                // 2. Proteção Geral (Para não travar a UI se a internet cair)
                // Se der erro aqui, o usuário nem percebe, apenas pula esse ciclo

                // A. Limpeza (Rápido)
                await _NotificacaoService.ExcluirNotificacoesAntigasAsync(DateTime.Now.AddDays(-90));

                // B. A parte pesada (Sincronização)
                var processosMonitorados = await _repositorioProcesso.ListarProcessosAtivosParaStatusAsync();

                // Aqui dentro deve ter aquele try/catch individual que conversamos antes
                await SincronizarTodasNotificacoes(processosMonitorados);

                // C. Atualiza UI
                await AtualizarContadorNotificacoesMenu();

                // Só recarrega a lista visual se o menu estiver aberto (Economiza consulta)
                if (contextMenuStripNotifications.Visible)
                {
                    await PopularContextMenuNotifications();
                }
            }
            catch (Exception ex)
            {
                // Log silencioso (Console ou Arquivo) para não incomodar o usuário com popups a cada 30s
                System.Diagnostics.Debug.WriteLine($"Erro no Timer de Notificação: {ex.Message}");
            }
            finally
            {
                // 3. RETOMA O TIMER
                // Independente se deu certo ou erro, volta a contar 30 segundos a partir de AGORA
                _notificacaoTimer.Start();
            }
        }

        #endregion

        #region Lógica de Sincronização e Carregamento

        private async Task CarregarDadosProcessos()
        {
            Cursor = Cursors.WaitCursor;
            try
            {
                // CORREÇÃO: Usa o método otimizado com projeção, pois aqui só precisamos dos dados
                // para gerar notificações, não precisamos do objeto inteiro pesado.
                var processos = await _repositorioProcesso.ListarProcessosAtivosParaStatusAsync();

                await SincronizarTodasNotificacoes(processos);
                await PopularContextMenuNotifications();
                await AtualizarContadorNotificacoesMenu();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar processos: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private async Task SincronizarTodasNotificacoes(List<Processo> processos)
        {
            foreach (var p in processos)
            {
                await _NotificacaoService.SincronizarNotificacoesDoProcessoAsync(p);
            }
        }

        private async Task AtualizarContadorNotificacoesMenu()
        {
            int totalNaoVisualizadas = await _notificacaoRepo.ContarNaoVisualizadasAsync();
            MenuItemNotifications.Text = totalNaoVisualizadas > 0
               ? $"Notificações ({totalNaoVisualizadas})"
               : "Notificações";
        }

        #endregion

        #region UI de Notificações e ContextMenu

        private async void menuItemNotificacoes_Click(object sender, EventArgs e)
        {
            await AtualizarContadorNotificacoesMenu();

            await PopularContextMenuNotifications();
            var parent = Menu;
            var menuLocation = parent.PointToScreen(MenuItemNotifications.Bounds.Location);
            contextMenuStripNotifications.Show(menuLocation.X, menuLocation.Y + MenuItemNotifications.Height);
        }

        private int notificacoesLimite = 20;
        private int notificacoesSkip = 0;

        private async Task PopularContextMenuNotifications()
        {
            contextMenuStripNotifications.Items.Clear();

            var pendentes = await _notificacaoRepo.ObterNotificacoesNaoVisualizadasAsync(notificacoesLimite, notificacoesSkip);
            int totalNaoVisualizadas = await _notificacaoRepo.ContarNaoVisualizadasAsync();

            if (notificacoesSkip > 0)
            {
                var btnVoltar = new ToolStripMenuItem("Voltar...");
                btnVoltar.Click += async (s, e) =>
                {
                    notificacoesSkip = Math.Max(notificacoesSkip - notificacoesLimite, 0);
                    await PopularContextMenuNotifications();
                };
                contextMenuStripNotifications.Items.Add(btnVoltar);
            }

            foreach (var notif in pendentes)
            {
                var itemMenu = new ToolStripMenuItem
                {
                    Text = notif.Mensagem ?? "[Mensagem vazia]",
                    Tag = notif.RefUsa
                };

                itemMenu.MouseDown += async (sender, e) =>
                {
                    if (e.Button == MouseButtons.Right && sender is ToolStripMenuItem menuItem && menuItem.Tag is string refUsa)
                    {
                        var originalColor = menuItem.BackColor;
                        menuItem.BackColor = Color.LightGreen;
                        await Task.Delay(300);

                        try
                        {
                            await _notificacaoRepo.MarcarComoVisualizadoAsync(refUsa, menuItem.Text);
                            await PopularContextMenuNotifications();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Erro ao finalizar notificação: {ex.Message}", "Erro");
                            menuItem.BackColor = originalColor;
                        }
                    }
                };

                contextMenuStripNotifications.Items.Add(itemMenu);
            }

            if (totalNaoVisualizadas > notificacoesSkip + notificacoesLimite)
            {
                var btnMais = new ToolStripMenuItem($"Ver mais... ({totalNaoVisualizadas - notificacoesSkip - notificacoesLimite} restantes)");
                btnMais.Click += async (s, e) =>
                {
                    notificacoesSkip += notificacoesLimite;
                    await PopularContextMenuNotifications();
                };
                contextMenuStripNotifications.Items.Add(btnMais);
            }

            contextMenuStripNotifications.Items.Add(new ToolStripSeparator());
            var fecharItem = new ToolStripMenuItem("Fechar Notificações");
            fecharItem.Click += (s, e) => contextMenuStripNotifications.Close();
            contextMenuStripNotifications.Items.Add(fecharItem);
        }

        #endregion

        #region Gerenciamento de Janelas e UI Auxiliar

        private TableLayoutPanel CriarTabela() => new TableLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, ColumnCount = 1, AutoScroll = true };
        private Label CriarLabel(string texto) => new Label { AutoSize = true, Font = new Font("Segoe UI", 10F), Margin = new Padding(5), Text = texto };

        private void MenuItemHome_Click(object? sender, EventArgs e)
        {

        }

        private T? ShowSingleFormOfType<T>(Func<T> factory, bool maximizar = true) where T : Form
        {
            panel1.Visible = false;

            if (_forms.TryGetValue(typeof(T), out var formExistente) && !formExistente.IsDisposed)
            {
                formExistente.WindowState = FormWindowState.Normal;
                formExistente.Activate();
                return (T)formExistente;
            }

            foreach (var f in MdiChildren) f.Close();
            _forms.Clear();

            try
            {
                var novoForm = factory(); // cria via factory, podendo passar parâmetros
                novoForm.MdiParent = this;
                novoForm.WindowState = maximizar ? FormWindowState.Maximized : FormWindowState.Normal;
                novoForm.AutoScroll = true;

                novoForm.FormClosed += (s, args) => _forms.Remove(typeof(T));
                novoForm.Show();
                _forms[typeof(T)] = novoForm;
                return novoForm;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao abrir o formulário {typeof(T).Name}:\n{ex.Message}",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        private void MenuItemExit_Click(object? sender, EventArgs e)
        {
            _logoutPeloMenu = true;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void MenuItemProcessoSantos_Click(object? sender, EventArgs e)
        => ShowSingleFormOfType(() => new frmSantos(_logadoUsuario));
        private void MenuItemProcessosItajai_Click(object? sender, EventArgs e)
        => ShowSingleFormOfType(() => new FrmItajaí(_logadoUsuario));
        private void MenuItemOrgaoAnuente_Click(object? sender, EventArgs e)
        => ShowSingleFormOfType(() => new FrmOrgaoAnuente(_logadoUsuario));
        private void MenuItemVistoria_Click(object sender, EventArgs e)
        => ShowSingleFormOfType(() => new FrmVistorias(_logadoUsuario));
        private void MenuItemEmAndamento_Click(object? sender, EventArgs e)
        => ShowSingleFormOfType(() => new FrmStatusProcessos(_logadoUsuario));
        private void MenuItemFinalizados_Click(object? sender, EventArgs e)
        => ShowSingleFormOfType(() => new frmFinalizados(_logadoUsuario));
        private void MenuItemFinanceiro_Click(object? sender, EventArgs e)
        => ShowSingleFormOfType(() => new FrmFinanceiro());
        private void MenuItemAdmin_Click(object? sender, EventArgs e)
        => ShowSingleFormOfType(() => new FrmAdmin());
        private void MenuItemVencimentos_Click(object sender, EventArgs e)
        => ShowSingleFormOfType(() => new FrmVencimentos());
        private void MenuItemMaximize_Click(object? sender, EventArgs e) => this.WindowState = FormWindowState.Maximized;
        private void MenuItemMinimize_Click(object? sender, EventArgs e) => this.WindowState = FormWindowState.Minimized;
        private async void MenuItemConfiguracoes_Click(object sender, EventArgs e)
        {
            try
            {
                var usuario = await _repositorioUsers.GetByIdAsync(_logadoUsuario.Id);
                if (usuario == null)
                {
                    MessageBox.Show("Erro ao carregar dados do usuário.", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                using var frmConfig = new frmConfiguracoes(usuario.Id, usuario.PreferenciasGrids);
                frmConfig.ShowDialog(this);

                // Sempre recarrega preferências depois de fechar
                usuario.PreferenciasGrids = frmConfig.ObterPreferencias();
                await _repositorioUsers.UpdateAsync(usuario);

                MessageBox.Show(
                    "Configurações salvas com sucesso!\n\nAs alterações serão aplicadas ao reabrir os grids.",
                    "Sucesso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao abrir configurações:\n{ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private async void BtnAddNotifUrg_Click(object sender, EventArgs e)
        {
            var usuariosDestino = (await _repositorioUsers.FindAllAsync())
                .Where(u => u.Id != _logadoUsuario.Id && u.Username != "admin")
                .Select(u => new UsuarioDestinoItem { Id = u.Id, NomeUsuario = u.Username })
                .ToList();

            var frm = new FrmAddNotifUrgente(_logadoUsuario.Id, usuariosDestino);
            if (frm.ShowDialog() == DialogResult.OK && frm.IdDestinoSelecionado.HasValue)
            {
                var notif = new NotifUrgente
                {
                    UsuarioOrigemId = _logadoUsuario.Id,
                    UsuarioDestinoId = frm.IdDestinoSelecionado.Value,
                    Mensagem = frm.MensagemCriada,
                    DataEnvio = DateTime.Now,
                    Done = false
                };
                await _repoNotificacoesUrgentes.InsertAsync(notif);
                await PopularTableLayoutUrgentes();
            }
        }

        private async Task PopularTableLayoutUrgentes()
        {
            TLNotifUrgentes.Controls.Clear();
            TLNotifUrgentes.RowCount = 0;
            TLNotifUrgentes.ColumnCount = 1;
            TLNotifUrgentes.RowStyles.Clear();

            var todosUsuarios = await _repositorioUsers.FindAllAsync();
            var lookupNome = todosUsuarios.ToDictionary(u => u.Id, u => u.Username);

            var minhas = await _repoNotificacoesUrgentes.GetByUsuarioOrigemAsync(_logadoUsuario.Id);
            var recebidas = await _repoNotificacoesUrgentes.GetByUsuarioDestinoAsync(_logadoUsuario.Id);

            var todas = minhas.Concat(recebidas)
                .GroupBy(n => n.Id)
                .Select(g => g.First())
                .Where(n => !n.Done)
                .OrderBy(n => n.DataEnvio)
                .ToList();

            for (int i = 0; i < todas.Count; i++)
            {
                var n = todas[i];

                string nomeDestino = lookupNome.TryGetValue(n.UsuarioDestinoId, out var nomeDest) ? nomeDest : n.UsuarioDestinoId.ToString();
                string nomeOrigem = lookupNome.TryGetValue(n.UsuarioOrigemId, out var nomeOrig) ? nomeOrig : n.UsuarioOrigemId.ToString();

                var notifControl = new NotificacaoUrgente
                {
                    Usuario = n.UsuarioOrigemId == _logadoUsuario.Id
                        ? $"Enviada para: {nomeDestino}"
                        : $"De: {nomeOrigem}",
                    Mensagem = n.Mensagem,
                    MensagemReadOnly = true,
                    BotaoEditarVisible = n.UsuarioOrigemId == _logadoUsuario.Id
                };

                notifControl.BtnExcluir.Visible = n.UsuarioOrigemId == _logadoUsuario.Id;
                notifControl.ExcluirClick += async (s, e) =>
                {
                    var confirma = MessageBox.Show("Tem certeza que deseja excluir esta notificação?", "Confirmação", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (confirma == DialogResult.Yes)
                    {
                        await _repoNotificacoesUrgentes.DeleteAsync(n.Id);
                        await PopularTableLayoutUrgentes();
                    }
                };

                notifControl.DoneClick += async (s, e) =>
                {
                    n.Done = true;
                    await _repoNotificacoesUrgentes.UpdateAsync(n);
                    await PopularTableLayoutUrgentes();
                };

                notifControl.EditClick += async (s, e) =>
                {
                    notifControl.MensagemReadOnly = false;
                    notifControl.FocusMensagem();
                };

                notifControl.MensagemEditada += async (s, novaMensagem) =>
                {
                    n.Mensagem = novaMensagem;
                    await _repoNotificacoesUrgentes.UpdateAsync(n);
                    notifControl.MensagemReadOnly = true;
                    MessageBox.Show("Mensagem atualizada com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await PopularTableLayoutUrgentes();
                };

                TLNotifUrgentes.RowStyles.Add(new RowStyle(SizeType.Absolute, notifControl.Height));
                TLNotifUrgentes.Controls.Add(notifControl, 0, i);
            }
        }

        private void lblEmAndamento_Click(object sender, EventArgs e)
        {

        }

        public static Bitmap SetImageOpacity(Image image, float opacity)
        {
            Bitmap bmp = new Bitmap(image.Width, image.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                ColorMatrix matrix = new ColorMatrix();
                matrix.Matrix33 = opacity;
                ImageAttributes attributes = new ImageAttributes();
                attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                g.DrawImage(image, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes);
            }
            return bmp;
        }

        private void MenuItemHome_DoubleClick(object sender, EventArgs e)
        {

        }

        private void MenuItemMenu_Click(object sender, EventArgs e)
        {
            foreach (var f in MdiChildren) f.Close();
            _forms.Clear();
            panel1.Visible = true; pictureBox1.Visible = true;
        }

        private void MenuItemChangePassword_Click(object sender, EventArgs e)
        {
            var frm = new FrmMudarSenha(_logadoUsuario);
            frm.ShowDialog();
        }
    }
    #endregion
}
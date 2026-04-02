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
        private readonly RepositorioLog _repositorioLog;
        private readonly NotificacaoService _NotificacaoService;

        private readonly FrmVencimentos EmailService;

        private bool _atualizandoInterface = false; // <--- ADICIONE ISSO
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
            _repositorioLog = new RepositorioLog();

            // Configuração do Timer de Sincronização
            _notificacaoTimer = new System.Windows.Forms.Timer();
            _notificacaoTimer.Interval = 30000; // 30 segundos
            _notificacaoTimer.Tick += NotificacaoTimer_Tick;

            EmailService = new FrmVencimentos();

            if (pictureBox1.Image != null)
            {
                pictureBox1.Image = SetImageOpacity(pictureBox1.Image, 0.2f);
            }
            panel1.Visible = true; pictureBox1.Visible = true;
        }

        #endregion

        #region Eventos Principais do Formulário e Timer

        private async void FrmPrincipal_Shown(object? sender, EventArgs e)
        {
            await PopularTableLayoutUrgentes(); // Garante a primeira carga
            await CarregarDadosProcessos();
            await EmailService.VerificarNotificacoesAutomaticas(); // Verifica vencimentos assim que o sistema abre
            _notificacaoTimer.Start();

            // Só começa a escutar o Alt+Tab DEPOIS que o formulário já abriu completamente
            this.Activated -= FrmPrincipal_Activated; // Garante que não duplica inscritos
            this.Activated += FrmPrincipal_Activated;
        }

        // Crie este método separado para ficar organizado
        private async void FrmPrincipal_Activated(object? sender, EventArgs e)
        {
            await PopularTableLayoutUrgentes();
        }

        private async void FrmPrincipal_Load(object sender, EventArgs e)
        {
            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            GridColumnManager.RegistrarCatalogosPadrao();

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
        => ShowSingleFormOfType(() => new FrmVencimentos() { _logadoNome = _logadoUsuario.Usuario });
        private void MenuItemMaximize_Click(object? sender, EventArgs e) => this.WindowState = FormWindowState.Maximized;
        private void MenuItemMinimize_Click(object? sender, EventArgs e) => this.WindowState = FormWindowState.Minimized;
        private async void MenuItemConfiguracoes_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Busca os dados mais atualizados do usuário uma única vez
                var usuario = await _repositorioUsers.GetByIdAsync(_logadoUsuario.Id);
                if (usuario == null)
                {
                    MessageBox.Show("Erro ao carregar dados do usuário.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // 2. Abre a tela passando as informações direto no construtor
                using var frmConfig = new frmConfiguracoes(usuario.Id, usuario.PreferenciasGrids, usuario.Username);

                // 3. Apenas salva no banco se o usuário confirmou no botão "Salvar"
                if (frmConfig.ShowDialog(this) == DialogResult.OK)
                {
                    usuario.PreferenciasGrids = frmConfig.ObterPreferencias();
                    await _repositorioUsers.UpdateAsync(usuario);

                    MessageBox.Show(
                        "Configurações salvas com sucesso!\n\nAs alterações serão aplicadas ao reabrir as telas.",
                        "Sucesso",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                // Se cair fora do 'if', significa que ele fechou a janela no 'X', então o banco fica intacto!
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao abrir configurações:\n{ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            if (_atualizandoInterface) return;
            _atualizandoInterface = true;

            TLNotifUrgentes.SuspendLayout();

            try
            {
                // 1. Limpeza
                while (TLNotifUrgentes.Controls.Count > 0)
                {
                    var c = TLNotifUrgentes.Controls[0];
                    TLNotifUrgentes.Controls.RemoveAt(0);
                    c.Dispose();
                }

                TLNotifUrgentes.RowStyles.Clear();
                TLNotifUrgentes.RowCount = 0;

                // 2. Busca e Unifica Dados
                var todasMinhas = await _repoNotificacoesUrgentes.GetByUsuarioOrigemAsync(_logadoUsuario.Id);
                var todasRecebidas = await _repoNotificacoesUrgentes.GetByUsuarioDestinoAsync(_logadoUsuario.Id);

                var users = await _repositorioUsers.FindAllAsync();
                var userDict = users.ToDictionary(u => u.Id, u => u.Username);

                var listaUnica = todasMinhas.Concat(todasRecebidas)
                                            .GroupBy(n => n.Id)
                                            .Select(g => g.First())
                                            .Where(n => !n.Done)
                                            .OrderByDescending(n => n.DataEnvio)
                                            .ToList();

                // 3. Adiciona os Itens
                foreach (var n in listaUnica)
                {
                    string nomeOrigem = userDict.TryGetValue(n.UsuarioOrigemId, out var uO) ? uO : "...";
                    string nomeDestino = userDict.TryGetValue(n.UsuarioDestinoId, out var uD) ? uD : "...";

                    var item = new NotificacaoUrgente
                    {
                        Dock = DockStyle.Fill,
                        Usuario = (n.UsuarioOrigemId == _logadoUsuario.Id) ? $"Para: {nomeDestino}" : $"De: {nomeOrigem}",
                        Mensagem = n.Mensagem,
                        MensagemReadOnly = true,
                        BotaoEditarVisible = (n.UsuarioOrigemId == _logadoUsuario.Id)
                    };

                    // Configurações visuais
                    item.BtnExcluir.Visible = (n.UsuarioOrigemId == _logadoUsuario.Id);

                    // Evento EXCLUIR (Lixeira) - Mantive igual, apenas deleta
                    item.ExcluirClick += async (s, e) =>
                    {
                        if (MessageBox.Show("Deseja realmente excluir esta notificação?", "Confirmação", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            await _repoNotificacoesUrgentes.DeleteAsync(n.Id);
                            _atualizandoInterface = false;
                            await PopularTableLayoutUrgentes();
                        }
                    };

                    // --- AQUI ESTÁ A ALTERAÇÃO SOLICITADA (Botão Check/Done) ---
                    item.DoneClick += async (s, e) =>
                    {
                        try
                        {
                            // 1. Registra o Log
                            await _repositorioLog.RegistrarLogAsync(
                                "Conclusão", _logadoUsuario.Usuario,
                                $"Notificação finalizada",
                                $"Mensagem: {n.Mensagem} | De: {nomeOrigem} | Para: {nomeDestino}"
                            );

                            // 2. Deleta do Banco de Dados (ao invés de dar Update n.Done=true)
                            await _repoNotificacoesUrgentes.DeleteAsync(n.Id);

                            // 3. Feedback Visual e Atualização
                            MessageBox.Show("Notificação concluída e arquivada.", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            _atualizandoInterface = false; // Destrava
                            await PopularTableLayoutUrgentes(); // Recarrega a lista
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Erro ao concluir: {ex.Message}");
                            _atualizandoInterface = false;
                        }
                    };
                    // -----------------------------------------------------------

                    item.EditClick += (s, e) => { item.MensagemReadOnly = false; item.FocusMensagem(); };
                    item.MensagemEditada += async (s, txt) =>
                    {
                        n.Mensagem = txt; await _repoNotificacoesUrgentes.UpdateAsync(n);
                        item.MensagemReadOnly = true; MessageBox.Show("Atualizado!");
                    };

                    TLNotifUrgentes.RowCount++;
                    TLNotifUrgentes.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                    TLNotifUrgentes.Controls.Add(item, 0, TLNotifUrgentes.RowCount - 1);
                }
            }
            finally
            {
                TLNotifUrgentes.ResumeLayout(true);
                _atualizandoInterface = false;
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
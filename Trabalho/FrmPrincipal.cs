using CLUSA;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Trabalho
{
    public partial class FrmPrincipal : Form
    {
        private readonly RepositorioProcesso _repositorioProcesso;
        private readonly RepositorioNotificacao _notificacaoRepo;
        private readonly RepositorioNotifUrgente _repoNotificacoesUrgentes;
        private readonly RepositorioUsers _repositorioUsers;

        private readonly Logado _logadoUsuario;
        private readonly Dictionary<Type, Form> _forms = new();
        private bool _logoutPeloMenu = false;

        private readonly HashSet<TabPage> _abasJaCarregadas = new HashSet<TabPage>();

        public FrmPrincipal(Logado logadoUsuario)
        {
            InitializeComponent();
            _logadoUsuario = logadoUsuario ?? throw new ArgumentNullException(nameof(logadoUsuario));


            MenuItemUsuario.Text = _logadoUsuario.Usuario;
            _repositorioProcesso = new RepositorioProcesso();
            _repositorioUsers = new RepositorioUsers();

            var client = new MongoClient(ConfigDatabase.MongoConnectionString);
            var database = client.GetDatabase(ConfigDatabase.MongoDatabaseName);
            _notificacaoRepo = new RepositorioNotificacao(database);
            _repoNotificacoesUrgentes = new RepositorioNotifUrgente(database);

            if (pictureBox1.Image != null)
            {
                pictureBox1.Image = SetImageOpacity(pictureBox1.Image, 0.2f);
            }
            panel1.Visible = true; pictureBox1.Visible = true;
            // Assinatura dos eventos
            this.Shown += FrmPrincipal_Shown;

        }


        #region "Eventos Principais do Formulário"

        private async void FrmPrincipal_Shown(object? sender, EventArgs e)
        {
            await CarregarDadosProcessos();


        }

        private async void FrmPrincipal_Load(object sender, EventArgs e)
        {
            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            await PopularTableLayoutUrgentes();
        }

        private void FrmPrincipal_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_logoutPeloMenu && e.CloseReason == CloseReason.UserClosing)
            {
                Application.Exit();
            }
        }
        #endregion

        #region "Carregamento de Dados e Abas (Lógica Otimizada)"

        private async Task CarregarDadosProcessos()
        {
            Cursor = Cursors.WaitCursor;
            try
            {
                var processos = await _repositorioProcesso.ListarTodosAsync();

                await GerarNotificacoes(processos);

                await PopularContextMenuNotifications();

                // <<< Atualiza o texto do menu de notificações >>>
                int totalNaoVisualizadas = await _notificacaoRepo.ContarNaoVisualizadasAsync();
                MenuItemNotifications.Text = totalNaoVisualizadas > 0
                    ? $"Notificações ({totalNaoVisualizadas})"
                    : "Notificações";
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

        #endregion

        #region "Lógica de Notificações (Otimizada)"

        private async void menuItemNotificacoes_Click(object sender, EventArgs e)
        {
            // Antes de abrir o painel, atualize o contador
            int totalNaoVisualizadas = await _notificacaoRepo.ContarNaoVisualizadasAsync();

            if (totalNaoVisualizadas > 0)
                MenuItemNotifications.Text = $"Notificações ({totalNaoVisualizadas})";
            else
                MenuItemNotifications.Text = $"Notificações";

            // Popula e mostra o ContextMenuStrip
            await PopularContextMenuNotifications();
            var parent = Menu;
            var menuLocation = parent.PointToScreen(MenuItemNotifications.Bounds.Location);
            contextMenuStripNotifications.Show(menuLocation.X, menuLocation.Y + MenuItemNotifications.Height);
        }



        public async Task GerarNotificacoes(List<Processo> processos)
        {
            var refsUsa = processos.Select(p => p.Ref_USA).Distinct().ToList();
            var notificacoesExistentes = await _notificacaoRepo.ObterNaoVisualizadasPorProcessosAsync(refsUsa, 500);
            var lookupExistentes = notificacoesExistentes.Select(n => $"{n.RefUsa}|{n.Mensagem}").ToHashSet();
            var novasNotificacoes = new List<Notificacao>();

            foreach (var p in processos)
            {
                if (p.DataDeAtracacao.HasValue)
                {
                    int dias = (p.DataDeAtracacao.Value - DateTime.Today).Days;
                    if (dias is >= 0 and <= 15)
                        TentarAdicionarNotificacao(novasNotificacoes, lookupExistentes, p.Ref_USA, $"Processo {p.Ref_USA}: Dar entrada no Mapa/Anvisa");
                    if (dias is >= 0 and <= 5)
                        TentarAdicionarNotificacao(novasNotificacoes, lookupExistentes, p.Ref_USA, $"Processo {p.Ref_USA}: Redestinar container ao terminal");
                }
                VerificarVencimento(p, p.VencimentoFreeTime, "Free Time", novasNotificacoes, lookupExistentes);
                VerificarVencimento(p, p.VencimentoFMA, "FMA", novasNotificacoes, lookupExistentes);
                VerificarVencimento(p, p.VencimentoLI_LPCO, "LI/LPCO", novasNotificacoes, lookupExistentes);
            }

            if (novasNotificacoes.Any())
            {
                await _notificacaoRepo.InsertManyAsync(novasNotificacoes);
            }
        }


        private void VerificarVencimento(Processo doc, DateTime? vencimento, string nomeExibicao, List<Notificacao> lista, HashSet<string> lookup)
        {
            if (!vencimento.HasValue) return;
            int dias = (vencimento.Value - DateTime.Today).Days;
            if (dias is >= 0 and <= 5)
            {
                string msg = $"Processo {doc.Ref_USA}: {nomeExibicao} vence em {dias} dia(s)";
                TentarAdicionarNotificacao(lista, lookup, doc.Ref_USA, msg);
            }
        }

        private void TentarAdicionarNotificacao(List<Notificacao> listaNovas, HashSet<string> lookup, string refUsa, string mensagem)
        {
            var chave = $"{refUsa}|{mensagem}";
            if (!lookup.Contains(chave))
            {
                listaNovas.Add(new Notificacao { RefUsa = refUsa, Mensagem = mensagem, DataCriacao = DateTime.Now, Visualizado = false });
                lookup.Add(chave);
            }
        }


        /// <summary>
        /// Busca as notificações não visualizadas no banco e atualiza o menu de notificações na tela.
        /// </summary>
        private int notificacoesLimite = 20;
        private int notificacoesSkip = 0;

        private async Task PopularContextMenuNotifications()
        {
            contextMenuStripNotifications.Items.Clear();

            var pendentes = await _notificacaoRepo.ObterNotificacoesNaoVisualizadasAsync(notificacoesLimite, notificacoesSkip);
            int totalNaoVisualizadas = await _notificacaoRepo.ContarNaoVisualizadasAsync();

            // Paginação: Botão "Voltar" se não está na primeira página
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

            // Adicione as notificações
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

            // Botão "Ver mais..." se houver mais notificações
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

        #region "Métodos Auxiliares de UI"

        private TableLayoutPanel CriarTabela() => new TableLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, ColumnCount = 1, AutoScroll = true };
        private Label CriarLabel(string texto) => new Label { AutoSize = true, Font = new Font("Segoe UI", 10F), Margin = new Padding(5), Text = texto };
        #endregion

        #region "Gerenciamento de Janelas MDI e Eventos de Menu"

        private void MenuItemHome_Click(object? sender, EventArgs e)
        {

        }

        private T? ShowSingleFormOfType<T>(bool maximizar = true) where T : Form, new()
        {
            panel1.Visible = false;
            if (_forms.TryGetValue(typeof(T), out var form) && !form.IsDisposed)
            {
                form.WindowState = FormWindowState.Normal;
                form.Activate();
                return (T)form;
            }

            foreach (var f in MdiChildren) f.Close();
            _forms.Clear();

            try
            {
                var novoForm = new T
                {
                    MdiParent = this,
                    WindowState = maximizar ? FormWindowState.Maximized : FormWindowState.Normal,
                    AutoScroll = true
                };
                novoForm.FormClosed += (s, args) => _forms.Remove(typeof(T));
                novoForm.Show();
                _forms[typeof(T)] = novoForm;
                return novoForm;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao abrir o formulário {typeof(T).Name}:\n{ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        private void MenuItemExit_Click(object? sender, EventArgs e)
        {
            _logoutPeloMenu = true;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void MenuItemProcessoSantos_Click(object? sender, EventArgs e) => ShowSingleFormOfType<frmSantos>();
        private void MenuItemProcessosItajai_Click(object? sender, EventArgs e) => ShowSingleFormOfType<FrmItajaí>();
        private void MenuItemOrgaoAnuente_Click(object? sender, EventArgs e) => ShowSingleFormOfType<FrmOrgaoAnuente>();
        private void MenuItemVistoria_Click(object sender, EventArgs e) => ShowSingleFormOfType<FrmVistorias>();
        private void MenuItemFinanceiro_Click(object? sender, EventArgs e) => ShowSingleFormOfType<FrmFinanceiro>();
        private void MenuItemAdmin_Click(object? sender, EventArgs e) => ShowSingleFormOfType<FrmAdmin>();
        private void MenuItemEmAndamento_Click(object? sender, EventArgs e) => ShowSingleFormOfType<FrmStatusProcessos>();
        private void MenuItemMaximize_Click(object? sender, EventArgs e) => this.WindowState = FormWindowState.Maximized;
        private void MenuItemMinimize_Click(object? sender, EventArgs e) => this.WindowState = FormWindowState.Minimized;

        #endregion

        #region "Notificações Urgentes"

        private async void BtnAddNotifUrg_Click(object sender, EventArgs e)
        {
            var usuariosDestino = (await _repositorioUsers.FindAllAsync())
                .Where(u => u.Id != _logadoUsuario.Id)
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
                .GroupBy(n => n.Id) // Use o campo único do modelo
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
                    // Torna mensagem editável
                    notifControl.MensagemReadOnly = false;
                    notifControl.FocusMensagem(); // Adicione método público no UserControl
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




        #endregion
        private void lblEmAndamento_Click(object sender, EventArgs e)
        {

        }

        public static Bitmap SetImageOpacity(Image image, float opacity)
        {
            Bitmap bmp = new Bitmap(image.Width, image.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                ColorMatrix matrix = new ColorMatrix();
                matrix.Matrix33 = opacity; // valor entre 0 (totalmente transparente) e 1 (totalmente opaco)
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
}

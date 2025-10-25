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

        private readonly Logado _logadoUsuario;
        private readonly Dictionary<Type, Form> _forms = new();
        private bool _logoutPeloMenu = false;

        private readonly HashSet<TabPage> _abasJaCarregadas = new HashSet<TabPage>();

        public FrmPrincipal(Logado logadoUsuario)
        {
            InitializeComponent();
            _logadoUsuario = logadoUsuario ?? throw new ArgumentNullException(nameof(logadoUsuario));

            // Inicializa os repositórios
            _repositorioProcesso = new RepositorioProcesso();

            // O RepositorioProcesso já cria o RepositorioNotificacao, podemos pegá-lo de lá
            // ou criar uma instância separada se preferir.
            var client = new MongoClient(ConfigDatabase.MongoConnectionString);
            var database = client.GetDatabase(ConfigDatabase.MongoDatabaseName);
            _notificacaoRepo = new RepositorioNotificacao(database);

            if (pictureBox1.Image != null)
            {
                pictureBox1.Image = SetImageOpacity(pictureBox1.Image, 0.2f); // 50% de transparência
            }
            panel1.Visible = true; pictureBox1.Visible = true;
            // Assinatura dos eventos
            this.Shown += FrmPrincipal_Shown;
            //TCabas.SelectedIndexChanged += TCabas_SelectedIndexChanged; // <-- Evento para carregar abas sob demanda
        }


        #region "Eventos Principais do Formulário"

        private async void FrmPrincipal_Shown(object? sender, EventArgs e)
        {
            // O formulário já está visível, agora carregamos os dados sem travar.
            await CarregarDadosProcessos();
        }

        private void FrmPrincipal_Load(object sender, EventArgs e)
        {
            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            //TCabas.Visible = false; // Esconde as abas até os dados serem carregados
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


        /// <summary>
        /// Cria as abas rapidamente, sem preencher o conteúdo.
        /// </summary>
        //private void MontarTabsProcessos(List<Processo> processos)
        //{
        //    TCabas.SuspendLayout();
        //    TCabas.TabPages.Clear();
        //    _abasJaCarregadas.Clear();

        //    // Adiciona as 3 abas principais
        //    TCabas.TabPages.Add(new TabPage("Data de Atracação") { Name = "DataDeAtracacao", Tag = processos });
        //    TCabas.TabPages.Add(new TabPage("Órgãos Anuentes") { Name = "OrgaoAnuentes", Tag = processos });
        //    TCabas.TabPages.Add(new TabPage("Finalizados") { Name = "Finalizados", Tag = processos });

        //    TCabas.ResumeLayout();

        //    // Força o carregamento da primeira aba visível
        //    if (TCabas.TabPages.Count > 0)
        //    {
        //        TCabas_SelectedIndexChanged(TCabas, EventArgs.Empty);
        //    }
        //}

        /// <summary>
        /// Evento disparado QUANDO o usuário clica em uma aba.
        /// </summary>
        //private void TCabas_SelectedIndexChanged(object? sender, EventArgs e)
        //{
        //    if (TCabas.SelectedTab == null) return;
        //    var abaSelecionada = TCabas.SelectedTab;

        //    // Se o conteúdo desta aba já foi criado, não faz nada.
        //    if (_abasJaCarregadas.Contains(abaSelecionada)) return;

        //    // Se for a primeira vez, cria o conteúdo.
        //    if (abaSelecionada.Tag is List<Processo> processos)
        //    {
        //        PopularAbaComControles(abaSelecionada, processos);
        //        _abasJaCarregadas.Add(abaSelecionada);
        //    }
        //}

        /// <summary>
        /// O "trabalho pesado": cria os controles para UMA aba específica.
        /// </summary>
        //private void PopularAbaComControles(TabPage aba, List<Processo> processos)
        //{
        //    Cursor = Cursors.WaitCursor;
        //    aba.SuspendLayout();

        //    var table = CriarTabela();
        //    aba.Controls.Add(table);

        //    // Filtra a lista de processos com base na aba clicada
        //    List<string> textosParaLabels = new List<string>();
        //    switch (aba.Name)
        //    {
        //        case "DataDeAtracacao":
        //            textosParaLabels = processos.Where(p => p.Status != "Finalizado")
        //                .Select(p => $"{p.Ref_USA} — {p.SR} — {(p.DataDeAtracacao.HasValue ? p.DataDeAtracacao.Value.ToString("dd/MM/yyyy") : "N/A")}").ToList();
        //            break;

        //        case "OrgaoAnuentes":
        //            textosParaLabels = processos.Where(p => p.Status != "Finalizado")
        //               .Select(p => $"{p.Ref_USA} — {p.Importador} — {p.OrgaosAnuentesString}").ToList();
        //            break;

        //        case "Finalizados":
        //            textosParaLabels = processos.Where(p => p.Status == "Finalizado")
        //                .Select(p => $"{p.Ref_USA} — Finalizado em: {(p.DataCarregamentoDI.HasValue ? p.DataCarregamentoDI.Value.ToString("dd/MM/yyyy") : "N/A")}").ToList();
        //            break;
        //    }

        //    table.Controls.AddRange(textosParaLabels.Select(CriarLabel).ToArray());

        //    aba.ResumeLayout();
        //    Cursor = Cursors.Default;
        //}

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

        private T? ShowSingleFormOfType<T>() where T : Form, new()
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
                    WindowState = FormWindowState.Maximized
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
            this.Close();
            FrmLogin.Instance?.Show();
        }

        private void MenuItemProcessoSantos_Click(object? sender, EventArgs e) => ShowSingleFormOfType<frmSantos>();
        private void MenuItemProcessosItajai_Click(object? sender, EventArgs e) => ShowSingleFormOfType<FrmItajaí>();
        private void MenuItemOrgaoAnuente_Click(object? sender, EventArgs e) => ShowSingleFormOfType<FrmOrgaoAnuente>();
        private void MenuItemVistoria_Click(object sender, EventArgs e) => ShowSingleFormOfType<FrmVistorias>();
        private void MenuItemFinanceiro_Click(object? sender, EventArgs e) => ShowSingleFormOfType<FrmFinanceiro>();
        private void MenuItemAdmin_Click(object? sender, EventArgs e) => ShowSingleFormOfType<FrmAdmin>();
        private void MenuItemMaximize_Click(object? sender, EventArgs e) => this.WindowState = FormWindowState.Maximized;
        private void MenuItemMinimize_Click(object? sender, EventArgs e) => this.WindowState = FormWindowState.Minimized;

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
    }
}

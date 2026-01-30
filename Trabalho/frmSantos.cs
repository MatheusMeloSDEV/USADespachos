using CLUSA;
using CLUSA.Repositories;
using CLUSA.Services;
using MongoDB.Driver;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using CLUSA.Models;

namespace Trabalho
{
    public partial class frmSantos : Form
    {
        private readonly RepositorioProcesso _repositorio;
        private int _estadoOrdenacaoRefUsa = 0;
        private DataGridViewColumn? _colunaOrdenada;
        private ListSortDirection _direcaoOrdenacao;
        private List<Processo> _listaOriginal = new();

        private FrmLoadingOverlay? _overlay;
        private readonly RepositorioUsers _repositorioUsers;
        private readonly RepositorioLog _logRepo;
        private Users? _usuarioLogado;
        private readonly Logado _logado;

        public frmSantos(Logado logado)
        {
            InitializeComponent();

            // --- OTIMIZAÇÃO VISUAL (Double Buffering) ---
            // Isso evita que o Grid pisque ou fique lento ao rolar
            typeof(DataGridView).InvokeMember("DoubleBuffered",
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty,
                null, DGVSantos, new object[] { true });

            _repositorio = new RepositorioProcesso();
            _repositorioUsers = new RepositorioUsers();
            _logRepo = new RepositorioLog();
            _logado = logado;
            this.Shown += FrmSantos_Shown;
        }

        private async void FrmSantos_Shown(object? sender, EventArgs e)
        {
            try
            {
                GridColumnManager.RegistrarCatalogosPadrao();
                _usuarioLogado = await _repositorioUsers.GetByIdAsync(_logado.Id);

                List<string>? colunasVisiveis = null;
                if (_usuarioLogado?.PreferenciasGrids != null)
                {
                    _usuarioLogado.PreferenciasGrids.TryGetValue("DGVSantos", out colunasVisiveis);
                }

                GridColumnManager.ConfigurarGrid(DGVSantos, "DGVSantos", colunasVisiveis);
                GridColumnManager.ConfigurarFormatacaoListas(DGVSantos);

                await CarregarDadosAsync();

                this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
                PopularComboBoxDePesquisa();

                if (CmbPesquisar.Items.Count > 0) CmbPesquisar.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar o formulário: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private async Task CarregarDadosAsync()
        {
            try
            {
                DGVSantos.SuspendLayout(); // Pausa o desenho do Grid para ganhar velocidade

                // --- BUSCA OTIMIZADA ---
                // O filtro "Status != Finalizado" agora é feito no banco, economizando rede e memória.
                var registros = await _repositorio.ListarPrincipalOtimizadoAsync("ITJ");

                // Ordenação Inicial na Memória (Ref_USA vazios para o fim, depois data)
                var registrosOrdenados = registros
                    .OrderBy(p => p.DataDeAtracacao == null || p.DataDeAtracacao == DateTime.MinValue ? 1 : 0)
                    .ThenBy(p => p.DataDeAtracacao ?? DateTime.MaxValue)
                    .ToList();

                _listaOriginal = registrosOrdenados;

                BsProcesso.DataSource = registrosOrdenados;
                DGVSantos.DataSource = BsProcesso;
                BsProcesso.ResetBindings(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar os dados: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                DGVSantos.ResumeLayout(); // Libera o desenho do Grid
            }
        }

        private void PopularComboBoxDePesquisa()
        {
            var camposIgnorados = new HashSet<string> { "Id", "OrgaosAnuentesEnvolvidos", "PossuiEmbarque", "VencimentoFreeTime", "VencimentoFMA" };
            CmbPesquisar.Items.Clear();

            foreach (DataGridViewColumn coluna in DGVSantos.Columns)
            {
                if (!string.IsNullOrEmpty(coluna.DataPropertyName) && !camposIgnorados.Contains(coluna.DataPropertyName))
                {
                    CmbPesquisar.Items.Add(new DisplayItem(coluna.DataPropertyName, coluna.HeaderText));
                }
            }
        }

        private Dictionary<string, string[]> _cacheAutoComplete = new();

        private async Task ConfigurarAutoCompletarParaPesquisaAsync()
        {
            if (CmbPesquisar.SelectedItem is not DisplayItem campoSelecionado) return;
            string campo = campoSelecionado.DataPropertyName;

            try
            {
                // Uso de CACHE para evitar ir ao banco a cada letra digitada
                if (!_cacheAutoComplete.ContainsKey(campo))
                {
                    var valores = await _repositorio.ObterValoresUnicosAsync(campo);
                    _cacheAutoComplete[campo] = valores.ToArray();
                }

                var collection = new AutoCompleteStringCollection();
                collection.AddRange(_cacheAutoComplete[campo]);

                if (TxtPesquisar.AutoCompleteCustomSource != collection)
                {
                    TxtPesquisar.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                    TxtPesquisar.AutoCompleteSource = AutoCompleteSource.CustomSource;
                    TxtPesquisar.AutoCompleteCustomSource = collection;
                }
            }
            catch (Exception ex)
            {
                // Falha silenciosa ou log simples para não atrapalhar o usuário
                Debug.WriteLine($"Erro autocomplete: {ex.Message}");
            }
        }
        private async void BtnAdicionar_Click(object sender, EventArgs e)
        {
            var processo = new Processo();
            OrigemProcesso Santos = OrigemProcesso.Santos;
            using var frm = new FrmModificaProcesso { processo = processo, Modo = "Adicionar", Origem = Santos };

            if (frm.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    await _repositorio.CreateAsync(processo);
                    _cacheAutoComplete.Clear(); // Limpa cache pois entrou dado novo
                    await CarregarDadosAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao adicionar: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private async void BtnExcluir_Click(object sender, EventArgs e)
        {
            if (BsProcesso.Current is not Processo processoSelecionado) return;

            if (MessageBox.Show($"Excluir '{processoSelecionado.Ref_USA}'?", "Confirmação", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    await _repositorio.DeleteAsync(processoSelecionado.Id.ToString());
                    await _logRepo.RegistrarLogAsync(
                        "Exclusão",
                        $"Processo {processoSelecionado.Ref_USA} excluído via Grid Santos",
                        $"Usuário: {_logado.Usuario}"
                    );
                    BsProcesso.Remove(processoSelecionado);
                    _cacheAutoComplete.Clear(); // Limpa cache
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao excluir: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private async void BtnEditar_Click(object? sender, EventArgs e)
        {
            if (BsProcesso.Current is not Processo processoSelecionado) return;

            using var frm = new FrmModificaProcesso { processo = processoSelecionado, Modo = "Editar" };
            frm.ShowDialog();

            _cacheAutoComplete.Clear(); // Limpa cache
            await CarregarDadosAsync();
        }

        private async void BtnPesquisar_Click(object sender, EventArgs e)
        {
            if (CmbPesquisar.SelectedItem is not DisplayItem campoSelecionado)
            {
                MessageBox.Show("Selecione um campo.", "Aviso");
                return;
            }

            var pesquisa = TxtPesquisar.Text;
            if (string.IsNullOrWhiteSpace(pesquisa))
            {
                BsProcesso.DataSource = _listaOriginal;
                BsProcesso.ResetBindings(false);
                return;
            }

            try
            {
                var resultados = await _repositorio.PesquisarAsync(campoSelecionado.DataPropertyName, pesquisa);
                BsProcesso.DataSource = resultados;
                BsProcesso.ResetBindings(false);

                if (!resultados.Any()) MessageBox.Show("Nenhum resultado.", "Info");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao pesquisar: {ex.Message}", "Erro");
            }
        }
        private async void DGVSantos_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || BsProcesso.Current is not Processo processoSelecionado) return;

            using var frm = new FrmModificaProcesso { processo = processoSelecionado, Visualização = true, Modo = "Visualizar" };
            frm.ShowDialog();
            await CarregarDadosAsync();
        }
        private async void BtnExportar_Click(object sender, EventArgs e)
        {
            // 1. Obtém a lista de importadores
            var importadores = await _repositorio.ObterValoresUnicosAsync("Importador");

            // 2. Exibe o formulário de seleção
            using var form = new ImporterSelectionForm(importadores);
            if (form.ShowDialog() == DialogResult.OK)
            {
                string importador = form.SelectedImporter;

                try
                {
                    MostrarLoading("Gerando documentos...");

                    var service = new CLUSA.Services.FollowUpService();

                    string pdfPath = await service.GerarArquivosEmDiscoAsync(importador);

                    EsconderLoading();

                    if (MessageBox.Show("Relatórios gerados com sucesso!\n\nDeseja abrir o PDF agora?",
                                        "Sucesso",
                                        MessageBoxButtons.YesNo,
                                        MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        // Abre o PDF gerado
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(pdfPath) { UseShellExecute = true });
                    }
                }
                catch (Exception ex)
                {
                    EsconderLoading();
                    MessageBox.Show($"Erro ao gerar documentos: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private async void CmbPesquisar_SelectedIndexChanged(object? sender, EventArgs e)
        {
            await ConfigurarAutoCompletarParaPesquisaAsync();
        }

        private async void BtnCancelar_Click(object sender, EventArgs e)
        {
            await CarregarDadosAsync();
            TxtPesquisar.Clear();
        }

        private void FrmProcesso_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                TxtPesquisar.PerformClick();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }


        public class DisplayItem
        {
            public string DataPropertyName { get; }
            public string HeaderText { get; }
            public DisplayItem(string dataPropertyName, string headerText)
            {
                DataPropertyName = dataPropertyName;
                HeaderText = headerText;
            }
            public override string ToString() => HeaderText;
        }
        private bool IsValueEmpty(object? value)
        {
            if (value == null || value == DBNull.Value)
            {
                return true;
            }
            if (value is string str)
            {
                return string.IsNullOrWhiteSpace(str);
            }
            return false;
        }
        private (int ano, int numero) ExtrairAnoNumero(string refUsa)
        {
            if (string.IsNullOrWhiteSpace(refUsa)) return (0, 0);
            string refLimpa = refUsa.Split(' ').FirstOrDefault() ?? refUsa;
            var partes = refLimpa.Split('/');
            int numero = 0, ano = 0;
            if (partes.Length == 2)
            {
                int.TryParse(partes[0], out numero);
                int.TryParse(partes[1], out ano);
            }
            return (ano, numero);
        }


        // Substitua seu método de clique no cabeçalho por este
        private void DGV_ColumnHeaderMouseClick(object? sender, DataGridViewCellMouseEventArgs e)
        {
            if (sender is not DataGridView dgv) return;
            var novaColuna = dgv.Columns[e.ColumnIndex];
            if (novaColuna.SortMode == DataGridViewColumnSortMode.NotSortable) return;
            if (BsProcesso.DataSource is not List<Processo> listaParaOrdenar) return;

            // 1. Determina a Direção da Ordenação (mesma lógica de antes)
            ListSortDirection direcao;
            if (_colunaOrdenada == null || _colunaOrdenada.Name != novaColuna.Name)
            {
                direcao = ListSortDirection.Ascending;
            }
            else
            {
                direcao = (_direcaoOrdenacao == ListSortDirection.Ascending)
                    ? ListSortDirection.Descending
                    : ListSortDirection.Ascending;
            }

            _colunaOrdenada = novaColuna;
            _direcaoOrdenacao = direcao;

            IEnumerable<Processo> listaOrdenada;

            // 2. Aplica a Lógica de Ordenação em Dois Níveis
            if (novaColuna.DataPropertyName == "Ref_USA")
            {
                // --- LÓGICA ESPECIAL PARA REF_USA ---
                var orderedByEmptiness = listaParaOrdenar
                    // NÍVEL 1: Jogar Ref_USA vazias para o final
                    .OrderBy(p => IsValueEmpty(p.Ref_USA) ? 1 : 0);

                listaOrdenada = direcao == ListSortDirection.Ascending
                    // NÍVEL 2: Ordenar as restantes pelo critério especial
                    ? orderedByEmptiness.ThenBy(p => ExtrairAnoNumero(p.Ref_USA))
                    : orderedByEmptiness.ThenByDescending(p => ExtrairAnoNumero(p.Ref_USA));
            }
            else
            {
                // --- LÓGICA GENÉRICA PARA OUTRAS COLUNAS ---
                var propInfo = typeof(Processo).GetProperty(novaColuna.DataPropertyName);
                if (propInfo == null) return;

                var orderedByEmptiness = listaParaOrdenar
                    // NÍVEL 1: Jogar valores vazios da coluna genérica para o final
                    .OrderBy(p => IsValueEmpty(propInfo.GetValue(p)) ? 1 : 0);

                // NÍVEL 2: Ordenar os valores restantes, com tratamento para datas
                if (propInfo.PropertyType == typeof(DateTime) || propInfo.PropertyType == typeof(DateTime?))
                {
                    listaOrdenada = direcao == ListSortDirection.Ascending
                        ? orderedByEmptiness.ThenBy(p => (DateTime?)propInfo.GetValue(p) ?? DateTime.MinValue)
                        : orderedByEmptiness.ThenByDescending(p => (DateTime?)propInfo.GetValue(p) ?? DateTime.MinValue);
                }
                else
                {
                    listaOrdenada = direcao == ListSortDirection.Ascending
                        ? orderedByEmptiness.ThenBy(p => propInfo.GetValue(p))
                        : orderedByEmptiness.ThenByDescending(p => propInfo.GetValue(p));
                }
            }

            // 3. Atualiza o DataGridView (mesma lógica de antes)
            BsProcesso.DataSource = listaOrdenada.ToList();
            BsProcesso.ResetBindings(false);

            // 4. Atualiza a Seta Visual (Glyph) no Cabeçalho (mesma lógica de antes)
            foreach (DataGridViewColumn column in dgv.Columns)
            {
                column.HeaderCell.SortGlyphDirection = (column.Name == novaColuna.Name)
                    ? (direcao == ListSortDirection.Ascending ? SortOrder.Ascending : SortOrder.Descending)
                    : SortOrder.None;
            }
        }

        private void BtnAjuda_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized)
                this.WindowState = FormWindowState.Normal;
            if (this.WindowState == FormWindowState.Normal)
                this.WindowState = FormWindowState.Maximized;
        }

        private void BtnDownloadTabela_Click(object sender, EventArgs e)
        {
            // 1. Validação básica
            if (DGVSantos.Rows.Count == 0)
            {
                MessageBox.Show("Não há dados na tabela para exportar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Lógica de seleção (Pergunta ao usuário se exporta tudo ou só a seleção)
            bool apenasSelecionadas = false;
            if (DGVSantos.SelectedRows.Count > 0)
            {
                var resp = MessageBox.Show(
                    $"Você tem {DGVSantos.SelectedRows.Count} linhas selecionadas.\nDeseja exportar APENAS a seleção?\n\n(Não = Exportar tudo)",
                    "Opções de Exportação",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);

                if (resp == DialogResult.Cancel) return;
                apenasSelecionadas = (resp == DialogResult.Yes);
            }

            // 3. Configura o Arquivo
            using var sfd = new SaveFileDialog();
            sfd.Filter = "Arquivo PDF (*.pdf)|*.pdf";
            sfd.FileName = $"Relatorio_Santos_{DateTime.Now:yyyyMMdd_HHmm}.pdf";

            if (sfd.ShowDialog() != DialogResult.OK) return;

            // 4. Executa a exportação usando o Serviço
            try
            {
                Cursor.Current = Cursors.WaitCursor; // Feedback visual simples

                // Chama a classe estática que criamos
                PdfExportService.ExportarGridParaPdf(
                    DGVSantos,
                    sfd.FileName,
                    "Relatório de Processos - Santos",
                    apenasSelecionadas
                );

                Cursor.Current = Cursors.Default;

                // 5. Log e Sucesso
                int qtdExportada = apenasSelecionadas ? DGVSantos.SelectedRows.Count : DGVSantos.Rows.Count;

                // Dispara o log sem travar a UI
                _ = Task.Run(() => _logRepo.RegistrarLogAsync(
                    "Exportação",
                    "Relatório PDF da tabela Santos gerado",
                    $"Usuário: {_logado.Usuario} | Registros: {qtdExportada}"
                ));

                if (MessageBox.Show("PDF gerado com sucesso! Deseja abrir agora?", "Sucesso",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    var p = new ProcessStartInfo(sfd.FileName) { UseShellExecute = true };
                    Process.Start(p);
                }
            }
            catch (IOException)
            {
                Cursor.Current = Cursors.Default;
                MessageBox.Show("O arquivo está aberto em outro programa. Feche-o e tente novamente.",
                    "Arquivo em Uso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                Cursor.Current = Cursors.Default;
                MessageBox.Show($"Erro ao gerar PDF: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void MostrarLoading(string mensagem)
        {
            if (_overlay != null) return;
            _overlay = new FrmLoadingOverlay { Opacity = 0.60 };
            _overlay.lblLoading.Text = mensagem;
            var rect = this.RectangleToScreen(this.ClientRectangle);
            _overlay.StartPosition = FormStartPosition.Manual;
            _overlay.Location = rect.Location;
            _overlay.Size = rect.Size;
            _overlay.Show(this);
            _overlay.BringToFront();
        }

        private void EsconderLoading()
        {
            _overlay?.Close();
            _overlay?.Dispose();
            _overlay = null;
        }
    }
}

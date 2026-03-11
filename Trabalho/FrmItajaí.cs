using CLUSA;
using CLUSA.Repositories;
using CLUSA.Services;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using CLUSA.Models;
using System.Reflection; // Necessário para o Double Buffered

namespace Trabalho
{
    public partial class FrmItajaí : Form
    {
        private readonly RepositorioProcesso _repositorio;
        private readonly RepositorioLog _repoLog;
        private DataGridViewColumn? _colunaOrdenada;
        private ListSortDirection _direcaoOrdenacao;
        private List<Processo> _listaOriginal = new();

        private FrmLoadingOverlay? _overlay;
        private readonly Logado _logado;
        private readonly RepositorioUsers _repositorioUsers;
        private Users? _usuarioLogado;

        private int _paginaAtual = 1;

        // --- MUDANÇA 1: Paginação Dinâmica ---
        private int _itensPorPagina = 50; // Deixou de ser const
        private bool _carregandoCombobox = true; // Trava para o Form Load

        private long _totalRegistros = 0;
        private Dictionary<string, string[]> _cacheAutoComplete = new(); // Cache da pesquisa

        public FrmItajaí(Logado logado)
        {
            InitializeComponent();

            // --- MUDANÇA 2: OTIMIZAÇÃO VISUAL (Double Buffering) ---
            typeof(DataGridView).InvokeMember("DoubleBuffered",
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty,
                null, DGVItajai, new object[] { true });

            _repositorio = new RepositorioProcesso();
            _repositorioUsers = new RepositorioUsers();
            _repoLog = new RepositorioLog();
            _logado = logado;
        }

        private async void FrmItajaí_Shown(object? sender, EventArgs e)
        {
            try
            {
                // 1) Carregar usuário e preferências
                _usuarioLogado = await _repositorioUsers.GetByIdAsync(_logado.Id);
                if (_usuarioLogado == null)
                {
                    MessageBox.Show("Não foi possível carregar o usuário logado.", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                GridColumnManager.RegistrarCatalogosPadrao();
                GridColumnManager.ConfigurarFormatacaoListas(DGVItajai);

                _usuarioLogado.PreferenciasGrids ??= new Dictionary<string, List<string>>();
                _usuarioLogado.PreferenciasGrids.TryGetValue("DGVItajai", out var colunasVisiveis);

                GridColumnManager.ConfigurarGrid(DGVItajai, "DGVItajai", colunasVisiveis);

                // --- MUDANÇA 3: Lê a preferência de paginação ---
                if (_usuarioLogado.ItensPorPagina > 0)
                {
                    _itensPorPagina = _usuarioLogado.ItensPorPagina;
                }

                string valorCombo = _itensPorPagina == int.MaxValue ? "Sem Limite" : _itensPorPagina.ToString();
                cbMaxRows.SelectedItem = valorCombo;
                _carregandoCombobox = false; // Libera o evento do combo

                await CarregarDadosAsync();

                this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
                PopularComboBoxDePesquisa();
                if (CmbPesquisar.Items.Count > 0)
                    CmbPesquisar.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar o formulário: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // --- MUDANÇA 4: CarregarDadosAsync agora mantém a seleção e ordenação visual ---
        private async Task CarregarDadosAsync()
        {
            try
            {
                MostrarLoading($"Carregando página {_paginaAtual}...");

                string campoBD = _colunaOrdenada?.DataPropertyName ?? "DataDeAtracacao";

                if (campoBD == "OrgaosAnuentesString" || string.IsNullOrWhiteSpace(campoBD))
                {
                    campoBD = "DataDeAtracacao";
                }

                bool isAsc = (_direcaoOrdenacao == ListSortDirection.Ascending);

                // Salva o item selecionado atualmente
                var idSelecionado = (BsProcesso.Current as Processo)?.Id;

                // Busca no banco
                var (itens, total) = await _repositorio.ListarPrincipalPaginadoAsync(
                    "Itajai",
                    _paginaAtual,
                    _itensPorPagina,
                    campoBD,
                    isAsc
                );

                _totalRegistros = total;
                _listaOriginal = itens;

                // Atualiza a Grid
                BsProcesso.DataSource = _listaOriginal;
                DGVItajai.DataSource = BsProcesso;
                BsProcesso.ResetBindings(false);

                // Restaura a seleção
                if (idSelecionado != null)
                {
                    var index = _listaOriginal.FindIndex(p => p.Id == idSelecionado);
                    if (index >= 0)
                    {
                        BsProcesso.Position = index;
                    }
                }

                // Restaura a setinha
                if (_colunaOrdenada != null)
                {
                    foreach (DataGridViewColumn col in DGVItajai.Columns)
                    {
                        if (col.Name == _colunaOrdenada.Name)
                            col.HeaderCell.SortGlyphDirection = isAsc ? SortOrder.Ascending : SortOrder.Descending;
                        else
                            col.HeaderCell.SortGlyphDirection = SortOrder.None;
                    }
                }

                AtualizarControlesPaginacao();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar dados: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                EsconderLoading();
            }
        }

        // --- MUDANÇA 5: Controles de paginação adaptados ---
        private void AtualizarControlesPaginacao()
        {
            if (_totalRegistros == 0)
            {
                lblQtd.Text = "0 registros";
                btnPrevious.Enabled = false;
                btnForward.Enabled = false;
                return;
            }

            if (_itensPorPagina == int.MaxValue)
            {
                lblQtd.Text = $"Mostrando todos os {_totalRegistros} registros";
                btnPrevious.Enabled = false;
                btnForward.Enabled = false;
                return;
            }

            int inicio = ((_paginaAtual - 1) * _itensPorPagina) + 1;
            int fim = inicio + _listaOriginal.Count - 1;

            lblQtd.Text = $"{inicio}-{fim} de {_totalRegistros}";

            btnPrevious.Enabled = _paginaAtual > 1;
            btnForward.Enabled = fim < _totalRegistros;
        }

        // --- MUDANÇA 6: Evento do ComboBox de paginação ---
        private async void cbMaxRows_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_carregandoCombobox || _usuarioLogado == null) return;

            string selecionado = cbMaxRows.SelectedItem?.ToString() ?? "50";

            if (selecionado == "Sem Limite")
            {
                _itensPorPagina = int.MaxValue;
            }
            else if (int.TryParse(selecionado, out int qtd))
            {
                _itensPorPagina = qtd;
            }

            try
            {
                _usuarioLogado.ItensPorPagina = _itensPorPagina;
                await _repositorioUsers.UpdateAsync(_usuarioLogado);

                _paginaAtual = 1;
                await CarregarDadosAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar sua preferência: {ex.Message}", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async void BtnForward_Click(object sender, EventArgs e)
        {
            _paginaAtual++;
            await CarregarDadosAsync();
        }

        private async void BtnPrevious_Click(object sender, EventArgs e)
        {
            if (_paginaAtual > 1)
            {
                _paginaAtual--;
                await CarregarDadosAsync();
            }
        }

        private void PopularComboBoxDePesquisa()
        {
            var camposIgnorados = new HashSet<string>
            {
                "Id", "OrgaosAnuentesEnvolvidos",
                "PossuiEmbarque", "VencimentoFreeTime", "VencimentoFMA"
            };

            CmbPesquisar.Items.Clear();

            foreach (DataGridViewColumn coluna in DGVItajai.Columns)
            {
                if (!string.IsNullOrEmpty(coluna.DataPropertyName) && !camposIgnorados.Contains(coluna.DataPropertyName))
                {
                    CmbPesquisar.Items.Add(new DisplayItem(coluna.DataPropertyName, coluna.HeaderText));
                }
            }
        }

        private async Task ConfigurarAutoCompletarParaPesquisaAsync()
        {
            if (CmbPesquisar.SelectedItem is not DisplayItem campoSelecionado) return;
            string campo = campoSelecionado.DataPropertyName;

            try
            {
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
                Debug.WriteLine($"Erro autocomplete: {ex.Message}");
            }
        }

        private async void BtnAdicionar_Click(object sender, EventArgs e)
        {
            var processo = new Processo();
            OrigemProcesso Itajai = OrigemProcesso.Itajai;
            using var frm = new FrmModificaProcesso { processo = processo, Modo = "Adicionar", Origem = Itajai, UsuarioLogado = _logado };

            if (frm.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    await _repositorio.CreateAsync(processo);
                    _cacheAutoComplete.Clear(); // Limpa cache
                    await CarregarDadosAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao adicionar o processo: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void BtnExcluir_Click(object sender, EventArgs e)
        {
            if (BsProcesso.Current is not Processo processoSelecionado)
            {
                MessageBox.Show("Nenhum processo selecionado para exclusão.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirmResult = MessageBox.Show($"Tem certeza que deseja excluir o processo '{processoSelecionado.Ref_USA}'?", "Confirmação", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmResult == DialogResult.Yes)
            {
                try
                {
                    await _repositorio.DeleteAsync(processoSelecionado.Id.ToString());
                    BsProcesso.Remove(processoSelecionado);
                    _cacheAutoComplete.Clear();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao excluir o processo: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            await CarregarDadosAsync();
        }

        // --- MUDANÇA 7: Editar com suporte a Pesquisa Inteligente ---
        private async void BtnEditar_Click(object? sender, EventArgs e)
        {
            if (BsProcesso.Current is not Processo processoSelecionado)
            {
                MessageBox.Show("Nenhum processo selecionado para edição.", "Aviso");
                return;
            }

            using var frm = new FrmModificaProcesso { processo = processoSelecionado, Modo = "Editar", UsuarioLogado = _logado };
            frm.ShowDialog();

            _cacheAutoComplete.Clear();

            if (!string.IsNullOrWhiteSpace(TxtPesquisar.Text))
            {
                await ExecutarPesquisaAsync();
            }
            else
            {
                await CarregarDadosAsync();
            }
        }

        private async void DGVItajai_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || BsProcesso.Current is not Processo processoSelecionado) return;

            using var frm = new FrmModificaProcesso { processo = processoSelecionado, Visualização = true, Modo = "Visualizar" };
            frm.ShowDialog();

            if (!string.IsNullOrWhiteSpace(TxtPesquisar.Text))
            {
                await ExecutarPesquisaAsync();
            }
            else
            {
                await CarregarDadosAsync();
            }
        }

        // --- MUDANÇA 8: Pesquisa Segura e Ordenação em Memória ---
        private void OrdenarListaEmMemoria()
        {
            if (_colunaOrdenada == null || BsProcesso.DataSource is not IEnumerable<Processo> listaPesquisa) return;

            var propInfo = typeof(Processo).GetProperty(_colunaOrdenada.DataPropertyName);
            if (propInfo == null) return;

            bool CampoEstaVazio(object valor)
            {
                if (valor == null) return true;
                if (valor is string texto && string.IsNullOrWhiteSpace(texto)) return true;
                return false;
            }

            if (_direcaoOrdenacao == ListSortDirection.Ascending)
            {
                BsProcesso.DataSource = listaPesquisa
                    .OrderBy(x => CampoEstaVazio(propInfo.GetValue(x)))
                    .ThenBy(x => propInfo.GetValue(x))
                    .ToList();
            }
            else
            {
                BsProcesso.DataSource = listaPesquisa
                    .OrderBy(x => CampoEstaVazio(propInfo.GetValue(x)))
                    .ThenByDescending(x => propInfo.GetValue(x))
                    .ToList();
            }
        }

        private async Task ExecutarPesquisaAsync()
        {
            if (CmbPesquisar.SelectedItem is not DisplayItem campoSelecionado) return;

            var pesquisa = TxtPesquisar.Text;
            if (string.IsNullOrWhiteSpace(pesquisa))
            {
                _paginaAtual = 1;
                await CarregarDadosAsync();
                return;
            }

            try
            {
                MostrarLoading("Pesquisando...");

                var resultados = await _repositorio.PesquisarAsync(campoSelecionado.DataPropertyName, pesquisa);

                resultados = resultados
                    .Where(p => !string.IsNullOrWhiteSpace(p.Ref_USA) && p.Ref_USA.EndsWith("ITJ", StringComparison.OrdinalIgnoreCase))
                    .ToList();

                BsProcesso.DataSource = resultados;

                OrdenarListaEmMemoria(); 

                BsProcesso.ResetBindings(false);

                lblQtd.Text = $"Encontrados: {resultados.Count}";
                btnForward.Enabled = false;
                btnPrevious.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro: {ex.Message}");
            }
            finally
            {
                EsconderLoading();
            }
        }

        private async void BtnPesquisar_Click(object sender, EventArgs e)
        {
            await ExecutarPesquisaAsync();
        }

        private async void DGV_ColumnHeaderMouseClick(object? sender, DataGridViewCellMouseEventArgs e)
        {
            if (sender is not DataGridView dgv) return;
            var novaColuna = dgv.Columns[e.ColumnIndex];

            if (novaColuna.SortMode == DataGridViewColumnSortMode.NotSortable || string.IsNullOrEmpty(novaColuna.DataPropertyName)) return;

            if (_colunaOrdenada != null && _colunaOrdenada.Name == novaColuna.Name)
            {
                _direcaoOrdenacao = (_direcaoOrdenacao == ListSortDirection.Ascending)
                    ? ListSortDirection.Descending
                    : ListSortDirection.Ascending;
            }
            else
            {
                _direcaoOrdenacao = ListSortDirection.Ascending;
            }
            _colunaOrdenada = novaColuna;

            foreach (DataGridViewColumn col in dgv.Columns)
            {
                if (col.Name == novaColuna.Name)
                    col.HeaderCell.SortGlyphDirection = (_direcaoOrdenacao == ListSortDirection.Ascending) ? SortOrder.Ascending : SortOrder.Descending;
                else
                    col.HeaderCell.SortGlyphDirection = SortOrder.None;
            }

            if (!string.IsNullOrWhiteSpace(TxtPesquisar.Text))
            {
                OrdenarListaEmMemoria();
                BsProcesso.ResetBindings(false);
            }
            else
            {
                _paginaAtual = 1;
                await CarregarDadosAsync();
            }
        }

        private async void BtnExportar_Click(object sender, EventArgs e)
        {
            var importadores = await _repositorio.ObterValoresUnicosAsync("Importador");

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
                                        "Sucesso", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
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
            TxtPesquisar.Clear(); // Limpa o texto primeiro
            await CarregarDadosAsync();
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

        private void BtnDownloadTabela_Click(object sender, EventArgs e)
        {
            if (DGVItajai.Rows.Count == 0)
            {
                MessageBox.Show("Não há dados na tabela para exportar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool apenasSelecionadas = false;
            if (DGVItajai.SelectedRows.Count > 0)
            {
                var resp = MessageBox.Show(
                    $"Você tem {DGVItajai.SelectedRows.Count} linhas selecionadas.\nDeseja exportar APENAS a seleção?\n\n(Não = Exportar tudo)",
                    "Opções de Exportação", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (resp == DialogResult.Cancel) return;
                apenasSelecionadas = (resp == DialogResult.Yes);
            }

            using var sfd = new SaveFileDialog();
            sfd.Filter = "Arquivo PDF (*.pdf)|*.pdf";
            sfd.FileName = $"Relatorio_Itajai_{DateTime.Now:yyyyMMdd_HHmm}.pdf";

            if (sfd.ShowDialog() != DialogResult.OK) return;

            try
            {
                Cursor.Current = Cursors.WaitCursor;

                PdfExportService.ExportarGridParaPdf(
                    DGVItajai, sfd.FileName, "Relatório de Processos - Itajaí", apenasSelecionadas
                );

                Cursor.Current = Cursors.Default;

                int qtdExportada = apenasSelecionadas ? DGVItajai.SelectedRows.Count : DGVItajai.Rows.Count;

                _ = Task.Run(() => _repoLog.RegistrarLogAsync(
                    "Exportação", _logado.Usuario,
                    "Relatório PDF da tabela Itajaí gerado",
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

        private void BtnAjuda_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized)
                this.WindowState = FormWindowState.Normal;
            else if (this.WindowState == FormWindowState.Normal)
                this.WindowState = FormWindowState.Maximized;
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
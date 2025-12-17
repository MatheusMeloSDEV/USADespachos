using CLUSA;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Font;
using iText.IO.Font.Constants;
using iText.IO.Font;
using iText.Kernel.Geom; // For PageSize
using iText.Kernel.Colors; // For ColorConstants
using System.IO;
using MongoDB.Driver;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;

namespace Trabalho
{
    public partial class frmSantos : Form
    {
        private readonly RepositorioProcesso _repositorio;
        private int _estadoOrdenacaoRefUsa = 0;
        private DataGridViewColumn? _colunaOrdenada;
        private ListSortDirection _direcaoOrdenacao;
        private List<Processo> _listaOriginal = new();

        private readonly RepositorioUsers _repositorioUsers;
        private readonly LogRepository _logRepo;
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
            _logRepo = new LogRepository();
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
        private void LimparCacheAutoComplete()
        {
            _cacheAutoComplete.Clear();
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
            // Obtém a lista de importadores únicos do repositório
            var importadores = await _repositorio.ObterValoresUnicosAsync("Importador");

            // Exibe um formulário para seleção do importador
            using var form = new ImporterSelectionForm(importadores);
            if (form.ShowDialog() == DialogResult.OK)
            {
                string importador = form.SelectedImporter;

                // 1) Cria sem using
                var progressForm = new ProgressForm();
                progressForm.Show(this);       // exibe modeless, com o próprio Form como owner


                await Task.Run(() =>
                {
                    string pdfPath = "";
                    string? mensagemErro = null;

                    try
                    {
                        pdfPath = PythonRunner.ExecutarExportador(importador).Trim();
                        _ = Task.Run(() => _logRepo.RegistrarLogAsync(
                            "Exportação",
                            "Follow-Up gerado",
                            $"Usuário: {_logado.Usuario} | Registros visíveis: {DGVSantos.RowCount}"
                        ));
                    }
                    catch (Exception ex)
                    {
                        mensagemErro = $"Erro durante exportação: {ex.Message}";
                    }

                    Invoke(new Action(() =>
                    {
                        progressForm.Close();
                        progressForm.Dispose();

                        if (mensagemErro != null)
                        {
                            MessageBox.Show(mensagemErro, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        var resp = MessageBox.Show(
                            "Exportação concluída. Deseja abrir o PDF?",
                            "Resultado",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question
                        );

                        if (resp == DialogResult.Yes && File.Exists(pdfPath))
                        {
                            try
                            {
                                // Abre o PDF com o aplicativo padrão, usando o Explorer
                                Process.Start("explorer.exe", pdfPath);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Erro ao tentar abrir o PDF: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }

                    }));
                });
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
            // 1. Get visible columns to export
            var colunasVisiveis = DGVSantos.Columns.Cast<DataGridViewColumn>()
                                           .Where(c => c.Visible)
                                           .ToList();

            if (colunasVisiveis.Count == 0)
            {
                MessageBox.Show("Nenhuma coluna visível para exportar.", "Aviso");
                return;
            }

            // 2. Ask user for file location
            using var sfd = new SaveFileDialog();
            sfd.Filter = "Arquivo PDF (*.pdf)|*.pdf";
            sfd.FileName = $"Relatorio_Santos_{DateTime.Now:yyyyMMdd_HHmm}.pdf";

            if (sfd.ShowDialog() != DialogResult.OK) return;

            try
            {
                // 3. Initialize PDF Writer and Document
                using var writer = new PdfWriter(sfd.FileName);
                using var pdf = new PdfDocument(writer);
                var document = new Document(pdf, PageSize.A3.Rotate());
                document.SetMargins(10, 10, 10, 10);

                // 4. Load Fonts (Standard Helvetica)
                // Using explicit encoding to be safe
                var fontRegular = PdfFontFactory.CreateFont(StandardFonts.HELVETICA, PdfEncodings.WINANSI);
                var fontBold = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD, PdfEncodings.WINANSI);

                // 5. Add Title
                var titulo = new Paragraph("Relatório de Processos - Santos")
                    .SetFont(fontBold)
                    .SetFontSize(18)
                    .SetTextAlignment(TextAlignment.CENTER);

                document.Add(titulo);
                document.Add(new Paragraph("\n")); // Spacer

                _ = Task.Run(() => _logRepo.RegistrarLogAsync(
                    "Exportação",
                    "Relatório PDF da tabela Santos gerado",
                    $"Usuário: {_logado.Usuario} | Registros visíveis: {DGVSantos.RowCount}"
                ));

                // 6. Create Table
                // UseAllAvailableWidth makes the table span the page width
                var table = new Table(UnitValue.CreatePercentArray(colunasVisiveis.Count)).UseAllAvailableWidth();

                // 7. Add Headers
                foreach (var col in colunasVisiveis)
                {
                    var headerText = col.HeaderText ?? string.Empty;
                    var cell = new Cell().Add(new Paragraph(headerText)
                        .SetFont(fontBold)
                        .SetFontSize(10));

                    cell.SetBackgroundColor(iText.Kernel.Colors.ColorConstants.LIGHT_GRAY);
                    table.AddHeaderCell(cell);
                }

                // 8. Add Data Rows
                foreach (DataGridViewRow row in DGVSantos.Rows)
                {
                    if (row.IsNewRow) continue;

                    foreach (var col in colunasVisiveis)
                    {
                        // Safe string conversion
                        var cellValue = row.Cells[col.Index].Value;
                        var textValue = cellValue?.ToString() ?? "";

                        var cell = new Cell().Add(new Paragraph(textValue)
                            .SetFont(fontRegular)
                            .SetFontSize(7));

                        table.AddCell(cell);
                    }
                }

                // 9. Add Table to Document and Close
                document.Add(table);
                document.Close();

                // 10. Success Message and Open File
                if (MessageBox.Show("PDF gerado com sucesso! Deseja abrir agora?", "Sucesso",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    // Open the PDF safely
                    var p = new ProcessStartInfo(sfd.FileName) { UseShellExecute = true };
                    Process.Start(p);
                }
            }
            catch (IOException ioEx)
            {
                // Catch file access errors specifically
                MessageBox.Show($"Não foi possível salvar o arquivo. Verifique se ele já está aberto em outro programa.\n\nDetalhe: {ioEx.Message}",
                    "Arquivo em Uso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                // Catch all other errors (like iText specific ones)
                var mensagemErro = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                MessageBox.Show($"Erro real: {mensagemErro}\n\nStack: {ex.StackTrace}",
                    "Erro Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

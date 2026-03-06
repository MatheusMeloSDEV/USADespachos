using CLUSA;
using CLUSA.Repositories;
using CLUSA.Services;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using CLUSA.Models;

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
        private const int _itensPorPagina = 50; // Quantidade por "fatiada"
        private long _totalRegistros = 0;

        public FrmItajaí(Logado logado)
        {
            InitializeComponent();
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

        private async Task CarregarDadosAsync()
        {
            try
            {
                MostrarLoading($"Carregando página {_paginaAtual}...");

                string campoBD = _colunaOrdenada?.DataPropertyName ?? "DataDeAtracacao";

                // Proteção: Se a coluna for ignorada no Mongo (ex: OrgaosAnuentesString), não podemos ordenar por ela no BD
                if (campoBD == "OrgaosAnuentesString" || string.IsNullOrWhiteSpace(campoBD))
                {
                    campoBD = "DataDeAtracacao";
                }

                bool isAsc = (_direcaoOrdenacao == ListSortDirection.Ascending);

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

                // Atualiza a Grid de forma limpa
                BsProcesso.DataSource = _listaOriginal;
                DGVItajai.DataSource = BsProcesso;
                BsProcesso.ResetBindings(false);

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
        private void AtualizarControlesPaginacao()
        {
            int inicio = ((_paginaAtual - 1) * _itensPorPagina) + 1;
            int fim = inicio + _listaOriginal.Count - 1;

            // Ex: "Mostrando 1-50 de 697"
            lblQtd.Text = $"{inicio}-{fim} de {_totalRegistros}";

            // Desativa botões se não houver para onde ir
            btnPrevious.Enabled = _paginaAtual > 1;
            btnForward.Enabled = fim < _totalRegistros;
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

            try
            {
                // MUDANÇA: Chamada assíncrona ao repositório.
                var valores = await _repositorio.ObterValoresUnicosAsync(campoSelecionado.DataPropertyName);
                var collection = new AutoCompleteStringCollection();
                collection.AddRange(valores.ToArray());

                TxtPesquisar.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                TxtPesquisar.AutoCompleteSource = AutoCompleteSource.CustomSource;
                TxtPesquisar.AutoCompleteCustomSource = collection;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao configurar o autocompletar: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                    // A nova versão do repositório cuida de TUDO (salvar em PROCESSO, MAPA, ANVISA, etc.)
                    await _repositorio.CreateAsync(processo);

                    // Apenas atualiza a tela
                    BsProcesso.Add(processo);
                    BsProcesso.ResetBindings(false);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao adicionar o processo: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            await CarregarDadosAsync();
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
                    // O repositório exclui o processo principal e TODOS os relacionados.
                    await _repositorio.DeleteAsync(processoSelecionado.Id.ToString());
                    BsProcesso.Remove(processoSelecionado);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao excluir o processo: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            await CarregarDadosAsync();
        }
        private async void BtnEditar_Click(object? sender, EventArgs e)
        {
            if (BsProcesso.Current is not Processo processoSelecionado)
            {
                MessageBox.Show("Nenhum processo selecionado para edição.", "Aviso");
                return;
            }

            using var frm = new FrmModificaProcesso { processo = processoSelecionado, Modo = "Editar", UsuarioLogado = _logado };
            frm.ShowDialog();

            await CarregarDadosAsync();
        }
        private async void BtnPesquisar_Click(object sender, EventArgs e)
        {
            if (CmbPesquisar.SelectedItem is not DisplayItem campoSelecionado) return;

            var pesquisa = TxtPesquisar.Text;
            if (string.IsNullOrWhiteSpace(pesquisa))
            {
                _paginaAtual = 1; // Reseta para o modo normal
                await CarregarDadosAsync();
                return;
            }

            try
            {
                MostrarLoading("Pesquisando...");

                // Na pesquisa, como costuma retornar poucos itens, 
                // podemos trazer direto ou paginar também. 
                // Aqui está a versão simples com contagem:
                var resultados = await _repositorio.PesquisarAsync(campoSelecionado.DataPropertyName, pesquisa);

                BsProcesso.DataSource = resultados;
                BsProcesso.ResetBindings(false);

                // Atualiza a label com o resultado da busca
                lblQtd.Text = $"Encontrados: {resultados.Count}";

                // Desativa paginação durante uma busca específica para não confundir o usuário
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
        private async void DGVItajai_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
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
            // 1. Validação básica
            if (DGVItajai.Rows.Count == 0)
            {
                MessageBox.Show("Não há dados na tabela para exportar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Lógica de seleção (Pergunta ao usuário se exporta tudo ou só a seleção)
            bool apenasSelecionadas = false;
            if (DGVItajai.SelectedRows.Count > 0)
            {
                var resp = MessageBox.Show(
                    $"Você tem {DGVItajai.SelectedRows.Count} linhas selecionadas.\nDeseja exportar APENAS a seleção?\n\n(Não = Exportar tudo)",
                    "Opções de Exportação",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);

                if (resp == DialogResult.Cancel) return;
                apenasSelecionadas = (resp == DialogResult.Yes);
            }

            // 3. Configura o Arquivo
            using var sfd = new SaveFileDialog();
            sfd.Filter = "Arquivo PDF (*.pdf)|*.pdf";
            sfd.FileName = $"Relatorio_Itakjai_{DateTime.Now:yyyyMMdd_HHmm}.pdf";

            if (sfd.ShowDialog() != DialogResult.OK) return;

            // 4. Executa a exportação usando o Serviço
            try
            {
                Cursor.Current = Cursors.WaitCursor; // Feedback visual simples

                // Chama a classe estática que criamos
                PdfExportService.ExportarGridParaPdf(
                    DGVItajai,
                    sfd.FileName,
                    "Relatório de Processos - Itajaí",
                    apenasSelecionadas
                );

                Cursor.Current = Cursors.Default;

                // 5. Log e Sucesso
                int qtdExportada = apenasSelecionadas ? DGVItajai.SelectedRows.Count : DGVItajai.Rows.Count;

                // Dispara o log sem travar a UI
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

        private async void DGV_ColumnHeaderMouseClick(object? sender, DataGridViewCellMouseEventArgs e)
        {
            if (sender is not DataGridView dgv) return;
            var novaColuna = dgv.Columns[e.ColumnIndex];
            if (novaColuna.SortMode == DataGridViewColumnSortMode.NotSortable) return;

            // 1. Define a Direção (Inverte se clicar na mesma coluna)
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

            // 2. RESETAR PARA A PÁGINA 1 (O que você pediu)
            _paginaAtual = 1;

            // 3. ATUALIZAR AS SETAS VISUAIS (Glyphs)
            foreach (DataGridViewColumn col in dgv.Columns)
            {
                if (col.Name == novaColuna.Name)
                    col.HeaderCell.SortGlyphDirection = (_direcaoOrdenacao == ListSortDirection.Ascending)
                        ? SortOrder.Ascending : SortOrder.Descending;
                else
                    col.HeaderCell.SortGlyphDirection = SortOrder.None;
            }

            // 4. RECARREGAR DO BANCO (Agora com a nova ordem e página 1)
            await CarregarDadosAsync();
        }
        private void BtnAjuda_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized)
                this.WindowState = FormWindowState.Normal;
            if (this.WindowState == FormWindowState.Normal)
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

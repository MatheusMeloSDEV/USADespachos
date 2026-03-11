using CLUSA;
using CLUSA.Models;
using CLUSA.Repositories;
using CLUSA.Services;
using MongoDB.Driver;
using Org.BouncyCastle.Crypto;
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

        private FrmLoadingOverlay? _overlay;
        private readonly RepositorioUsers _repositorioUsers;
        private readonly RepositorioLog _logRepo;
        private Users? _usuarioLogado;
        private readonly Logado _logado;

        private int _paginaAtual = 1;
        private int _itensPorPagina = 50;
        private bool _carregandoCombobox = true;
        private long _totalRegistros = 0;
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
                if (_usuarioLogado != null && _usuarioLogado.ItensPorPagina > 0)
                {
                    _itensPorPagina = _usuarioLogado.ItensPorPagina;
                }

                // Ajusta o texto do ComboBox para refletir o banco de dados
                string valorCombo = _itensPorPagina == int.MaxValue ? "Sem Limite" : _itensPorPagina.ToString();
                cbMaxRows.SelectedItem = valorCombo;
                _carregandoCombobox = false;

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
                MostrarLoading($"Carregando página {_paginaAtual}...");

                string campoBD = _colunaOrdenada?.DataPropertyName ?? "DataDeAtracacao";

                // Proteção: Se a coluna for ignorada no Mongo, usamos um padrão
                if (campoBD == "OrgaosAnuentesString" || string.IsNullOrWhiteSpace(campoBD))
                {
                    campoBD = "DataDeAtracacao";
                }

                bool isAsc = (_direcaoOrdenacao == ListSortDirection.Ascending);

                // --- 1. SALVA O ITEM SELECIONADO ATUALMENTE ---
                var idSelecionado = (BsProcesso.Current as Processo)?.Id;

                // Busca no banco (já respeitando a página atual e a ordenação salva nas variáveis!)
                var (itens, total) = await _repositorio.ListarPrincipalPaginadoAsync(
                    "Santos",
                    _paginaAtual,
                    _itensPorPagina,
                    campoBD,
                    isAsc
                );

                _totalRegistros = total;
                _listaOriginal = itens;

                // Atualiza a Grid
                BsProcesso.DataSource = _listaOriginal;
                DGVSantos.DataSource = BsProcesso;
                BsProcesso.ResetBindings(false);

                // --- 2. RESTAURA A SELEÇÃO DA LINHA ---
                // Assim a tela não "pula" lá para o topo depois que você edita
                if (idSelecionado != null)
                {
                    var index = _listaOriginal.FindIndex(p => p.Id == idSelecionado);
                    if (index >= 0)
                    {
                        BsProcesso.Position = index;
                    }
                }

                // --- 3. RESTAURA A SETINHA DO CABEÇALHO ---
                // O C# apaga a seta visualmente quando recarregamos a fonte de dados, isso força ele a desenhar de novo.
                if (_colunaOrdenada != null)
                {
                    foreach (DataGridViewColumn col in DGVSantos.Columns)
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
            using var frm = new FrmModificaProcesso { processo = processo, Modo = "Adicionar", Origem = Santos, UsuarioLogado = _logado };

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
                        "Exclusão", _logado.Usuario,
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

            using var frm = new FrmModificaProcesso { processo = processoSelecionado, Modo = "Editar", UsuarioLogado = _logado };
            frm.ShowDialog();

            _cacheAutoComplete.Clear(); // Limpa cache

            if (!string.IsNullOrWhiteSpace(TxtPesquisar.Text))
            {
                await ExecutarPesquisaAsync(); // Espera a pesquisa e a ordenação terminarem com segurança
            }
            else
            {
                await CarregarDadosAsync();
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

                BsProcesso.DataSource = resultados;

                // --- A MÁGICA AQUI ---
                // Se já existir uma coluna ordenada, ele aplica a ordem instantaneamente
                // antes de desenhar a grade de novo!
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

        // O clique do botão agora só chama o método de forma segura
        private async void BtnPesquisar_Click(object sender, EventArgs e)
        {
            await ExecutarPesquisaAsync();
        }
        private async void DGVSantos_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || BsProcesso.Current is not Processo processoSelecionado) return;

            using var frm = new FrmModificaProcesso { processo = processoSelecionado, Visualização = true, Modo = "Visualizar" };
            frm.ShowDialog();

            // --- MESMA MÁGICA PARA O DUPLO CLIQUE ---
            if (!string.IsNullOrWhiteSpace(TxtPesquisar.Text))
            {
                BtnPesquisar.PerformClick();
            }
            else
            {
                await CarregarDadosAsync();
            }
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
            cbMaxRows.Enabled = true;
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


        // Substitua seu método de clique no cabeçalho por este
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

            // Se tem texto na pesquisa, ordena em memória
            if (!string.IsNullOrWhiteSpace(TxtPesquisar.Text))
            {
                OrdenarListaEmMemoria();
                BsProcesso.ResetBindings(false);
            }
            else
            {
                // Se não tem pesquisa, carrega paginado do banco
                _paginaAtual = 1;
                await CarregarDadosAsync();
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
                    "Exportação", _logado.Usuario,
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

        private async void cbMaxRows_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Evita que o evento dispare sozinho enquanto a tela está abrindo
            if (_carregandoCombobox || _usuarioLogado == null) return;

            string selecionado = cbMaxRows.SelectedItem?.ToString() ?? "50";

            // "Sem Limite" = O maior número possível no C# (2 bilhões de registros)
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
                // Salva a nova preferência no banco de dados do usuário
                _usuarioLogado.ItensPorPagina = _itensPorPagina;

                // Chame o método que você usa no seu repositório para salvar a edição do usuário
                // Pode ser UpdateAsync, UpsertAsync ou SalvarAlteracoesAsync, dependendo de como você nomeou
                await _repositorioUsers.UpdateAsync(_usuarioLogado);

                // Reseta para a página 1 e recarrega os dados com o novo limite
                _paginaAtual = 1;
                await CarregarDadosAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar sua preferência: {ex.Message}", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void AtualizarControlesPaginacao()
        {
            // Se não tiver nenhum registro
            if (_totalRegistros == 0)
            {
                lblQtd.Text = "0 registros";
                btnPrevious.Enabled = false;
                btnForward.Enabled = false;
                return;
            }

            // Se o usuário escolheu "Sem Limite", o texto e os botões mudam
            if (_itensPorPagina == int.MaxValue)
            {
                lblQtd.Text = $"Mostrando todos os {_totalRegistros} registros";
                btnPrevious.Enabled = false;
                btnForward.Enabled = false;
                return;
            }

            // Cálculo normal de paginação
            int inicio = ((_paginaAtual - 1) * _itensPorPagina) + 1;
            int fim = inicio + _listaOriginal.Count - 1;

            lblQtd.Text = $"{inicio}-{fim} de {_totalRegistros}";

            btnPrevious.Enabled = _paginaAtual > 1;
            btnForward.Enabled = fim < _totalRegistros;
        }

        private void OrdenarListaEmMemoria()
        {
            // Se não tem coluna clicada ou não é uma lista, não faz nada
            if (_colunaOrdenada == null || BsProcesso.DataSource is not IEnumerable<Processo> listaPesquisa) return;

            var propInfo = typeof(Processo).GetProperty(_colunaOrdenada.DataPropertyName);
            if (propInfo == null) return;

            bool CampoEstaVazio(object valor)
            {
                if (valor == null) return true;
                if (valor is string texto && string.IsNullOrWhiteSpace(texto)) return true;
                return false;
            }

            // Aplica a ordem mantendo os vazios no final
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
    }
}

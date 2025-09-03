using CLUSA;
using MongoDB.Driver;
using System.Diagnostics;

namespace Trabalho
{
    public partial class frmSantos : Form
    {
        private readonly RepositorioProcesso _repositorio;
        private int _estadoOrdenacaoRefUsa = 0; // 0 = original, 1 = asc, 2 = desc
        private List<Processo> _listaOriginal = new List<Processo>();

        public frmSantos()
        {
            InitializeComponent();
            _repositorio = new RepositorioProcesso();
        }
        private static System.Drawing.Image? CarregarImagemDoRecurso(System.Reflection.Assembly assembly, string resourcePath)
        {
            using var stream = assembly.GetManifestResourceStream(resourcePath);
            return stream != null ? System.Drawing.Image.FromStream(stream) : null;
        }
        private void ImagensBotoes()
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var resources = assembly.GetManifestResourceNames();
            foreach (var r in resources)
            {
                Console.WriteLine(r);
            }

            var recursos = new (string CaminhoRecurso, ToolStripButton Botao)[]
            {
                ("Trabalho.Imagens.botao-adicionar.png", BtnAdicionar),
                ("Trabalho.Imagens.botao-editar.png", BtnEditar),
                ("Trabalho.Imagens.excluir.png", BtnRemover),
                ("Trabalho.Imagens.exportar.png", BtnExportar),
                ("Trabalho.Imagens.cancelar.png", BtnCancelar),
                ("Trabalho.Imagens.lupa-de-pesquisa.png", BtnPesquisar),
                ("Trabalho.Imagens.recarregar.png", BtnReload)
            };

            foreach (var (caminho, botao) in recursos)
            {
                botao.Image = CarregarImagemDoRecurso(assembly, caminho);
            }
        }
        private void ConfigurarColunasDataGridViewProcesso()
        {
            DataGridView1.Columns.Clear();

            // Configuração básica
            DataGridView1.AutoGenerateColumns = false;
            DataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            DataGridView1.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True;
            DataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DataGridView1.RowHeadersVisible = false;
            DataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            DataGridView1.DefaultCellStyle.WrapMode = DataGridViewTriState.True;

            // Ref. USA - menor peso
            DataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Ref_USA",
                HeaderText = "Ref. USA",
                Name = "ColunaRefUSA",
                FillWeight = 40
            });

            // S. Ref - menor peso
            DataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "SR",
                HeaderText = "S. Ref",
                Name = "ColunaSR",
                FillWeight = 40
            });

            // Importador
            DataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Importador",
                HeaderText = "Importador",
                Name = "ColunaImportador",
                FillWeight = 80
            });

            // Exportador
            DataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Exportador",
                HeaderText = "Exportador",
                Name = "ColunaExportador",
                FillWeight = 80
            });

            // Produto
            DataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Produto",
                HeaderText = "Produto",
                Name = "ColunaProduto",
                FillWeight = 80
            });

            // Navio
            DataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Veiculo",
                HeaderText = "Veículo",
                Name = "ColunaVeiculo",
                FillWeight = 80
            });

            // Porto Destino
            DataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "PortoDestino",
                HeaderText = "Porto Destino",
                Name = "ColunaPortoDestino",
                FillWeight = 80
            });

            // Pendência (texto longo com quebra de linha)
            DataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Pendencia",
                HeaderText = "Pendência",
                Name = "ColunaPendencia",
                FillWeight = 120,
                DefaultCellStyle = new DataGridViewCellStyle { WrapMode = DataGridViewTriState.True }
            });


            // 2) Estilo geral
            foreach (DataGridViewColumn coluna in DataGridView1.Columns)
            {
                coluna.DefaultCellStyle.Font = new Font("Segoe UI", 10);
                coluna.DefaultCellStyle.ForeColor = Color.Black;
                coluna.DefaultCellStyle.BackColor = Color.White;
                coluna.DefaultCellStyle.SelectionBackColor = Color.LightBlue;
                coluna.DefaultCellStyle.SelectionForeColor = Color.Black;
                coluna.MinimumWidth = 100;
            }
        }

        private void FrmProcesso_Load(object sender, EventArgs e)
        {
            try
            {
                ConfigurarColunasDataGridViewProcesso();

                var registros = _repositorio.FindAll();
                // Ordena sempre do menor para o maior
                var registrosOrdenados = registros.OrderBy(p => ExtrairAnoNumero(p.Ref_USA)).ToList();
                _listaOriginal = registrosOrdenados;

                if (registrosOrdenados.Any())
                {
                    BsProcesso.DataSource = registrosOrdenados;
                    DataGridView1.DataSource = BsProcesso;
                }
                else
                {
                    MessageBox.Show(
                        "Operação concluída com sucesso.",
                        "Sucesso",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }

                ImagensBotoes();
                this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
                PopularComboBoxDePesquisa();

                if (CmbPesquisar.Items.Count > 0)
                {
                    CmbPesquisar.SelectedIndex = 0;
                }
                ConfigurarAutoCompletarParaPesquisa();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar os dados: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PopularComboBoxDePesquisa()
        {
            var camposIgnorados = new HashSet<string>
            {
                "Id", "TDecex", "TAnvisa", "TMapa", "TImetro", "TIbama",
                "PossuiEmbarque", "VencimentoFreeTime", "VencimentoFMA"
            };

            CmbPesquisar.Items.Clear();

            foreach (DataGridViewColumn coluna in DataGridView1.Columns)
            {
                if (!string.IsNullOrEmpty(coluna.DataPropertyName) &&
                    !camposIgnorados.Contains(coluna.DataPropertyName))
                {
                    CmbPesquisar.Items.Add(new DisplayItem(coluna.DataPropertyName, coluna.HeaderText));
                }
            }

            // "Desligue" o evento antes de definir o índice
            CmbPesquisar.SelectedIndexChanged -= CmbPesquisar_SelectedIndexChanged;

            if (CmbPesquisar.Items.Count > 0)
            {
                CmbPesquisar.SelectedIndex = 0;
            }

            // "Religue" o evento para que funcione para o usuário
            CmbPesquisar.SelectedIndexChanged += CmbPesquisar_SelectedIndexChanged;
        }

        private void ConfigurarAutoCompletarParaPesquisa()
        {
            if (CmbPesquisar.SelectedItem is DisplayItem campoSelecionado)
            {
                var valores = _repositorio.ObterValoresUnicos(campoSelecionado.DataPropertyName);
                TxtPesquisar.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                TxtPesquisar.AutoCompleteSource = AutoCompleteSource.CustomSource;
                var collection = new AutoCompleteStringCollection();
                collection.AddRange(valores.ToArray());
                TxtPesquisar.AutoCompleteCustomSource = collection;
            }
            else
            {
                MessageBox.Show("Selecione um campo para configurar o autocompletar.",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private async void BtnAdicionar_Click(object sender, EventArgs e)
        {
            var processo = new Processo();
            using var frm = new FrmModificaProcesso { processo = processo, Modo = "Adicionar", Visualização = false };
            frm.ShowDialog();

            if (frm.DialogResult == DialogResult.OK)
            {
                // Adiciona o processo principal
                await _repositorio.Create(processo, "PROCESSO");
                BsProcesso.Add(processo);
                BsProcesso.ResetBindings(false);

                // Verifica e adiciona nas coleções específicas se necessário
                if (!_repositorio.VerificarExistencia(processo))
                {
                    if (processo.TMapa)
                    {
                        await _repositorio.Create(processo, "MAPA");
                    }
                    if (processo.TAnvisa)
                    {
                        await _repositorio.Create(processo, "Anvisa");
                    }
                    if (processo.TDecex)
                    {
                        await _repositorio.Create(processo, "Decex");
                    }
                    if (processo.TImetro)
                    {
                        await _repositorio.Create(processo, "IMETRO");
                    }
                    if (processo.TIbama)
                    {
                        await _repositorio.Create(processo, "IBAMA");
                    }
                }
            }
        }
        private void BtnReload_Click(object sender, EventArgs e)
        {
            var registros = _repositorio.FindAll();
            var registrosOrdenados = registros.OrderBy(p => ExtrairAnoNumero(p.Ref_USA)).ToList();
            _listaOriginal = registrosOrdenados;
            BsProcesso.DataSource = registrosOrdenados;
            DataGridView1.DataSource = BsProcesso;
        }
        private async void BtnExcluir_Click(object sender, EventArgs e)
        {
            if (BsProcesso.Current is not Processo processoSelecionado)
            {
                MessageBox.Show("Nenhum processo selecionado para exclusão.",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirmResult = MessageBox.Show(
                $"Tem certeza de que deseja excluir o processo '{processoSelecionado.Ref_USA}'?",
                "Confirmação de Exclusão", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmResult == DialogResult.Yes)
            {
                await _repositorio.Delete(processoSelecionado);
                BsProcesso.Remove(processoSelecionado);
                BsProcesso.ResetBindings(false);
            }
        }
        private async void BtnEditar_Click(object sender, EventArgs e)
        {
            if (BsProcesso.Current is not Processo processoSelecionado)
            {
                MessageBox.Show("Nenhum processo selecionado para edição.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using var frm = new FrmModificaProcesso { processo = processoSelecionado, Modo = "Editar", Visualização = false };
            frm.ShowDialog();

            if (frm.DialogResult == DialogResult.OK)
            {
                // Atualiza o processo principal
                await _repositorio.Update(processoSelecionado);

                BsProcesso.ResetBindings(false);
            }
        }


        private void BtnPesquisar_Click(object sender, EventArgs e)
        {
            if (CmbPesquisar.SelectedItem is not DisplayItem campoSelecionado)
            {
                MessageBox.Show("Selecione um campo para pesquisar.",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var filtro = campoSelecionado.DataPropertyName;
            var pesquisa = TxtPesquisar.Text;

            if (string.IsNullOrEmpty(pesquisa))
            {
                MessageBox.Show("Digite um valor para pesquisar.",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var resultados = _repositorio.Find(filtro, pesquisa);

            if (resultados.Any())
            {
                BsProcesso.DataSource = resultados;
                BsProcesso.ResetBindings(false);
            }
            else
            {
                MessageBox.Show("Nenhum resultado encontrado.",
                    "Informação", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }


        private void DataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (BsProcesso.Current is not Processo processoSelecionado)
            {
                MessageBox.Show("Nenhum processo selecionado para edição.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using var frm = new FrmModificaProcesso { processo = processoSelecionado, Visualização = true, Modo = "Visualizar" };
            frm.ShowDialog();

            // Sempre recarrega e ordena após fechar o diálogo
            var registros = _repositorio.FindAll();
            var registrosOrdenados = registros.OrderBy(p => ExtrairAnoNumero(p.Ref_USA)).ToList();
            _listaOriginal = registrosOrdenados;
            BsProcesso.DataSource = registrosOrdenados;
            DataGridView1.DataSource = BsProcesso;
        }

        private void BtnExportar_Click(object sender, EventArgs e)
        {
            // Obtém a lista de importadores únicos do repositório
            var importadores = _repositorio.ObterImportadoresUnicos();

            // Exibe um formulário para seleção do importador
            using var form = new ImporterSelectionForm(importadores);
            if (form.ShowDialog() == DialogResult.OK)
            {
                string importador = form.SelectedImporter;

                // 1) Cria sem using
                var progressForm = new ProgressForm();
                progressForm.Show(this);       // exibe modeless, com o próprio Form como owner


                Task.Run(() =>
                {
                    string pdfPath = "";
                    string? mensagemErro = null;

                    try
                    {
                        pdfPath = PythonRunner.ExecutarExportador(importador).Trim();
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


        private void CmbPesquisar_SelectedIndexChanged(object sender, EventArgs e)
        {
            ConfigurarAutoCompletarParaPesquisa();
        }

        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            BsProcesso.DataSource = _repositorio.FindAll();
            BsProcesso.ResetBindings(false);
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
        private void DataGridView1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            var coluna = DataGridView1.Columns[e.ColumnIndex];
            if (coluna.Name == "ColunaRefUSA")
            {
                var lista = BsProcesso.DataSource as List<Processo>;
                if (lista == null) return;

                _estadoOrdenacaoRefUsa = (_estadoOrdenacaoRefUsa + 1) % 3;

                List<Processo> listaOrdenada;
                string header = "Ref. USA";
                switch (_estadoOrdenacaoRefUsa)
                {
                    case 1: // Ascendente
                        listaOrdenada = _listaOriginal.OrderBy(p => ExtrairAnoNumero(p.Ref_USA)).ToList();
                        header += "  ↓";
                        break;
                    case 2: // Descendente
                        listaOrdenada = _listaOriginal.OrderByDescending(p => ExtrairAnoNumero(p.Ref_USA)).ToList();
                        header += "  ↑";
                        break;
                    default: // Original
                        listaOrdenada = new List<Processo>(_listaOriginal);
                        break;
                }

                BsProcesso.DataSource = listaOrdenada;
                DataGridView1.DataSource = BsProcesso;
                DataGridView1.Columns[0].HeaderText = header;
            }
        }

        // Função auxiliar para extrair ano e número do formato 0000/0000
        private (int ano, int numero) ExtrairAnoNumero(string refUsa)
        {
            if (string.IsNullOrWhiteSpace(refUsa)) return (0, 0);
            var partes = refUsa.Split('/');
            int numero = 0, ano = 0;
            if (partes.Length == 2)
            {
                int.TryParse(partes[0], out numero);
                int.TryParse(partes[1], out ano);
            }
            return (ano, numero);
        }
    }
}

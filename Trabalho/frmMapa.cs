using CLUSA;
using static Trabalho.frmSantos;

namespace Trabalho
{
    public partial class FrmMapa : Form
    {
        private readonly RepositorioOrgaoAnuente<MAPA> _repositorio;

        public FrmMapa()
        {
            InitializeComponent();
            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            _repositorio = new RepositorioOrgaoAnuente<MAPA>("MAPA");
        }

        private void FrmMapa_Load(object sender, EventArgs e)
        {
            try
            {
                try
                {
                    Conectar();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao carregar o formulário: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                ConfigurarInterface();
                PopularToolStripComboBox();
                ImagensBotoes();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar os dados: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Conectar()
        {
            try
            {
                ConfigurarColunasDataGridView();

                var registros = _repositorio.ListarTodos();
                var registrosOrdenados = registros
                    .OrderBy(m => ExtrairAnoNumero(m.Ref_USA))
                    .ToList();

                if (registrosOrdenados.Count > 0)
                {
                    BSmapa.DataSource = registrosOrdenados;
                    dataGridView1.DataSource = BSmapa;
                }
                else
                {
                    MessageBox.Show("Nenhum registro foi encontrado para carregar no DataGridView.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar o DataGridView: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

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

        private void ConfigurarColunasDataGridView()
        {
            dataGridView1.Columns.Clear();
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dataGridView1.DefaultCellStyle.WrapMode = DataGridViewTriState.True;

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Id",
                HeaderText = "ID",
                Name = "ColunaId",
                ReadOnly = true,
                Visible = false
            });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Ref_USA",
                HeaderText = "Ref. USA",
                Name = "ColunaRefUSA"
            });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "SR",
                HeaderText = "SR",
                Name = "ColunaSR"
            });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Importador",
                HeaderText = "Importador",
                Name = "ColunaImportador"
            });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Exportador",
                HeaderText = "Exportador",
                Name = "ColunaExportador"
            });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Navio",
                HeaderText = "Navio",
                Name = "ColunaNavio"
            });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Produto",
                HeaderText = "Produto",
                Name = "ColunaProduto"
            });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "NCM",
                HeaderText = "NCM",
                Name = "ColunaNCM"
            });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Pendencia",
                HeaderText = "Pendência",
                Name = "ColunaPendencia"
            });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "StatusDoProcesso",
                HeaderText = "Status do Processo",
                Name = "ColunaStatusDoProcesso"
            });

            foreach (DataGridViewColumn coluna in dataGridView1.Columns)
            {
                coluna.DefaultCellStyle.Font = new Font("Segoe UI", 10);
                coluna.DefaultCellStyle.ForeColor = Color.Black;
                coluna.DefaultCellStyle.BackColor = Color.White;
                coluna.DefaultCellStyle.SelectionBackColor = Color.LightBlue;
                coluna.DefaultCellStyle.SelectionForeColor = Color.Black;
                coluna.MinimumWidth = 100;
            }
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
                ("Trabalho.Imagens.botao-editar.png", BtnEditar),
                ("Trabalho.Imagens.cancelar.png", BtnCancelar),
                ("Trabalho.Imagens.lupa-de-pesquisa.png", BtnPesquisar),
                ("Trabalho.Imagens.recarregar.png", BtnReload)
            };

            foreach (var (caminho, botao) in recursos)
            {
                botao.Image = CarregarImagemDoRecurso(assembly, caminho);
            }
        }

        private static System.Drawing.Image? CarregarImagemDoRecurso(System.Reflection.Assembly assembly, string resourcePath)
        {
            using var stream = assembly.GetManifestResourceStream(resourcePath);
            return stream != null ? System.Drawing.Image.FromStream(stream) : null;
        }

        private void ConfigurarInterface()
        {
            if (FrmLogin.Instance.Escuro)
            {
                toolStrip1.BackColor = SystemColors.ControlDark;
                this.BackColor = SystemColors.ControlDark;
                CmbPesquisar.BackColor = SystemColors.ControlDarkDark;
                TxtPesquisar.BackColor = SystemColors.ControlDarkDark;
                dataGridView1.DefaultCellStyle.BackColor = SystemColors.ControlDark;
                dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = SystemColors.ControlDarkDark;
                dataGridView1.BackgroundColor = SystemColors.ControlDark;
            }
        }

        private void PopularToolStripComboBox()
        {
            var camposIgnorados = new HashSet<string> { "Id", "PossuiEmbarque" };
            CmbPesquisar.Items.Clear();

            foreach (DataGridViewColumn coluna in dataGridView1.Columns)
            {
                if (!string.IsNullOrEmpty(coluna.DataPropertyName) && !camposIgnorados.Contains(coluna.DataPropertyName))
                {
                    CmbPesquisar.Items.Add(new DisplayItem(coluna.DataPropertyName, coluna.HeaderText));
                }
            }

            if (CmbPesquisar.Items.Count > 0)
            {
                CmbPesquisar.SelectedIndex = 0;
            }
        }

        private void ConfigurarAutoCompletarParaPesquisa()
        {
            try
            {
                if (CmbPesquisar.SelectedItem == null)
                {
                    MessageBox.Show("Selecione um campo para configurar o autocompletar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var campoSelecionado = CmbPesquisar.SelectedItem?.ToString();

                if (!string.IsNullOrEmpty(campoSelecionado))
                {
                    var registros = _repositorio.ListarTodos();
                    var valores = registros
                        .Select(r => r.GetType().GetProperty(campoSelecionado)?.GetValue(r, null)?.ToString())
                        .Where(v => !string.IsNullOrEmpty(v))
                        .Distinct()
                        .ToArray(); // Corrigido para ToArray()

                    var autoCompleteCollection = new AutoCompleteStringCollection();
                    autoCompleteCollection.AddRange(valores!); // Corrigido para garantir string[]
                    TxtPesquisar.AutoCompleteCustomSource = autoCompleteCollection;
                    TxtPesquisar.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                    TxtPesquisar.AutoCompleteSource = AutoCompleteSource.CustomSource;
                }
                else
                {
                    MessageBox.Show("Selecione um campo para configurar o autocompletar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao configurar o autocompletar: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            try
            {
                var registros = _repositorio.ListarTodos();
                var registrosOrdenados = registros
                    .OrderBy(m => ExtrairAnoNumero(m.Ref_USA))
                    .ToList();
                BSmapa.DataSource = registrosOrdenados;
                BSmapa.ResetBindings(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar todos os processos: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnReload_Click(object sender, EventArgs e)
        {
            var registros = _repositorio.ListarTodos();
            var registrosOrdenados = registros
                .OrderBy(m => ExtrairAnoNumero(m.Ref_USA))
                .ToList();
            BSmapa.DataSource = registrosOrdenados;
            dataGridView1.DataSource = BSmapa;
        }

        private void BtnPesquisar_Click(object sender, EventArgs e)
        {
            if (CmbPesquisar.SelectedItem is DisplayItem campoSelecionado)
            {
                var dataPropertyName = campoSelecionado.DataPropertyName;
                var textoPesquisa = TxtPesquisar.Text;

                if (!string.IsNullOrEmpty(textoPesquisa))
                {
                    BSmapa.Filter = $"{dataPropertyName} LIKE '%{textoPesquisa}%'";
                }
                else
                {
                    MessageBox.Show("Digite um valor para pesquisar.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("Selecione um campo para pesquisar.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async void BtnEditar_Click(object sender, EventArgs e)
        {
            if (BSmapa.Current is not MAPA mapaAtual)
            {
                MessageBox.Show("Nenhum registro selecionado para edição.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var refUsaSelecionado = mapaAtual.Ref_USA;
            var entidade = _repositorio.ObterPorRefUsa(refUsaSelecionado);
            if (entidade != null)
            {
                using var frm = new FrmModifica<MAPA>("MAPA", entidade);
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    await RecarregarEOrdenarDadosAsync();
                }
            }
        }
        private async Task RecarregarEOrdenarDadosAsync()
        {
            try
            {
                // 1. Busca os dados mais recentes de forma assíncrona.
                var registros = await _repositorio.ListarTodosAsync();

                // 2. Ordena os dados usando sua lógica customizada.
                var registrosOrdenados = registros
                    .OrderBy(m => ExtrairAnoNumero(m.Ref_USA))
                    .ToList();

                // 3. Atualiza a fonte de dados do DataGridView.
                BSmapa.DataSource = registrosOrdenados;
                BSmapa.ResetBindings(false); // Garante que a interface reflita a mudança.
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao recarregar os dados: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void DataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (BSmapa.Current is not MAPA mapaSelecionado)
            {
                MessageBox.Show("Nenhum processo selecionado para edição.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using var frm = new FrmModifica<MAPA>("MAPA", mapaSelecionado, true);
            frm.ShowDialog();

            await RecarregarEOrdenarDadosAsync();
        }

        private void CmbPesquisar_SelectedIndexChanged(object sender, EventArgs e)
        {
            ConfigurarAutoCompletarParaPesquisa();
        }
    }
}

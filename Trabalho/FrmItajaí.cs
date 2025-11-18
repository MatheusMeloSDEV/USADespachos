using CLUSA;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Trabalho
{
    public partial class FrmItajaí : Form
    {
        private readonly RepositorioProcesso _repositorio;
        private DataGridViewColumn? _colunaOrdenada;
        private ListSortDirection _direcaoOrdenacao;
        private List<Processo> _listaOriginal = new();

        public FrmItajaí()
        {
            InitializeComponent();
            _repositorio = new RepositorioProcesso();
            this.Shown += FrmItajaí_Shown;
        }
        private async void FrmItajaí_Shown(object? sender, EventArgs e)
        {
            try
            {
                ConfigurarColunasDataGridViewProcesso();
                await CarregarDadosAsync();

                this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

                PopularComboBoxDePesquisa();

                if (CmbPesquisar.Items.Count > 0)
                {
                    CmbPesquisar.SelectedIndex = 0;
                }
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
                var registros = await _repositorio.ListarPorSufixoRefUsaAsync("ITJ");
                var registrosFiltrados = registros.Where(p => p.Status != "Finalizado");

                var registrosOrdenados = registrosFiltrados
                    .OrderBy(p => p.DataDeAtracacao == null || p.DataDeAtracacao == DateTime.MinValue ? 1 : 0) // primeiro os com data
                    .ThenBy(p => p.DataDeAtracacao ?? DateTime.MaxValue)
                    .ToList();
                _listaOriginal = registrosOrdenados;

                BsProcesso.DataSource = registrosOrdenados;
                DGVItajai.DataSource = BsProcesso;
                BsProcesso.ResetBindings(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar os dados: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ConfigurarColunasDataGridViewProcesso()
        {
            DGVItajai.Columns.Clear();

            // --- Configuração Geral da Grade ---
            DGVItajai.AutoGenerateColumns = false;
            DGVItajai.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // Mantém o preenchimento
            DGVItajai.RowHeadersVisible = false;
            DGVItajai.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            DGVItajai.DefaultCellStyle.WrapMode = DataGridViewTriState.False;
            DGVItajai.ShowCellToolTips = true;
            var dateCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" };

            // --- Adicionando as Colunas com Largura Mínima ---

            DGVItajai.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Ref_USA",
                HeaderText = "Ref. USA",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                MinimumWidth = 90
            });
            DGVItajai.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "SR",
                HeaderText = "S. Ref",
                MinimumWidth = 60
            });
            DGVItajai.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Importador",
                HeaderText = "Importador",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader,
                MinimumWidth = 80 // <-- MUDANÇA
            });
            DGVItajai.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Veiculo",
                HeaderText = "Veículo",
                FillWeight = 140,
                MinimumWidth = 80 // <-- MUDANÇA
            });
            DGVItajai.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "DataDeAtracacao",
                HeaderText = "Data de Atracação",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                MinimumWidth = 70 // <-- MUDANÇA
            });
            DGVItajai.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Terminal",
                HeaderText = "Terminal",
                FillWeight = 140,
                MinimumWidth = 140
            });
            DGVItajai.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "LocalDeDesembaraco",
                HeaderText = "Local",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                MinimumWidth = 120 // <-- MUDANÇA
            });
            DGVItajai.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Container",
                HeaderText = "Container",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader,
                MinimumWidth = 120
            });
            DGVItajai.Columns.Add(new DataGridViewCheckBoxColumn
            {
                DataPropertyName = "Redestinacao",
                HeaderText = "Redes.",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader,
                MinimumWidth = 50
            });
            DGVItajai.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "CE",
                HeaderText = "CE",
                FillWeight = 90,
                MinimumWidth = 100 // <-- MUDANÇA
            });
            DGVItajai.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "FreeTime",
                HeaderText = "F.T",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                MinimumWidth = 40 // <-- MUDANÇA
            });
            DGVItajai.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "VencimentoFreeTime",
                HeaderText = "Venc. F. Time",
                DefaultCellStyle = dateCellStyle,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                MinimumWidth = 100 // <-- MUDANÇA
            });
            DGVItajai.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "VencimentoFMA",
                HeaderText = "Venc. FMA",
                DefaultCellStyle = dateCellStyle,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                MinimumWidth = 100 // <-- MUDANÇA
            });
            DGVItajai.Columns.Add(new DataGridViewCheckBoxColumn
            {
                DataPropertyName = "CapaOK",
                HeaderText = "Capa",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader,
                MinimumWidth = 40
            });
            DGVItajai.Columns.Add(new DataGridViewCheckBoxColumn
            {
                DataPropertyName = "Numerario",
                HeaderText = "Num.",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader,
                MinimumWidth = 40
            });
            DGVItajai.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "RascunhoDI",
                HeaderText = "Rascunho DI",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                MinimumWidth = 120
            });
            DGVItajai.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Pendencia",
                HeaderText = "Pendência",
                FillWeight = 160,
                MinimumWidth = 160 // <-- MUDANÇA
            });
            DGVItajai.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Status",
                HeaderText = "Status",
                FillWeight = 180,
                MinimumWidth = 120 // <-- MUDANÇA
            });

            foreach (DataGridViewColumn coluna in DGVItajai.Columns)
            {
                coluna.DefaultCellStyle.Font = new Font("Segoe UI", 10);
                coluna.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                coluna.SortMode = DataGridViewColumnSortMode.Programmatic;

                // Centralizar checkbox
                if (coluna is DataGridViewCheckBoxColumn || coluna.HeaderText == "F.T")
                {
                    coluna.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
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
            using var frm = new FrmModificaProcesso { processo = processo, Modo = "Adicionar", Origem = Itajai };

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

            using var frm = new FrmModificaProcesso { processo = processoSelecionado, Modo = "Editar" };
            frm.ShowDialog();

            await CarregarDadosAsync();
        }
        private async void BtnPesquisar_Click(object sender, EventArgs e)
        {
            if (CmbPesquisar.SelectedItem is not DisplayItem campoSelecionado)
            {
                MessageBox.Show("Selecione um campo para pesquisar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var pesquisa = TxtPesquisar.Text;
            if (string.IsNullOrWhiteSpace(pesquisa))
            {
                // Se a pesquisa estiver vazia, recarrega todos os dados originais.
                BsProcesso.DataSource = _listaOriginal;
                BsProcesso.ResetBindings(false);
                return;
            }

            try
            {
                // MUDANÇA: Chamada assíncrona.
                var resultados = await _repositorio.PesquisarAsync(campoSelecionado.DataPropertyName, pesquisa);
                BsProcesso.DataSource = resultados;
                BsProcesso.ResetBindings(false);

                if (!resultados.Any())
                {
                    MessageBox.Show("Nenhum resultado encontrado.", "Informação", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao pesquisar: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
    }
}

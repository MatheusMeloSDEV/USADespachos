using CLUSA;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Trabalho
{
    public partial class frmFinalizados : Form
    {
        private readonly RepositorioProcesso _repositorio;
        private int _estadoOrdenacaoRefUsa = 0;
        private DataGridViewColumn? _colunaOrdenada;
        private ListSortDirection _direcaoOrdenacao;
        private List<Processo> _listaOriginal = new();
        public frmFinalizados()
        {
            _repositorio = new RepositorioProcesso();
            InitializeComponent();
        }
        private async void FrmFinalizados_Shown(object? sender, EventArgs e)
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
                var registros = await _repositorio.ListarTodosAsync();
                var registrosFinalizados = registros
                    .Where(p => p.Status == "Finalizado")
                    .OrderBy(p => p.DataDeAtracacao == null ? 1 : 0)
                    .ThenBy(p => p.DataDeAtracacao ?? DateTime.MaxValue)
                    .ToList();

                BsProcesso.DataSource = registrosFinalizados;
                DGVFinalizados.DataSource = BsProcesso;
                BsProcesso.ResetBindings(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar os dados: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ConfigurarColunasDataGridViewProcesso()
        {
            DGVFinalizados.Columns.Clear();

            // --- Configuração Geral da Grade ---
            DGVFinalizados.AutoGenerateColumns = false;
            DGVFinalizados.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // Mantém o preenchimento
            DGVFinalizados.RowHeadersVisible = false;
            DGVFinalizados.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            DGVFinalizados.DefaultCellStyle.WrapMode = DataGridViewTriState.False;
            DGVFinalizados.ShowCellToolTips = true;
            var dateCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" };

            // --- Adicionando as Colunas com Largura Mínima ---

            DGVFinalizados.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Ref_USA",
                HeaderText = "Ref. USA",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                MinimumWidth = 90
            });
            DGVFinalizados.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "SR",
                HeaderText = "S. Ref",
                MinimumWidth = 60
            });
            DGVFinalizados.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Importador",
                HeaderText = "Importador",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader,
                MinimumWidth = 80 // <-- MUDANÇA
            });
            DGVFinalizados.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Veiculo",
                HeaderText = "Veículo",
                FillWeight = 140,
                MinimumWidth = 80 // <-- MUDANÇA
            });
            DGVFinalizados.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "DataDeAtracacao",
                HeaderText = "Data de Atracação",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                MinimumWidth = 70 // <-- MUDANÇA
            });
            DGVFinalizados.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Terminal",
                HeaderText = "Terminal",
                FillWeight = 140,
                MinimumWidth = 140
            });
            DGVFinalizados.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "LocalDeDesembaraco",
                HeaderText = "Local",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                MinimumWidth = 120 // <-- MUDANÇA
            });
            DGVFinalizados.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Container",
                HeaderText = "Container",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader,
                MinimumWidth = 120
            });
            DGVFinalizados.Columns.Add(new DataGridViewCheckBoxColumn
            {
                DataPropertyName = "Redestinacao",
                HeaderText = "Redes.",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader,
                MinimumWidth = 50
            });
            DGVFinalizados.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "CE",
                HeaderText = "CE",
                FillWeight = 90,
                MinimumWidth = 100 // <-- MUDANÇA
            });
            DGVFinalizados.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "FreeTime",
                HeaderText = "F.T",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                MinimumWidth = 40 // <-- MUDANÇA
            });
            DGVFinalizados.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "VencimentoFreeTime",
                HeaderText = "Venc. F. Time",
                DefaultCellStyle = dateCellStyle,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                MinimumWidth = 100 // <-- MUDANÇA
            });
            DGVFinalizados.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "VencimentoFMA",
                HeaderText = "Venc. FMA",
                DefaultCellStyle = dateCellStyle,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                MinimumWidth = 100 // <-- MUDANÇA
            });
            DGVFinalizados.Columns.Add(new DataGridViewCheckBoxColumn
            {
                DataPropertyName = "CapaOK",
                HeaderText = "Capa",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader,
                MinimumWidth = 40
            });
            DGVFinalizados.Columns.Add(new DataGridViewCheckBoxColumn
            {
                DataPropertyName = "Numerario",
                HeaderText = "Num.",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader,
                MinimumWidth = 40
            });
            DGVFinalizados.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "RascunhoDI",
                HeaderText = "Rascunho DI",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                MinimumWidth = 120
            });
            DGVFinalizados.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Pendencia",
                HeaderText = "Pendência",
                FillWeight = 160,
                MinimumWidth = 160 // <-- MUDANÇA
            });
            DGVFinalizados.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Status",
                HeaderText = "Status",
                FillWeight = 180,
                MinimumWidth = 120 // <-- MUDANÇA
            });

            foreach (DataGridViewColumn coluna in DGVFinalizados.Columns)
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
        }
        private async void DGVFinalizados_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || BsProcesso.Current is not Processo processoSelecionado) return;

            using var frm = new FrmModificaProcesso { processo = processoSelecionado, Visualização = true, Modo = "Visualizar" };
            frm.ShowDialog();

            await CarregarDadosAsync();
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

        private void FrmFinalizados_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                TxtPesquisar.PerformClick();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
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
        private void PopularComboBoxDePesquisa()
        {
            var camposIgnorados = new HashSet<string>
            {
                "Id", "OrgaosAnuentesEnvolvidos",
                "PossuiEmbarque", "VencimentoFreeTime", "VencimentoFMA"
            };

            CmbPesquisar.Items.Clear();

            foreach (DataGridViewColumn coluna in DGVFinalizados.Columns)
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
}

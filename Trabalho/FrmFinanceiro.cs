using CLUSA.Repositories;
using CLUSA.Models;

namespace Trabalho
{
    public partial class FrmFinanceiro : Form
    {
        private readonly RepositorioFatura _repositorioFatura;
        private readonly RepositorioRecibo _repositorioRecibo;

        public FrmFinanceiro()
        {
            InitializeComponent();
            _repositorioFatura = new();
            _repositorioRecibo = new();
            _bsFaturas = new BindingSource();
            _bsRecibos = new BindingSource();
        }

        private async void FrmFinanceiro_Shown(object? sender, EventArgs e)
        {
            ConfigurarGrids();
            await CarregarDadosAsync();
        }

        /// <summary>
        /// Configura as colunas e o comportamento das duas grades.
        /// </summary>
        private void ConfigurarGrids()
        {
            // --- Configuração da Grade de Faturamento ---
            DGVFaturamento.DataSource = _bsFaturas;
            DGVFaturamento.AutoGenerateColumns = false;
            DGVFaturamento.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            DGVFaturamento.RowHeadersVisible = false;
            DGVFaturamento.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            DGVFaturamento.AllowUserToAddRows = false;
            DGVFaturamento.ReadOnly = true;

            DGVFaturamento.Columns.Clear();
            DGVFaturamento.Columns.AddRange(new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn { DataPropertyName = "Ref_USA", HeaderText = "Ref. USA", MinimumWidth = 20 },
                new DataGridViewTextBoxColumn { DataPropertyName = "Importador", HeaderText = "Importador", MinimumWidth = 100 },
                new DataGridViewTextBoxColumn { DataPropertyName = "Saldo", HeaderText = "Saldo", FillWeight = 50, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" } },
                new DataGridViewTextBoxColumn { DataPropertyName = "TipoFinalizacao", HeaderText = "Tipo", FillWeight = 40 }
            });
        }

        /// <summary>
        /// Busca os dados do banco e os vincula às grades.
        /// </summary>
        private async Task CarregarDadosAsync()
        {
            try
            {
                var listaDeFaturas = await _repositorioFatura.FindRefAsync();
                _bsFaturas.DataSource = listaDeFaturas;
                _bsFaturas.ResetBindings(false);

                var listaDeRecibos = await _repositorioRecibo.FindRefAsync();
                _bsRecibos.DataSource = listaDeRecibos;
                _bsRecibos.ResetBindings(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar os dados: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Abre a tela de edição quando o usuário dá um duplo clique em uma fatura.
        /// </summary>
        private void DGVFaturamento_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || _bsFaturas.Current is not Fatura faturaSelecionada) return;

            using var detalhesForm = new DetalhesForm(faturaSelecionada.Ref_USA, faturaSelecionada.Importador, TipoDocumentoFinanceiro.Fatura);
            detalhesForm.ShowDialog();
            // Após fechar, pode ser necessário recarregar os dados do painel.
            _ = CarregarDadosAsync();
        }

        /// <summary>
        /// Abre a tela de edição quando o usuário dá um duplo clique em um recibo.
        /// </summary>
        private void DGVRecibo_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || _bsRecibos.Current is not Recibo reciboSelecionado) return;

            // MUDANÇA: Chama o novo formulário unificado, especificando o tipo.
            using var detalhesForm = new DetalhesForm(reciboSelecionado.Ref_USA, reciboSelecionado.Importador, TipoDocumentoFinanceiro.Recibo);
            detalhesForm.ShowDialog();
            // Após fechar, pode ser necessário recarregar os dados do painel.
            _ = CarregarDadosAsync();

        }
    }
}
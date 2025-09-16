using CLUSA; // Sua namespace dos modelos e repositórios
using System.Windows.Forms;

namespace Trabalho
{
    public partial class FrmOrgaoAnuente : Form
    {
        private readonly RepositorioOrgaoAnuente _repositorioOrgaoAnuente;
        private readonly RepositorioProcesso _repositorioProcesso;
        private readonly BindingSource _bsOrgaosViewModel;
        private List<OrgaoAnuenteViewModel> _listaOriginalViewModel = new();

        public FrmOrgaoAnuente()
        {
            InitializeComponent();
            _repositorioOrgaoAnuente = new RepositorioOrgaoAnuente();
            _repositorioProcesso = new RepositorioProcesso();
            _bsOrgaosViewModel = new BindingSource();
        }

        private async void FrmOrgaoAnuente_Shown(object? sender, EventArgs e)
        {
            try
            {
                ConfigurarDataGridView();
                await CarregarDadosAsync();
                PopularComboBoxPesquisa();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar o formulário: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigurarDataGridView()
        {
            DgvOrgaoAnuente.AutoGenerateColumns = false;
            DgvOrgaoAnuente.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            DgvOrgaoAnuente.RowHeadersVisible = false;
            DgvOrgaoAnuente.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            DgvOrgaoAnuente.AllowUserToAddRows = false;
            DgvOrgaoAnuente.ReadOnly = true;

            DgvOrgaoAnuente.Columns.AddRange(new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn { DataPropertyName = "Tipo", HeaderText = "Órgão", FillWeight = 30 },
                new DataGridViewTextBoxColumn { DataPropertyName = "Ref_USA", HeaderText = "Ref. USA", FillWeight = 40 },
                new DataGridViewTextBoxColumn { DataPropertyName = "Importador", HeaderText = "Importador", FillWeight = 80 },
                new DataGridViewTextBoxColumn { DataPropertyName = "Pendencia", HeaderText = "Pendência", FillWeight = 100 },
                new DataGridViewTextBoxColumn { DataPropertyName = "StatusDoProcesso", HeaderText = "Status", FillWeight = 100 },
                new DataGridViewTextBoxColumn { DataPropertyName = "Inspecao", HeaderText = "Inspeção", DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" }, FillWeight = 40 }
            });

            // Atribuir o BindingSource ao DataGridView
            DgvOrgaoAnuente.DataSource = _bsOrgaosViewModel;
        }

        private async Task CarregarDadosAsync()
        {
            // 1. Busca todos os órgãos e todos os processos
            var todosOrgaos = await _repositorioOrgaoAnuente.GetAllAsync();
            var todosProcessos = await _repositorioProcesso.ListarTodosAsync();

            // 2. Cria um dicionário de processos para busca rápida (muito mais eficiente)
            var processosDict = todosProcessos.ToDictionary(p => p.Ref_USA);

            // 3. Monta a lista de ViewModel, combinando os dados
            _listaOriginalViewModel = todosOrgaos.Select(orgao =>
            {
                // Tenta encontrar o processo correspondente no dicionário
                processosDict.TryGetValue(orgao.Ref_USA, out var processoCorrespondente);

                return new OrgaoAnuenteViewModel
                {
                    OrgaoAnuenteId = orgao.Id,
                    Tipo = orgao.Tipo,
                    Ref_USA = orgao.Ref_USA,
                    Pendencia = orgao.Pendencia,
                    StatusDoProcesso = orgao.StatusDoProcesso,
                    Inspecao = orgao.Inspecao,
                    Importador = processoCorrespondente?.Importador ?? "N/A" // Se não encontrar o processo, exibe "N/A"
                };
            }).ToList();

            _bsOrgaosViewModel.DataSource = new List<OrgaoAnuenteViewModel>(_listaOriginalViewModel);
            _bsOrgaosViewModel.ResetBindings(false);
        }

        private void PopularComboBoxPesquisa()
        {
            CbPesquisa.Items.Clear();
            foreach (DataGridViewColumn coluna in DgvOrgaoAnuente.Columns)
            {
                if (!string.IsNullOrEmpty(coluna.DataPropertyName))
                {
                    CbPesquisa.Items.Add(new DisplayItem(coluna.DataPropertyName, coluna.HeaderText));
                }
            }
            if (CbPesquisa.Items.Count > 0)
            {
                CbPesquisa.SelectedIndex = 0;
            }
        }

        private async void BtnEditar_Click(object? sender, EventArgs e)
        {
            if (_bsOrgaosViewModel.Current is not OrgaoAnuenteViewModel viewModel)
            {
                MessageBox.Show("Nenhum item selecionado para edição.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // 1. Busca os objetos originais do banco
                var orgaoParaEditar = await _repositorioOrgaoAnuente.GetByIdAsync(viewModel.OrgaoAnuenteId.ToString());
                if (orgaoParaEditar == null) throw new Exception("O item selecionado não foi encontrado no banco.");

                var processo = await _repositorioProcesso.GetByRefUsaAsync(orgaoParaEditar.Ref_USA);
                if (processo == null) throw new Exception($"Processo principal '{orgaoParaEditar.Ref_USA}' não encontrado.");

                // 2. Abre o formulário de edição
                using var frm = new FrmModificaOrgaoAnuente { OrgaoAnuente = orgaoParaEditar, Processo = processo };

                // 3. Se o formulário de edição salvou com sucesso (retornou OK)...
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    // 4. ...apenas recarregamos os dados na grade para exibir as alterações.
                    await CarregarDadosAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao abrir a tela de edição: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnPesquisar_Click(object? sender, EventArgs e)
        {
            if (CbPesquisa.SelectedItem is not DisplayItem campoSelecionado || string.IsNullOrWhiteSpace(TxtPesquisa.Text))
            {
                BtnCancelar_Click(sender, e); // Se a pesquisa for vazia, restaura a lista
                return;
            }

            string pesquisa = TxtPesquisa.Text.ToLowerInvariant();

            // Usando reflection para tornar a pesquisa genérica
            var propriedade = typeof(OrgaoAnuenteViewModel).GetProperty(campoSelecionado.DataPropertyName);
            if (propriedade == null) return;

            var resultados = _listaOriginalViewModel.Where(vm =>
            {
                var valor = propriedade.GetValue(vm)?.ToString() ?? "";
                return valor.ToLowerInvariant().Contains(pesquisa);
            }).ToList();

            _bsOrgaosViewModel.DataSource = resultados;
            _bsOrgaosViewModel.ResetBindings(false);
        }

        private void BtnCancelar_Click(object? sender, EventArgs e)
        {
            TxtPesquisa.Clear();
            _bsOrgaosViewModel.DataSource = new List<OrgaoAnuenteViewModel>(_listaOriginalViewModel);
            _bsOrgaosViewModel.ResetBindings(false);
        }

        // Classe auxiliar para o ComboBox
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
}
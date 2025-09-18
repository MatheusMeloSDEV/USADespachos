using CLUSA; // Sua namespace dos modelos e repositórios
using System.Windows.Forms;

namespace Trabalho
{
    public partial class FrmOrgaoAnuente : Form
    {
        private readonly RepositorioOrgaoAnuente _repositorioOrgaoAnuente;
        private readonly RepositorioProcesso _repositorioProcesso;
        private readonly BindingSource _bsLpcoViewModel;
        private List<LpcoViewModel> _listaOriginalViewModel = new();

        public FrmOrgaoAnuente()
        {
            InitializeComponent();
            _repositorioOrgaoAnuente = new RepositorioOrgaoAnuente();
            _repositorioProcesso = new RepositorioProcesso();
            _bsLpcoViewModel = new BindingSource();
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
            DgvOrgaoAnuente.DataSource = _bsLpcoViewModel;
            DgvOrgaoAnuente.AutoGenerateColumns = false;
            DgvOrgaoAnuente.RowHeadersVisible = false;
            DgvOrgaoAnuente.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            DgvOrgaoAnuente.AllowUserToAddRows = false;
            DgvOrgaoAnuente.ReadOnly = true;
            DgvOrgaoAnuente.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            DgvOrgaoAnuente.Columns.Clear();

            // --- Colunas Principais de Identificação ---
            DgvOrgaoAnuente.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Ref_USA",
                HeaderText = "Ref. USA",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                MinimumWidth = 90
            });
            DgvOrgaoAnuente.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Importador",
                HeaderText = "Importador",
                FillWeight = 150 // Mais espaço para nomes longos
            });
            DgvOrgaoAnuente.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "NumeroLI",
                HeaderText = "Nº da LI",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                MinimumWidth = 90
            });
            DgvOrgaoAnuente.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "LPCO",
                HeaderText = "Nº do LPCO",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                MinimumWidth = 100
            });
            DgvOrgaoAnuente.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "NomeOrgao",
                HeaderText = "Órgão",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            });

            // --- Colunas de Dados da Carga ---
            DgvOrgaoAnuente.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Produto",
                HeaderText = "Produto",
                FillWeight = 150 // Mais espaço para nomes longos
            });
            DgvOrgaoAnuente.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Container",
                HeaderText = "Container",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                MinimumWidth = 110
            });
            DgvOrgaoAnuente.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Conhecimento",
                HeaderText = "Conhecimento",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                MinimumWidth = 110
            });
            DgvOrgaoAnuente.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Origem",
                HeaderText = "Origem",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                MinimumWidth = 80
            });

            // --- Colunas do LPCO ---

            // --- Colunas de Datas ---
            DgvOrgaoAnuente.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "DataChegada",
                HeaderText = "Chegada",
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" },
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            });
            DgvOrgaoAnuente.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Inspecao",
                HeaderText = "Inspeção",
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" },
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            });

            // --- Colunas de Status ---
            DgvOrgaoAnuente.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "HistoricoDoProcesso",
                HeaderText = "Histórico Do Processo",
                FillWeight = 120
            });
            DgvOrgaoAnuente.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Pendencia",
                HeaderText = "Pendência",
                FillWeight = 180 
            });
        }

        private async Task CarregarDadosAsync()
        {
            // 1. Busca APENAS a lista de LIs (OrgaosAnuentes). Não precisamos mais buscar os Processos aqui.
            var todasAsLIs = await _repositorioOrgaoAnuente.GetAllAsync();

            // 2. Mapeia e "achata" a estrutura para o ViewModel de forma direta.
            _listaOriginalViewModel = todasAsLIs
                .SelectMany(li =>
                    // Se uma LI não tiver LPCOs, cria um LPCO "fantasma" para que a LI ainda apareça na grade.
                    (li.LPCO.Any() ? li.LPCO : new List<LpcoInfo> { new LpcoInfo() })
                    .Select(lpco => new LpcoViewModel
                    {
                        // Todos os dados agora vêm do objeto 'li' (OrgaoAnuente), que já é completo.
                        OrgaoAnuenteId = li.Id,
                        Ref_USA = li.Ref_USA,
                        Importador = li.Importador,
                        NumeroLI = li.Numero,
                        Produto = li.Produto,
                        Container = li.Container,
                        Origem = li.Origem,
                        Conhecimento = li.Conhecimento,
                        DataChegada = li.DataChegada,
                        Inspecao = li.Inspecao,
                        HistoricoDoProcesso = li.HistoricoDoProcesso,
                        Pendencia = li.Pendencia,

                        // Dados específicos do LPCO
                        NomeOrgao = lpco.NomeOrgao,
                        LPCO = lpco.LPCO,
                        DataRegistroLPCO = lpco.DataRegistroLPCO,
                        ParametrizacaoLPCO = lpco.ParametrizacaoLPCO
                    }))
                .ToList();

            _bsLpcoViewModel.DataSource = new List<LpcoViewModel>(_listaOriginalViewModel);
            _bsLpcoViewModel.ResetBindings(false);
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
            if (_bsLpcoViewModel.Current is not LpcoViewModel viewModel)
            {
                MessageBox.Show("Nenhum item selecionado para edição.", "Aviso");
                return;
            }

            try
            {
                // 1. Busca os objetos mais recentes do banco de dados
                var orgaoParaEditar = await _repositorioOrgaoAnuente.GetByIdAsync(viewModel.OrgaoAnuenteId.ToString());
                if (orgaoParaEditar == null)
                {
                    MessageBox.Show("O item selecionado não foi encontrado no banco. A lista será atualizada.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    await CarregarDadosAsync();
                    return;
                }

                var processo = await _repositorioProcesso.GetByRefUsaAsync(orgaoParaEditar.Ref_USA);
                if (processo == null)
                {
                    MessageBox.Show($"Processo principal '{orgaoParaEditar.Ref_USA}' não encontrado.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // 2. Abre a tela de edição, que modificará o objeto 'orgaoParaEditar' em memória
                using var frm = new FrmModificaOrgaoAnuente { OrgaoAnuente = orgaoParaEditar, Processo = processo };

                // 3. Se o formulário de edição foi fechado com "OK"...
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    // --- INÍCIO DA LÓGICA DE SALVAMENTO QUE ESTAVA FALTANDO ---

                    // 4. Salva o objeto OrgaoAnuente (que foi modificado pelo frm) de volta no banco.
                    await _repositorioOrgaoAnuente.UpdateAsync(orgaoParaEditar);

                    // 5. Sincroniza a mudança de volta para a lista de LIs do Processo principal.
                    // Encontra a LI correspondente na lista do Processo pelo número.
                    var liNoProcesso = processo.LI.FirstOrDefault(li => li.Numero == orgaoParaEditar.Numero);
                    if (liNoProcesso != null)
                    {
                        // Copia os dados atualizados para o objeto dentro da lista do processo
                        // (Isso é importante para manter a consistência da "fonte da verdade")
                        liNoProcesso.NCM = orgaoParaEditar.NCM;
                        liNoProcesso.DataRegistro = orgaoParaEditar.DataRegistro;
                        liNoProcesso.LPCO = orgaoParaEditar.LPCO;
                    }

                    processo.HistoricoDoProcesso = orgaoParaEditar.HistoricoDoProcesso;
                    processo.Pendencia = orgaoParaEditar.Pendencia;

                    // 6. Salva o objeto Processo principal, agora com a LI interna atualizada.
                    await _repositorioProcesso.UpdateAsync(processo);

                    MessageBox.Show("Alterações salvas com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // --- FIM DA LÓGICA DE SALVAMENTO ---

                    // 7. Recarrega os dados na grade para exibir as alterações salvas.
                    await CarregarDadosAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao editar o órgão anuente: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            var propriedade = typeof(LpcoViewModel).GetProperty(campoSelecionado.DataPropertyName);
            if (propriedade == null) return;

            var resultados = _listaOriginalViewModel.Where(vm =>
            {
                var valor = propriedade.GetValue(vm)?.ToString() ?? "";
                return valor.ToLowerInvariant().Contains(pesquisa);
            }).ToList();

            _bsLpcoViewModel.DataSource = resultados;
            _bsLpcoViewModel.ResetBindings(false);
        }

        private void BtnCancelar_Click(object? sender, EventArgs e)
        {
            TxtPesquisa.Clear();
            _bsLpcoViewModel.DataSource = new List<LpcoViewModel>(_listaOriginalViewModel);
            _bsLpcoViewModel.ResetBindings(false);
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
using CLUSA; // Sua namespace dos modelos e repositórios
using System.ComponentModel;
using System.Windows.Forms;

namespace Trabalho
{
    public partial class FrmOrgaoAnuente : Form
    {
        private readonly RepositorioOrgaoAnuente _repositorioOrgaoAnuente;
        private readonly RepositorioProcesso _repositorioProcesso;
        private readonly BindingSource _bsLpcoViewModel;
        private List<LpcoViewModel> _listaOriginalViewModel = new();
        private DataGridViewColumn? _colunaOrdenada;
        private ListSortDirection _direcaoOrdenacao;

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
                AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader,
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
                DataPropertyName = "MotivoExigencia", 
                HeaderText = "Motivo Exigência", 
                FillWeight = 90 
            });
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
            var todasAsLIs = await _repositorioOrgaoAnuente.GetAllAsync();

            _listaOriginalViewModel = todasAsLIs
                .SelectMany(li =>
                    (li.LPCO.Any() ? li.LPCO : new List<LpcoInfo> { new LpcoInfo() })
                    .Select(lpco => new LpcoViewModel
                    {
                        // IDs para referência futura
                        OrgaoAnuenteId = li.Id,

                        // Dados herdados da LI (OrgaoAnuente)
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
                        StatusLI = li.StatusLI,

                        // Dados específicos do LPCO
                        NomeOrgao = lpco.NomeOrgao,
                        LPCO = lpco.LPCO,
                        DataRegistroLPCO = lpco.DataRegistroLPCO,
                        ParametrizacaoLPCO = lpco.ParametrizacaoLPCO,
                        MotivoExigencia = lpco.MotivoExigencia?.ToUpper() == "PENDENTE"
                                ? $"{lpco.NomeOrgao} {lpco.MotivoExigencia.ToUpper()}" // Ex: "MAPA PENDENTE"
                                : lpco.MotivoExigencia, // Ex: "CUMPRIDA" ou ""
                    }))
                .ToList();

            // MUDANÇA: A ordenação agora usa o ViewModel completo
            var listaOrdenada = _listaOriginalViewModel
                .OrderBy(vm => GetStatusPriority(vm)) // 1. Prioridade por Status/Exigência
                .ThenByDescending(vm => ExtrairAnoNumero(vm.Ref_USA).ano)
                .ThenBy(vm => ExtrairAnoNumero(vm.Ref_USA).numero)
                .ToList();

            _bsLpcoViewModel.DataSource = listaOrdenada;
            _bsLpcoViewModel.ResetBindings(false);
        }

        // Função auxiliar para extrair ano e número do formato 0000/00
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
        #region "Lógica de Ordenação e Coloração"

        /// <summary>
        /// Define a prioridade numérica para cada LINHA, usada para a ordenação.
        /// Números menores aparecem primeiro.
        /// </summary>
        private int GetStatusPriority(LpcoViewModel viewModel)
        {
            // MUDANÇA: A ordem de verificação agora segue a sua nova prioridade.

            // 1. Prioridade 1: VERDE
            if (viewModel.StatusLI?.ToUpper() == "PRONTO PARA ENTRADA")
            {
                return 1;
            }

            // 2. Prioridade 2: VERMELHO
            if (viewModel.MotivoExigencia?.ToUpper().Contains("PENDENTE") == true)
            {
                return 2;
            }

            // 3. Prioridade 3: AMARELO
            if (viewModel.StatusLI?.ToUpper() == "PENDÊNCIA DOCUMENTAL")
            {
                return 3;
            }

            // 5. Prioridade 5: Todos os outros
            return 5;
        }
        private void DgvOrgaoAnuente_DataBindingComplete(object? sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow row in DgvOrgaoAnuente.Rows)
            {
                if (row.DataBoundItem is LpcoViewModel viewModel)
                {
                    // Define a cor padrão primeiro
                    row.DefaultCellStyle.BackColor = SystemColors.Window;
                    row.DefaultCellStyle.ForeColor = SystemColors.ControlText;

                    // MUDANÇA: Aplica a nova hierarquia de cores
                    if (viewModel.MotivoExigencia?.ToUpper().Contains("PENDENTE") == true)
                    {
                        row.DefaultCellStyle.BackColor = Color.LightCoral; // Vermelho claro
                    }
                    else if (viewModel.StatusLI?.ToUpper() == "PRONTO PARA ENTRADA")
                    {
                        row.DefaultCellStyle.BackColor = Color.LightGreen;
                    }
                    else if (viewModel.StatusLI?.ToUpper() == "PENDÊNCIA DOCUMENTAL")
                    {
                        row.DefaultCellStyle.BackColor = Color.LightYellow;
                    }
                }
            }
        }
        private void DgvOrgaoAnuente_ColumnHeaderMouseClick(object? sender, DataGridViewCellMouseEventArgs e)
        {
            if (sender is not DataGridView dgv) return;
            var novaColuna = dgv.Columns[e.ColumnIndex];

            if (_bsLpcoViewModel.DataSource is not List<LpcoViewModel> listaParaOrdenar) return;

            // 1. Determina a Direção
            ListSortDirection direcao = ListSortDirection.Ascending;
            if (_colunaOrdenada?.Name == novaColuna.Name && _direcaoOrdenacao == ListSortDirection.Ascending)
            {
                direcao = ListSortDirection.Descending;
            }

            _colunaOrdenada = novaColuna;
            _direcaoOrdenacao = direcao;

            // 2. Lógica de Ordenação Genérica para o ViewModel
            var propInfo = typeof(LpcoViewModel).GetProperty(novaColuna.DataPropertyName);
            if (propInfo == null) return;

            // NÍVEL 1: Ordena jogando valores vazios/nulos para o final
            var listaOrdenada = listaParaOrdenar
                .OrderBy(vm => IsValueEmpty(propInfo.GetValue(vm)));

            // NÍVEL 2: Aplica a ordenação principal na coluna clicada
            if (direcao == ListSortDirection.Ascending)
            {
                listaOrdenada = listaOrdenada.ThenBy(vm => propInfo.GetValue(vm));
            }
            else
            {
                listaOrdenada = listaOrdenada.ThenByDescending(vm => propInfo.GetValue(vm));
            }

            _bsLpcoViewModel.DataSource = listaOrdenada.ToList();
            _bsLpcoViewModel.ResetBindings(false);

            // 3. Atualiza a Seta Visual (Glyph) no Cabeçalho
            foreach (DataGridViewColumn column in dgv.Columns)
            {
                column.HeaderCell.SortGlyphDirection = (column.Name == novaColuna.Name)
                    ? (direcao == ListSortDirection.Ascending ? SortOrder.Ascending : SortOrder.Descending)
                    : SortOrder.None;
            }
        }

        #endregion
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

                using (var frm = new FrmModificaOrgaoAnuente(_repositorioOrgaoAnuente, _repositorioProcesso))
                {
                    frm.OrgaoAnuente = orgaoParaEditar;
                    frm.Processo = processo;
                    // frm.IsViewOnly = true; // Se for modo de visualização

                    // Agora, a lógica de salvamento está DENTRO do frm.
                    // Nós só precisamos verificar se o usuário clicou em OK.
                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        // Se o usuário clicou OK, os dados já foram salvos.
                        // Agora só precisamos atualizar a grade na tela principal.
                        await CarregarDadosAsync();
                    }
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
        private async void DgvOrgaoAnuente_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || DgvOrgaoAnuente.Rows[e.RowIndex].DataBoundItem is not LpcoViewModel viewModel) return;

            try
            {
                // 3. Busca os dados mais recentes do banco, assim como no botão Editar
                var orgaoParaVisualizar = await _repositorioOrgaoAnuente.GetByIdAsync(viewModel.OrgaoAnuenteId.ToString());
                if (orgaoParaVisualizar == null)
                {
                    MessageBox.Show("O item selecionado não foi encontrado no banco. A lista será atualizada.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    await CarregarDadosAsync();
                    return;
                }

                var processo = await _repositorioProcesso.GetByRefUsaAsync(orgaoParaVisualizar.Ref_USA);
                if (processo == null)
                {
                    MessageBox.Show($"Processo principal '{orgaoParaVisualizar.Ref_USA}' não encontrado.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Abre o formulário no modo de visualização
                using (var frm = new FrmModificaOrgaoAnuente(_repositorioOrgaoAnuente, _repositorioProcesso))
                {
                    frm.Processo = processo;
                    frm.OrgaoAnuente = orgaoParaVisualizar;
                    frm.IsViewOnly = true;
                    frm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao abrir detalhes: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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



        // Substitua seu método de clique no cabeçalho por este
        private void DGV_ColumnHeaderMouseClick(object? sender, DataGridViewCellMouseEventArgs e)
        {
            if (sender is not DataGridView dgv) return;
            var novaColuna = dgv.Columns[e.ColumnIndex];
            if (novaColuna.SortMode == DataGridViewColumnSortMode.NotSortable) return;
            if (_bsLpcoViewModel.DataSource is not List<Processo> listaParaOrdenar) return;

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
            _bsLpcoViewModel.DataSource = listaOrdenada.ToList();
            _bsLpcoViewModel.ResetBindings(false);

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

        private void FrmOrgaoAnuente_Load(object sender, EventArgs e)
        {

        }
    }
}
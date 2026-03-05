using CLUSA.Repositories;
using CLUSA.Models;
using CLUSA.Interfaces;
using System.ComponentModel;

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

        private readonly Logado _logado;
        private readonly RepositorioUsers _repositorioUsers;
        private readonly RepositorioLog _logRepo;
        private Users? _usuarioLogado;

        public FrmOrgaoAnuente(Logado logado)
        {
            InitializeComponent();
            _repositorioOrgaoAnuente = new RepositorioOrgaoAnuente();
            _repositorioProcesso = new RepositorioProcesso();
            _bsLpcoViewModel = new BindingSource();
            DgvOrgaoAnuente.DataSource = _bsLpcoViewModel;

            _repositorioUsers = new RepositorioUsers();
            _logRepo = new RepositorioLog();
            _logado = logado;
        }

        private async void FrmOrgaoAnuente_Shown(object? sender, EventArgs e)
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

                _usuarioLogado.PreferenciasGrids ??= new Dictionary<string, List<string>>();
                _usuarioLogado.PreferenciasGrids.TryGetValue("DgvOrgaoAnuente", out var colunasVisiveis); 

                GridColumnManager.ConfigurarGrid(DgvOrgaoAnuente, "DgvOrgaoAnuente", colunasVisiveis);

                await CarregarDadosAsync();

                this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
                PopularComboBoxPesquisa();
                if (CbPesquisa.Items.Count > 0)
                    CbPesquisa.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar o formulário: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task CarregarDadosAsync()
        {
            Cursor = Cursors.WaitCursor;
            try
            {
                // 1. Busca apenas os Ref_USA dos processos ATIVOS (rápido, traz só string)
                var refsAtivos = await _repositorioProcesso.ListarRefUsaAtivosAsync();

                // 2. BUSCA OTIMIZADA: Traz do MongoDB apenas as LIs que pertencem a esses processos
                // Em vez de trazer TUDO e filtrar na memória.
                var lisAtivas = await _repositorioOrgaoAnuente.GetByListaRefUsaAsync(refsAtivos);

                // 3. Mapeamento em Memória (Rápido pois a lista já veio filtrada do banco)
                var listaMapeada = await Task.Run(() =>
                {
                    return lisAtivas.SelectMany(li =>
                        (li.LPCO != null && li.LPCO.Any() ? li.LPCO : new List<LpcoInfo> { new LpcoInfo() })
                        .Select(lpco => new LpcoViewModel
                        {
                            OrgaoAnuenteId = li.Id,
                            Ref_USA = li.Ref_USA,
                            Importador = li.Importador,
                            NumeroLI = li.Numero,
                            Produto = li.Produto,
                            Container = li.Container,
                            Origem = li.Origem,
                            Conhecimento = li.Conhecimento,
                            Terminal = li.Terminal,
                            DataChegada = li.DataChegada,
                            Inspecao = li.Inspecao,
                            HistoricoDoProcesso = li.HistoricoDoProcesso,
                            Pendencia = li.Pendencia,
                            StatusLPCO = lpco.StatusLPCO,
                            NomeOrgao = lpco.NomeOrgao,
                            LPCO = lpco.LPCO,
                            DataRegistroLPCO = lpco.DataRegistroLPCO,
                            ParametrizacaoLPCO = lpco.ParametrizacaoLPCO,
                            MotivoExigencia = lpco.MotivoExigencia?.ToUpper() == "EXIGÊNCIA PENDENTE"
                                ? $"{lpco.NomeOrgao} {lpco.MotivoExigencia.ToUpper()}"
                                : lpco.MotivoExigencia,
                        }))
                    .ToList();
                });

                // 4. Ordenação
                _listaOriginalViewModel = listaMapeada
                        .OrderBy(vm => GetStatusPriority(vm))
                        .ThenByDescending(vm => ExtrairAnoNumero(vm.Ref_USA).ano)
                        .ThenBy(vm => ExtrairAnoNumero(vm.Ref_USA).numero)
                        .ToList();

                _bsLpcoViewModel.DataSource = _listaOriginalViewModel;
                _bsLpcoViewModel.ResetBindings(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar dados: {ex.Message}");
            }
            finally
            {
                Cursor = Cursors.Default;
            }
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
            if (viewModel.StatusLPCO?.ToUpper() == "PRONTO PARA ENTRADA")
            {
                return 1;
            }

            // 2. Prioridade 2: VERMELHO
            if (viewModel.MotivoExigencia?.ToUpper().Contains("PENDENTE") == true)
            {
                return 2;
            }

            // 3. Prioridade 3: AMARELO
            if (viewModel.StatusLPCO?.ToUpper() == "PENDÊNCIA DOCUMENTAL")
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
                    else if (viewModel.StatusLPCO?.ToUpper() == "PRONTO PARA ENTRADA")
                    {
                        row.DefaultCellStyle.BackColor = Color.LightGreen;
                    }
                    else if (viewModel.StatusLPCO?.ToUpper() == "PENDÊNCIA DOCUMENTAL")
                    {
                        row.DefaultCellStyle.BackColor = Color.Yellow;
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

                using (var frm = new FrmModificaOrgaoAnuente(_repositorioOrgaoAnuente, _repositorioProcesso) { _logadoNome = _logado.Usuario })
                {
                    frm.OrgaoAnuente = orgaoParaEditar;
                    frm.Processo = processo;
                    // frm.IsViewOnly = true; // Se for modo de visualização

                    // Agora, a lógica de salvamento está DENTRO do frm.
                    // Nós só precisamos verificar se o usuário clicou em OK.
                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        await _logRepo.RegistrarLogAsync(
                            "Edição", _logado.Usuario,
                            $"Órgão Anuente (LI {orgaoParaEditar.Numero}) do processo {orgaoParaEditar.Ref_USA} foi atualizado",
                            $"Usuário: {_logado.Usuario}"
                        );
                        await CarregarDadosAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                await _logRepo.RegistrarLogAsync("Erro", _logado.Usuario, "Falha ao abrir edição de Órgão Anuente", ex.Message);
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
        private async void BtnCancelar_Click(object? sender, EventArgs e)
        {
            TxtPesquisa.Clear();

            // MUDANÇA: Em vez de recarregar tudo do banco, apenas restaura
            // a lista original que já está em memória e ordenada por prioridade.
            _bsLpcoViewModel.DataSource = new List<LpcoViewModel>(_listaOriginalViewModel);
            _bsLpcoViewModel.ResetBindings(false);

            // Limpa a indicação visual de ordenação por clique no cabeçalho
            foreach (DataGridViewColumn column in DgvOrgaoAnuente.Columns)
            {
                column.HeaderCell.SortGlyphDirection = SortOrder.None;
            }
            _colunaOrdenada = null; // Reseta o estado da ordenação por clique
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
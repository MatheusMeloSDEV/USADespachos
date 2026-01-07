using CLUSA.Models;
using CLUSA.Repositories;
using System.ComponentModel;
using System.Data;

namespace Trabalho
{
    public partial class frmFinalizados : Form
    {
        private readonly RepositorioProcesso _repositorio;
        private int _estadoOrdenacaoRefUsa = 0;
        private DataGridViewColumn? _colunaOrdenada;
        private ListSortDirection _direcaoOrdenacao;
        private List<Processo> _listaOriginal = new();

        private readonly Logado _logado;
        private readonly RepositorioUsers _repositorioUsers;
        private Users? _usuarioLogado;

        public frmFinalizados(Logado logado)
        {
            _repositorio = new RepositorioProcesso();
            InitializeComponent();
            _repositorioUsers = new RepositorioUsers();
            _logado = logado;
        }
        private async void FrmFinalizados_Shown(object? sender, EventArgs e)
        {
            try
            {
                _usuarioLogado = await _repositorioUsers.GetByIdAsync(_logado.Id);
                if (_usuarioLogado == null)
                {
                    MessageBox.Show("Não foi possível carregar o usuário logado.", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                GridColumnManager.RegistrarCatalogosPadrao();

                _usuarioLogado.PreferenciasGrids ??= new Dictionary<string, List<string>>();
                _usuarioLogado.PreferenciasGrids.TryGetValue("DGVFinalizados", out var colunasVisiveis);

                GridColumnManager.ConfigurarGrid(DGVFinalizados, "DGVFinalizados", colunasVisiveis);

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
                var registros = await _repositorio.ListarFinalizadosAsync();
                var registrosOrdenados = registros
                    .OrderBy(p => p.DataDeAtracacao == null ? 1 : 0) // Nulos para o final (ou início, conforme sua lógica)
                    .ThenBy(p => p.DataDeAtracacao ?? DateTime.MaxValue)
                    .ToList();

                BsProcesso.DataSource = registrosOrdenados;
                DGVFinalizados.DataSource = BsProcesso;
                BsProcesso.ResetBindings(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar os dados: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        // Adicione esta função dentro da classe frmFinalizados

        private void DGVFinalizados_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            // Obtém a fonte de dados atual. Se estiver vazia, não faz nada.
            // IMPORTANTE: Assumimos que o DataSource é uma List<Processo>
            if (BsProcesso.DataSource is not List<Processo> listaAtual || listaAtual.Count == 0) return;

            var coluna = DGVFinalizados.Columns[e.ColumnIndex];
            var propriedade = coluna.DataPropertyName;

            // Se a coluna não tiver propriedade vinculada, ignora
            if (string.IsNullOrWhiteSpace(propriedade)) return;

            // --- 1. Alternância da Direção (Ascendente / Descendente) ---
            ListSortDirection direcao = ListSortDirection.Ascending;

            // Se clicou na mesma coluna que já estava ordenada
            if (_colunaOrdenada != null && _colunaOrdenada.Name == coluna.Name)
            {
                // Inverte a direção
                direcao = (_direcaoOrdenacao == ListSortDirection.Ascending)
                    ? ListSortDirection.Descending
                    : ListSortDirection.Ascending;
            }

            _colunaOrdenada = coluna;
            _direcaoOrdenacao = direcao;

            // --- 2. Lógica de Ordenação ---

            // Usa Reflection para descobrir o tipo da propriedade e obter seu valor
            var propInfo = typeof(Processo).GetProperty(propriedade);
            if (propInfo == null) return;

            List<Processo> listaOrdenada;

            // --- CASO ESPECIAL: REF_USA (formato 0000/00) ---
            if (propriedade == "Ref_USA")
            {
                // Função auxiliar para tratar nulos
                Func<Processo, bool> itjFinalizado = p => (p.Ref_USA?.Trim().EndsWith("ITJ", StringComparison.OrdinalIgnoreCase) ?? false);
                Func<Processo, bool> refUsaVazio = p => string.IsNullOrWhiteSpace(p.Ref_USA);

                // Ordena primeiro jogando os "vazios" e "ITJ" para o final (opcional, mas recomendado)
                var baseQuery = listaAtual
                    .OrderBy(p => itjFinalizado(p) ? 1 : 0) // ITJ no final
                    .ThenBy(p => refUsaVazio(p) ? 1 : 0);   // Vazios mais abaixo

                if (direcao == ListSortDirection.Ascending)
                {
                    listaOrdenada = baseQuery.ThenBy(p => ExtrairAnoNumero(p.Ref_USA)).ToList();
                }
                else
                {
                    listaOrdenada = baseQuery.ThenByDescending(p => ExtrairAnoNumero(p.Ref_USA)).ToList();
                }
            }
            // --- CASO GENÉRICO: Datas, Strings, Números ---
            else
            {
                // Helper para jogar nulos para o final na ordenação ascendente
                // (Você pode ajustar isso se preferir nulos no início)

                if (direcao == ListSortDirection.Ascending)
                {
                    listaOrdenada = listaAtual
                        .OrderBy(p => propInfo.GetValue(p) == null ? 1 : 0) // Nulos pro final
                        .ThenBy(p => propInfo.GetValue(p))
                        .ToList();
                }
                else
                {
                    listaOrdenada = listaAtual
                        .OrderBy(p => propInfo.GetValue(p) == null ? 1 : 0) // Nulos pro final mesmo no desc
                        .ThenByDescending(p => propInfo.GetValue(p))
                        .ToList();
                }
            }

            // --- 3. Atualiza o Grid ---
            BsProcesso.DataSource = listaOrdenada;
            BsProcesso.ResetBindings(false);

            // --- 4. Atualiza a Seta Visual (Glyph) ---
            foreach (DataGridViewColumn col in DGVFinalizados.Columns)
            {
                col.HeaderCell.SortGlyphDirection = (col.Name == coluna.Name)
                    ? (direcao == ListSortDirection.Ascending ? SortOrder.Ascending : SortOrder.Descending)
                    : SortOrder.None;
            }
        }

        // Método auxiliar necessário para ordenar Ref_USA corretamente (ano/numero)
        private (int ano, int numero) ExtrairAnoNumero(string? refUsa)
        {
            if (string.IsNullOrWhiteSpace(refUsa)) return (0, 0);
            var partes = refUsa.Split('/'); // Assume formato "NNNN/AA" ou similar
            int numero = 0, ano = 0;

            // Tenta extrair número (antes da barra)
            if (partes.Length > 0) int.TryParse(partes[0], out numero);

            // Tenta extrair ano (depois da barra)
            if (partes.Length > 1) int.TryParse(partes[1], out ano);

            return (ano, numero);
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

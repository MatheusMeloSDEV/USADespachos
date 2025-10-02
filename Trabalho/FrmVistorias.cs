using CLUSA;
using MongoDB.Driver;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Trabalho
{
    public partial class FrmVistorias : Form
    {
        private readonly VistoriaService _vistoriaService;
        private readonly RepositorioVistorias _repositorioVistorias;
        private readonly RepositorioProcesso _repositorioProcesso;

        private readonly BindingSource _bsAguardandoDef;
        private readonly BindingSource _bsVistoriaAgendada;
        private readonly BindingSource _bsSolicitadoData;
        private readonly BindingSource _bsAguardandoChegada;

        public FrmVistorias()
        {
            InitializeComponent();

            var client = new MongoClient(ConfigDatabase.MongoConnectionString);
            var database = client.GetDatabase(ConfigDatabase.MongoDatabaseName);

            _vistoriaService = new VistoriaService(database);
            _repositorioVistorias = new RepositorioVistorias(database);
            _repositorioProcesso = new RepositorioProcesso();

            _bsAguardandoDef = new BindingSource();
            _bsVistoriaAgendada = new BindingSource();
            _bsSolicitadoData = new BindingSource();
            _bsAguardandoChegada = new BindingSource();
        }

        private async void FrmVistorias_Shown(object? sender, EventArgs e)
        {
            ConfigurarGrids();
            await CarregarDadosAsync();
        }

        private async Task CarregarDadosAsync()
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                await _vistoriaService.SincronizarVistoriasAsync();
                var todasAsVistorias = await _repositorioVistorias.GetAllAsync();

                _bsAguardandoDef.DataSource = todasAsVistorias.Where(v => v.Status == StatusVistoria.AguardandoDeferimento).ToList();
                _bsVistoriaAgendada.DataSource = todasAsVistorias.Where(v => v.Status == StatusVistoria.VistoriaAgendada).ToList();
                _bsSolicitadoData.DataSource = todasAsVistorias.Where(v => v.Status == StatusVistoria.SolicitarDataVistoria).ToList();
                _bsAguardandoChegada.DataSource = todasAsVistorias.Where(v => v.Status == StatusVistoria.AguardandoChegadaParaAgendar).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar vistorias: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void ConfigurarGrids()
        {
            // Vincula cada BindingSource à sua respectiva grade
            DGVAguardandoDef.DataSource = _bsAguardandoDef;
            DGVVistoriaAgendada.DataSource = _bsVistoriaAgendada;
            DGVSolicitadoDataVistoria.DataSource = _bsSolicitadoData;
            DGVAguardandoChegAgendVistoria.DataSource = _bsAguardandoChegada;

            // MUDANÇA: Chama a configuração para TODAS as 4 grades
            ConfigurarGrid(DGVAguardandoChegAgendVistoria);
            ConfigurarGrid(DGVSolicitadoDataVistoria);
            ConfigurarGrid(DGVVistoriaAgendada);
            ConfigurarGrid(DGVAguardandoDef);
        }

        /// <summary>
        /// Método auxiliar que aplica um layout padrão e colunas a um DataGridView.
        /// </summary>
        private void ConfigurarGrid(DataGridView dgv)
        {
            dgv.AutoGenerateColumns = false;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.RowHeadersVisible = false;
            dgv.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dgv.AllowUserToAddRows = false;
            dgv.ReadOnly = false;

            // MUDANÇA 1: Conecta o novo evento para configurar a caixa de edição.
            dgv.EditingControlShowing += DGV_EditingControlShowing;
            dgv.CellValueChanged += DGV_CellValueChanged;

            // MUDANÇA 2: Permite que a altura das linhas se ajuste ao conteúdo.
            dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            dgv.Columns.Clear();
            dgv.Columns.AddRange(new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn { DataPropertyName = "LI", HeaderText = "L.I.", ReadOnly = true, AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells, MinimumWidth = 90 },
                new DataGridViewTextBoxColumn { DataPropertyName = "LPCO", HeaderText = "LPCO", ReadOnly = true, FillWeight = 120, AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells, MinimumWidth = 120 },
                new DataGridViewTextBoxColumn { DataPropertyName = "Importador", HeaderText = "Importador", ReadOnly = true, FillWeight = 120, AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells, MinimumWidth = 120 },
                new DataGridViewTextBoxColumn { DataPropertyName = "Container", HeaderText = "Container", ReadOnly = true, AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells, MinimumWidth = 110 },
                new DataGridViewTextBoxColumn { DataPropertyName = "Ref_USA", HeaderText = "Ref. USA", ReadOnly = true, AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells, MinimumWidth = 90 },
                new DataGridViewTextBoxColumn { DataPropertyName = "Produto", HeaderText = "Produto", ReadOnly = true, FillWeight = 150, MinimumWidth = 150 },
                new DataGridViewTextBoxColumn { DataPropertyName = "Terminal", HeaderText = "Terminal", ReadOnly = true, AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells, FillWeight = 150, MinimumWidth = 150 },
                new DataGridViewTextBoxColumn { DataPropertyName = "Conhecimento", HeaderText = "Conhecimento", ReadOnly = true, AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells, MinimumWidth = 110 },
                new DataGridViewTextBoxColumn { DataPropertyName = "ParametrizacaoLPCO", HeaderText = "Status LPCO", ReadOnly = true, FillWeight = 100, AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells, MinimumWidth = 100 },

                new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "Notas",
                    HeaderText = "Notas",
                    ReadOnly = false,
                    FillWeight = 180,
                    MinimumWidth = 150,
                    // MUDANÇA 3: Permite que o texto na célula "Notas" quebre a linha.
                    DefaultCellStyle = new DataGridViewCellStyle { WrapMode = DataGridViewTriState.True }
                }
            });
        }
        private void DGV_EditingControlShowing(object? sender, DataGridViewEditingControlShowingEventArgs e)
        {
            // 1. Identifica qual DataGridView disparou o evento. Se não for um, sai.
            if (sender is not DataGridView dgv) return;

            // 2. Garante que a célula atual não seja nula.
            if (dgv.CurrentCell == null) return;

            // 3. Verifica se a célula que está sendo editada é um TextBox.
            if (e.Control is TextBox editingTextBox)
            {
                // 4. Verifica se a coluna da célula atual é a de "Notas".
                if (dgv.CurrentCell.OwningColumn.DataPropertyName == "Notas")
                {
                    // Se for, permite múltiplas linhas e a tecla Enter.
                    editingTextBox.Multiline = true;
                    editingTextBox.AcceptsReturn = true;
                    editingTextBox.WordWrap = true;
                }
                else
                {
                    // Se não for, garante que outras colunas de texto continuem com uma única linha.
                    editingTextBox.Multiline = false;
                    editingTextBox.AcceptsReturn = false;
                    editingTextBox.WordWrap = false;
                }
            }
        }
        /// <summary>
        /// Evento disparado quando o usuário termina de editar uma célula.
        /// </summary>
        private async void DGV_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
        {
            // Garante que o evento foi disparado por uma grade e em uma linha válida
            if (sender is not DataGridView dgv || e.RowIndex < 0) return;

            // Pega o nome da coluna que foi editada
            string nomeColunaEditada = dgv.Columns[e.ColumnIndex].DataPropertyName;

            // Se a coluna editada não for "Notas", não faz nada.
            if (nomeColunaEditada != "Notas") return;

            // Pega o objeto Vistoria da linha que foi alterada
            if (dgv.Rows[e.RowIndex].DataBoundItem is Vistoria vistoriaEditada)
            {
                try
                {
                    // Salva o objeto inteiro (com a nota atualizada) no banco de dados.
                    await _repositorioVistorias.UpsertAsync(vistoriaEditada);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao salvar a nota: {ex.Message}", "Erro de Salvamento", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    // Opcional: recarregar os dados para reverter a alteração visual
                    await CarregarDadosAsync();
                }
            }
        }
        #region "Lógica de Movimentação de Vistorias"

        /// <summary>
        /// Move a vistoria selecionada de uma grade de origem para uma de destino.
        /// </summary>
        /// <param name="dgvOrigem">A DataGridView de onde o item sairá.</param>
        /// <param name="bsOrigem">O BindingSource da grade de origem.</param>
        /// <param name="bsDestino">O BindingSource da grade de destino.</param>
        /// <param name="novoStatus">O novo status a ser atribuído à vistoria.</param>
        private async Task MoverVistoria(DataGridView dgvOrigem, BindingSource bsOrigem, BindingSource bsDestino, StatusVistoria novoStatus)
        {
            if (dgvOrigem.CurrentRow == null || dgvOrigem.CurrentRow.DataBoundItem is not Vistoria vistoriaSelecionada)
            {
                MessageBox.Show("Por favor, selecione um item para mover.", "Aviso");
                return;
            }

            try
            {
                vistoriaSelecionada.Status = novoStatus;
                await _repositorioVistorias.UpsertAsync(vistoriaSelecionada);

                bsOrigem.Remove(vistoriaSelecionada);
                bsDestino.Add(vistoriaSelecionada);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao atualizar o status da vistoria: {ex.Message}", "Erro");
                await CarregarDadosAsync();
            }
        }

        // --- MÉTODOS PARA SUBIR DE ESTÁGIO ---
        private async void BtnSobeSolicitado_Click(object? sender, EventArgs e)
        {
            await MoverVistoria(DGVAguardandoChegAgendVistoria, _bsAguardandoChegada, _bsSolicitadoData, StatusVistoria.SolicitarDataVistoria);
        }

        private async void BtnSobeAgendada_Click(object? sender, EventArgs e)
        {
            await MoverVistoria(DGVSolicitadoDataVistoria, _bsSolicitadoData, _bsVistoriaAgendada, StatusVistoria.VistoriaAgendada);
        }

        private async void BtnSobeAguardDef_Click(object? sender, EventArgs e)
        {
            await MoverVistoria(DGVVistoriaAgendada, _bsVistoriaAgendada, _bsAguardandoDef, StatusVistoria.AguardandoDeferimento);
        }

        // --- NOVOS MÉTODOS PARA DESCER DE ESTÁGIO ---
        private async void btnDesceParaAgendada_Click(object? sender, EventArgs e)
        {
            await MoverVistoria(DGVAguardandoDef, _bsAguardandoDef, _bsVistoriaAgendada, StatusVistoria.VistoriaAgendada);
        }

        private async void btnDesceParaSolicitado_Click(object? sender, EventArgs e)
        {
            await MoverVistoria(DGVVistoriaAgendada, _bsVistoriaAgendada, _bsSolicitadoData, StatusVistoria.SolicitarDataVistoria);
        }

        private async void btnDesceParaAguardando_Click(object? sender, EventArgs e)
        {
            await MoverVistoria(DGVSolicitadoDataVistoria, _bsSolicitadoData, _bsAguardandoChegada, StatusVistoria.AguardandoChegadaParaAgendar);
        }

        // O último botão é um pouco diferente, pois ele "finaliza" o processo.
        private async void BtnDeferido_Click(object? sender, EventArgs e)
        {
            if (DGVAguardandoDef.CurrentRow == null || DGVAguardandoDef.CurrentRow.DataBoundItem is not Vistoria vistoriaSelecionada)
            {
                MessageBox.Show("Por favor, selecione um item para finalizar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var resultado = MessageBox.Show($"Tem certeza que deseja marcar a vistoria do LPCO '{vistoriaSelecionada.LPCO}' como DEFERIDA?", "Confirmar Deferimento", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (resultado == DialogResult.No) return;

            try
            {
                // 1. Busca o Processo principal no banco de dados.
                var processo = await _repositorioProcesso.GetByRefUsaAsync(vistoriaSelecionada.Ref_USA);
                if (processo == null)
                {
                    MessageBox.Show("Processo original não encontrado. Não é possível atualizar o status.", "Erro");
                    return;
                }

                // 2. Encontra o LPCO específico dentro da lista de LIs do processo.
                var lpcoParaAtualizar = processo.LI
                    .SelectMany(li => li.LPCO)
                    .FirstOrDefault(lpco => lpco.LPCO == vistoriaSelecionada.LPCO);

                if (lpcoParaAtualizar != null)
                {
                    // 3. Atualiza o status do LPCO para "DEFERIDO".
                    lpcoParaAtualizar.MotivoExigencia = "DEFERIDO";

                    // 4. Salva o Processo inteiro. A lógica de SincronizarLicencas
                    //    irá propagar essa alteração para o OrgaoAnuente correspondente.
                    await _repositorioProcesso.UpdateAsync(processo);
                }

                // 5. Deleta o registro da vistoria, já que ela foi concluída.
                await _repositorioVistorias.DeleteByLpcoAsync(vistoriaSelecionada.LPCO);

                // 6. Remove o item da grade na tela.
                _bsAguardandoDef.Remove(vistoriaSelecionada);

                MessageBox.Show("Vistoria finalizada e status do LPCO atualizado com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao finalizar a vistoria: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
    }
}
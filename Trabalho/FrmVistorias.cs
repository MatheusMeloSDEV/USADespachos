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
        private readonly BindingSource _bsAguardandoLaudo;
        private readonly BindingSource _bsProcessosDadoEntrada;

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
            _bsAguardandoLaudo = new BindingSource();
            _bsProcessosDadoEntrada = new BindingSource();
        }
        private async Task CarregarDadosAsync()
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                await _vistoriaService.SincronizarVistoriasAsync();
                var todasAsVistorias = await _repositorioVistorias.GetAllAsync();

                // MUDANÇA: Os BindingSources agora recebem a lista de 'Vistoria' diretamente.
                _bsAguardandoLaudo.DataSource = todasAsVistorias
                    .Where(v => v.Status == StatusVistoria.AguardandoLaudo)
                    .OrderBy(v => v.Previsao ?? DateTime.MaxValue)
                    .ToList();
                _bsAguardandoDef.DataSource = todasAsVistorias
                    .Where(v => v.Status == StatusVistoria.AguardandoDeferimento)
                    .OrderBy(v => v.Previsao ?? DateTime.MaxValue)
                    .ToList();

                _bsVistoriaAgendada.DataSource = todasAsVistorias
                    .Where(v => v.Status == StatusVistoria.VistoriaAgendada)
                    .OrderBy(v => v.Previsao ?? DateTime.MaxValue)
                    .ToList();

                _bsSolicitadoData.DataSource = todasAsVistorias
                    .Where(v => v.Status == StatusVistoria.SolicitarDataVistoria)
                    .OrderBy(v => v.Previsao ?? DateTime.MaxValue)
                    .ToList();

                _bsAguardandoChegada.DataSource = todasAsVistorias
                    .Where(v => v.Status == StatusVistoria.AguardandoChegadaParaAgendar)
                    .OrderBy(v => v.Previsao ?? DateTime.MaxValue)
                    .ToList();
                _bsProcessosDadoEntrada.DataSource = todasAsVistorias
                    .Where(v => v.Status == StatusVistoria.ProcessoDadoEntrada)
                    .OrderBy(v => v.Previsao ?? DateTime.MaxValue)
                    .ToList();
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
        private async void FrmVistorias_Shown(object? sender, EventArgs e)
        {
            _timer.Interval = 60000;
            _timer.Tick += async (s, e) => await SincronizarPeriodicamente();
            _timer.Start();

            ConfigurarGrids();
            await CarregarDadosAsync();
        }
        private async Task SincronizarPeriodicamente()
        {
            var alteracoes = await _vistoriaService.SincronizarVistoriasAsync();
            if (alteracoes != null && alteracoes.Any())
            {
                BtnRecarrega.Text = $"Atualização pendentes {alteracoes.Count}";
            }
        }
        private async void BtnRecarrega_Click(object sender, EventArgs e)
        {
            BtnRecarrega.Enabled = false;
            BtnRecarrega.Text = "Atualizando...";

            await SincronizarPeriodicamente();
            await CarregarDadosAsync();

            BtnRecarrega.Enabled = true;
            BtnRecarrega.Text = "";
        }
        private void ConfigurarGrids()
        {
            // Vincula cada BindingSource à sua respectiva grade
            DGVAguardandoDef.DataSource = _bsAguardandoDef;
            DGVVistoriaAgendada.DataSource = _bsVistoriaAgendada;
            DGVSolicitadoDataVistoria.DataSource = _bsSolicitadoData;
            DGVAguardandoChegAgendVistoria.DataSource = _bsAguardandoChegada;
            DGVLaudo.DataSource = _bsAguardandoLaudo;
            DGVProcessosDadoEntrada.DataSource = _bsProcessosDadoEntrada;

            // MUDANÇA: Chama a configuração para TODAS as 4 grades
            ConfigurarGrid(DGVAguardandoChegAgendVistoria);
            ConfigurarGrid(DGVSolicitadoDataVistoria);
            ConfigurarGrid(DGVVistoriaAgendada);
            ConfigurarGrid(DGVAguardandoDef);
            ConfigurarGrid(DGVLaudo);
            ConfigurarGrid(DGVProcessosDadoEntrada);
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
            dgv.EditingControlShowing += DGV_EditingControlShowing;
            dgv.CellValueChanged += DGV_CellValueChanged;
            dgv.DataBindingComplete += DGV_DataBindingComplete;
            dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            dgv.Columns.Clear();
            dgv.Columns.AddRange(new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn { DataPropertyName = "LI", HeaderText = "L.I.", ReadOnly = true, AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells, MinimumWidth = 90 },
                new DataGridViewTextBoxColumn { DataPropertyName = "LPCO", HeaderText = "LPCO", ReadOnly = true, AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells, MinimumWidth = 120 },
                new DataGridViewTextBoxColumn { DataPropertyName = "Importador", HeaderText = "Importador", ReadOnly = true, FillWeight = 120, MinimumWidth = 120 },
                new DataGridViewTextBoxColumn { DataPropertyName = "Container", HeaderText = "Container", ReadOnly = true, AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells, MinimumWidth = 110 },
                new DataGridViewTextBoxColumn { DataPropertyName = "Ref_USA", HeaderText = "Ref. USA", ReadOnly = true, AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells, MinimumWidth = 90 },
                new DataGridViewTextBoxColumn { DataPropertyName = "Produto", HeaderText = "Produto", ReadOnly = true, FillWeight = 150, MinimumWidth = 150 },
                new DataGridViewTextBoxColumn { DataPropertyName = "Terminal", HeaderText = "Terminal", ReadOnly = true, AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells, MinimumWidth = 150 },
                new DataGridViewTextBoxColumn { DataPropertyName = "Conhecimento", HeaderText = "Conhecimento", ReadOnly = true, AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells, MinimumWidth = 110 },
                new DataGridViewTextBoxColumn { DataPropertyName = "Previsao", HeaderText = "Previsão", ReadOnly = true, DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" }, AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells, MinimumWidth = 90 },
                new DataGridViewTextBoxColumn { DataPropertyName = "ParametrizacaoLPCO", HeaderText = "Status LPCO", ReadOnly = true, AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells, MinimumWidth = 100 },
                new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "Notas",
                    HeaderText = "Notas",
                    ReadOnly = false,
                    FillWeight = 180,
                    MinimumWidth = 150,
                    DefaultCellStyle = new DataGridViewCellStyle { WrapMode = DataGridViewTriState.True }
                }
            });
        }
        private void DGV_EditingControlShowing(object? sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (sender is not DataGridView dgv || dgv.CurrentCell == null) return;
            if (e.Control is TextBox editingTextBox)
            {
                editingTextBox.Multiline = (dgv.CurrentCell.OwningColumn.DataPropertyName == "Notas");
                editingTextBox.AcceptsReturn = editingTextBox.Multiline;
                editingTextBox.WordWrap = editingTextBox.Multiline;
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

        private void DGV_DataBindingComplete(object? sender, DataGridViewBindingCompleteEventArgs e)
        {
            // Garante que o evento foi disparado por uma grade
            if (sender is not DataGridView dgv) return;

            // Percorre cada linha da grade que acabou de ser preenchida
            foreach (DataGridViewRow row in dgv.Rows)
            {
                // Pega o objeto 'Vistoria' associado à linha
                if (row.DataBoundItem is Vistoria vistoria)
                {
                    // A REGRA: Verifica se a parametrização é "Coleta de Amostra"
                    if (vistoria.ParametrizacaoLPCO?.ToUpper() == "COLETA DE AMOSTRA")
                    {
                        // Se for, pinta o fundo da linha com um amarelo forte
                        row.DefaultCellStyle.BackColor = Color.Gold;
                        row.DefaultCellStyle.ForeColor = Color.Black; // Garante que o texto fique legível
                    }
                    else
                    {
                        // Se não for, garante que a linha tenha a cor padrão
                        row.DefaultCellStyle.BackColor = SystemColors.Window;
                        row.DefaultCellStyle.ForeColor = SystemColors.ControlText;
                    }
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
        private async void BtnSobeLaudo_Click(object sender, EventArgs e)
        {
            await MoverVistoria(DGVAguardandoDef, _bsAguardandoDef, _bsAguardandoLaudo, StatusVistoria.AguardandoLaudo);
        }

        // --- NOVOS MÉTODOS PARA DESCER DE ESTÁGIO ---
        private async void btnDesceParaAgendada_Click(object? sender, EventArgs e)
        {
            await MoverVistoria(DGVAguardandoDef, _bsAguardandoDef, _bsVistoriaAgendada, StatusVistoria.VistoriaAgendada);
        }
        private async void BtnDesceDeferimento_Click(object sender, EventArgs e)
        {
            await MoverVistoria(DGVLaudo, _bsAguardandoLaudo, _bsAguardandoDef, StatusVistoria.AguardandoDeferimento);
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

        private async void BtnDeferido_Click_1(object sender, EventArgs e)
        {
            if (DGVLaudo.CurrentRow == null || DGVLaudo.CurrentRow.DataBoundItem is not Vistoria vistoriaSelecionada)
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
                _bsAguardandoLaudo.Remove(vistoriaSelecionada);

                MessageBox.Show("Vistoria finalizada e status do LPCO atualizado com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao finalizar a vistoria: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void MostrarSomenteLinhaConteudo(int linhaConteudoSelecionada)
        {
            TableVistorias.SuspendLayout();
            int[] linhasConteudo = { 2, 4, 6, 8 };

            foreach (int linha in linhasConteudo)
            {
                if (linha < TableVistorias.RowStyles.Count)
                {
                    if (linha == (linhaConteudoSelecionada))
                    {
                        TableVistorias.RowStyles[linha].SizeType = SizeType.Percent;
                        TableVistorias.RowStyles[linha].Height = 100;
                    }
                    else
                    {
                        TableVistorias.RowStyles[linha].SizeType = SizeType.Absolute;
                        TableVistorias.RowStyles[linha].Height = 0;
                    }
                }
            }
            TableVistorias.ResumeLayout();
        }



        private void LblSolicitadoDataVistoria_Click(object sender, EventArgs e)
        {
            MostrarSomenteLinhaConteudo(8);
        }

        private void LblVistoriaAgendada_Click(object sender, EventArgs e)
        {
            MostrarSomenteLinhaConteudo(6);
        }

        private void LblAguardandoLaudo_Click(object sender, EventArgs e)
        {
            MostrarSomenteLinhaConteudo(2);
        }

        private void LblAguardandoDeferimento_Click(object sender, EventArgs e)
        {
            MostrarSomenteLinhaConteudo(4);
        }
    }
}
using CLUSA;
using MongoDB.Driver;
using System.Data;
using System.Diagnostics;

namespace Trabalho
{
    public partial class FrmVistorias : Form
    {
        private readonly VistoriaService _vistoriaService;
        private readonly RepositorioVistorias _repositorioVistorias;
        private readonly RepositorioProcesso _repositorioProcesso;

        private readonly Logado _logado;
        private readonly RepositorioUsers _repositorioUsers;
        private Users? _usuarioLogado;

        // BindingSources
        private readonly BindingSource _bsAguardandoDef = new();
        private readonly BindingSource _bsVistoriaAgendada = new();
        private readonly BindingSource _bsSolicitadoData = new();
        private readonly BindingSource _bsAguardandoChegada = new();
        private readonly BindingSource _bsAguardandoLaudo = new();
        private readonly BindingSource _bsProcessosDadoEntrada = new();

        private readonly Queue<OperacaoPendente<Vistoria>> _filaVistoriasPendentes = new();
        private void SetDoubleBuffered(Control control)
        {
            // Habilita a propriedade protegida DoubleBuffered via Reflection
            typeof(Control).InvokeMember("DoubleBuffered",
                System.Reflection.BindingFlags.SetProperty |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.NonPublic,
                null, control, new object[] { true });
        }

        public FrmVistorias(Logado logado)
        {
            InitializeComponent();

            this.AutoScroll = true;
            this.AutoScrollMinSize = new Size(1400, 900);

            var client = new MongoClient(ConfigDatabase.MongoConnectionString);
            var database = client.GetDatabase(ConfigDatabase.MongoDatabaseName);

            _vistoriaService = new VistoriaService(database);
            _repositorioVistorias = new RepositorioVistorias(database);
            _repositorioProcesso = new RepositorioProcesso();

            _repositorioUsers = new RepositorioUsers();
            _logado = logado;

            SetDoubleBuffered(DGVAguardandoChegAgendVistoria);
            SetDoubleBuffered(DGVSolicitadoDataVistoria);
            SetDoubleBuffered(DGVVistoriaAgendada);
            SetDoubleBuffered(DGVAguardandoDef);
            SetDoubleBuffered(DGVLaudo);
            SetDoubleBuffered(DGVProcessosDadoEntrada);
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (_filaVistoriasPendentes.Count > 0)
            {
                var result = MessageBox.Show(
                    $"ATENÇÃO: Existem {_filaVistoriasPendentes.Count} alterações pendentes de sincronização!\n\n" +
                    "Se você fechar agora, ESSES DADOS SERÃO PERDIDOS.\n\n" +
                    "Deseja fechar mesmo assim?",
                    "Risco de Perda de Dados",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button2);

                if (result == DialogResult.No)
                {
                    e.Cancel = true; 
                    return;
                }
            }

            // Para o timer
            if (_timer != null)
            {
                _timer.Stop();
                _timer.Dispose();
            }

            base.OnFormClosing(e);
        }
        private async Task CarregarDadosAsync()
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                await _vistoriaService.SincronizarVistoriasAsync();
                var todasAsVistorias = await _repositorioVistorias.GetAllAsync();

                var listasProcessadas = await Task.Run(() =>
                {
                    return new
                    {
                        AguardandoLaudo = FiltrarOrdenar(todasAsVistorias, StatusVistoria.AguardandoLaudo),
                        AguardandoDef = FiltrarOrdenar(todasAsVistorias, StatusVistoria.AguardandoDeferimento),
                        Agendada = FiltrarOrdenar(todasAsVistorias, StatusVistoria.VistoriaAgendada),
                        Solicitado = FiltrarOrdenar(todasAsVistorias, StatusVistoria.SolicitarDataVistoria),
                        AguardandoChegada = FiltrarOrdenar(todasAsVistorias, StatusVistoria.AguardandoChegadaParaAgendar),
                        DadoEntrada = FiltrarOrdenar(todasAsVistorias, StatusVistoria.ProcessoDadoEntrada)
                    };
                });

                _bsAguardandoLaudo.DataSource = listasProcessadas.AguardandoLaudo;
                _bsAguardandoDef.DataSource = listasProcessadas.AguardandoDef;
                _bsVistoriaAgendada.DataSource = listasProcessadas.Agendada;
                _bsSolicitadoData.DataSource = listasProcessadas.Solicitado;
                _bsAguardandoChegada.DataSource = listasProcessadas.AguardandoChegada;
                _bsProcessosDadoEntrada.DataSource = listasProcessadas.DadoEntrada;

                AjustarTodosDataGridViews();
            }
            catch (MongoDB.Driver.MongoConnectionException ex)
            {
                MessageBox.Show(
                    $"Perda de conexão com o MongoDB. Verifique o servidor e a rede.\n\nDetalhes: {ex.Message}",
                    "Erro de conexão",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erro ao carregar vistorias: {ex.Message}",
                    "Erro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private async void FrmVistorias_Shown(object? sender, EventArgs e)
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
            _usuarioLogado.PreferenciasGrids.TryGetValue("DGVVistorias", out var colunasVisiveis);

            // Configura TODAS as grades com o mesmo catálogo/colunas
            GridColumnManager.ConfigurarGrid(DGVAguardandoChegAgendVistoria, "DGVVistorias", colunasVisiveis);
            GridColumnManager.ConfigurarGrid(DGVSolicitadoDataVistoria, "DGVVistorias", colunasVisiveis);
            GridColumnManager.ConfigurarGrid(DGVVistoriaAgendada, "DGVVistorias", colunasVisiveis);
            GridColumnManager.ConfigurarGrid(DGVAguardandoDef, "DGVVistorias", colunasVisiveis);
            GridColumnManager.ConfigurarGrid(DGVLaudo, "DGVVistorias", colunasVisiveis);
            GridColumnManager.ConfigurarGrid(DGVProcessosDadoEntrada, "DGVVistorias", colunasVisiveis);

            ConfigurarGrids();
            await CarregarDadosAsync();

            // Timer de atualização
            _timer.Interval = 60000;
            _timer.Tick += async (s, ev) =>
            {
                await ProcessarFilaVistoriasAsync();
                await SincronizarPeriodicamente();
            };
            _timer.Start();
        }


        #region "Wrappers com Sistema de Fila"

        private async Task UpsertVistoriaComFilaAsync(Vistoria vistoria)
        {
            try
            {
                await _repositorioVistorias.UpsertAsync(vistoria);
            }
            catch (MongoDB.Driver.MongoConnectionException ex)
            {
                _filaVistoriasPendentes.Enqueue(new OperacaoPendente<Vistoria>
                {
                    Tipo = TipoOperacaoGenerica.Update,
                    Entidade = vistoria
                });

                MessageBox.Show(
                    $"Sem conexão com o banco de dados. A alteração foi colocada em fila para reenvio.\n\nOperações pendentes: {_filaVistoriasPendentes.Count}",
                    "Aviso - Modo Offline",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        private async Task DeleteVistoriaComFilaAsync(string lpco)
        {
            try
            {
                await _repositorioVistorias.DeleteByLpcoAsync(lpco);
            }
            catch (MongoDB.Driver.MongoConnectionException ex)
            {
                _filaVistoriasPendentes.Enqueue(new OperacaoPendente<Vistoria>
                {
                    Tipo = TipoOperacaoGenerica.Delete,
                    Chave = lpco
                });

                MessageBox.Show(
                    $"Sem conexão com o banco de dados. A exclusão foi colocada em fila para reenvio.\n\nOperações pendentes: {_filaVistoriasPendentes.Count}",
                    "Aviso - Modo Offline",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        private async Task ProcessarFilaVistoriasAsync()
        {
            if (_filaVistoriasPendentes.Count == 0) return;

            int processadas = 0;
            int errosDados = 0;

            while (_filaVistoriasPendentes.Count > 0)
            {
                var op = _filaVistoriasPendentes.Peek();

                try
                {
                    switch (op.Tipo)
                    {
                        case TipoOperacaoGenerica.Insert:
                        case TipoOperacaoGenerica.Update:
                            if (op.Entidade != null)
                                await _repositorioVistorias.UpsertAsync(op.Entidade);
                            break;

                        case TipoOperacaoGenerica.Delete:
                            if (op.Chave is string lpco)
                                await _repositorioVistorias.DeleteByLpcoAsync(lpco);
                            break;
                    }

                    _filaVistoriasPendentes.Dequeue();
                    processadas++;
                }
                catch (MongoDB.Driver.MongoConnectionException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    var itemComErro = _filaVistoriasPendentes.Dequeue();
                    errosDados++;
                    Debug.WriteLine($"Erro fatal ao processar item pendente (descartado): {ex.Message}");
                }
            }

            if (processadas > 0)
            {
                BtnRecarrega.Text = "Sincronização concluída!";
                await Task.Delay(3000);
                BtnRecarrega.Text = "";
            }

            if (errosDados > 0)
            {
                MessageBox.Show($"{errosDados} operações falharam por dados inválidos e foram descartadas.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        #endregion

        private void AjustarAlturaDataGridView(DataGridView dgv)
        {
            // Se não tiver linhas, altura = 0
            if (dgv.Rows.Count == 0)
            {
                dgv.Height = 0;
                dgv.Visible = false; // Opcional: esconder completamente quando vazio
                return;
            }

            // Se tiver linhas, calcular altura necessária
            dgv.Visible = true; // Opcional: mostrar quando tiver dados
            int alturaTotal = dgv.ColumnHeadersHeight;

            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (row.Visible)
                    alturaTotal += row.Height;
            }

            dgv.Height = alturaTotal;
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
            dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            dgv.EditingControlShowing += DGV_EditingControlShowing;
            dgv.CellValueChanged += DGV_CellValueChanged;
            dgv.DataBindingComplete += DGV_DataBindingComplete;

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
            if (sender is not DataGridView dgv || e.RowIndex < 0) return;

            string nomeColunaEditada = dgv.Columns[e.ColumnIndex].DataPropertyName;

            if (nomeColunaEditada != "Notas") return;

            if (dgv.Rows[e.RowIndex].DataBoundItem is Vistoria vistoriaEditada)
            {
                await UpsertVistoriaComFilaAsync(vistoriaEditada);
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
        private List<Vistoria> FiltrarOrdenar(List<Vistoria> lista, StatusVistoria status)
        {
            return lista
                .Where(v => v.Status == status)
                .OrderBy(v => v.Previsao ?? DateTime.MaxValue)
                .ToList();
        }
        #region "Lógica de Movimentação de Vistorias"

        /// <summary>
        /// Move a vistoria selecionada de uma grade de origem para uma de destino.
        /// </summary>
        /// <param name="dgvOrigem">A DataGridView de onde o item sairá.</param>
        /// <param name="bsOrigem">O BindingSource da grade de origem.</param>
        /// <param name="bsDestino">O BindingSource da grade de destino.</param>
        /// <param name="novoStatus">O novo status a ser atribuído à vistoria.</param>
        private async Task MoverVistoria(DataGridView dgvOrigem, DataGridView dgvDestino, BindingSource bsOrigem, BindingSource bsDestino, StatusVistoria novoStatus)
        {
            if (dgvOrigem.CurrentRow == null || dgvOrigem.CurrentRow.DataBoundItem is not Vistoria vistoriaSelecionada)
            {
                MessageBox.Show("Por favor, selecione um item para mover.", "Aviso");
                return;
            }

            vistoriaSelecionada.Status = novoStatus;
            await UpsertVistoriaComFilaAsync(vistoriaSelecionada);

            bsOrigem.Remove(vistoriaSelecionada);
            bsDestino.Add(vistoriaSelecionada);

            AjustarAlturaDataGridView(dgvOrigem);
            AjustarAlturaDataGridView(dgvDestino);
        }

        private void AjustarTodosDataGridViews()
        {
            AjustarAlturaDataGridView(DGVAguardandoChegAgendVistoria);
            AjustarAlturaDataGridView(DGVAguardandoDef);
            AjustarAlturaDataGridView(DGVLaudo);
            AjustarAlturaDataGridView(DGVProcessosDadoEntrada);
            AjustarAlturaDataGridView(DGVSolicitadoDataVistoria);
            AjustarAlturaDataGridView(DGVVistoriaAgendada);
        }

        // --- MÉTODOS PARA SUBIR DE ESTÁGIO ---
        private async void BtnSobeSolicitado_Click(object? sender, EventArgs e)
        {
            await MoverVistoria(DGVAguardandoChegAgendVistoria, DGVSolicitadoDataVistoria, _bsAguardandoChegada, _bsSolicitadoData, StatusVistoria.SolicitarDataVistoria);
        }

        private async void BtnSobeAgendada_Click(object? sender, EventArgs e)
        {
            await MoverVistoria(DGVSolicitadoDataVistoria, DGVVistoriaAgendada, _bsSolicitadoData, _bsVistoriaAgendada, StatusVistoria.VistoriaAgendada);
        }

        private async void BtnSobeAguardDef_Click(object? sender, EventArgs e)
        {
            await MoverVistoria(DGVVistoriaAgendada, DGVAguardandoDef, _bsVistoriaAgendada, _bsAguardandoDef, StatusVistoria.AguardandoDeferimento);
        }
        private async void BtnSobeLaudo_Click(object sender, EventArgs e)
        {
            await MoverVistoria(DGVAguardandoDef, DGVLaudo, _bsAguardandoDef, _bsAguardandoLaudo, StatusVistoria.AguardandoLaudo);
        }

        // --- NOVOS MÉTODOS PARA DESCER DE ESTÁGIO ---
        private async void btnDesceParaAgendada_Click(object? sender, EventArgs e)
        {
            await MoverVistoria(DGVAguardandoDef, DGVSolicitadoDataVistoria, _bsAguardandoDef, _bsVistoriaAgendada, StatusVistoria.VistoriaAgendada);
        }
        private async void BtnDesceDeferimento_Click(object sender, EventArgs e)
        {
            await MoverVistoria(DGVLaudo, DGVVistoriaAgendada, _bsAguardandoLaudo, _bsAguardandoDef, StatusVistoria.AguardandoDeferimento);
        }

        private async void btnDesceParaSolicitado_Click(object? sender, EventArgs e)
        {
            await MoverVistoria(DGVVistoriaAgendada, DGVSolicitadoDataVistoria, _bsVistoriaAgendada, _bsSolicitadoData, StatusVistoria.SolicitarDataVistoria);
        }

        private async void btnDesceParaAguardando_Click(object? sender, EventArgs e)
        {
            await MoverVistoria(DGVSolicitadoDataVistoria, DGVAguardandoChegAgendVistoria, _bsSolicitadoData, _bsAguardandoChegada, StatusVistoria.AguardandoChegadaParaAgendar);
        }

        // O último botão é um pouco diferente, pois ele "finaliza" o processo.

        private async Task FinalizarVistoriaAsync(Vistoria vistoria, string novoStatusMotivoExigencia, BindingSource bindingSource)
        {
            if (vistoria == null) return;

            var acao = novoStatusMotivoExigencia == "DEFERIDO" ? "DEFERIR" : "CANCELAR";
            var resultado = MessageBox.Show(
                $"Tem certeza que deseja marcar a vistoria do LPCO '{vistoria.LPCO}' como {novoStatusMotivoExigencia}?",
                $"Confirmar {acao}", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (resultado == DialogResult.No) return;

            try
            {
                Cursor = Cursors.WaitCursor;

                // 1. Atualiza o Processo Principal (Marca LPCO como DEFERIDO/CANCELADA)
                // Isso impede que a vistoria seja recriada no futuro
                await _repositorioProcesso.AtualizarStatusLpcoAsync(vistoria.Ref_USA, vistoria.LPCO, novoStatusMotivoExigencia);

                // 2. Remove a Vistoria da coleção de Vistorias
                await DeleteVistoriaComFilaAsync(vistoria.LPCO);

                // 3. Atualiza a UI (Remove da tela)
                bindingSource.Remove(vistoria);

                MessageBox.Show($"Vistoria finalizada e LPCO atualizado como {novoStatusMotivoExigencia}!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao processar a solicitação: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }
        private async void BtnDeferido_Click(object? sender, EventArgs e)
        {
            if (DGVAguardandoDef.CurrentRow?.DataBoundItem is Vistoria vistoria)
            {
                await FinalizarVistoriaAsync(vistoria, "DEFERIDO", _bsAguardandoDef);
            }
            else
            {
                MessageBox.Show("Selecione um item para finalizar.", "Aviso");
            }
        }

        private async void BtnDeferido_Click_1(object sender, EventArgs e)
        {
            // Este botão parece estar no DGVLaudo (baseado no seu código anterior)
            if (DGVLaudo.CurrentRow?.DataBoundItem is Vistoria vistoria)
            {
                await FinalizarVistoriaAsync(vistoria, "DEFERIDO", _bsAguardandoLaudo);
            }
            else
            {
                MessageBox.Show("Selecione um item para finalizar.", "Aviso");
            }
        }

        private async void BtnCancelada_Click(object sender, EventArgs e)
        {
            if (DGVProcessosDadoEntrada.CurrentRow?.DataBoundItem is Vistoria vistoria)
            {
                await FinalizarVistoriaAsync(vistoria, "CANCELADA", _bsProcessosDadoEntrada);
            }
            else
            {
                MessageBox.Show("Selecione um item para cancelar.", "Aviso");
            }
        }
        #endregion
    }
    #region "Operações Pendentes - Sistema de Fila"
    public enum TipoOperacaoGenerica
    {
        Insert,
        Update,
        Delete
    }

    public class OperacaoPendente<T>
    {
        public TipoOperacaoGenerica Tipo { get; set; }
        public T? Entidade { get; set; }
        public object? Chave { get; set; }
    }
    #endregion
}
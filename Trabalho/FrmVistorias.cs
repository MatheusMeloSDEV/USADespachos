using CLUSA.Repositories;
using CLUSA.Services;
using CLUSA.Models;
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
        private readonly RepositorioLog _logRepo;
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
            _logRepo = new RepositorioLog();

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
                await _logRepo.RegistrarLogAsync(
                    "Sincronização", _logado.Usuario,
                    "Fila offline de vistorias processada com sucesso",
                    $"Itens sincronizados: {processadas} | Erros descartados: {errosDados}"
                );

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
                dgv.Visible = false;
                return;
            }

            // Se tiver linhas, calcular altura necessária
            dgv.Visible = true;
            int alturaTotal = dgv.ColumnHeadersHeight;

            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (row.Visible)
                    alturaTotal += row.Height;
            }

            // --- NOVO: Aumenta um pouco mais se for o grid específico ---
            if (dgv == DGVProcessosDadoEntrada)
            {
                // Adiciona 30 pixels extras (ajuste esse valor conforme seu gosto)
                // Isso ajuda caso apareça uma barra de rolagem horizontal ou apenas para dar destaque
                alturaTotal += 10;
            }
            // -----------------------------------------------------------

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
            var query = lista.Where(v => v.Status == status);

            if (status == StatusVistoria.ProcessoDadoEntrada)
            {
                // Ordena por Data de Registro se for Processo Dado Entrada
                return query
                    .OrderBy(v => v.DataRegistroLPCO ?? DateTime.MaxValue)
                    .ToList();
            }
            else
            {
                // Ordena por Previsão para todos os outros status
                return query
                    .OrderBy(v => v.Previsao ?? DateTime.MaxValue)
                    .ToList();
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
        private async Task MoverVistoria(DataGridView dgvOrigem, DataGridView dgvDestino, BindingSource bsOrigem, BindingSource bsDestino, StatusVistoria novoStatus)
        {
            if (dgvOrigem.CurrentRow == null || dgvOrigem.CurrentRow.DataBoundItem is not Vistoria vistoriaSelecionada)
            {
                MessageBox.Show("Por favor, selecione um item para mover.", "Aviso");
                return;
            }

            // Guarda o status antigo para o log antes de mudar
            var statusAntigo = vistoriaSelecionada.Status;

            vistoriaSelecionada.Status = novoStatus;
            await UpsertVistoriaComFilaAsync(vistoriaSelecionada);

            bsOrigem.Remove(vistoriaSelecionada);
            bsDestino.Add(vistoriaSelecionada);

            AjustarAlturaDataGridView(dgvOrigem);
            AjustarAlturaDataGridView(dgvDestino);

            // --- 4. LOG DE MOVIMENTAÇÃO ---
            // Fire-and-forget seguro (não trava a UI)
            _ = _logRepo.RegistrarLogAsync(
                "Movimentação Vistoria", _logado.Usuario,
                $"LPCO {vistoriaSelecionada.LPCO} movido para {novoStatus}",
                $"Processo: {vistoriaSelecionada.Ref_USA} | De: {statusAntigo} -> Para: {novoStatus} | Usuário: {_logado.Usuario}"
            );
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
            // 1. Verifica se tem um item selecionado ANTES de pedir a data
            if (DGVSolicitadoDataVistoria.CurrentRow?.DataBoundItem is not Vistoria vistoriaSelecionada)
            {
                MessageBox.Show("Por favor, selecione um item para agendar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Chama a nossa janelinha de calendário
            DateTime? dataEscolhida = SolicitarDataVistoria();

            // Se ele cancelou ou fechou a janela, a gente para o processo aqui mesmo
            if (dataEscolhida == null) return;

            try
            {
                Cursor = Cursors.WaitCursor;

                // 3. Busca o processo no banco para atualizar o Histórico
                var processo = await _repositorioProcesso.GetByRefUsaAsync(vistoriaSelecionada.Ref_USA);

                if (processo != null)
                {
                    // Monta a frase exatamente como você pediu
                    string novaLinha = $"{DateTime.Now:dd/MM/yyyy} Vistoria agendada para: {dataEscolhida.Value:dd/MM/yyyy}";
                    string historicoAntigo = processo.HistoricoDoProcesso ?? "";

                    string novoHistorico = $"{novaLinha}\r\n{historicoAntigo}".Trim();

                    // Usa o UpdateParcial para alterar SOMENTE o histórico e não mexer no resto do processo
                    var atualizacoes = new List<UpdateDefinition<Processo>>
            {
                Builders<Processo>.Update.Set(p => p.HistoricoDoProcesso, novoHistorico)
            };

                    await _repositorioProcesso.UpdateParcialAsync(processo.Id, atualizacoes);
                }

                // 4. Se deu tudo certo no banco, move a vistoria para a próxima grid (Seu código original)
                await MoverVistoria(DGVSolicitadoDataVistoria, DGVVistoriaAgendada, _bsSolicitadoData, _bsVistoriaAgendada, StatusVistoria.VistoriaAgendada);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao agendar e salvar o histórico: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private async void BtnSobeAguardDef_Click(object? sender, EventArgs e)
        {
            
            // 1. Verifica se tem um item selecionado ANTES de pedir a data
            if (DGVVistoriaAgendada.CurrentRow?.DataBoundItem is not Vistoria vistoriaSelecionada)
            {
                MessageBox.Show("Por favor, selecione um item.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Chama a nossa janelinha de calendário
            DateTime? dataEscolhida = SolicitarDataVistoria();

            // Se ele cancelou ou fechou a janela, a gente para o processo aqui mesmo
            if (dataEscolhida == null) return;

            try
            {
                Cursor = Cursors.WaitCursor;

                // 3. Busca o processo no banco para atualizar o Histórico
                var processo = await _repositorioProcesso.GetByRefUsaAsync(vistoriaSelecionada.Ref_USA);

                if (processo != null)
                {
                    // Monta a frase exatamente como você pediu
                    string novaLinha = $"{DateTime.Now:dd/MM/yyyy} Vistoria realizada em: {dataEscolhida.Value:dd/MM/yyyy}";
                    string historicoAntigo = processo.HistoricoDoProcesso ?? "";

                    string novoHistorico = $"{novaLinha}\r\n{historicoAntigo}".Trim();

                    var atualizacoes = new List<UpdateDefinition<Processo>>
            {
                Builders<Processo>.Update.Set(p => p.HistoricoDoProcesso, novoHistorico)
            };

                    await _repositorioProcesso.UpdateParcialAsync(processo.Id, atualizacoes);
                }

                await MoverVistoria(DGVVistoriaAgendada, DGVAguardandoDef, _bsVistoriaAgendada, _bsAguardandoDef, StatusVistoria.AguardandoDeferimento);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao agendar e salvar o histórico: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
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

        private async Task FinalizarVistoriaAsync(Vistoria vistoria, string novoStatusMotivoExigencia, BindingSource bindingSource, bool adicionarNoHistorico = false, string forcarParametrizacao = null)
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

                string textoHistorico = null;
                if (adicionarNoHistorico)
                {
                    textoHistorico = $"{DateTime.Now:dd/MM/yyyy} LPCO {vistoria.LPCO} foi deferido.";
                }

                // Envia todos os comandos para o repositório
                await _repositorioProcesso.AtualizarStatusLpcoAsync(vistoria.Ref_USA, vistoria.LPCO, novoStatusMotivoExigencia, textoHistorico, forcarParametrizacao);

                await DeleteVistoriaComFilaAsync(vistoria.LPCO);
                bindingSource.Remove(vistoria);

                await _logRepo.RegistrarLogAsync("Finalização Vistoria", _logado.Usuario, $"Vistoria {acao} para LPCO {vistoria.LPCO}", $"Processo: {vistoria.Ref_USA}");

                MessageBox.Show($"Vistoria finalizada e LPCO atualizado como {novoStatusMotivoExigencia}!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                await _logRepo.RegistrarLogAsync("Erro Vistoria", _logado.Usuario, $"Falha ao finalizar {vistoria.LPCO}", ex.Message);
                MessageBox.Show($"Erro ao processar a solicitação: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
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

        // 1. Botão da Grade de Aguardando Deferimento
        private async void BtnDeferido_Click(object? sender, EventArgs e)
        {
            if (DGVAguardandoDef.CurrentRow?.DataBoundItem is Vistoria vistoria)
            {
                // 'true' para histórico, 'null' para não alterar a parametrização
                await FinalizarVistoriaAsync(vistoria, "DEFERIDO", _bsAguardandoDef, true, null);
            }
            else
            {
                MessageBox.Show("Selecione um item para finalizar.", "Aviso");
            }
        }

        // 2. Botão da Grade de Laudo
        private async void BtnDeferido_Click_1(object sender, EventArgs e)
        {
            if (DGVLaudo.CurrentRow?.DataBoundItem is Vistoria vistoria)
            {
                // 'true' para histórico, 'null' para não alterar a parametrização
                await FinalizarVistoriaAsync(vistoria, "DEFERIDO", _bsAguardandoLaudo, true, null);
            }
            else
            {
                MessageBox.Show("Selecione um item para finalizar.", "Aviso");
            }
        }

        // 3. O Botão da Grade de Processos Dado Entrada (Apenas ele força o "Documental")
        private async void BtnDeferirProcessoDEntrada_Click(object sender, EventArgs e)
        {
            if (DGVProcessosDadoEntrada.CurrentRow?.DataBoundItem is Vistoria vistoria)
            {
                // 'true' para histórico, '"Documental"' para forçar a parametrização
                await FinalizarVistoriaAsync(vistoria, "DEFERIDO", _bsProcessosDadoEntrada, true, "Documental");
            }
            else
            {
                MessageBox.Show("Selecione um item para finalizar.", "Aviso");
            }
        }
        private DateTime? SolicitarDataVistoria()
        {
            // Cria uma janelinha de diálogo dinâmica na hora
            using var form = new Form
            {
                Text = "Agendar Vistoria",
                Size = new Size(320, 160),
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                MaximizeBox = false,
                MinimizeBox = false
            };

            var lbl = new Label { Text = "Selecione a data:", Left = 20, Top = 15, Width = 260 };
            var dtp = new DateTimePicker { Left = 20, Top = 40, Width = 260, Format = DateTimePickerFormat.Short };
            var btnOk = new Button { Text = "Confirmar", Left = 110, Top = 75, Width = 80, DialogResult = DialogResult.OK };
            var btnCancel = new Button { Text = "Cancelar", Left = 200, Top = 75, Width = 80, DialogResult = DialogResult.Cancel };

            form.Controls.Add(lbl);
            form.Controls.Add(dtp);
            form.Controls.Add(btnOk);
            form.Controls.Add(btnCancel);

            form.AcceptButton = btnOk;   // Permite dar 'Enter'
            form.CancelButton = btnCancel; // Permite dar 'Esc'

            // Mostra a tela e retorna a data se ele clicar em Confirmar
            if (form.ShowDialog(this) == DialogResult.OK)
            {
                return dtp.Value;
            }
            return null; // Retorna nulo se ele cancelar
        }
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
}
    #endregion
//0}
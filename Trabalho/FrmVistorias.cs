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
        private readonly RepositorioVistoriaDUIMP _repositorioVistoriaDUIMP;
        private Users? _usuarioLogado;

        // BindingSources (LI)
        private readonly BindingSource _bsAguardandoDef = new();
        private readonly BindingSource _bsVistoriaAgendada = new();
        private readonly BindingSource _bsSolicitadoData = new();
        private readonly BindingSource _bsAguardandoChegada = new();
        private readonly BindingSource _bsAguardandoLaudo = new();
        private readonly BindingSource _bsProcessosDadoEntrada = new();

        // BindingSources (DUIMP) - duplicados para o novo tlDUIMP
        private readonly BindingSource _bsDAguardandoDef = new();
        private readonly BindingSource _bsDVistoriaAgendada = new();
        private readonly BindingSource _bsDSolicitadoData = new();
        private readonly BindingSource _bsDAguardandoChegada = new();
        private readonly BindingSource _bsDAguardandoLaudo = new();
        private readonly BindingSource _bsDProcessosDadoEntrada = new();

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

        private List<CLUSA.Models.VistoriaDUIMP> FiltrarOrdenarDUIMP(List<CLUSA.Models.VistoriaDUIMP> lista, StatusVistoria status)
        {
            if (lista == null) return new List<CLUSA.Models.VistoriaDUIMP>();

            // Normaliza strings removendo caracteres não alfanuméricos e maiúsculas para comparação flexível
            static string Normalize(string? s)
            {
                if (string.IsNullOrWhiteSpace(s)) return string.Empty;
                var sb = new System.Text.StringBuilder();
                foreach (var ch in s)
                {
                    if (char.IsLetterOrDigit(ch)) sb.Append(char.ToUpperInvariant(ch));
                }
                return sb.ToString();
            }

            var nomeStatus = status.ToString();
            var nomeStatusNorm = Normalize(nomeStatus);

            var query = lista.Where(v =>
            {
                var s = v.Status ?? string.Empty;
                var sNorm = Normalize(s);

                // Considera correspondência se a string normalizada do processo contém ou é contida no nome do status
                return (!string.IsNullOrEmpty(sNorm) && (sNorm.Contains(nomeStatusNorm) || nomeStatusNorm.Contains(sNorm)));
            });

            if (status == StatusVistoria.ProcessoDadoEntrada)
            {
                return query.OrderBy(v => v.DataRegistro ?? DateTime.MaxValue).ToList();
            }
            else
            {
                return query.OrderBy(v => v.DataDeAtracacao ?? DateTime.MaxValue).ToList();
            }
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
            _repositorioVistoriaDUIMP = new RepositorioVistoriaDUIMP(database);
            _repositorioProcesso = new RepositorioProcesso();
            _logRepo = new RepositorioLog();

            _repositorioUsers = new RepositorioUsers();
            _logado = logado;

            // LI grids
            SetDoubleBuffered(dgvAguardandoChegAgendVistoria);
            SetDoubleBuffered(dgvSolicitadoDataVistoria);
            SetDoubleBuffered(dgvVistoriaAgendada);
            SetDoubleBuffered(dgvAguardandoDef);
            SetDoubleBuffered(dgvLaudo);
            SetDoubleBuffered(dgvProcessosDadoEntrada);

            // DUIMP grids (novo tlDUIMP)
            SetDoubleBuffered(dgvDUIMPAguardandoRIF);
            SetDoubleBuffered(dgvDSolicitadoDataVistoria);
            SetDoubleBuffered(dgvDVistoriaAgendada);
            SetDoubleBuffered(dgvDAguardandoDef);
            SetDoubleBuffered(dgvDLaudo);
            SetDoubleBuffered(dgvDUIMPProcessosRegistrados);
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

                // LI binding sources
                _bsAguardandoLaudo.DataSource = listasProcessadas.AguardandoLaudo;
                _bsAguardandoDef.DataSource = listasProcessadas.AguardandoDef;
                _bsVistoriaAgendada.DataSource = listasProcessadas.Agendada;
                _bsSolicitadoData.DataSource = listasProcessadas.Solicitado;
                _bsAguardandoChegada.DataSource = listasProcessadas.AguardandoChegada;
                _bsProcessosDadoEntrada.DataSource = listasProcessadas.DadoEntrada;

                // DUIMP binding sources: carregar da coleção separada VistoriasDUIMP
                var todasAsVistoriasDUIMP = await _repositorioVistoriaDUIMP.GetAllAsync();

                // Agora aplicamos filtragem por status para popular cada BindingSource DUIMP
                var listaAguardandoLaudo = FiltrarOrdenarDUIMP(todasAsVistoriasDUIMP, StatusVistoria.AguardandoLaudo);
                var listaAguardandoDef = FiltrarOrdenarDUIMP(todasAsVistoriasDUIMP, StatusVistoria.AguardandoDeferimento);
                var listaAgendada = FiltrarOrdenarDUIMP(todasAsVistoriasDUIMP, StatusVistoria.VistoriaAgendada);
                var listaSolicitado = FiltrarOrdenarDUIMP(todasAsVistoriasDUIMP, StatusVistoria.SolicitarDataVistoria);
                var listaAguardandoChegada = FiltrarOrdenarDUIMP(todasAsVistoriasDUIMP, StatusVistoria.AguardandoChegadaParaAgendar);
                var listaDadoEntrada = FiltrarOrdenarDUIMP(todasAsVistoriasDUIMP, StatusVistoria.ProcessoDadoEntrada);

                _bsDAguardandoLaudo.DataSource = listaAguardandoLaudo;
                _bsDAguardandoDef.DataSource = listaAguardandoDef;
                _bsDVistoriaAgendada.DataSource = listaAgendada;
                _bsDSolicitadoData.DataSource = listaSolicitado;
                _bsDAguardandoChegada.DataSource = listaAguardandoChegada;
                _bsDProcessosDadoEntrada.DataSource = listaDadoEntrada;

                // Diagnóstico rápido: se há registros no banco mas nenhum foi mapeado para os status,
                // mostra um alerta com exemplos para ajudar a identificar problema de mapeamento de Status.
                try
                {
                    int total = todasAsVistoriasDUIMP?.Count ?? 0;
                    int totalFiltrados = (listaAguardandoLaudo?.Count ?? 0) + (listaAguardandoDef?.Count ?? 0) + (listaAgendada?.Count ?? 0)
                                         + (listaSolicitado?.Count ?? 0) + (listaAguardandoChegada?.Count ?? 0) + (listaDadoEntrada?.Count ?? 0);

                    if (total > 0 && totalFiltrados == 0)
                    {
                        var exemplos = string.Join("\n", todasAsVistoriasDUIMP.Take(5).Select(v => $"{v.DUIMP} - Status: '{v.Status}'"));
                        MessageBox.Show($"Foram encontrados {total} registros em VistoriasDUIMP, mas nenhum corresponde aos StatusVistoria esperados.\n\nExemplos:\n{exemplos}",
                            "Diagnóstico VistoriasDUIMP",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch { }

                AjustarTodosDataGridViews();
                // Assegura que os cabeçalhos das grades DUIMP estejam sincronizados com as LI
                IgualarCabecalhosEntreGridsLIeDUIMP();
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
            _usuarioLogado.PreferenciasGrids.TryGetValue("DGVVistoriasDUIMP", out var colunasVisiveisDUIMP);

            // Configura TODAS as grades com o mesmo catálogo/colunas (LI)
            GridColumnManager.ConfigurarGrid(dgvAguardandoChegAgendVistoria, "DGVVistorias", colunasVisiveis);
            GridColumnManager.ConfigurarGrid(dgvSolicitadoDataVistoria, "DGVVistorias", colunasVisiveis);
            GridColumnManager.ConfigurarGrid(dgvVistoriaAgendada, "DGVVistorias", colunasVisiveis);
            GridColumnManager.ConfigurarGrid(dgvAguardandoDef, "DGVVistorias", colunasVisiveis);
            GridColumnManager.ConfigurarGrid(dgvLaudo, "DGVVistorias", colunasVisiveis);
            GridColumnManager.ConfigurarGrid(dgvProcessosDadoEntrada, "DGVVistorias", colunasVisiveis);

            // Configura TODAS as grades do TL DUIMP com catálogo específico para VistoriaDUIMP
            GridColumnManager.ConfigurarGrid(dgvDUIMPAguardandoRIF, "DGVVistoriasDUIMP", colunasVisiveisDUIMP ?? colunasVisiveis);
            GridColumnManager.ConfigurarGrid(dgvDSolicitadoDataVistoria, "DGVVistoriasDUIMP", colunasVisiveisDUIMP ?? colunasVisiveis);
            GridColumnManager.ConfigurarGrid(dgvDVistoriaAgendada, "DGVVistoriasDUIMP", colunasVisiveisDUIMP ?? colunasVisiveis);
            GridColumnManager.ConfigurarGrid(dgvDAguardandoDef, "DGVVistoriasDUIMP", colunasVisiveisDUIMP ?? colunasVisiveis);
            GridColumnManager.ConfigurarGrid(dgvDLaudo, "DGVVistoriasDUIMP", colunasVisiveisDUIMP ?? colunasVisiveis);
            GridColumnManager.ConfigurarGrid(dgvDUIMPProcessosRegistrados, "DGVVistoriasDUIMP", colunasVisiveisDUIMP ?? colunasVisiveis);

            ConfigurarGrids();
            await CarregarDadosAsync();

            // Timer de atualização
            _timer.Interval = 60000;
            _timer.Tick += async (s, ev) =>
            {
                await ProcessarFilaVistoriasAsync();
                await ProcessarFilaVistoriasDUIMPAsync();
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

        // Wrappers e fila para VistoriaDUIMP
        private readonly Queue<OperacaoPendente<CLUSA.Models.VistoriaDUIMP>> _filaVistoriasDUIMPPendentes = new();

        private async Task UpsertVistoriaDUIMPComFilaAsync(CLUSA.Models.VistoriaDUIMP item)
        {
            try
            {
                await _repositorioVistoriaDUIMP.UpsertAsync(item);
            }
            catch (MongoDB.Driver.MongoConnectionException)
            {
                _filaVistoriasDUIMPPendentes.Enqueue(new OperacaoPendente<CLUSA.Models.VistoriaDUIMP>
                {
                    Tipo = TipoOperacaoGenerica.Update,
                    Entidade = item
                });

                MessageBox.Show(
                    $"Sem conexão com o banco de dados. A alteração na DUIMP foi colocada em fila para reenvio.\n\nOperações pendentes: {_filaVistoriasDUIMPPendentes.Count}",
                    "Aviso - Modo Offline",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        private async Task DeleteVistoriaDUIMPComFilaAsync(string duimp)
        {
            try
            {
                await _repositorioVistoriaDUIMP.DeleteByDUIMPAsync(duimp);
            }
            catch (MongoDB.Driver.MongoConnectionException)
            {
                _filaVistoriasDUIMPPendentes.Enqueue(new OperacaoPendente<CLUSA.Models.VistoriaDUIMP>
                {
                    Tipo = TipoOperacaoGenerica.Delete,
                    Chave = duimp
                });

                MessageBox.Show(
                    $"Sem conexão com o banco de dados. A exclusão da DUIMP foi colocada em fila para reenvio.\n\nOperações pendentes: {_filaVistoriasDUIMPPendentes.Count}",
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

        private async Task ProcessarFilaVistoriasDUIMPAsync()
        {
            if (_filaVistoriasDUIMPPendentes.Count == 0) return;

            int processadas = 0;
            int errosDados = 0;

            while (_filaVistoriasDUIMPPendentes.Count > 0)
            {
                var op = _filaVistoriasDUIMPPendentes.Peek();

                try
                {
                    switch (op.Tipo)
                    {
                        case TipoOperacaoGenerica.Insert:
                        case TipoOperacaoGenerica.Update:
                            if (op.Entidade != null)
                                await _repositorioVistoriaDUIMP.UpsertAsync(op.Entidade);
                            break;

                        case TipoOperacaoGenerica.Delete:
                            if (op.Chave is string duimp)
                                await _repositorioVistoriaDUIMP.DeleteByDUIMPAsync(duimp);
                            break;
                    }

                    _filaVistoriasDUIMPPendentes.Dequeue();
                    processadas++;
                }
                catch (MongoDB.Driver.MongoConnectionException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    var itemComErro = _filaVistoriasDUIMPPendentes.Dequeue();
                    errosDados++;
                    Debug.WriteLine($"Erro fatal ao processar item pendente DUIMP (descartado): {ex.Message}");
                }
            }

            if (processadas > 0)
            {
                await _logRepo.RegistrarLogAsync(
                    "SincronizaçãoDUIMP", _logado.Usuario,
                    "Fila offline de vistorias DUIMP processada com sucesso",
                    $"Itens sincronizados: {processadas} | Erros descartados: {errosDados}"
                );

                BtnRecarrega.Text = "Sincronização DUIMP concluída!";
                await Task.Delay(3000);
                BtnRecarrega.Text = "";
            }

            if (errosDados > 0)
            {
                MessageBox.Show($"{errosDados} operações DUIMP falharam por dados inválidos e foram descartadas.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

            // --- NOVO: Aumenta um pouco mais se for o grid específico --->
            if (dgv == dgvProcessosDadoEntrada || dgv == dgvDUIMPProcessosRegistrados)
            {
                // Adiciona 10 pixels extras (ajuste esse valor conforme seu gosto)
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

            await ProcessarFilaVistoriasAsync();
            await ProcessarFilaVistoriasDUIMPAsync();
            await SincronizarPeriodicamente();
            await CarregarDadosAsync();

            BtnRecarrega.Enabled = true;
            BtnRecarrega.Text = "";
        }
        private void ConfigurarGrids()
        {
            // Vincula cada BindingSource à sua respectiva grade (LI)
            dgvAguardandoDef.DataSource = _bsAguardandoDef;
            dgvVistoriaAgendada.DataSource = _bsVistoriaAgendada;
            dgvSolicitadoDataVistoria.DataSource = _bsSolicitadoData;
            dgvAguardandoChegAgendVistoria.DataSource = _bsAguardandoChegada;
            dgvLaudo.DataSource = _bsAguardandoLaudo;
            dgvProcessosDadoEntrada.DataSource = _bsProcessosDadoEntrada;

            // Vincula cada BindingSource à sua respectiva grade (DUIMP)
            dgvDAguardandoDef.DataSource = _bsDAguardandoDef;
            dgvDVistoriaAgendada.DataSource = _bsDVistoriaAgendada;
            dgvDSolicitadoDataVistoria.DataSource = _bsDSolicitadoData;
            dgvDUIMPAguardandoRIF.DataSource = _bsDAguardandoChegada;
            dgvDLaudo.DataSource = _bsDAguardandoLaudo;
            dgvDUIMPProcessosRegistrados.DataSource = _bsDProcessosDadoEntrada;

            // Configura LI grids
            ConfigurarGrid(dgvAguardandoChegAgendVistoria);
            ConfigurarGrid(dgvSolicitadoDataVistoria);
            ConfigurarGrid(dgvVistoriaAgendada);
            ConfigurarGrid(dgvAguardandoDef);
            ConfigurarGrid(dgvLaudo);
            ConfigurarGrid(dgvProcessosDadoEntrada);

            // Configura DUIMP grids
            ConfigurarGrid(dgvDUIMPAguardandoRIF);
            ConfigurarGrid(dgvDSolicitadoDataVistoria);
            ConfigurarGrid(dgvDVistoriaAgendada);
            ConfigurarGrid(dgvDAguardandoDef);
            ConfigurarGrid(dgvDLaudo);
            ConfigurarGrid(dgvDUIMPProcessosRegistrados);
            // Garante que os cabeçalhos das grades DUIMP (DGVD*) fiquem iguais aos correspondentes das grades LI
            IgualarCabecalhosEntreGridsLIeDUIMP();
        }

        private void IgualarCabecalhosEntreGridsLIeDUIMP()
        {
            try
            {
                var pares = new (DataGridView li, DataGridView duimp)[]
                {
                    (dgvAguardandoChegAgendVistoria, dgvDUIMPAguardandoRIF),
                    (dgvSolicitadoDataVistoria, dgvDSolicitadoDataVistoria),
                    (dgvVistoriaAgendada, dgvDVistoriaAgendada),
                    (dgvAguardandoDef, dgvDAguardandoDef),
                    (dgvLaudo, dgvDLaudo),
                    (dgvProcessosDadoEntrada, dgvDUIMPProcessosRegistrados)
                };

                foreach (var (li, duimp) in pares)
                {
                    if (li == null || duimp == null) continue;

                    // copia modo e altura do cabeçalho
                    duimp.ColumnHeadersHeightSizeMode = li.ColumnHeadersHeightSizeMode;
                    duimp.ColumnHeadersHeight = li.ColumnHeadersHeight;
                    duimp.EnableHeadersVisualStyles = li.EnableHeadersVisualStyles;
                    // copia estilo visual do cabeçalho para manter mesma aparência
                    duimp.ColumnHeadersDefaultCellStyle = li.ColumnHeadersDefaultCellStyle;
                }
            }
            catch
            {
                // Não propagar exceção de UI
            }
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

            // Cabeçalho: define um tamanho padrão e permite redimensionamento programático
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            // Valor padrão que será usado caso o par correspondente (LI) não esteja disponível
            dgv.ColumnHeadersHeight = 30;

            // Aplica estilo de cabeçalho consistente
            dgv.EnableHeadersVisualStyles = true;
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

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

            var item = dgv.Rows[e.RowIndex].DataBoundItem;

            // Permitimos salvar alterações nas Notas para ambos os tipos
            if (nomeColunaEditada == "Notas")
            {
                if (item is Vistoria vistoriaEditada)
                {
                    await UpsertVistoriaComFilaAsync(vistoriaEditada);
                }
                else if (item is CLUSA.Models.VistoriaDUIMP vistoriaDuimp)
                {
                    await UpsertVistoriaDUIMPComFilaAsync(vistoriaDuimp);
                }

                return;
            }

            // Permite alteração da data Deferido SOMENTE nas grades DUIMP quando o TabDUIMP estiver ativo
            if (nomeColunaEditada == "Deferido")
            {
                // Verifica se o item é DUIMP
                if (item is CLUSA.Models.VistoriaDUIMP vistoriaDuimp2)
                {
                    // Só salva se o usuário estiver na aba DUIMP
                    if (tabControl1?.SelectedTab == tabDUIMP)
                    {
                        await UpsertVistoriaDUIMPComFilaAsync(vistoriaDuimp2);
                    }
                    else
                    {
                        // Rejeita alteração fora da aba DUIMP: restaura valor anterior (refresh do binding)
                        MessageBox.Show("A data de Deferido só pode ser alterada dentro da aba DUIMP.", "Permissão Negada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        // Forçar recarregar binding para desfazer edição na UI
                        dgv.SuspendLayout();
                        var bs = dgv.DataSource as BindingSource;
                        if (bs != null)
                        {
                            bs.ResetBindings(false);
                        }
                        dgv.ResumeLayout();
                    }
                }

                return;
            }
        }

        private void DGV_DataBindingComplete(object? sender, DataGridViewBindingCompleteEventArgs e)
        {
            // Garante que o evento foi disparado por uma grade
            if (sender is not DataGridView dgv) return;

            // Percorre cada linha da grade que acabou de ser preenchida
            foreach (DataGridViewRow row in dgv.Rows)
            {
                // Pega o objeto associado à linha (pode ser Vistoria ou VistoriaDUIMP)
                if (row.DataBoundItem is Vistoria vistoria)
                {
                    // A REGRA: Verifica se a parametrização é "Coleta de Amostra"
                    if (vistoria.ParametrizacaoLPCO?.ToUpper() == "COLETA DE AMOSTRA")
                    {
                        row.DefaultCellStyle.BackColor = Color.Gold;
                        row.DefaultCellStyle.ForeColor = Color.Black;
                    }
                    else
                    {
                        row.DefaultCellStyle.BackColor = SystemColors.Window;
                        row.DefaultCellStyle.ForeColor = SystemColors.ControlText;
                    }
                }
                else if (row.DataBoundItem is CLUSA.Models.VistoriaDUIMP duimp)
                {
                    // Regras visuais para DUIMP: se houver observação específica, destacar levemente
                    if (!string.IsNullOrWhiteSpace(duimp.Notas))
                    {
                        row.DefaultCellStyle.BackColor = Color.LightYellow;
                        row.DefaultCellStyle.ForeColor = Color.Black;
                    }
                    else
                    {
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
            if (dgvOrigem.CurrentRow == null)
            {
                MessageBox.Show("Por favor, selecione um item para mover.", "Aviso");
                return;
            }

            var dataItem = dgvOrigem.CurrentRow.DataBoundItem;

            // Caso seja Vistoria (LI)
            if (dataItem is Vistoria vistoriaSelecionada)
            {
                var statusAntigo = vistoriaSelecionada.Status;
                vistoriaSelecionada.Status = novoStatus;
                await UpsertVistoriaComFilaAsync(vistoriaSelecionada);

                bsOrigem.Remove(vistoriaSelecionada);
                bsDestino.Add(vistoriaSelecionada);

                AjustarAlturaDataGridView(dgvOrigem);
                AjustarAlturaDataGridView(dgvDestino);

                _ = _logRepo.RegistrarLogAsync(
                    "Movimentação Vistoria", _logado.Usuario,
                    $"LPCO {vistoriaSelecionada.LPCO} movido para {novoStatus}",
                    $"Processo: {vistoriaSelecionada.Ref_USA} | De: {statusAntigo} -> Para: {novoStatus} | Usuário: {_logado.Usuario}"
                );
                return;
            }

            // Caso seja VistoriaDUIMP (DUIMP)
            if (dataItem is CLUSA.Models.VistoriaDUIMP vistoriaDuimp)
            {
                var statusAntigo = vistoriaDuimp.Status;
                // Guarda o novo status como string do enum
                vistoriaDuimp.Status = novoStatus.ToString();
                await UpsertVistoriaDUIMPComFilaAsync(vistoriaDuimp);

                bsOrigem.Remove(vistoriaDuimp);
                bsDestino.Add(vistoriaDuimp);

                AjustarAlturaDataGridView(dgvOrigem);
                AjustarAlturaDataGridView(dgvDestino);

                _ = _logRepo.RegistrarLogAsync(
                    "Movimentação VistoriaDUIMP", _logado.Usuario,
                    $"DUIMP {vistoriaDuimp.DUIMP} movido para {novoStatus}",
                    $"Processo: {vistoriaDuimp.Ref_USA} | De: {statusAntigo} -> Para: {novoStatus} | Usuário: {_logado.Usuario}"
                );
                return;
            }

            MessageBox.Show("Tipo de item desconhecido. Não foi possível mover.", "Aviso");
        }

        private void AjustarTodosDataGridViews()
        {
            // LI
            AjustarAlturaDataGridView(dgvAguardandoChegAgendVistoria);
            AjustarAlturaDataGridView(dgvAguardandoDef);
            AjustarAlturaDataGridView(dgvLaudo);
            AjustarAlturaDataGridView(dgvProcessosDadoEntrada);
            AjustarAlturaDataGridView(dgvSolicitadoDataVistoria);
            AjustarAlturaDataGridView(dgvVistoriaAgendada);

            // DUIMP
            AjustarAlturaDataGridView(dgvDUIMPAguardandoRIF);
            AjustarAlturaDataGridView(dgvDAguardandoDef);
            AjustarAlturaDataGridView(dgvDLaudo);
            AjustarAlturaDataGridView(dgvDUIMPProcessosRegistrados);
            AjustarAlturaDataGridView(dgvDSolicitadoDataVistoria);
            AjustarAlturaDataGridView(dgvDVistoriaAgendada);
        }

        // --- MÉTODOS PARA SUBIR DE ESTÁGIO ---
        private async void BtnSobeSolicitado_Click(object? sender, EventArgs e)
        {
            await MoverVistoria(dgvAguardandoChegAgendVistoria, dgvSolicitadoDataVistoria, _bsAguardandoChegada, _bsSolicitadoData, StatusVistoria.SolicitarDataVistoria);
        }

        private async void BtnSobeAgendada_Click(object? sender, EventArgs e)
        {
            // 1. Verifica se tem um item selecionado ANTES de pedir a data
            if (dgvSolicitadoDataVistoria.CurrentRow?.DataBoundItem is not Vistoria vistoriaSelecionada)
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
                await MoverVistoria(dgvSolicitadoDataVistoria, dgvVistoriaAgendada, _bsSolicitadoData, _bsVistoriaAgendada, StatusVistoria.VistoriaAgendada);
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
            if (dgvVistoriaAgendada.CurrentRow?.DataBoundItem is not Vistoria vistoriaSelecionada)
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

                await MoverVistoria(dgvVistoriaAgendada, dgvAguardandoDef, _bsVistoriaAgendada, _bsAguardandoDef, StatusVistoria.AguardandoDeferimento);
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
            await MoverVistoria(dgvAguardandoDef, dgvLaudo, _bsAguardandoDef, _bsAguardandoLaudo, StatusVistoria.AguardandoLaudo);
        }

        // --- NOVOS MÉTODOS PARA DESCER DE ESTÁGIO ---
        private async void btnDesceParaAgendada_Click(object? sender, EventArgs e)
        {
            await MoverVistoria(dgvAguardandoDef, dgvSolicitadoDataVistoria, _bsAguardandoDef, _bsVistoriaAgendada, StatusVistoria.VistoriaAgendada);
        }
        private async void BtnDesceDeferimento_Click(object sender, EventArgs e)
        {
            await MoverVistoria(dgvLaudo, dgvVistoriaAgendada, _bsAguardandoLaudo, _bsAguardandoDef, StatusVistoria.AguardandoDeferimento);
        }

        private async void btnDesceParaSolicitado_Click(object? sender, EventArgs e)
        {
            await MoverVistoria(dgvVistoriaAgendada, dgvSolicitadoDataVistoria, _bsVistoriaAgendada, _bsSolicitadoData, StatusVistoria.SolicitarDataVistoria);
        }

        private async void btnDesceParaAguardando_Click(object? sender, EventArgs e)
        {
            await MoverVistoria(dgvSolicitadoDataVistoria, dgvAguardandoChegAgendVistoria, _bsSolicitadoData, _bsAguardandoChegada, StatusVistoria.AguardandoChegadaParaAgendar);
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
            if (dgvProcessosDadoEntrada.CurrentRow?.DataBoundItem is Vistoria vistoria)
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
            if (dgvAguardandoDef.CurrentRow?.DataBoundItem is Vistoria vistoria)
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
            if (dgvLaudo.CurrentRow?.DataBoundItem is Vistoria vistoria)
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
            if (dgvProcessosDadoEntrada.CurrentRow?.DataBoundItem is Vistoria vistoria)
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
        // Adicionar estes métodos na região "Lógica de Movimentação de Vistorias" (próximo aos métodos LI já existentes)
        private async void BtnDSobeSolicitado_Click(object? sender, EventArgs e)
        {
            await MoverVistoria(dgvDUIMPAguardandoRIF, dgvDSolicitadoDataVistoria,
                _bsDAguardandoChegada, _bsDSolicitadoData, StatusVistoria.SolicitarDataVistoria);
        }

        private async void BtnDSobeAgendada_Click(object? sender, EventArgs e)
        {
            // CORRIGIDO: Verifica se o item é VistoriaDUIMP
            if (dgvDSolicitadoDataVistoria.CurrentRow?.DataBoundItem is not CLUSA.Models.VistoriaDUIMP vistoriaSelecionada)
            {
                MessageBox.Show("Por favor, selecione um item para agendar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DateTime? dataEscolhida = SolicitarDataVistoria();
            if (dataEscolhida == null) return;

            try
            {
                Cursor = Cursors.WaitCursor;

                var processo = await _repositorioProcesso.GetByRefUsaAsync(vistoriaSelecionada.Ref_USA);
                if (processo != null)
                {
                    string novaLinha = $"{DateTime.Now:dd/MM/yyyy} Vistoria agendada para: {dataEscolhida.Value:dd/MM/yyyy}";
                    string historicoAntigo = processo.HistoricoDoProcesso ?? "";
                    string novoHistorico = $"{novaLinha}\r\n{historicoAntigo}".Trim();

                    var updates = new List<UpdateDefinition<Processo>>
            {
                Builders<Processo>.Update.Set(p => p.HistoricoDoProcesso, novoHistorico)
            };

                    await _repositorioProcesso.UpdateParcialAsync(processo.Id, updates);
                }

                // Move usando os BindingSources corretos da DUIMP
                await MoverVistoria(dgvDSolicitadoDataVistoria, dgvDVistoriaAgendada, _bsDSolicitadoData, _bsDVistoriaAgendada, StatusVistoria.VistoriaAgendada);
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

        private async void BtnDSobeAguardDef_Click(object? sender, EventArgs e)
        {
            // CORRIGIDO: Verifica se o item é VistoriaDUIMP
            if (dgvDVistoriaAgendada.CurrentRow?.DataBoundItem is not CLUSA.Models.VistoriaDUIMP vistoriaSelecionada)
            {
                MessageBox.Show("Por favor, selecione um item.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DateTime? dataEscolhida = SolicitarDataVistoria();
            if (dataEscolhida == null) return;

            try
            {
                Cursor = Cursors.WaitCursor;

                var processo = await _repositorioProcesso.GetByRefUsaAsync(vistoriaSelecionada.Ref_USA);
                if (processo != null)
                {
                    string novaLinha = $"{DateTime.Now:dd/MM/yyyy} Vistoria realizada em: {dataEscolhida.Value:dd/MM/yyyy}";
                    string historicoAntigo = processo.HistoricoDoProcesso ?? "";
                    string novoHistorico = $"{novaLinha}\r\n{historicoAntigo}".Trim();

                    var updates = new List<UpdateDefinition<Processo>>
            {
                Builders<Processo>.Update.Set(p => p.HistoricoDoProcesso, novoHistorico)
            };

                    await _repositorioProcesso.UpdateParcialAsync(processo.Id, updates);
                }

                await MoverVistoria(dgvDVistoriaAgendada, dgvDAguardandoDef, _bsDVistoriaAgendada, _bsDAguardandoDef, StatusVistoria.AguardandoDeferimento);
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

        private async void BtnDSobeLaudo_Click(object sender, EventArgs e)
        {
            await MoverVistoria(dgvDAguardandoDef, dgvDLaudo, _bsDAguardandoDef, _bsDAguardandoLaudo, StatusVistoria.AguardandoLaudo);
        }

        // Descidas (DUIMP)
        private async void btnDDesceParaAgendada_Click(object? sender, EventArgs e)
        {
            await MoverVistoria(dgvDAguardandoDef, dgvDSolicitadoDataVistoria, _bsDAguardandoDef, _bsDVistoriaAgendada, StatusVistoria.VistoriaAgendada);
        }

        private async void BtnDDesceDeferimento_Click(object sender, EventArgs e)
        {
            await MoverVistoria(dgvDLaudo, dgvDVistoriaAgendada, _bsDAguardandoLaudo, _bsDAguardandoDef, StatusVistoria.AguardandoDeferimento);
        }

        private async void btnDDesceParaSolicitado_Click(object? sender, EventArgs e)
        {
            await MoverVistoria(dgvDVistoriaAgendada, dgvDSolicitadoDataVistoria, _bsDVistoriaAgendada, _bsDSolicitadoData, StatusVistoria.SolicitarDataVistoria);
        }

        private async void btnDDesceParaAguardando_Click(object? sender, EventArgs e)
        {
            await MoverVistoria(dgvDSolicitadoDataVistoria, dgvDUIMPAguardandoRIF, _bsDSolicitadoData, _bsDAguardandoChegada, StatusVistoria.AguardandoChegadaParaAgendar);
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
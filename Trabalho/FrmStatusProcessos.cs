using CLUSA.Helpers;
using CLUSA.Models;
using CLUSA.Services;
using CLUSA.Repositories;
using System.Data;
using System.Reflection; 

namespace Trabalho
{
    public partial class FrmStatusProcessos : Form
    {
        public enum StatusBloco
        {
            AguardandoCE,
            ParaRedestinar,
            Redestinados,
            AtracadosSemPresencaCarga,
            SituacaoSIGVIG,
            AtracadosComPresencaCarga,
            Deferidos,
            SolicitarNumerario,
            DIDUIMPParaDigitacao
        }

        private class BlocoConfiguracao
        {
            public required string Nome { get; set; }
            public required Color Cor { get; set; }
            public required string GridName { get; set; }
            public required Func<FrmStatusProcessos, Control> ObterBotao { get; set; }
        }

        private readonly Dictionary<StatusBloco, BlocoConfiguracao> _configuracoes;

        // Estado da Tela
        private StatusBloco? _statusBlocoAtual;
        private List<Processo> _processosExibidos = [];
        private List<Processo> _todosProcessos = []; // Cache único do banco

        // Ordenação
        private string? _ultimaColunaOrdenada = null;
        private bool _ultimaDirecaoAscendente = true;

        // Componentes Auxiliares
        private FrmLoadingOverlay? _overlay;
        private readonly RepositorioUsers _repositorioUsers = new();
        private readonly RepositorioLog _repoLog = new();
        private Users? _usuarioLogado;
        private readonly Logado _logado;

        private enum BlocoExibido
        {
            Nenhum,
            StatusPadrao,
            SolicitarNumerario,
            DIDUIMPParaDigitacao
        }
        private BlocoExibido _blocoExibidoAtual = BlocoExibido.Nenhum;

        public FrmStatusProcessos(Logado logado)
        {
            InitializeComponent();
            MostrarItens.Visible = false;
            _logado = logado;
            DGVSelecionado.DoubleBuffered(true);

            // Inicializa configurações mapeando Status -> UI
            _configuracoes = new Dictionary<StatusBloco, BlocoConfiguracao>
            {
                { StatusBloco.AguardandoCE, new BlocoConfiguracao { Nome = "Aguardando CE", Cor = Color.BlueViolet, GridName = "DGVAguardandoCE", ObterBotao = f => f.BtnAguardandoCE } },
                { StatusBloco.ParaRedestinar, new BlocoConfiguracao { Nome = "Para Redestinar", Cor = Color.Red, GridName = "DGVParaRedestinar", ObterBotao = f => f.BtnParaRedestinar } },
                { StatusBloco.Redestinados, new BlocoConfiguracao { Nome = "Redestinados", Cor = Color.FromArgb(0, 192, 192), GridName = "DGVRedestinados", ObterBotao = f => f.BtnRedestinados } },
                { StatusBloco.AtracadosSemPresencaCarga, new BlocoConfiguracao { Nome = "Atracados S/Presença de Carga", Cor = Color.Yellow, GridName = "DGVAtracadosSemPresencaCarga", ObterBotao = f => f.BtnAtracadosSPresencaDeCarga } },
                { StatusBloco.SituacaoSIGVIG, new BlocoConfiguracao { Nome = "Atracados Situação SIGVIG", Cor = Color.FromArgb(255, 128, 0), GridName = "DGVSituacaoSIGVIG", ObterBotao = f => f.BtnSituacaoSIGVIG } },
                { StatusBloco.AtracadosComPresencaCarga, new BlocoConfiguracao { Nome = "Atracados com Presença de Carga", Cor = Color.Black, GridName = "DGVAtracadosComPresencaCarga", ObterBotao = f => f.BtnAtracadosCPresencaDeCarga } },
                { StatusBloco.Deferidos, new BlocoConfiguracao { Nome = "Deferidos", Cor = Color.Lime, GridName = "DGVDeferidos", ObterBotao = f => f.BtnDeferidos } },
                { StatusBloco.SolicitarNumerario, new BlocoConfiguracao { Nome = "Solicitar Numerário", Cor = Color.FromArgb(255, 192, 192), GridName = "DGVSolicitarNumerario", ObterBotao = f => f.BtnSolicitarNumerario } },
                { StatusBloco.DIDUIMPParaDigitacao, new BlocoConfiguracao { Nome = "DI/DUIMP para Digitação", Cor = Color.FromArgb(192, 0, 0), GridName = "DGVDIDUIMPParaDigitacao", ObterBotao = f => f.BtnDIDUIMPParaDigitacao } }
            };
        }
        private async void FrmStatusProcessos_Load(object? sender, EventArgs e)
        {
            _usuarioLogado = await _repositorioUsers.GetByIdAsync(_logado.Id);
            if (_usuarioLogado == null)
            {
                MessageBox.Show("Erro ao carregar usuário.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            GridColumnManager.RegistrarCatalogosPadrao();
            GridColumnManager.ConfigurarFormatacaoListas(DGVSelecionado);

            await CarregarProcessosAsync();
        }
        private List<Processo> ObterProcessosPorStatus(StatusBloco status)
        {
            var hoje = DateTime.Now.Date;

            // Filtros unificados
            return status switch
            {
                StatusBloco.AguardandoCE => _todosProcessos
                    .Where(p => string.IsNullOrWhiteSpace(p.CE)).ToList(),

                StatusBloco.ParaRedestinar => _todosProcessos
                    .Where(p => !string.IsNullOrWhiteSpace(p.Veiculo) && p.Redestinacao != true).ToList(),

                StatusBloco.Redestinados => _todosProcessos
                    .Where(p => p.Redestinacao == true && (!p.DataDeAtracacao.HasValue || p.DataDeAtracacao.Value.Date > hoje)).ToList(),

                StatusBloco.AtracadosSemPresencaCarga => _todosProcessos
                    .Where(p => p.DataDeAtracacao.HasValue && p.DataDeAtracacao.Value.Date <= hoje && !p.PresencaDeCarga).ToList(),

                StatusBloco.SituacaoSIGVIG => _todosProcessos
                    .Where(p => p.DataDeAtracacao.HasValue && p.DataDeAtracacao.Value.Date <= hoje && !p.SigVig).ToList(),

                StatusBloco.AtracadosComPresencaCarga => _todosProcessos
                    .Where(p => p.PresencaDeCarga).ToList(),

                StatusBloco.Deferidos => _todosProcessos
                    .Where(p => ProcessoHelper.IsDeferido(p) && !p.DataRegistroDI.HasValue).ToList(),

                StatusBloco.SolicitarNumerario => _todosProcessos
                    .Where(p => p.DataDeAtracacao.HasValue && !p.Numerario).ToList(),

                StatusBloco.DIDUIMPParaDigitacao => _todosProcessos
                    .Where(p => p.DataDeAtracacao.HasValue && string.IsNullOrWhiteSpace(p.RascunhoDI)).ToList(),

                _ => new List<Processo>()
            };
        }
        private async Task CarregarProcessosAsync()
        {
            try
            {
                MostrarLoading("Carregando processos...");
                var processoService = new RepositorioProcesso();

                var todos = await processoService.ListarProcessosAtivosParaStatusAsync();

                // 2. MELHORIA: Filtragem Case-Insensitive segura
                _todosProcessos = todos
                    .Where(p => !string.Equals(p.Status, "Finalizado", StringComparison.OrdinalIgnoreCase))
                    .ToList();

                // Processamento paralelo para cálculo de propriedades não persistidas
                await Task.Run(() =>
                {
                    _todosProcessos.AsParallel().ForAll(p => ProcessoHelper.AtualizarCondicaoProcesso(p));
                });

                AtualizarContadores();
            }
            finally
            {
                EsconderLoading();
            }
        }

        private void MostrarItensPorStatus(StatusBloco status)
        {
            DGVSelecionado.SuspendLayout();
            try
            {
                MostrarItens.Visible = true;
                Blocos.Visible = false;
                _statusBlocoAtual = status;

                var processos = ObterProcessosPorStatus(status);
                _processosExibidos = OrdenarLista(processos);

                if (!_configuracoes.TryGetValue(status, out var config)) return;

                if (_usuarioLogado?.PreferenciasGrids == null)
                {
                    if (_usuarioLogado != null)
                        _usuarioLogado.PreferenciasGrids = new Dictionary<string, List<string>>();
                    else
                        return;
                }

                _usuarioLogado.PreferenciasGrids.TryGetValue(config.GridName, out var colunasVisiveis);

                DGVSelecionado.DataSource = null;
                DGVSelecionado.Columns.Clear();

                GridColumnManager.ConfigurarGrid(DGVSelecionado, config.GridName, colunasVisiveis ?? new List<string>());
                DGVSelecionado.DataSource = _processosExibidos;

                LblTitulo.Text = $"{config.Nome} ({processos.Count})";
                LblTitulo.ForeColor = config.Cor == Color.Black ? Color.White : Color.Black;
                LblTitulo.BackColor = config.Cor;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao exibir grid: {ex.Message}");
            }
            finally
            {
                DGVSelecionado.ResumeLayout();
            }
        }

        private List<Processo> OrdenarLista(List<Processo>? lista)
        {
            if (lista == null || lista.Count == 0) return new List<Processo>();

            // Lógica de ordenação específica para Ref_USA
            if (string.IsNullOrEmpty(_ultimaColunaOrdenada) || _ultimaColunaOrdenada == "Ref_USA")
            {
                // 3. REFATORAÇÃO: Lógica de ordenação mais limpa
                var query = lista
                    .OrderBy(p => IsITJ(p.Ref_USA) ? 1 : 0) // ITJ pro final
                    .ThenBy(p => string.IsNullOrWhiteSpace(p.Ref_USA) ? 1 : 0); // Vazios pro final

                return _ultimaDirecaoAscendente
                    ? query.ThenBy(p => ExtrairAnoNumeroSortKey(p.Ref_USA)).ToList()
                    : query.ThenByDescending(p => ExtrairAnoNumeroSortKey(p.Ref_USA)).ToList();
            }

            // Reflection para outras colunas
            var propInfo = typeof(Processo).GetProperty(_ultimaColunaOrdenada);
            if (propInfo == null) return lista;

            Func<Processo, object?> selector = p => propInfo.GetValue(p);

            return _ultimaDirecaoAscendente
                ? lista.OrderBy(p => selector(p) == null ? 1 : 0).ThenBy(selector).ToList()
                : lista.OrderBy(p => selector(p) == null ? 1 : 0).ThenByDescending(selector).ToList();
        }

        private static bool IsITJ(string? refUsa) =>
            !string.IsNullOrEmpty(refUsa) && refUsa.TrimEnd().EndsWith("ITJ", StringComparison.OrdinalIgnoreCase);

        private static long ExtrairAnoNumeroSortKey(string? refUsa)
        {
            if (string.IsNullOrWhiteSpace(refUsa)) return 0;

            // 4. PERFORMANCE: Span para evitar alocação de string desnecessária (Opcional, mas bom para listas grandes)
            // Mantendo lógica simples do split original mas com proteção
            var partes = refUsa.Split(new[] { '/', ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (partes.Length >= 2 && int.TryParse(partes[0], out int numero) && int.TryParse(partes[1], out int ano))
            {
                int anoCompleto = ano < 100 ? 2000 + ano : ano;
                return (long)anoCompleto * 1000000 + numero;
            }
            return 0;
        }

        private void DGVSelecionado_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            var coluna = DGVSelecionado.Columns[e.ColumnIndex];
            var propriedade = coluna.DataPropertyName;

            if (string.IsNullOrWhiteSpace(propriedade) || _processosExibidos.Count == 0) return;

            if (_ultimaColunaOrdenada == propriedade)
                _ultimaDirecaoAscendente = !_ultimaDirecaoAscendente;
            else
            {
                _ultimaColunaOrdenada = propriedade;
                _ultimaDirecaoAscendente = true;
            }

            // Apenas reordena a lista atual, não recarrega do banco
            _processosExibidos = OrdenarLista(_processosExibidos);
            DGVSelecionado.DataSource = _processosExibidos;
            DGVSelecionado.Refresh();

            // Atualiza setinhas
            foreach (DataGridViewColumn col in DGVSelecionado.Columns)
                col.HeaderCell.SortGlyphDirection = SortOrder.None;

            coluna.HeaderCell.SortGlyphDirection = _ultimaDirecaoAscendente ? SortOrder.Ascending : SortOrder.Descending;
        }

        private void AtualizarContadores()
        {
            // 5. LIMPEZA: Loop genérico usando a configuração centralizada
            foreach (var kvp in _configuracoes)
            {
                StatusBloco status = kvp.Key;
                BlocoConfiguracao config = kvp.Value;

                int count = ObterProcessosPorStatus(status).Count;
                var btn = config.ObterBotao(this);

                if (btn != null)
                {
                    btn.Text = count > 0 ? $"{config.Nome}\n({count})" : config.Nome;
                }
            }
        }

        private void BtnAguardandoCE_Click(object sender, EventArgs e) => MostrarItensPorStatus(StatusBloco.AguardandoCE);
        private void BtnParaRedestinar_Click(object sender, EventArgs e) => MostrarItensPorStatus(StatusBloco.ParaRedestinar);
        private void BtnRedestinados_Click(object sender, EventArgs e) => MostrarItensPorStatus(StatusBloco.Redestinados);
        private void BtnAtracadosSPresencaDeCarga_Click(object sender, EventArgs e) => MostrarItensPorStatus(StatusBloco.AtracadosSemPresencaCarga);
        private void BtnSituacaoSIGVIG_Click(object sender, EventArgs e) => MostrarItensPorStatus(StatusBloco.SituacaoSIGVIG);
        private void BtnAtracadosCPresencaDeCarga_Click(object sender, EventArgs e) => MostrarItensPorStatus(StatusBloco.AtracadosComPresencaCarga);
        private void BtnDeferidos_Click(object sender, EventArgs e) => MostrarItensPorStatus(StatusBloco.Deferidos);
        private void BtnSolicitarNumerario_Click(object sender, EventArgs e) => MostrarItensPorStatus(StatusBloco.SolicitarNumerario);
        private void BtnDIDUIMPParaDigitacao_Click(object sender, EventArgs e) => MostrarItensPorStatus(StatusBloco.DIDUIMPParaDigitacao);

        private async void DGVSelecionado_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= _processosExibidos.Count) return;

            var processoSelecionado = _processosExibidos[e.RowIndex];
            string idSelecionado = processoSelecionado.Id.ToString(); // Supondo que Id exista na Model

            // 6. UX: Só recarrega se o usuário realmente alterar algo (DialogResult.OK)
            // Se o seu form FrmModificaProcesso não retorna OK, remova o "if" e deixe o reload direto.
            using var frm = new FrmModificaProcesso { processo = processoSelecionado, Modo = "Editar" };

            // Sugestão: Configurar FrmModificaProcesso para retornar DialogResult.OK ao salvar
            var result = frm.ShowDialog();

            // Recarrega sempre para garantir consistência (seguro)
            await CarregarProcessosAsync();

            if (_statusBlocoAtual.HasValue)
            {
                MostrarItensPorStatus(_statusBlocoAtual.Value);
                RestaurarSelecao(idSelecionado);
            }
        }

        // EVENTO 1: Força o Commit imediato ao clicar no CheckBox
        private void DGVSelecionado_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (DGVSelecionado.IsCurrentCellDirty)
            {
                // Isso força o evento CellValueChanged a disparar imediatamente
                DGVSelecionado.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        // EVENTO 2: Salva no Banco de Dados
        private async void DGVSelecionado_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            MessageBox.Show($"{DGVSelecionado.Columns[e.ColumnIndex].DataPropertyName} de {_processosExibidos[e.RowIndex].Ref_USA} foi alterado.");

            // Verificações de segurança
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            // Evita erro se a lista estiver vazia ou índice inválido
            if (e.RowIndex >= _processosExibidos.Count) return;

            // Verifica se a coluna alterada é do tipo CheckBox
            if (DGVSelecionado.Columns[e.ColumnIndex] is DataGridViewCheckBoxColumn)
            {
                try
                {
                    var processoAlterado = _processosExibidos[e.RowIndex];

                    var repo = new RepositorioProcesso();

                    await repo.UpdateAsync(processoAlterado);

                    await _repoLog.RegistrarLogAsync("Edição", _logado.Usuario,
                                                               $"CheckBox {DGVSelecionado.Columns[e.ColumnIndex].DataPropertyName} de {processoAlterado.Ref_USA} foi alterado. " +
                                                               $"(Agora: Capa: {processoAlterado.CapaOK} / Numerário: {processoAlterado.Numerario})", $"ID: {processoAlterado.Id}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao atualizar checkbox: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    await CarregarProcessosAsync(); 
                }
            }
        }

        private void RestaurarSelecao(string idProcesso)
        {
            if (string.IsNullOrEmpty(idProcesso)) return;

            // Busca eficiente
            var item = _processosExibidos.FirstOrDefault(p => p.Id.ToString() == idProcesso);
            if (item != null)
            {
                int index = _processosExibidos.IndexOf(item);
                if (index >= 0 && index < DGVSelecionado.Rows.Count)
                {
                    DGVSelecionado.ClearSelection();
                    DGVSelecionado.Rows[index].Selected = true;
                    DGVSelecionado.FirstDisplayedScrollingRowIndex = index;
                }
            }
        }
        private void BtnVoltar_Click(object sender, EventArgs e)
        {
            MostrarItens.Visible = false;
            Blocos.Visible = true;
            AtualizarContadores();
        }
        private void BtnDownloadPDF_Click(object sender, EventArgs e)
        {
            // 1. Verifica se tem dados antes de abrir qualquer janela
            if (DGVSelecionado.Rows.Count == 0)
            {
                MessageBox.Show("Não há dados na tabela para exportar.", "Aviso");
                return;
            }

            // 2. Lógica de seleção (Pergunta ao usuário)
            bool apenasSelecionadas = false;
            if (DGVSelecionado.SelectedRows.Count > 0)
            {
                var resp = MessageBox.Show(
                    $"Você tem {DGVSelecionado.SelectedRows.Count} linhas selecionadas.\nDeseja exportar APENAS a seleção?\n\n(Não = Exportar tudo)",
                    "Opções de Exportação",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);

                if (resp == DialogResult.Cancel) return;
                apenasSelecionadas = (resp == DialogResult.Yes);
            }

            // 3. Define nome do arquivo e Título
            using var sfd = new SaveFileDialog();
            sfd.Filter = "Arquivo PDF (*.pdf)|*.pdf";

            string nomeBloco = _statusBlocoAtual.HasValue ? _configuracoes[_statusBlocoAtual.Value].Nome : "Geral";
            // Remove caracteres inválidos do nome do arquivo se houver
            string nomeLimpo = string.Join("_", nomeBloco.Split(Path.GetInvalidFileNameChars()));

            sfd.FileName = $"Relatorio_{nomeLimpo}_{DateTime.Now:yyyyMMdd_HHmm}.pdf";

            if (sfd.ShowDialog() != DialogResult.OK) return;

            // 4. Chama a classe separada (Service)
            try
            {
                MostrarLoading("Gerando PDF...");

                // A Mágica acontece aqui: Uma linha resolve tudo
                PdfExportService.ExportarGridParaPdf(
                    DGVSelecionado,
                    sfd.FileName,
                    $"Relatório: {nomeBloco}",
                    apenasSelecionadas
                );

                EsconderLoading();

                // 5. Log e Sucesso
                int qtdRegistros = apenasSelecionadas ? DGVSelecionado.SelectedRows.Count : DGVSelecionado.Rows.Count; // Aproximado para log

                _ = Task.Run(() => _repoLog.RegistrarLogAsync(
                    "Exportação", _logado.Usuario,
                    $"PDF gerado: {nomeBloco}",
                    $"Registros exportados: {qtdRegistros}"
                ));

                if (MessageBox.Show("PDF gerado com sucesso! Deseja abrir agora?", "Sucesso",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    var p = new System.Diagnostics.ProcessStartInfo(sfd.FileName) { UseShellExecute = true };
                    System.Diagnostics.Process.Start(p);
                }
            }
            catch (System.IO.IOException)
            {
                EsconderLoading();
                MessageBox.Show("O arquivo está aberto em outro programa. Feche-o e tente novamente.",
                    "Arquivo em Uso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                EsconderLoading();
                MessageBox.Show($"Erro ao gerar PDF: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void MostrarLoading(string mensagem)
        {
            if (_overlay != null) return;
            _overlay = new FrmLoadingOverlay { Opacity = 0.60 };
            _overlay.lblLoading.Text = mensagem;
            var rect = this.RectangleToScreen(this.ClientRectangle);
            _overlay.StartPosition = FormStartPosition.Manual;
            _overlay.Location = rect.Location;
            _overlay.Size = rect.Size;
            _overlay.Show(this);
            _overlay.BringToFront();
        }

        private void EsconderLoading()
        {
            _overlay?.Close();
            _overlay?.Dispose();
            _overlay = null;
        }
    }
}
public static class DataGridViewExtensions
{
    public static void DoubleBuffered(this DataGridView dgv, bool setting)
    {
        Type dgvType = dgv.GetType();
        PropertyInfo? pi = dgvType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
        pi?.SetValue(dgv, setting, null);
    }
}
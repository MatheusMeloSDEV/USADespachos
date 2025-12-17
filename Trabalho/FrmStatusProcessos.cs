using CLUSA; // Models e ProcessoHelper aqui
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing; // Necessário para Color
using System.Linq;
using System.Reflection; // Para o DoubleBuffer
using System.Threading.Tasks;
using System.Windows.Forms;

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

        private static readonly Dictionary<StatusBloco, (string Nome, Color Cor, string GridName)> BlocoConfig =
            new()
            {
                { StatusBloco.AguardandoCE, ("Aguardando CE", Color.BlueViolet, "DGVAguardandoCE") },
                { StatusBloco.ParaRedestinar, ("Para Redestinar", Color.Red, "DGVParaRedestinar") },
                { StatusBloco.Redestinados, ("Redestinados", Color.FromArgb(0,192,192), "DGVRedestinados") },
                { StatusBloco.AtracadosSemPresencaCarga, ("Atracados S/Presença de Carga", Color.Yellow, "DGVAtracadosSemPresencaCarga") },
                { StatusBloco.SituacaoSIGVIG, ("Atracados Situação SIGVIG", Color.FromArgb(255,128,0), "DGVSituacaoSIGVIG") },
                { StatusBloco.AtracadosComPresencaCarga, ("Atracados com Presença de Carga", Color.Black, "DGVAtracadosComPresencaCarga") },
                { StatusBloco.Deferidos, ("Deferidos", Color.Lime, "DGVDeferidos") },
                { StatusBloco.SolicitarNumerario, ("Solicitar Numerário", Color.FromArgb(255,192,192), "DGVSolicitarNumerario") },
                { StatusBloco.DIDUIMPParaDigitacao, ("DI/DUIMP para Digitação", Color.FromArgb(192,0,0), "DGVDIDUIMPParaDigitacao") }
            };

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
        private Users? _usuarioLogado;
        private readonly Logado _logado;

        private Control? ObterControlePorStatus(StatusBloco status)
        {
            return status switch
            {
                StatusBloco.AguardandoCE => BtnAguardandoCE,
                StatusBloco.ParaRedestinar => BtnParaRedestinar,
                StatusBloco.Redestinados => BtnRedestinados,
                StatusBloco.AtracadosSemPresencaCarga => BtnAtracadosSPresencaDeCarga,
                StatusBloco.SituacaoSIGVIG => BtnSituacaoSIGVIG,
                StatusBloco.AtracadosComPresencaCarga => BtnAtracadosCPresencaDeCarga,
                StatusBloco.Deferidos => BtnDeferidos,
                StatusBloco.SolicitarNumerario => BtnSolicitarNumerario,
                StatusBloco.DIDUIMPParaDigitacao => BtnDIDUIMPParaDigitacao,
                _ => null
            };
        }

        private enum BlocoExibido
        {
            Nenhum,
            StatusPadrao,
            SolicitarNumerario,
            DIDUIMPParaDigitacao
        }
        private BlocoExibido _blocoExibidoAtual = BlocoExibido.Nenhum;

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

        private static readonly Dictionary<string, Func<Processo, object?>> _propSelectors = new()
        {
            // Identificadores Principais
            { "Ref_USA", p => p.Ref_USA },
            { "Importador", p => p.Importador },
            { "SR", p => p.SR },
            { "Produto", p => p.Produto },
            { "Marca", p => p.Marca },
            { "Veiculo", p => p.Veiculo },
            { "Conhecimento", p => p.Conhecimento },
            { "Armador", p => p.Armador },
            { "CE", p => p.CE },
            { "Container", p => p.Container },
            { "Origem", p => p.Origem },
        
            // Logística e Prazos
            { "PortoDestino", p => p.PortoDestino },
            { "Terminal", p => p.Terminal },
            { "LocalDeDesembaraco", p => p.LocalDeDesembaraco },
            { "FLO", p => p.FLO },
            { "FreeTime", p => p.FreeTime },
            { "VencimentoFreeTime", p => p.VencimentoFreeTime },
            { "VencimentoFMA", p => p.VencimentoFMA },
            { "VencimentoLI_LPCO", p => p.VencimentoLI_LPCO },
        
            // Datas Importantes
            { "DataDeAtracacao", p => p.DataDeAtracacao },
            { "DataEmbarque", p => p.DataEmbarque },
            { "Inspecao", p => p.Inspecao },
            { "DataRecebOriginais", p => p.DataRecebOriginais },
            { "FormaRecOriginais", p => p.FormaRecOriginais },
        
            // DI / Desembaraço
            { "DI", p => p.DI },
            { "RascunhoDI", p => p.RascunhoDI },
            { "ParametrizacaoDI", p => p.ParametrizacaoDI },
            { "DataRegistroDI", p => p.DataRegistroDI },
            { "DataDesembaracoDI", p => p.DataDesembaracoDI },
            { "DataCarregamentoDI", p => p.DataCarregamentoDI },
            { "DataMinutaDI", p => p.DataMinutaDI },
        
            // Status e Controles (CheckBoxes / Booleans)
            { "PresencaDeCarga", p => p.PresencaDeCarga },
            { "CapaOK", p => p.CapaOK },
            { "SIGVIGLiberado", p => p.SIGVIGLiberado },
            { "SIGVIGSelecionado", p => p.SIGVIGSelecionado },
            { "ResultadoLab", p => p.ResultadoLab },
            { "Amostra", p => p.Amostra },
            { "Desovado", p => p.Desovado },
            { "Redestinacao", p => p.Redestinacao },
            { "Numerario", p => p.Numerario },
            { "SigVig", p => p.SigVig }, // Verifique se na model é SigVig ou SIGVIG
            { "PossuiEmbarque", p => p.PossuiEmbarque },
        
            // Campos Descritivos / Status Geral
            { "HistoricoDoProcesso", p => p.HistoricoDoProcesso },
            { "Pendencia", p => p.Pendencia },
            { "Status", p => p.Status },
            { "CondicaoProcesso", p => p.CondicaoProcesso },
            { "OrgaosAnuentesString", p => p.OrgaosAnuentesString }
        };

        private static readonly Dictionary<StatusBloco, (string Nome, Color Cor)> BlocoInfo =
            new()
            {
                { StatusBloco.AguardandoCE, ("Aguardando CE", Color.BlueViolet) },
                { StatusBloco.ParaRedestinar, ("Para Redestinar", Color.Red) },
                { StatusBloco.Redestinados, ("Redestinados", Color.FromArgb(0,192,192)) },
                { StatusBloco.AtracadosSemPresencaCarga, ("Atracados S/Presença de Carga", Color.Yellow) },
                { StatusBloco.SituacaoSIGVIG, ("Situação SIGVIG", Color.FromArgb(255,128,0)) },
                { StatusBloco.AtracadosComPresencaCarga, ("Atracados com Presença de Carga", Color.Black) },
                { StatusBloco.Deferidos, ("Deferidos", Color.Lime) },
                { StatusBloco.SolicitarNumerario, ("Solicitar Numerário", Color.FromArgb(255,192,192)) },
                { StatusBloco.DIDUIMPParaDigitacao, ("DI/DUIMP para Digitação", Color.FromArgb(192,0,0)) }
            };

        private readonly List<dynamic> _dadosExibicaoAtual = [];
        public FrmStatusProcessos(Logado logado)
        {
            InitializeComponent();
            MostrarItens.Visible = false;
            _logado = logado;
            _bindingSource = [];
            DGVSelecionado.DoubleBuffered(true);
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

            // AQUI: A Única chamada ao banco de dados
            await CarregarProcessosAsync();
        }
        // Só filtra pelo status calculado
        // SUBSTITUIR ESTE MÉTODO NO SEU FrmStatusProcessos.cs
        private List<Processo> ObterProcessosPorStatus(StatusBloco status)
        {
            // Variável auxiliar para data atual (normalizada para data sem hora, se necessário)
            var hoje = DateTime.Now.Date;

            return status switch
            {
                // 1. AGUARDANDO CE: Entra quando CE em branco (Navio já validado pelo status "Ativo" do banco ou Ref_USA)
                StatusBloco.AguardandoCE => _todosProcessos
                    .Where(p => (string.IsNullOrWhiteSpace(p.CE)))
                    .ToList(),

                // 2. PARA REDESTINAR: Tem CE preenchido, mas não está marcado como Redestinação
                StatusBloco.ParaRedestinar => _todosProcessos
                    .Where(p => !string.IsNullOrWhiteSpace(p.Veiculo)
                             && p.Redestinacao != true)
                    .ToList(),

                // 3. REDESTINADOS: Marcado como Redestinação e ainda não atracou (ou atracou hoje/futuro)
                // *Nota: Sai quando Data de Atracação for atual ou menor (ou seja, quando atraca de fato, vira atracado)*
                StatusBloco.Redestinados => _todosProcessos
                    .Where(p => p.Redestinacao == true
                             && (!p.DataDeAtracacao.HasValue || p.DataDeAtracacao.Value.Date > hoje))
                    .ToList(),

                // 4. ATRACADOS SEM PRESENÇA: Atracado (data <= hoje) e sem flag de carga
                StatusBloco.AtracadosSemPresencaCarga => _todosProcessos
                    .Where(p => p.DataDeAtracacao.HasValue
                             && p.DataDeAtracacao.Value.Date <= hoje
                             && !p.PresencaDeCarga)
                    .ToList(),

                // 5. ATRACADOS SITUAÇÃO SIGVIG: Atracado (data <= hoje) e SigVig pendente (false)
                StatusBloco.SituacaoSIGVIG => _todosProcessos
                    .Where(p => p.DataDeAtracacao.HasValue
                             && p.DataDeAtracacao.Value.Date <= hoje
                             && !p.SigVig) // !p.SigVig significa que não está OK
                    .ToList(),

                // 6. ATRACADOS COM PRESENÇA: Apenas a flag de presença
                StatusBloco.AtracadosComPresencaCarga => _todosProcessos
                    .Where(p => p.PresencaDeCarga)
                    .ToList(),

                // 7. DEFERIDOS: Regra complexa de LI/LPCO
                StatusBloco.Deferidos => _todosProcessos
                    .Where(p => ProcessoHelper.IsDeferido(p) && !p.DataRegistroDI.HasValue)
                    .ToList(),

                // 8. SOLICITAR NUMERÁRIO: Atracado e sem numerário
                StatusBloco.SolicitarNumerario => _todosProcessos
                    .Where(p => p.DataDeAtracacao.HasValue && !p.Numerario)
                    .ToList(),

                // 9. DI/DUIMP PARA DIGITAÇÃO: Atracado e sem rascunho de DI
                StatusBloco.DIDUIMPParaDigitacao => _todosProcessos
                    .Where(p => p.DataDeAtracacao.HasValue && string.IsNullOrWhiteSpace(p.RascunhoDI))
                    .ToList(),

                _ => new List<Processo>()
            };
        }
        private async Task CarregarProcessosAsync()
        {
            try
            {
                MostrarLoading("Carregando processos...");

                var processoService = new RepositorioProcesso();

                // 1. Busca Única no Banco
                var todos = await processoService.ListarProcessosAtivosParaStatusAsync();

                // 2. Filtra finalizados gerais e guarda em memória
                _todosProcessos = todos
                    .Where(p => !string.Equals(p.Status, "Finalizado", StringComparison.OrdinalIgnoreCase))
                    .ToList();

                // Nota: Ainda chamamos o Helper para preencher a coluna "CondicaoProcesso" (para exibição no Grid),
                // mas NÃO usamos mais ela para filtrar os blocos.
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
                // 1. Prepara UI
                MostrarItens.Visible = true;
                Blocos.Visible = false;

                _statusBlocoAtual = status;

                // 2. Obtém e Ordena Dados
                var processos = ObterProcessosPorStatus(status);
                _processosExibidos = OrdenarLista(processos);

                // 3. Configura Grid
                if (!BlocoConfig.TryGetValue(status, out var config)) return;

                if (_usuarioLogado?.PreferenciasGrids == null)
                    _usuarioLogado!.PreferenciasGrids = new Dictionary<string, List<string>>();

                _usuarioLogado.PreferenciasGrids.TryGetValue(config.GridName, out var colunasVisiveis);

                // Limpeza completa
                DGVSelecionado.DataSource = null;
                DGVSelecionado.Columns.Clear();

                GridColumnManager.ConfigurarGrid(DGVSelecionado, config.GridName, colunasVisiveis);

                // 4. Vincula Dados
                DGVSelecionado.DataSource = _processosExibidos;

                // 5. Atualiza Cabeçalho
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


        // OTIMIZAÇÃO DE ORDENAÇÃO
        private List<Processo> OrdenarLista(List<Processo>? lista)
        {
            if (lista == null || lista.Count == 0) return new List<Processo>();

            Func<Processo, object?> selector = null;
            if (!string.IsNullOrEmpty(_ultimaColunaOrdenada))
            {
                var propInfo = typeof(Processo).GetProperty(_ultimaColunaOrdenada);
                if (propInfo != null) selector = p => propInfo.GetValue(p);
            }

            if (selector == null || _ultimaColunaOrdenada == "Ref_USA")
            {
                var query = lista
                    .OrderBy(p => IsITJ(p.Ref_USA) ? 1 : 0)
                    .ThenBy(p => string.IsNullOrWhiteSpace(p.Ref_USA) ? 1 : 0);

                return _ultimaDirecaoAscendente
                    ? query.ThenBy(p => ExtrairAnoNumeroSortKey(p.Ref_USA)).ToList()
                    : query.ThenByDescending(p => ExtrairAnoNumeroSortKey(p.Ref_USA)).ToList();
            }

            return _ultimaDirecaoAscendente
                ? lista.OrderBy(p => selector(p) == null ? 1 : 0).ThenBy(selector).ToList()
                : lista.OrderBy(p => selector(p) == null ? 1 : 0).ThenByDescending(selector).ToList();
        }
        private static bool IsITJ(string refUsa) => refUsa != null && refUsa.TrimEnd().EndsWith("ITJ", StringComparison.OrdinalIgnoreCase);

        private static long ExtrairAnoNumeroSortKey(string refUsa)
        {
            if (string.IsNullOrWhiteSpace(refUsa)) return 0;
            var partes = refUsa.Split('/', ' ');
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

            _processosExibidos = OrdenarLista(_processosExibidos);
            DGVSelecionado.DataSource = _processosExibidos;

            foreach (DataGridViewColumn col in DGVSelecionado.Columns)
                col.HeaderCell.SortGlyphDirection = (col.Name == coluna.Name)
                    ? (_ultimaDirecaoAscendente ? SortOrder.Ascending : SortOrder.Descending)
                    : SortOrder.None;
        }
        private void AtualizarContadores()
        {
            foreach (StatusBloco status in Enum.GetValues(typeof(StatusBloco)))
            {
                // Chama a lógica independente para cada bloco
                int count = ObterProcessosPorStatus(status).Count;

                var controle = ObterControlePorStatus(status);

                if (controle != null && BlocoConfig.TryGetValue(status, out var info))
                {
                    controle.Text = count > 0 ? $"{info.Nome}\n({count})" : info.Nome;
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
            string idSelecionado = processoSelecionado.Id.ToString();

            using var frm = new FrmModificaProcesso { processo = processoSelecionado, Modo = "Editar" };
            frm.ShowDialog();

            await CarregarProcessosAsync();

            if (_statusBlocoAtual.HasValue)
            {
                MostrarItensPorStatus(_statusBlocoAtual.Value);
                RestaurarSelecao(idSelecionado);
            }
        }

        private void RestaurarSelecao(string idProcesso)
        {
            if (string.IsNullOrEmpty(idProcesso)) return;
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
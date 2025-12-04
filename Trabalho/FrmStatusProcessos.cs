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
        private string ObterNomeGridStatus(StatusBloco status)
        {
            return status switch
            {
                StatusBloco.AguardandoCE => "DGVAguardandoCE",
                StatusBloco.ParaRedestinar => "DGVParaRedestinar",
                StatusBloco.Redestinados => "DGVRedestinados",
                StatusBloco.AtracadosSemPresencaCarga => "DGVAtracadosSemPresencaCarga",
                StatusBloco.SituacaoSIGVIG => "DGVSituacaoSIGVIG",
                StatusBloco.AtracadosComPresencaCarga => "DGVAtracadosComPresencaCarga",
                StatusBloco.Deferidos => "DGVDeferidos",
                StatusBloco.SolicitarNumerario => "DGVSolicitarNumerario",
                StatusBloco.DIDUIMPParaDigitacao => "DGVDIDUIMPParaDigitacao",
                _ => "DGVAguardandoCE"
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
        private StatusBloco? _statusBlocoAtual;
        private List<Processo> _processosExibidos = new List<Processo>();
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
        private void MostrarLoading(string mensagem = "Carregando...")
        {
            if (_overlay != null) return;

            _overlay = new FrmLoadingOverlay();
            _overlay.Opacity = 0.60;
            _overlay.lblLoading.Text = mensagem;

            // Faz o overlay cobrir TODO o cliente do formulário
            var rect = this.RectangleToScreen(this.ClientRectangle);
            _overlay.StartPosition = FormStartPosition.Manual;
            _overlay.Location = rect.Location;
            _overlay.Size = rect.Size;

            _overlay.Show(this);
            _overlay.BringToFront();
        }


        private void EsconderLoading()
        {
            if (_overlay == null) return;
            _overlay.Close();
            _overlay.Dispose();
            _overlay = null;
        }
        // Dicionário estático para ordenação rápida (evita Reflection lento)
        private static readonly Dictionary<string, Func<Processo, object>> _propSelectors = new()
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

        private List<dynamic> _dadosExibicaoAtual = new();
        private string? _ultimaColunaOrdenada = null;
        private bool _ultimaDirecaoAscendente = true;
        private List<Processo> _todosProcessos = new();
        private FrmLoadingOverlay? _overlay;

        private readonly RepositorioUsers _repositorioUsers = new();
        private Users? _usuarioLogado;
        private Logado _logado;
        public FrmStatusProcessos(Logado logado)
        {
            InitializeComponent();
            MostrarItens.Visible = false;

            _logado = logado;
            _bindingSource = new BindingSource();
            DGVSelecionado.DoubleBuffered(true);
        }
        private async void FrmStatusProcessos_Load(object? sender, EventArgs e)
        {
            _usuarioLogado = await _repositorioUsers.GetByIdAsync(_logado.Id);
            if (_usuarioLogado == null)
            {
                MessageBox.Show("Não foi possível carregar o usuário logado.", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            GridColumnManager.RegistrarCatalogosPadrao();
            await CarregarProcessosAsync();
        }
        // Só filtra pelo status calculado
        private List<Processo> ObterProcessosPorStatus(StatusBloco status)
        {
            string statusStr = status.ToString();
            return _todosProcessos
                .Where(p => p.CondicaoProcesso == statusStr)
                .ToList();
        }
        private async Task CarregarProcessosAsync()
        {
            try
            {
                MostrarLoading("Carregando processos...");

                var processoService = new RepositorioProcesso();
                var todos = await processoService.ListarProcessosAtivosParaStatusAsync();
                var processosNaoFinalizados = todos
                    .Where(p => !string.Equals(p.Status, "Finalizado", StringComparison.OrdinalIgnoreCase))
                    .ToList();

                _todosProcessos = await Task.Run(() =>
                {
                    processosNaoFinalizados.AsParallel()
                        .ForAll(p => ProcessoHelper.AtualizarCondicaoProcesso(p));
                    return processosNaoFinalizados;
                });

                _bindingSource.DataSource = _todosProcessos;
                _bindingSource.ResetBindings(false);

                AtualizarContadores();
            }
            finally
            {
                EsconderLoading();
            }
        }

        private void MostrarItensPorStatus(StatusBloco status)
        {
            DGVSelecionado.SuspendLayout(); // Pára de desenhar
            try
            {
                _blocoExibidoAtual = BlocoExibido.StatusPadrao;
                _statusBlocoAtual = status;

                var processos = ObterProcessosPorStatus(status);
                _processosExibidos = OrdenarLista(processos);

                var nomeGrid = ObterNomeGridStatus(status);

                // Configuração de colunas
                if (_usuarioLogado.PreferenciasGrids == null)
                    _usuarioLogado.PreferenciasGrids = new Dictionary<string, List<string>>();

                _usuarioLogado.PreferenciasGrids.TryGetValue(nomeGrid, out var colunasVisiveis);
                GridColumnManager.ConfigurarGrid(DGVSelecionado, nomeGrid, colunasVisiveis);

                _bindingSource.DataSource = _processosExibidos;
                _bindingSource.ResetBindings(false);

                // UI Updates
                var info = BlocoInfo[status];
                LblTitulo.Text = $"{info.Nome} ({processos.Count})";
                LblTitulo.ForeColor = info.Cor == Color.Black ? Color.White : Color.Black;
                LblTitulo.BackColor = info.Cor;

                Blocos.Visible = false;
                MostrarItens.Visible = true;
            }
            finally
            {
                DGVSelecionado.ResumeLayout(); // Volta a desenhar
            }
        }


        // OTIMIZAÇÃO DE ORDENAÇÃO
        private List<Processo> OrdenarLista(List<Processo> lista)
        {
            // Se a lista for pequena, não faz diferença, mas para listas grandes, previne alocações
            if (lista == null || lista.Count == 0) return lista;

            // 1. Lógica Padrão (Sem clique ou reset)
            if (string.IsNullOrEmpty(_ultimaColunaOrdenada))
            {
                return lista
                    .OrderBy(p => IsITJ(p.Ref_USA) ? 1 : 0) // ITJ no fundo
                    .ThenBy(p => string.IsNullOrWhiteSpace(p.Ref_USA) ? 1 : 0)
                    .ThenBy(p => ExtrairAnoNumeroSortKey(p.Ref_USA)) // Usa chave numérica direta
                    .ToList();
            }

            // Recupera o seletor rápido do dicionário (Sem Reflection)
            if (!_propSelectors.TryGetValue(_ultimaColunaOrdenada, out var selector))
            {
                // Fallback caso a coluna não esteja mapeada (segurança)
                selector = p => p.GetType().GetProperty(_ultimaColunaOrdenada)?.GetValue(p);
            }

            // --- LÓGICA REF_USA (Mantida a regra de negócio do ITJ) ---
            if (_ultimaColunaOrdenada == "Ref_USA")
            {
                var queryBase = lista
                    .OrderBy(p => IsITJ(p.Ref_USA) ? 1 : 0)
                    .ThenBy(p => string.IsNullOrWhiteSpace(p.Ref_USA) ? 1 : 0);

                return _ultimaDirecaoAscendente
                    ? queryBase.ThenBy(p => ExtrairAnoNumeroSortKey(p.Ref_USA)).ToList()
                    : queryBase.ThenByDescending(p => ExtrairAnoNumeroSortKey(p.Ref_USA)).ToList();
            }

            // --- ORDENAÇÃO GENÉRICA OTIMIZADA ---
            return _ultimaDirecaoAscendente
                ? lista.OrderBy(p => selector(p) == null ? 1 : 0).ThenBy(selector).ToList()
                : lista.OrderBy(p => selector(p) == null ? 1 : 0).ThenByDescending(selector).ToList();
        }

        // Helper rápido para verificar ITJ
        private bool IsITJ(string refUsa) =>
            refUsa != null && refUsa.TrimEnd().EndsWith("ITJ", StringComparison.OrdinalIgnoreCase);

        // Otimização: Retorna um long (ex: 202400123) para ordenação numérica rápida sem criar tuplas ou structs
        private long ExtrairAnoNumeroSortKey(string refUsa)
        {
            if (string.IsNullOrWhiteSpace(refUsa)) return 0;

            // Assume formato "NUMERO/ANO" ex: "123/25" ou "123/2025"
            // Evita Split se possível para performance extrema, mas Split é aceitável aqui.
            var partes = refUsa.Split('/', ' ');
            if (partes.Length >= 2)
            {
                if (int.TryParse(partes[0], out int numero) && int.TryParse(partes[1], out int ano))
                {
                    // Normaliza ano (ex: 25 vira 2025) para garantir ordenação correta
                    int anoCompleto = ano < 100 ? 2000 + ano : ano;
                    return (long)anoCompleto * 1000000 + numero;
                }
            }
            return 0;
        }
        private void DGVSelecionado_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            var coluna = DGVSelecionado.Columns[e.ColumnIndex];
            var propriedade = coluna.DataPropertyName;

            // Validação básica
            if (string.IsNullOrWhiteSpace(propriedade) || _processosExibidos.Count == 0) return;

            // 1. Define a Direção (Alterna Ascendente/Descendente)
            if (_ultimaColunaOrdenada == propriedade)
            {
                _ultimaDirecaoAscendente = !_ultimaDirecaoAscendente;
            }
            else
            {
                _ultimaColunaOrdenada = propriedade;
                _ultimaDirecaoAscendente = true; // Nova coluna começa Ascendente
            }

            // 2. Chama o método central que criamos no passo anterior
            // Ele vai pegar a lista atual, ordenar baseada nas variáveis acima e retornar a lista pronta.
            _processosExibidos = OrdenarLista(_processosExibidos);

            // 3. Atualiza a Grade (Ligação Direta)
            DGVSelecionado.DataSource = null; // Reset para garantir refresh visual
            DGVSelecionado.DataSource = _processosExibidos;

            // 4. Atualiza as Setinhas (Glyphs) no cabeçalho
            foreach (DataGridViewColumn col in DGVSelecionado.Columns)
            {
                if (col.Name == coluna.Name)
                {
                    col.HeaderCell.SortGlyphDirection = _ultimaDirecaoAscendente
                        ? SortOrder.Ascending
                        : SortOrder.Descending;
                }
                else
                {
                    col.HeaderCell.SortGlyphDirection = SortOrder.None;
                }
            }
        }
        private (int ano, int numero) ExtrairAnoNumero(string refUsa)
        {
            if (string.IsNullOrWhiteSpace(refUsa)) return (0, 0);
            string refLimpa = refUsa.Split(' ').FirstOrDefault() ?? refUsa;
            var partes = refLimpa.Split('/');
            int numero = 0, ano = 0;
            if (partes.Length == 2)
            {
                int.TryParse(partes[0], out numero);
                int.TryParse(partes[1], out ano);
            }
            return (ano, numero);
        }
        private void AtualizarContadores()
        {
            // Para os blocos "normais"
            foreach (StatusBloco status in Enum.GetValues(typeof(StatusBloco)))
            {
                var count = ObterProcessosPorStatus(status).Count;
                var label = ObterLabelPorStatus(status);
                var textoBase = BlocoInfo[status].Nome;
                if (label != null)
                    label.Text = count > 0 ? $"{textoBase}\n({count})" : textoBase;
            }

            // Bloco especial "Solicitar Numerário"
            var countSolicitarNumerario = ObterProcessosSolicitarNumerario().Count;
            if (BtnSolicitarNumerario != null)
                BtnSolicitarNumerario.Text = $"Solicitar Numerário\n({countSolicitarNumerario})";

            // Bloco especial "DI/DUIMP para Digitação"
            var countDIDuimp = ObterProcessosDIDuimpParaDigitacao().Count;
            if (BtnDIDUIMPParaDigitacao != null)
                BtnDIDUIMPParaDigitacao.Text = $"DI/DUIMP para Digitação\n({countDIDuimp})";
        }


        // Métodos de vinculação de UI
        private Label ObterLabelPorStatus(StatusBloco status)
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
                _ => throw new ArgumentException($"Status inválido: {status}", nameof(status))
            };
        }
        private List<Processo> ObterProcessosSolicitarNumerario()
        {
            // Aqui você define exatamente as regras de entrada/saída
            return _todosProcessos
                .Where(p =>
                    p.DataDeAtracacao.HasValue &&
                    !p.Numerario
                ).ToList();
        }

        private List<Processo> ObterProcessosDIDuimpParaDigitacao()
        {
            return _todosProcessos
                .Where(p =>
                    p.DataDeAtracacao.HasValue &&
                    string.IsNullOrWhiteSpace(p.RascunhoDI)
                ).ToList();
        }
        // Event handlers por status
        private void BtnAguardandoCE_Click(object sender, EventArgs e) =>
            MostrarItensPorStatus(StatusBloco.AguardandoCE);
        private void BtnParaRedestinar_Click(object sender, EventArgs e) =>
            MostrarItensPorStatus(StatusBloco.ParaRedestinar);
        private void BtnRedestinados_Click(object sender, EventArgs e) =>
            MostrarItensPorStatus(StatusBloco.Redestinados);
        private void BtnAtracadosSPresencaDeCarga_Click(object sender, EventArgs e) =>
            MostrarItensPorStatus(StatusBloco.AtracadosSemPresencaCarga);
        private void BtnSituacaoSIGVIG_Click(object sender, EventArgs e) =>
            MostrarItensPorStatus(StatusBloco.SituacaoSIGVIG);
        private void BtnAtracadosCPresencaDeCarga_Click(object sender, EventArgs e) =>
            MostrarItensPorStatus(StatusBloco.AtracadosComPresencaCarga);
        private void BtnDeferidos_Click(object sender, EventArgs e) =>
            MostrarItensPorStatus(StatusBloco.Deferidos);
        private void BtnSolicitarNumerario_Click(object sender, EventArgs e)
        {
            _blocoExibidoAtual = BlocoExibido.SolicitarNumerario;
            var processos = ObterProcessosDIDuimpParaDigitacao();

            var processosOrdenados = processos
                .OrderBy(p => (p.Ref_USA?.Trim().EndsWith("ITJ", StringComparison.OrdinalIgnoreCase) ?? false) ? 1 : 0)
                .ThenBy(p => string.IsNullOrWhiteSpace(p.Ref_USA) ? 1 : 0)
                .ThenBy(p => ExtrairAnoNumero(p.Ref_USA))
                .ToList();

            _processosExibidos = processosOrdenados;

            var nomeGrid = ObterNomeGridStatus(StatusBloco.SolicitarNumerario);
            _usuarioLogado.PreferenciasGrids ??= new Dictionary<string, List<string>>();
            _usuarioLogado.PreferenciasGrids.TryGetValue(nomeGrid, out var colunasVisiveis);
            GridColumnManager.ConfigurarGrid(DGVSelecionado, nomeGrid, colunasVisiveis);

            _bindingSource.DataSource = _processosExibidos;
            _bindingSource.ResetBindings(false);

            var blocoInfo = BlocoInfo[StatusBloco.SolicitarNumerario];
            LblTitulo.Text = $"{blocoInfo.Nome} ({processos.Count})";
            LblTitulo.ForeColor = blocoInfo.Cor == Color.Black ? Color.White : Color.Black;
            LblTitulo.BackColor = blocoInfo.Cor;

            Blocos.Visible = false;
            MostrarItens.Visible = true;
        }
        private void BtnDIDUIMPParaDigitacao_Click(object sender, EventArgs e)
        {
            _blocoExibidoAtual = BlocoExibido.DIDUIMPParaDigitacao;
            var processos = ObterProcessosDIDuimpParaDigitacao();

            var processosOrdenados = processos
                .OrderBy(p => (p.Ref_USA?.Trim().EndsWith("ITJ", StringComparison.OrdinalIgnoreCase) ?? false) ? 1 : 0)
                .ThenBy(p => string.IsNullOrWhiteSpace(p.Ref_USA) ? 1 : 0)
                .ThenBy(p => ExtrairAnoNumero(p.Ref_USA))
                .ToList();

            _processosExibidos = processosOrdenados;

            var nomeGrid = ObterNomeGridStatus(StatusBloco.DIDUIMPParaDigitacao);
            _usuarioLogado.PreferenciasGrids ??= new Dictionary<string, List<string>>();
            _usuarioLogado.PreferenciasGrids.TryGetValue(nomeGrid, out var colunasVisiveis);
            GridColumnManager.ConfigurarGrid(DGVSelecionado, nomeGrid, colunasVisiveis);

            _bindingSource.DataSource = _processosExibidos;
            _bindingSource.ResetBindings(false);

            var blocoInfo = BlocoInfo[StatusBloco.DIDUIMPParaDigitacao];
            LblTitulo.Text = $"{blocoInfo.Nome} ({processos.Count})";
            LblTitulo.ForeColor = blocoInfo.Cor == Color.Black ? Color.White : Color.Black;
            LblTitulo.BackColor = blocoInfo.Cor;

            Blocos.Visible = false;
            MostrarItens.Visible = true;
        }
        private async void DGVSelecionado_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= _processosExibidos.Count) return;

            var processoSelecionado = _processosExibidos[e.RowIndex];

            // 1. Guarda o ID do processo que estamos editando
            string idSelecionado = processoSelecionado.Id.ToString();

            using var frm = new FrmModificaProcesso { processo = processoSelecionado, Modo = "Editar" };
            frm.ShowDialog();

            await CarregarProcessosAsync();

            // Recarrega a lista (que agora vai respeitar a ordenação graças ao passo 1 e 2)
            switch (_blocoExibidoAtual)
            {
                case BlocoExibido.SolicitarNumerario:
                    BtnSolicitarNumerario_Click(null, EventArgs.Empty);
                    break;
                case BlocoExibido.DIDUIMPParaDigitacao:
                    BtnDIDUIMPParaDigitacao_Click(null, EventArgs.Empty);
                    break;
                case BlocoExibido.StatusPadrao:
                    if (_statusBlocoAtual.HasValue)
                        MostrarItensPorStatus(_statusBlocoAtual.Value);
                    break;
            }

            // 2. Restaura a seleção e o scroll para o item que foi editado
            RestaurarSelecao(idSelecionado);
        }

        private void RestaurarSelecao(string idProcesso)
        {
            if (string.IsNullOrEmpty(idProcesso)) return;

            // Procura na lista atual onde está o processo com esse ID
            var item = _processosExibidos.FirstOrDefault(p => p.Id.ToString() == idProcesso);

            if (item != null)
            {
                int index = _processosExibidos.IndexOf(item);
                if (index >= 0 && index < DGVSelecionado.Rows.Count)
                {
                    DGVSelecionado.ClearSelection();
                    DGVSelecionado.Rows[index].Selected = true;

                    // Rola a tela até o item
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
        PropertyInfo pi = dgvType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
        pi?.SetValue(dgv, setting, null);
    }
}
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using CLUSA; // Models e ProcessoHelper aqui

namespace Trabalho
{
    public partial class FrmStatusProcessos : Form
    {
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

        // Otimizado: apenas info visual dos blocos
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

        private List<Processo> _todosProcessos = new();

        public FrmStatusProcessos()
        {
            InitializeComponent();
            MostrarItens.Visible = false;
        }
        private async void FrmStatusProcessos_Load(object? sender, EventArgs e)
        {
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

        // Chame sempre que abrir/trocar/cadastrar/editar processos
        private async Task CarregarProcessosAsync()
        {
            var processoService = new RepositorioProcesso();
            var todos = await processoService.ListarTodosAsync();
            _todosProcessos = todos.Where(p => !string.Equals(p.Status, "Finalizado", StringComparison.OrdinalIgnoreCase)).ToList();
            foreach (var p in _todosProcessos)
                ProcessoHelper.AtualizarCondicaoProcesso(p);
            AtualizarContadores();
        }


        private void MostrarItensPorStatus(StatusBloco status)
        {
            _blocoExibidoAtual = BlocoExibido.StatusPadrao;
            _statusBlocoAtual = status;

            var processos = ObterProcessosPorStatus(status);
            _processosExibidos = processos;
            var blocoInfo = BlocoInfo[status];

            LblTitulo.Text = $"{blocoInfo.Nome} ({processos.Count})";
            LblTitulo.ForeColor = blocoInfo.Cor == Color.Black ? Color.White : Color.Black;
            LblTitulo.BackColor = blocoInfo.Cor;

            var dadosExibicao = processos.Select(p => new
            {
                p.Ref_USA,
                p.SR,
                p.Importador,
                p.Exportador,
                p.Produto,
                p.Container,
                p.PortoDestino,
                p.Veiculo,
                p.DataDeAtracacao, // Use DateTime, o DataGridView formata pelo DefaultCellStyle!
                p.OrgaosAnuentesString,
                p.FreeTime,
                p.VencimentoFreeTime,
                p.VencimentoFMA,
                p.DI,
                LPCOs = ProcessoHelper.ObterResumoLPCOs(p),
                p.HistoricoDoProcesso,
                p.Pendencia,
                p.Status
            }).ToList();


            DGVSelecionado.DataSource = dadosExibicao;
            ConfigurarDataGridView();

            Blocos.Visible = false;
            MostrarItens.Visible = true;
        }

        // UI helpers
        private void ConfigurarDataGridView()
        {
            DGVSelecionado.Columns.Clear();
            DGVSelecionado.AutoGenerateColumns = false;
            DGVSelecionado.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            DGVSelecionado.RowHeadersVisible = false;
            DGVSelecionado.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            DGVSelecionado.DefaultCellStyle.WrapMode = DataGridViewTriState.False;
            DGVSelecionado.ShowCellToolTips = true;
            DGVSelecionado.BackgroundColor = Color.White;
            DGVSelecionado.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);

            var dateCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" };

            // Adiciona as colunas com largura e header definidos
            DGVSelecionado.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Ref_USA",
                HeaderText = "Ref. USA",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                MinimumWidth = 90
            });
            DGVSelecionado.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "SR",
                HeaderText = "S. Ref",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                MinimumWidth = 80
            });
            DGVSelecionado.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Importador",
                HeaderText = "Importador",
                FillWeight = 140,
                MinimumWidth = 140
            });
            DGVSelecionado.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Exportador",
                HeaderText = "Exportador",
                FillWeight = 140,
                MinimumWidth = 140
            });
            DGVSelecionado.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Produto",
                HeaderText = "Produto",
                FillWeight = 180,
                MinimumWidth = 200
            });
            DGVSelecionado.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Container",
                HeaderText = "Container",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader,
                MinimumWidth = 120
            });
            DGVSelecionado.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "PortoDestino",
                HeaderText = "Porto Destino",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                MinimumWidth = 110
            });
            DGVSelecionado.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Veiculo",
                HeaderText = "Veículo",
                FillWeight = 110,
                MinimumWidth = 120
            });
            DGVSelecionado.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "DataDeAtracacao",
                HeaderText = "Data de Atracação",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                MinimumWidth = 70,
                DefaultCellStyle = dateCellStyle
            });
            DGVSelecionado.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "LPCOs",
                HeaderText = "LPCOs (Deferidos/Total)",
                MinimumWidth = 100
            });
            DGVSelecionado.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "OrgaosAnuentesString",
                HeaderText = "Anuente",
                FillWeight = 90,
                MinimumWidth = 100
            });
            DGVSelecionado.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "FreeTime",
                HeaderText = "F.T (dias)",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                MinimumWidth = 70
            });
            DGVSelecionado.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "VencimentoFreeTime",
                HeaderText = "Venc. F. Time",
                DefaultCellStyle = dateCellStyle,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                MinimumWidth = 100
            });
            DGVSelecionado.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "VencimentoFMA",
                HeaderText = "Venc. FMA",
                DefaultCellStyle = dateCellStyle,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                MinimumWidth = 100
            });
            DGVSelecionado.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "DI",
                HeaderText = "DI",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                MinimumWidth = 120
            });
            DGVSelecionado.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "HistoricoDoProcesso",
                HeaderText = "Histórico",
                FillWeight = 200,
                MinimumWidth = 200
            });
            DGVSelecionado.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Pendencia",
                HeaderText = "Pendência",
                FillWeight = 160,
                MinimumWidth = 160
            });
            DGVSelecionado.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Status",
                HeaderText = "Status",
                FillWeight = 110,
                MinimumWidth = 120
            });

            // Estilo dos headers (mantém do seu antigo ConfigurarDataGridView)
            var headerStyle = new DataGridViewCellStyle
            {
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                BackColor = Color.FromArgb(230, 230, 230),
                ForeColor = Color.Black
            };

            foreach (DataGridViewColumn col in DGVSelecionado.Columns)
            {
                col.HeaderCell.Style = headerStyle;
                col.DefaultCellStyle.Font = new Font("Segoe UI", 10);
                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            }
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
            var processos = ObterProcessosSolicitarNumerario();
            _processosExibidos = processos;

            var blocoInfo = BlocoInfo[StatusBloco.SolicitarNumerario];
            LblTitulo.Text = $"{blocoInfo.Nome} ({processos.Count})";
            LblTitulo.ForeColor = blocoInfo.Cor == Color.Black ? Color.White : Color.Black;
            LblTitulo.BackColor = blocoInfo.Cor;

            var dadosExibicao = processos.Select(p => new
            {
                p.Ref_USA,
                p.SR,
                p.Importador,
                p.Exportador,
                p.Produto,
                p.Container,
                p.PortoDestino,
                p.Veiculo,
                p.DataDeAtracacao, // Use DateTime, o DataGridView formata pelo DefaultCellStyle!
                p.OrgaosAnuentesString,
                p.FreeTime,
                p.VencimentoFreeTime,
                p.VencimentoFMA,
                p.DI,
                LPCOs = ProcessoHelper.ObterResumoLPCOs(p),
                p.HistoricoDoProcesso,
                p.Pendencia,
                p.Status
            }).ToList();

            DGVSelecionado.DataSource = dadosExibicao;
            ConfigurarDataGridView();

            Blocos.Visible = false;
            MostrarItens.Visible = true;
        }
        private void BtnDIDUIMPParaDigitacao_Click(object sender, EventArgs e)
        {
            _blocoExibidoAtual = BlocoExibido.DIDUIMPParaDigitacao;
            var processos = ObterProcessosDIDuimpParaDigitacao();
            _processosExibidos = processos;

            var blocoInfo = BlocoInfo[StatusBloco.DIDUIMPParaDigitacao];
            LblTitulo.Text = $"{blocoInfo.Nome} ({processos.Count})";
            LblTitulo.ForeColor = blocoInfo.Cor == Color.Black ? Color.White : Color.Black;
            LblTitulo.BackColor = blocoInfo.Cor;

            var dadosExibicao = processos.Select(p => new
            {
                p.Ref_USA,
                p.SR,
                p.Importador,
                p.Exportador,
                p.Produto,
                p.Container,
                p.PortoDestino,
                p.Veiculo,
                p.DataDeAtracacao, // Use DateTime, o DataGridView formata pelo DefaultCellStyle!
                p.OrgaosAnuentesString,
                p.FreeTime,
                p.VencimentoFreeTime,
                p.VencimentoFMA,
                p.DI,
                LPCOs = ProcessoHelper.ObterResumoLPCOs(p),
                p.HistoricoDoProcesso,
                p.Pendencia,
                p.Status
            }).ToList();

            DGVSelecionado.DataSource = dadosExibicao;
            ConfigurarDataGridView();

            Blocos.Visible = false;
            MostrarItens.Visible = true;
        }
        private async void DGVSelecionado_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= _processosExibidos.Count) return;

            var processoSelecionado = _processosExibidos[e.RowIndex];

            using var frm = new FrmModificaProcesso { processo = processoSelecionado, Modo = "Editar" };
            frm.ShowDialog();

            // Se quiser que a atualização já seja async:
            await CarregarProcessosAsync();

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
                default:
                    break;
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

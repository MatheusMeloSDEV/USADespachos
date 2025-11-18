using CLUSA; // Models e ProcessoHelper aqui
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

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

        private List<dynamic> _dadosExibicaoAtual = new();
        private string? _ultimaColunaOrdenada = null;
        private bool _ultimaDirecaoAscendente = true;
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

            var dadosExibicao = processos
                .OrderBy(p => (p.Ref_USA?.Trim().EndsWith("ITJ", StringComparison.OrdinalIgnoreCase) ?? false) ? 1 : 0) // ITJ no final
                .ThenBy(p => string.IsNullOrWhiteSpace(p.Ref_USA) ? 1 : 0) // Ref_USA vazio mais abaixo
                .ThenBy(p => ExtrairAnoNumero(p.Ref_USA)) // Ordena normalmente
                .Select(p => new {
                    p.Ref_USA,
                    p.SR,
                    p.Importador,
                    p.Veiculo,
                    p.DataDeAtracacao,
                    p.Terminal,
                    p.LocalDeDesembaraco,
                    p.Container,
                    p.Redestinacao,
                    p.CE,
                    p.FreeTime,
                    p.VencimentoFreeTime,
                    p.VencimentoFMA,
                    p.CapaOK,
                    p.Numerario,
                    p.RascunhoDI,
                    p.Pendencia,
                    p.Status,
                })
                .ToList();


            _dadosExibicaoAtual = dadosExibicao.Cast<dynamic>().ToList();

            DGVSelecionado.DataSource = _dadosExibicaoAtual;
            ConfigurarColunasDataGridViewProcesso();

            Blocos.Visible = false;
            MostrarItens.Visible = true;
        }

        // UI helpers
        private void ConfigurarColunasDataGridViewProcesso()
        {
            DGVSelecionado.Columns.Clear();

            // --- Configuração Geral da Grade ---
            DGVSelecionado.AutoGenerateColumns = false;
            DGVSelecionado.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // Mantém o preenchimento
            DGVSelecionado.RowHeadersVisible = false;
            DGVSelecionado.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            DGVSelecionado.DefaultCellStyle.WrapMode = DataGridViewTriState.False;
            DGVSelecionado.ShowCellToolTips = true;
            var dateCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" };

            // --- Adicionando as Colunas com Largura Mínima ---

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
                MinimumWidth = 60
            });
            DGVSelecionado.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Importador",
                HeaderText = "Importador",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader,
                MinimumWidth = 80 // <-- MUDANÇA
            });
            DGVSelecionado.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Veiculo",
                HeaderText = "Veículo",
                FillWeight = 140,
                MinimumWidth = 80 // <-- MUDANÇA
            });
            DGVSelecionado.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "DataDeAtracacao",
                HeaderText = "Data de Atracação",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                MinimumWidth = 70 // <-- MUDANÇA
            });
            DGVSelecionado.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Terminal",
                HeaderText = "Terminal",
                FillWeight = 140,
                MinimumWidth = 140
            });
            DGVSelecionado.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "LocalDeDesembaraco",
                HeaderText = "Local",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                MinimumWidth = 120 // <-- MUDANÇA
            });
            DGVSelecionado.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Container",
                HeaderText = "Container",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader,
                MinimumWidth = 120
            });
            DGVSelecionado.Columns.Add(new DataGridViewCheckBoxColumn
            {
                DataPropertyName = "Redestinacao",
                HeaderText = "Redes.",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader,
                MinimumWidth = 50
            });
            DGVSelecionado.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "CE",
                HeaderText = "CE",
                FillWeight = 90,
                MinimumWidth = 100 // <-- MUDANÇA
            });
            DGVSelecionado.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "FreeTime",
                HeaderText = "F.T",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                MinimumWidth = 40 // <-- MUDANÇA
            });
            DGVSelecionado.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "VencimentoFreeTime",
                HeaderText = "Venc. F. Time",
                DefaultCellStyle = dateCellStyle,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                MinimumWidth = 100 // <-- MUDANÇA
            });
            DGVSelecionado.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "VencimentoFMA",
                HeaderText = "Venc. FMA",
                DefaultCellStyle = dateCellStyle,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                MinimumWidth = 100 // <-- MUDANÇA
            });
            DGVSelecionado.Columns.Add(new DataGridViewCheckBoxColumn
            {
                DataPropertyName = "CapaOK",
                HeaderText = "Capa",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader,
                MinimumWidth = 40
            });
            DGVSelecionado.Columns.Add(new DataGridViewCheckBoxColumn
            {
                DataPropertyName = "Numerario",
                HeaderText = "Num.",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader,
                MinimumWidth = 40
            });
            DGVSelecionado.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "RascunhoDI",
                HeaderText = "Rascunho DI",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                MinimumWidth = 120
            });
            DGVSelecionado.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Pendencia",
                HeaderText = "Pendência",
                FillWeight = 160,
                MinimumWidth = 160 // <-- MUDANÇA
            });
            DGVSelecionado.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Status",
                HeaderText = "Status",
                FillWeight = 180,
                MinimumWidth = 120 // <-- MUDANÇA
            });

            foreach (DataGridViewColumn coluna in DGVSelecionado.Columns)
            {
                coluna.DefaultCellStyle.Font = new Font("Segoe UI", 10);
                coluna.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                coluna.SortMode = DataGridViewColumnSortMode.Programmatic;

                // Centralizar checkbox
                if (coluna is DataGridViewCheckBoxColumn || coluna.HeaderText == "F.T")
                {
                    coluna.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
            }
        }
        private void DGVSelecionado_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            var coluna = DGVSelecionado.Columns[e.ColumnIndex];
            var propriedade = coluna.DataPropertyName;
            if (string.IsNullOrWhiteSpace(propriedade) || _dadosExibicaoAtual.Count == 0) return;

            // Alternância asc/desc
            bool ascendente = true;
            if (_ultimaColunaOrdenada == propriedade)
                ascendente = !_ultimaDirecaoAscendente;
            _ultimaColunaOrdenada = propriedade;
            _ultimaDirecaoAscendente = ascendente;

            // Busca o tipo da propriedade usando reflection na primeira linha
            var primeiroItem = _dadosExibicaoAtual[0];
            var propInfo = primeiroItem.GetType().GetProperty(propriedade);
            if (propInfo == null) return;

            List<dynamic> novaLista;

            // ---- Lógica especial para Ref_USA ----
            if (propriedade == "Ref_USA")
            {
                Func<dynamic, bool> itjFinalizado = d => ((d.Ref_USA as string)?.Trim().EndsWith("ITJ", StringComparison.OrdinalIgnoreCase) ?? false);
                Func<dynamic, bool> refUsaVazio = d => string.IsNullOrWhiteSpace((string?)d.Ref_USA);

                var ordered = _dadosExibicaoAtual
                    .OrderBy(d => itjFinalizado(d) ? 1 : 0) // ITJ fica no final
                    .ThenBy(d => refUsaVazio(d) ? 1 : 0);   // depois Ref_USA vazios

                if (ascendente)
                    novaLista = ordered.ThenBy(d => ExtrairAnoNumero((string?)d.Ref_USA)).ToList();
                else
                    novaLista = ordered.ThenByDescending(d => ExtrairAnoNumero((string?)d.Ref_USA)).ToList();
            }


            else if (propInfo.PropertyType == typeof(DateTime) || propInfo.PropertyType == typeof(DateTime?))
            {
                if (ascendente)
                {
                    novaLista = _dadosExibicaoAtual
                        .OrderBy(d => propInfo.GetValue(d) == null ? 1 : 0)
                        .ThenBy(d => (DateTime?)propInfo.GetValue(d) ?? DateTime.MinValue)
                        .ToList();
                }
                else
                {
                    novaLista = _dadosExibicaoAtual
                        .OrderBy(d => propInfo.GetValue(d) == null ? 1 : 0)
                        .ThenByDescending(d => (DateTime?)propInfo.GetValue(d) ?? DateTime.MinValue)
                        .ToList();
                }
            }
            else if (propInfo.PropertyType == typeof(string))
            {
                if (ascendente)
                {
                    novaLista = _dadosExibicaoAtual
                        .OrderBy(d => string.IsNullOrWhiteSpace((string?)propInfo.GetValue(d)) ? 1 : 0)
                        .ThenBy(d => (string?)propInfo.GetValue(d) ?? "")
                        .ToList();
                }
                else
                {
                    novaLista = _dadosExibicaoAtual
                        .OrderBy(d => string.IsNullOrWhiteSpace((string?)propInfo.GetValue(d)) ? 1 : 0)
                        .ThenByDescending(d => (string?)propInfo.GetValue(d) ?? "")
                        .ToList();
                }
            }
            else if (propInfo.PropertyType == typeof(bool) || propInfo.PropertyType == typeof(bool?))
            {
                if (ascendente)
                {
                    novaLista = _dadosExibicaoAtual.OrderBy(d => (bool?)propInfo.GetValue(d) ?? false).ToList();
                }
                else
                {
                    novaLista = _dadosExibicaoAtual.OrderByDescending(d => (bool?)propInfo.GetValue(d) ?? false).ToList();
                }
            }
            else if (Nullable.GetUnderlyingType(propInfo.PropertyType) != null)
            {
                if (ascendente)
                {
                    novaLista = _dadosExibicaoAtual
                        .OrderBy(d => propInfo.GetValue(d) == null ? 1 : 0)
                        .ThenBy(d => Convert.ToDecimal(propInfo.GetValue(d) ?? 0))
                        .ToList();
                }
                else
                {
                    novaLista = _dadosExibicaoAtual
                        .OrderBy(d => propInfo.GetValue(d) == null ? 1 : 0)
                        .ThenByDescending(d => Convert.ToDecimal(propInfo.GetValue(d) ?? 0))
                        .ToList();
                }
            }
            else
            {
                if (ascendente)
                {
                    novaLista = _dadosExibicaoAtual.OrderBy(d => propInfo.GetValue(d) ?? 0).ToList();
                }
                else
                {
                    novaLista = _dadosExibicaoAtual.OrderByDescending(d => propInfo.GetValue(d) ?? 0).ToList();
                }
            }

            // Atualiza o grid
            DGVSelecionado.DataSource = null;
            DGVSelecionado.DataSource = novaLista;
            _dadosExibicaoAtual = novaLista;

            foreach (DataGridViewColumn col in DGVSelecionado.Columns)
            {
                col.HeaderCell.SortGlyphDirection = (col.Name == coluna.Name)
                    ? (ascendente ? SortOrder.Ascending : SortOrder.Descending)
                    : SortOrder.None;
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
            var processos = ObterProcessosSolicitarNumerario();
            _processosExibidos = processos;

            var blocoInfo = BlocoInfo[StatusBloco.SolicitarNumerario];
            LblTitulo.Text = $"{blocoInfo.Nome} ({processos.Count})";
            LblTitulo.ForeColor = blocoInfo.Cor == Color.Black ? Color.White : Color.Black;
            LblTitulo.BackColor = blocoInfo.Cor;

            var dadosExibicao = processos
                .OrderBy(p => (p.Ref_USA?.Trim().EndsWith("ITJ", StringComparison.OrdinalIgnoreCase) ?? false) ? 1 : 0) // ITJ no final
                .ThenBy(p => string.IsNullOrWhiteSpace(p.Ref_USA) ? 1 : 0) // Ref_USA vazio mais abaixo
                .ThenBy(p => ExtrairAnoNumero(p.Ref_USA)) // Ordena normalmente
                .Select(p => new {
                    p.Ref_USA,
                    p.SR,
                    p.Importador,
                    p.Veiculo,
                    p.DataDeAtracacao,
                    p.Terminal,
                    p.LocalDeDesembaraco,
                    p.Container,
                    p.Redestinacao,
                    p.CE,
                    p.FreeTime,
                    p.VencimentoFreeTime,
                    p.VencimentoFMA,
                    p.CapaOK,
                    p.Numerario,
                    p.RascunhoDI,
                    p.Pendencia,
                    p.Status,
                })
                .ToList();


            DGVSelecionado.DataSource = dadosExibicao;
            ConfigurarColunasDataGridViewProcesso();
            
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

            var dadosExibicao = processos
                .OrderBy(p => (p.Ref_USA?.Trim().EndsWith("ITJ", StringComparison.OrdinalIgnoreCase) ?? false) ? 1 : 0) // ITJ no final
                .ThenBy(p => string.IsNullOrWhiteSpace(p.Ref_USA) ? 1 : 0) // Ref_USA vazio mais abaixo
                .ThenBy(p => ExtrairAnoNumero(p.Ref_USA)) // Ordena normalmente
                .Select(p => new {
                    p.Ref_USA,
                    p.SR,
                    p.Importador,
                    p.Veiculo,
                    p.DataDeAtracacao,
                    p.Terminal,
                    p.LocalDeDesembaraco,
                    p.Container,
                    p.Redestinacao,
                    p.CE,
                    p.FreeTime,
                    p.VencimentoFreeTime,
                    p.VencimentoFMA,
                    p.CapaOK,
                    p.Numerario,
                    p.RascunhoDI,
                    p.Pendencia,
                    p.Status,
                })
                .ToList();


            DGVSelecionado.DataSource = dadosExibicao;
            ConfigurarColunasDataGridViewProcesso();

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

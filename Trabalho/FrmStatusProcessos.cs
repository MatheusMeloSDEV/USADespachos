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
            _bindingSource = new BindingSource();
            DGVSelecionado.DataSource = _bindingSource;
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
            Cursor.Current = Cursors.WaitCursor;

            var processoService = new RepositorioProcesso();
            var todos = await processoService.ListarProcessosAtivosParaStatusAsync();
            var processosNaoFinalizados = todos
                .Where(p => !string.Equals(p.Status, "Finalizado", StringComparison.OrdinalIgnoreCase))
                .ToList();

            _todosProcessos = await Task.Run(() =>
            {
                processosNaoFinalizados.AsParallel().ForAll(p => ProcessoHelper.AtualizarCondicaoProcesso(p));
                return processosNaoFinalizados;
            }); 

            _bindingSource.DataSource = _todosProcessos;
            _bindingSource.DataMember = null;

            AtualizarContadores();
            Cursor.Current = Cursors.Default;
        }


        private void MostrarItensPorStatus(StatusBloco status)
        {
            _blocoExibidoAtual = BlocoExibido.StatusPadrao;
            _statusBlocoAtual = status;

            var processos = ObterProcessosPorStatus(status);

            var processosOrdenados = OrdenarLista(processos);

            _processosExibidos = processosOrdenados;
            _dadosExibicaoAtual = processosOrdenados.Cast<dynamic>().ToList();

            var blocoInfo = BlocoInfo[status];
            LblTitulo.Text = $"{blocoInfo.Nome} ({processos.Count})";
            LblTitulo.ForeColor = blocoInfo.Cor == Color.Black ? Color.White : Color.Black;
            LblTitulo.BackColor = blocoInfo.Cor;
            DGVSelecionado.DataSource = _processosExibidos; 

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

                // Deixe todas como ordenáveis manualmente
                coluna.SortMode = DataGridViewColumnSortMode.Programmatic;

                // Centralizar checkbox e "F.T"
                if (coluna is DataGridViewCheckBoxColumn || coluna.HeaderText == "F.T")
                {
                    coluna.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
            }
        }
        private List<Processo> OrdenarLista(List<Processo> lista)
        {
            // 1. Se o usuário NÃO clicou em nenhuma coluna ainda, usa a ordenação PADRÃO (Ref_USA)
            if (string.IsNullOrEmpty(_ultimaColunaOrdenada))
            {
                return lista
                    .OrderBy(p => (p.Ref_USA?.Trim().EndsWith("ITJ", StringComparison.OrdinalIgnoreCase) ?? false) ? 1 : 0)
                    .ThenBy(p => string.IsNullOrWhiteSpace(p.Ref_USA) ? 1 : 0)
                    .ThenBy(p => ExtrairAnoNumero(p.Ref_USA))
                    .ToList();
            }

            var propInfo = typeof(Processo).GetProperty(_ultimaColunaOrdenada);
            if (propInfo == null) return lista;

            // --- LÓGICA ESPECIAL PARA REF_USA (ITJ FIXO NO FUNDO) ---
            if (_ultimaColunaOrdenada == "Ref_USA")
            {
                Func<Processo, bool> ehITJ = p => (p.Ref_USA?.Trim().EndsWith("ITJ", StringComparison.OrdinalIgnoreCase) ?? false);
                Func<Processo, bool> ehVazio = p => string.IsNullOrWhiteSpace(p.Ref_USA);

                // 1. Cria a "Base" da ordenação.
                // Ao usar OrderBy fixo aqui, criamos uma regra que NÃO muda com o clique do usuário.
                // ITJ ganha peso 1 (fundo), Normais ganham peso 0 (topo).
                var queryBase = lista
                    .OrderBy(p => ehITJ(p) ? 1 : 0)    // REGRA DE OURO: ITJ sempre vai para o grupo de baixo
                    .ThenBy(p => ehVazio(p) ? 1 : 0);  // Vazios ficam abaixo dos normais, mas acima ou junto com ITJ dependendo da lógica

                // 2. Aplica a ordenação do usuário (Asc/Desc) APENAS no conteúdo (Ano/Número)
                if (_ultimaDirecaoAscendente)
                {
                    return queryBase
                        .ThenBy(p => ExtrairAnoNumero(p.Ref_USA))
                        .ToList();
                }
                else
                {
                    return queryBase
                        .ThenByDescending(p => ExtrairAnoNumero(p.Ref_USA))
                        .ToList();
                }
            }

            // Função auxiliar para tratar nulos na ordenação
            bool IsEmpty(object? v) => v == null || (v is string s && string.IsNullOrWhiteSpace(s));

            if (_ultimaDirecaoAscendente)
            {
                return lista
                    .OrderBy(p => IsEmpty(propInfo.GetValue(p)) ? 1 : 0) // Nulos no final
                    .ThenBy(p => propInfo.GetValue(p))
                    .ToList();
            }
            else
            {
                return lista
                    .OrderBy(p => IsEmpty(propInfo.GetValue(p)) ? 1 : 0) // Nulos no final
                    .ThenByDescending(p => propInfo.GetValue(p))
                    .ToList();
            }
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

            var blocoInfo = BlocoInfo[StatusBloco.SolicitarNumerario];
            LblTitulo.Text = $"{blocoInfo.Nome} ({processos.Count})";
            LblTitulo.ForeColor = blocoInfo.Cor == Color.Black ? Color.White : Color.Black;
            LblTitulo.BackColor = blocoInfo.Cor;

            _dadosExibicaoAtual = processosOrdenados.Cast<dynamic>().ToList();
            DGVSelecionado.DataSource = _processosExibidos;

            ConfigurarColunasDataGridViewProcesso();

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

            var blocoInfo = BlocoInfo[StatusBloco.DIDUIMPParaDigitacao];
            LblTitulo.Text = $"{blocoInfo.Nome} ({processos.Count})";
            LblTitulo.ForeColor = blocoInfo.Cor == Color.Black ? Color.White : Color.Black;
            LblTitulo.BackColor = blocoInfo.Cor;

            _dadosExibicaoAtual = processosOrdenados.Cast<dynamic>().ToList();
            DGVSelecionado.DataSource = _processosExibidos; 

            ConfigurarColunasDataGridViewProcesso();

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

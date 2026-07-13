using CLUSA.Models;
using CLUSA.Repositories;
using MongoDB.Bson;
using System.Data;

namespace Trabalho
{
    public partial class frmConfiguracoes : Form
    {
        private readonly ObjectId _usuarioId;
        public string _logadoNome;
        private Dictionary<string, List<string>> _preferenciasLocais;
        private readonly RepositorioLog _logRepo;
        private string _gridAtual;

        // Lista dos 11 grids disponíveis
        private readonly Dictionary<string, string> _gridsDisponiveis = new()
        {
            { "DGVAguardandoCE", "📦 Aguardando CE" },
            { "DGVParaRedestinar", "📦 Para Redestinar" },
            { "DGVRedestinados", "📦 Redestinados" },
            { "DGVAtracadosSemPresencaCarga", "📦 Atracados S/Presença de Carga" },
            { "DGVSituacaoSIGVIG", "📦 Situação SIGVIG" },
            { "DGVAtracadosComPresencaCarga", "📦 Atracados com Presença de Carga" },
            { "DGVDeferidos", "📦 Deferidos" },
            { "DGVSolicitarNumerario", "📦 Solicitar Numerário" },
            { "DGVDIDUIMPParaDigitacao", "📦 DI/DUIMP para Digitação" },
            { "DGVSantos", "🚢 Processos Santos" },
            { "DGVItajai", "🚢 Processos Itajaí" },
            { "DGVFinalizados", "✅ Finalizados" },
            { "DgvOrgaoAnuente", "🏛️ Órgãos Anuentes" },
            { "DGVVistorias", "🔍 Vistorias" },
            { "DGVVistoriasDUIMP", "🔍 Vistorias DUIMP" },
        };

        // CONSTRUTOR OTIMIZADO: Recebe as preferências já prontas da tela principal
        public frmConfiguracoes(ObjectId usuarioId, Dictionary<string, List<string>> preferenciasAtuais, string logadoNome)
        {
            InitializeComponent();

            _usuarioId = usuarioId;
            _logadoNome = logadoNome;
            _logRepo = new RepositorioLog();

            // Fazemos uma cópia profunda (Clone) do dicionário para que, 
            // se o usuário cancelar (fechar no X), não afete a lista original.
            _preferenciasLocais = new Dictionary<string, List<string>>();
            if (preferenciasAtuais != null)
            {
                foreach (var kvp in preferenciasAtuais)
                {
                    _preferenciasLocais[kvp.Key] = new List<string>(kvp.Value);
                }
            }

            CarregarGridsDisponiveis();
            GridColumnManager.RegistrarCatalogosPadrao();
        }

        private List<string> ObterPadraoInicial(string nomeGrid)
        {
            if (nomeGrid == "DGVOrgaoAnuente" || nomeGrid == "DGVVistorias")
                return null;

            return new List<string>
            {
                "Importador", "Exportador", "Ref_USA", "SR", "Veiculo",
                "DataDeAtracacao", "Terminal", "LocalDeDesembaraco", "Container",
                "CE", "OrgaosAnuentesString", "FreeTime", "VencimentoFreeTime",
                "VencimentoFMA", "Numerario", "CapaOK", "DI"
            };
        }

        private void CarregarGridsDisponiveis()
        {
            cmbGrids.Items.Clear();
            foreach (var grid in _gridsDisponiveis)
            {
                cmbGrids.Items.Add($"{grid.Value}");
            }
            if (cmbGrids.Items.Count > 0) cmbGrids.SelectedIndex = 0;
        }

        private void CmbGrids_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbGrids.SelectedIndex < 0) return;

            if (!string.IsNullOrEmpty(_gridAtual))
                SalvarColunasGridAtual();

            var textoSelecionado = cmbGrids.SelectedItem.ToString();
            _gridAtual = _gridsDisponiveis.FirstOrDefault(g => g.Value == textoSelecionado).Key;

            CarregarColunasDoGrid(_gridAtual);
        }

        private void CarregarColunasDoGrid(string nomeGrid)
        {
            dgvColunas.Rows.Clear();
            var todasColunas = GridColumnManager.ObterCatalogo(nomeGrid);

            if (!todasColunas.Any())
            {
                lblContador.Text = "⚠️ Nenhuma coluna disponível para este grid";
                return;
            }

            _preferenciasLocais.TryGetValue(nomeGrid, out var colunasParaExibir);

            if (colunasParaExibir == null || !colunasParaExibir.Any())
                colunasParaExibir = ObterPadraoInicial(nomeGrid);

            int ordem = 1;

            if (colunasParaExibir != null && colunasParaExibir.Any())
            {
                foreach (var nomeColuna in colunasParaExibir)
                {
                    var coluna = todasColunas.FirstOrDefault(c => c.NomePropriedade == nomeColuna);
                    if (coluna != null) AdicionarLinhaColunaNoGrid(ordem++, coluna, true);
                }

                foreach (var coluna in todasColunas)
                {
                    if (!colunasParaExibir.Contains(coluna.NomePropriedade))
                        AdicionarLinhaColunaNoGrid(ordem++, coluna, false);
                }
            }
            else
            {
                foreach (var coluna in todasColunas)
                    AdicionarLinhaColunaNoGrid(ordem++, coluna, true);
            }

            AtualizarContador();
            AtualizarNumeracao();
        }

        private void AdicionarLinhaColunaNoGrid(int ordem, DefinicaoColuna coluna, bool visivel)
        {
            string tipo = coluna.TipoColuna switch
            {
                TipoColunaGrid.CheckBox => "☑ CheckBox",
                TipoColunaGrid.ComboBox => "📋 ComboBox",
                TipoColunaGrid.Image => "🖼️ Imagem",
                TipoColunaGrid.Button => "🔘 Botão",
                TipoColunaGrid.Link => "🔗 Link",
                _ => "📝 Texto"
            };

            int rowIndex = dgvColunas.Rows.Add(ordem, visivel, coluna.TituloExibicao, coluna.NomePropriedade, tipo);
            dgvColunas.Rows[rowIndex].Tag = coluna;

            if (!visivel)
            {
                dgvColunas.Rows[rowIndex].DefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
                dgvColunas.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.Gray;
            }
        }

        private void AtualizarNumeracao()
        {
            for (int i = 0; i < dgvColunas.Rows.Count; i++)
                dgvColunas.Rows[i].Cells["colOrdem"].Value = i + 1;
        }

        private void SalvarColunasGridAtual()
        {
            if (string.IsNullOrEmpty(_gridAtual)) return;

            var colunasSelecionadas = new List<string>();
            foreach (DataGridViewRow row in dgvColunas.Rows)
            {
                bool visivel = row.Cells["colVisivel"].Value != null && (bool)row.Cells["colVisivel"].Value;
                if (visivel && row.Tag is DefinicaoColuna coluna)
                    colunasSelecionadas.Add(coluna.NomePropriedade);
            }

            _preferenciasLocais[_gridAtual] = colunasSelecionadas;
        }

        private void BtnSubir_Click(object sender, EventArgs e)
        {
            if (dgvColunas.CurrentRow == null || dgvColunas.CurrentRow.Index == 0) return;
            int idx = dgvColunas.CurrentRow.Index;

            var rowCopy = (DataGridViewRow)dgvColunas.Rows[idx].Clone();
            for (int i = 0; i < dgvColunas.Columns.Count; i++)
                rowCopy.Cells[i].Value = dgvColunas.Rows[idx].Cells[i].Value;

            rowCopy.Tag = dgvColunas.Rows[idx].Tag;

            dgvColunas.Rows.RemoveAt(idx);
            dgvColunas.Rows.Insert(idx - 1, rowCopy);
            dgvColunas.CurrentCell = dgvColunas.Rows[idx - 1].Cells[0];

            AtualizarNumeracao();
        }

        private void BtnDescer_Click(object sender, EventArgs e)
        {
            if (dgvColunas.CurrentRow == null || dgvColunas.CurrentRow.Index == dgvColunas.Rows.Count - 1) return;
            int idx = dgvColunas.CurrentRow.Index;

            var rowCopy = (DataGridViewRow)dgvColunas.Rows[idx].Clone();
            for (int i = 0; i < dgvColunas.Columns.Count; i++)
                rowCopy.Cells[i].Value = dgvColunas.Rows[idx].Cells[i].Value;

            rowCopy.Tag = dgvColunas.Rows[idx].Tag;

            dgvColunas.Rows.RemoveAt(idx);
            dgvColunas.Rows.Insert(idx + 1, rowCopy);
            dgvColunas.CurrentCell = dgvColunas.Rows[idx + 1].Cells[0];

            AtualizarNumeracao();
        }

        private async void BtnReset_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_gridAtual)) return;
            string nomeGridAmigavel = _gridsDisponiveis[_gridAtual];

            var resultado = MessageBox.Show(
                $"Deseja restaurar o padrão original do grid '{nomeGridAmigavel}'?",
                "Restaurar Padrão", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (resultado == DialogResult.Yes)
            {
                if (_preferenciasLocais.ContainsKey(_gridAtual))
                    _preferenciasLocais.Remove(_gridAtual);

                CarregarColunasDoGrid(_gridAtual);

                await _logRepo.RegistrarLogAsync("Configuração", _logadoNome, $"Restaurou padrão original do grid: {nomeGridAmigavel}", "Ação de Reset manual");
            }
        }

        private void frmConfiguracoes_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!string.IsNullOrEmpty(_gridAtual))
                SalvarColunasGridAtual();
        }

        private async void BtnSalvar_Click(object sender, EventArgs e)
        {
            SalvarColunasGridAtual();

            string detalhesLog = "Nenhum grid selecionado";
            if (!string.IsNullOrEmpty(_gridAtual))
            {
                _preferenciasLocais.TryGetValue(_gridAtual, out var colunas);
                int qtdColunas = colunas?.Count ?? 0;
                string nomeAmigavel = _gridsDisponiveis.ContainsKey(_gridAtual) ? _gridsDisponiveis[_gridAtual] : _gridAtual;
                detalhesLog = $"Grid: {nomeAmigavel} | Colunas Visíveis: {qtdColunas}";
            }

            try
            {
                await _logRepo.RegistrarLogAsync("Configuração", _logadoNome, "Preferências de colunas alteradas pelo usuário", detalhesLog);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Erro ao gravar log: " + ex.Message);
            }

            // O SEGRED0 AQUI: Nós não mostramos mais a mensagem duplicada.
            // Apenas informamos ao sistema que o usuário clicou em OK e fechamos a tela!
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void DgvColunas_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            if (dgvColunas.Columns[e.ColumnIndex].Name == "colVisivel")
            {
                bool visivel = dgvColunas.Rows[e.RowIndex].Cells["colVisivel"].Value != null
                    && (bool)dgvColunas.Rows[e.RowIndex].Cells["colVisivel"].Value;

                if (visivel)
                {
                    dgvColunas.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.White;
                    dgvColunas.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Black;
                }
                else
                {
                    dgvColunas.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
                    dgvColunas.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Gray;
                }

                AtualizarContador();
            }
        }

        private void BtnSelectAll_Click(object sender, EventArgs e)
        {
            dgvColunas.SuspendLayout();
            foreach (DataGridViewRow row in dgvColunas.Rows)
            {
                row.Cells["colVisivel"].Value = true;
                row.DefaultCellStyle.BackColor = Color.White;
                row.DefaultCellStyle.ForeColor = Color.Black;
            }
            dgvColunas.ResumeLayout();
            AtualizarContador();
        }

        private void BtnRetSelectAll_Click(object sender, EventArgs e)
        {
            dgvColunas.SuspendLayout();
            foreach (DataGridViewRow row in dgvColunas.Rows)
            {
                row.Cells["colVisivel"].Value = false;
                row.DefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
                row.DefaultCellStyle.ForeColor = Color.Gray;
            }
            dgvColunas.ResumeLayout();
            AtualizarContador();
        }

        private void AtualizarContador()
        {
            int total = dgvColunas.Rows.Count;
            int marcadas = dgvColunas.Rows.Cast<DataGridViewRow>()
                .Count(r => r.Cells["colVisivel"].Value != null && (bool)r.Cells["colVisivel"].Value);

            lblContador.Text = $"📊 {marcadas} de {total} colunas selecionadas";
        }

        public Dictionary<string, List<string>> ObterPreferencias()
        {
            return _preferenciasLocais;
        }
    }
}
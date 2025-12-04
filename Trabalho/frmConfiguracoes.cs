using CLUSA;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Trabalho
{
    public partial class frmConfiguracoes : Form
    {
        private readonly ObjectId _usuarioId;
        private Dictionary<string, List<string>> _preferenciasLocais;
        private string _gridAtual;

        // Lista dos 11 grids disponíveis
        private readonly Dictionary<string, string> _gridsDisponiveis = new()
        {
            // Blocos de Processos (Santos)
            { "DGVAguardandoCE", "📦 Aguardando CE" },
            { "DGVParaRedestinar", "📦 Para Redestinar" },
            { "DGVRedestinados", "📦 Redestinados" },
            { "DGVAtracadosSemPresencaCarga", "📦 Atracados S/Presença de Carga" },
            { "DGVSituacaoSIGVIG", "📦 Situação SIGVIG" },
            { "DGVAtracadosComPresencaCarga", "📦 Atracados com Presença de Carga" },
            { "DGVDeferidos", "📦 Deferidos" },
            { "DGVSolicitarNumerario", "📦 Solicitar Numerário" },
            { "DGVDIDUIMPParaDigitacao", "📦 DI/DUIMP para Digitação" },
            
            // Grids Especiais
            { "DGVSantos", "🚢 Processos Santos" },
            { "DGVItajai", "🚢 Processos Itajaí" },
            { "DGVFinalizados", "✅ Finalizados" },
            { "DGVOrgaoAnuente", "🏛️ Órgãos Anuentes" },
            { "DGVVistorias", "🔍 Vistorias" }
        };

        public frmConfiguracoes(ObjectId usuarioId, Dictionary<string, List<string>> preferenciasAtuais)
        {
            _usuarioId = usuarioId;
            _preferenciasLocais = new Dictionary<string, List<string>>(preferenciasAtuais ?? new Dictionary<string, List<string>>());

            InitializeComponent();
            CarregarGridsDisponiveis();
            GridColumnManager.RegistrarCatalogosPadrao();
        }

        private void CarregarGridsDisponiveis()
        {
            cmbGrids.Items.Clear();

            foreach (var grid in _gridsDisponiveis)
            {
                cmbGrids.Items.Add($"{grid.Value}");
            }

            if (cmbGrids.Items.Count > 0)
            {
                cmbGrids.SelectedIndex = 0;
            }
        }

        private void CmbGrids_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbGrids.SelectedIndex < 0) return;

            // Salvar alterações do grid anterior antes de trocar
            if (!string.IsNullOrEmpty(_gridAtual))
            {
                SalvarColunasGridAtual();
            }

            // Obter nome do grid selecionado
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

            // Obter preferências salvas para este grid
            _preferenciasLocais.TryGetValue(nomeGrid, out var colunasSalvas);

            int ordem = 1;

            if (colunasSalvas != null && colunasSalvas.Any())
            {
                // Adiciona primeiro as colunas na ordem salva
                foreach (var nomeColuna in colunasSalvas)
                {
                    var coluna = todasColunas.FirstOrDefault(c => c.NomePropriedade == nomeColuna);
                    if (coluna != null)
                    {
                        AdicionarLinhaColunaNoGrid(ordem++, coluna, true);
                    }
                }

                // Depois adiciona as não selecionadas
                foreach (var coluna in todasColunas)
                {
                    if (!colunasSalvas.Contains(coluna.NomePropriedade))
                    {
                        AdicionarLinhaColunaNoGrid(ordem++, coluna, false);
                    }
                }
            }
            else
            {
                // Se não tem preferências, adiciona todas marcadas (padrão)
                foreach (var coluna in todasColunas)
                {
                    AdicionarLinhaColunaNoGrid(ordem++, coluna, true);
                }
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

            int rowIndex = dgvColunas.Rows.Add(
                ordem,
                visivel,
                coluna.TituloExibicao,
                coluna.NomePropriedade,
                tipo
            );

            // Armazenar o objeto DefinicaoColuna na Tag da linha
            dgvColunas.Rows[rowIndex].Tag = coluna;

            // Colorir linha se não estiver visível
            if (!visivel)
            {
                dgvColunas.Rows[rowIndex].DefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
                dgvColunas.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.Gray;
            }
        }

        private void AtualizarNumeracao()
        {
            for (int i = 0; i < dgvColunas.Rows.Count; i++)
            {
                dgvColunas.Rows[i].Cells["colOrdem"].Value = i + 1;
            }
        }

        private void SalvarColunasGridAtual()
        {
            if (string.IsNullOrEmpty(_gridAtual)) return;

            var colunasSelecionadas = new List<string>();

            foreach (DataGridViewRow row in dgvColunas.Rows)
            {
                bool visivel = row.Cells["colVisivel"].Value != null && (bool)row.Cells["colVisivel"].Value;

                if (visivel && row.Tag is DefinicaoColuna coluna)
                {
                    colunasSelecionadas.Add(coluna.NomePropriedade);
                }
            }

            _preferenciasLocais[_gridAtual] = colunasSelecionadas;
        }

        private void BtnSubir_Click(object sender, EventArgs e)
        {
            if (dgvColunas.CurrentRow == null || dgvColunas.CurrentRow.Index == 0) return;

            int idx = dgvColunas.CurrentRow.Index;

            var rowCopy = (DataGridViewRow)dgvColunas.Rows[idx].Clone();
            for (int i = 0; i < dgvColunas.Columns.Count; i++)
            {
                rowCopy.Cells[i].Value = dgvColunas.Rows[idx].Cells[i].Value;
            }
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
            {
                rowCopy.Cells[i].Value = dgvColunas.Rows[idx].Cells[i].Value;
            }
            rowCopy.Tag = dgvColunas.Rows[idx].Tag;

            dgvColunas.Rows.RemoveAt(idx);
            dgvColunas.Rows.Insert(idx + 1, rowCopy);
            dgvColunas.CurrentCell = dgvColunas.Rows[idx + 1].Cells[0];

            AtualizarNumeracao();
        }
        private void BtnReset_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_gridAtual)) return;

            var resultado = MessageBox.Show(
                $"Tem certeza que deseja resetar as colunas do grid '{_gridsDisponiveis[_gridAtual]}'?\n\nTodas as colunas serão marcadas e a ordem será resetada.",
                "Confirmar Reset",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (resultado == DialogResult.Yes)
            {
                _preferenciasLocais.Remove(_gridAtual);
                CarregarColunasDoGrid(_gridAtual);

                MessageBox.Show("Grid resetado com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void frmConfiguracoes_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!string.IsNullOrEmpty(_gridAtual))
                SalvarColunasGridAtual();
        }

        private void BtnSalvar_Click(object sender, EventArgs e)
        {
            SalvarColunasGridAtual();
            MessageBox.Show(
                        "Configurações salvas com sucesso!\n\nAs alterações serão aplicadas ao reabrir os grids.",
                        "Sucesso",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
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
        // Botão: Selecionar Todas (Marca o CheckBox de TODAS as linhas)
        private void BtnSelectAll_Click(object sender, EventArgs e)
        {
            dgvColunas.SuspendLayout(); // Otimização de pintura

            foreach (DataGridViewRow row in dgvColunas.Rows)
            {
                // Define o CheckBox como TRUE (Marcado)
                row.Cells["colVisivel"].Value = true;

                // Atualiza a cor para Branco (Ativado)
                row.DefaultCellStyle.BackColor = Color.White;
                row.DefaultCellStyle.ForeColor = Color.Black;
            }

            dgvColunas.ResumeLayout();
            AtualizarContador();
        }

        // Botão: Retirar Todas (Desmarca o CheckBox de TODAS as linhas)
        private void BtnRetSelectAll_Click(object sender, EventArgs e)
        {
            dgvColunas.SuspendLayout(); // Otimização de pintura

            foreach (DataGridViewRow row in dgvColunas.Rows)
            {
                // Define o CheckBox como FALSE (Desmarcado)
                row.Cells["colVisivel"].Value = false;

                // Atualiza a cor para Cinza (Desativado)
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

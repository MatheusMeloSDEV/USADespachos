using CLUSA.Models;
using CLUSA.Repositories;
using DocumentFormat.OpenXml.Presentation;
using MongoDB.Bson;
using MongoDB.Driver;
using ReaLTaiizor.Forms;
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
    public partial class frmConfiguracoesNovo : PoisonForm
    {
        private readonly ObjectId _usuarioId;
        public string _logadoNome;
        private Dictionary<string, List<string>> _preferenciasLocais;
        private readonly RepositorioLog _logRepo;
        private readonly RepositorioUsers _repositorioUsers;
        private string _gridAtual;
        private List<string> ObterPadraoInicial(string nomeGrid)
        {
            // 1. EXCEÇÕES: Grids que NÃO devem seguir o padrão global
            // Se for Anuente ou Vistoria, retorna null.
            // (Retornar null faz o sistema exibir TODAS as colunas disponíveis naquele grid)
            if (nomeGrid == "DGVOrgaoAnuente" || nomeGrid == "DGVVistorias")
            {
                return null;
            }

            // 2. PADRÃO GLOBAL (Default)
            // Aplica-se a DGVSantos, DGVItajai, AguardandoCE, Finalizados, etc.
            return new List<string>
                {
                    "Importador",           // IMP
                    "Exportador",           // EXP
                    "Ref_USA",              // N/REF
                    "SR",                   // SFREF
                    "Veiculo",              // NAVIO
                    "DataDeAtracacao",      // DATA e HRS
                    "Terminal",             // TERMINAL
                    "LocalDeDesembaraco",   // LOCAL
                    "Container",            // CONTAINER
                    "CE",                   // CE
                    "OrgaosAnuentesString", // ANUT
                    "FreeTime",             // FT
                    "VencimentoFreeTime",   // VENC
                    "VencimentoFMA",        // FMA
                    "Numerario",            // NUM
                    "CapaOK",               // CP
                    "DI"                    // DI
                };
        }
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
            { "DgvOrgaoAnuente", "🏛️ Órgãos Anuentes" },
            { "DGVVistorias", "🔍 Vistorias" }
        };
        public frmConfiguracoesNovo(ObjectId usuarioId)
        {
            _usuarioId = usuarioId;
            _logRepo = new RepositorioLog();

            var client = new MongoClient(ConfigDatabase.MongoConnectionString);
            var database = client.GetDatabase(ConfigDatabase.MongoDatabaseName);
            _repositorioUsers = new RepositorioUsers(database);

            InitializeComponent();

            // 2. Registre o evento Load para carregar o banco sem travar a tela
            this.Load += Form1_Load;
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                // Aqui o await funciona corretamente sem congelar o programa
                var usuario = await GetUser(_usuarioId);

                if (usuario != null)
                {
                    _preferenciasLocais = new Dictionary<string, List<string>>(
                        usuario.PreferenciasGrids ?? new Dictionary<string, List<string>>()
                    );

                    _logadoNome = usuario.Username; // Aproveite para setar o nome do logado

                    CarregarGridsDisponiveis();
                    GridColumnManager.RegistrarCatalogosPadrao();

                    toggleEstiloNovo.Checked = usuario.UsarEstiloNovo;
                    toggleModoEscuro.Checked = usuario.ModoEscuro;

                    AplicarEstilo(usuario.UsarEstiloNovo, usuario.ModoEscuro);
                }
                else
                {
                    MessageBox.Show("Usuário não encontrado no banco de dados.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao conectar ao MongoDB: {ex.Message}");
            }
        }
        private async Task<Users> GetUser(ObjectId user)
        {
            return await _repositorioUsers.GetByIdAsync(user);

        }
        private async void btnSalvar_Click(object sender, EventArgs e)
        {
            SalvarColunasGridAtual();

            // Lógica para montar mensagem detalhada do log
            string detalhesLog = "Nenhum grid selecionado";
            if (!string.IsNullOrEmpty(_gridAtual))
            {
                // Conta quantas colunas estão ativas para esse grid específico
                _preferenciasLocais.TryGetValue(_gridAtual, out var colunas);
                int qtdColunas = colunas?.Count ?? 0;
                string nomeAmigavel = _gridsDisponiveis.ContainsKey(_gridAtual) ? _gridsDisponiveis[_gridAtual] : _gridAtual;

                detalhesLog = $"Grid: {nomeAmigavel} | Colunas Visíveis: {qtdColunas}";
            }

            // --- LOG DE CONFIGURAÇÃO ---
            // Note: Mudei a assinatura do método para 'async void' para suportar o await
            try
            {
                await _logRepo.RegistrarLogAsync(
                    "Configuração", _logadoNome,
                    "Preferências de colunas alteradas pelo usuário",
                    detalhesLog
                );
            }
            catch (Exception ex)
            {
                // Se der erro no log, não impede o fluxo principal, mas é bom saber (debug)
                System.Diagnostics.Debug.WriteLine("Erro ao gravar log: " + ex.Message);
            }

            MessageBox.Show(
                "Configurações salvas com sucesso!\n\nAs alterações serão aplicadas ao reabrir os grids.",
                "Sucesso",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void btnBaixo_Click(object sender, EventArgs e)
        {
            if (dgvConfiguracoes.CurrentRow == null || dgvConfiguracoes.CurrentRow.Index == dgvConfiguracoes.Rows.Count - 1) return;

            int idx = dgvConfiguracoes.CurrentRow.Index;

            var rowCopy = (DataGridViewRow)dgvConfiguracoes.Rows[idx].Clone();
            for (int i = 0; i < dgvConfiguracoes.Columns.Count; i++)
            {
                rowCopy.Cells[i].Value = dgvConfiguracoes.Rows[idx].Cells[i].Value;
            }
            rowCopy.Tag = dgvConfiguracoes.Rows[idx].Tag;

            dgvConfiguracoes.Rows.RemoveAt(idx);
            dgvConfiguracoes.Rows.Insert(idx + 1, rowCopy);
            dgvConfiguracoes.CurrentCell = dgvConfiguracoes.Rows[idx + 1].Cells[0];

            AtualizarNumeracao();
        }

        private void btnCima_Click(object sender, EventArgs e)
        {
            if (dgvConfiguracoes.CurrentRow == null || dgvConfiguracoes.CurrentRow.Index == 0) return;

            int idx = dgvConfiguracoes.CurrentRow.Index;

            var rowCopy = (DataGridViewRow)dgvConfiguracoes.Rows[idx].Clone();
            for (int i = 0; i < dgvConfiguracoes.Columns.Count; i++)
            {
                rowCopy.Cells[i].Value = dgvConfiguracoes.Rows[idx].Cells[i].Value;
            }
            rowCopy.Tag = dgvConfiguracoes.Rows[idx].Tag;

            dgvConfiguracoes.Rows.RemoveAt(idx);
            dgvConfiguracoes.Rows.Insert(idx - 1, rowCopy);
            dgvConfiguracoes.CurrentCell = dgvConfiguracoes.Rows[idx - 1].Cells[0];

            AtualizarNumeracao();
        }

        private void CarregarColunasDoGrid(string nomeGrid)
        {
            dgvConfiguracoes.Rows.Clear();

            var todasColunas = GridColumnManager.ObterCatalogo(nomeGrid);

            if (!todasColunas.Any())
            {
                lblContador.Text = "⚠️ Nenhuma coluna disponível para este grid";
                return;
            }

            // 1. Tenta obter preferências salvas pelo usuário
            _preferenciasLocais.TryGetValue(nomeGrid, out var colunasParaExibir);

            // 2. SE NÃO tiver nada salvo, tenta pegar o "Padrão Inicial" (Sua lista do Excel)
            if (colunasParaExibir == null || !colunasParaExibir.Any())
            {
                colunasParaExibir = ObterPadraoInicial(nomeGrid);
            }

            int ordem = 1;

            // 3. Verifica se agora temos uma lista (seja do usuário ou o padrão)
            if (colunasParaExibir != null && colunasParaExibir.Any())
            {
                // A. Adiciona primeiro as colunas da lista (Marcadas como VISÍVEIS)
                foreach (var nomeColuna in colunasParaExibir)
                {
                    var coluna = todasColunas.FirstOrDefault(c => c.NomePropriedade == nomeColuna);
                    if (coluna != null)
                    {
                        AdicionarLinhaColunaNoGrid(ordem++, coluna, true);
                    }
                }

                // B. Depois adiciona o resto das colunas que sobraram (Marcadas como NÃO VISÍVEIS)
                foreach (var coluna in todasColunas)
                {
                    // Se a coluna não estava na lista de cima, adiciona ela agora
                    if (!colunasParaExibir.Contains(coluna.NomePropriedade))
                    {
                        AdicionarLinhaColunaNoGrid(ordem++, coluna, false);
                    }
                }
            }
            else
            {
                // 4. Caso extremo: Não tem salvo e não tem padrão definido -> Mostra TUDO
                foreach (var coluna in todasColunas)
                {
                    AdicionarLinhaColunaNoGrid(ordem++, coluna, true);
                }
            }

            AtualizarContador();
            AtualizarNumeracao();
        }
        private void CarregarGridsDisponiveis()
        {
            cmbGrid.Items.Clear();

            foreach (var grid in _gridsDisponiveis)
            {
                cmbGrid.Items.Add($"{grid.Value}");
            }

            if (cmbGrid.Items.Count > 0)
            {
                cmbGrid.SelectedIndex = 0;
            }
        }
        private void AtualizarContador()
        {
            int total = dgvConfiguracoes.Rows.Count;
            int marcadas = dgvConfiguracoes.Rows.Cast<DataGridViewRow>()
                .Count(r => r.Cells["colVisivel"].Value != null && (bool)r.Cells["colVisivel"].Value);

            lblContador.Text = $"📊 {marcadas} de {total} colunas selecionadas";
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

            int rowIndex = dgvConfiguracoes.Rows.Add(
                ordem,
                visivel,
                coluna.TituloExibicao,
                coluna.NomePropriedade,
                tipo
            );

            // Armazenar o objeto DefinicaoColuna na Tag da linha
            dgvConfiguracoes.Rows[rowIndex].Tag = coluna;

            // Colorir linha se não estiver visível
            if (!visivel)
            {
                dgvConfiguracoes.Rows[rowIndex].DefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
                dgvConfiguracoes.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.Gray;
            }
        }

        private void AtualizarNumeracao()
        {
            for (int i = 0; i < dgvConfiguracoes.Rows.Count; i++)
            {
                dgvConfiguracoes.Rows[i].Cells["colOrdem"].Value = i + 1;
            }
        }

        private void SalvarColunasGridAtual()
        {
            if (string.IsNullOrEmpty(_gridAtual)) return;

            var colunasSelecionadas = new List<string>();

            foreach (DataGridViewRow row in dgvConfiguracoes.Rows)
            {
                bool visivel = row.Cells["colVisivel"].Value != null && (bool)row.Cells["colVisivel"].Value;

                if (visivel && row.Tag is DefinicaoColuna coluna)
                {
                    colunasSelecionadas.Add(coluna.NomePropriedade);
                }
            }

            _preferenciasLocais[_gridAtual] = colunasSelecionadas;
        }

        private void cmbGrid_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbGrid.SelectedIndex < 0) return;

            // Salvar alterações do grid anterior antes de trocar
            if (!string.IsNullOrEmpty(_gridAtual))
            {
                SalvarColunasGridAtual();
            }

            // Obter nome do grid selecionado
            var textoSelecionado = cmbGrid.SelectedItem.ToString();
            _gridAtual = _gridsDisponiveis.FirstOrDefault(g => g.Value == textoSelecionado).Key;

            CarregarColunasDoGrid(_gridAtual);
        }

        private void AplicarEstilo(bool estiloNovo, bool modoEscuro)
        {
            if (estiloNovo)
            {
                if (modoEscuro)
                {
                    this.Theme = ReaLTaiizor.Enum.Poison.ThemeStyle.Dark;
                    this.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Silver;
                    this.BackColor = Color.White;
                    cmbGrid.Theme = ReaLTaiizor.Enum.Poison.ThemeStyle.Dark;
                    cmbGrid.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Silver;
                    lblContador.Theme = ReaLTaiizor.Enum.Poison.ThemeStyle.Dark;
                    lblContador.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Silver;
                    dgvConfiguracoes.Theme = ReaLTaiizor.Enum.Poison.ThemeStyle.Dark;
                    dgvConfiguracoes.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Silver;
                    tcConfiguracoes.Theme = ReaLTaiizor.Enum.Poison.ThemeStyle.Dark;
                    tcConfiguracoes.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Silver;
                    rbSelectAll.Theme = ReaLTaiizor.Enum.Poison.ThemeStyle.Dark;
                    rbSelectAll.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Silver;
                    lblEstilo.Theme = ReaLTaiizor.Enum.Poison.ThemeStyle.Dark;
                    lblEstilo.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Silver;
                    lblModoEscuro.Theme = ReaLTaiizor.Enum.Poison.ThemeStyle.Dark;
                    lblModoEscuro.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Silver;

                    toggleEstiloNovo.BGColor = Color.FromArgb(84, 85, 86);
                    toggleEstiloNovo.ToggleColor = Color.FromArgb(45, 47, 49);
                    toggleModoEscuro.BGColor = Color.FromArgb(84, 85, 86);
                    toggleModoEscuro.ToggleColor = Color.FromArgb(45, 47, 49);

                    btnBaixo.InactiveColor = Color.FromArgb(32, 34, 37);
                    btnCima.InactiveColor = Color.FromArgb(32, 34, 37);
                    btnCancelar.InactiveColor = Color.FromArgb(32, 34, 37);
                    btnSalvar.InactiveColor = Color.FromArgb(32, 34, 37);
                    btnSalvarEstilo.InactiveColor = Color.FromArgb(32, 34, 37);

                    btnBaixo.ForeColor = Color.White;
                    btnCima.ForeColor = Color.White;
                    btnSalvar.ForeColor = Color.White;
                    btnCancelar.ForeColor = Color.White;
                    btnSalvarEstilo.ForeColor = Color.White;

                    btnSalvar.BorderColor = Color.FromArgb(32, 34, 37);
                    btnCancelar.BorderColor = Color.FromArgb(32, 34, 37);
                    btnCima.BorderColor = Color.FromArgb(32, 34, 37);
                    btnBaixo.BorderColor = Color.FromArgb(32, 34, 37);
                    btnSalvarEstilo.BorderColor = Color.FromArgb(32, 34, 37);

                    this.Invalidate();
                    this.Refresh();
                }
                else
                {
                    this.Theme = ReaLTaiizor.Enum.Poison.ThemeStyle.Light;
                    this.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Blue;
                    this.BackColor = Color.White;
                    cmbGrid.Theme = ReaLTaiizor.Enum.Poison.ThemeStyle.Light;
                    cmbGrid.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Blue;
                    lblContador.Theme = ReaLTaiizor.Enum.Poison.ThemeStyle.Light;
                    lblContador.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Blue;
                    dgvConfiguracoes.Theme = ReaLTaiizor.Enum.Poison.ThemeStyle.Light;
                    dgvConfiguracoes.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Blue;
                    tcConfiguracoes.Theme = ReaLTaiizor.Enum.Poison.ThemeStyle.Light;
                    tcConfiguracoes.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Blue;
                    rbSelectAll.Theme = ReaLTaiizor.Enum.Poison.ThemeStyle.Light;
                    rbSelectAll.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Blue;
                    lblEstilo.Theme = ReaLTaiizor.Enum.Poison.ThemeStyle.Light;
                    lblEstilo.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Blue;
                    lblModoEscuro.Theme = ReaLTaiizor.Enum.Poison.ThemeStyle.Light;
                    lblModoEscuro.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Blue;

                    toggleEstiloNovo.BGColor = Color.Silver;
                    toggleEstiloNovo.ToggleColor = Color.DodgerBlue;
                    toggleModoEscuro.BGColor = Color.Silver;
                    toggleModoEscuro.ToggleColor = Color.DodgerBlue;

                    btnBaixo.InactiveColor = Color.Transparent;
                    btnCima.InactiveColor = Color.Transparent;
                    btnCancelar.InactiveColor = Color.Transparent;
                    btnSalvar.InactiveColor = Color.Transparent;
                    btnSalvarEstilo.InactiveColor = Color.Transparent;

                    btnBaixo.ForeColor = Color.Black;
                    btnCima.ForeColor = Color.Black;
                    btnSalvar.ForeColor = Color.Black;
                    btnCancelar.ForeColor = Color.Black;
                    btnSalvarEstilo.ForeColor = Color.Black;

                    btnSalvar.BorderColor = Color.DodgerBlue;
                    btnCancelar.BorderColor = Color.DodgerBlue;
                    btnCima.BorderColor = Color.DodgerBlue;
                    btnBaixo.BorderColor = Color.DodgerBlue;
                    btnSalvarEstilo.BorderColor = Color.DodgerBlue;

                    this.Invalidate();
                    this.Refresh();
                }
            }
        }

        private void toggleEstiloNovo_CheckedChanged(object sender)
        {
            // O Modo Escuro só está disponível no Estilo Novo
            toggleModoEscuro.Enabled = toggleEstiloNovo.Checked;

            if (!toggleEstiloNovo.Checked)
            {
                toggleModoEscuro.Checked = false; // Desliga o escuro se voltar pro antigo
            }


        }

        private async void btnSalvarEstilo_Click(object sender, EventArgs e)
        {
            try
            {
                var usuario = await GetUser(_usuarioId);

                if (usuario != null)
                {
                    // 2. ATRIBUI OS VALORES DOS TOGGLES (Importante!)
                    usuario.UsarEstiloNovo = toggleEstiloNovo.Checked;
                    usuario.ModoEscuro = toggleModoEscuro.Checked;

                    // 3. Salva as colunas do grid atual no dicionário local
                    SalvarColunasGridAtual();
                    usuario.PreferenciasGrids = _preferenciasLocais;

                    // 4. Salva tudo no banco
                    await _repositorioUsers.UpdateAsync(usuario);

                    MessageBox.Show("Configurações salvas com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao salvar: " + ex.Message);
            }
        }

        private void toggleModoEscuro_CheckedChanged(object sender)
        {
            AplicarEstilo(toggleEstiloNovo.Checked, toggleModoEscuro.Checked);

            this.Invalidate();
            this.Refresh();
        }
    }
}


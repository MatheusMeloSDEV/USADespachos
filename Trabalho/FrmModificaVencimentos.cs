using CLUSA.Models;
using CLUSA.Repositories;
using CLUSA.Helpers;

namespace Trabalho
{
    public partial class FrmModificaVencimentos : Form
    {
        private readonly RepositorioVencimento _repoVencimento;
        private readonly RepositorioLog _repoLog;
        private string _idEdicao = null;
        public string _logadoNome;

        public class ImportadorOpcao { public string Nome { get; set; } public List<string> Cnpjs { get; set; } }

        public FrmModificaVencimentos(string idParaEditar = null)
        {
            InitializeComponent();
            _repoVencimento = new RepositorioVencimento();
            _repoLog = new RepositorioLog();
            _idEdicao = idParaEditar;

            CarregarComboBox();
            ConfigurarEventosDosChecks(); // Liga a lógica visual

            if (!string.IsNullOrEmpty(_idEdicao))
            {
                CarregarDadosParaEdicao();
                this.Text = "Editar Vencimentos";
            }
        }

        // Método auxiliar para ativar/desativar o calendário quando clica no check
        private void ConfigurarEventosDosChecks()
        {
            // Antigos
            chkRadar.CheckedChanged += (s, e) => dtpRadar.Enabled = chkRadar.Checked;
            chkProcuracao.CheckedChanged += (s, e) => dtpProcuracao.Enabled = chkProcuracao.Checked;
            chkEcac.CheckedChanged += (s, e) => dtpEcac.Enabled = chkEcac.Checked;
            chkSigvig.CheckedChanged += (s, e) => dtpSigvig.Enabled = chkSigvig.Checked;
            chkLecom.CheckedChanged += (s, e) => dtpLecom.Enabled = chkLecom.Checked;

            // --- NOVOS (Azeite e Vinho) ---
            // Assumindo que cbAzeite e cbVinho são os CheckBoxes novos
            cbAzeite.CheckedChanged += (s, e) => dtpAzeite.Enabled = cbAzeite.Checked;
            cbVinho.CheckedChanged += (s, e) => dtpVinho.Enabled = cbVinho.Checked;

            // Inicia desabilitados ou habilitados conforme o designer
            dtpRadar.Enabled = chkRadar.Checked;
            dtpProcuracao.Enabled = chkProcuracao.Checked;
            dtpEcac.Enabled = chkEcac.Checked;
            dtpSigvig.Enabled = chkSigvig.Checked;
            dtpLecom.Enabled = chkLecom.Checked;

            // --- NOVOS ---
            dtpAzeite.Enabled = cbAzeite.Checked;
            dtpVinho.Enabled = cbVinho.Checked;
        }

        // Lógica de LOAD (Carregar do Banco para a Tela)
        private async void CarregarDadosParaEdicao()
        {
            try
            {
                var v = await _repoVencimento.ObterPorIdAsync(_idEdicao);
                if (v != null)
                {
                    int index = cbImportador.FindStringExact(v.Importador);
                    cbImportador.SelectedIndex = index;

                    PreencherCampo(v.DataVencimentoRadar, chkRadar, dtpRadar);
                    PreencherCampo(v.DataVencimentoProcuracao, chkProcuracao, dtpProcuracao);
                    PreencherCampo(v.DataVencimentoEcac, chkEcac, dtpEcac);
                    PreencherCampo(v.DataVencimentoSigvig, chkSigvig, dtpSigvig);
                    PreencherCampo(v.DataVencimentoLecom, chkLecom, dtpLecom);

                    // --- NOVOS ---
                    PreencherCampo(v.DataVencimentoAzeite, cbAzeite, dtpAzeite);
                    PreencherCampo(v.DataVencimentoVinho, cbVinho, dtpVinho);
                }
            }
            catch (Exception ex) { MessageBox.Show("Erro: " + ex.Message); }
        }

        // Helper para limpar o código acima
        private void PreencherCampo(DateTime? data, CheckBox chk, DateTimePicker dtp)
        {
            if (data.HasValue)
            {
                chk.Checked = true;
                dtp.Value = data.Value;
            }
            else
            {
                chk.Checked = false;
            }
        }

        // Lógica de SAVE (Da Tela para o Banco)
        private async void btnEnviar_Click(object sender, EventArgs e)
        {
            if (cbImportador.SelectedItem is ImportadorOpcao itemSelecionado)
            {
                try
                {
                    var vencimento = new Vencimento
                    {
                        Id = _idEdicao,
                        Importador = itemSelecionado.Nome,
                        Cnpjs = itemSelecionado.Cnpjs,

                        DataVencimentoRadar = chkRadar.Checked ? dtpRadar.Value : (DateTime?)null,
                        DataVencimentoProcuracao = chkProcuracao.Checked ? dtpProcuracao.Value : (DateTime?)null,
                        DataVencimentoEcac = chkEcac.Checked ? dtpEcac.Value : (DateTime?)null,
                        DataVencimentoSigvig = chkSigvig.Checked ? dtpSigvig.Value : (DateTime?)null,
                        DataVencimentoLecom = chkLecom.Checked ? dtpLecom.Value : (DateTime?)null,

                        // --- NOVOS ---
                        DataVencimentoAzeite = cbAzeite.Checked ? dtpAzeite.Value : (DateTime?)null,
                        DataVencimentoVinho = cbVinho.Checked ? dtpVinho.Value : (DateTime?)null,
                        // -------------

                        DataUltimaNotificacao = null
                    };

                    if (string.IsNullOrEmpty(_idEdicao))
                    {
                        await _repoVencimento.AdicionarAsync(vencimento);
                        await _repoLog.RegistrarLogAsync("Criação", _logadoNome, $"Novo vencimento criado para {vencimento.Importador}");
                    }
                    else
                    {
                        // Se for edição, precisamos manter a data da ultima notificação ou resetar?
                        // Normalmente mantemos o que estava no banco se quisermos evitar spam, 
                        // ou resetamos se quisermos que a mudança de data force um novo email.
                        // A lógica atual reseta para null (força novo email em breve).

                        await _repoVencimento.AtualizarAsync(vencimento);
                        await _repoLog.RegistrarLogAsync("Edição", _logadoNome, $"Vencimento de {vencimento.Importador} foi alterado.", $"ID: {_idEdicao}");
                    }

                    MessageBox.Show("Salvo com sucesso!");
                    this.Close();
                }
                catch (Exception ex) { MessageBox.Show("Erro: " + ex.Message); }
            }
            else { MessageBox.Show("Selecione um importador."); }
        }

        private void CarregarComboBox()
        {
            var dadosBrutos = DadosEstaticos.ObterListaCNPJs();
            var listaParaCombo = dadosBrutos
                .GroupBy(x => x.Nome)
                .Select(g => new ImportadorOpcao { Nome = g.Key, Cnpjs = g.Select(i => i.Cnpj).ToList() })
                .OrderBy(x => x.Nome)
                .ToList();
            cbImportador.DataSource = listaParaCombo;
            cbImportador.DisplayMember = "Nome";
        }
    }
}
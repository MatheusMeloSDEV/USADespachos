using CLUSA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Trabalho
{
    public partial class FrmModificaVencimentos : Form
    {
        private readonly VencimentoRepository _repoVencimento;
        private readonly LogRepository _repoLog;
        private string _idEdicao = null;

        public class ImportadorOpcao { public string Nome { get; set; } public List<string> Cnpjs { get; set; } }

        public FrmModificaVencimentos(string idParaEditar = null)
        {
            InitializeComponent();
            _repoVencimento = new VencimentoRepository();
            _repoLog = new LogRepository();
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
            chkRadar.CheckedChanged += (s, e) => dtpRadar.Enabled = chkRadar.Checked;
            chkProcuracao.CheckedChanged += (s, e) => dtpProcuracao.Enabled = chkProcuracao.Checked;
            chkEcac.CheckedChanged += (s, e) => dtpEcac.Enabled = chkEcac.Checked;
            chkSigvig.CheckedChanged += (s, e) => dtpSigvig.Enabled = chkSigvig.Checked;
            chkLecom.CheckedChanged += (s, e) => dtpLecom.Enabled = chkLecom.Checked;

            // Inicia desabilitados (opcional, depende de como vc deixou no designer)
            dtpRadar.Enabled = chkRadar.Checked;
            dtpProcuracao.Enabled = chkProcuracao.Checked;
            dtpEcac.Enabled = chkEcac.Checked;
            dtpSigvig.Enabled = chkSigvig.Checked;
            dtpLecom.Enabled = chkLecom.Checked;
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

                        DataUltimaNotificacao = null
                    };

                    if (string.IsNullOrEmpty(_idEdicao))
                    {
                        await _repoVencimento.AdicionarAsync(vencimento);
                        await _repoLog.RegistrarLogAsync("Criação", $"Novo vencimento criado para {vencimento.Importador}");
                    }
                    else
                    {
                        await _repoVencimento.AtualizarAsync(vencimento);
                        await _repoLog.RegistrarLogAsync("Edição", $"Vencimento de {vencimento.Importador} foi alterado.", $"ID: {_idEdicao}");
                    }

                    MessageBox.Show("Salvo com sucesso!");
                    this.Close();
                }
                catch (Exception ex) { MessageBox.Show("Erro: " + ex.Message); }
            }
            else { MessageBox.Show("Selecione um importador."); }
        }

        // ... (Mantenha o método CarregarComboBox igual)
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
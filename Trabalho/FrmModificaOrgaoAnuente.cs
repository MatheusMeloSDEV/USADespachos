using CLUSA;
using System.Data;

namespace Trabalho
{
    public partial class FrmModificaOrgaoAnuente : Form
    {
        public OrgaoAnuente OrgaoAnuente { get; set; }
        public Processo Processo { get; set; }
        private bool _dadosForamAlterados = false;

        public FrmModificaOrgaoAnuente()
        {
            InitializeComponent();
            OrgaoAnuente = new OrgaoAnuente();
            Processo = new Processo();
        }

        private void FrmModificaOrgaoAnuente_Load(object? sender, EventArgs e)
        {
            if (OrgaoAnuente == null || Processo == null)
            {
                MessageBox.Show("Erro: Os dados não foram carregados.", "Erro");
                this.Close();
                return;
            }
            CarregarDados();
            AnexarEventoDeAlteracao(this);
        }
        private void MarcarComoAlterado(object? sender, EventArgs e)
        {
            // Uma vez que algo muda, a bandeira é levantada e permanece assim até salvarmos.
            if (!_dadosForamAlterados)
            {
                _dadosForamAlterados = true;
                this.Text += "*"; // Opcional: Adiciona um "*" no título para indicar alterações
            }
        }

        private void AnexarEventoDeAlteracao(Control parent)
        {
            foreach (Control c in parent.Controls)
            {
                switch (c)
                {
                    case TextBox box: box.TextChanged += MarcarComoAlterado; break;
                    case ComboBox box: box.SelectedIndexChanged += MarcarComoAlterado; break;
                    case DateTimePicker dtp: dtp.ValueChanged += MarcarComoAlterado; break;
                    case CheckBox chk: chk.CheckedChanged += MarcarComoAlterado; break;
                    case NumericUpDown num: num.ValueChanged += MarcarComoAlterado; break;
                    case CheckedListBox clb: clb.ItemCheck += (s, e) => MarcarComoAlterado(s, e); break;
                }

                // Faz o mesmo para controles dentro de outros containers (ex: GroupBox)
                if (c.HasChildren)
                {
                    AnexarEventoDeAlteracao(c);
                }
            }
        }
        private void frmModificaOrgaoAnuente_FormClosing(object? sender, FormClosingEventArgs e)
        {
            // Se a bandeira de alterações estiver levantada...
            if (_dadosForamAlterados)
            {
                // ...pergunta ao usuário o que fazer.
                var resultado = MessageBox.Show(
                    "Você tem alterações não salvas. Deseja fechar e descartar as alterações?",
                    "Atenção",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                // Se o usuário escolher "Não", nós cancelamos o fechamento do formulário.
                if (resultado == DialogResult.No)
                {
                    e.Cancel = true;
                }
            }
            // Se a bandeira não estiver levantada, o formulário fecha normalmente sem perguntar nada.
        }
        private void CarregarDados()
        {
            this.Text = string.IsNullOrWhiteSpace(OrgaoAnuente.Numero)
                ? "Detalhes da LI"
        :         $"LI - {OrgaoAnuente.Numero}";

            // --- Carrega dados de CONTEXTO (somente leitura) ---
            TXTnr.Text = Processo.Ref_USA;
            TXTsr.Text = Processo.SR;
            TXTimportador.Text = Processo.Importador;
            TXTexportador.Text = Processo.Exportador;
            TXTProduto.Text = Processo.Produto;
            TXTstatusdoprocesso.Text = Processo.HistoricoDoProcesso;
            TXTpendencia.Text = Processo.Pendencia;


            // Trava os campos de contexto
            new List<TextBox> { TXTnr, TXTsr, TXTimportador, TXTexportador, TXTProduto }.ForEach(txt => txt.ReadOnly = true);

            // --- Carrega dados EDITÁVEIS do OrgaoAnuente (a LI) ---
            TXTpendencia.DataBindings.Add("Text", OrgaoAnuente, nameof(OrgaoAnuente.Pendencia), false, DataSourceUpdateMode.OnPropertyChanged);
            TXTstatusdoprocesso.DataBindings.Add("Text", OrgaoAnuente, nameof(OrgaoAnuente.HistoricoDoProcesso), false, DataSourceUpdateMode.OnPropertyChanged);

            DTPdatadeinspecao.Enabled = false;
            ConfigurarDatePickerNulavel(DTPdatadeinspecao, OrgaoAnuente.Inspecao);

            // --- Carrega dados da LI nos campos do GroupBox "Dados Li" ---
            TxtLi.DataBindings.Add("Text", OrgaoAnuente, nameof(OrgaoAnuente.Numero), false, DataSourceUpdateMode.OnPropertyChanged);
            TxtNCM.DataBindings.Add("Text", OrgaoAnuente, nameof(OrgaoAnuente.NCM), false, DataSourceUpdateMode.OnPropertyChanged);
            ConfigurarDatePickerNulavel(DtpDataRegistro, OrgaoAnuente.DataRegistro);

            TxtLi.ReadOnly = true;
            // --- Carrega as abas de LPCO ---
            CarregarAbasLpco();
        }
        private void AdicionarAbaLpco(LpcoInfo lpco)
        {
            var tabPage = new TabPage(lpco.NomeOrgao) { Name = $"tabPage_{lpco.NomeOrgao}", Tag = lpco };
            var lblLpco = new Label { Text = "LPCO:", Location = new Point(15, 20), AutoSize = true };
            var txtLpco = new TextBox { Name = "txtLpco", Location = new Point(120, 17), Width = 150 };
            var lblParam = new Label { Text = "Parametrização:", Location = new Point(15, 50), AutoSize = true };
            var cmbParam = new ComboBox
            {
                Name = "cmbParam",
                Location = new Point(120, 47),
                Width = 150,
                DropDownStyle = ComboBoxStyle.DropDownList // Garante que o usuário só possa escolher as opções
            };
            // Adiciona as opções ao ComboBox
            cmbParam.Items.AddRange(new string[] { "", "Documental", "Exame Físico" });
            var lblDataReg = new Label { Text = "Data Registro:", Location = new Point(300, 20), AutoSize = true };
            var dtpDataReg = new DateTimePicker { Name = "dtpDataReg", Location = new Point(410, 17), Width = 120, Format = DateTimePickerFormat.Short };
            var lblDataDef = new Label { Text = "Data Deferimento:", Location = new Point(300, 50), AutoSize = true };
            var dtpDataDef = new DateTimePicker { Name = "dtpDataDef", Location = new Point(410, 47), Width = 120, Format = DateTimePickerFormat.Short };

            tabPage.Controls.AddRange(new Control[] { lblLpco, txtLpco, lblParam, cmbParam, lblDataReg, dtpDataReg, lblDataDef, dtpDataDef });

            txtLpco.DataBindings.Add("Text", lpco, nameof(LpcoInfo.LPCO), false, DataSourceUpdateMode.OnPropertyChanged);
            cmbParam.DataBindings.Add("Text", lpco, nameof(LpcoInfo.ParametrizacaoLPCO), false, DataSourceUpdateMode.OnPropertyChanged);
            ConfigurarDatePickerNulavel(dtpDataReg, lpco.DataRegistroLPCO);
            ConfigurarDatePickerNulavel(dtpDataDef, lpco.DataDeferimentoLPCO);

            tabControl1.TabPages.Add(tabPage);
        }
        private void BtnNovoOrgaoAnuente_Click(object? sender, EventArgs e)
        {
            string? nomeOrgao = CBOrgaoAnuente.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(nomeOrgao)) { MessageBox.Show("Selecione um órgão."); return; }
            if (OrgaoAnuente.LPCO.Any(l => l.NomeOrgao == nomeOrgao)) { MessageBox.Show($"Já existe um LPCO para o órgão '{nomeOrgao}'."); return; }

            var novoLpco = new LpcoInfo { NomeOrgao = nomeOrgao };
            OrgaoAnuente.LPCO.Add(novoLpco);
            AdicionarAbaLpco(novoLpco);
            tabControl1.SelectTab(tabControl1.TabPages.Count - 1);
        }

        private void SalvarDadosDosControles()
        {
            this.ValidateChildren(); // Força a atualização de todos os bindings

            OrgaoAnuente.Inspecao = DTPdatadeinspecao.Checked ? DTPdatadeinspecao.Value : null;
            OrgaoAnuente.DataRegistro = DtpDataRegistro.Checked ? DtpDataRegistro.Value : null;

            // Salva os dados das abas de LPCO
            foreach (TabPage abaLpco in tabControl1.TabPages)
            {
                if (abaLpco.Tag is LpcoInfo lpco)
                {
                    if (abaLpco.Controls.Find("txtLpco", true).FirstOrDefault() is TextBox txtLpco) lpco.LPCO = txtLpco.Text;
                    if (abaLpco.Controls.Find("cmbParam", true).FirstOrDefault() is ComboBox cmbParam) lpco.ParametrizacaoLPCO = cmbParam.Text;
                    if (abaLpco.Controls.Find("dtpDataReg", true).FirstOrDefault() is DateTimePicker dtpReg) lpco.DataRegistroLPCO = dtpReg.Checked ? dtpReg.Value : null;
                    if (abaLpco.Controls.Find("dtpDataDef", true).FirstOrDefault() is DateTimePicker dtpDef) lpco.DataDeferimentoLPCO = dtpDef.Checked ? dtpDef.Value : null;
                }
            }
        }
        private void ConfigurarDatePickerNulavel(DateTimePicker dtp, DateTime? data)
        {
            dtp.ShowCheckBox = true;
            if (data.HasValue)
            {
                dtp.Checked = true;
                dtp.Value = data.Value;
                dtp.Format = DateTimePickerFormat.Short;
            }
            else
            {
                dtp.Checked = false;
                dtp.Format = DateTimePickerFormat.Custom;
                dtp.CustomFormat = " ";
            }
            dtp.ValueChanged += (s, e) => {
                if (s is DateTimePicker picker)
                {
                    picker.Format = picker.Checked ? DateTimePickerFormat.Short : DateTimePickerFormat.Custom;
                }
            };
        }
        private void BtnOK_Click(object? sender, EventArgs e)
        {
            SalvarDadosDosControles();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        private void CarregarAbasLpco()
        {
            tabControl1.TabPages.Clear(); // Limpa as abas de exemplo do designer
            foreach (var lpco in OrgaoAnuente.LPCO)
            {
                AdicionarAbaLpco(lpco);
            }
        }

        private void BtnCancelar_Click(object? sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
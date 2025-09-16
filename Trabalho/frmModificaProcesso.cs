using CLUSA;
using System.Diagnostics;

namespace Trabalho
{
    public partial class FrmModificaProcesso : Form
    {
        public Processo processo;
        public string? Modo;
        public bool Visualização;
        public List<LicencaImportacao> listaLis = new List<LicencaImportacao>();
        public FrmModificaProcesso()
        {
            InitializeComponent();
            processo = new();
        }

        private void FrmModificaProcesso_Load(object sender, EventArgs e)
        {
            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);


            SelecionarItemCheckedListBox(checkedListBox2, processo.FormaRecOriginais);
            SelecionarItensCheckedListBox(checkedListBox1, processo.DocRecebidos);

            if (Modo == "Editar") { TXTnr.Enabled = false; }

            if (Modo == "Adicionar")
            {
                btnCapa.Enabled = false;
                btnRelatorio.Enabled = false;
                DTPdataderegistrodi.Value = System.DateTime.Today;
                DTPdatadedesembaracodi.Value = System.DateTime.Today;
                DTPdatadecarregamentodi.Value = System.DateTime.Today;
                DTPdatadeinspecao.Value = System.DateTime.Today;
                DTPdatadeatracacao.Value = System.DateTime.Today;
                DTPdatadeembarque.Value = System.DateTime.Today;
                DTPDataRecOriginais.Value = System.DateTime.Today;
            }
            else if (Modo == "Visualizar")
            {
                Visualização = true;
            }
            btnRelatorio.MouseClick += btnRelatorio_MouseClick;

            CarregarLis(processo);
            PopularMarca();
            bsModificaProcesso.DataSource = processo;
            InicializarDateTimePickersComCheckbox();
            CarregarDateTimePickers(processo);

            if (Visualização) SetCamposSomenteLeitura(this);
        }
        // Carrega as LIs existentes de um processo
        private DateTime? GetDateIfChecked(DateTimePicker dtp)
            => dtp.Checked ? (DateTime?)dtp.Value : null;
        private void CarregarDateTimePickers(Processo p)
        {
            // Mapeamento de cada DTP ao par (data, flag)
            var mapeamento = new Dictionary<DateTimePicker, (DateTime? data, bool has)>()
            {
                { DTPdataderegistrodi,        (p.DataRegistroDI,        p.CheckDataRegistroDI) },
                { DTPdatadedesembaracodi,     (p.DataDesembaracoDI,     p.CheckDataDesembaracoDI) },
                { DTPdatadecarregamentodi,    (p.DataCarregamentoDI,    p.CheckDataCarregamentoDI) },
                { DTPdatadeinspecao,          (p.Inspecao,              p.CheckInspecao) },
                { DTPdatadeatracacao,         (p.DataDeAtracacao,       p.CheckDataDeAtracacao) },
                { DTPdatadeembarque,          (p.DataEmbarque,          p.CheckDataEmbarque) },
                { DTPDataRecOriginais,        (p.DataRecebOriginais,    p.CheckDataRecebOriginais) },
                { dtpDataMinuta,              (p.DataMinutaDI,          p.CheckDataMinutaDI) },
                { dtpVencimentoFMA,           (p.VencimentoFMA,         p.VencimentoFMA.HasValue) },
                { dtpVencimentoFreeTime,      (p.VencimentoFreeTime,    p.VencimentoFreeTime.HasValue) },
                { dtpVencimentoLI_LPCO,       (p.VencimentoLI_LPCO,     p.VencimentoLI_LPCO.HasValue) }
            };

            foreach (var kv in mapeamento)
            {
                var dtp = kv.Key;
                var (date, has) = kv.Value;

                // 1) Se quiser exibir checkbox interno (opcional)
                dtp.ShowCheckBox = true;

                // 2) Sincroniza o Checked com o banco
                dtp.Checked = has;

                // 3) Se tiver data, formata e atribui; senão, mantém em branco
                if (has && date.HasValue)
                {
                    dtp.Format = DateTimePickerFormat.Short;
                    dtp.Value = date.Value;
                }
                else
                {
                    dtp.Format = DateTimePickerFormat.Custom;
                    dtp.CustomFormat = " -";
                }
            }
        }
        private void SetCamposSomenteLeitura(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                switch (control)
                {
                    case TextBox textBox:
                        textBox.ReadOnly = true;
                        break;

                    case DateTimePicker:
                    case CheckBox:
                    case ComboBox:
                    case NumericUpDown:
                    case CheckedListBox:
                        control.Enabled = false;
                        break;
                }

                // Recursivamente trata controles compostos (GroupBox, Panel, etc.)
                if (control.HasChildren)
                {
                    SetCamposSomenteLeitura(control);
                }
            }
        }
        public void SelecionarItensCheckedListBox(CheckedListBox clb, string[] itensSelecionados)
        {
            for (int i = 0; i < clb.Items.Count; i++)
            {
                string item = clb.Items[i]?.ToString() ?? string.Empty;
                clb.SetItemChecked(i, itensSelecionados.Contains(item));
            }
        }
        private void SelecionarItemCheckedListBox(CheckedListBox clb, string itemParaSelecionar, bool selecaoUnica = true)
        {
            if (string.IsNullOrEmpty(itemParaSelecionar))
            {
                for (int i = 0; i < clb.Items.Count; i++)
                    clb.SetItemChecked(i, false);
                return;
            }

            for (int i = 0; i < clb.Items.Count; i++)
            {
                bool deveMarcar = clb.Items[i].ToString() == itemParaSelecionar;
                clb.SetItemChecked(i, deveMarcar || (!selecaoUnica && clb.GetItemChecked(i)));

                if (selecaoUnica && !deveMarcar)
                    clb.SetItemChecked(i, false);
            }
        }
        public string[] ObterItensSelecionados(CheckedListBox clb)
        {
            return clb.CheckedItems
                      .OfType<string>()
                      .ToArray();
        }
        private void DateTimePicker_OnValueChanged(object sender, EventArgs e)
        {
            var dtp = (DateTimePicker)sender;

            // 1) Ajusta o formato visual
            dtp.Format = dtp.Checked
                ? DateTimePickerFormat.Short
                : DateTimePickerFormat.Custom;
            if (!dtp.Checked)
                dtp.CustomFormat = "' -'";

            // 2) Descobre o nome da propriedade no seu modelo
            //    ex: dtp.Name = "DTPdatadeinspecao" → campo = "datadeinspecao"
            var campo = dtp.Name.Substring(3);

            // 3) Atualiza a propriedade DateTime? (DataX) 
            var propData = typeof(Processo).GetProperty(
                // supõe que o nome da prop é PascalCase igual ao suffix do DTP:
                char.ToUpper(campo[0]) + campo.Substring(1)
            );
            if (propData != null && propData.PropertyType == typeof(DateTime?))
            {
                propData.SetValue(processo, GetDateIfChecked(dtp));
            }

            // 4) Atualiza o flag CheckDataX (bool)
            var propCheck = typeof(Processo).GetProperty("Check" +
                char.ToUpper(campo[0]) + campo.Substring(1));
            if (propCheck != null && propCheck.PropertyType == typeof(bool))
            {
                propCheck.SetValue(processo, dtp.Checked);
            }
        }

        private void InicializarDateTimePickersComCheckbox()
        {
            // Liste aqui todos os seus DateTimePickers que devem ter checkbox interno
            var dtps = new[]
            {
                dtpDataMinuta,
                DTPdataderegistrodi,
                DTPdatadedesembaracodi,
                DTPdatadecarregamentodi,
                DTPdatadeinspecao,
                DTPdatadeatracacao,
                DTPdatadeembarque,
                DTPDataRecOriginais
            };

            foreach (var dtp in dtps)
            {
                dtp.ShowCheckBox = true;
                dtp.ValueChanged += DateTimePicker_OnValueChanged;
                // caso queira capturar também o uncheck via clique:
                dtp.MouseUp += (s, e2) => DateTimePicker_OnValueChanged(s, null);
            }
        }
        private void AtualizarPainelLi()
        {
            flpLis.Controls.Clear(); // flpLis é o seu FlowLayoutPanel

            foreach (var li in listaLis)
            {
                // Cria uma instância do nosso novo controle de exibição
                var displayControl = new LiDisplayControl();

                // Carrega os dados da LI no controle
                displayControl.CarregarDados(li);

                // Adiciona o controle preenchido ao painel
                flpLis.Controls.Add(displayControl);
            }
        }
        public void CarregarLis(Processo processo)
        {
            if (processo?.LI != null)
            {
                listaLis = processo.LI.ToList();
                AtualizarPainelLi();
            }
        }
        private void PopularMarca()
        {
            string marcaCompleta = processo.Marca;
            string[] modulosEspacos = new[] { "Sacos", "Caixas", "Pallets" };

            string numMarcaStr = "";
            string textoMarca = "";

            if (modulosEspacos.Any(m => marcaCompleta.EndsWith(" " + m)))
            {
                // Ex: "10 Sacos"
                int indexEspaco = marcaCompleta.LastIndexOf(' ');
                if (indexEspaco > 0)
                {
                    numMarcaStr = marcaCompleta.Substring(0, indexEspaco);
                    textoMarca = marcaCompleta.Substring(indexEspaco + 1);
                }
            }
            else if (marcaCompleta.Contains(" x "))
            {
                // Ex: "10 x 40 DRY"
                int indexX = marcaCompleta.IndexOf(" x ");
                numMarcaStr = marcaCompleta.Substring(0, indexX);
                textoMarca = marcaCompleta.Substring(indexX + 3);
            }

            // Converte e atribui
            if (decimal.TryParse(numMarcaStr, out decimal num))
                numMarca.Value = Math.Min(numMarca.Maximum, Math.Max(numMarca.Minimum, num)); // garante dentro do intervalo

            cbMarca.Text = textoMarca;

        }
        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAdiciona_Click(object sender, EventArgs e)
        {
            // Mapeamento direto dos controles para o objeto 'processo'
            processo.Importador = TXTimportador.Text;
            processo.Veiculo = txtVeiculo.Text;
            processo.Ref_USA = TXTnr.Text;
            processo.SR = TXTsr.Text;
            processo.Exportador = TXTexportador.Text;
            processo.Produto = TXTProduto.Text;
            processo.PortoDestino = TXTportodedestino.Text;
            processo.Origem = txtOrigem.Text;
            processo.FLO = TXTflo.Text;
            processo.Terminal = txtTerminal.Text;
            processo.FreeTime = (int)NUMfreetime.Value;
            processo.Conhecimento = txtConhecimento.Text;
            processo.Armador = txtArmador.Text;
            processo.DI = TXTdi.Text;
            processo.ParametrizacaoDI = CBparametrizacaodi.Text;
            processo.HistóricoDoProcesso = TXTstatusdoprocesso.Text;
            processo.Pendencia = TXTpendencia.Text;
            processo.Amostra = CBamostra.Checked;
            processo.Desovado = CBdesovado.Checked;

            // --- LÓGICA SIMPLIFICADA PARA DATAS ---
            // A propriedade do processo recebe o valor do DTP se estiver marcado, senão recebe null.
            processo.DataRegistroDI = DTPdataderegistrodi.Checked ? DTPdataderegistrodi.Value : null;
            processo.DataDesembaracoDI = DTPdatadedesembaracodi.Checked ? DTPdatadedesembaracodi.Value : null;
            processo.DataCarregamentoDI = DTPdatadecarregamentodi.Checked ? DTPdatadecarregamentodi.Value : null;
            processo.Inspecao = DTPdatadeinspecao.Checked ? DTPdatadeinspecao.Value : null;
            processo.DataDeAtracacao = DTPdatadeatracacao.Checked ? DTPdatadeatracacao.Value : null;
            processo.DataEmbarque = DTPdatadeembarque.Checked ? DTPdatadeembarque.Value : null;
            processo.DataRecebOriginais = DTPDataRecOriginais.Checked ? DTPDataRecOriginais.Value : null;
            processo.DataMinutaDI = dtpDataMinuta.Checked ? dtpDataMinuta.Value : null;

            // --- LÓGICA DE LISTAS E CÁLCULOS ---
            processo.LI = listaLis;
            processo.DocRecebidos = ObterItensSelecionados(checkedListBox1);
            processo.FormaRecOriginais = checkedListBox2.CheckedItems.Count > 0
                    ? checkedListBox2.CheckedItems[0]?.ToString() ?? string.Empty
                    : string.Empty;

            // Constrói a string da Marca
            processo.Marca = (new[] { "Sacos", "Caixas", "Pallets" }.Contains(cbMarca.Text))
                ? $"{numMarca.Value} {cbMarca.Text}"
                : $"{numMarca.Value} x {cbMarca.Text}";

            // O DialogResult é definido para fechar o form e sinalizar que a operação foi um sucesso
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void CBmapa_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void checkedListBox2_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            for (int i = 0; i < checkedListBox2.Items.Count; i++)
            {
                if (i != e.Index)
                {
                    checkedListBox2.SetItemChecked(i, false);
                }
            }
        }

        private void btnRelatorio_Click(object sender, EventArgs e)
        {
            string referencia = TXTnr.Text;

            // 1) Cria sem using
            var progressForm = new ProgressForm();
            progressForm.Show(this);       // exibe modeless, com o próprio Form como owner

            Task.Run(() =>
            {
                string pdfPath = "";
                string mensagemErro = null;

                try
                {
                    pdfPath = PythonRunner.ExecutarRelatorio(referencia).Trim();
                }
                catch (Exception ex)
                {
                    mensagemErro = $"Erro durante exportação: {ex.Message}";
                }

                Invoke(new Action(() =>
                {
                    progressForm.Close();
                    progressForm.Dispose();

                    if (mensagemErro != null)
                    {
                        MessageBox.Show(mensagemErro, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    var resp = MessageBox.Show(
                        "Exportação concluída. Deseja abrir o PDF?",
                        "Resultado",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question
                    );

                    if (resp == DialogResult.Yes && File.Exists(pdfPath))
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = pdfPath,
                            UseShellExecute = true
                        });
                    }

                    this.Close();
                }));
            });
        }

        private void btnRelatorio_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                // Caminho da pasta do relatório
                string pasta = @"C:\UsaDespachos\Docs\Relatorio"; // Substitua pelo caminho real

                if (!string.IsNullOrEmpty(pasta) && System.IO.Directory.Exists(pasta))
                {
                    Process.Start("explorer.exe", pasta);
                }
                else
                {
                    MessageBox.Show("Pasta do relatório não encontrada.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnCapa_Click(object sender, EventArgs e)
        {
            var frm = new FrmModificaCapa();
            if (processo.Capa != null)
                frm.capa = processo.Capa;
            frm.Modo = this.Modo;
            frm.ref_usa = processo.Ref_USA;
            frm.Visualizacao = (this.Modo == "Visualizar");

            if (frm.ShowDialog(this) == DialogResult.OK)
            {
                if (processo.Capa == null)
                    processo.Capa = new Capa();

                processo.Capa = frm.capa;
            }
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void label32_Click(object sender, EventArgs e)
        {

        }
    }
}

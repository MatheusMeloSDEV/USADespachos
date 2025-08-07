using CLUSA;
using System.Diagnostics;

namespace Trabalho
{
    public partial class FrmModificaProcesso : Form, ILiHandler
    {
        public Processo processo;
        public string? Modo;
        public bool Visualização;
        public List<LiInfo> listaLis = new List<LiInfo>();
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
                { DTPDataRecOriginais,        (p.DataRecebOriginais,    p.CheckDataRecebOriginais) }
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
        public bool ContainsLi(string numeroLi)
        {
            return listaLis.Any(li => li.Numero == numeroLi);
        }

        public void AdicionarLi(LiInfo li)
        {
            listaLis.Add(li);
            AtualizarPainelLi();
        }

        public void AtualizarLi(string numeroLi, LiInfo liAtualizada)
        {
            var existente = listaLis.FirstOrDefault(li => li.Numero == numeroLi);
            if (existente != null)
            {
                existente.OrgaosAnuentes = liAtualizada.OrgaosAnuentes;
                existente.NCM = liAtualizada.NCM;
                existente.DataRegistroLI = liAtualizada.DataRegistroLI;
                existente.CheckDataRegistroLI = liAtualizada.CheckDataRegistroLI;
                existente.LPCO = liAtualizada.LPCO;
                existente.DataRegistroLPCO = liAtualizada.DataRegistroLPCO;
                existente.CheckDataRegistroLPCO = liAtualizada.CheckDataRegistroLPCO;
                existente.DataDeferimentoLPCO = liAtualizada.DataDeferimentoLPCO;
                existente.CheckDataDeferimentoLPCO = liAtualizada.CheckDataDeferimentoLPCO;
                existente.ParametrizacaoLPCO = liAtualizada.ParametrizacaoLPCO;

                AtualizarPainelLi();
            }
        }

        public void RemoverLi(string numeroLi)
        {
            var li = listaLis.FirstOrDefault(x => x.Numero == numeroLi);
            if (li != null)
            {
                listaLis.Remove(li);
                AtualizarPainelLi();
            }
        }
        private void AtualizarPainelLi()
        {
            flpLis.Controls.Clear();
            flpLis.FlowDirection = FlowDirection.LeftToRight;
            flpLis.WrapContents = true;
            flpLis.AutoScroll = true;

            int panelWidth = (flpLis.ClientSize.Width - SystemInformation.VerticalScrollBarWidth) / 2 - 4;
            int panelHeight = 40;

            foreach (var li in listaLis)
            {
                var panel = new Panel
                {
                    Size = new Size(panelWidth, panelHeight),
                    BorderStyle = BorderStyle.FixedSingle,
                    Margin = new Padding(2)
                };

                var lbl = new Label
                {
                    Text = $"LI: {li.Numero}",
                    AutoSize = true,
                    Location = new Point(5, 10)
                };

                var somenteVisualizacao = this.Visualização;
                var btnVisualizar = new Button
                {
                    Text = somenteVisualizacao ? "Visualizar" : "Editar",
                    Size = new Size(75, 25),
                    Location = new Point(panel.Width - 80, 7),
                    Anchor = AnchorStyles.Top | AnchorStyles.Right
                };

                btnVisualizar.Click += (s, e) =>
                {
                    var frm = new frmLi(
                        li.Numero, li.OrgaosAnuentes, li.NCM, li.LPCO,
                        li.DataRegistroLI, li.CheckDataRegistroLI,
                        li.DataRegistroLPCO, li.CheckDataRegistroLPCO,
                        li.DataDeferimentoLPCO, li.CheckDataDeferimentoLPCO,
                        li.ParametrizacaoLPCO,
                        somenteVisualizacao
                    );
                    frm.Owner = this;
                    frm.ShowDialog(this);
                };

                panel.Controls.Add(lbl);
                panel.Controls.Add(btnVisualizar);
                flpLis.Controls.Add(panel);
            }
        }
        private void btnAdicionarLi_Click(object sender, EventArgs e)
        {
            var frm = new frmLi();
            frm.Owner = this;
            frm.ShowDialog();
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
            processo.TMapa = CBmapa.Checked;
            processo.TAnvisa = CBanvisa.Checked;
            processo.TDecex = CBdecex.Checked;
            processo.TIbama = CBibama.Checked;
            processo.TImetro = CBimetro.Checked;

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
            processo.FreeTime = int.Parse(NUMfreetime.Text);
            processo.VencimentoFreeTime = DataHelper.CalcularVencimento(DTPdatadeatracacao.Value, int.Parse(NUMfreetime.Text));
            processo.VencimentoFMA = DataHelper.CalcularVencimento(DTPdatadeatracacao.Value, 85);
            if (listaLis.Count > 0)
            {
                processo.VencimentoLI_LPCO = DataHelper.CalcularVencimento(listaLis[0].DataRegistroLI, 85);
            }
            processo.Conhecimento = txtConhecimento.Text;
            processo.Armador = txtArmador.Text;
            processo.LI = listaLis;

            processo.DI = TXTdi.Text;
            processo.ParametrizacaoDI = CBparametrizacaodi.Text;

            processo.DocRecebidos = ObterItensSelecionados(checkedListBox1);
            processo.FormaRecOriginais =
                checkedListBox2.CheckedItems.Count > 0
                    ? checkedListBox2.CheckedItems[0]?.ToString() ?? string.Empty
                    : string.Empty;

            processo.StatusDoProcesso = TXTstatusdoprocesso.Text;
            processo.Pendencia = TXTpendencia.Text;
            processo.Amostra = CBamostra.Checked;
            processo.Desovado = CBdesovado.Checked;

            if (DTPdataderegistrodi.Checked)
            {
                processo.DataRegistroDI = DTPdataderegistrodi.Value;
                processo.CheckDataRegistroDI = true;
            }
            else
            {
                processo.DataRegistroDI = default;
                processo.CheckDataRegistroDI = false;
            }

            if (DTPdataderegistrodi.Checked)
            {
                processo.DataRegistroDI = DTPdataderegistrodi.Value;
                processo.CheckDataRegistroDI = true;
            }
            else
            {
                processo.DataRegistroDI = default;
                processo.CheckDataRegistroDI = false;
            }

            // Registro DI
            if (DTPdataderegistrodi.Checked)
            {
                processo.DataRegistroDI = DTPdataderegistrodi.Value;
                processo.CheckDataRegistroDI = true;
            }
            else
            {
                processo.DataRegistroDI = default;
                processo.CheckDataRegistroDI = false;
            }

            // Desembaraço DI
            if (DTPdatadedesembaracodi.Checked)
            {
                processo.DataDesembaracoDI = DTPdatadedesembaracodi.Value;
                processo.CheckDataDesembaracoDI = true;
            }
            else
            {
                processo.DataDesembaracoDI = default;
                processo.CheckDataDesembaracoDI = false;
            }

            // Carregamento DI
            if (DTPdatadecarregamentodi.Checked)
            {
                processo.DataCarregamentoDI = DTPdatadecarregamentodi.Value;
                processo.CheckDataCarregamentoDI = true;
            }
            else
            {
                processo.DataCarregamentoDI = default;
                processo.CheckDataCarregamentoDI = false;
            }

            // Inspeção
            if (DTPdatadeinspecao.Checked)
            {
                processo.Inspecao = DTPdatadeinspecao.Value;
                processo.CheckInspecao = true;
            }
            else
            {
                processo.Inspecao = default;
                processo.CheckInspecao = false;
            }

            // Atracação
            if (DTPdatadeatracacao.Checked)
            {
                processo.DataDeAtracacao = DTPdatadeatracacao.Value;
                processo.CheckDataDeAtracacao = true;
            }
            else
            {
                processo.DataDeAtracacao = default;
                processo.CheckDataDeAtracacao = false;
            }

            // Embarque
            if (DTPdatadeembarque.Checked)
            {
                processo.DataEmbarque = DTPdatadeembarque.Value;
                processo.CheckDataEmbarque = true;
            }
            else
            {
                processo.DataEmbarque = default;
                processo.CheckDataEmbarque = false;
            }

            // Recebimento de Originais
            if (DTPDataRecOriginais.Checked)
            {
                processo.DataRecebOriginais = DTPDataRecOriginais.Value;
                processo.CheckDataRecebOriginais = true;
            }
            else
            {
                processo.DataRecebOriginais = default;
                processo.CheckDataRecebOriginais = false;
            }

            switch (cbMarca.Text)
            {
                case "Sacos":
                case "Caixas":
                case "Pallets":
                    processo.Marca = $"{numMarca.Value} {cbMarca.Text}";
                    break;

                default:
                    processo.Marca = $"{numMarca.Value} x {cbMarca.Text}";
                    break;
            }
            DialogResult confirmResult;

            switch (Modo)
            {
                case "Editar":
                    confirmResult = MessageBox.Show(
                            $"Tem certeza de que deseja editar o processo {processo.Ref_USA}?",
                            "Confirmação",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);
                    if (confirmResult == DialogResult.Yes) { DialogResult = DialogResult.OK; }
                    break;
                case "Adicionar":
                    confirmResult = MessageBox.Show(
                            $"Tem certeza de que deseja adicionar o novo processo?",
                            "Confirmação",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);
                    if (confirmResult == DialogResult.Yes) { DialogResult = DialogResult.OK; }
                    break;
                case "Visualizar":
                    DialogResult = DialogResult.OK;
                    break;
            }
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
            if (processo.Capa != null && processo.Capa.Count > 0)
                frm.capa = processo.Capa[0];
            frm.Modo = this.Modo;
            frm.ref_usa = processo.Ref_USA;
            frm.Visualizacao = (this.Modo == "Visualizar");

            if (frm.ShowDialog(this) == DialogResult.OK)
            {
                if (processo.Capa == null)
                    processo.Capa = new List<Capa>();

                if (processo.Capa.Count > 0)
                    processo.Capa[0] = frm.capa;
                else
                    processo.Capa.Add(frm.capa);
            }
        }

    }
}

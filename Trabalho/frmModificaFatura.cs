using CLUSA;
using System.Diagnostics;
using System.Globalization;

namespace Trabalho
{
    public partial class frmModificaFatura : Form
    {
        RepositorioFatura _repositorio;
        public Fatura FaturaAtual { get; set; }

        private string[] nomesDocumentos = Array.Empty<string>();
        private string[] numerosDocumentos = Array.Empty<string>();

        List<Agencia> Agencias = new List<Agencia>();

        private Button btnAddDoc = null!;

        public frmModificaFatura(Fatura fatura)
        {
            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            _repositorio = new();
            InitializeComponent();
            Agencias = fatura.Agencias;
            foreach (var ag in Agencias)
                AdicionarPainelAgencia(ag.Numero, ag.Custo);
            bindingSource1.DataSource = fatura;
            FaturaAtual = fatura;
            InicializarControlesDocumentos();
            PreencherDocumentosExistentes();
        }
        private void InicializarControlesDocumentos()
        {
            // Cria o botão de adicionar
            btnAddDoc = new Button
            {
                Text = "+",
                Width = 30,
                Height = 30,
                Margin = new Padding(5)
            };
            btnAddDoc.Click += BtnAddDoc_Click;

            // Adiciona o botão + ao FlowLayoutPanel
            flpDocumentos.Controls.Add(btnAddDoc);
        }
        private DateTime? GetDateIfChecked(DateTimePicker dtp)
            => dtp.Checked ? (DateTime?)dtp.Value : null;
        private void BtnAddDoc_Click(object? sender, EventArgs e)
        {
            // Abre o formulário para adicionar um novo documento
            using (frmDocumento frm = new frmDocumento())
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    // Obtém os valores do form
                    string nomeDoc = frm.NomeDocumento;
                    string numeroDoc = frm.NumeroDocumento;

                    // 1) Redimensiona e adiciona aos arrays
                    int oldLen = nomesDocumentos.Length;
                    Array.Resize(ref nomesDocumentos, oldLen + 1);
                    nomesDocumentos[oldLen] = nomeDoc;

                    int oldNumLen = numerosDocumentos.Length;
                    Array.Resize(ref numerosDocumentos, oldNumLen + 1);
                    numerosDocumentos[oldNumLen] = numeroDoc;

                    // 2) Cria e configura o botão
                    var docBtn = new Button
                    {
                        Text = nomeDoc,
                        AutoSize = true,
                        Margin = new Padding(5),
                        Tag = numeroDoc
                    };
                    docBtn.Click += (s, ev) =>
                    {
                        MessageBox.Show(
                            $"Documento: {nomeDoc}\nNúmero: {numeroDoc}",
                            "Detalhes",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
                    };

                    // 3) Adiciona o botão ao FlowLayoutPanel
                    flpDocumentos.Controls.Add(docBtn);

                    // 4) Garante que o botão '+' fique sempre na primeira posição
                    if (!flpDocumentos.Controls.Contains(btnAddDoc))
                    {
                        flpDocumentos.Controls.Add(btnAddDoc);
                    }
                    flpDocumentos.Controls.SetChildIndex(btnAddDoc, 0);
                }
            }
        }
        private void AplicarMascaraMoeda(object sender, EventArgs e)
        {
            // Garante que o sender é um TextBox
            if (!(sender is TextBox txt))
                return;

            // Se o texto estiver vazio, zera e posiciona cursor
            if (string.IsNullOrWhiteSpace(txt.Text))
            {
                txt.Text = "0,00";
                txt.SelectionStart = txt.Text.Length;
                return;
            }

            // Remove tudo que não for dígito
            string somenteDigitos = new string(txt.Text.Where(char.IsDigit).ToArray());

            if (string.IsNullOrEmpty(somenteDigitos))
            {
                txt.Text = "0,00";
                txt.SelectionStart = txt.Text.Length;
                return;
            }

            // Converte para decimal considerando 2 casas (centavos)
            if (!decimal.TryParse(somenteDigitos, out decimal valor))
            {
                // Se falhar, zera
                txt.Text = "0,00";
                txt.SelectionStart = txt.Text.Length;
                return;
            }
            valor /= 100;

            // Formata no padrão pt-BR como moeda
            txt.Text = valor.ToString("C", CultureInfo.GetCultureInfo("pt-BR"));
            txt.SelectionStart = txt.Text.Length;
        }
        private void PreencherDocumentosExistentes()
        {
            // 1. Limpa tudo
            flpDocumentos.Controls.Clear();

            // 2. Adiciona o btnAddDoc primeiro (índice 0)
            if (btnAddDoc != null)
            {
                flpDocumentos.Controls.Add(btnAddDoc);
            }
            else
            {
                Debug.WriteLine("⚠ btnAddDoc é null!");
            }

            // 3. Adiciona os botões dos documentos
            for (int i = 0; i < nomesDocumentos.Length; i++)
            {
                string nomeDoc = nomesDocumentos[i];
                string numeroDoc = numerosDocumentos[i];

                var docBtn = new Button
                {
                    Text = nomeDoc,
                    AutoSize = true,
                    Margin = new Padding(5),
                    Tag = numeroDoc,
                };

                docBtn.Click += (s, ev) =>
                {
                    MessageBox.Show(
                        $"Documento: {nomeDoc}\nNúmero: {numeroDoc}",
                        "Detalhes",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                };

                flpDocumentos.Controls.Add(docBtn);
            }
        }
        private void AdicionarPainelAgencia(string numero, decimal custo)
        {
            var panel = new Panel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(5),
                Margin = new Padding(3),
                BorderStyle = BorderStyle.FixedSingle
            };

            var lbl = new Label
            {
                Text = $"{numero} - R$ {custo:F2}",
                AutoSize = true,
                Margin = new Padding(0, 10, 6, 0)
            };

            var btnRemover = new Button
            {
                Text = "x",
                Size = new Size(25, 25),
                BackColor = Color.LightCoral,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(0, 4, 0, 0)
            };
            btnRemover.FlatAppearance.BorderSize = 0;

            btnRemover.Click += (s, e) =>
            {
                flowLayoutPanel1.Controls.Remove(panel);
                Agencias.RemoveAll(a => a.Numero == numero && a.Custo == custo);
            };

            var innerFlow = new FlowLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false
            };

            innerFlow.Controls.Add(lbl);
            innerFlow.Controls.Add(btnRemover);
            panel.Controls.Add(innerFlow);
            flowLayoutPanel1.Controls.Add(panel);
        }

        private void btnAdicionarAgencia_Click(object sender, EventArgs e)
        {
            using var frm = new DetalhesAgencia();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                var nova = (frm.NumeroAgencia, frm.PrecoCusto);
                Agencias.Add(new Agencia { Numero = nova.NumeroAgencia, Custo = nova.PrecoCusto });
                AdicionarPainelAgencia(nova.NumeroAgencia, nova.PrecoCusto);
            }
        }

        private async void btnSalvar_Click(object sender, EventArgs e)
        {
            try
            {
                // --- helpers de parsing ---
                float ParseFloat(TextBox tb)
                    => float.TryParse(tb.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out var v) ? v : 0f;

                decimal ParseCurrency(TextBox tb)
                {
                    // Tenta converter toda a string, incluindo "R$", parênteses, sinal, separadores
                    if (decimal.TryParse(
                            tb.Text,
                            NumberStyles.Currency,
                            CultureInfo.GetCultureInfo("pt-BR"),
                            out var valor))
                    {
                        return valor;
                    }
                    return 0m;
                }

                // --- atualização do objeto ---
                FaturaAtual.Quantidade = ParseFloat(txtQuantidade);
                FaturaAtual.ValRecebidos = ParseCurrency(txtValRecebidos);
                FaturaAtual.DataRecebimento = dtpDataRecebimento.Checked ? dtpDataRecebimento.Value : (DateTime?)null;

                // Despesas Aduaneiras
                FaturaAtual.ImpostoImportacao = ParseCurrency(txtImpostoImportacao);
                FaturaAtual.IPI = ParseCurrency(txtIPI);
                FaturaAtual.DI_ADICAO = ParseCurrency(txtDI_ADICAO);     // controle correto
                FaturaAtual.PIS_PASEP = ParseCurrency(txtPIS);
                FaturaAtual.COFINS = ParseCurrency(txtCONFINS);    // note o nome correto txtCOFINS
                FaturaAtual.MULTA_LI = ParseCurrency(txtMultaLI);
                FaturaAtual.ICMS = ParseCurrency(txtICMS);

                FaturaAtual.Agencias = Agencias;
                FaturaAtual.ArmazenagemN = txtArmazenagemN.Text;
                FaturaAtual.ArmazenagemP = ParseCurrency(txtArmazenagemP);

                // Outras Despesas
                FaturaAtual.FreteMaritimoN = txtFreteMaritimoN.Text;
                FaturaAtual.FreteMaritimoP = ParseCurrency(txtFreteMaritimoP);
                FaturaAtual.Marinha_MercanteN = txtMarinhaMercanteN.Text;
                FaturaAtual.Marinha_MercanteP = ParseCurrency(txtMarinhaMercanteP);
                FaturaAtual.GRUANVISAN = txtGRUAnvisaN.Text;
                FaturaAtual.GRUANVISAP = ParseCurrency(txtGRUAnvisaP);
                FaturaAtual.LiCancelada_IndeferidaN = txtLiCanceladaN.Text;
                FaturaAtual.LiCancelada_IndeferidaP = ParseCurrency(txtLiCanceladaP);
                FaturaAtual.ExpedienteLiCanceladaN = txtExpedienteLiN.Text;
                FaturaAtual.ExpedienteLiCanceladaP = ParseCurrency(txtExpedienteLiP);
                FaturaAtual.EncaminhamentoAmostrasN = txtEncaminhamentoAmostrasN.Text;
                FaturaAtual.EncaminhamentoAmostrasP = ParseCurrency(txtEncaminhamentoAmostrasP);
                FaturaAtual.DarfAnvisaN = txtDarfAnvisaN.Text;
                FaturaAtual.DarfAnvisaP = ParseCurrency(txtDarfAnvisaP);
                FaturaAtual.MotoboyN = txtMotoboyN.Text;
                FaturaAtual.MotoboyP = ParseCurrency(txtMotoboyP);
                FaturaAtual.LiP = ParseCurrency(txtLiP);
                FaturaAtual.DespesasDesembaracoN = txtDespesasDesembaracoN.Text;
                FaturaAtual.DespesasDesembaracoP = ParseCurrency(txtDespesasDesembaracoP);
                FaturaAtual.Expediente = ParseCurrency(txtExpedienteP);
                FaturaAtual.HD = ParseCurrency(txtHDP);
                FaturaAtual.Cartorio = ParseCurrency(txtCartorioP);

                // Documentos anexos
                FaturaAtual.NomesDocumentosAnexos = nomesDocumentos.ToArray();
                FaturaAtual.NumeroDocumentosAnexos = numerosDocumentos.ToArray();

                // Totais e fechamento
                FaturaAtual.TotalDespesas = ParseCurrency(txtTotalDespesas);
                FaturaAtual.NComissao = ParseCurrency(txtComissaoP);
                FaturaAtual.SubTotal = ParseCurrency(txtSubTotal);
                FaturaAtual.Adiantamento = ParseCurrency(txtAdiantamento);
                FaturaAtual.Saldo = ParseCurrency(txtSaldo);

                // Tipo de finalização
                FaturaAtual.TipoFinalizacao =
                    FaturaAtual.Saldo < 0 ? "S/FAVOR" : "N/FAVOR";

                // --- grava no MongoDB ---
                await _repositorio.UpdateAsync(FaturaAtual);

                MessageBox.Show("Fatura atualizada com sucesso!", "Sucesso",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar: {ex.Message}", "Erro",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCalcularTotal_Click(object sender, EventArgs e)
        {
            // Cultura pt-BR para parsing e formatação
            var culture = CultureInfo.GetCultureInfo("pt-BR");

            // Função local para converter texto em R$ para decimal
            decimal Parse(string text)
            {
                if (decimal.TryParse(text, NumberStyles.Currency, culture, out var v))
                    return v;
                return 0m;
            }

            // 1) Soma de todas as variáveis terminadas em “P”
            decimal totalDespesas =
                  Parse(txtImpostoImportacao.Text)
                + Parse(txtIPI.Text)
                + Parse(txtDI_ADICAO.Text)
                + Parse(txtPIS.Text)
                + Parse(txtCONFINS.Text)
                + Parse(txtMultaLI.Text)
                + Parse(txtICMS.Text)
                + Parse(txtArmazenagemP.Text)
                + Parse(txtFreteMaritimoP.Text)
                + Parse(txtMarinhaMercanteP.Text)
                + Parse(txtGRUAnvisaP.Text)
                + Parse(txtLiCanceladaP.Text)
                + Parse(txtExpedienteLiP.Text)
                + Parse(txtEncaminhamentoAmostrasP.Text)
                + Parse(txtDarfAnvisaP.Text)
                + Parse(txtMotoboyP.Text)
                + Parse(txtLiP.Text)
                + Parse(txtDespesasDesembaracoP.Text)
                + Parse(txtExpedienteP.Text)
                + Parse(txtHDP.Text)
                + Parse(txtCartorioP.Text);

            txtTotalDespesas.Text = totalDespesas.ToString("C", culture);

            // 2) Adiantamento = ValRecebidos
            decimal adiantamento = Parse(txtValRecebidos.Text);
            txtAdiantamento.Text = adiantamento.ToString("C", culture);

            // 3) Subtotal = TotalDespesas + Comissão
            decimal comissao = Parse(txtComissaoP.Text);
            decimal subTotal = totalDespesas + comissao;
            txtSubTotal.Text = subTotal.ToString("C", culture);

            // 4) Saldo = (TotalDespesas + Comissão) - Adiantamento
            decimal saldo = subTotal - adiantamento;
            txtSaldo.Text = saldo.ToString("C", culture);

            if (saldo < 0)
            {
                txtTipoFinalizacao.Text = "S/FAVOR";
                txtTipoFinalizacao.BackColor = Color.Red;
            }
            else
            {
                txtTipoFinalizacao.Text = "N/FAVOR";
                txtTipoFinalizacao.BackColor = Color.Lime;
            }
            btnOK.Enabled = true;
        }

        private void frmModificaFatura_Load(object sender, EventArgs e)
        {
            // 1) Data de Atracação
            if (FaturaAtual.DataAtracacao.HasValue)
            {
                dtpDataAtracação.Format = DateTimePickerFormat.Short;
                dtpDataAtracação.Value = FaturaAtual.DataAtracacao.Value;
            }
            else
            {
                dtpDataAtracação.Format = DateTimePickerFormat.Custom;
                dtpDataAtracação.CustomFormat = "' -'";
            }

            // 2) Data de Desembaraço DI
            if (FaturaAtual.DataDesembaracoDI.HasValue)
            {
                dtpdataDesembaracoDI.Format = DateTimePickerFormat.Short;
                dtpdataDesembaracoDI.Value = FaturaAtual.DataDesembaracoDI.Value;
            }
            else
            {
                dtpdataDesembaracoDI.Format = DateTimePickerFormat.Custom;
                dtpdataDesembaracoDI.CustomFormat = "' -'";
            }

            // 3) Data do DI
            if (FaturaAtual.DAtaDI.HasValue)
            {
                dtpDI.Format = DateTimePickerFormat.Short;
                dtpDI.Value = FaturaAtual.DAtaDI.Value;
            }
            else
            {
                dtpDI.Format = DateTimePickerFormat.Custom;
                dtpDI.CustomFormat = "' -'";
            }
            InicializarDateTimePickersComCheckbox();
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
                propData.SetValue(FaturaAtual, GetDateIfChecked(dtp));
            }

            // 4) Atualiza o flag CheckDataX (bool)
            var propCheck = typeof(Processo).GetProperty("Check" +
                char.ToUpper(campo[0]) + campo.Substring(1));
            if (propCheck != null && propCheck.PropertyType == typeof(bool))
            {
                propCheck.SetValue(FaturaAtual, dtp.Checked);
            }
        }

        private void InicializarDateTimePickersComCheckbox()
        {
            // Liste aqui todos os seus DateTimePickers que devem ter checkbox interno
            var dtps = new[]
            {
            dtpDataAtracação,
            dtpdataDesembaracoDI,
            dtpDataRecebimento,
            dtpDI
            };

            foreach (var dtp in dtps)
            {
                dtp.ShowCheckBox = true;
                dtp.ValueChanged += DateTimePicker_OnValueChanged;
                // caso queira capturar também o uncheck via clique:
                dtp.MouseUp += (s, e2) => DateTimePicker_OnValueChanged(s, null);
            }
        }
        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

    }
}

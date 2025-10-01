using CLUSA;
using System.Diagnostics;
using System.Globalization;

namespace Trabalho
{
    public partial class frmModificaFatura : Form
    {
        public Fatura Fatura { get; set; }
        private readonly RepositorioFatura _repositorioFatura;
        private List<Agencia> _agencias = new();
        private Button _btnAddDoc = new();
        private bool _dadosForamAlterados = false;
        public frmModificaFatura(Fatura faturaParaEditar)
        {
            InitializeComponent();
            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            _repositorioFatura = new RepositorioFatura();

            // Guarda a referência ao objeto que será modificado.
            this.Fatura = faturaParaEditar;

            // O evento Load agora é usado para carregar os dados na tela.
            
        }
        private void FrmModificaFatura_Load(object? sender, EventArgs e)
        {
            try
            {
                // Vincula o objeto Fatura que recebemos ao BindingSource.
                BsFatura.DataSource = Fatura;
                 
                // Configura os controles com lógica especial.
                CarregarControlesDeData();
                CarregarAgencias();
                CarregarDocumentos();
                AnexarEventoDeAlteracao(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar os dados da fatura: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
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
        private void frmModificaFatura_FormClosing(object? sender, FormClosingEventArgs e)
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
        private void CarregarDocumentos()
        {
            // 1. Limpa tudo, exceto o botão de adicionar que será recriado.
            flpDocumentos.Controls.Clear();

            // 2. Cria e posiciona o botão "+"
            _btnAddDoc = new Button { Text = "+", Width = 30, Height = 30, Margin = new Padding(5) };
            _btnAddDoc.Click += BtnAddDoc_Click;
            flpDocumentos.Controls.Add(_btnAddDoc);

            // 3. Adiciona os botões para os documentos que já existem na Fatura
            if (Fatura.NomesDocumentosAnexos != null && Fatura.NumeroDocumentosAnexos != null)
            {
                for (int i = 0; i < Fatura.NomesDocumentosAnexos.Length; i++)
                {
                    // Garante que não haja inconsistência nos arrays
                    if (i < Fatura.NumeroDocumentosAnexos.Length)
                    {
                        AdicionarBotaoDocumento(Fatura.NomesDocumentosAnexos[i], Fatura.NumeroDocumentosAnexos[i]);
                    }
                }
            }
        }
        private void BtnAddDoc_Click(object? sender, EventArgs e)
        {
            using (var frm = new frmDocumento())
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    string nomeDoc = frm.NomeDocumento;
                    string numeroDoc = frm.NumeroDocumento;

                    // Adiciona os novos dados às listas na memória
                    Fatura.NomesDocumentosAnexos = Fatura.NomesDocumentosAnexos.Append(nomeDoc).ToArray();
                    Fatura.NumeroDocumentosAnexos = Fatura.NumeroDocumentosAnexos.Append(numeroDoc).ToArray();

                    // Adiciona o botão para o novo documento na tela
                    AdicionarBotaoDocumento(nomeDoc, numeroDoc);
                }
            }
        }
        private void AdicionarBotaoDocumento(string nomeDoc, string numeroDoc)
        {
            var docBtn = new Button
            {
                Text = nomeDoc,
                AutoSize = true,
                Margin = new Padding(5),
                Tag = numeroDoc
            };

            // Evento para mostrar detalhes ao clicar
            docBtn.Click += (s, ev) => {
                MessageBox.Show($"Documento: {nomeDoc}\nNúmero: {numeroDoc}", "Detalhes", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            // Adiciona o botão antes do botão "+"
            int buttonIndex = flpDocumentos.Controls.GetChildIndex(_btnAddDoc);
            flpDocumentos.Controls.Add(docBtn);
            flpDocumentos.Controls.SetChildIndex(docBtn, buttonIndex);
        }
        private void SalvarDadosDosControles()
        {
            BsFatura.EndEdit();
            this.ValidateChildren();

            // Salva as datas dos DateTimePickers.
            Fatura.DataAtracacao = dtpDataAtracação.Checked ? dtpDataAtracação.Value : null;
            Fatura.DataDesembaracoDI = dtpdataDesembaracoDI.Checked ? dtpdataDesembaracoDI.Value : null;
            Fatura.DAtaDI = dtpDI.Checked ? dtpDI.Value : null;
            Fatura.DataRecebimento = dtpDataRecebimento.Checked ? dtpDataRecebimento.Value : null;

            // Salva as listas que são gerenciadas manualmente.
            Fatura.Agencias = _agencias;
            // A lógica de documentos foi simplificada para usar um único FlowLayoutPanel.
        }
        private async void btnOK_Click(object? sender, EventArgs e)
        {
            try
            {
                SalvarDadosDosControles();

                // Chama o repositório para salvar a fatura no banco de dados.
                await _repositorioFatura.UpdateAsync(Fatura);

                MessageBox.Show("Fatura salva com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                _dadosForamAlterados = false;
                this.Text = this.Text.Replace("*", "");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar a fatura: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void CarregarAgencias()
        {
            _agencias = Fatura.Agencias ?? new List<Agencia>();
            flowLayoutPanel1.Controls.Clear();
            foreach (var ag in _agencias)
            {
                AdicionarPainelAgencia(ag);
            }
        }
        private void CarregarControlesDeData()
        {
            // Mapeia cada controle de data à sua respectiva propriedade no objeto Fatura
            ConfigurarDatePickerNulavel(dtpDataAtracação, Fatura.DataAtracacao);
            ConfigurarDatePickerNulavel(dtpdataDesembaracoDI, Fatura.DataDesembaracoDI);
            ConfigurarDatePickerNulavel(dtpDI, Fatura.DAtaDI);
            ConfigurarDatePickerNulavel(dtpDataRecebimento, Fatura.DataRecebimento);
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

            // Evento para atualizar o formato visual quando o usuário interage
            dtp.ValueChanged -= Dtp_ValueChanged_Format; // Evita adicionar o evento múltiplas vezes
            dtp.ValueChanged += Dtp_ValueChanged_Format;
        }
        private void Dtp_ValueChanged_Format(object? sender, EventArgs e)
        {
            if (sender is DateTimePicker picker)
            {
                picker.Format = picker.Checked ? DateTimePickerFormat.Short : DateTimePickerFormat.Custom;
            }
        }
        private void btnCalcularTotal_Click(object? sender, EventArgs e)
        {
            // Força a atualização do objeto Fatura com os valores da tela antes de calcular
            BsFatura.EndEdit();
            this.ValidateChildren();

            // A lógica de cálculo agora opera diretamente sobre o objeto Fatura, o que é mais seguro.
            decimal totalDespesas = Fatura.ImpostoImportacao + Fatura.IPI + Fatura.PIS_PASEP + Fatura.COFINS +
                                  Fatura.MULTA_LI + Fatura.ICMS + Fatura.ArmazenagemP + Fatura.FreteMaritimoP +
                                  Fatura.Marinha_MercanteP + Fatura.GRUANVISAP + Fatura.LiCancelada_IndeferidaP +
                                  Fatura.ExpedienteLiCanceladaP + Fatura.EncaminhamentoAmostrasP + Fatura.DarfAnvisaP +
                                  Fatura.MotoboyP + Fatura.LiP + Fatura.DespesasDesembaracoP +
                                  Fatura.Expediente + Fatura.HD + Fatura.Cartorio;

            // Adiciona os custos das agências dinâmicas
            totalDespesas += Fatura.Agencias.Sum(ag => ag.Custo);

            Fatura.TotalDespesas = totalDespesas;
            Fatura.Adiantamento = Fatura.ValRecebidos;
            Fatura.SubTotal = Fatura.TotalDespesas + Fatura.NComissao;
            Fatura.Saldo = Fatura.SubTotal - Fatura.Adiantamento;

            Fatura.TipoFinalizacao = Fatura.Saldo < 0 ? "S/FAVOR" : "N/FAVOR";
            txtTipoFinalizacao.BackColor = Fatura.Saldo < 0 ? Color.Red : Color.Lime;

            // Atualiza a tela com os novos valores calculados
            BsFatura.ResetBindings(false);
            btnOK.Enabled = true;
        }

        private void AplicarMascaraMoeda(object? sender, EventArgs e)
        {
            if (sender is not TextBox txt) return;

            // Para evitar loops infinitos, removemos e readicionamos o evento
            txt.TextChanged -= AplicarMascaraMoeda;

            string valor = new string(txt.Text.Where(char.IsDigit).ToArray());
            if (decimal.TryParse(valor, out decimal valorDecimal))
            {
                valorDecimal /= 100;
                txt.Text = valorDecimal.ToString("C", CultureInfo.GetCultureInfo("pt-BR"));
                txt.SelectionStart = txt.Text.Length;
            }

            txt.TextChanged += AplicarMascaraMoeda;
        }
        private void btnAdicionarAgencia_Click(object? sender, EventArgs e)
        {
            using var frm = new DetalhesAgencia();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                var novaAgencia = new Agencia { Numero = frm.NumeroAgencia, Custo = frm.PrecoCusto };
                Fatura.Agencias.Add(novaAgencia);
                AdicionarPainelAgencia(novaAgencia);
            }
        }
        private void AdicionarPainelAgencia(Agencia agencia)
        {
            var panel = new Panel
            {
                Height = 30,
                Width = 150,
                Padding = new Padding(5),
                Margin = new Padding(3),
                BorderStyle = BorderStyle.FixedSingle,
                Tag = agencia // Guarda a referência ao objeto
            };

            var lbl = new Label { Text = $"{agencia.Numero} - {agencia.Custo:C}", AutoSize = true, Margin = new Padding(0, 5, 5, 0) };
            var btnRemover = new Button { Text = "x", Size = new Size(23, 23), BackColor = Color.LightCoral };

            btnRemover.Click += (s, e) => {
                flowLayoutPanel1.Controls.Remove(panel);
                Fatura.Agencias.Remove(agencia);
            };

            var innerFlow = new FlowLayoutPanel { AutoSize = true, FlowDirection = FlowDirection.LeftToRight, WrapContents = false };
            innerFlow.Controls.Add(lbl);
            innerFlow.Controls.Add(btnRemover);
            panel.Controls.Add(innerFlow);
            flowLayoutPanel1.Controls.Add(panel);
        }
    }
}

using CLUSA;

namespace Trabalho
{
    public partial class UCOrgaoAnuente : UserControl
    {
        // Essa lista guarda as LIs em memória (será preenchida pelo Form genérico)
        private List<LiInfo> listaLis = new List<LiInfo>();
        private string _nomeOrgao;
        private OrgaoAnuente _entidadeAtual;

        // Define se o controle está em modo de visualização (sem edição)
        public bool Visualizacao { get; set; }

        public UCOrgaoAnuente()
        {
            InitializeComponent();
        }

        // Chame este método para “injetar” no UserControl a entidade (T) que vem do Form genérico
        public void CarregarCamposBase(OrgaoAnuente entidade)
        {
            _entidadeAtual = entidade;
            // Entidade já estiver populada pelo Form, então apenas reflete os campos:
            TXTnr.Text = entidade.Ref_USA;
            TXTimportador.Text = entidade.Importador;
            TXTsr.Text = entidade.SR;
            TXTexportador.Text = entidade.Exportador;
            TXTterminal.Text = entidade.Terminal;
            TXTNavio.Text = entidade.Veiculo;
            TXTProduto.Text = entidade.Produto;
            TXTorigem.Text = entidade.Origem;
            TXTstatusdoprocesso.Text = entidade.StatusDoProcesso;
            TXTpendencia.Text = entidade.Pendencia;
            CBamostra.Checked = entidade.Amostra;

            if (entidade.CheckDataDeAtracacao && entidade.DataDeAtracacao.HasValue)
            {
                DTPdatadeatracacao.Checked = true;
                DTPdatadeatracacao.Value = entidade.DataDeAtracacao.Value;
                DTPdatadeatracacao.Format = DateTimePickerFormat.Short;
            }
            else
            {
                DTPdatadeatracacao.Checked = false;
                DTPdatadeatracacao.Format = DateTimePickerFormat.Custom;
                DTPdatadeatracacao.CustomFormat = " ";
            }

            // Data de Embarque
            if (entidade.CheckDataEmbarque && entidade.DataEmbarque.HasValue)
            {
                DTPdatadeembarque.Checked = true;
                DTPdatadeembarque.Value = entidade.DataEmbarque.Value;
                DTPdatadeembarque.Format = DateTimePickerFormat.Short;
            }
            else
            {
                DTPdatadeembarque.Checked = false;
                DTPdatadeembarque.Format = DateTimePickerFormat.Custom;
                DTPdatadeembarque.CustomFormat = " ";
            }

            // Inspeção
            if (entidade.CheckInspecao && entidade.Inspecao.HasValue)
            {
                DTPdatadeinspecao.Checked = true;
                DTPdatadeinspecao.Value = entidade.Inspecao.Value;
                DTPdatadeinspecao.Format = DateTimePickerFormat.Short;
            }
            else
            {
                DTPdatadeinspecao.Checked = false;
                DTPdatadeinspecao.Format = DateTimePickerFormat.Custom;
                DTPdatadeinspecao.CustomFormat = " ";
            }
        }

        public void CarregarLisBase(OrgaoAnuente entidade)
        {
            _nomeOrgao = entidade.GetType().Name.ToUpperInvariant();

            listaLis = entidade.LI?
                .Where(li => li.LpcosPorOrgao != null && 
                             li.LpcosPorOrgao.Any(lpco => 
                                  string.Equals(lpco.NomeOrgao, _nomeOrgao, StringComparison.OrdinalIgnoreCase))) 
                .ToList() ?? new List<LiInfo>();

            AtualizarPainelLi();
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

                var btnVisualizar = new Button
                {
                    Text = Visualizacao ? "Visualizar" : "Editar",
                    Size = new Size(75, 25),
                    Anchor = AnchorStyles.Top | AnchorStyles.Right
                };
                btnVisualizar.Location = new Point(panel.Width - 80, 7);

                btnVisualizar.Click += (s, e) =>
                {
                    // 1. Encontra o LPCO específico para este órgão dentro da LI clicada
                    var lpcoParaEditar = li.LpcosPorOrgao.FirstOrDefault(lpco =>
                        string.Equals(lpco.NomeOrgao, _nomeOrgao, StringComparison.OrdinalIgnoreCase));

                    // 2. Verifica se os dados do LPCO foram encontrados
                    if (lpcoParaEditar == null)
                    {
                        MessageBox.Show($"Não foram encontrados dados de LPCO para o órgão {_nomeOrgao} nesta LI.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // 3. Abre o formulário de detalhes do LPCO, passando o objeto e o modo de visualização
                    using (var frm = new frmLpcoDetalhes(lpcoParaEditar, Visualizacao))
                    {
                        frm.Owner = this.FindForm();
                        frm.ShowDialog();
                        // Ao fechar, o objeto 'lpcoParaEditar' já estará atualizado em memória.
                    }
                };

                panel.Controls.Add(lbl);
                panel.Controls.Add(btnVisualizar);
                flpLis.Controls.Add(panel);
            }
        }

        // Chame este método para ler os valores do UserControl e popular a Entidade
        public void ExtrairParaEntidade(OrgaoAnuente entidade)
        {
            entidade.Ref_USA = TXTnr.Text;
            entidade.Importador = TXTimportador.Text;
            entidade.SR = TXTsr.Text;
            entidade.Exportador = TXTexportador.Text;
            entidade.Terminal = TXTterminal.Text;
            entidade.Veiculo = TXTNavio.Text;
            entidade.Produto = TXTProduto.Text;
            entidade.Origem = TXTorigem.Text;
            entidade.StatusDoProcesso = TXTstatusdoprocesso.Text;
            entidade.Pendencia = TXTpendencia.Text;
            entidade.Amostra = CBamostra.Checked;
            entidade.LI = listaLis.ToList();
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            // Quando o botão "Salvar" for clicado, apenas dispara um evento para o form pai:
            OnConfirmar?.Invoke(this, EventArgs.Empty);
        }

        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            // Dispara evento de cancelamento para o form pai
            OnCancelar?.Invoke(this, EventArgs.Empty);
        }

        private void TErro_Tick(object sender, EventArgs e)
        {
            // Lógica de timer, se necessário
        }

        // Eventos que o Form genérico irá escutar
        public event EventHandler OnConfirmar;
        public event EventHandler OnCancelar;

        private void DateTimePicker_OnValueChanged(object sender, EventArgs e)
        {
            // Garante que o sender é um DateTimePicker e que a entidade foi carregada
            if (sender is not DateTimePicker picker || _entidadeAtual == null)
                return;

            // 1. Ajusta o formato visual (seu código atual)
            picker.Format = picker.Checked
                ? DateTimePickerFormat.Short
                : DateTimePickerFormat.Custom;
            if (!picker.Checked)
                picker.CustomFormat = "' -'";

            // 2. Determina quais propriedades da entidade devem ser atualizadas
            string nomePropData;
            string nomePropCheck;

            switch (picker.Name)
            {
                case "DTPdatadeatracacao":
                    nomePropData = nameof(OrgaoAnuente.DataDeAtracacao);
                    nomePropCheck = nameof(OrgaoAnuente.CheckDataDeAtracacao);
                    break;

                case "DTPdatadeembarque":
                    nomePropData = nameof(OrgaoAnuente.DataEmbarque);
                    nomePropCheck = nameof(OrgaoAnuente.CheckDataEmbarque);
                    break;

                case "DTPdatadeinspecao":
                    nomePropData = nameof(OrgaoAnuente.Inspecao);
                    nomePropCheck = nameof(OrgaoAnuente.CheckInspecao);
                    break;

                default:
                    return; // Sai do método se o DTP não for um dos mapeados
            }

            // 3. Atualiza as propriedades na entidade usando Reflection
            DateTime? valor = picker.Checked ? picker.Value : null;

            var propData = typeof(OrgaoAnuente).GetProperty(nomePropData);
            propData?.SetValue(_entidadeAtual, valor);

            var propCheck = typeof(OrgaoAnuente).GetProperty(nomePropCheck);
            propCheck?.SetValue(_entidadeAtual, picker.Checked);
        }

        private void UCOrgaoAnuente_Load(object sender, EventArgs e)
        {
            InicializarDateTimePickersComCheckbox();

            if (Visualizacao)
            {
                AplicarModoSomenteLeitura();
            }
        }
        private void AplicarModoSomenteLeitura()
        {
            BtnOK.Visible = false; 
            BtnCancelar.Text = "Fechar";

            SetCamposSomenteLeitura(this.Controls);
        }
        private void SetCamposSomenteLeitura(Control.ControlCollection controls)
        {
            foreach (Control control in controls)
            {
                if (control is TextBox textBox)
                {
                    textBox.ReadOnly = true;
                }
                else if (control is ComboBox ||
                         control is CheckBox ||
                         control is DateTimePicker ||
                         control is NumericUpDown)
                {
                    control.Enabled = false;
                }
                if (control.Controls.Count > 0)
                {
                    SetCamposSomenteLeitura(control.Controls);
                }
            }
        }
        private void InicializarDateTimePickersComCheckbox()
        {
            // Liste aqui todos os seus DateTimePickers que devem ter checkbox interno
            var dtps = new[]
            {
                DTPdatadeinspecao,
                DTPdatadeatracacao,
                DTPdatadeembarque
            };

            foreach (var dtp in dtps)
            {
                dtp.ShowCheckBox = true;
                dtp.ValueChanged += DateTimePicker_OnValueChanged;
                // caso queira capturar também o uncheck via clique:
                dtp.MouseUp += (s, e2) => DateTimePicker_OnValueChanged(s, null);
            }
        }
    }
}

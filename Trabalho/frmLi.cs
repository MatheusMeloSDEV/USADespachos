using CLUSA;

namespace Trabalho
{
    public partial class frmLi : Form
    {
        public LiInfo Li { get; private set; }
        private bool _somenteVisualizacao;
        private string? _numeroOriginal;

        public frmLi(LiInfo? liParaEditar, bool somenteVisualizacao)
        {
            InitializeComponent();
            InicializarDateTimePickersComCheckbox();
            Li = liParaEditar ?? new LiInfo();
            _somenteVisualizacao = somenteVisualizacao;
            if (liParaEditar != null)
            {
                _numeroOriginal = liParaEditar.Numero;
            }
        }

        private void frmLi_Load(object sender, EventArgs e)
        {
            TxtLi.Text = Li.Numero;
            TxtNCM.Text = Li.NCM;

            if (Li.CheckDataRegistroLI && Li.DataRegistroLI != null!)
            {
                dtpDataRegistroLI.Checked = true;
                dtpDataRegistroLI.Value = Li.DataRegistroLI.Value;
                dtpDataRegistroLI.Format = DateTimePickerFormat.Short;
            }
            else
            {
                dtpDataRegistroLI.Checked = false;
                dtpDataRegistroLI.Format = DateTimePickerFormat.Custom;
                dtpDataRegistroLI.CustomFormat = " ";
            }

            cbMapa.Tag = "MAPA";
            cbAnvisa.Tag = "ANVISA";
            cbDecex.Tag = "DECEX";
            cbIbama.Tag = "IBAMA";
            cbInmetro.Tag = "INMETRO";

            SincronizarCheckBoxesComDados();

            // Ajusta para apenas leitura se necessário
            if (_somenteVisualizacao)
            {
                TxtLi.ReadOnly = true;
                TxtNCM.ReadOnly = true;
                dtpDataRegistroLI.Enabled = false;

                // Desabilita os checkboxes
                foreach (var cb in GBOrgaosAnuentes.Controls.OfType<CheckBox>())
                {
                    cb.Enabled = false;
                }

                btnOK.Visible = false;
                btnRemover.Text = "Fechar"; // No modo visualização, o botão "Remover" apenas fecha
            }
        }

        private void OrgaoCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            var checkBox = sender as CheckBox;
            if (checkBox == null || !checkBox.Focused) return; // Evita disparos múltiplos

            var nomeOrgao = checkBox.Tag.ToString();
            var lpcoExistente = Li.LpcosPorOrgao.FirstOrDefault(l => l.NomeOrgao == nomeOrgao);

            if (checkBox.Checked)
            {
                LpcoInfo lpcoParaEditar = lpcoExistente ?? new LpcoInfo { NomeOrgao = nomeOrgao };

                using (var frm = new frmLpcoDetalhes(lpcoParaEditar))
                {
                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        if (lpcoExistente == null)
                        {
                            Li.LpcosPorOrgao.Add(lpcoParaEditar);
                        }
                    }
                    else
                    {
                        checkBox.CheckedChanged -= OrgaoCheckBox_CheckedChanged;
                        checkBox.Checked = false; // Cancela, então desmarca sem disparar o evento de novo
                        checkBox.CheckedChanged += OrgaoCheckBox_CheckedChanged;
                    }
                }
            }
            else
            {
                if (lpcoExistente != null)
                {
                    var confirmacao = MessageBox.Show(
                        $"Deseja remover os dados do LPCO para o órgão {nomeOrgao}?",
                        "Confirmar Remoção", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (confirmacao == DialogResult.Yes)
                    {
                        Li.LpcosPorOrgao.Remove(lpcoExistente);
                    }
                    else
                    {
                        checkBox.CheckedChanged -= OrgaoCheckBox_CheckedChanged;
                        checkBox.Checked = true; // Usuário desistiu, marca de volta sem disparar o evento
                        checkBox.CheckedChanged += OrgaoCheckBox_CheckedChanged;
                    }
                }
            }
        }

        private void SincronizarCheckBoxesComDados()
        {
            foreach (var cb in GBOrgaosAnuentes.Controls.OfType<CheckBox>())
            {
                // Desativa temporariamente o evento para evitar que seja disparado
                cb.CheckedChanged -= OrgaoCheckBox_CheckedChanged;

                var nomeOrgao = cb.Tag?.ToString();
                if (nomeOrgao != null)
                {
                    cb.Checked = Li.LpcosPorOrgao.Any(lpco => lpco.NomeOrgao == nomeOrgao);
                }

                // Reativa o evento
                cb.CheckedChanged += OrgaoCheckBox_CheckedChanged;
            }
        }

        // Método simplificado para o único DateTimePicker que restou
        private void DateTimePicker_OnValueChanged(object? sender, EventArgs e)
        {
            if (sender is not DateTimePicker picker) return;

            if (picker.Checked)
            {
                picker.Format = DateTimePickerFormat.Short;
            }
            else
            {
                picker.Format = DateTimePickerFormat.Custom;
                picker.CustomFormat = " ";
            }

            if (picker.Name == "dtpDataRegistroLI")
            {
                Li.CheckDataRegistroLI = picker.Checked;
                Li.DataRegistroLI = picker.Checked ? picker.Value : null;
            }
        }

        private void InicializarDateTimePickersComCheckbox()
        {
            // Liste aqui todos os seus DateTimePickers que devem ter checkbox interno
            var dtps = new[]
            {
                dtpDataRegistroLI
            };

            foreach (var dtp in dtps)
            {
                dtp.ShowCheckBox = true;
                dtp.Checked = false;

                dtp.Format = DateTimePickerFormat.Custom;
                dtp.CustomFormat = " ";

                dtp.ValueChanged += DateTimePicker_OnValueChanged;
                dtp.MouseUp += (s, e2) => DateTimePicker_OnValueChanged(s, null);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Li.Numero = TxtLi.Text.Trim();
            Li.NCM = TxtNCM.Text.Trim();
            Li.CheckDataRegistroLI = dtpDataRegistroLI.Checked;
            Li.DataRegistroLI = dtpDataRegistroLI.Checked ? dtpDataRegistroLI.Value : null;

            if (string.IsNullOrWhiteSpace(Li.Numero))
            {
                MessageBox.Show("O número da LI é obrigatório.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (Owner is ILiHandler handler)
            {
                // Lógica de decisão simplificada:
                bool eraUmaLiExistente = !string.IsNullOrEmpty(_numeroOriginal);

                if (eraUmaLiExistente)
                {
                    // Se estávamos editando, sempre chamamos AtualizarLi.
                    // Passamos o número ORIGINAL para que o sistema saiba qual LI encontrar e substituir.
                    handler.AtualizarLi(_numeroOriginal, Li);
                }
                else
                {
                    // Se não havia número original, é uma LI nova.
                    handler.AdicionarLi(Li);
                }
            }
            this.DialogResult = DialogResult.OK;
        }

        private void btnRemover_Click(object sender, EventArgs e)
        {
            if (_somenteVisualizacao)
            {
                this.Close(); // Se for só visualização, o botão apenas fecha
                return;
            }

            if (string.IsNullOrEmpty(Li.Numero))
            {
                this.Close(); // Se for uma LI nova, não há o que remover, então age como "Cancelar"
                return;
            }

            var confirmacao = MessageBox.Show(
                $"Você tem certeza que deseja remover a LI '{Li.Numero}'?",
                "Confirmar Remoção",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirmacao == DialogResult.Yes)
            {
                if (Owner is ILiHandler handler)
                {
                    handler.RemoverLi(Li.Numero);
                }
                this.DialogResult = DialogResult.Abort;
                this.Close();
            }
        }
    }
}
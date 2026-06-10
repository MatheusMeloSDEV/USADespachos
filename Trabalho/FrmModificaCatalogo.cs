using CLUSA.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Trabalho
{
    public partial class FrmModificaCatalogo : Form
    {
        public List<Orgao> Orgaos { get; private set; } = new List<Orgao>();
        // Modo: "Adicionar" (padrão) ou "Editar"
        public enum ModoFormulario { Adicionar, Editar }
        public ModoFormulario Modo { get; set; } = ModoFormulario.Adicionar;

        // Quando em modo Editar, o chamador pode passar um catálogo para carregar
        public CLUSA.Models.Catalogo? CatalogoInicial { get; set; }

        private bool _catalogoInteragido = false;
        public FrmModificaCatalogo()
        {
            InitializeComponent();
            // Anexa o evento do botão de adicionar órgão
            BtnAdicionarOrgao.Click += BtnAdicionarOrgao_Click;
            this.Load += FrmModificaCatalogo_Load;

        }

        private void BtnAdicionarOrgao_Click(object? sender, EventArgs e)
        {
            // Nome do órgão a partir do ComboBox (text ou item selecionado)
            var tabName = cbOrgao.SelectedItem?.ToString() ?? cbOrgao.Text;
            if (string.IsNullOrWhiteSpace(tabName))
            {
                MessageBox.Show("Selecione um órgão antes de adicionar.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Cria uma nova TabPage baseada na MAPA (template)
            var newTab = new TabPage(tabName);
            newTab.Padding = MAPA.Padding;
            newTab.Size = MAPA.Size;

            // Copia controles do template MAPA
            foreach (Control ctrl in MAPA.Controls)
            {
                var copy = CloneControl(ctrl);
                if (copy != null)
                {
                    // Ajusta nome para evitar duplicatas
                    copy.Name = GetUniqueControlName(newTab, ctrl.Name);
                    newTab.Controls.Add(copy);
                }
            }

            // ✅ ADICIONE ESTA LINHA AQUI (aplica as regras ocultando o que não precisa)
            AplicarRegrasDoOrgao(newTab, tabName);

            TbOrgao.TabPages.Add(newTab);
            TbOrgao.SelectedTab = newTab;

            // Marcar interação (habilita remoção de catálogo quando apropriado)
            Catalogo_UserInteracted(this, EventArgs.Empty);
        }
        private void AplicarRegrasDoOrgao(TabPage aba, string nomeOrgao)
        {
            // Define a lógica de quem mostra o quê
            bool mostrarInspecaoColeta = nomeOrgao.Contains("MAPA", StringComparison.OrdinalIgnoreCase) ||
                                         nomeOrgao.Contains("ANVISA", StringComparison.OrdinalIgnoreCase);

            bool mostrarComunicado = nomeOrgao.Contains("ANVISA", StringComparison.OrdinalIgnoreCase);

            foreach (Control c in aba.Controls)
            {
                // Resgata o nome original do controle (do template MAPA)
                var nomeOriginal = c.Tag as string ?? c.Name;

                switch (nomeOriginal)
                {
                    case "dtpOrgaoInspecao":
                    case "lblInspecao":
                    case "dtpOrgaoColeta":
                    case "lblColeta":
                        c.Visible = mostrarInspecaoColeta;
                        break;

                    case "cbOrgaoComunicado":
                    case "lblComunicado":
                        c.Visible = mostrarComunicado;
                        break;

                        // Nota: "cbOrgaoParametrizacao" e "lblParametrizacao" não precisam de case 
                        // pois ficam visíveis para todos (MAPA, ANVISA e DECEX).
                }
            }
        }

        private Control? CloneControl(Control original)
        {
            if (original is Label lbl)
            {
                var c = new Label();
                c.Text = lbl.Text;
                c.AutoSize = lbl.AutoSize;
                c.Location = lbl.Location;
                c.Size = lbl.Size;
                c.Tag = original.Name;
                return c;
            }
            if (original is ComboBox cb)
            {
                var c = new ComboBox();
                c.Location = cb.Location;
                c.Size = cb.Size;
                c.DropDownStyle = cb.DropDownStyle;
                foreach (var it in cb.Items) c.Items.Add(it);

                // Garante que nasça sem nada selecionado
                c.SelectedIndex = -1;
                c.Tag = original.Name;
                return c;
            }
            if (original is DateTimePicker dtp)
            {
                var c = new DateTimePicker();
                c.Location = dtp.Location;
                c.Size = dtp.Size;

                // Configura para nascer desmarcado e visualmente vazio
                c.ShowCheckBox = true;
                c.Checked = false;
                c.Format = DateTimePickerFormat.Custom;
                c.CustomFormat = " ";

                // Atrela o evento para mostrar a data apenas quando o usuário marcar a caixinha
                c.ValueChanged -= Dtp_ValueChanged_Format;
                c.ValueChanged += Dtp_ValueChanged_Format;

                c.Tag = original.Name;
                return c;
            }
            if (original is TextBox tb)
            {
                var c = new TextBox();
                c.Location = tb.Location;
                c.Size = tb.Size;
                c.Text = string.Empty; // Garante que nasça vazio
                return c;
            }

            try
            {
                var c = (Control)Activator.CreateInstance(original.GetType())!;
                c.Location = original.Location;
                c.Size = original.Size;
                c.Text = string.Empty;
                c.Tag = original.Name;
                return c;
            }
            catch
            {
                return null;
            }
        }

        private void Dtp_ValueChanged_Format(object? sender, EventArgs e)
        {
            if (sender is DateTimePicker picker)
            {
                if (picker.Checked)
                {
                    picker.Format = DateTimePickerFormat.Short; // Mostra a data
                }
                else
                {
                    picker.Format = DateTimePickerFormat.Custom;
                    picker.CustomFormat = " "; // Esconde a data
                }
            }
        }

        private string GetUniqueControlName(Control container, string baseName)
        {
            var name = baseName;
            int i = 1;
            while (container.Controls.Find(name, true).Length > 0)
            {
                name = baseName + "_" + i;
                i++;
            }
            return name;
        }

        private void Catalogo_UserInteracted(object? sender, EventArgs e)
        {
            // Marca que houve interação com o catálogo
            _catalogoInteragido = true;

            // Se estamos em modo Editar, mostramos e habilitamos o botão de remoção.
            // Em modo Adicionar, o botão permanece oculto até o salvamento (requisito do usuário).
            if (Modo == ModoFormulario.Editar)
            {
                BtnRemoverCatalogo.Visible = true;
                BtnRemoverCatalogo.Enabled = true;
            }
            else
            {
                // apenas habilita (mas não mostra) para permitir remoção posterior
                BtnRemoverCatalogo.Enabled = false;
            }
        }

        private void BtnSalvar_Click(object sender, EventArgs e)
        {
            Orgaos.Clear();

            foreach (TabPage tab in TbOrgao.TabPages)
            {
                try
                {
                    var orgaoId = tab.Text;
                    string parametrizacao = "";
                    DateTime? inspecao = null; // ✅ Inicializa como nulo real
                    DateTime? coleta = null;   // ✅ Inicializa como nulo real
                    string comunicado = "";

                    foreach (Control c in tab.Controls)
                    {
                        var originalName = c.Tag as string ?? c.Name;
                        switch (originalName)
                        {
                            case "cbOrgaoParametrizacao":
                                if (c is ComboBox cbp) parametrizacao = cbp.Text;
                                break;
                            case "dtpOrgaoInspecao":
                                // Se estiver checado, extrai o valor real
                                if (c is DateTimePicker dtpi && dtpi.Checked) inspecao = dtpi.Value;
                                break;
                            case "dtpOrgaoColeta":
                                if (c is DateTimePicker dtpc && dtpc.Checked) coleta = dtpc.Value;
                                break;
                            case "cbOrgaoComunicado":
                                if (c is ComboBox cbc) comunicado = cbc.Text;
                                break;
                        }
                    }

                    var org = new Orgao(orgaoId, parametrizacao, inspecao.GetValueOrDefault(), coleta.GetValueOrDefault(), comunicado);
                    Orgaos.Add(org);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Erro ao processar aba {tab.Text}: {ex.Message}");
                }
            }

            var catalogo = new Catalogo
            {
                NCM = txtNCM.Text ?? string.Empty,
                cClassTrib = txtcClassTrib.Text ?? string.Empty,
                Orgaos = Orgaos
            };

            this.CatalogoInicial = catalogo;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private async void BtnRemoverOrgao_Click(object? sender, EventArgs e)
        {
            if (TbOrgao.SelectedTab == null)
            {
                MessageBox.Show("Nenhuma aba selecionada para remover.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var tab = TbOrgao.SelectedTab;
            var resp = MessageBox.Show($"Remover a aba '{tab.Text}'?", "Confirmar remoção", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (resp == DialogResult.Yes)
            {
                TbOrgao.TabPages.Remove(tab);
                tab.Dispose();
            }
        }

        private async void BtnRemoverCatalogo_Click(object? sender, EventArgs e)
        {
            // Só permite remover se já houve interação
            if (!_catalogoInteragido)
            {
                MessageBox.Show("Interaja com o catálogo antes de removê-lo.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var resp = MessageBox.Show("Deseja remover este catálogo (dados não poderão ser recuperados)?", "Confirmar remoção", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (resp == DialogResult.Yes)
            {
                // Fecha o formulário sinalizando remoção pelo DialogResult
                this.DialogResult = DialogResult.Abort;
                this.Close();
            }
        }

        private void FrmModificaCatalogo_Load(object? sender, EventArgs e)
        {
            if (Modo == ModoFormulario.Editar)
            {
                BtnRemoverCatalogo.Visible = true;
                BtnRemoverCatalogo.Enabled = true;

                if (CatalogoInicial != null)
                {
                    txtNCM.Text = CatalogoInicial.NCM;
                    txtcClassTrib.Text = CatalogoInicial.cClassTrib;

                    foreach (var org in CatalogoInicial.Orgaos)
                    {
                        cbOrgao.Text = org.OrgaoId;
                        BtnAdicionarOrgao_Click(this, EventArgs.Empty);

                        var tab = TbOrgao.TabPages.Cast<TabPage>().LastOrDefault();
                        if (tab != null)
                        {
                            foreach (Control c in tab.Controls)
                            {
                                var originalName = c.Tag as string ?? c.Name;
                                switch (originalName)
                                {
                                    case "cbOrgaoParametrizacao":
                                        if (c is ComboBox cbp) cbp.Text = org.Parametrizacao;
                                        break;
                                    case "dtpOrgaoInspecao":
                                        if (c is DateTimePicker dtpi)
                                        {
                                            // Valida se tem valor e se a data está dentro do limite do WinForms
                                            if (org.Inspecao.HasValue && org.Inspecao.Value.Year >= 1753)
                                            {
                                                dtpi.Format = DateTimePickerFormat.Short;
                                                dtpi.Value = org.Inspecao.Value;
                                                dtpi.Checked = true;
                                            }
                                            else
                                            {
                                                dtpi.Checked = false;
                                                dtpi.Format = DateTimePickerFormat.Custom;
                                                dtpi.CustomFormat = " ";
                                            }
                                        }
                                        break;
                                    case "dtpOrgaoColeta":
                                        if (c is DateTimePicker dtpc)
                                        {
                                            if (org.Coleta.HasValue && org.Coleta.Value.Year >= 1753)
                                            {
                                                dtpc.Format = DateTimePickerFormat.Short;
                                                dtpc.Value = org.Coleta.Value;
                                                dtpc.Checked = true;
                                            }
                                            else
                                            {
                                                dtpc.Checked = false;
                                                dtpc.Format = DateTimePickerFormat.Custom;
                                                dtpc.CustomFormat = " ";
                                            }
                                        }
                                        break;
                                    case "cbOrgaoComunicado":
                                        if (c is ComboBox cbc) cbc.Text = org.Comunicado;
                                        break;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                BtnRemoverCatalogo.Visible = false;
                BtnRemoverCatalogo.Enabled = false;
            }
        }

        private bool _formatandoNCM = false;

        private void txtNCM_TextChanged(object sender, EventArgs e)
        {
            if (_formatandoNCM) return; // Evita loop infinito
            _formatandoNCM = true;

            // Extrai só os números
            string raw = new string(txtNCM.Text.Where(char.IsDigit).ToArray());
            if (raw.Length > 8) raw = raw.Substring(0, 8); // Limita a 8 dígitos

            // Monta a máscara 0000.00.00 progressivamente
            string formatted = raw;
            if (raw.Length > 6)
                formatted = raw.Substring(0, 4) + "." + raw.Substring(4, 2) + "." + raw.Substring(6);
            else if (raw.Length > 4)
                formatted = raw.Substring(0, 4) + "." + raw.Substring(4);

            txtNCM.Text = formatted;

            // Joga o cursor para o final para você continuar digitando
            txtNCM.SelectionStart = txtNCM.Text.Length;

            _formatandoNCM = false;
        }

        private bool _formatandoClass = false;

        private void txtcClassTrib_TextChanged(object sender, EventArgs e)
        {
            if (_formatandoClass) return; // Evita loop infinito
            _formatandoClass = true;

            // Pega os números e remove zeros à esquerda velhos para simular a "empurrada"
            string raw = new string(txtcClassTrib.Text.Where(char.IsDigit).ToArray()).TrimStart('0');

            if (raw.Length > 6) raw = raw.Substring(raw.Length - 6); // Limite de 6 dígitos

            if (raw.Length > 0)
            {
                // Preenche com os zeros e envolve nos colchetes
                txtcClassTrib.Text = $"[{raw.PadLeft(6, '0')}]";

                // Joga o cursor para ANTES do colchete final "]"
                txtcClassTrib.SelectionStart = txtcClassTrib.Text.Length - 1;
            }
            else
            {
                txtcClassTrib.Text = "";
            }

            _formatandoClass = false;
        }

        // --- PROTEÇÃO CONTRA LETRAS (MANTENHA ESTE MÉTODO) ---
        private void ApenasNumeros_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Permite apenas números e a tecla Backspace
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
    }
}

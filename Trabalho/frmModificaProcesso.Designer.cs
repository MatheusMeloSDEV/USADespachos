namespace Trabalho
{
    partial class FrmModificaProcesso
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            bsModificaProcesso = new BindingSource(components);
            groupBox2 = new GroupBox();
            label18 = new Label();
            dtpDataMinuta = new DateTimePicker();
            label19 = new Label();
            label1 = new Label();
            DTPdatadedesembaracodi = new DateTimePicker();
            DTPdataderegistrodi = new DateTimePicker();
            LBLinspecao = new Label();
            TXTdi = new TextBox();
            label21 = new Label();
            label23 = new Label();
            DTPdatadeinspecao = new DateTimePicker();
            label6 = new Label();
            CBparametrizacaodi = new ComboBox();
            DTPdatadecarregamentodi = new DateTimePicker();
            CBamostra = new CheckBox();
            CBdesovado = new CheckBox();
            button1 = new Button();
            groupBox1 = new GroupBox();
            CBmapa = new CheckBox();
            CBimetro = new CheckBox();
            CBanvisa = new CheckBox();
            CBibama = new CheckBox();
            CBdecex = new CheckBox();
            btnAdiciona = new Button();
            NUMfreetime = new NumericUpDown();
            label17 = new Label();
            TXTimportador = new TextBox();
            label14 = new Label();
            TXTportodedestino = new TextBox();
            label15 = new Label();
            label12 = new Label();
            label11 = new Label();
            label9 = new Label();
            TXTsr = new TextBox();
            label5 = new Label();
            txtConhecimento = new TextBox();
            txtArmador = new TextBox();
            label7 = new Label();
            TXTProduto = new TextBox();
            label2 = new Label();
            TXTflo = new TextBox();
            Exportador = new Label();
            TXTexportador = new TextBox();
            LBLdatadeatracacao = new Label();
            LBLdatadeembarque = new Label();
            DTPdatadeatracacao = new DateTimePicker();
            DTPdatadeembarque = new DateTimePicker();
            label10 = new Label();
            TXTstatusdoprocesso = new TextBox();
            label8 = new Label();
            TXTpendencia = new TextBox();
            label13 = new Label();
            txtVeiculo = new TextBox();
            checkedListBox1 = new CheckedListBox();
            label16 = new Label();
            checkedListBox2 = new CheckedListBox();
            label24 = new Label();
            label25 = new Label();
            DTPDataRecOriginais = new DateTimePicker();
            TXTnr = new MaskedTextBox();
            groupBox4 = new GroupBox();
            button3 = new Button();
            flpLis = new FlowLayoutPanel();
            txtTerminal = new TextBox();
            label4 = new Label();
            label20 = new Label();
            label22 = new Label();
            dtpVencimentoLI_LPCO = new DateTimePicker();
            dtpVencimentoFreeTime = new DateTimePicker();
            label26 = new Label();
            dtpVencimentoFMA = new DateTimePicker();
            groupBox3 = new GroupBox();
            label27 = new Label();
            cbMarca = new ComboBox();
            numMarca = new NumericUpDown();
            txtOrigem = new TextBox();
            label3 = new Label();
            btnCapa = new Button();
            btnRelatorio = new Button();
            ((System.ComponentModel.ISupportInitialize)bsModificaProcesso).BeginInit();
            groupBox2.SuspendLayout();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)NUMfreetime).BeginInit();
            groupBox4.SuspendLayout();
            groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numMarca).BeginInit();
            SuspendLayout();
            // 
            // bsModificaProcesso
            // 
            bsModificaProcesso.DataSource = typeof(CLUSA.Processo);
            // 
            // groupBox2
            // 
            groupBox2.Anchor = AnchorStyles.None;
            groupBox2.Controls.Add(label18);
            groupBox2.Controls.Add(dtpDataMinuta);
            groupBox2.Controls.Add(label19);
            groupBox2.Controls.Add(label1);
            groupBox2.Controls.Add(DTPdatadedesembaracodi);
            groupBox2.Controls.Add(DTPdataderegistrodi);
            groupBox2.Controls.Add(LBLinspecao);
            groupBox2.Controls.Add(TXTdi);
            groupBox2.Controls.Add(label21);
            groupBox2.Controls.Add(label23);
            groupBox2.Controls.Add(DTPdatadeinspecao);
            groupBox2.Controls.Add(label6);
            groupBox2.Controls.Add(CBparametrizacaodi);
            groupBox2.Controls.Add(DTPdatadecarregamentodi);
            groupBox2.Location = new Point(1059, 215);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(318, 228);
            groupBox2.TabIndex = 17;
            groupBox2.TabStop = false;
            groupBox2.Text = "DI";
            // 
            // label18
            // 
            label18.Anchor = AnchorStyles.None;
            label18.AutoSize = true;
            label18.Font = new Font("Segoe UI", 10F);
            label18.Location = new Point(181, 166);
            label18.Name = "label18";
            label18.Size = new Size(105, 19);
            label18.TabIndex = 304;
            label18.Text = "Data de Minuta";
            // 
            // dtpDataMinuta
            // 
            dtpDataMinuta.Anchor = AnchorStyles.None;
            dtpDataMinuta.Format = DateTimePickerFormat.Short;
            dtpDataMinuta.Location = new Point(166, 188);
            dtpDataMinuta.MinDate = new DateTime(2023, 8, 22, 0, 0, 0, 0);
            dtpDataMinuta.Name = "dtpDataMinuta";
            dtpDataMinuta.Size = new Size(135, 23);
            dtpDataMinuta.TabIndex = 303;
            dtpDataMinuta.Value = new DateTime(2023, 8, 22, 0, 0, 0, 0);
            // 
            // label19
            // 
            label19.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label19.AutoSize = true;
            label19.Font = new Font("Segoe UI", 10F);
            label19.Location = new Point(16, 120);
            label19.Name = "label19";
            label19.Size = new Size(144, 19);
            label19.TabIndex = 152;
            label19.Text = "Data de Desembaraço";
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 11F);
            label1.Location = new Point(28, 72);
            label1.Name = "label1";
            label1.Size = new Size(121, 20);
            label1.TabIndex = 153;
            label1.Text = "Data de Registro";
            // 
            // DTPdatadedesembaracodi
            // 
            DTPdatadedesembaracodi.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            DTPdatadedesembaracodi.Format = DateTimePickerFormat.Short;
            DTPdatadedesembaracodi.Location = new Point(21, 142);
            DTPdatadedesembaracodi.MinDate = new DateTime(2024, 10, 17, 0, 0, 0, 0);
            DTPdatadedesembaracodi.Name = "DTPdatadedesembaracodi";
            DTPdatadedesembaracodi.Size = new Size(135, 23);
            DTPdatadedesembaracodi.TabIndex = 150;
            DTPdatadedesembaracodi.Value = new DateTime(2024, 10, 17, 0, 0, 0, 0);
            DTPdatadedesembaracodi.ValueChanged += DateTimePicker_OnValueChanged;
            // 
            // DTPdataderegistrodi
            // 
            DTPdataderegistrodi.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            DTPdataderegistrodi.Format = DateTimePickerFormat.Short;
            DTPdataderegistrodi.Location = new Point(21, 94);
            DTPdataderegistrodi.MinDate = new DateTime(2024, 10, 17, 0, 0, 0, 0);
            DTPdataderegistrodi.Name = "DTPdataderegistrodi";
            DTPdataderegistrodi.Size = new Size(135, 23);
            DTPdataderegistrodi.TabIndex = 149;
            DTPdataderegistrodi.Value = new DateTime(2024, 10, 17, 0, 0, 0, 0);
            DTPdataderegistrodi.ValueChanged += DateTimePicker_OnValueChanged;
            // 
            // LBLinspecao
            // 
            LBLinspecao.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            LBLinspecao.AutoSize = true;
            LBLinspecao.Font = new Font("Segoe UI", 11F);
            LBLinspecao.Location = new Point(54, 165);
            LBLinspecao.Name = "LBLinspecao";
            LBLinspecao.Size = new Size(68, 20);
            LBLinspecao.TabIndex = 302;
            LBLinspecao.Text = "Inspeção";
            // 
            // TXTdi
            // 
            TXTdi.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            TXTdi.Cursor = Cursors.IBeam;
            TXTdi.DataBindings.Add(new Binding("Text", bsModificaProcesso, "DI", true));
            TXTdi.Location = new Point(93, 46);
            TXTdi.Name = "TXTdi";
            TXTdi.Size = new Size(135, 23);
            TXTdi.TabIndex = 148;
            // 
            // label21
            // 
            label21.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label21.AutoSize = true;
            label21.Font = new Font("Segoe UI", 12F);
            label21.Location = new Point(146, 22);
            label21.Name = "label21";
            label21.Size = new Size(28, 21);
            label21.TabIndex = 151;
            label21.Text = "N°";
            // 
            // label23
            // 
            label23.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label23.AutoSize = true;
            label23.Font = new Font("Segoe UI", 11F);
            label23.Location = new Point(178, 72);
            label23.Name = "label23";
            label23.Size = new Size(111, 20);
            label23.TabIndex = 147;
            label23.Text = "Parametrização";
            // 
            // DTPdatadeinspecao
            // 
            DTPdatadeinspecao.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            DTPdatadeinspecao.Format = DateTimePickerFormat.Short;
            DTPdatadeinspecao.Location = new Point(21, 188);
            DTPdatadeinspecao.MinDate = new DateTime(2023, 8, 22, 0, 0, 0, 0);
            DTPdatadeinspecao.Name = "DTPdatadeinspecao";
            DTPdatadeinspecao.Size = new Size(135, 23);
            DTPdatadeinspecao.TabIndex = 23;
            DTPdatadeinspecao.Value = new DateTime(2023, 8, 22, 0, 0, 0, 0);
            DTPdatadeinspecao.ValueChanged += DateTimePicker_OnValueChanged;
            // 
            // label6
            // 
            label6.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 10F);
            label6.Location = new Point(159, 119);
            label6.Name = "label6";
            label6.Size = new Size(149, 19);
            label6.TabIndex = 144;
            label6.Text = "Data de Carregamento";
            // 
            // CBparametrizacaodi
            // 
            CBparametrizacaodi.AutoCompleteMode = AutoCompleteMode.Suggest;
            CBparametrizacaodi.DataBindings.Add(new Binding("Text", bsModificaProcesso, "ParametrizacaoDI", true));
            CBparametrizacaodi.DataBindings.Add(new Binding("SelectedItem", bsModificaProcesso, "ParametrizacaoDI", true));
            CBparametrizacaodi.DataBindings.Add(new Binding("SelectedValue", bsModificaProcesso, "ParametrizacaoDI", true));
            CBparametrizacaodi.FormattingEnabled = true;
            CBparametrizacaodi.Items.AddRange(new object[] { "Verde", "Amarelo", "Vermelho" });
            CBparametrizacaodi.Location = new Point(166, 93);
            CBparametrizacaodi.Name = "CBparametrizacaodi";
            CBparametrizacaodi.Size = new Size(135, 23);
            CBparametrizacaodi.TabIndex = 21;
            // 
            // DTPdatadecarregamentodi
            // 
            DTPdatadecarregamentodi.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            DTPdatadecarregamentodi.Format = DateTimePickerFormat.Short;
            DTPdatadecarregamentodi.Location = new Point(166, 142);
            DTPdatadecarregamentodi.MinDate = new DateTime(2024, 10, 17, 0, 0, 0, 0);
            DTPdatadecarregamentodi.Name = "DTPdatadecarregamentodi";
            DTPdatadecarregamentodi.Size = new Size(135, 23);
            DTPdatadecarregamentodi.TabIndex = 20;
            DTPdatadecarregamentodi.Value = new DateTime(2024, 10, 17, 0, 0, 0, 0);
            DTPdatadecarregamentodi.ValueChanged += DateTimePicker_OnValueChanged;
            // 
            // CBamostra
            // 
            CBamostra.Anchor = AnchorStyles.None;
            CBamostra.AutoSize = true;
            CBamostra.DataBindings.Add(new Binding("Checked", bsModificaProcesso, "Amostra", true));
            CBamostra.Font = new Font("Segoe UI", 12F);
            CBamostra.Location = new Point(957, 68);
            CBamostra.Name = "CBamostra";
            CBamostra.Size = new Size(88, 25);
            CBamostra.TabIndex = 133;
            CBamostra.Text = "Amostra";
            CBamostra.UseVisualStyleBackColor = true;
            // 
            // CBdesovado
            // 
            CBdesovado.Anchor = AnchorStyles.None;
            CBdesovado.AutoSize = true;
            CBdesovado.DataBindings.Add(new Binding("Checked", bsModificaProcesso, "Desovado", true));
            CBdesovado.Font = new Font("Segoe UI", 12F);
            CBdesovado.Location = new Point(957, 99);
            CBdesovado.Name = "CBdesovado";
            CBdesovado.Size = new Size(98, 25);
            CBdesovado.TabIndex = 100;
            CBdesovado.Text = "Desovado";
            CBdesovado.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            button1.Anchor = AnchorStyles.None;
            button1.Cursor = Cursors.Hand;
            button1.Location = new Point(968, 595);
            button1.Name = "button1";
            button1.Size = new Size(81, 37);
            button1.TabIndex = 27;
            button1.Text = "Cancelar";
            button1.UseVisualStyleBackColor = true;
            button1.Click += btnCancelar_Click;
            // 
            // groupBox1
            // 
            groupBox1.Anchor = AnchorStyles.None;
            groupBox1.Controls.Add(CBmapa);
            groupBox1.Controls.Add(CBimetro);
            groupBox1.Controls.Add(CBanvisa);
            groupBox1.Controls.Add(CBibama);
            groupBox1.Controls.Add(CBdecex);
            groupBox1.Location = new Point(515, 557);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(358, 75);
            groupBox1.TabIndex = 128;
            groupBox1.TabStop = false;
            groupBox1.Text = "Orgãos Anuentes";
            // 
            // CBmapa
            // 
            CBmapa.AutoSize = true;
            CBmapa.DataBindings.Add(new Binding("Checked", bsModificaProcesso, "TMapa", true));
            CBmapa.Location = new Point(27, 32);
            CBmapa.Name = "CBmapa";
            CBmapa.Size = new Size(56, 19);
            CBmapa.TabIndex = 30;
            CBmapa.Text = "Mapa";
            CBmapa.UseVisualStyleBackColor = true;
            CBmapa.CheckedChanged += CBmapa_CheckedChanged;
            // 
            // CBimetro
            // 
            CBimetro.AutoSize = true;
            CBimetro.DataBindings.Add(new Binding("Checked", bsModificaProcesso, "TImetro", true));
            CBimetro.Location = new Point(283, 32);
            CBimetro.Name = "CBimetro";
            CBimetro.Size = new Size(61, 19);
            CBimetro.TabIndex = 34;
            CBimetro.Text = "Imetro";
            CBimetro.UseVisualStyleBackColor = true;
            // 
            // CBanvisa
            // 
            CBanvisa.AutoSize = true;
            CBanvisa.DataBindings.Add(new Binding("Checked", bsModificaProcesso, "TAnvisa", true));
            CBanvisa.Location = new Point(89, 32);
            CBanvisa.Name = "CBanvisa";
            CBanvisa.Size = new Size(61, 19);
            CBanvisa.TabIndex = 31;
            CBanvisa.Text = "Anvisa";
            CBanvisa.UseVisualStyleBackColor = true;
            // 
            // CBibama
            // 
            CBibama.AutoSize = true;
            CBibama.DataBindings.Add(new Binding("Checked", bsModificaProcesso, "TIbama", true));
            CBibama.Location = new Point(220, 32);
            CBibama.Name = "CBibama";
            CBibama.Size = new Size(59, 19);
            CBibama.TabIndex = 33;
            CBibama.Text = "Ibama";
            CBibama.UseVisualStyleBackColor = true;
            // 
            // CBdecex
            // 
            CBdecex.AutoSize = true;
            CBdecex.DataBindings.Add(new Binding("Checked", bsModificaProcesso, "TDecex", true));
            CBdecex.Location = new Point(156, 32);
            CBdecex.Name = "CBdecex";
            CBdecex.Size = new Size(57, 19);
            CBdecex.TabIndex = 32;
            CBdecex.Text = "Decex";
            CBdecex.UseVisualStyleBackColor = true;
            // 
            // btnAdiciona
            // 
            btnAdiciona.Anchor = AnchorStyles.None;
            btnAdiciona.Cursor = Cursors.Hand;
            btnAdiciona.Location = new Point(968, 558);
            btnAdiciona.Name = "btnAdiciona";
            btnAdiciona.Size = new Size(81, 37);
            btnAdiciona.TabIndex = 28;
            btnAdiciona.Text = "Ok";
            btnAdiciona.UseVisualStyleBackColor = true;
            btnAdiciona.Click += btnAdiciona_Click;
            // 
            // NUMfreetime
            // 
            NUMfreetime.Anchor = AnchorStyles.None;
            NUMfreetime.DataBindings.Add(new Binding("Value", bsModificaProcesso, "FreeTime", true));
            NUMfreetime.Location = new Point(963, 39);
            NUMfreetime.Name = "NUMfreetime";
            NUMfreetime.Size = new Size(77, 23);
            NUMfreetime.TabIndex = 9;
            // 
            // label17
            // 
            label17.Anchor = AnchorStyles.None;
            label17.AutoSize = true;
            label17.Font = new Font("Segoe UI", 12F);
            label17.Location = new Point(40, 63);
            label17.Name = "label17";
            label17.Size = new Size(89, 21);
            label17.TabIndex = 287;
            label17.Text = "Importador";
            // 
            // TXTimportador
            // 
            TXTimportador.Anchor = AnchorStyles.None;
            TXTimportador.Cursor = Cursors.IBeam;
            TXTimportador.DataBindings.Add(new Binding("Text", bsModificaProcesso, "Importador", true));
            TXTimportador.Location = new Point(135, 62);
            TXTimportador.Name = "TXTimportador";
            TXTimportador.Size = new Size(167, 23);
            TXTimportador.TabIndex = 3;
            // 
            // label14
            // 
            label14.Anchor = AnchorStyles.None;
            label14.AutoSize = true;
            label14.Font = new Font("Segoe UI", 12F);
            label14.Location = new Point(40, 103);
            label14.Name = "label14";
            label14.Size = new Size(125, 21);
            label14.TabIndex = 285;
            label14.Text = "Porto de Destino";
            // 
            // TXTportodedestino
            // 
            TXTportodedestino.Anchor = AnchorStyles.None;
            TXTportodedestino.Cursor = Cursors.IBeam;
            TXTportodedestino.DataBindings.Add(new Binding("Text", bsModificaProcesso, "PortoDestino", true));
            TXTportodedestino.Location = new Point(171, 102);
            TXTportodedestino.Name = "TXTportodedestino";
            TXTportodedestino.Size = new Size(131, 23);
            TXTportodedestino.TabIndex = 7;
            // 
            // label15
            // 
            label15.Anchor = AnchorStyles.None;
            label15.AutoSize = true;
            label15.Font = new Font("Segoe UI", 12F);
            label15.Location = new Point(962, 16);
            label15.Name = "label15";
            label15.Size = new Size(78, 21);
            label15.TabIndex = 283;
            label15.Text = "Free Time";
            // 
            // label12
            // 
            label12.Anchor = AnchorStyles.None;
            label12.AutoSize = true;
            label12.Font = new Font("Segoe UI", 12F);
            label12.Location = new Point(330, 144);
            label12.Name = "label12";
            label12.Size = new Size(72, 21);
            label12.TabIndex = 282;
            label12.Text = "Armador";
            // 
            // label11
            // 
            label11.Anchor = AnchorStyles.None;
            label11.AutoSize = true;
            label11.Font = new Font("Segoe UI", 12F);
            label11.Location = new Point(330, 25);
            label11.Name = "label11";
            label11.Size = new Size(49, 21);
            label11.TabIndex = 281;
            label11.Text = "S. Ref";
            // 
            // label9
            // 
            label9.Anchor = AnchorStyles.None;
            label9.AutoSize = true;
            label9.Font = new Font("Segoe UI", 12F);
            label9.Location = new Point(40, 25);
            label9.Name = "label9";
            label9.Size = new Size(66, 21);
            label9.TabIndex = 280;
            label9.Text = "Ref. Usa";
            // 
            // TXTsr
            // 
            TXTsr.Anchor = AnchorStyles.None;
            TXTsr.Cursor = Cursors.IBeam;
            TXTsr.DataBindings.Add(new Binding("Text", bsModificaProcesso, "SR", true));
            TXTsr.Location = new Point(385, 24);
            TXTsr.Name = "TXTsr";
            TXTsr.Size = new Size(210, 23);
            TXTsr.TabIndex = 2;
            // 
            // label5
            // 
            label5.Anchor = AnchorStyles.None;
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 12F);
            label5.Location = new Point(621, 63);
            label5.Name = "label5";
            label5.Size = new Size(111, 21);
            label5.TabIndex = 277;
            label5.Text = "Conhecimento";
            // 
            // txtConhecimento
            // 
            txtConhecimento.Anchor = AnchorStyles.None;
            txtConhecimento.Cursor = Cursors.IBeam;
            txtConhecimento.DataBindings.Add(new Binding("Text", bsModificaProcesso, "Conhecimento", true));
            txtConhecimento.Location = new Point(738, 62);
            txtConhecimento.Name = "txtConhecimento";
            txtConhecimento.Size = new Size(198, 23);
            txtConhecimento.TabIndex = 10;
            // 
            // txtArmador
            // 
            txtArmador.Anchor = AnchorStyles.None;
            txtArmador.Cursor = Cursors.IBeam;
            txtArmador.DataBindings.Add(new Binding("Text", bsModificaProcesso, "Armador", true));
            txtArmador.Location = new Point(408, 143);
            txtArmador.Name = "txtArmador";
            txtArmador.Size = new Size(187, 23);
            txtArmador.TabIndex = 11;
            // 
            // label7
            // 
            label7.Anchor = AnchorStyles.None;
            label7.AutoSize = true;
            label7.Font = new Font("Segoe UI", 12F);
            label7.Location = new Point(621, 144);
            label7.Name = "label7";
            label7.Size = new Size(66, 21);
            label7.TabIndex = 274;
            label7.Text = "Produto";
            // 
            // TXTProduto
            // 
            TXTProduto.Anchor = AnchorStyles.None;
            TXTProduto.Cursor = Cursors.IBeam;
            TXTProduto.DataBindings.Add(new Binding("Text", bsModificaProcesso, "Produto", true));
            TXTProduto.Location = new Point(693, 143);
            TXTProduto.Name = "TXTProduto";
            TXTProduto.Size = new Size(347, 23);
            TXTProduto.TabIndex = 6;
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.None;
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F);
            label2.Location = new Point(621, 103);
            label2.Name = "label2";
            label2.Size = new Size(37, 21);
            label2.TabIndex = 270;
            label2.Text = "FLO";
            // 
            // TXTflo
            // 
            TXTflo.Anchor = AnchorStyles.None;
            TXTflo.Cursor = Cursors.IBeam;
            TXTflo.DataBindings.Add(new Binding("Text", bsModificaProcesso, "FLO", true));
            TXTflo.Location = new Point(664, 102);
            TXTflo.Name = "TXTflo";
            TXTflo.Size = new Size(272, 23);
            TXTflo.TabIndex = 9;
            // 
            // Exportador
            // 
            Exportador.Anchor = AnchorStyles.None;
            Exportador.AutoSize = true;
            Exportador.Font = new Font("Segoe UI", 12F);
            Exportador.Location = new Point(330, 63);
            Exportador.Name = "Exportador";
            Exportador.Size = new Size(86, 21);
            Exportador.TabIndex = 268;
            Exportador.Text = "Exportador";
            // 
            // TXTexportador
            // 
            TXTexportador.Anchor = AnchorStyles.None;
            TXTexportador.Cursor = Cursors.IBeam;
            TXTexportador.DataBindings.Add(new Binding("Text", bsModificaProcesso, "Exportador", true));
            TXTexportador.Location = new Point(422, 62);
            TXTexportador.Name = "TXTexportador";
            TXTexportador.Size = new Size(173, 23);
            TXTexportador.TabIndex = 5;
            // 
            // LBLdatadeatracacao
            // 
            LBLdatadeatracacao.Anchor = AnchorStyles.None;
            LBLdatadeatracacao.AutoSize = true;
            LBLdatadeatracacao.Font = new Font("Segoe UI", 10F);
            LBLdatadeatracacao.Location = new Point(1235, 457);
            LBLdatadeatracacao.Name = "LBLdatadeatracacao";
            LBLdatadeatracacao.Size = new Size(115, 19);
            LBLdatadeatracacao.TabIndex = 301;
            LBLdatadeatracacao.Text = "Data de Chegada";
            // 
            // LBLdatadeembarque
            // 
            LBLdatadeembarque.Anchor = AnchorStyles.None;
            LBLdatadeembarque.AutoSize = true;
            LBLdatadeembarque.Font = new Font("Segoe UI", 10F);
            LBLdatadeembarque.Location = new Point(1087, 457);
            LBLdatadeembarque.Name = "LBLdatadeembarque";
            LBLdatadeembarque.Size = new Size(123, 19);
            LBLdatadeembarque.TabIndex = 300;
            LBLdatadeembarque.Text = "Data de Embarque";
            // 
            // DTPdatadeatracacao
            // 
            DTPdatadeatracacao.Anchor = AnchorStyles.None;
            DTPdatadeatracacao.Format = DateTimePickerFormat.Short;
            DTPdatadeatracacao.Location = new Point(1225, 479);
            DTPdatadeatracacao.MinDate = new DateTime(2023, 8, 22, 0, 0, 0, 0);
            DTPdatadeatracacao.Name = "DTPdatadeatracacao";
            DTPdatadeatracacao.Size = new Size(135, 23);
            DTPdatadeatracacao.TabIndex = 22;
            DTPdatadeatracacao.Value = new DateTime(2023, 8, 22, 0, 0, 0, 0);
            DTPdatadeatracacao.ValueChanged += DateTimePicker_OnValueChanged;
            // 
            // DTPdatadeembarque
            // 
            DTPdatadeembarque.Anchor = AnchorStyles.None;
            DTPdatadeembarque.Format = DateTimePickerFormat.Short;
            DTPdatadeembarque.Location = new Point(1080, 479);
            DTPdatadeembarque.MinDate = new DateTime(2023, 8, 22, 0, 0, 0, 0);
            DTPdatadeembarque.Name = "DTPdatadeembarque";
            DTPdatadeembarque.Size = new Size(135, 23);
            DTPdatadeembarque.TabIndex = 24;
            DTPdatadeembarque.Value = new DateTime(2023, 8, 22, 0, 0, 0, 0);
            DTPdatadeembarque.ValueChanged += DateTimePicker_OnValueChanged;
            // 
            // label10
            // 
            label10.Anchor = AnchorStyles.None;
            label10.AutoSize = true;
            label10.Font = new Font("Segoe UI", 12F, FontStyle.Underline);
            label10.Location = new Point(36, 180);
            label10.Name = "label10";
            label10.Size = new Size(140, 21);
            label10.TabIndex = 292;
            label10.Text = "Status do Processo";
            // 
            // TXTstatusdoprocesso
            // 
            TXTstatusdoprocesso.Anchor = AnchorStyles.None;
            TXTstatusdoprocesso.DataBindings.Add(new Binding("Text", bsModificaProcesso, "StatusDoProcesso", true));
            TXTstatusdoprocesso.Location = new Point(50, 204);
            TXTstatusdoprocesso.Multiline = true;
            TXTstatusdoprocesso.Name = "TXTstatusdoprocesso";
            TXTstatusdoprocesso.Size = new Size(430, 325);
            TXTstatusdoprocesso.TabIndex = 25;
            // 
            // label8
            // 
            label8.Anchor = AnchorStyles.None;
            label8.AutoSize = true;
            label8.Font = new Font("Segoe UI", 12F, FontStyle.Underline);
            label8.Location = new Point(40, 532);
            label8.Name = "label8";
            label8.Size = new Size(80, 21);
            label8.TabIndex = 290;
            label8.Text = "Pendência";
            // 
            // TXTpendencia
            // 
            TXTpendencia.Anchor = AnchorStyles.None;
            TXTpendencia.DataBindings.Add(new Binding("Text", bsModificaProcesso, "Pendencia", true));
            TXTpendencia.Location = new Point(50, 556);
            TXTpendencia.Multiline = true;
            TXTpendencia.Name = "TXTpendencia";
            TXTpendencia.Size = new Size(430, 76);
            TXTpendencia.TabIndex = 26;
            // 
            // label13
            // 
            label13.Anchor = AnchorStyles.None;
            label13.AutoSize = true;
            label13.Font = new Font("Segoe UI", 12F);
            label13.Location = new Point(330, 103);
            label13.Name = "label13";
            label13.Size = new Size(60, 21);
            label13.TabIndex = 306;
            label13.Text = "Veículo";
            // 
            // txtVeiculo
            // 
            txtVeiculo.Anchor = AnchorStyles.None;
            txtVeiculo.Cursor = Cursors.IBeam;
            txtVeiculo.DataBindings.Add(new Binding("Text", bsModificaProcesso, "Veiculo", true));
            txtVeiculo.Location = new Point(396, 102);
            txtVeiculo.Name = "txtVeiculo";
            txtVeiculo.Size = new Size(199, 23);
            txtVeiculo.TabIndex = 4;
            // 
            // checkedListBox1
            // 
            checkedListBox1.Anchor = AnchorStyles.None;
            checkedListBox1.BorderStyle = BorderStyle.FixedSingle;
            checkedListBox1.FormattingEnabled = true;
            checkedListBox1.Items.AddRange(new object[] { "BL", "Fatura", "Packing List", "CO", "Fito", "CSI", "CA", "CF" });
            checkedListBox1.Location = new Point(1107, 39);
            checkedListBox1.Name = "checkedListBox1";
            checkedListBox1.Size = new Size(108, 110);
            checkedListBox1.TabIndex = 307;
            // 
            // label16
            // 
            label16.Anchor = AnchorStyles.None;
            label16.AutoSize = true;
            label16.Font = new Font("Segoe UI", 11F);
            label16.Location = new Point(1104, 16);
            label16.Name = "label16";
            label16.Size = new Size(115, 20);
            label16.TabIndex = 308;
            label16.Text = "Docs Recebidos";
            // 
            // checkedListBox2
            // 
            checkedListBox2.Anchor = AnchorStyles.None;
            checkedListBox2.BorderStyle = BorderStyle.FixedSingle;
            checkedListBox2.FormattingEnabled = true;
            checkedListBox2.Items.AddRange(new object[] { "DHL", "UPS", "Correio", "Fedex", "Daytona" });
            checkedListBox2.Location = new Point(1242, 39);
            checkedListBox2.Name = "checkedListBox2";
            checkedListBox2.Size = new Size(79, 110);
            checkedListBox2.TabIndex = 309;
            checkedListBox2.ItemCheck += checkedListBox2_ItemCheck;
            // 
            // label24
            // 
            label24.Anchor = AnchorStyles.None;
            label24.AutoSize = true;
            label24.Font = new Font("Segoe UI", 11F);
            label24.Location = new Point(1240, 16);
            label24.Name = "label24";
            label24.Size = new Size(82, 20);
            label24.TabIndex = 310;
            label24.Text = "Forma Rec.";
            // 
            // label25
            // 
            label25.Anchor = AnchorStyles.None;
            label25.AutoSize = true;
            label25.Font = new Font("Segoe UI", 10F);
            label25.Location = new Point(1232, 164);
            label25.Name = "label25";
            label25.Size = new Size(124, 19);
            label25.TabIndex = 312;
            label25.Text = "Data Rec. Originais";
            // 
            // DTPDataRecOriginais
            // 
            DTPDataRecOriginais.Anchor = AnchorStyles.None;
            DTPDataRecOriginais.Format = DateTimePickerFormat.Short;
            DTPDataRecOriginais.Location = new Point(1225, 186);
            DTPDataRecOriginais.MinDate = new DateTime(2023, 8, 22, 0, 0, 0, 0);
            DTPDataRecOriginais.Name = "DTPDataRecOriginais";
            DTPDataRecOriginais.Size = new Size(135, 23);
            DTPDataRecOriginais.TabIndex = 311;
            DTPDataRecOriginais.Value = new DateTime(2023, 8, 22, 0, 0, 0, 0);
            DTPDataRecOriginais.ValueChanged += DateTimePicker_OnValueChanged;
            // 
            // TXTnr
            // 
            TXTnr.Anchor = AnchorStyles.None;
            TXTnr.DataBindings.Add(new Binding("Text", bsModificaProcesso, "Ref_USA", true));
            TXTnr.Location = new Point(112, 24);
            TXTnr.Mask = "0000/0000";
            TXTnr.Name = "TXTnr";
            TXTnr.Size = new Size(190, 23);
            TXTnr.TabIndex = 313;
            // 
            // groupBox4
            // 
            groupBox4.Anchor = AnchorStyles.None;
            groupBox4.Controls.Add(button3);
            groupBox4.Controls.Add(flpLis);
            groupBox4.Location = new Point(515, 180);
            groupBox4.Name = "groupBox4";
            groupBox4.Size = new Size(534, 371);
            groupBox4.TabIndex = 391;
            groupBox4.TabStop = false;
            groupBox4.Text = "LI";
            // 
            // button3
            // 
            button3.Font = new Font("Segoe UI", 8F);
            button3.Location = new Point(26, -4);
            button3.Name = "button3";
            button3.Size = new Size(20, 20);
            button3.TabIndex = 381;
            button3.Text = "+";
            button3.UseVisualStyleBackColor = true;
            button3.Click += btnAdicionarLi_Click;
            // 
            // flpLis
            // 
            flpLis.Location = new Point(6, 22);
            flpLis.Name = "flpLis";
            flpLis.Size = new Size(522, 339);
            flpLis.TabIndex = 380;
            // 
            // txtTerminal
            // 
            txtTerminal.Anchor = AnchorStyles.None;
            txtTerminal.Cursor = Cursors.IBeam;
            txtTerminal.DataBindings.Add(new Binding("Text", bsModificaProcesso, "Terminal", true));
            txtTerminal.Location = new Point(693, 24);
            txtTerminal.Name = "txtTerminal";
            txtTerminal.Size = new Size(243, 23);
            txtTerminal.TabIndex = 392;
            // 
            // label4
            // 
            label4.Anchor = AnchorStyles.None;
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 12F);
            label4.Location = new Point(621, 25);
            label4.Name = "label4";
            label4.Size = new Size(69, 21);
            label4.TabIndex = 393;
            label4.Text = "Terminal";
            // 
            // label20
            // 
            label20.Anchor = AnchorStyles.None;
            label20.AutoSize = true;
            label20.Font = new Font("Segoe UI", 10F);
            label20.Location = new Point(161, 18);
            label20.Name = "label20";
            label20.Size = new Size(136, 19);
            label20.TabIndex = 397;
            label20.Text = "Vencimento LI/LPCO";
            // 
            // label22
            // 
            label22.Anchor = AnchorStyles.None;
            label22.AutoSize = true;
            label22.Font = new Font("Segoe UI", 10F);
            label22.Location = new Point(12, 18);
            label22.Name = "label22";
            label22.Size = new Size(144, 19);
            label22.TabIndex = 396;
            label22.Text = "Vencimento Free Time";
            // 
            // dtpVencimentoLI_LPCO
            // 
            dtpVencimentoLI_LPCO.Anchor = AnchorStyles.None;
            dtpVencimentoLI_LPCO.DataBindings.Add(new Binding("Value", bsModificaProcesso, "VencimentoLI_LPCO", true));
            dtpVencimentoLI_LPCO.Enabled = false;
            dtpVencimentoLI_LPCO.Format = DateTimePickerFormat.Short;
            dtpVencimentoLI_LPCO.Location = new Point(162, 40);
            dtpVencimentoLI_LPCO.MinDate = new DateTime(2023, 8, 22, 0, 0, 0, 0);
            dtpVencimentoLI_LPCO.Name = "dtpVencimentoLI_LPCO";
            dtpVencimentoLI_LPCO.Size = new Size(135, 23);
            dtpVencimentoLI_LPCO.TabIndex = 394;
            dtpVencimentoLI_LPCO.Value = new DateTime(2023, 8, 22, 0, 0, 0, 0);
            // 
            // dtpVencimentoFreeTime
            // 
            dtpVencimentoFreeTime.Anchor = AnchorStyles.None;
            dtpVencimentoFreeTime.DataBindings.Add(new Binding("Value", bsModificaProcesso, "VencimentoFreeTime", true));
            dtpVencimentoFreeTime.Enabled = false;
            dtpVencimentoFreeTime.Format = DateTimePickerFormat.Short;
            dtpVencimentoFreeTime.Location = new Point(17, 40);
            dtpVencimentoFreeTime.MinDate = new DateTime(2023, 8, 22, 0, 0, 0, 0);
            dtpVencimentoFreeTime.Name = "dtpVencimentoFreeTime";
            dtpVencimentoFreeTime.Size = new Size(135, 23);
            dtpVencimentoFreeTime.TabIndex = 395;
            dtpVencimentoFreeTime.Value = new DateTime(2023, 8, 22, 0, 0, 0, 0);
            // 
            // label26
            // 
            label26.Anchor = AnchorStyles.None;
            label26.AutoSize = true;
            label26.Font = new Font("Segoe UI", 10F);
            label26.Location = new Point(98, 66);
            label26.Name = "label26";
            label26.Size = new Size(114, 19);
            label26.TabIndex = 401;
            label26.Text = "Vencimento FMA";
            // 
            // dtpVencimentoFMA
            // 
            dtpVencimentoFMA.Anchor = AnchorStyles.None;
            dtpVencimentoFMA.DataBindings.Add(new Binding("Value", bsModificaProcesso, "VencimentoFMA", true));
            dtpVencimentoFMA.Enabled = false;
            dtpVencimentoFMA.Format = DateTimePickerFormat.Short;
            dtpVencimentoFMA.Location = new Point(88, 88);
            dtpVencimentoFMA.MinDate = new DateTime(2023, 8, 22, 0, 0, 0, 0);
            dtpVencimentoFMA.Name = "dtpVencimentoFMA";
            dtpVencimentoFMA.Size = new Size(135, 23);
            dtpVencimentoFMA.TabIndex = 398;
            dtpVencimentoFMA.Value = new DateTime(2023, 8, 22, 0, 0, 0, 0);
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(label22);
            groupBox3.Controls.Add(label26);
            groupBox3.Controls.Add(dtpVencimentoFreeTime);
            groupBox3.Controls.Add(dtpVencimentoFMA);
            groupBox3.Controls.Add(dtpVencimentoLI_LPCO);
            groupBox3.Controls.Add(label20);
            groupBox3.Location = new Point(1061, 508);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(316, 124);
            groupBox3.TabIndex = 402;
            groupBox3.TabStop = false;
            groupBox3.Text = "Vencimentos";
            // 
            // label27
            // 
            label27.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label27.AutoSize = true;
            label27.Font = new Font("Segoe UI", 12F);
            label27.Location = new Point(1113, 162);
            label27.Name = "label27";
            label27.Size = new Size(53, 21);
            label27.TabIndex = 306;
            label27.Text = "Marca";
            // 
            // cbMarca
            // 
            cbMarca.AutoCompleteMode = AutoCompleteMode.Suggest;
            cbMarca.FormattingEnabled = true;
            cbMarca.Items.AddRange(new object[] { "20 DRY", "40 DRY", "20 RF", "40 RF", "20 HC", "40 HC", "Sacos", "Caixas", "Pallets" });
            cbMarca.Location = new Point(1133, 186);
            cbMarca.Name = "cbMarca";
            cbMarca.Size = new Size(82, 23);
            cbMarca.TabIndex = 305;
            // 
            // numMarca
            // 
            numMarca.Anchor = AnchorStyles.None;
            numMarca.DataBindings.Add(new Binding("Value", bsModificaProcesso, "FreeTime", true));
            numMarca.Location = new Point(1080, 186);
            numMarca.Name = "numMarca";
            numMarca.Size = new Size(54, 23);
            numMarca.TabIndex = 403;
            // 
            // txtOrigem
            // 
            txtOrigem.Anchor = AnchorStyles.None;
            txtOrigem.Cursor = Cursors.IBeam;
            txtOrigem.DataBindings.Add(new Binding("Text", bsModificaProcesso, "Origem", true));
            txtOrigem.Location = new Point(109, 144);
            txtOrigem.Name = "txtOrigem";
            txtOrigem.Size = new Size(193, 23);
            txtOrigem.TabIndex = 8;
            // 
            // label3
            // 
            label3.Anchor = AnchorStyles.None;
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 12F);
            label3.Location = new Point(40, 145);
            label3.Name = "label3";
            label3.Size = new Size(63, 21);
            label3.TabIndex = 272;
            label3.Text = "Origem";
            // 
            // btnCapa
            // 
            btnCapa.Anchor = AnchorStyles.None;
            btnCapa.Cursor = Cursors.Hand;
            btnCapa.Location = new Point(881, 595);
            btnCapa.Name = "btnCapa";
            btnCapa.Size = new Size(81, 37);
            btnCapa.TabIndex = 404;
            btnCapa.Text = "Capa";
            btnCapa.UseVisualStyleBackColor = true;
            btnCapa.Click += btnCapa_Click;
            // 
            // btnRelatorio
            // 
            btnRelatorio.Anchor = AnchorStyles.None;
            btnRelatorio.Cursor = Cursors.Hand;
            btnRelatorio.Location = new Point(881, 558);
            btnRelatorio.Name = "btnRelatorio";
            btnRelatorio.Size = new Size(81, 37);
            btnRelatorio.TabIndex = 405;
            btnRelatorio.Text = "Relatório";
            btnRelatorio.UseVisualStyleBackColor = true;
            btnRelatorio.Click += btnRelatorio_Click;
            // 
            // FrmModificaProcesso
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1409, 647);
            Controls.Add(btnCapa);
            Controls.Add(btnRelatorio);
            Controls.Add(numMarca);
            Controls.Add(cbMarca);
            Controls.Add(groupBox3);
            Controls.Add(label27);
            Controls.Add(label4);
            Controls.Add(txtTerminal);
            Controls.Add(groupBox4);
            Controls.Add(TXTnr);
            Controls.Add(label25);
            Controls.Add(DTPDataRecOriginais);
            Controls.Add(label24);
            Controls.Add(checkedListBox2);
            Controls.Add(label16);
            Controls.Add(checkedListBox1);
            Controls.Add(label13);
            Controls.Add(txtVeiculo);
            Controls.Add(LBLdatadeatracacao);
            Controls.Add(LBLdatadeembarque);
            Controls.Add(DTPdatadeatracacao);
            Controls.Add(DTPdatadeembarque);
            Controls.Add(label10);
            Controls.Add(TXTstatusdoprocesso);
            Controls.Add(label8);
            Controls.Add(TXTpendencia);
            Controls.Add(NUMfreetime);
            Controls.Add(label17);
            Controls.Add(TXTimportador);
            Controls.Add(label14);
            Controls.Add(TXTportodedestino);
            Controls.Add(label15);
            Controls.Add(label12);
            Controls.Add(label11);
            Controls.Add(label9);
            Controls.Add(TXTsr);
            Controls.Add(label5);
            Controls.Add(txtConhecimento);
            Controls.Add(txtArmador);
            Controls.Add(label7);
            Controls.Add(TXTProduto);
            Controls.Add(label3);
            Controls.Add(txtOrigem);
            Controls.Add(label2);
            Controls.Add(TXTflo);
            Controls.Add(Exportador);
            Controls.Add(TXTexportador);
            Controls.Add(groupBox2);
            Controls.Add(CBamostra);
            Controls.Add(CBdesovado);
            Controls.Add(button1);
            Controls.Add(groupBox1);
            Controls.Add(btnAdiciona);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "FrmModificaProcesso";
            Text = "Processo";
            Load += FrmModificaProcesso_Load;
            ((System.ComponentModel.ISupportInitialize)bsModificaProcesso).EndInit();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)NUMfreetime).EndInit();
            groupBox4.ResumeLayout(false);
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numMarca).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private BindingSource bsModificaProcesso;
        private GroupBox groupBox2;
        private ComboBox CBparametrizacaodi;
        private DateTimePicker DTPdatadecarregamentodi;
        private CheckBox CBamostra;
        private CheckBox CBdesovado;
        private Button button1;
        private GroupBox groupBox1;
        private CheckBox CBmapa;
        private CheckBox CBimetro;
        private CheckBox CBanvisa;
        private CheckBox CBibama;
        private CheckBox CBdecex;
        private Button btnAdiciona;
        private Label label6;
        private Label label23;
        private NumericUpDown NUMfreetime;
        private Label label17;
        private TextBox TXTimportador;
        private Label label14;
        private TextBox TXTportodedestino;
        private Label label15;
        private Label label12;
        private Label label11;
        private Label label9;
        private TextBox TXTsr;
        private Label label5;
        private TextBox txtConhecimento;
        private TextBox txtArmador;
        private Label label7;
        private TextBox TXTProduto;
        private Label label2;
        private TextBox TXTflo;
        private Label Exportador;
        private TextBox TXTexportador;
        private Label LBLinspecao;
        private Label LBLdatadeatracacao;
        private Label LBLdatadeembarque;
        private DateTimePicker DTPdatadeinspecao;
        private DateTimePicker DTPdatadeatracacao;
        private DateTimePicker DTPdatadeembarque;
        private Label label10;
        private TextBox TXTstatusdoprocesso;
        private Label label8;
        private TextBox TXTpendencia;
        private Label label13;
        private TextBox txtVeiculo;
        private Label label19;
        private Label label1;
        private DateTimePicker DTPdatadedesembaracodi;
        private DateTimePicker DTPdataderegistrodi;
        private TextBox TXTdi;
        private Label label21;
        private CheckedListBox checkedListBox1;
        private Label label16;
        private CheckedListBox checkedListBox2;
        private Label label24;
        private Label label25;
        private DateTimePicker DTPDataRecOriginais;
        private MaskedTextBox TXTnr;
        private Button button2;
        private GroupBox groupBox3;
        private GroupBox groupBox4;
        private Button button3;
        private FlowLayoutPanel flpLis;
        private TextBox txtTerminal;
        private Label label4;
        private Label label18;
        private DateTimePicker dtpDataMinuta;
        private Label label20;
        private Label label22;
        private DateTimePicker dtpVencimentoLI_LPCO;
        private DateTimePicker dtpVencimentoFreeTime;
        private Label label26;
        private DateTimePicker dtpVencimentoFMA;
        private Label label27;
        private ComboBox cbMarca;
        private NumericUpDown numMarca;
        private TextBox txtOrigem;
        private Label label3;
        private Button btnCapa;
        private Button btnRelatorio;
    }
}
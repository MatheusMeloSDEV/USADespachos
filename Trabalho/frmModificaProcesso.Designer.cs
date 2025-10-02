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
            groupBox2 = new GroupBox();
            label18 = new Label();
            dtpDataMinuta = new DateTimePicker();
            label19 = new Label();
            label1 = new Label();
            DTPdatadedesembaracodi = new DateTimePicker();
            DTPdataderegistrodi = new DateTimePicker();
            LBLinspecao = new Label();
            TXTdi = new TextBox();
            BsModificaProcesso = new BindingSource(components);
            label21 = new Label();
            label23 = new Label();
            DTPdatadeinspecao = new DateTimePicker();
            label6 = new Label();
            CBparametrizacaodi = new ComboBox();
            DTPdatadecarregamentodi = new DateTimePicker();
            CBamostra = new CheckBox();
            CBdesovado = new CheckBox();
            btnAdiciona = new Button();
            NUMfreetime = new NumericUpDown();
            label17 = new Label();
            TXTimportador = new TextBox();
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
            TXTstatusdoprocesso = new TextBox();
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
            btnCapa = new Button();
            btnRelatorio = new Button();
            TCLi = new TabControl();
            label32 = new Label();
            CbStatus = new ComboBox();
            LBLdatadeatracacao = new Label();
            LBLdatadeembarque = new Label();
            DTPdatadeatracacao = new DateTimePicker();
            DTPdatadeembarque = new DateTimePicker();
            groupBox1 = new GroupBox();
            TxtContainer = new TextBox();
            label8 = new Label();
            txtOrigem = new TextBox();
            label3 = new Label();
            TXTportodedestino = new TextBox();
            label14 = new Label();
            groupBox4 = new GroupBox();
            CbLiberado = new CheckBox();
            CbSelecionado = new CheckBox();
            CbResultadoLaboratorial = new CheckBox();
            CbPresencaCarga = new CheckBox();
            groupBox6 = new GroupBox();
            groupBox7 = new GroupBox();
            BtnLI = new Button();
            BtnExcluirLI = new Button();
            flowLayoutPanel1 = new FlowLayoutPanel();
            groupBox8 = new GroupBox();
            groupBox9 = new GroupBox();
            groupBox10 = new GroupBox();
            groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)BsModificaProcesso).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NUMfreetime).BeginInit();
            groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numMarca).BeginInit();
            groupBox1.SuspendLayout();
            groupBox4.SuspendLayout();
            groupBox6.SuspendLayout();
            groupBox7.SuspendLayout();
            groupBox8.SuspendLayout();
            groupBox9.SuspendLayout();
            groupBox10.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox2
            // 
            groupBox2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            groupBox2.AutoSizeMode = AutoSizeMode.GrowAndShrink;
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
            groupBox2.Location = new Point(1113, 274);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(318, 228);
            groupBox2.TabIndex = 17;
            groupBox2.TabStop = false;
            groupBox2.Text = "DI";
            // 
            // label18
            // 
            label18.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label18.AutoSize = true;
            label18.Font = new Font("Microsoft Sans Serif", 9.75F);
            label18.Location = new Point(185, 161);
            label18.Name = "label18";
            label18.Size = new Size(97, 16);
            label18.TabIndex = 304;
            label18.Text = "Data de Minuta";
            // 
            // dtpDataMinuta
            // 
            dtpDataMinuta.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dtpDataMinuta.Format = DateTimePickerFormat.Short;
            dtpDataMinuta.Location = new Point(166, 180);
            dtpDataMinuta.MinDate = new DateTime(2023, 8, 22, 0, 0, 0, 0);
            dtpDataMinuta.Name = "dtpDataMinuta";
            dtpDataMinuta.Size = new Size(135, 23);
            dtpDataMinuta.TabIndex = 303;
            dtpDataMinuta.Value = new DateTime(2023, 8, 22, 0, 0, 0, 0);
            // 
            // label19
            // 
            label19.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label19.AutoSize = true;
            label19.Font = new Font("Microsoft Sans Serif", 9.75F);
            label19.Location = new Point(16, 115);
            label19.Name = "label19";
            label19.Size = new Size(145, 16);
            label19.TabIndex = 152;
            label19.Text = "Data de Desembaraço";
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label1.AutoSize = true;
            label1.Font = new Font("Microsoft Sans Serif", 9.75F);
            label1.Location = new Point(34, 64);
            label1.Name = "label1";
            label1.Size = new Size(109, 16);
            label1.TabIndex = 153;
            label1.Text = "Data de Registro";
            // 
            // DTPdatadedesembaracodi
            // 
            DTPdatadedesembaracodi.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            DTPdatadedesembaracodi.Format = DateTimePickerFormat.Short;
            DTPdatadedesembaracodi.Location = new Point(21, 134);
            DTPdatadedesembaracodi.MinDate = new DateTime(2024, 10, 17, 0, 0, 0, 0);
            DTPdatadedesembaracodi.Name = "DTPdatadedesembaracodi";
            DTPdatadedesembaracodi.Size = new Size(135, 23);
            DTPdatadedesembaracodi.TabIndex = 150;
            DTPdatadedesembaracodi.Value = new DateTime(2024, 10, 17, 0, 0, 0, 0);
            // 
            // DTPdataderegistrodi
            // 
            DTPdataderegistrodi.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            DTPdataderegistrodi.Format = DateTimePickerFormat.Short;
            DTPdataderegistrodi.Location = new Point(21, 86);
            DTPdataderegistrodi.MinDate = new DateTime(2024, 10, 17, 0, 0, 0, 0);
            DTPdataderegistrodi.Name = "DTPdataderegistrodi";
            DTPdataderegistrodi.Size = new Size(135, 23);
            DTPdataderegistrodi.TabIndex = 149;
            DTPdataderegistrodi.Value = new DateTime(2024, 10, 17, 0, 0, 0, 0);
            // 
            // LBLinspecao
            // 
            LBLinspecao.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            LBLinspecao.AutoSize = true;
            LBLinspecao.Font = new Font("Microsoft Sans Serif", 9.75F);
            LBLinspecao.Location = new Point(57, 161);
            LBLinspecao.Name = "LBLinspecao";
            LBLinspecao.Size = new Size(63, 16);
            LBLinspecao.TabIndex = 302;
            LBLinspecao.Text = "Inspeção";
            // 
            // TXTdi
            // 
            TXTdi.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            TXTdi.Cursor = Cursors.IBeam;
            TXTdi.DataBindings.Add(new Binding("Text", BsModificaProcesso, "DI", true));
            TXTdi.Location = new Point(93, 38);
            TXTdi.Name = "TXTdi";
            TXTdi.Size = new Size(135, 23);
            TXTdi.TabIndex = 148;
            // 
            // BsModificaProcesso
            // 
            BsModificaProcesso.DataSource = typeof(CLUSA.Processo);
            // 
            // label21
            // 
            label21.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label21.AutoSize = true;
            label21.Font = new Font("Microsoft Sans Serif", 9.75F);
            label21.Location = new Point(150, 19);
            label21.Name = "label21";
            label21.Size = new Size(21, 16);
            label21.TabIndex = 151;
            label21.Text = "N°";
            // 
            // label23
            // 
            label23.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label23.AutoSize = true;
            label23.Font = new Font("Microsoft Sans Serif", 9.75F);
            label23.Location = new Point(182, 66);
            label23.Name = "label23";
            label23.Size = new Size(102, 16);
            label23.TabIndex = 147;
            label23.Text = "Parametrização";
            // 
            // DTPdatadeinspecao
            // 
            DTPdatadeinspecao.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            DTPdatadeinspecao.Format = DateTimePickerFormat.Short;
            DTPdatadeinspecao.Location = new Point(21, 180);
            DTPdatadeinspecao.MinDate = new DateTime(2023, 8, 22, 0, 0, 0, 0);
            DTPdatadeinspecao.Name = "DTPdatadeinspecao";
            DTPdatadeinspecao.Size = new Size(135, 23);
            DTPdatadeinspecao.TabIndex = 23;
            DTPdatadeinspecao.Value = new DateTime(2023, 8, 22, 0, 0, 0, 0);
            // 
            // label6
            // 
            label6.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label6.AutoSize = true;
            label6.Font = new Font("Microsoft Sans Serif", 9.75F);
            label6.Location = new Point(161, 115);
            label6.Name = "label6";
            label6.Size = new Size(144, 16);
            label6.TabIndex = 144;
            label6.Text = "Data de Carregamento";
            // 
            // CBparametrizacaodi
            // 
            CBparametrizacaodi.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            CBparametrizacaodi.AutoCompleteMode = AutoCompleteMode.Suggest;
            CBparametrizacaodi.DataBindings.Add(new Binding("Text", BsModificaProcesso, "ParametrizacaoDI", true));
            CBparametrizacaodi.FormattingEnabled = true;
            CBparametrizacaodi.Items.AddRange(new object[] { "Verde", "Amarelo", "Vermelho" });
            CBparametrizacaodi.Location = new Point(166, 85);
            CBparametrizacaodi.Name = "CBparametrizacaodi";
            CBparametrizacaodi.Size = new Size(135, 23);
            CBparametrizacaodi.TabIndex = 21;
            // 
            // DTPdatadecarregamentodi
            // 
            DTPdatadecarregamentodi.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            DTPdatadecarregamentodi.Format = DateTimePickerFormat.Short;
            DTPdatadecarregamentodi.Location = new Point(166, 134);
            DTPdatadecarregamentodi.MinDate = new DateTime(2024, 10, 17, 0, 0, 0, 0);
            DTPdatadecarregamentodi.Name = "DTPdatadecarregamentodi";
            DTPdatadecarregamentodi.Size = new Size(135, 23);
            DTPdatadecarregamentodi.TabIndex = 20;
            DTPdatadecarregamentodi.Value = new DateTime(2024, 10, 17, 0, 0, 0, 0);
            // 
            // CBamostra
            // 
            CBamostra.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            CBamostra.AutoSize = true;
            CBamostra.DataBindings.Add(new Binding("Checked", BsModificaProcesso, "Amostra", true));
            CBamostra.Font = new Font("Microsoft Sans Serif", 9.75F);
            CBamostra.Location = new Point(911, 44);
            CBamostra.Name = "CBamostra";
            CBamostra.Size = new Size(76, 20);
            CBamostra.TabIndex = 133;
            CBamostra.Text = "Amostra";
            CBamostra.UseVisualStyleBackColor = true;
            // 
            // CBdesovado
            // 
            CBdesovado.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            CBdesovado.AutoSize = true;
            CBdesovado.DataBindings.Add(new Binding("Checked", BsModificaProcesso, "Desovado", true));
            CBdesovado.Font = new Font("Microsoft Sans Serif", 9.75F);
            CBdesovado.Location = new Point(993, 44);
            CBdesovado.Name = "CBdesovado";
            CBdesovado.Size = new Size(90, 20);
            CBdesovado.TabIndex = 100;
            CBdesovado.Text = "Desovado";
            CBdesovado.UseVisualStyleBackColor = true;
            // 
            // btnAdiciona
            // 
            btnAdiciona.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            btnAdiciona.Cursor = Cursors.Hand;
            btnAdiciona.Location = new Point(10, 134);
            btnAdiciona.Name = "btnAdiciona";
            btnAdiciona.Size = new Size(200, 29);
            btnAdiciona.TabIndex = 28;
            btnAdiciona.Text = "Salvar";
            btnAdiciona.UseVisualStyleBackColor = true;
            btnAdiciona.Click += btnAdiciona_Click;
            // 
            // NUMfreetime
            // 
            NUMfreetime.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            NUMfreetime.DataBindings.Add(new Binding("Value", BsModificaProcesso, "FreeTime", true));
            NUMfreetime.Location = new Point(986, 17);
            NUMfreetime.Name = "NUMfreetime";
            NUMfreetime.Size = new Size(97, 23);
            NUMfreetime.TabIndex = 800;
            // 
            // label17
            // 
            label17.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label17.AutoSize = true;
            label17.Font = new Font("Microsoft Sans Serif", 9.75F);
            label17.Location = new Point(15, 62);
            label17.Name = "label17";
            label17.Size = new Size(72, 16);
            label17.TabIndex = 287;
            label17.Text = "Importador";
            // 
            // TXTimportador
            // 
            TXTimportador.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            TXTimportador.Cursor = Cursors.IBeam;
            TXTimportador.DataBindings.Add(new Binding("Text", BsModificaProcesso, "Importador", true));
            TXTimportador.Location = new Point(104, 60);
            TXTimportador.Name = "TXTimportador";
            TXTimportador.Size = new Size(159, 23);
            TXTimportador.TabIndex = 4;
            // 
            // label15
            // 
            label15.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label15.AutoSize = true;
            label15.Font = new Font("Microsoft Sans Serif", 9.75F);
            label15.Location = new Point(911, 19);
            label15.Name = "label15";
            label15.Size = new Size(69, 16);
            label15.TabIndex = 283;
            label15.Text = "Free Time";
            // 
            // label12
            // 
            label12.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label12.AutoSize = true;
            label12.Font = new Font("Microsoft Sans Serif", 9.75F);
            label12.Location = new Point(282, 91);
            label12.Name = "label12";
            label12.Size = new Size(59, 16);
            label12.TabIndex = 282;
            label12.Text = "Armador";
            // 
            // label11
            // 
            label11.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label11.AutoSize = true;
            label11.Font = new Font("Microsoft Sans Serif", 9.75F);
            label11.Location = new Point(197, 33);
            label11.Name = "label11";
            label11.Size = new Size(43, 16);
            label11.TabIndex = 281;
            label11.Text = "S. Ref";
            // 
            // label9
            // 
            label9.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label9.AutoSize = true;
            label9.Font = new Font("Microsoft Sans Serif", 9.75F);
            label9.Location = new Point(15, 33);
            label9.Name = "label9";
            label9.Size = new Size(59, 16);
            label9.TabIndex = 280;
            label9.Text = "Ref. Usa";
            // 
            // TXTsr
            // 
            TXTsr.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            TXTsr.Cursor = Cursors.IBeam;
            TXTsr.DataBindings.Add(new Binding("Text", BsModificaProcesso, "SR", true));
            TXTsr.Location = new Point(250, 31);
            TXTsr.Name = "TXTsr";
            TXTsr.Size = new Size(107, 23);
            TXTsr.TabIndex = 2;
            // 
            // label5
            // 
            label5.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label5.AutoSize = true;
            label5.Font = new Font("Microsoft Sans Serif", 9.75F);
            label5.Location = new Point(550, 62);
            label5.Name = "label5";
            label5.Size = new Size(93, 16);
            label5.TabIndex = 277;
            label5.Text = "Conhecimento";
            // 
            // txtConhecimento
            // 
            txtConhecimento.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtConhecimento.Cursor = Cursors.IBeam;
            txtConhecimento.DataBindings.Add(new Binding("Text", BsModificaProcesso, "Conhecimento", true));
            txtConhecimento.Location = new Point(667, 60);
            txtConhecimento.Name = "txtConhecimento";
            txtConhecimento.Size = new Size(227, 23);
            txtConhecimento.TabIndex = 6;
            // 
            // txtArmador
            // 
            txtArmador.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtArmador.Cursor = Cursors.IBeam;
            txtArmador.DataBindings.Add(new Binding("Text", BsModificaProcesso, "Armador", true));
            txtArmador.Location = new Point(360, 89);
            txtArmador.Name = "txtArmador";
            txtArmador.Size = new Size(173, 23);
            txtArmador.TabIndex = 11;
            // 
            // label7
            // 
            label7.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label7.AutoSize = true;
            label7.Font = new Font("Microsoft Sans Serif", 9.75F);
            label7.Location = new Point(15, 120);
            label7.Name = "label7";
            label7.Size = new Size(54, 16);
            label7.TabIndex = 274;
            label7.Text = "Produto";
            // 
            // TXTProduto
            // 
            TXTProduto.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            TXTProduto.Cursor = Cursors.IBeam;
            TXTProduto.DataBindings.Add(new Binding("Text", BsModificaProcesso, "Produto", true));
            TXTProduto.Location = new Point(87, 118);
            TXTProduto.Name = "TXTProduto";
            TXTProduto.Size = new Size(243, 23);
            TXTProduto.TabIndex = 12;
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label2.AutoSize = true;
            label2.Font = new Font("Microsoft Sans Serif", 9.75F);
            label2.Location = new Point(638, 120);
            label2.Name = "label2";
            label2.Size = new Size(32, 16);
            label2.TabIndex = 270;
            label2.Text = "FLO";
            // 
            // TXTflo
            // 
            TXTflo.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            TXTflo.Cursor = Cursors.IBeam;
            TXTflo.DataBindings.Add(new Binding("Text", BsModificaProcesso, "FLO", true));
            TXTflo.Location = new Point(688, 118);
            TXTflo.Name = "TXTflo";
            TXTflo.Size = new Size(206, 23);
            TXTflo.TabIndex = 9;
            // 
            // Exportador
            // 
            Exportador.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            Exportador.AutoSize = true;
            Exportador.Font = new Font("Microsoft Sans Serif", 9.75F);
            Exportador.Location = new Point(15, 91);
            Exportador.Name = "Exportador";
            Exportador.Size = new Size(73, 16);
            Exportador.TabIndex = 268;
            Exportador.Text = "Exportador";
            // 
            // TXTexportador
            // 
            TXTexportador.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            TXTexportador.Cursor = Cursors.IBeam;
            TXTexportador.DataBindings.Add(new Binding("Text", BsModificaProcesso, "Exportador", true));
            TXTexportador.Location = new Point(104, 89);
            TXTexportador.Name = "TXTexportador";
            TXTexportador.Size = new Size(159, 23);
            TXTexportador.TabIndex = 5;
            // 
            // TXTstatusdoprocesso
            // 
            TXTstatusdoprocesso.DataBindings.Add(new Binding("Text", BsModificaProcesso, "HistoricoDoProcesso", true));
            TXTstatusdoprocesso.Dock = DockStyle.Fill;
            TXTstatusdoprocesso.Location = new Point(3, 21);
            TXTstatusdoprocesso.Multiline = true;
            TXTstatusdoprocesso.Name = "TXTstatusdoprocesso";
            TXTstatusdoprocesso.Size = new Size(331, 426);
            TXTstatusdoprocesso.TabIndex = 25;
            // 
            // TXTpendencia
            // 
            TXTpendencia.DataBindings.Add(new Binding("Text", BsModificaProcesso, "Pendencia", true));
            TXTpendencia.Dock = DockStyle.Fill;
            TXTpendencia.Location = new Point(3, 21);
            TXTpendencia.Multiline = true;
            TXTpendencia.Name = "TXTpendencia";
            TXTpendencia.Size = new Size(519, 147);
            TXTpendencia.TabIndex = 26;
            // 
            // label13
            // 
            label13.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label13.AutoSize = true;
            label13.Font = new Font("Microsoft Sans Serif", 9.75F);
            label13.Location = new Point(282, 62);
            label13.Name = "label13";
            label13.Size = new Size(52, 16);
            label13.TabIndex = 306;
            label13.Text = "Veículo";
            // 
            // txtVeiculo
            // 
            txtVeiculo.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtVeiculo.Cursor = Cursors.IBeam;
            txtVeiculo.DataBindings.Add(new Binding("Text", BsModificaProcesso, "Veiculo", true));
            txtVeiculo.Location = new Point(348, 60);
            txtVeiculo.Name = "txtVeiculo";
            txtVeiculo.Size = new Size(185, 23);
            txtVeiculo.TabIndex = 8;
            // 
            // checkedListBox1
            // 
            checkedListBox1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            checkedListBox1.BorderStyle = BorderStyle.FixedSingle;
            checkedListBox1.FormattingEnabled = true;
            checkedListBox1.Items.AddRange(new object[] { "BL", "Fatura", "Packing List", "CO", "Fito", "CSI", "CA", "CF" });
            checkedListBox1.Location = new Point(46, 37);
            checkedListBox1.Name = "checkedListBox1";
            checkedListBox1.Size = new Size(108, 92);
            checkedListBox1.TabIndex = 307;
            // 
            // label16
            // 
            label16.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label16.AutoSize = true;
            label16.Font = new Font("Microsoft Sans Serif", 9.75F);
            label16.Location = new Point(46, 14);
            label16.Name = "label16";
            label16.Size = new Size(109, 16);
            label16.TabIndex = 308;
            label16.Text = "Docs Recebidos";
            // 
            // checkedListBox2
            // 
            checkedListBox2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            checkedListBox2.BorderStyle = BorderStyle.FixedSingle;
            checkedListBox2.FormattingEnabled = true;
            checkedListBox2.Items.AddRange(new object[] { "DHL", "UPS", "Correio", "Fedex", "Daytona" });
            checkedListBox2.Location = new Point(181, 37);
            checkedListBox2.Name = "checkedListBox2";
            checkedListBox2.Size = new Size(79, 92);
            checkedListBox2.TabIndex = 309;
            checkedListBox2.ItemCheck += checkedListBox2_ItemCheck;
            // 
            // label24
            // 
            label24.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label24.AutoSize = true;
            label24.Font = new Font("Microsoft Sans Serif", 9.75F);
            label24.Location = new Point(182, 14);
            label24.Name = "label24";
            label24.Size = new Size(77, 16);
            label24.TabIndex = 310;
            label24.Text = "Forma Rec.";
            // 
            // label25
            // 
            label25.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label25.AutoSize = true;
            label25.Font = new Font("Microsoft Sans Serif", 9.75F);
            label25.Location = new Point(168, 139);
            label25.Name = "label25";
            label25.Size = new Size(123, 16);
            label25.TabIndex = 312;
            label25.Text = "Data Rec. Originais";
            // 
            // DTPDataRecOriginais
            // 
            DTPDataRecOriginais.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            DTPDataRecOriginais.Format = DateTimePickerFormat.Short;
            DTPDataRecOriginais.Location = new Point(162, 161);
            DTPDataRecOriginais.MinDate = new DateTime(2023, 8, 22, 0, 0, 0, 0);
            DTPDataRecOriginais.Name = "DTPDataRecOriginais";
            DTPDataRecOriginais.Size = new Size(135, 23);
            DTPDataRecOriginais.TabIndex = 311;
            DTPDataRecOriginais.Value = new DateTime(2023, 8, 22, 0, 0, 0, 0);
            // 
            // TXTnr
            // 
            TXTnr.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            TXTnr.DataBindings.Add(new Binding("Text", BsModificaProcesso, "Ref_USA", true));
            TXTnr.Location = new Point(87, 31);
            TXTnr.Mask = "0000/0000";
            TXTnr.Name = "TXTnr";
            TXTnr.Size = new Size(95, 23);
            TXTnr.TabIndex = 1;
            // 
            // txtTerminal
            // 
            txtTerminal.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtTerminal.Cursor = Cursors.IBeam;
            txtTerminal.DataBindings.Add(new Binding("Text", BsModificaProcesso, "Terminal", true));
            txtTerminal.Location = new Point(420, 118);
            txtTerminal.Name = "txtTerminal";
            txtTerminal.Size = new Size(203, 23);
            txtTerminal.TabIndex = 3;
            // 
            // label4
            // 
            label4.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label4.AutoSize = true;
            label4.Font = new Font("Microsoft Sans Serif", 9.75F);
            label4.Location = new Point(348, 120);
            label4.Name = "label4";
            label4.Size = new Size(60, 16);
            label4.TabIndex = 393;
            label4.Text = "Terminal";
            // 
            // label20
            // 
            label20.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label20.AutoSize = true;
            label20.Font = new Font("Microsoft Sans Serif", 9.75F);
            label20.Location = new Point(165, 18);
            label20.Name = "label20";
            label20.Size = new Size(130, 16);
            label20.TabIndex = 397;
            label20.Text = "Vencimento LI/LPCO";
            // 
            // label22
            // 
            label22.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label22.AutoSize = true;
            label22.Font = new Font("Microsoft Sans Serif", 9.75F);
            label22.Location = new Point(14, 18);
            label22.Name = "label22";
            label22.Size = new Size(143, 16);
            label22.TabIndex = 396;
            label22.Text = "Vencimento Free Time";
            // 
            // dtpVencimentoLI_LPCO
            // 
            dtpVencimentoLI_LPCO.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dtpVencimentoLI_LPCO.Enabled = false;
            dtpVencimentoLI_LPCO.Format = DateTimePickerFormat.Short;
            dtpVencimentoLI_LPCO.Location = new Point(163, 40);
            dtpVencimentoLI_LPCO.MinDate = new DateTime(2023, 8, 22, 0, 0, 0, 0);
            dtpVencimentoLI_LPCO.Name = "dtpVencimentoLI_LPCO";
            dtpVencimentoLI_LPCO.Size = new Size(135, 23);
            dtpVencimentoLI_LPCO.TabIndex = 394;
            dtpVencimentoLI_LPCO.Value = new DateTime(2023, 8, 22, 0, 0, 0, 0);
            // 
            // dtpVencimentoFreeTime
            // 
            dtpVencimentoFreeTime.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dtpVencimentoFreeTime.Enabled = false;
            dtpVencimentoFreeTime.Format = DateTimePickerFormat.Short;
            dtpVencimentoFreeTime.Location = new Point(18, 40);
            dtpVencimentoFreeTime.MinDate = new DateTime(2023, 8, 22, 0, 0, 0, 0);
            dtpVencimentoFreeTime.Name = "dtpVencimentoFreeTime";
            dtpVencimentoFreeTime.Size = new Size(135, 23);
            dtpVencimentoFreeTime.TabIndex = 395;
            dtpVencimentoFreeTime.Value = new DateTime(2023, 8, 22, 0, 0, 0, 0);
            // 
            // label26
            // 
            label26.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label26.AutoSize = true;
            label26.Font = new Font("Microsoft Sans Serif", 9.75F);
            label26.Location = new Point(102, 66);
            label26.Name = "label26";
            label26.Size = new Size(109, 16);
            label26.TabIndex = 401;
            label26.Text = "Vencimento FMA";
            // 
            // dtpVencimentoFMA
            // 
            dtpVencimentoFMA.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dtpVencimentoFMA.Enabled = false;
            dtpVencimentoFMA.Format = DateTimePickerFormat.Short;
            dtpVencimentoFMA.Location = new Point(89, 88);
            dtpVencimentoFMA.MinDate = new DateTime(2023, 8, 22, 0, 0, 0, 0);
            dtpVencimentoFMA.Name = "dtpVencimentoFMA";
            dtpVencimentoFMA.Size = new Size(135, 23);
            dtpVencimentoFMA.TabIndex = 398;
            dtpVencimentoFMA.Value = new DateTime(2023, 8, 22, 0, 0, 0, 0);
            // 
            // groupBox3
            // 
            groupBox3.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            groupBox3.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            groupBox3.Controls.Add(label22);
            groupBox3.Controls.Add(label26);
            groupBox3.Controls.Add(dtpVencimentoFreeTime);
            groupBox3.Controls.Add(dtpVencimentoFMA);
            groupBox3.Controls.Add(dtpVencimentoLI_LPCO);
            groupBox3.Controls.Add(label20);
            groupBox3.Location = new Point(1113, 508);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(318, 124);
            groupBox3.TabIndex = 402;
            groupBox3.TabStop = false;
            groupBox3.Text = "Vencimentos";
            // 
            // label27
            // 
            label27.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label27.AutoSize = true;
            label27.Font = new Font("Microsoft Sans Serif", 9.75F);
            label27.Location = new Point(61, 139);
            label27.Name = "label27";
            label27.Size = new Size(45, 16);
            label27.TabIndex = 306;
            label27.Text = "Marca";
            // 
            // cbMarca
            // 
            cbMarca.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            cbMarca.AutoCompleteMode = AutoCompleteMode.Suggest;
            cbMarca.FormattingEnabled = true;
            cbMarca.Items.AddRange(new object[] { "20 DRY", "40 DRY", "20 RF", "40 RF", "20 HC", "40 HC", "20 ST", "40 ST", "Sacos", "Caixas", "Pallets" });
            cbMarca.Location = new Point(61, 161);
            cbMarca.Name = "cbMarca";
            cbMarca.Size = new Size(89, 23);
            cbMarca.TabIndex = 305;
            // 
            // numMarca
            // 
            numMarca.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            numMarca.Location = new Point(15, 161);
            numMarca.Name = "numMarca";
            numMarca.Size = new Size(49, 23);
            numMarca.TabIndex = 403;
            // 
            // btnCapa
            // 
            btnCapa.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            btnCapa.Cursor = Cursors.Hand;
            btnCapa.Location = new Point(10, 65);
            btnCapa.Name = "btnCapa";
            btnCapa.Size = new Size(200, 29);
            btnCapa.TabIndex = 404;
            btnCapa.Text = "Capa";
            btnCapa.UseVisualStyleBackColor = true;
            btnCapa.Click += btnCapa_Click;
            // 
            // btnRelatorio
            // 
            btnRelatorio.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            btnRelatorio.Cursor = Cursors.Hand;
            btnRelatorio.Location = new Point(10, 99);
            btnRelatorio.Name = "btnRelatorio";
            btnRelatorio.Size = new Size(200, 29);
            btnRelatorio.TabIndex = 405;
            btnRelatorio.Text = "Relatório";
            btnRelatorio.UseVisualStyleBackColor = true;
            btnRelatorio.Click += btnRelatorio_Click;
            // 
            // TCLi
            // 
            TCLi.Dock = DockStyle.Fill;
            TCLi.Location = new Point(3, 19);
            TCLi.Name = "TCLi";
            TCLi.SelectedIndex = 0;
            TCLi.Size = new Size(744, 261);
            TCLi.SizeMode = TabSizeMode.Fixed;
            TCLi.TabIndex = 801;
            // 
            // label32
            // 
            label32.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label32.AutoSize = true;
            label32.Font = new Font("Microsoft Sans Serif", 9.75F);
            label32.Location = new Point(89, 17);
            label32.Name = "label32";
            label32.Size = new Size(44, 16);
            label32.TabIndex = 802;
            label32.Text = "Status";
            // 
            // CbStatus
            // 
            CbStatus.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            CbStatus.AutoCompleteMode = AutoCompleteMode.Suggest;
            CbStatus.DataBindings.Add(new Binding("Text", BsModificaProcesso, "Status", true));
            CbStatus.FormattingEnabled = true;
            CbStatus.Items.AddRange(new object[] { "", "Aguardando embarque", "Aguardando atracação", "Aguardando presença de carga", "Aguardando SIGVIG", "Aguardando documentos finais para LPCO", "Aguardando parametrização LI/LPCO", "Aguardando inspeção/coleta LI/LPCO", "Aguardando deferimento LI/LPCO", "Aguardando registro DI/DUIMP", "Aguardando parametrização DI/DUIMP", "Aguardando desembaraço", "Aguardando inspeção DI/DUIMP", "Aguardando desbloqueio Siscomex Carga", "Aguardando minuta devolução container vazio", "Aguardando resultado laboratório ", "Finalizado" });
            CbStatus.Location = new Point(10, 36);
            CbStatus.Name = "CbStatus";
            CbStatus.Size = new Size(200, 23);
            CbStatus.TabIndex = 803;
            // 
            // LBLdatadeatracacao
            // 
            LBLdatadeatracacao.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            LBLdatadeatracacao.AutoSize = true;
            LBLdatadeatracacao.Font = new Font("Microsoft Sans Serif", 9.75F);
            LBLdatadeatracacao.Location = new Point(169, 194);
            LBLdatadeatracacao.Name = "LBLdatadeatracacao";
            LBLdatadeatracacao.Size = new Size(114, 16);
            LBLdatadeatracacao.TabIndex = 807;
            LBLdatadeatracacao.Text = "Data de Chegada";
            // 
            // LBLdatadeembarque
            // 
            LBLdatadeembarque.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            LBLdatadeembarque.AutoSize = true;
            LBLdatadeembarque.Font = new Font("Microsoft Sans Serif", 9.75F);
            LBLdatadeembarque.Location = new Point(22, 194);
            LBLdatadeembarque.Name = "LBLdatadeembarque";
            LBLdatadeembarque.Size = new Size(121, 16);
            LBLdatadeembarque.TabIndex = 806;
            LBLdatadeembarque.Text = "Data de Embarque";
            // 
            // DTPdatadeatracacao
            // 
            DTPdatadeatracacao.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            DTPdatadeatracacao.Format = DateTimePickerFormat.Short;
            DTPdatadeatracacao.Location = new Point(159, 216);
            DTPdatadeatracacao.MinDate = new DateTime(2023, 8, 22, 0, 0, 0, 0);
            DTPdatadeatracacao.Name = "DTPdatadeatracacao";
            DTPdatadeatracacao.Size = new Size(135, 23);
            DTPdatadeatracacao.TabIndex = 804;
            DTPdatadeatracacao.Value = new DateTime(2023, 8, 22, 0, 0, 0, 0);
            // 
            // DTPdatadeembarque
            // 
            DTPdatadeembarque.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            DTPdatadeembarque.Format = DateTimePickerFormat.Short;
            DTPdatadeembarque.Location = new Point(15, 216);
            DTPdatadeembarque.MinDate = new DateTime(2023, 8, 22, 0, 0, 0, 0);
            DTPdatadeembarque.Name = "DTPdatadeembarque";
            DTPdatadeembarque.Size = new Size(135, 23);
            DTPdatadeembarque.TabIndex = 805;
            DTPdatadeembarque.Value = new DateTime(2023, 8, 22, 0, 0, 0, 0);
            // 
            // groupBox1
            // 
            groupBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            groupBox1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            groupBox1.Controls.Add(TxtContainer);
            groupBox1.Controls.Add(label8);
            groupBox1.Controls.Add(txtOrigem);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(TXTportodedestino);
            groupBox1.Controls.Add(label14);
            groupBox1.Controls.Add(groupBox4);
            groupBox1.Controls.Add(CbResultadoLaboratorial);
            groupBox1.Controls.Add(CbPresencaCarga);
            groupBox1.Controls.Add(label9);
            groupBox1.Controls.Add(TXTexportador);
            groupBox1.Controls.Add(Exportador);
            groupBox1.Controls.Add(TXTflo);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(TXTProduto);
            groupBox1.Controls.Add(label7);
            groupBox1.Controls.Add(txtArmador);
            groupBox1.Controls.Add(txtConhecimento);
            groupBox1.Controls.Add(label5);
            groupBox1.Controls.Add(TXTsr);
            groupBox1.Controls.Add(label4);
            groupBox1.Controls.Add(label11);
            groupBox1.Controls.Add(txtTerminal);
            groupBox1.Controls.Add(label12);
            groupBox1.Controls.Add(TXTnr);
            groupBox1.Controls.Add(TXTimportador);
            groupBox1.Controls.Add(label17);
            groupBox1.Controls.Add(txtVeiculo);
            groupBox1.Controls.Add(NUMfreetime);
            groupBox1.Controls.Add(CBdesovado);
            groupBox1.Controls.Add(CBamostra);
            groupBox1.Controls.Add(label13);
            groupBox1.Controls.Add(label15);
            groupBox1.Location = new Point(14, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(1105, 164);
            groupBox1.TabIndex = 808;
            groupBox1.TabStop = false;
            groupBox1.Text = "Informações Gerais";
            // 
            // TxtContainer
            // 
            TxtContainer.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            TxtContainer.Cursor = Cursors.IBeam;
            TxtContainer.DataBindings.Add(new Binding("Text", BsModificaProcesso, "Container", true));
            TxtContainer.Location = new Point(638, 89);
            TxtContainer.Name = "TxtContainer";
            TxtContainer.Size = new Size(256, 23);
            TxtContainer.TabIndex = 808;
            // 
            // label8
            // 
            label8.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label8.AutoSize = true;
            label8.Font = new Font("Microsoft Sans Serif", 9.75F);
            label8.Location = new Point(550, 91);
            label8.Name = "label8";
            label8.Size = new Size(64, 16);
            label8.TabIndex = 809;
            label8.Text = "Container";
            // 
            // txtOrigem
            // 
            txtOrigem.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtOrigem.Cursor = Cursors.IBeam;
            txtOrigem.DataBindings.Add(new Binding("Text", BsModificaProcesso, "Origem", true));
            txtOrigem.Location = new Point(707, 32);
            txtOrigem.Name = "txtOrigem";
            txtOrigem.Size = new Size(187, 23);
            txtOrigem.TabIndex = 805;
            // 
            // label3
            // 
            label3.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label3.AutoSize = true;
            label3.Font = new Font("Microsoft Sans Serif", 9.75F);
            label3.Location = new Point(638, 33);
            label3.Name = "label3";
            label3.Size = new Size(51, 16);
            label3.TabIndex = 806;
            label3.Text = "Origem";
            // 
            // TXTportodedestino
            // 
            TXTportodedestino.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            TXTportodedestino.Cursor = Cursors.IBeam;
            TXTportodedestino.DataBindings.Add(new Binding("Text", BsModificaProcesso, "PortoDestino", true));
            TXTportodedestino.Location = new Point(510, 31);
            TXTportodedestino.Name = "TXTportodedestino";
            TXTportodedestino.Size = new Size(113, 23);
            TXTportodedestino.TabIndex = 804;
            // 
            // label14
            // 
            label14.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label14.AutoSize = true;
            label14.Font = new Font("Microsoft Sans Serif", 9.75F);
            label14.Location = new Point(379, 33);
            label14.Name = "label14";
            label14.Size = new Size(107, 16);
            label14.TabIndex = 807;
            label14.Text = "Porto de Destino";
            // 
            // groupBox4
            // 
            groupBox4.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBox4.Controls.Add(CbLiberado);
            groupBox4.Controls.Add(CbSelecionado);
            groupBox4.Location = new Point(910, 116);
            groupBox4.Name = "groupBox4";
            groupBox4.Size = new Size(173, 42);
            groupBox4.TabIndex = 803;
            groupBox4.TabStop = false;
            groupBox4.Text = "SIGVIG";
            // 
            // CbLiberado
            // 
            CbLiberado.Anchor = AnchorStyles.None;
            CbLiberado.AutoSize = true;
            CbLiberado.DataBindings.Add(new Binding("Checked", BsModificaProcesso, "SIGVIGLiberado", true));
            CbLiberado.Font = new Font("Microsoft Sans Serif", 8F);
            CbLiberado.Location = new Point(97, 19);
            CbLiberado.Name = "CbLiberado";
            CbLiberado.Size = new Size(67, 17);
            CbLiberado.TabIndex = 804;
            CbLiberado.Text = "Liberado";
            CbLiberado.UseVisualStyleBackColor = true;
            // 
            // CbSelecionado
            // 
            CbSelecionado.Anchor = AnchorStyles.None;
            CbSelecionado.AutoSize = true;
            CbSelecionado.DataBindings.Add(new Binding("Checked", BsModificaProcesso, "SIGVIGSelecionado", true));
            CbSelecionado.Font = new Font("Microsoft Sans Serif", 8F);
            CbSelecionado.Location = new Point(11, 19);
            CbSelecionado.Name = "CbSelecionado";
            CbSelecionado.Size = new Size(85, 17);
            CbSelecionado.TabIndex = 805;
            CbSelecionado.Text = "Selecionado";
            CbSelecionado.UseVisualStyleBackColor = true;
            // 
            // CbResultadoLaboratorial
            // 
            CbResultadoLaboratorial.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            CbResultadoLaboratorial.AutoSize = true;
            CbResultadoLaboratorial.DataBindings.Add(new Binding("Checked", BsModificaProcesso, "ResultadoLab", true));
            CbResultadoLaboratorial.Font = new Font("Microsoft Sans Serif", 9.75F);
            CbResultadoLaboratorial.Location = new Point(911, 96);
            CbResultadoLaboratorial.Name = "CbResultadoLaboratorial";
            CbResultadoLaboratorial.Size = new Size(163, 20);
            CbResultadoLaboratorial.TabIndex = 801;
            CbResultadoLaboratorial.Text = "Resultado Laboratorial";
            CbResultadoLaboratorial.UseVisualStyleBackColor = true;
            // 
            // CbPresencaCarga
            // 
            CbPresencaCarga.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            CbPresencaCarga.AutoSize = true;
            CbPresencaCarga.DataBindings.Add(new Binding("Checked", BsModificaProcesso, "PresencaDeCarga", true));
            CbPresencaCarga.Font = new Font("Microsoft Sans Serif", 9.75F);
            CbPresencaCarga.Location = new Point(911, 70);
            CbPresencaCarga.Name = "CbPresencaCarga";
            CbPresencaCarga.Size = new Size(143, 20);
            CbPresencaCarga.TabIndex = 802;
            CbPresencaCarga.Text = "Presença de Carga";
            CbPresencaCarga.UseVisualStyleBackColor = true;
            // 
            // groupBox6
            // 
            groupBox6.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            groupBox6.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            groupBox6.Controls.Add(TXTstatusdoprocesso);
            groupBox6.Font = new Font("Segoe UI", 10F);
            groupBox6.Location = new Point(14, 182);
            groupBox6.Name = "groupBox6";
            groupBox6.Size = new Size(337, 450);
            groupBox6.TabIndex = 808;
            groupBox6.TabStop = false;
            groupBox6.Text = "Histórico do Processo";
            // 
            // groupBox7
            // 
            groupBox7.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBox7.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            groupBox7.Controls.Add(TXTpendencia);
            groupBox7.Font = new Font("Segoe UI", 10F);
            groupBox7.Location = new Point(357, 461);
            groupBox7.Name = "groupBox7";
            groupBox7.Size = new Size(525, 171);
            groupBox7.TabIndex = 809;
            groupBox7.TabStop = false;
            groupBox7.Text = "Pendência";
            // 
            // BtnLI
            // 
            BtnLI.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            BtnLI.Location = new Point(474, 0);
            BtnLI.Name = "BtnLI";
            BtnLI.Size = new Size(129, 23);
            BtnLI.TabIndex = 818;
            BtnLI.Text = "Novo LI";
            BtnLI.UseVisualStyleBackColor = true;
            BtnLI.Click += BtnLI_Click;
            // 
            // BtnExcluirLI
            // 
            BtnExcluirLI.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            BtnExcluirLI.Location = new Point(609, 0);
            BtnExcluirLI.Name = "BtnExcluirLI";
            BtnExcluirLI.Size = new Size(129, 23);
            BtnExcluirLI.TabIndex = 819;
            BtnExcluirLI.Text = "Excluir LI";
            BtnExcluirLI.UseVisualStyleBackColor = true;
            BtnExcluirLI.Click += BtnExcluirLi_Click;
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.AutoSize = true;
            flowLayoutPanel1.Location = new Point(3, 635);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(1445, 641);
            flowLayoutPanel1.TabIndex = 820;
            // 
            // groupBox8
            // 
            groupBox8.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBox8.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            groupBox8.Controls.Add(label32);
            groupBox8.Controls.Add(btnAdiciona);
            groupBox8.Controls.Add(btnRelatorio);
            groupBox8.Controls.Add(btnCapa);
            groupBox8.Controls.Add(CbStatus);
            groupBox8.Location = new Point(888, 461);
            groupBox8.Name = "groupBox8";
            groupBox8.Size = new Size(219, 171);
            groupBox8.TabIndex = 821;
            groupBox8.TabStop = false;
            // 
            // groupBox9
            // 
            groupBox9.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            groupBox9.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            groupBox9.Controls.Add(label16);
            groupBox9.Controls.Add(checkedListBox1);
            groupBox9.Controls.Add(checkedListBox2);
            groupBox9.Controls.Add(LBLdatadeatracacao);
            groupBox9.Controls.Add(label24);
            groupBox9.Controls.Add(LBLdatadeembarque);
            groupBox9.Controls.Add(DTPDataRecOriginais);
            groupBox9.Controls.Add(DTPdatadeatracacao);
            groupBox9.Controls.Add(label25);
            groupBox9.Controls.Add(DTPdatadeembarque);
            groupBox9.Controls.Add(label27);
            groupBox9.Controls.Add(cbMarca);
            groupBox9.Controls.Add(numMarca);
            groupBox9.Location = new Point(1125, 12);
            groupBox9.Name = "groupBox9";
            groupBox9.Size = new Size(306, 256);
            groupBox9.TabIndex = 822;
            groupBox9.TabStop = false;
            // 
            // groupBox10
            // 
            groupBox10.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBox10.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            groupBox10.Controls.Add(BtnExcluirLI);
            groupBox10.Controls.Add(BtnLI);
            groupBox10.Controls.Add(TCLi);
            groupBox10.Location = new Point(357, 182);
            groupBox10.Name = "groupBox10";
            groupBox10.Size = new Size(750, 283);
            groupBox10.TabIndex = 823;
            groupBox10.TabStop = false;
            // 
            // FrmModificaProcesso
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1443, 639);
            Controls.Add(groupBox7);
            Controls.Add(groupBox3);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Controls.Add(groupBox6);
            Controls.Add(flowLayoutPanel1);
            Controls.Add(groupBox8);
            Controls.Add(groupBox9);
            Controls.Add(groupBox10);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "FrmModificaProcesso";
            Text = "Processo";
            FormClosing += frmModificaProcesso_FormClosing;
            Load += FrmModificaProcesso_Load;
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)BsModificaProcesso).EndInit();
            ((System.ComponentModel.ISupportInitialize)NUMfreetime).EndInit();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numMarca).EndInit();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox4.ResumeLayout(false);
            groupBox4.PerformLayout();
            groupBox6.ResumeLayout(false);
            groupBox6.PerformLayout();
            groupBox7.ResumeLayout(false);
            groupBox7.PerformLayout();
            groupBox8.ResumeLayout(false);
            groupBox8.PerformLayout();
            groupBox9.ResumeLayout(false);
            groupBox9.PerformLayout();
            groupBox10.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private GroupBox groupBox2;
        private ComboBox CBparametrizacaodi;
        private DateTimePicker DTPdatadecarregamentodi;
        private CheckBox CBamostra;
        private CheckBox CBdesovado;
        private Button btnAdiciona;
        private Label label6;
        private Label label23;
        private NumericUpDown NUMfreetime;
        private Label label17;
        private TextBox TXTimportador;
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
        private DateTimePicker DTPdatadeinspecao;
        private TextBox TXTstatusdoprocesso;
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
        private GroupBox groupBox3;
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
        private Button btnCapa;
        private Button btnRelatorio;
        private TabControl TCLi;
        private Label label32;
        private ComboBox CbStatus;
        private Label LBLdatadeatracacao;
        private Label LBLdatadeembarque;
        private DateTimePicker DTPdatadeatracacao;
        private DateTimePicker DTPdatadeembarque;
        private GroupBox groupBox1;
        private CheckBox CbResultadoLaboratorial;
        private CheckBox CbPresencaCarga;
        private GroupBox groupBox4;
        private CheckBox CbLiberado;
        private CheckBox CbSelecionado;
        private TextBox txtOrigem;
        private Label label3;
        private TextBox TXTportodedestino;
        private Label label14;
        private GroupBox groupBox6;
        private GroupBox groupBox7;
        private TextBox TxtContainer;
        private Label label8;
        private BindingSource BsModificaProcesso;
        private Button BtnLI;
        private Button BtnExcluirLI;
        private FlowLayoutPanel flowLayoutPanel1;
        private GroupBox groupBox8;
        private GroupBox groupBox9;
        private GroupBox groupBox10;
    }
}
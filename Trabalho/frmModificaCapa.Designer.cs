namespace Trabalho
{
    partial class FrmModificaCapa
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        // Generated code
        #endregion

        private void InitializeComponent()
        {
            label1 = new Label();
            txtMaster = new TextBox();
            TxtIncoterm = new TextBox();
            label3 = new Label();
            txtContainer = new TextBox();
            label4 = new Label();
            txtMarinha = new TextBox();
            label5 = new Label();
            txtCE = new TextBox();
            label6 = new Label();
            txtDTA = new TextBox();
            label8 = new Label();
            DTPSigvig = new DateTimePicker();
            cbIncotern = new CheckBox();
            ItensAdicionais = new CheckedListBox();
            DTPAverbar = new DateTimePicker();
            label9 = new Label();
            label10 = new Label();
            DTPLiberarBL = new DateTimePicker();
            label11 = new Label();
            DTPIsencaoMarinha = new DateTimePicker();
            label12 = new Label();
            DTPSisCarga = new DateTimePicker();
            label15 = new Label();
            DTPEntTransporte = new DateTimePicker();
            label16 = new Label();
            DTPICMS = new DateTimePicker();
            label17 = new Label();
            txtTransporte = new TextBox();
            txtDOSSIE = new TextBox();
            label13 = new Label();
            label14 = new Label();
            DTPEntAlfandega = new DateTimePicker();
            Impostos = new CheckedListBox();
            label19 = new Label();
            DTPConferenciaFisica = new DateTimePicker();
            label18 = new Label();
            txtObservacao = new TextBox();
            btnExportar = new Button();
            btnSalvar = new Button();
            btnCancelar = new Button();
            txtPagoPor = new TextBox();
            label20 = new Label();
            groupBox1 = new GroupBox();
            cbArmazenagem = new CheckBox();
            cbArmFaturado = new CheckBox();
            cbNumerario = new CheckedListBox();
            label21 = new Label();
            label22 = new Label();
            groupBox2 = new GroupBox();
            CbLiberado = new CheckBox();
            CbSelecionado = new CheckBox();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(46, 15);
            label1.TabIndex = 0;
            label1.Text = "Master:";
            // 
            // txtMaster
            // 
            txtMaster.Location = new Point(64, 6);
            txtMaster.Name = "txtMaster";
            txtMaster.Size = new Size(410, 23);
            txtMaster.TabIndex = 1;
            // 
            // TxtIncoterm
            // 
            TxtIncoterm.Location = new Point(72, 64);
            TxtIncoterm.Name = "TxtIncoterm";
            TxtIncoterm.Size = new Size(221, 23);
            TxtIncoterm.TabIndex = 5;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 67);
            label3.Name = "label3";
            label3.Size = new Size(58, 15);
            label3.TabIndex = 4;
            label3.Text = "Incoterm:";
            // 
            // txtContainer
            // 
            txtContainer.Location = new Point(80, 35);
            txtContainer.Name = "txtContainer";
            txtContainer.Size = new Size(394, 23);
            txtContainer.TabIndex = 7;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(12, 38);
            label4.Name = "label4";
            label4.Size = new Size(62, 15);
            label4.TabIndex = 6;
            label4.Text = "Container:";
            // 
            // txtMarinha
            // 
            txtMarinha.Location = new Point(72, 122);
            txtMarinha.Name = "txtMarinha";
            txtMarinha.Size = new Size(165, 23);
            txtMarinha.TabIndex = 15;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(12, 125);
            label5.Name = "label5";
            label5.Size = new Size(54, 15);
            label5.TabIndex = 14;
            label5.Text = "Marinha:";
            // 
            // txtCE
            // 
            txtCE.Location = new Point(272, 122);
            txtCE.Name = "txtCE";
            txtCE.Size = new Size(202, 23);
            txtCE.TabIndex = 13;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(242, 125);
            label6.Name = "label6";
            label6.Size = new Size(24, 15);
            label6.TabIndex = 12;
            label6.Text = "CE:";
            // 
            // txtDTA
            // 
            txtDTA.Location = new Point(49, 93);
            txtDTA.Name = "txtDTA";
            txtDTA.Size = new Size(425, 23);
            txtDTA.TabIndex = 9;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(12, 96);
            label8.Name = "label8";
            label8.Size = new Size(31, 15);
            label8.TabIndex = 8;
            label8.Text = "DTA:";
            // 
            // DTPSigvig
            // 
            DTPSigvig.Format = DateTimePickerFormat.Short;
            DTPSigvig.Location = new Point(247, 8);
            DTPSigvig.Name = "DTPSigvig";
            DTPSigvig.Size = new Size(175, 23);
            DTPSigvig.TabIndex = 17;
            // 
            // cbIncotern
            // 
            cbIncotern.AutoSize = true;
            cbIncotern.Location = new Point(324, 66);
            cbIncotern.Name = "cbIncotern";
            cbIncotern.Size = new Size(150, 19);
            cbIncotern.TabIndex = 18;
            cbIncotern.Text = "Documentos de acordo";
            cbIncotern.UseVisualStyleBackColor = true;
            // 
            // ItensAdicionais
            // 
            ItensAdicionais.FormattingEnabled = true;
            ItensAdicionais.Items.AddRange(new object[] { "Tela do Canal", "Lançado", "Consulta SEFAZ", "DAT & LI Deferida", "DANFE" });
            ItensAdicionais.Location = new Point(13, 452);
            ItensAdicionais.MultiColumn = true;
            ItensAdicionais.Name = "ItensAdicionais";
            ItensAdicionais.Size = new Size(366, 76);
            ItensAdicionais.TabIndex = 19;
            // 
            // DTPAverbar
            // 
            DTPAverbar.Format = DateTimePickerFormat.Short;
            DTPAverbar.Location = new Point(69, 230);
            DTPAverbar.Name = "DTPAverbar";
            DTPAverbar.Size = new Size(151, 23);
            DTPAverbar.TabIndex = 20;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(12, 234);
            label9.Name = "label9";
            label9.Size = new Size(51, 15);
            label9.TabIndex = 21;
            label9.Text = "Averbar:";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(226, 234);
            label10.Name = "label10";
            label10.Size = new Size(67, 15);
            label10.TabIndex = 23;
            label10.Text = "Liberar B/L:";
            // 
            // DTPLiberarBL
            // 
            DTPLiberarBL.Format = DateTimePickerFormat.Short;
            DTPLiberarBL.Location = new Point(299, 230);
            DTPLiberarBL.Name = "DTPLiberarBL";
            DTPLiberarBL.Size = new Size(175, 23);
            DTPLiberarBL.TabIndex = 22;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new Point(12, 263);
            label11.Name = "label11";
            label11.Size = new Size(95, 15);
            label11.TabIndex = 25;
            label11.Text = "M.M ou Isenção:";
            // 
            // DTPIsencaoMarinha
            // 
            DTPIsencaoMarinha.Format = DateTimePickerFormat.Short;
            DTPIsencaoMarinha.Location = new Point(113, 259);
            DTPIsencaoMarinha.Name = "DTPIsencaoMarinha";
            DTPIsencaoMarinha.Size = new Size(107, 23);
            DTPIsencaoMarinha.TabIndex = 24;
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Location = new Point(226, 263);
            label12.Name = "label12";
            label12.Size = new Size(56, 15);
            label12.TabIndex = 27;
            label12.Text = "SISCarga:";
            // 
            // DTPSisCarga
            // 
            DTPSisCarga.Format = DateTimePickerFormat.Short;
            DTPSisCarga.Location = new Point(288, 259);
            DTPSisCarga.Name = "DTPSisCarga";
            DTPSisCarga.Size = new Size(186, 23);
            DTPSisCarga.TabIndex = 26;
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Location = new Point(13, 350);
            label15.Name = "label15";
            label15.Size = new Size(136, 15);
            label15.TabIndex = 31;
            label15.Text = "Ent Para Transportadora:";
            // 
            // DTPEntTransporte
            // 
            DTPEntTransporte.Format = DateTimePickerFormat.Short;
            DTPEntTransporte.Location = new Point(155, 346);
            DTPEntTransporte.Name = "DTPEntTransporte";
            DTPEntTransporte.Size = new Size(107, 23);
            DTPEntTransporte.TabIndex = 30;
            // 
            // label16
            // 
            label16.AutoSize = true;
            label16.Location = new Point(12, 292);
            label16.Name = "label16";
            label16.Size = new Size(38, 15);
            label16.TabIndex = 29;
            label16.Text = "ICMS:";
            // 
            // DTPICMS
            // 
            DTPICMS.Format = DateTimePickerFormat.Short;
            DTPICMS.Location = new Point(56, 288);
            DTPICMS.Name = "DTPICMS";
            DTPICMS.Size = new Size(164, 23);
            DTPICMS.TabIndex = 28;
            // 
            // label17
            // 
            label17.AutoSize = true;
            label17.Location = new Point(267, 350);
            label17.Name = "label17";
            label17.Size = new Size(90, 15);
            label17.TabIndex = 36;
            label17.Text = "Transportadora:";
            // 
            // txtTransporte
            // 
            txtTransporte.Location = new Point(363, 347);
            txtTransporte.Name = "txtTransporte";
            txtTransporte.Size = new Size(112, 23);
            txtTransporte.TabIndex = 37;
            // 
            // txtDOSSIE
            // 
            txtDOSSIE.Location = new Point(280, 317);
            txtDOSSIE.Name = "txtDOSSIE";
            txtDOSSIE.Size = new Size(194, 23);
            txtDOSSIE.TabIndex = 41;
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Location = new Point(226, 321);
            label13.Name = "label13";
            label13.Size = new Size(48, 15);
            label13.TabIndex = 40;
            label13.Text = "DOSSIÊ:";
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Location = new Point(12, 321);
            label14.Name = "label14";
            label14.Size = new Size(84, 15);
            label14.TabIndex = 39;
            label14.Text = "Ent Alfândega:";
            // 
            // DTPEntAlfandega
            // 
            DTPEntAlfandega.Format = DateTimePickerFormat.Short;
            DTPEntAlfandega.Location = new Point(102, 317);
            DTPEntAlfandega.Name = "DTPEntAlfandega";
            DTPEntAlfandega.Size = new Size(118, 23);
            DTPEntAlfandega.TabIndex = 38;
            // 
            // Impostos
            // 
            Impostos.FormattingEnabled = true;
            Impostos.Items.AddRange(new object[] { "I.I.", "I.P.I.", "PIS/PASEP", "COFINS", "ICMS" });
            Impostos.Location = new Point(12, 182);
            Impostos.MultiColumn = true;
            Impostos.Name = "Impostos";
            Impostos.Size = new Size(462, 40);
            Impostos.TabIndex = 42;
            // 
            // label19
            // 
            label19.AutoSize = true;
            label19.Location = new Point(226, 292);
            label19.Name = "label19";
            label19.Size = new Size(106, 15);
            label19.TabIndex = 44;
            label19.Text = "Conferência Física:";
            // 
            // DTPConferenciaFisica
            // 
            DTPConferenciaFisica.Format = DateTimePickerFormat.Short;
            DTPConferenciaFisica.Location = new Point(338, 288);
            DTPConferenciaFisica.Name = "DTPConferenciaFisica";
            DTPConferenciaFisica.Size = new Size(136, 23);
            DTPConferenciaFisica.TabIndex = 43;
            // 
            // label18
            // 
            label18.AutoSize = true;
            label18.Font = new Font("Segoe UI", 9F, FontStyle.Underline);
            label18.Location = new Point(480, 9);
            label18.Name = "label18";
            label18.Size = new Size(72, 15);
            label18.TabIndex = 45;
            label18.Text = "Observação:";
            // 
            // txtObservacao
            // 
            txtObservacao.Location = new Point(480, 27);
            txtObservacao.Multiline = true;
            txtObservacao.Name = "txtObservacao";
            txtObservacao.Size = new Size(254, 399);
            txtObservacao.TabIndex = 46;
            // 
            // btnExportar
            // 
            btnExportar.Location = new Point(12, 534);
            btnExportar.Name = "btnExportar";
            btnExportar.Size = new Size(211, 23);
            btnExportar.TabIndex = 48;
            btnExportar.Text = "Exportar";
            btnExportar.UseVisualStyleBackColor = true;
            btnExportar.Click += btnExportar_Click;
            // 
            // btnSalvar
            // 
            btnSalvar.Location = new Point(272, 534);
            btnSalvar.Name = "btnSalvar";
            btnSalvar.Size = new Size(211, 23);
            btnSalvar.TabIndex = 49;
            btnSalvar.Text = "Salvar";
            btnSalvar.UseVisualStyleBackColor = true;
            btnSalvar.Click += btnSalvar_Click_1;
            // 
            // btnCancelar
            // 
            btnCancelar.Location = new Point(523, 534);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new Size(211, 23);
            btnCancelar.TabIndex = 50;
            btnCancelar.Text = "Cancelar";
            btnCancelar.UseVisualStyleBackColor = true;
            btnCancelar.Click += btnCancelar_Click_1;
            // 
            // txtPagoPor
            // 
            txtPagoPor.Location = new Point(285, 20);
            txtPagoPor.Name = "txtPagoPor";
            txtPagoPor.Size = new Size(150, 23);
            txtPagoPor.TabIndex = 52;
            // 
            // label20
            // 
            label20.AutoSize = true;
            label20.Location = new Point(225, 23);
            label20.Name = "label20";
            label20.Size = new Size(58, 15);
            label20.TabIndex = 51;
            label20.Text = "Pago Por:";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(cbArmazenagem);
            groupBox1.Controls.Add(cbArmFaturado);
            groupBox1.Controls.Add(txtPagoPor);
            groupBox1.Controls.Add(label20);
            groupBox1.Location = new Point(13, 375);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(461, 51);
            groupBox1.TabIndex = 53;
            groupBox1.TabStop = false;
            groupBox1.Text = "Armazenamento";
            // 
            // cbArmazenagem
            // 
            cbArmazenagem.AutoSize = true;
            cbArmazenagem.Location = new Point(24, 22);
            cbArmazenagem.Name = "cbArmazenagem";
            cbArmazenagem.Size = new Size(103, 19);
            cbArmazenagem.TabIndex = 53;
            cbArmazenagem.Text = "Armazenagem";
            cbArmazenagem.UseVisualStyleBackColor = true;
            // 
            // cbArmFaturado
            // 
            cbArmFaturado.AutoSize = true;
            cbArmFaturado.Location = new Point(143, 22);
            cbArmFaturado.Name = "cbArmFaturado";
            cbArmFaturado.Size = new Size(73, 19);
            cbArmFaturado.TabIndex = 0;
            cbArmFaturado.Text = "Faturado";
            cbArmFaturado.UseVisualStyleBackColor = true;
            // 
            // cbNumerario
            // 
            cbNumerario.FormattingEnabled = true;
            cbNumerario.Items.AddRange(new object[] { "Prestação Serviço", "Agência", "Tributos", "Completo", "Complementar" });
            cbNumerario.Location = new Point(385, 452);
            cbNumerario.MultiColumn = true;
            cbNumerario.Name = "cbNumerario";
            cbNumerario.Size = new Size(349, 76);
            cbNumerario.TabIndex = 54;
            // 
            // label21
            // 
            label21.AutoSize = true;
            label21.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            label21.Location = new Point(156, 429);
            label21.Name = "label21";
            label21.Size = new Size(81, 20);
            label21.TabIndex = 55;
            label21.Text = "Adicionais";
            // 
            // label22
            // 
            label22.AutoSize = true;
            label22.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            label22.Location = new Point(517, 429);
            label22.Name = "label22";
            label22.Size = new Size(85, 20);
            label22.TabIndex = 56;
            label22.Text = "Numerário";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(CbLiberado);
            groupBox2.Controls.Add(CbSelecionado);
            groupBox2.Controls.Add(DTPSigvig);
            groupBox2.Location = new Point(11, 148);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(463, 33);
            groupBox2.TabIndex = 57;
            groupBox2.TabStop = false;
            groupBox2.Text = "Sigvig";
            // 
            // CbLiberado
            // 
            CbLiberado.AutoSize = true;
            CbLiberado.Location = new Point(168, 9);
            CbLiberado.Name = "CbLiberado";
            CbLiberado.Size = new Size(72, 19);
            CbLiberado.TabIndex = 19;
            CbLiberado.Text = "Liberado";
            CbLiberado.UseVisualStyleBackColor = true;
            // 
            // CbSelecionado
            // 
            CbSelecionado.AutoSize = true;
            CbSelecionado.Location = new Point(72, 9);
            CbSelecionado.Name = "CbSelecionado";
            CbSelecionado.Size = new Size(90, 19);
            CbSelecionado.TabIndex = 18;
            CbSelecionado.Text = "Selecionado";
            CbSelecionado.UseVisualStyleBackColor = true;
            // 
            // FrmModificaCapa
            // 
            ClientSize = new Size(746, 565);
            Controls.Add(groupBox2);
            Controls.Add(label22);
            Controls.Add(label21);
            Controls.Add(cbNumerario);
            Controls.Add(groupBox1);
            Controls.Add(btnCancelar);
            Controls.Add(btnSalvar);
            Controls.Add(btnExportar);
            Controls.Add(txtObservacao);
            Controls.Add(label18);
            Controls.Add(label19);
            Controls.Add(DTPConferenciaFisica);
            Controls.Add(Impostos);
            Controls.Add(txtDOSSIE);
            Controls.Add(label13);
            Controls.Add(label14);
            Controls.Add(DTPEntAlfandega);
            Controls.Add(txtTransporte);
            Controls.Add(label17);
            Controls.Add(label15);
            Controls.Add(DTPEntTransporte);
            Controls.Add(label16);
            Controls.Add(DTPICMS);
            Controls.Add(label12);
            Controls.Add(DTPSisCarga);
            Controls.Add(label11);
            Controls.Add(DTPIsencaoMarinha);
            Controls.Add(label10);
            Controls.Add(DTPLiberarBL);
            Controls.Add(label9);
            Controls.Add(DTPAverbar);
            Controls.Add(ItensAdicionais);
            Controls.Add(cbIncotern);
            Controls.Add(txtMarinha);
            Controls.Add(label5);
            Controls.Add(txtCE);
            Controls.Add(label6);
            Controls.Add(txtDTA);
            Controls.Add(label8);
            Controls.Add(txtContainer);
            Controls.Add(label4);
            Controls.Add(TxtIncoterm);
            Controls.Add(label3);
            Controls.Add(txtMaster);
            Controls.Add(label1);
            Name = "FrmModificaCapa";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Modificar Capa";
            FormClosing += frmModificaCapa_FormClosing;
            Load += FrmModificaCapa_Load;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }
        private Label label1;
        private TextBox txtMaster;
        private TextBox TxtIncoterm;
        private Label label3;
        private TextBox txtContainer;
        private Label label4;
        private TextBox txtMarinha;
        private Label label5;
        private TextBox txtCE;
        private Label label6;
        private TextBox txtDTA;
        private Label label8;
        private DateTimePicker DTPSigvig;
        private CheckBox cbIncotern;
        private CheckedListBox ItensAdicionais;
        private DateTimePicker DTPAverbar;
        private Label label9;
        private Label label10;
        private DateTimePicker DTPLiberarBL;
        private Label label11;
        private DateTimePicker DTPIsencaoMarinha;
        private Label label12;
        private DateTimePicker DTPSisCarga;
        private Label label15;
        private DateTimePicker DTPEntTransporte;
        private Label label16;
        private DateTimePicker DTPICMS;
        private Label label17;
        private TextBox txtTransporte;
        private TextBox txtDOSSIE;
        private Label label13;
        private Label label14;
        private DateTimePicker DTPEntAlfandega;
        private CheckedListBox Impostos;
        private Label label19;
        private DateTimePicker DTPConferenciaFisica;
        private Label label18;
        private TextBox txtObservacao;
        private Button btnExportar;
        private Button btnSalvar;
        private Button btnCancelar;
        private TextBox txtPagoPor;
        private Label label20;
        private GroupBox groupBox1;
        private CheckBox cbArmazenagem;
        private CheckBox cbArmFaturado;
        private CheckedListBox cbNumerario;
        private Label label21;
        private Label label22;
        private GroupBox groupBox2;
        private CheckBox CbSelecionado;
        private CheckBox CbLiberado;
    }
}
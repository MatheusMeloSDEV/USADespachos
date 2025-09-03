using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;


namespace Trabalho
{
    partial class UCOrgaoAnuente : UserControl
    {
        private IContainer components = null;
        private BindingSource BsModifica;
        private System.Windows.Forms.Timer tErro;
        private Label LBLinspecao;
        private Label LBLdatadeatracacao;
        private Label LBLdatadeembarque;
        private DateTimePicker DTPdatadeinspecao;
        private CheckBox CBamostra;
        private Button BtnCancelar;
        private Label Label14;
        private TextBox TXTorigem;
        private Button BtnOK;
        private DateTimePicker DTPdatadeatracacao;
        private DateTimePicker DTPdatadeembarque;
        private Label Label11;
        private Label Label9;
        private TextBox TXTsr;
        private TextBox TXTnr;
        private Label Label10;
        private TextBox TXTstatusdoprocesso;
        private Label Label7;
        private TextBox TXTProduto;
        private Label Label8;
        private TextBox TXTpendencia;
        private Label Exportador;
        private TextBox TXTexportador;
        private Label label2;
        private TextBox TXTNavio;
        private Label Label17;
        private TextBox TXTimportador;
        private GroupBox groupBox1;
        private FlowLayoutPanel flpLis;
        private TextBox TXTterminal;

        /// <summary> 
        /// Required designer variable. 
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code 
        private void InitializeComponent()
        {
            components = new Container();
            BsModifica = new BindingSource(components);
            tErro = new System.Windows.Forms.Timer(components);
            LBLinspecao = new Label();
            LBLdatadeatracacao = new Label();
            LBLdatadeembarque = new Label();
            DTPdatadeinspecao = new DateTimePicker();
            CBamostra = new CheckBox();
            BtnCancelar = new Button();
            Label14 = new Label();
            TXTorigem = new TextBox();
            BtnOK = new Button();
            DTPdatadeatracacao = new DateTimePicker();
            DTPdatadeembarque = new DateTimePicker();
            Label11 = new Label();
            Label9 = new Label();
            TXTsr = new TextBox();
            TXTnr = new TextBox();
            Label10 = new Label();
            TXTstatusdoprocesso = new TextBox();
            Label7 = new Label();
            TXTProduto = new TextBox();
            Label8 = new Label();
            TXTpendencia = new TextBox();
            Exportador = new Label();
            TXTexportador = new TextBox();
            label2 = new Label();
            TXTNavio = new TextBox();
            Label17 = new Label();
            TXTimportador = new TextBox();
            TXTterminal = new TextBox();
            groupBox1 = new GroupBox();
            flpLis = new FlowLayoutPanel();
            ((ISupportInitialize)BsModifica).BeginInit();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // tErro
            // 
            tErro.Tick += TErro_Tick;
            // 
            // LBLinspecao
            // 
            LBLinspecao.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            LBLinspecao.AutoSize = true;
            LBLinspecao.Font = new Font("Segoe UI", 11F);
            LBLinspecao.Location = new Point(438, 333);
            LBLinspecao.Name = "LBLinspecao";
            LBLinspecao.Size = new Size(68, 20);
            LBLinspecao.TabIndex = 304;
            LBLinspecao.Text = "Inspeção";
            // 
            // LBLdatadeatracacao
            // 
            LBLdatadeatracacao.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            LBLdatadeatracacao.AutoSize = true;
            LBLdatadeatracacao.Font = new Font("Segoe UI", 11F);
            LBLdatadeatracacao.Location = new Point(238, 333);
            LBLdatadeatracacao.Name = "LBLdatadeatracacao";
            LBLdatadeatracacao.Size = new Size(133, 20);
            LBLdatadeatracacao.TabIndex = 303;
            LBLdatadeatracacao.Text = "Data de Atracação";
            // 
            // LBLdatadeembarque
            // 
            LBLdatadeembarque.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            LBLdatadeembarque.AutoSize = true;
            LBLdatadeembarque.Font = new Font("Segoe UI", 11F);
            LBLdatadeembarque.Location = new Point(574, 334);
            LBLdatadeembarque.Name = "LBLdatadeembarque";
            LBLdatadeembarque.Size = new Size(134, 20);
            LBLdatadeembarque.TabIndex = 302;
            LBLdatadeembarque.Text = "Data de Embarque";
            // 
            // DTPdatadeinspecao
            // 
            DTPdatadeinspecao.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            DTPdatadeinspecao.Format = DateTimePickerFormat.Short;
            DTPdatadeinspecao.Location = new Point(405, 357);
            DTPdatadeinspecao.Name = "DTPdatadeinspecao";
            DTPdatadeinspecao.Size = new Size(135, 23);
            DTPdatadeinspecao.TabIndex = 296;
            DTPdatadeinspecao.ValueChanged += DateTimePicker_OnValueChanged;
            // 
            // CBamostra
            // 
            CBamostra.AutoSize = true;
            CBamostra.Font = new Font("Segoe UI", 12F);
            CBamostra.Location = new Point(48, 542);
            CBamostra.Name = "CBamostra";
            CBamostra.Size = new Size(88, 25);
            CBamostra.TabIndex = 295;
            CBamostra.Text = "Amostra";
            CBamostra.UseVisualStyleBackColor = true;
            // 
            // BtnCancelar
            // 
            BtnCancelar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            BtnCancelar.Location = new Point(662, 535);
            BtnCancelar.Name = "BtnCancelar";
            BtnCancelar.Size = new Size(112, 32);
            BtnCancelar.TabIndex = 294;
            BtnCancelar.Text = "Cancelar";
            BtnCancelar.UseVisualStyleBackColor = true;
            BtnCancelar.Click += BtnCancelar_Click;
            // 
            // Label14
            // 
            Label14.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            Label14.AutoSize = true;
            Label14.Font = new Font("Segoe UI", 12F);
            Label14.Location = new Point(653, 63);
            Label14.Name = "Label14";
            Label14.Size = new Size(63, 21);
            Label14.TabIndex = 293;
            Label14.Text = "Origem";
            // 
            // TXTorigem
            // 
            TXTorigem.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            TXTorigem.Location = new Point(722, 61);
            TXTorigem.Name = "TXTorigem";
            TXTorigem.Size = new Size(181, 23);
            TXTorigem.TabIndex = 292;
            // 
            // BtnOK
            // 
            BtnOK.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            BtnOK.Location = new Point(780, 535);
            BtnOK.Name = "BtnOK";
            BtnOK.Size = new Size(112, 32);
            BtnOK.TabIndex = 291;
            BtnOK.Text = "Salvar";
            BtnOK.UseVisualStyleBackColor = true;
            BtnOK.Click += BtnOK_Click;
            // 
            // DTPdatadeatracacao
            // 
            DTPdatadeatracacao.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            DTPdatadeatracacao.Format = DateTimePickerFormat.Short;
            DTPdatadeatracacao.Location = new Point(237, 357);
            DTPdatadeatracacao.Name = "DTPdatadeatracacao";
            DTPdatadeatracacao.Size = new Size(135, 23);
            DTPdatadeatracacao.TabIndex = 290;
            DTPdatadeatracacao.ValueChanged += DateTimePicker_OnValueChanged;
            // 
            // DTPdatadeembarque
            // 
            DTPdatadeembarque.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            DTPdatadeembarque.Format = DateTimePickerFormat.Short;
            DTPdatadeembarque.Location = new Point(574, 357);
            DTPdatadeembarque.Name = "DTPdatadeembarque";
            DTPdatadeembarque.Size = new Size(135, 23);
            DTPdatadeembarque.TabIndex = 289;
            DTPdatadeembarque.ValueChanged += DateTimePicker_OnValueChanged;
            // 
            // Label11
            // 
            Label11.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            Label11.AutoSize = true;
            Label11.Font = new Font("Segoe UI", 12F);
            Label11.Location = new Point(495, 12);
            Label11.Name = "Label11";
            Label11.Size = new Size(49, 21);
            Label11.TabIndex = 288;
            Label11.Text = "S. Ref";
            // 
            // Label9
            // 
            Label9.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            Label9.AutoSize = true;
            Label9.Font = new Font("Segoe UI", 12F);
            Label9.Location = new Point(254, 12);
            Label9.Name = "Label9";
            Label9.Size = new Size(70, 21);
            Label9.TabIndex = 287;
            Label9.Text = "Ref. USA";
            // 
            // TXTsr
            // 
            TXTsr.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            TXTsr.Location = new Point(550, 12);
            TXTsr.Name = "TXTsr";
            TXTsr.Size = new Size(136, 23);
            TXTsr.TabIndex = 286;
            // 
            // TXTnr
            // 
            TXTnr.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            TXTnr.Location = new Point(326, 12);
            TXTnr.Name = "TXTnr";
            TXTnr.Size = new Size(126, 23);
            TXTnr.TabIndex = 285;
            // 
            // Label10
            // 
            Label10.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            Label10.AutoSize = true;
            Label10.Font = new Font("Segoe UI", 12F, FontStyle.Underline);
            Label10.Location = new Point(46, 394);
            Label10.Name = "Label10";
            Label10.Size = new Size(140, 21);
            Label10.TabIndex = 284;
            Label10.Text = "Status do Processo";
            // 
            // TXTstatusdoprocesso
            // 
            TXTstatusdoprocesso.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            TXTstatusdoprocesso.Location = new Point(192, 393);
            TXTstatusdoprocesso.Multiline = true;
            TXTstatusdoprocesso.Name = "TXTstatusdoprocesso";
            TXTstatusdoprocesso.Size = new Size(684, 74);
            TXTstatusdoprocesso.TabIndex = 283;
            // 
            // Label7
            // 
            Label7.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            Label7.AutoSize = true;
            Label7.Font = new Font("Segoe UI", 12F);
            Label7.Location = new Point(342, 63);
            Label7.Name = "Label7";
            Label7.Size = new Size(66, 21);
            Label7.TabIndex = 282;
            Label7.Text = "Produto";
            // 
            // TXTProduto
            // 
            TXTProduto.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            TXTProduto.Location = new Point(414, 61);
            TXTProduto.Name = "TXTProduto";
            TXTProduto.Size = new Size(178, 23);
            TXTProduto.TabIndex = 281;
            // 
            // Label8
            // 
            Label8.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            Label8.AutoSize = true;
            Label8.Font = new Font("Segoe UI", 12F);
            Label8.Location = new Point(94, 486);
            Label8.Name = "Label8";
            Label8.Size = new Size(80, 21);
            Label8.TabIndex = 280;
            Label8.Text = "Pendência";
            // 
            // TXTpendencia
            // 
            TXTpendencia.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            TXTpendencia.Location = new Point(177, 484);
            TXTpendencia.Name = "TXTpendencia";
            TXTpendencia.Size = new Size(671, 23);
            TXTpendencia.TabIndex = 279;
            // 
            // Exportador
            // 
            Exportador.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            Exportador.AutoSize = true;
            Exportador.Font = new Font("Segoe UI", 12F);
            Exportador.Location = new Point(37, 63);
            Exportador.Name = "Exportador";
            Exportador.Size = new Size(86, 21);
            Exportador.TabIndex = 278;
            Exportador.Text = "Exportador";
            // 
            // TXTexportador
            // 
            TXTexportador.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            TXTexportador.Location = new Point(132, 61);
            TXTexportador.Name = "TXTexportador";
            TXTexportador.Size = new Size(137, 23);
            TXTexportador.TabIndex = 277;
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F);
            label2.Location = new Point(506, 114);
            label2.Name = "label2";
            label2.Size = new Size(60, 21);
            label2.TabIndex = 389;
            label2.Text = "Veículo";
            // 
            // TXTNavio
            // 
            TXTNavio.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            TXTNavio.Location = new Point(572, 112);
            TXTNavio.Name = "TXTNavio";
            TXTNavio.Size = new Size(136, 23);
            TXTNavio.TabIndex = 388;
            // 
            // Label17
            // 
            Label17.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            Label17.AutoSize = true;
            Label17.Font = new Font("Segoe UI", 12F);
            Label17.Location = new Point(242, 118);
            Label17.Name = "Label17";
            Label17.Size = new Size(89, 21);
            Label17.TabIndex = 385;
            Label17.Text = "Importador";
            // 
            // TXTimportador
            // 
            TXTimportador.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            TXTimportador.Location = new Point(337, 116);
            TXTimportador.Name = "TXTimportador";
            TXTimportador.Size = new Size(126, 23);
            TXTimportador.TabIndex = 384;
            // 
            // TXTterminal
            // 
            TXTterminal.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            TXTterminal.Location = new Point(132, 112);
            TXTterminal.Name = "TXTterminal";
            TXTterminal.Size = new Size(100, 23);
            TXTterminal.TabIndex = 391;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(flpLis);
            groupBox1.Location = new Point(12, 145);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(917, 175);
            groupBox1.TabIndex = 390;
            groupBox1.TabStop = false;
            groupBox1.Text = "LI";
            // 
            // flpLis
            // 
            flpLis.Location = new Point(6, 22);
            flpLis.Name = "flpLis";
            flpLis.Size = new Size(905, 147);
            flpLis.TabIndex = 380;
            // 
            // UCOrgaoAnuente
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(groupBox1);
            Controls.Add(label2);
            Controls.Add(TXTNavio);
            Controls.Add(Label17);
            Controls.Add(TXTimportador);
            Controls.Add(LBLinspecao);
            Controls.Add(LBLdatadeatracacao);
            Controls.Add(LBLdatadeembarque);
            Controls.Add(DTPdatadeinspecao);
            Controls.Add(CBamostra);
            Controls.Add(BtnCancelar);
            Controls.Add(Label14);
            Controls.Add(TXTorigem);
            Controls.Add(BtnOK);
            Controls.Add(DTPdatadeatracacao);
            Controls.Add(DTPdatadeembarque);
            Controls.Add(Label11);
            Controls.Add(Label9);
            Controls.Add(TXTsr);
            Controls.Add(TXTnr);
            Controls.Add(Label10);
            Controls.Add(TXTstatusdoprocesso);
            Controls.Add(Label7);
            Controls.Add(TXTProduto);
            Controls.Add(Label8);
            Controls.Add(TXTpendencia);
            Controls.Add(Exportador);
            Controls.Add(TXTexportador);
            Name = "UCOrgaoAnuente";
            Size = new Size(941, 582);
            Load += UCOrgaoAnuente_Load;
            ((ISupportInitialize)BsModifica).EndInit();
            groupBox1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }
        #endregion
    }
}

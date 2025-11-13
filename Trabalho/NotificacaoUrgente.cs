using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Trabalho
{
    public partial class NotificacaoUrgente : UserControl
    {
        public string Usuario
        {
            get => lblUser.Text;
            set => lblUser.Text = value;
        }
        public string Mensagem
        {
            get => txtMensagem.Text;
            set => txtMensagem.Text = value;
        }
        public bool MensagemReadOnly 
        {
            get => txtMensagem.ReadOnly; 
            set => txtMensagem.ReadOnly = value; 
        }
        public bool BotaoEditarVisible 
        {
            get => BtnEditar.Visible; 
            set => BtnEditar.Visible = value; 
        }

        public event EventHandler ExcluirClick;
        public event EventHandler DoneClick;
        public event EventHandler EditClick;
        public event EventHandler<string> MensagemEditada;
        public void FocusMensagem() { txtMensagem.Focus(); }

        public NotificacaoUrgente()
        {
            InitializeComponent();
            txtMensagem.ReadOnly = true;
            BtnEditar.Visible = true; // Visibilidade controlada externamente
            BtnExcluir.Click += (s, e) => ExcluirClick?.Invoke(this, EventArgs.Empty);
            BtnDone.Click += (s, e) =>
            {
                MessageBox.Show("Mensagem concluída!");
                DoneClick?.Invoke(this, EventArgs.Empty);
            };

            BtnEditar.Click += (s, e) =>
            {
                // Libera edição e foca no campo
                txtMensagem.ReadOnly = false;
                txtMensagem.Focus();
            };

            // Salva ao pressionar Enter OU ao perder o foco
            txtMensagem.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter && !txtMensagem.ReadOnly)
                {
                    SalvarMensagemEditada();
                    e.Handled = true;
                }
            };
            txtMensagem.Leave += (s, e) =>
            {
                if (!txtMensagem.ReadOnly)
                {
                    SalvarMensagemEditada();
                }
            };
        }
        private void SalvarMensagemEditada()
        {
            if (!txtMensagem.ReadOnly)
            {
                txtMensagem.ReadOnly = true;
                MensagemEditada?.Invoke(this, txtMensagem.Text);
            }
        }
    }
}

using CLUSA;
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
    public partial class FrmMudarSenha : Form
    {
        private Logado _usuarioLogado;
        public FrmMudarSenha(Logado usuarioLogado)
        {
            InitializeComponent();
            _usuarioLogado = usuarioLogado;
        }

        private async void BtnSalvar_Click(object sender, EventArgs e)
        {
            string oldPass = TxtOldPassword.Text;
            string newPass = TxtNewPassword.Text;

            if (string.IsNullOrWhiteSpace(oldPass) || string.IsNullOrWhiteSpace(newPass))
            {
                MessageBox.Show("Preencha ambos os campos.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var repo = new RepositorioUsers();
            // Busque o usuário pelo Id do usuário logado (que foi passado no construtor)
            var user = repo.ListaUsers.FirstOrDefault(u => u.Id == _usuarioLogado.Id);

            if (user == null)
            {
                MessageBox.Show("Usuário não encontrado.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Valide senha antiga
            if (user.Password != oldPass)
            {
                MessageBox.Show("Senha antiga incorreta.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Atualize senha
            user.Password = newPass;
            await repo.UpdateAsync(user);

            MessageBox.Show("Senha alterada com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }
    }
}

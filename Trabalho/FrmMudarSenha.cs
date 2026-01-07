using CLUSA.Repositories;
using CLUSA.Models;

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

            // CORREÇÃO 1: Passar o banco de dados para o construtor
            var db = ConfigDatabase.GetDatabase();
            var repo = new RepositorioUsers(db);

            try
            {
                var user = await repo.GetByIdAsync(_usuarioLogado.Id);

                if (user == null)
                {
                    MessageBox.Show("Usuário não encontrado no banco de dados.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

                // Salva a alteração
                await repo.UpdateAsync(user);

                MessageBox.Show("Senha alterada com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao alterar senha: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

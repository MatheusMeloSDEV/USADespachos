using CLUSA;
using MongoDB.Bson;

namespace Trabalho
{
    public partial class FrmAddNotifUrgente : Form
    {
        public ObjectId IdOrigem { get; }
        public ObjectId? IdDestinoSelecionado { get; private set; }
        public string MensagemCriada { get; private set; } = "";

        public FrmAddNotifUrgente(ObjectId idOrigem, List<UsuarioDestinoItem> usuariosDestino)
        {
            InitializeComponent();
            IdOrigem = idOrigem;

            comboBoxDestino.DataSource = usuariosDestino;
            comboBoxDestino.DisplayMember = "NomeUsuario";
            comboBoxDestino.ValueMember = "Id";
        }


        private void BtnEnviar_Click(object sender, EventArgs e)
        {
            if (comboBoxDestino.SelectedValue is ObjectId idDest && !string.IsNullOrWhiteSpace(txtMensagem.Text))
            {
                IdDestinoSelecionado = idDest;
                MensagemCriada = txtMensagem.Text;
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                MessageBox.Show("Selecione o usuário e digite a mensagem.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}

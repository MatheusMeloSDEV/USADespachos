using CLUSA;

namespace Trabalho
{
    public class FrmModifica<T> : Form where T : OrgaoAnuente, new()
    {
        private readonly RepositorioOrgaoAnuente<T> _repositorio;
        private readonly string _nomeColecao;
        public bool VisualizacaoCheck;
        public T Entidade { get; set; }
        private UCOrgaoAnuente uc; // nosso UserControl

        public FrmModifica(string nomeColecao, T? entidade = null, bool visualizacao = false) // Adicione o parâmetro aqui
        {
            _nomeColecao = nomeColecao;
            _repositorio = new RepositorioOrgaoAnuente<T>(_nomeColecao);
            Entidade = entidade ?? new T();
            VisualizacaoCheck = visualizacao; 

            uc = new UCOrgaoAnuente
            {
                Dock = DockStyle.Fill,
                Visualizacao = this.VisualizacaoCheck 
            };
            this.Controls.Clear();
            this.Controls.Add(uc);

            this.Width = 960;  
            this.Height = 630; 

            this.Load += FrmModifica_Load;

            uc.OnConfirmar += Uc_OnConfirmar;
            uc.OnCancelar += Uc_OnCancelar;
        }

        private async void FrmModifica_Load(object sender, EventArgs e)
        {
            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

            // Carrega dados no UserControl
            var entidadeDoRepo = await _repositorio.ObterPorIdAsync(Entidade.Ref_USA);
            if (entidadeDoRepo != null)
                Entidade = entidadeDoRepo;

            uc.CarregarCamposBase(Entidade);
            uc.CarregarLisBase(Entidade);
        }

        // Quando o usuário clicar em “Salvar” no UserControl:
        private async void Uc_OnConfirmar(object sender, EventArgs e)
        {
            if (VisualizacaoCheck)
            {
                this.DialogResult = DialogResult.Cancel; // Ou OK, dependendo do que preferir
                this.Close();
                return;
            }

            uc.ExtrairParaEntidade(Entidade);

            // Salva no repositório
            await _repositorio.AtualizarAsync(Entidade.Ref_USA, Entidade);

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        // Quando o usuário clicar em “Cancelar” no UserControl:
        private void Uc_OnCancelar(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

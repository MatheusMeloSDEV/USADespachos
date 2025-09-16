using CLUSA;

namespace Trabalho
{
    public partial class LiDisplayControl : UserControl
    {
        public LiDisplayControl()
        {
            InitializeComponent();
        }

        public void CarregarDados(LicencaImportacao li)
        {
            lblNumeroLi.Text = $"LI: {li.Numero}";
            lblNcm.Text = $"NCM: {li.NCM}";

            if (li.DataRegistro.HasValue)
            {
                lblDataRegistro.Text = $"Registro: {li.DataRegistro.Value:dd/MM/yyyy}";
            }
            else
            {
                lblDataRegistro.Text = "Registro: -";
            }
        }
    }
}
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
    public partial class FrmLoadingOverlay : Form
    {
        public FrmLoadingOverlay()
        {
            InitializeComponent();
        }
        private void FrmLoadingOverlay_Load(object sender, EventArgs e)
        {
            CentralizarControles();
        }

        private void FrmLoadingOverlay_Resize(object sender, EventArgs e)
        {
            CentralizarControles();
        }

        private void CentralizarControles()
        {
            // centraliza o PictureBox
            picLoading.Left = (ClientSize.Width - picLoading.Width) / 2;
            picLoading.Top = (ClientSize.Height - picLoading.Height) / 2 - 20;

            // centraliza o Label logo abaixo
            lblLoading.AutoSize = true;
            lblLoading.Left = (ClientSize.Width - lblLoading.Width) / 2;
            lblLoading.Top = picLoading.Bottom + 10;
        }
    }
}

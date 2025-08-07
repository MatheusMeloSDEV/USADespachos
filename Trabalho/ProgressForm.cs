namespace Trabalho
{
    public partial class ProgressForm : Form
    {
        private readonly Label _messageLabel;
        private readonly ProgressBar _marqueeBar;

        public ProgressForm()
        {
            // Basic form settings
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.ControlBox = false;
            this.ShowInTaskbar = false;
            this.Text = "Aguarde";
            this.ClientSize = new Size(400, 100);

            // Message label
            _messageLabel = new Label()
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                Text = "Aguarde, a operação pode levar alguns segundos.",
            };
            this.Controls.Add(_messageLabel);

            // Marquee progress bar
            _marqueeBar = new ProgressBar()
            {
                Style = ProgressBarStyle.Marquee,
                MarqueeAnimationSpeed = 30,
                Dock = DockStyle.Bottom,
                Height = 20,
            };
            this.Controls.Add(_marqueeBar);

            InitializeComponent();
        }
    }
}

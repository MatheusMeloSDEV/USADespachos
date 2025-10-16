using CLUSA;

namespace Trabalho
{
    public partial class LpcoEditControl : UserControl
    {
        private LpcoInfo? _lpco;

        public LpcoEditControl()
        {
            InitializeComponent();
        }

        public void VincularDados(LpcoInfo lpco)
        {
            _lpco = lpco;
            if (_lpco == null) return;

            // Vincula os dados usando DataBinding
            TxtLPCO.DataBindings.Add("Text", _lpco, nameof(_lpco.LPCO), false, DataSourceUpdateMode.OnPropertyChanged);
            CbParametrizacao.DataBindings.Add("Text", _lpco, nameof(_lpco.ParametrizacaoLPCO), false, DataSourceUpdateMode.OnPropertyChanged);
            CbMotivoExigencia.DataBindings.Add("Text", _lpco, nameof(_lpco.MotivoExigencia), false, DataSourceUpdateMode.OnPropertyChanged);
            ConfigurarDatePickerNulavel(DtpDataRegistroLPCO, _lpco.DataRegistroLPCO);
            ConfigurarDatePickerNulavel(DtpDataDeferimentoLPCO, _lpco.DataDeferimentoLPCO);


            CbParametrizacao.DropDownStyle = ComboBoxStyle.DropDownList;
            CbParametrizacao.FlatStyle = FlatStyle.Flat;
            CbParametrizacao.BackColor = SystemColors.Window; // Ou Color.White
            CbParametrizacao.Enter += (s, e) => CbParametrizacao.BackColor = SystemColors.Window;
            CbParametrizacao.Leave += (s, e) => CbParametrizacao.BackColor = SystemColors.Window;

            CbMotivoExigencia.DropDownStyle = ComboBoxStyle.DropDownList;
            CbMotivoExigencia.FlatStyle = FlatStyle.Flat;
            CbMotivoExigencia.BackColor = SystemColors.Window; // Ou Color.White
            CbMotivoExigencia.Enter += (s, e) => CbMotivoExigencia.BackColor = SystemColors.Window;
            CbMotivoExigencia.Leave += (s, e) => CbMotivoExigencia.BackColor = SystemColors.Window;

        }

        public void SalvarAlteracoes()
        {
            if (_lpco == null) return;

            this.BindingContext[_lpco].EndCurrentEdit();

            _lpco.DataRegistroLPCO = DtpDataRegistroLPCO.Checked ? DtpDataRegistroLPCO.Value : null;
            _lpco.DataDeferimentoLPCO = DtpDataDeferimentoLPCO.Checked ? DtpDataDeferimentoLPCO.Value : null;
        }

        private void ConfigurarDatePickerNulavel(DateTimePicker dtp, DateTime? data)
        {
            dtp.ShowCheckBox = true;
            if (data.HasValue)
            {
                dtp.Checked = true;
                dtp.Value = data.Value;
                dtp.Format = DateTimePickerFormat.Short;
            }
            else
            {
                dtp.Checked = false;
                dtp.Value = DateTime.Today;
                dtp.Format = DateTimePickerFormat.Custom;
                dtp.CustomFormat = " ";
            }
            dtp.ValueChanged += (s, e) => {
                if (s is DateTimePicker picker) { picker.Format = picker.Checked ? DateTimePickerFormat.Short : DateTimePickerFormat.Custom; }
            };
        }
    }
}
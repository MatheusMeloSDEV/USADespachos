namespace CLUSA
{
    public class MAPA : OrgaoAnuente
    {
        public DateTime? InspecaoMapa { get; set; } = null;
        public bool CheckInspecaoMapa { get; set; } = false;

        public MAPA() { }

        public MAPA(Processo p)
        {
            Ref_USA = p.Ref_USA;
            Importador = p.Importador;
            SR = p.SR;
            Exportador = p.Exportador;
            Terminal = p.Terminal;
            Veiculo = p.Veiculo;
            Produto = p.Produto;
            Origem = p.Origem;
            DataDeAtracacao = p.DataDeAtracacao;
            CheckDataDeAtracacao = p.CheckDataDeAtracacao;
            DataEmbarque = p.DataEmbarque;
            CheckDataEmbarque = p.CheckDataEmbarque;
            Pendencia = p.Pendencia;
            StatusDoProcesso = p.StatusDoProcesso;
            LI = p.LI;
            Amostra = p.Amostra;
        }
    }

    public class DECEX : OrgaoAnuente
    {
        public DateTime? InspecaoDecex { get; set; } = null;
        public bool CheckInspecaoDecex { get; set; } = false;

        public DECEX() { }

        public DECEX(Processo p)
        {
            Ref_USA = p.Ref_USA;
            Importador = p.Importador;
            SR = p.SR;
            Exportador = p.Exportador;
            Terminal = p.Terminal;
            Veiculo = p.Veiculo;
            Produto = p.Produto;
            Origem = p.Origem;
            DataDeAtracacao = p.DataDeAtracacao;
            CheckDataDeAtracacao = p.CheckDataDeAtracacao;
            DataEmbarque = p.DataEmbarque;
            CheckDataEmbarque = p.CheckDataEmbarque;
            Pendencia = p.Pendencia;
            StatusDoProcesso = p.StatusDoProcesso;
            LI = p.LI;
            Amostra = p.Amostra;
        }
    }

    public class ANVISA : OrgaoAnuente
    {
        public DateTime? InspecaoAnvisa { get; set; } = null;
        public bool CheckInspecaoAnvisa { get; set; } = false;

        public ANVISA() { }

        public ANVISA(Processo p)
        {
            Ref_USA = p.Ref_USA;
            Importador = p.Importador;
            SR = p.SR;
            Exportador = p.Exportador;
            Terminal = p.Terminal;
            Veiculo = p.Veiculo;
            Produto = p.Produto;
            Origem = p.Origem;
            DataDeAtracacao = p.DataDeAtracacao;
            CheckDataDeAtracacao = p.CheckDataDeAtracacao;
            DataEmbarque = p.DataEmbarque;
            CheckDataEmbarque = p.CheckDataEmbarque;
            Pendencia = p.Pendencia;
            StatusDoProcesso = p.StatusDoProcesso;
            LI = p.LI;
            Amostra = p.Amostra;
        }
    }

    public class IBAMA : OrgaoAnuente
    {
        public DateTime? InspecaoIbama { get; set; } = null;
        public bool CheckInspecaoIbama { get; set; } = false;

        public IBAMA() { }

        public IBAMA(Processo p)
        {
            Ref_USA = p.Ref_USA;
            Importador = p.Importador;
            SR = p.SR;
            Exportador = p.Exportador;
            Terminal = p.Terminal;
            Veiculo = p.Veiculo;
            Produto = p.Produto;
            Origem = p.Origem;
            DataDeAtracacao = p.DataDeAtracacao;
            CheckDataDeAtracacao = p.CheckDataDeAtracacao;
            DataEmbarque = p.DataEmbarque;
            CheckDataEmbarque = p.CheckDataEmbarque;
            Pendencia = p.Pendencia;
            StatusDoProcesso = p.StatusDoProcesso;
            LI = p.LI;
            Amostra = p.Amostra;
        }
    }

    public class INMETRO : OrgaoAnuente
    {
        public DateTime? InspecaoInmetro { get; set; } = null;
        public bool CheckInspecaoInmetro { get; set; } = false;

        public INMETRO() { }

        public INMETRO(Processo p)
        {
            Ref_USA = p.Ref_USA;
            Importador = p.Importador;
            SR = p.SR;
            Exportador = p.Exportador;
            Terminal = p.Terminal;
            Veiculo = p.Veiculo;
            Produto = p.Produto;
            Origem = p.Origem;
            DataDeAtracacao = p.DataDeAtracacao;
            CheckDataDeAtracacao = p.CheckDataDeAtracacao;
            DataEmbarque = p.DataEmbarque;
            CheckDataEmbarque = p.CheckDataEmbarque;
            Pendencia = p.Pendencia;
            StatusDoProcesso = p.StatusDoProcesso;
            LI = p.LI;
            Amostra = p.Amostra;
        }
    }
}
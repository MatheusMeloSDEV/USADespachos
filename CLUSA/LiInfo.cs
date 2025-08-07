namespace CLUSA
{
    public class LiInfo
    {
        public string Numero { get; set; } = string.Empty;
        public List<string> OrgaosAnuentes { get; set; } = new List<string>();
        public string NCM { get; set; } = string.Empty;
        public string LPCO { get; set; } = string.Empty;
        public DateTime? DataRegistroLI { get; set; } = (DateTime?)null;
        public bool CheckDataRegistroLI { get; set; } = false;
        public DateTime? DataRegistroLPCO { get; set; } = (DateTime?)null;
        public bool CheckDataRegistroLPCO { get; set; } = false;
        public DateTime? DataDeferimentoLPCO { get; set; } = (DateTime?)null;
        public bool CheckDataDeferimentoLPCO { get; set; } = false;
        public string ParametrizacaoLPCO { get; set; } = string.Empty;

        public LiInfo(string numero, List<string> orgaos, string ncm, string lpco, DateTime? dataRegistroLI, bool checkDataRegistroLI, DateTime? dataRegistro, bool checkDataRegistro, DateTime? dataDeferimento, bool checkDataDeferimento, string parametrizacao)
        {
            Numero = numero;
            OrgaosAnuentes = orgaos;
            NCM = ncm;
            LPCO = lpco;
            DataRegistroLI = dataRegistroLI;
            CheckDataRegistroLI = checkDataRegistroLI;
            DataRegistroLPCO = dataRegistro;
            CheckDataRegistroLPCO = checkDataRegistro;
            DataDeferimentoLPCO = dataDeferimento;
            CheckDataDeferimentoLPCO = checkDataDeferimento;
            ParametrizacaoLPCO = parametrizacao;
        }
        public LiInfo() { }
    }
}

namespace CLUSA
{
    public class LiInfo
    {
        public string Numero { get; set; } = string.Empty;
        public string NCM { get; set; } = string.Empty;
        public DateTime? DataRegistroLI { get; set; }
        public bool CheckDataRegistroLI { get; set; }

        public List<LpcoInfo> LpcosPorOrgao { get; set; } = new List<LpcoInfo>();

        public LiInfo() { }

        public LiInfo(string numero, string ncm, DateTime? dataRegistroLI, bool checkDataRegistroLI, List<LpcoInfo> lpcos)
        {
            Numero = numero;
            NCM = ncm;
            DataRegistroLI = dataRegistroLI;
            CheckDataRegistroLI = checkDataRegistroLI;
            LpcosPorOrgao = lpcos ?? new List<LpcoInfo>();
        }
    }
}

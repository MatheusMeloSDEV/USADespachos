using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLUSA
{
    public class LpcoInfo
    {
        public string NomeOrgao { get; set; } = string.Empty;
        public string LPCO { get; set; } = string.Empty;
        public DateTime? DataRegistroLPCO { get; set; }
        public DateTime? DataDeferimentoLPCO { get; set; }
        public string ParametrizacaoLPCO { get; set; } = string.Empty;
    }
}
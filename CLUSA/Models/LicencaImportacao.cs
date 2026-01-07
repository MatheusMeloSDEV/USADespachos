using System;
using System.Collections.Generic;

namespace CLUSA.Models
{
    public class LicencaImportacao
    {
        public string Numero { get; set; } = string.Empty;
        public string NCM { get; set; } = string.Empty;
        public DateTime? DataRegistro { get; set; }
        public bool Amostra { get; set; } = false;
        public List<LpcoInfo> LPCO { get; set; } = new();
    }
}
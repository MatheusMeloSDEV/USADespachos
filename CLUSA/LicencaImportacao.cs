using CLUSA;

public class LicencaImportacao
{
    public string Numero { get; set; } = string.Empty;
    public string NCM { get; set; } = string.Empty;
    public DateTime? DataRegistro { get; set; }
    public bool Amostra { get; set; } = false;
    public string StatusLI { get; set; } = string.Empty;
    public List<LpcoInfo> LPCO { get; set; } = new();

}
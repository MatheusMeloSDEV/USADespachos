using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

public class DataHelper
{
    public static DateTime? CalcularVencimento(DateTime? dataBase, int dias)
    {
        if (!dataBase.HasValue)
        {
            return null;
        }

        return dataBase.Value.AddDays(dias);
    }

}

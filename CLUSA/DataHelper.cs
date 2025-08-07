public class DataHelper
{
    public static DateTime CalcularVencimento(DateTime? dataBase, int dias)
    {
        if (!dataBase.HasValue)
            throw new ArgumentException("A data base não pode ser nula.");

        return dataBase.Value.AddDays(dias);
    }
}
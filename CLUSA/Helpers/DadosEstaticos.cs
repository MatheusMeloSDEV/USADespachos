using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLUSA.Helpers
{
    public static class DadosEstaticos
    {
        // Retorna a lista bruta sempre que precisar
        public static List<(string Nome, string Cnpj)> ObterListaCNPJs()
        {
            return new List<(string Nome, string Cnpj)>
            {
                ("ACCIO", "48.583.422/0001-63"),
                ("ALICE ALIMENTOS", "39.304.199/0001-87"),
                ("ALICE ALIMENTOS", "39.304.199/0002-68"),
                ("AURORA", "83.310.441/0083-63"),
                ("BRASCOD", "05.399.489/0001-30"),
                ("CASA FLORA", "62.808.506/0007-74"),
                ("CASA FLORA", "62.808.506/0001-89"),
                ("COPY DATA", "01.208.994/0002-80"),
                ("DAMPER", "51.512.514/0001-67"),
                ("ELTO COMERCIAL", "20.277.795/0001-97"),
                ("FMG", "15.810.362/0001-15"),
                ("FREEWAY", "04.600.832/0003-61"),
                ("FREEWAY", "04.600.832/0002-80"),
                ("FREEWAY", "04.600.832/0001-08"),
                ("FREEWAY", "04.600.832/0004-42"),
                ("FRUGAL", "02.736.467/0003-91"),
                ("FRUGAL", "02.736.467/0002-00"),
                ("KUKAMAR", "09.606.174/0001-77"),
                ("LEITESOL", "65.979.973/0002-40"),
                ("LIBRA", "45.848.470/0001-48"),
                ("MARHUA", "48.950.432/0001-90"),
                ("MARCOL", "47.462.981/0001-52"),
                ("MARNOBRE", "18.861.087/0001-57"),
                ("MGA", "60.356.037/0001-89"),
                ("NOR IMPORT", "07.635.660/0001-98"),
                ("REBELA", "69.324.853/0001-85"),
                ("SEIKO", "45.865.824/0001-62"),
                ("VANUCCI", "30.037.571/0001-61"),
                ("VILA SIMPATIA", "07.722.158/0001-14"),
                ("ZARAGOZA", "05.868.574/0010-90"),
                ("ZARAGOZA", "05.868.574/0005-23")
            };
        }
    }
}

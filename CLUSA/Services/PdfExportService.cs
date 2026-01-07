using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Font;
using iText.IO.Font.Constants;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Layout.Borders;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections;

namespace CLUSA.Services
{
    public static class PdfExportService
    {
        public static void ExportarGridParaPdf(DataGridView dgv, string caminhoArquivo, string titulo, bool apenasSelecionadas = false)
        {
            // --- 1. SEPARAÇÃO DE COLUNAS ---
            var todasColunas = dgv.Columns.Cast<DataGridViewColumn>()
                .Where(c => c.Visible)
                .OrderBy(c => c.DisplayIndex)
                .ToList();

            if (todasColunas.Count == 0) throw new Exception("Não há colunas visíveis.");

            // Identifica colunas especiais
            var colHistorico = todasColunas.FirstOrDefault(c =>
                c.HeaderText.IndexOf("Histórico", StringComparison.OrdinalIgnoreCase) >= 0 ||
                c.HeaderText.IndexOf("Historico", StringComparison.OrdinalIgnoreCase) >= 0 ||
                c.DataPropertyName == "HistoricoDoProcesso");

            var colPendencia = todasColunas.FirstOrDefault(c =>
                c.HeaderText.IndexOf("Pendência", StringComparison.OrdinalIgnoreCase) >= 0 ||
                c.HeaderText.IndexOf("Pendencia", StringComparison.OrdinalIgnoreCase) >= 0 ||
                c.DataPropertyName.IndexOf("Pendencia", StringComparison.OrdinalIgnoreCase) >= 0);

            // Remove as especiais da contagem horizontal
            var colunasPadrao = todasColunas
                .Where(c => c != colHistorico && c != colPendencia)
                .ToList();

            if (colunasPadrao.Count == 0) throw new Exception("Nenhuma coluna padrão encontrada.");


            // --- LÓGICA DE FONTE DINÂMICA ---
            // Quanto mais colunas, menor a fonte para caber na folha A2
            int qtd = colunasPadrao.Count;
            float fontSizeHeader;
            float fontSizeData;

            if (qtd > 40) { fontSizeHeader = 9f; fontSizeData = 8f; } // Extremo
            else if (qtd > 30) { fontSizeHeader = 12f; fontSizeData = 11.5f; } // Seu caso (32 colunas)
            else if (qtd > 20) { fontSizeHeader = 14f; fontSizeData = 13.5f; } // Médio
            else if (qtd > 10) { fontSizeHeader = 16f; fontSizeData = 14f; } // Confortável
            else { fontSizeHeader = 18f; fontSizeData = 16f; } // Poucas colunas


            // --- 2. LINHAS ---
            List<DataGridViewRow> linhas;
            if (apenasSelecionadas && dgv.SelectedRows.Count > 0)
                linhas = dgv.SelectedRows.Cast<DataGridViewRow>().OrderBy(r => r.Index).ToList();
            else
                linhas = dgv.Rows.Cast<DataGridViewRow>().Where(r => !r.IsNewRow).OrderBy(r => r.Index).ToList();

            if (linhas.Count == 0) throw new Exception("Não há dados.");


            // --- 3. PDF SETUP (A2) ---
            using var writer = new PdfWriter(caminhoArquivo);
            using var pdf = new PdfDocument(writer);
            var document = new Document(pdf, PageSize.A1.Rotate());
            document.SetMargins(15, 10, 10, 10);

            var fontRegular = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
            var fontBold = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
            var fontOblique = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_OBLIQUE);

            // Título
            document.Add(new Paragraph(titulo).SetFont(fontBold).SetFontSize(18).SetTextAlignment(TextAlignment.CENTER));
            document.Add(new Paragraph($"Gerado em: {DateTime.Now:dd/MM/yyyy HH:mm} | {linhas.Count} Registros | {qtd} Colunas")
                .SetFontSize(10).SetTextAlignment(TextAlignment.CENTER));
            document.Add(new Paragraph("\n").SetFontSize(5));


            // --- 4. TABELA ---
            float[] larguras = colunasPadrao.Select(c => (float)c.Width).ToArray();
            var table = new Table(UnitValue.CreatePercentArray(larguras)).UseAllAvailableWidth();

            // Cabeçalhos
            foreach (var col in colunasPadrao)
            {
                var cell = new Cell().Add(new Paragraph(col.HeaderText)
                    .SetFont(fontBold)
                    .SetFontSize(fontSizeHeader)); // <-- Fonte Dinâmica

                cell.SetBackgroundColor(ColorConstants.LIGHT_GRAY);
                cell.SetPadding(1);
                cell.SetTextAlignment(TextAlignment.CENTER);
                table.AddHeaderCell(cell);
            }

            // Dados
            foreach (var row in linhas)
            {
                // A. Colunas Padrão
                foreach (var col in colunasPadrao)
                {
                    var cellValue = row.Cells[col.Index].Value;
                    string val = FormatarValorCelula(cellValue);

                    var cell = new Cell().Add(new Paragraph(val)
                        .SetFont(fontRegular)
                        .SetFontSize(fontSizeData)); // <-- Fonte Dinâmica

                    cell.SetPadding(0.5f);
                    cell.SetVerticalAlignment(VerticalAlignment.MIDDLE);

                    if (colHistorico != null || colPendencia != null)
                        cell.SetBorderBottom(new SolidBorder(ColorConstants.LIGHT_GRAY, 0.5f));

                    if (IsNumericOrDate(col.ValueType)) cell.SetTextAlignment(TextAlignment.CENTER);
                    else cell.SetTextAlignment(TextAlignment.LEFT);

                    table.AddCell(cell);
                }

                // B. Pendência
                if (colPendencia != null)
                {
                    AdicionarLinhaDetalhe(table, row, colPendencia, "PENDÊNCIA",
                        colunasPadrao.Count, new DeviceRgb(255, 240, 240),
                        fontBold, fontOblique, ColorConstants.RED,
                        fontSizeData); // Passamos a fonte para o detalhe também
                }

                // C. Histórico
                if (colHistorico != null)
                {
                    AdicionarLinhaDetalhe(table, row, colHistorico, "HISTÓRICO",
                        colunasPadrao.Count, new DeviceRgb(250, 250, 250),
                        fontBold, fontOblique, ColorConstants.DARK_GRAY,
                        fontSizeData);
                }
            }

            document.Add(table);
            document.Close();
        }

        // --- HELPER ATUALIZADO (Recebe fontSize) ---
        private static void AdicionarLinhaDetalhe(Table table, DataGridViewRow row, DataGridViewColumn col,
            string titulo, int colspan, Color bgColor, PdfFont fontTitulo, PdfFont fontTexto, Color corTitulo, float fontSize)
        {
            var valor = row.Cells[col.Index].Value;
            string texto = FormatarValorCelula(valor);

            var cell = new Cell(1, colspan);
            cell.SetPadding(2);
            cell.SetBackgroundColor(bgColor);
            cell.SetBorderBottom(new SolidBorder(ColorConstants.BLACK, 1f));

            var p = new Paragraph();

            // Título um pouquinho menor que o dado normal para ficar elegante
            var spanTitulo = new Text($"{titulo}: ")
                .SetFont(fontTitulo)
                .SetFontSize(fontSize)
                .SetFontColor(corTitulo);
            p.Add(spanTitulo);

            if (string.IsNullOrWhiteSpace(texto))
            {
                p.Add(new Text("-").SetFont(fontTexto).SetFontSize(fontSize).SetFontColor(ColorConstants.GRAY));
            }
            else
            {
                var linhasTexto = texto.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                bool primeiraLinha = true;
                foreach (var l in linhasTexto)
                {
                    if (!primeiraLinha) p.Add(new Text(" | ").SetFontColor(ColorConstants.LIGHT_GRAY));

                    p.Add(new Text(l.Trim())
                        .SetFont(fontTexto)
                        .SetFontSize(fontSize) // Usa a fonte dinâmica calculada
                        .SetFontColor(ColorConstants.BLACK));

                    primeiraLinha = false;
                }
            }

            cell.Add(p);
            cell.SetKeepTogether(true);
            table.AddCell(cell);
        }

        // --- FORMATTERS (IGUAL) ---
        private static string FormatarValorCelula(object value)
        {
            if (value == null) return "";
            if (value is bool b) return b ? "Sim" : "";
            if (value is DateTime d) return d.TimeOfDay.TotalSeconds == 0 ? d.ToString("dd/MM/yyyy") : d.ToString("dd/MM/yyyy HH:mm");

            if (value is IEnumerable lista && !(value is string))
            {
                var textos = new List<string>();
                foreach (var item in lista)
                {
                    if (item == null) continue;
                    var propNumero = item.GetType().GetProperty("Numero");
                    if (propNumero != null)
                    {
                        var valorNum = propNumero.GetValue(item);
                        if (valorNum != null) textos.Add(valorNum.ToString());
                    }
                    else textos.Add(item.ToString());
                }
                return string.Join(", ", textos);
            }
            return value.ToString();
        }

        private static bool IsNumericOrDate(Type t)
        {
            if (t == null) return false;
            return Type.GetTypeCode(t) == TypeCode.Int32 || Type.GetTypeCode(t) == TypeCode.Decimal ||
                   Type.GetTypeCode(t) == TypeCode.Double || Type.GetTypeCode(t) == TypeCode.DateTime;
        }
    }
}
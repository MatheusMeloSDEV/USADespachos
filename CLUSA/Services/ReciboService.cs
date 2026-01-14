using ClosedXML.Excel;
using MongoDB.Driver;
using System;
using System.IO;
using System.Threading.Tasks;
using CLUSA.Models;
using CLUSA.Repositories;

// iText7
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Font;
using iText.IO.Font.Constants;
using iText.Kernel.Colors;
using iText.IO.Image;
using iText.Layout.Borders;
using iText.Kernel.Geom;

namespace CLUSA.Services
{
    public class ReciboService
    {
        private const string Colecao = "Recibo";
        private readonly string _pastaDestino = @"C:\UsaDespachos\Docs\Recibos";
        private readonly string _caminhoLogo = @"C:\UsaDespachos\Logos\ReciboLogo.png";

        public ReciboService()
        {
            if (!Directory.Exists(_pastaDestino)) Directory.CreateDirectory(_pastaDestino);
        }

        public async Task<string> GerarReciboAsync(string refUsa)
        {
            var recibo = await BuscarReciboAsync(refUsa);

            string refFormatada = refUsa.Replace("/", "-").Replace("\\", "-");
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string nomeBase = $"Recibo_{refFormatada}_{timestamp}";

            // 1. Gera Excel
            string caminhoExcel = System.IO.Path.Combine(_pastaDestino, $"{nomeBase}.xlsx");
            GerarExcel(caminhoExcel, recibo);

            // 2. Gera PDF
            string caminhoPdf = System.IO.Path.Combine(_pastaDestino, $"{nomeBase}.pdf");
            GerarPdf(caminhoPdf, recibo);

            return caminhoPdf;
        }

        private async Task<Recibo> BuscarReciboAsync(string refUsa)
        {
            var db = ConfigDatabase.GetDatabase();
            var collection = db.GetCollection<Recibo>(Colecao);
            var filtro = Builders<Recibo>.Filter.Eq(r => r.Ref_USA, refUsa.Trim());
            var recibo = await collection.Find(filtro).FirstOrDefaultAsync();

            if (recibo == null) throw new Exception($"Recibo para '{refUsa}' não encontrado.");
            return recibo;
        }

        #region Geração Excel (ClosedXML) - Mantido Igual
        private void GerarExcel(string caminho, Recibo r)
        {
            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Recibo");

            ws.PageSetup.PageOrientation = XLPageOrientation.Portrait;
            ws.PageSetup.FitToPages(1, 1);
            ws.PageSetup.Margins.SetLeft(0.1).SetRight(0.1).SetTop(0.3).SetBottom(0.3);

            ws.Column(1).Width = 20; ws.Column(2).Width = 18; ws.Column(3).Width = 35; ws.Column(4).Width = 18; ws.Column(5).Width = 20;

            for (int i = 1; i <= 35; i++) ws.Row(i).Height = 20;
            ws.Row(1).Height = 30; ws.Row(2).Height = 30; ws.Row(3).Height = 42;

            if (File.Exists(_caminhoLogo))
            {
                var pic = ws.AddPicture(_caminhoLogo).MoveTo(ws.Cell("A1"));
                pic.Width = 220; pic.Height = 75;
            }

            ws.Range("A1:E1").Merge(); ws.Range("A2:E2").Merge(); ws.Range("A3:E3").Merge();
            ws.Range("A4:E4").Merge(); ws.Range("A5:E5").Merge(); ws.Range("A6:E6").Merge();
            ws.Range("A8:C8").Merge(); ws.Range("D8:E8").Merge();
            ws.Range("A9:C9").Merge(); ws.Range("D9:E9").Merge();
            ws.Range("A10:C10").Merge(); ws.Range("D10:E10").Merge();
            ws.Range("D12:E13").Merge();
            ws.Range("B16:D16").Merge();
            ws.Range("C17:D17").Merge(); ws.Range("C18:D18").Merge();
            ws.Range("C19:D19").Merge(); ws.Range("C20:D20").Merge(); ws.Range("C21:D21").Merge();
            ws.Range("B25:D25").Merge();
            ws.Range("A31:E31").Merge(); ws.Range("A32:E32").Merge();
            ws.Range("A34:E34").Merge(); ws.Range("A35:E35").Merge();

            SetVal(ws, "A1", "U.S.A", "Times New Roman", 22, false, XLAlignmentHorizontalValues.Center);
            SetVal(ws, "A2", "Despachos Aduaneiros Ltda.", "Times New Roman", 22, false, XLAlignmentHorizontalValues.Center);
            SetVal(ws, "A3", "Recibo", "Arial", 30, true, XLAlignmentHorizontalValues.Center);
            SetVal(ws, "A4", $"Recebemos da Empresa {r.Importador}", "Arial", 14, true, XLAlignmentHorizontalValues.Center);
            SetVal(ws, "A5", $"Endereço: {r.Endereco_Importador}", "Arial", 14, true, XLAlignmentHorizontalValues.Center);
            SetVal(ws, "A6", "Os valores referentes as despesas abaixo mencionadas", "Arial", 12, false, XLAlignmentHorizontalValues.Center);

            foreach (var cell in ws.Row(7).Cells(1, 5)) cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#D9D9D9");

            SetMoneyRow(ws, 8, "Emissão Licença", r.EmissaoLicenca);
            SetMoneyRow(ws, 9, "Expediente", r.Expediente);
            SetMoneyRow(ws, 10, "Honorários Despachante", r.HonorariosDespachante);

            var cellTotal = ws.Cell("D12");
            cellTotal.Value = r.Total;
            cellTotal.Style.NumberFormat.Format = "\"R$\"#,##0.00";
            cellTotal.Style.Font.FontName = "Arial";
            cellTotal.Style.Font.FontSize = 16;
            cellTotal.Style.Font.Bold = true;
            cellTotal.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            cellTotal.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("B16").Value = "Referente ao processo:";
            ws.Cell("B16").Style.Font.FontName = "Arial";
            ws.Cell("B16").Style.Font.FontSize = 16;
            ws.Cell("B16").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            SetDetailRow(ws, 17, "Ref. U.S.A", r.Ref_USA);
            SetDetailRow(ws, 18, "SR", r.SR);
            SetDetailRow(ws, 19, "Veiculo", r.Veiculo);
            SetDetailRow(ws, 20, "Exportador", r.Exportador);
            SetDetailRow(ws, 21, "Mercadoria", r.Mercadoria);

            string txtData = !string.IsNullOrWhiteSpace(r.Datahoje) ? r.Datahoje : DateTime.Now.ToString("dd 'de' MMMM 'de' yyyy", new System.Globalization.CultureInfo("pt-BR"));
            ws.Cell("B25").Value = $"Santos, {txtData}";
            ws.Cell("B25").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("B25").Style.Font.FontName = "Arial";
            ws.Cell("B25").Style.Font.FontSize = 16;

            ws.Range("B28:D28").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

            SetFooter(ws, 31, "Matriz: Rua Comendador Martins nº 55 Altos - Sala 22 - Vila Mathias - CEP 11015-530 - Santos - S.P.");
            SetFooter(ws, 32, " Fone: (13)3222.8899 - 2202.8369  - e-mail: josecarlos@usadespachos.com.br - usa@bignet.com.br ");
            foreach (var cell in ws.Row(33).Cells(1, 5)) cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#D9D9D9");
            SetFooter(ws, 34, "Filial: Rua Manoel Dono Morgado nº 100 -  CEP 88301-462 - Fazenda – Itajaí - S.C.");
            SetFooter(ws, 35, "Fone: (47)3045.1439 - 3083.1430  - e-mail: nestor@usadespachos.com.br");

            AplicarBorda(ws, 1, 2, 1, 5);
            AplicarBorda(ws, 3, 3, 1, 5, XLBorderStyleValues.Thick);
            AplicarBorda(ws, 8, 10, 1, 5);
            AplicarBorda(ws, 12, 13, 4, 5, XLBorderStyleValues.Thick);
            AplicarBorda(ws, 31, 35, 1, 5, XLBorderStyleValues.Thick);
            AplicarBorda(ws, 1, 35, 1, 5, XLBorderStyleValues.Thick);

            ws.Range("B16:D16").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            ws.Range("B16:D21").Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
            for (int i = 17; i <= 21; i++) ws.Range(i, 2, i, 4).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            wb.SaveAs(caminho);
        }

        private void SetVal(IXLWorksheet ws, string cell, string val, string font, double size, bool bold, XLAlignmentHorizontalValues align)
        {
            ws.Cell(cell).Value = val;
            ws.Cell(cell).Style.Font.FontName = font;
            ws.Cell(cell).Style.Font.FontSize = size;
            ws.Cell(cell).Style.Font.Bold = bold;
            ws.Cell(cell).Style.Alignment.Horizontal = align;
            ws.Cell(cell).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        }
        private void SetMoneyRow(IXLWorksheet ws, int row, string label, decimal val)
        {
            ws.Cell(row, 1).Value = label;
            ws.Cell(row, 1).Style.Font.FontName = "Arial";
            ws.Cell(row, 1).Style.Font.FontSize = 16;
            var cellVal = ws.Cell(row, 4);
            cellVal.Value = val;
            cellVal.Style.NumberFormat.Format = "\"R$\"#,##0.00";
            cellVal.Style.Font.FontName = "Arial";
            cellVal.Style.Font.FontSize = 16;
            cellVal.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
        }
        private void SetDetailRow(IXLWorksheet ws, int row, string label, string val)
        {
            ws.Cell(row, 2).Value = label;
            ws.Cell(row, 2).Style.Font.FontName = "Arial";
            ws.Cell(row, 2).Style.Font.FontSize = 16;
            ws.Cell(row, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            var cellVal = ws.Cell(row, 3);
            cellVal.Value = val;
            cellVal.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            cellVal.Style.Alignment.ShrinkToFit = true;
            double size = 16;
            if ((val?.Length ?? 0) > 20) size = 12;
            if ((val?.Length ?? 0) > 30) size = 10;
            cellVal.Style.Font.FontName = "Arial";
            cellVal.Style.Font.FontSize = size;
        }
        private void SetFooter(IXLWorksheet ws, int row, string text)
        {
            ws.Cell(row, 1).Value = text;
            ws.Cell(row, 1).Style.Font.FontName = "Arial";
            ws.Cell(row, 1).Style.Font.FontSize = 12;
            ws.Cell(row, 1).Style.Font.Bold = true;
            ws.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        }
        private void AplicarBorda(IXLWorksheet ws, int r1, int r2, int c1, int c2, XLBorderStyleValues style = XLBorderStyleValues.Thin)
        {
            ws.Range(r1, c1, r2, c2).Style.Border.OutsideBorder = style;
            if (style == XLBorderStyleValues.Thin) ws.Range(r1, c1, r2, c2).Style.Border.InsideBorder = style;
        }
        #endregion

        #region Geração PDF (iText7) - Atualizado para MiddleCenter e Bordas

        private void GerarPdf(string caminho, Recibo r)
        {
            using var writer = new PdfWriter(caminho);
            using var pdf = new PdfDocument(writer);
            var document = new Document(pdf, PageSize.A4);
            document.SetMargins(20, 20, 20, 20);

            var fontTimes = PdfFontFactory.CreateFont(StandardFonts.TIMES_ROMAN);
            var fontArial = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
            var fontArialBold = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

            // Tabela Mestra (5 colunas)
            float[] widths = { 20, 18, 35, 18, 20 };
            var table = new Table(UnitValue.CreatePercentArray(widths)).UseAllAvailableWidth();

            // --- CABEÇALHO ---
            var cellLogo = new Cell(2, 5).SetBorder(Border.NO_BORDER).SetPadding(5);
            if (File.Exists(_caminhoLogo))
            {
                try
                {
                    ImageData imgData = ImageDataFactory.Create(_caminhoLogo);
                    Image img = new Image(imgData).ScaleToFit(180, 70);
                    cellLogo.Add(img.SetHorizontalAlignment(HorizontalAlignment.CENTER));
                }
                catch { }
            }
            table.AddCell(cellLogo);

            AddCellCenter(table, "U.S.A", fontTimes, 22, 5);
            AddCellCenter(table, "Despachos Aduaneiros Ltda.", fontTimes, 22, 5);

            // Recibo
            var cellRecibo = new Cell(1, 5).Add(new Paragraph("Recibo").SetFont(fontArialBold).SetFontSize(30))
                .SetTextAlignment(TextAlignment.CENTER)
                .SetBorder(new SolidBorder(ColorConstants.BLACK, 2));
            table.AddCell(cellRecibo);

            // Dados Cliente
            AddCellCenter(table, $"Recebemos da Empresa {r.Importador}", fontArialBold, 14, 5);
            AddCellCenter(table, $"Endereço: {r.Endereco_Importador}", fontArialBold, 14, 5);
            AddCellCenter(table, "Os valores referentes as despesas abaixo mencionadas", fontArial, 12, 5);

            // Barra Cinza
            table.AddCell(new Cell(1, 5).SetHeight(10).SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetBorder(Border.NO_BORDER));

            // --- VALORES ---
            AddRowMoney(table, "Emissão Licença", r.EmissaoLicenca, fontArial);
            AddRowMoney(table, "Expediente", r.Expediente, fontArial);
            AddRowMoney(table, "Honorários Despachante", r.HonorariosDespachante, fontArial);

            // Total
            table.AddCell(new Cell(1, 3).SetBorder(Border.NO_BORDER));
            var cellTotal = new Cell(2, 2)
                .Add(new Paragraph($"R$ {r.Total:N2}").SetFont(fontArialBold).SetFontSize(16))
                .SetTextAlignment(TextAlignment.RIGHT)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                .SetBorder(new SolidBorder(ColorConstants.BLACK, 2));
            table.AddCell(cellTotal);

            // Espaço
            table.AddCell(new Cell(1, 5).SetHeight(20).SetBorder(Border.NO_BORDER));

            // --- DETALHES (TABELA ESTILIZADA IGUAL A IMAGEM) ---
            var subTable = new Table(UnitValue.CreatePercentArray(new float[] { 30, 70 })).UseAllAvailableWidth();

            // Título da Tabela com Borda
            subTable.AddHeaderCell(new Cell(1, 2)
                .Add(new Paragraph("Referente ao processo:").SetFont(fontArial).SetFontSize(16))
                .SetTextAlignment(TextAlignment.CENTER)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                .SetBorder(new SolidBorder(ColorConstants.BLACK, 1))); // Borda preta simples

            // Linhas com Grade
            AddDetail(subTable, "Ref. U.S.A", r.Ref_USA, fontArial);
            AddDetail(subTable, "SR", r.SR, fontArial);
            AddDetail(subTable, "Veiculo", r.Veiculo, fontArial);
            AddDetail(subTable, "Exportador", r.Exportador, fontArial);
            AddDetail(subTable, "Mercadoria", r.Mercadoria, fontArial);

            // Adiciona a sub-tabela ao PDF
            var cellDetails = new Cell(1, 5).Add(subTable).SetPadding(10).SetBorder(Border.NO_BORDER);
            table.AddCell(cellDetails);

            // Data e Assinatura
            string txtData = !string.IsNullOrWhiteSpace(r.Datahoje) ? r.Datahoje : DateTime.Now.ToString("dd 'de' MMMM 'de' yyyy", new System.Globalization.CultureInfo("pt-BR"));
            table.AddCell(new Cell(1, 5).Add(new Paragraph($"Santos, {txtData}").SetFont(fontArial).SetFontSize(16))
                .SetTextAlignment(TextAlignment.CENTER).SetPaddingTop(20).SetBorder(Border.NO_BORDER));

            table.AddCell(new Cell(1, 5).Add(new Paragraph("__________________________________________"))
                .SetTextAlignment(TextAlignment.CENTER).SetPaddingBottom(20).SetBorder(Border.NO_BORDER));

            // --- RODAPÉ ---
            var cellFooter = new Cell(1, 5).SetBorder(new SolidBorder(ColorConstants.BLACK, 2)).SetPadding(0);

            cellFooter.Add(new Paragraph("Matriz: Rua Comendador Martins nº 55 Altos - Sala 22 - Vila Mathias - CEP 11015-530 - Santos - S.P.")
                .SetFont(fontArialBold).SetFontSize(10).SetTextAlignment(TextAlignment.CENTER));

            cellFooter.Add(new Paragraph(" Fone: (13)3222.8899 - 2202.8369  - e-mail: josecarlos@usadespachos.com.br - usa@bignet.com.br ")
                .SetFont(fontArialBold).SetFontSize(10).SetTextAlignment(TextAlignment.CENTER));

            cellFooter.Add(new Div().SetHeight(5).SetBackgroundColor(ColorConstants.LIGHT_GRAY));

            cellFooter.Add(new Paragraph("Filial: Rua Manoel Dono Morgado nº 100 -  CEP 88301-462 - Fazenda – Itajaí - S.C.")
                .SetFont(fontArialBold).SetFontSize(10).SetTextAlignment(TextAlignment.CENTER));

            cellFooter.Add(new Paragraph("Fone: (47)3045.1439 - 3083.1430  - e-mail: nestor@usadespachos.com.br")
                .SetFont(fontArialBold).SetFontSize(10).SetTextAlignment(TextAlignment.CENTER));

            table.AddCell(cellFooter);

            document.Add(table);
            document.Close();
        }

        // Helpers PDF
        private void AddCellCenter(Table t, string txt, PdfFont font, float size, int colspan)
        {
            t.AddCell(new Cell(1, colspan).Add(new Paragraph(txt).SetFont(font).SetFontSize(size))
                .SetTextAlignment(TextAlignment.CENTER).SetBorder(Border.NO_BORDER));
        }

        private void AddRowMoney(Table t, string label, decimal val, PdfFont font)
        {
            t.AddCell(new Cell(1, 3).Add(new Paragraph(label).SetFont(font).SetFontSize(16))
                .SetBorder(new SolidBorder(ColorConstants.BLACK, 0.5f)));

            t.AddCell(new Cell(1, 2).Add(new Paragraph($"R$ {val:N2}").SetFont(font).SetFontSize(16))
                .SetTextAlignment(TextAlignment.RIGHT).SetBorder(new SolidBorder(ColorConstants.BLACK, 0.5f)));
        }

        // Helper Atualizado: Borda na grade + MiddleCenter
        private void AddDetail(Table t, string label, string val, PdfFont font)
        {
            // Coluna 1 (Label): Alinhado à Esquerda mas Verticalmente ao Centro, com borda
            t.AddCell(new Cell()
                .Add(new Paragraph(label).SetFont(font).SetFontSize(16))
                .SetTextAlignment(TextAlignment.LEFT)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                .SetPaddingLeft(5)
                .SetBorder(new SolidBorder(ColorConstants.BLACK, 1)));

            // Ajuste de fonte
            float size = 16;
            if ((val?.Length ?? 0) > 20) size = 12;
            if ((val?.Length ?? 0) > 30) size = 10;

            // Coluna 2 (Valor): Middle Center, com borda
            t.AddCell(new Cell()
                .Add(new Paragraph(val ?? "").SetFont(font).SetFontSize(size))
                .SetTextAlignment(TextAlignment.CENTER)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                .SetBorder(new SolidBorder(ColorConstants.BLACK, 1)));
        }

        #endregion
    }
}
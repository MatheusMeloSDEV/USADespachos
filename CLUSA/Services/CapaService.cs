using ClosedXML.Excel;
using CLUSA.Models;
using CLUSA.Repositories;
using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CLUSA.Services
{
    public class CapaService
    {
        private const string Colecao = "PROCESSO";
        private readonly string _pastaDestino = @"C:\UsaDespachos\Docs\Capa";
        private readonly string _caminhoLogo = @"C:\UsaDespachos\Exportador\logo.png";

        public CapaService()
        {
            if (!Directory.Exists(_pastaDestino))
                Directory.CreateDirectory(_pastaDestino);
        }

        public async Task<string> GerarCapaAsync(string refUsa)
        {
            var processo = await BuscarProcessoAsync(refUsa);

            string refFormatada = refUsa.Replace("/", "-").Replace("\\", "-");
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string nomeBase = $"Capa_{refFormatada}_{timestamp}";

            // 1. Gera Excel
            string caminhoExcel = System.IO.Path.Combine(_pastaDestino, $"{nomeBase}.xlsx");
            GerarExcel(caminhoExcel, processo);

            // 2. Gera PDF
            string caminhoPdf = System.IO.Path.Combine(_pastaDestino, $"{nomeBase}.pdf");
            GerarPdf(caminhoPdf, processo);

            return caminhoPdf;
        }

        private async Task<Processo> BuscarProcessoAsync(string refUsa)
        {
            var db = ConfigDatabase.GetDatabase();
            var collection = db.GetCollection<Processo>(Colecao);
            var filtro = Builders<Processo>.Filter.Eq(p => p.Ref_USA, refUsa);
            var processo = await collection.Find(filtro).FirstOrDefaultAsync();

            if (processo == null) throw new Exception($"Processo '{refUsa}' não encontrado.");

            processo.Capa ??= new Capa();
            processo.LI ??= new List<LicencaImportacao>();
            return processo;
        }

        #region Geração Excel (ClosedXML)

        private void GerarExcel(string caminho, Processo p)
        {
            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Capa");

            // Configuração Pagina
            ws.PageSetup.PageOrientation = XLPageOrientation.Portrait;
            ws.PageSetup.FitToPages(1, 1);
            ws.PageSetup.Margins.SetLeft(0.25).SetRight(0.25).SetTop(0.3).SetBottom(0.3);

            // Larguras (A=1 ... H=8)
            ws.Column(1).Width = 26; // A
            ws.Column(2).Width = 45; // B
            ws.Column(3).Width = 12; // C
            ws.Column(4).Width = 25; // D
            ws.Column(5).Width = 40; // E
            ws.Column(6).Width = 10; // F
            ws.Column(7).Width = 30; // G
            ws.Column(8).Width = 50; // H

            // Cabeçalho e Logo
            ws.Range("A1:A2").Merge();
            if (File.Exists(_caminhoLogo))
            {
                try
                {
                    var pic = ws.AddPicture(_caminhoLogo).MoveTo(ws.Cell(1, 1));
                    pic.Width = 180; pic.Height = 80;
                }
                catch { }
            }

            var titleRange = ws.Range("B1:H1"); titleRange.Merge().Value = "U.S.A";
            var subTitleRange = ws.Range("B2:H2"); subTitleRange.Merge().Value = "Despachos Aduaneiros Ltda.";

            var headerStyle = ws.Range("B1:H2").Style;
            headerStyle.Font.FontName = "Times New Roman";
            headerStyle.Font.FontSize = 28;
            headerStyle.Font.Bold = true;
            headerStyle.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            headerStyle.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            // Bordas Cabeçalho
            var headerBox = ws.Range("A1:H2");
            headerBox.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            ws.Cell("A1").Style.Border.RightBorder = XLBorderStyleValues.Thin;
            titleRange.Style.Border.BottomBorder = XLBorderStyleValues.None;
            subTitleRange.Style.Border.TopBorder = XLBorderStyleValues.None;

            // Linha Cinza
            ws.Range("A3:H3").Merge().Style.Fill.BackgroundColor = XLColor.FromHtml("#D0D0D0");
            ws.Range("A3:H3").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            // --- 4. DADOS PRINCIPAIS ---
            int row = 4;

            // Linha 4
            SetVal(ws, row, 1, "N/REF:", p.Ref_USA);
            SetVal(ws, row, 3, "S/REF:", p.SR, 2);
            SetVal(ws, row, 6, "EXP:", p.Exportador, 2);

            // Linha 5
            row++;
            SetVal(ws, row, 1, "Cliente", p.Importador);
            SetVal(ws, row, 3, "Procedência", p.Origem, 2);
            SetVal(ws, row, 6, "Embarcação", p.Veiculo, 2);

            // Linha 6
            row++;
            SetVal(ws, row, 1, "Produto", p.Produto, 4);
            SetVal(ws, row, 6, "Chegada", DataStr(p.DataDeAtracacao), 2);

            // Linha 7
            row++;
            string ncms = string.Join("; ", p.LI.Select(l => l.NCM).Distinct());
            SetVal(ws, row, 1, "NCM", ncms, 4);
            SetVal(ws, row, 6, "Armador", p.Armador);

            // Free Time (H)
            ws.Cell(row, 8).Value = $"Free Time: {p.FreeTime}";
            ws.Cell(row, 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell(row, 8).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            // --- MUDANÇA DE ORDEM AQUI (LINHA 8) ---
            row++;
            // 1. Master: Col 1-2 (Label A, Valor B)
            SetVal(ws, row, 1, "Master", p.Capa.Master, 0);

            // 2. Conhecimento: Col 3-6 (Label C, Valor D-F) -> MergeExtra 2 para pegar D,E,F
            SetVal(ws, row, 3, "Conhecimento", p.Conhecimento, 2);

            // 3. Container: Col 7-8 (Label G, Valor H)
            SetVal(ws, row, 7, "Container", p.Capa.Container, 0);

            // Linha 9: Terminal e Redestinado
            row++;
            ws.Cell(row, 1).Value = "Terminal";
            ws.Cell(row, 1).Style.Font.Bold = true;
            ws.Range(row, 2, row, 4).Merge().Value = p.Terminal;
             
            string txtRedestinado = $"{(p.Capa.DAT_IIDeferida ? "[ X ]" : "[   ]")} REDESTINADO - DATA";
            ws.Range(row, 5, row, 8).Merge().Value = txtRedestinado;

            // --- 5. LIs (MODIFICADO) ---
            row++;
            if (p.LI.Any())
            {
                foreach (var li in p.LI)
                {
                    int count = li.LPCO.Count > 0 ? li.LPCO.Count : 1;

                    var rangeLi = ws.Range(row, 3, row + count - 1, 3);
                    rangeLi.Merge().Value = $"LI nº {li.Numero}";
                    rangeLi.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    rangeLi.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    if (li.LPCO.Count == 0)
                    {
                        ws.Range(row, 4, row, 8).Merge().Value = "Sem LPCOs associados";
                        ws.Range(row, 4, row, 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        row++;
                    }
                    else
                    {
                        foreach (var lpco in li.LPCO)
                        {
                            ws.Range(row, 4, row, 5).Merge().Value = $"LPCO nº {lpco.LPCO}";
                            ws.Range(row, 4, row, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                            ws.Range(row, 7, row, 8).Merge().Value = lpco.NomeOrgao;
                            ws.Range(row, 7, row, 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                            row++;
                        }
                    }
                }
            }

            // SIGVIG
            ws.Cell(row, 1).Value = "SIGVIG"; ws.Cell(row, 1).Style.Font.Bold = true;
            ws.Range(row, 2, row, 3).Merge().Value = $"{Check(p.Capa.SigvigSelecionado)} Selecionado";
            ws.Range(row, 4, row, 8).Merge().Value = $"{Check(p.Capa.SigvigLiberado)} Liberado : {DataStr(p.Capa.SigvigData)}";
            row++;

            // Incoterm
            ws.Cell(row, 1).Value = "Incoterm"; ws.Cell(row, 1).Style.Font.Bold = true;
            ws.Range(row, 2, row, 4).Merge().Value = p.Capa.Incoterm;
            ws.Range(row, 5, row, 8).Merge().Value = "[   ] MOEDA/PESO/VALOR DE ACORDO COM DOCS.";
            row++;

            // Numerario
            var opsNum = new[] { "Prestação Serviço", "Agência", "Tributos", "Completo", "Complementar" };
            var selsNum = p.Capa.Numerario?.ToList() ?? new List<string>();
            string txtNum = string.Join("  ", opsNum.Select(o => $"{(selsNum.Contains(o) ? "[ X ]" : "[   ]")} {o}"));
            ws.Cell(row, 1).Value = "Numerário"; ws.Cell(row, 1).Style.Font.Bold = true;
            ws.Range(row, 2, row, 8).Merge().Value = txtNum;
            row++;

            SetVal(ws, row, 1, "DTA", p.Capa.DTA, 7); row++;
            SetVal(ws, row, 1, "DI", p.DI, 7); row++;

            ws.Cell(row, 1).Value = "Marinha"; ws.Cell(row, 1).Style.Font.Bold = true;
            ws.Range(row, 2, row, 4).Merge().Value = p.Capa.Marinha;
            ws.Range(row, 5, row, 8).Merge().Value = $"CE: {p.Capa.CE}";
            row++;

            var opsImp = new[] { "I.I", "I.P.I.", "PIS/PASEP", "COFINS", "ICMS" };
            var selsImp = p.Capa.Imposto?.ToList() ?? new List<string>(); 
            string txtImp = string.Join("  ", opsImp.Select(o => $"{(selsImp.Contains(o) ? "[ X ]" : "[   ]")} {o}"));
            ws.Cell(row, 1).Value = "Imposto"; ws.Cell(row, 1).Style.Font.Bold = true;
            ws.Range(row, 2, row, 8).Merge().Value = txtImp;
            row++;

            ws.Cell(row, 1).Value = "Status do Processo"; ws.Cell(row, 1).Style.Font.Bold = true;
            ws.Cell(row, 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#D0D0D0"); ws.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Range(row, 2, row, 8).Merge().Value = p.HistoricoDoProcesso ?? ""; ws.Cell(row, 2).Style.Alignment.WrapText = true;
            row++;

            ws.Cell(row, 1).Value = "Observações"; ws.Cell(row, 1).Style.Font.Bold = true;
            ws.Cell(row, 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#D0D0D0"); ws.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Range(row, 2, row, 8).Merge().Value = p.Capa.Observacoes; ws.Cell(row, 2).Style.Alignment.WrapText = true;
            row++;

            // Checklist
            ws.Cell(row, 1).Value = $"{Check(p.Capa.TelaDoCanal)} Tela do Canal"; ws.Range(row, 1, row, 3).Merge();
            string dataAv = p.Capa.AverbarData.HasValue ? DataStr(p.Capa.AverbarData) : "        /       /";
            ws.Cell(row, 4).Value = $"{Check(p.Capa.Averbar)} AVERBAR\nDATA: {dataAv}"; ws.Cell(row, 4).Style.Alignment.WrapText = true; ws.Range(row, 4, row, 6).Merge();
            string dataLib = p.Capa.LiberarBLData.HasValue ? DataStr(p.Capa.LiberarBLData) : "        /       /";
            ws.Cell(row, 7).Value = $"{Check(p.Capa.LiberarBL)} LIBERAR B/L\nDATA: {dataLib}"; ws.Cell(row, 7).Style.Alignment.WrapText = true; ws.Range(row, 7, row, 8).Merge();
            row++;

            string dataMar = p.Capa.MarinhaMercante_IsencaoData.HasValue ? DataStr(p.Capa.MarinhaMercante_IsencaoData) : "        /       /";
            ws.Cell(row, 1).Value = $"{Check(p.Capa.MarinhaMercante_Isencao)} MARINHA MERCANTE/ISENÇÃO\nDATA: {dataMar}"; ws.Cell(row, 1).Style.Alignment.WrapText = true; ws.Range(row, 1, row, 3).Merge();
            string dataIcms = p.Capa.ICMS_ExoneracaoData.HasValue ? DataStr(p.Capa.ICMS_ExoneracaoData) : "        /       /";
            ws.Cell(row, 4).Value = $"{Check(p.Capa.ICMS_Exoneracao)} I.C.M.S. OU EXONERAÇÃO\nDATA: {dataIcms}"; ws.Cell(row, 4).Style.Alignment.WrapText = true; ws.Range(row, 4, row, 6).Merge();
            ws.Cell(row, 7).Value = $"{Check(p.Capa.Lancado)} Lançado"; ws.Range(row, 7, row, 8).Merge();
            row++;

            ws.Cell(row, 1).Value = $"{Check(p.Capa.ConsultaSEFAZ)} Consulta SEFAZ"; ws.Range(row, 1, row, 3).Merge();
            ws.Cell(row, 4).Value = $"{Check(p.Capa.DAT_IIDeferida)} DAT II Deferida"; ws.Range(row, 4, row, 6).Merge();
            string dataSis = p.Capa.SISCargaLiberadoData.HasValue ? DataStr(p.Capa.SISCargaLiberadoData) : "        /       /";
            ws.Cell(row, 7).Value = $"{Check(p.Capa.SISCargaLiberado)} SISCARGA LIBERADO\nDATA: {dataSis}"; ws.Cell(row, 7).Style.Alignment.WrapText = true; ws.Range(row, 7, row, 8).Merge();
            row++;

            ws.Cell(row, 1).Value = $"{Check(p.Capa.DANFE)} DANFE"; ws.Range(row, 1, row, 3).Merge();
            ws.Cell(row, 4).Value = $"{Check(p.Capa.Armazenagem)} Armazenagem - {Check(p.Capa.Faturado)} Faturado\nPago por: {p.Capa.PagoPor}"; ws.Cell(row, 4).Style.Alignment.WrapText = true; ws.Range(row, 4, row, 6).Merge();
            string txtEntT = $"{Check(p.Capa.ENTTransporte)} ENT Transporte Nº {p.Capa.ENTTransporteN}"; if (p.Capa.ENTTransporteData != null) txtEntT += $" - {DataStr(p.Capa.ENTTransporteData)}";
            ws.Cell(row, 7).Value = txtEntT; ws.Range(row, 7, row, 8).Merge();
            row++;

            string txtEntA = $"{Check(p.Capa.ENTAlfandega)} ENT Alfândega Dossiê {p.Capa.ENTAlfandegaDossie}"; if (p.Capa.ENTAlfandegaData != null) txtEntA += $" - {DataStr(p.Capa.ENTAlfandegaData)}";
            ws.Cell(row, 1).Value = txtEntA; ws.Range(row, 1, row, 3).Merge();
            string dataConf = p.Capa.ConferenciaFisicaData.HasValue ? DataStr(p.Capa.ConferenciaFisicaData) : "        /       /";
            ws.Cell(row, 4).Value = $"{Check(p.Capa.ConferenciaFisica)} CONFERENCIA FÍSICA\nDATA: {dataConf}"; ws.Cell(row, 4).Style.Alignment.WrapText = true; ws.Range(row, 4, row, 6).Merge();

            // Bordas finais
            var rangeTotal = ws.Range(4, 1, row, 8);
            rangeTotal.Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
            rangeTotal.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            var allCells = ws.Range(1, 1, row, 8);
            allCells.Style.Font.FontName = "Aptos Narrow";
            allCells.Style.Font.FontSize = 12;
            ws.Rows(1, row).AdjustToContents();

            wb.SaveAs(caminho);
        }

        private void SetVal(IXLWorksheet ws, int r, int c, string label, string val, int mergeExtra = 0)
        {
            ws.Cell(r, c).Value = label;
            ws.Cell(r, c).Style.Font.Bold = true;
            var valCell = ws.Cell(r, c + 1);
            valCell.Value = val;
            valCell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            valCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            if (mergeExtra > 0) ws.Range(r, c + 1, r, c + 1 + mergeExtra).Merge();
        }
        private string DataStr(DateTime? d) => d.HasValue ? d.Value.ToString("dd/MM/yyyy") : "";
        private string Check(bool b) => b ? "[ X ]" : "[   ]";

        #endregion

        #region Geração PDF (iText7)

        private void GerarPdf(string caminho, Processo p)
        {
            using var writer = new PdfWriter(caminho);
            using var pdf = new PdfDocument(writer);
            var document = new Document(pdf, PageSize.A4);
            document.SetMargins(20, 20, 20, 20);

            var fontBold = PdfFontFactory.CreateFont(StandardFonts.TIMES_BOLD);
            var fontRegular = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
            var bordaPreta = new SolidBorder(ColorConstants.BLACK, 1f);

            // 1. Logo
            if (File.Exists(_caminhoLogo))
            {
                try
                {
                    ImageData imgData = ImageDataFactory.Create(_caminhoLogo);
                    Image logo = new Image(imgData);
                    logo.ScaleToFit(140, 70);
                    logo.SetFixedPosition(1, 130, 750);
                    document.Add(logo);
                }
                catch { }
            }

            // Tabela 8 Colunas
            // A=1, B=2, C=3, D=4, E=5, F=6, G=7, H=8
            float[] larguras = { 3f, 4.5f, 1.2f, 2.5f, 4.0f, 0.7f, 3.0f, 5.0f };
            var table = new Table(UnitValue.CreatePercentArray(larguras)).UseAllAvailableWidth();

            // Cabeçalho
            Cell cellVazia = new Cell(2, 1).SetBorder(bordaPreta).SetBorderRight(Border.NO_BORDER);
            table.AddCell(cellVazia);

            var cellTitulo1 = CriarCellTexto("U.S.A", 1, 7, fontBold, 24, true);
            cellTitulo1.SetBorder(bordaPreta).SetBorderLeft(Border.NO_BORDER).SetBorderBottom(Border.NO_BORDER);
            table.AddCell(cellTitulo1);

            var cellTitulo2 = CriarCellTexto("Despachos Aduaneiros Ltda.", 1, 7, fontBold, 20, true);
            cellTitulo2.SetBorder(bordaPreta).SetBorderLeft(Border.NO_BORDER).SetBorderTop(Border.NO_BORDER);
            table.AddCell(cellTitulo2);

            table.AddCell(new Cell(1, 8).SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetHeight(10).SetBorder(bordaPreta));

            // --- DADOS ALINHADOS ---

            // Linha 4
            AddLabelValue(table, "N/REF:", p.Ref_USA, fontBold, fontRegular);     // A-B
            AddLabelValue(table, "S/REF:", p.SR, fontBold, fontRegular, 1, 2);    // C-E
            AddLabelValue(table, "EXP:", p.Exportador, fontBold, fontRegular, 1, 2); // F-H

            // Linha 5
            AddLabelValue(table, "Cliente", p.Importador, fontBold, fontRegular); // A-B
            AddLabelValue(table, "Procedência", p.Origem, fontBold, fontRegular, 1, 2); // C-E
            AddLabelValue(table, "Embarcação", p.Veiculo, fontBold, fontRegular, 1, 2); // F-H

            // Linha 6
            AddLabelValue(table, "Produto", p.Produto, fontBold, fontRegular, 1, 4); // A-E
            AddLabelValue(table, "Chegada", DataStr(p.DataDeAtracacao), fontBold, fontRegular, 1, 2); // F-H

            // Linha 7
            string ncms = string.Join("; ", p.LI.Select(l => l.NCM).Distinct());
            AddLabelValue(table, "NCM", ncms, fontBold, fontRegular, 1, 4); // A-E
            AddLabelValue(table, "Armador", p.Armador, fontBold, fontRegular); // F-G 

            // Free Time no H
            table.AddCell(CriarCellTexto($"Free Time: {p.FreeTime}", 1, 1, fontRegular, 10, true)); // H

            // --- MUDANÇA DE ORDEM AQUI (LINHA 8) ---
            // 1. Master: Col 1-2 (Label A, Valor B)
            AddLabelValue(table, "Master", p.Capa.Master, fontBold, fontRegular);

            // 2. Conhecimento: Col 3-6 (Label C, Valor D-F) -> 3 cols para o valor
            AddLabelValue(table, "Conhecimento", p.Conhecimento, fontBold, fontRegular, 1, 2);

            // 3. Container: Col 7-8 (Label G, Valor H)
            AddLabelValue(table, "Container", p.Capa.Container, fontBold, fontRegular, 1, 2);

            // Terminal e Redestinado
            table.AddCell(CriarCellTexto("Terminal", 1, 1, fontBold));
            table.AddCell(CriarCellTexto(p.Terminal, 1, 3, fontRegular)); // B-D
            string txtRedestinado = $"{(p.Capa.DAT_IIDeferida ? "[X]" : "[ ]")} REDESTINADO - DATA";
            table.AddCell(CriarCellTexto(txtRedestinado, 1, 4, fontRegular)); // E-H

            // --- LIs (MODIFICADO) ---
            if (p.LI.Any())
            {
                foreach (var li in p.LI)
                {
                    int linhasLpco = li.LPCO.Count > 0 ? li.LPCO.Count : 1;

                    table.AddCell(CriarCellTexto($"LI nº {li.Numero}", linhasLpco, 2, fontBold, 10, true));

                    if (li.LPCO.Count == 0)
                    {
                        // Sem LPCO: Col 4-8 (D-H)
                        table.AddCell(CriarCellTexto("Sem LPCOs associados", 1, 5, fontRegular, 10, true));
                    }
                    else
                    {
                        foreach (var lpco in li.LPCO)
                        {
                            // Col 4-5 (D-E): LPCO
                            table.AddCell(CriarCellTexto($"LPCO nº {lpco.LPCO}", 1, 4, fontRegular, 10, true));

                            // Col 7-8 (G-H): Orgão
                            table.AddCell(CriarCellTexto(lpco.NomeOrgao, 1, 2, fontRegular, 10, true));
                        }
                    }
                }
            }

            // SIGVIG
            table.AddCell(CriarCellTexto("SIGVIG", 1, 1, fontBold));
            string sigSel = Check(p.Capa.SigvigSelecionado) + " Selecionado";
            string sigLib = Check(p.Capa.SigvigLiberado) + " Liberado : " + DataStr(p.Capa.SigvigData);
            table.AddCell(CriarCellTexto(sigSel, 1, 2, fontRegular));
            table.AddCell(CriarCellTexto(sigLib, 1, 5, fontRegular));

            // Incoterm
            table.AddCell(CriarCellTexto("Incoterm", 1, 1, fontBold));
            table.AddCell(CriarCellTexto(p.Capa.Incoterm, 1, 3, fontRegular));
            table.AddCell(CriarCellTexto("[ ] MOEDA/PESO/VALOR DE ACORDO COM DOCS.", 1, 4, fontRegular));

            // Numerário
            var opsNum = new[] { "Prestação Serviço", "Agência", "Tributos", "Completo", "Complementar" };
            var selsNum = p.Capa.Numerario?.ToList() ?? new List<string>();
            string txtNum = string.Join("  ", opsNum.Select(o => $"{(selsNum.Contains(o) ? "[ X ]" : "[   ]")} {o}"));

            table.AddCell(CriarCellTexto("Numerário", 1, 1, fontBold));
            table.AddCell(CriarCellTexto(txtNum, 1, 7, fontRegular));

            // DTA, DI, Marinha
            AddLabelValue(table, "DTA", p.Capa.DTA, fontBold, fontRegular, 1, 7);
            AddLabelValue(table, "DI", p.DI, fontBold, fontRegular, 1, 7);

            // Marinha e CE
            table.AddCell(CriarCellTexto("Marinha", 1, 1, fontBold));
            table.AddCell(CriarCellTexto(p.Capa.Marinha, 1, 3, fontRegular));
            table.AddCell(CriarCellTexto($"CE: {p.Capa.CE}", 1, 4, fontRegular));

            // Imposto
            var opsImp = new[] { "I.I", "I.P.I.", "PIS/PASEP", "COFINS", "ICMS" };
            var selsImp = p.Capa.Imposto?.ToList() ?? new List<string>();
            string txtImp = string.Join("  ", opsImp.Select(o => $"{(selsImp.Contains(o) ? "[ X ]" : "[   ]")} {o}"));

            table.AddCell(CriarCellTexto("Imposto", 1, 1, fontBold));
            table.AddCell(CriarCellTexto(txtImp, 1, 7, fontRegular));

            // Status e Observações
            table.AddCell(CriarCellTexto("Status do Processo", 1, 1, fontBold, 10, true, ColorConstants.LIGHT_GRAY));
            table.AddCell(CriarCellTexto(p.HistoricoDoProcesso, 1, 7, fontRegular));

            table.AddCell(CriarCellTexto("Observações", 1, 1, fontBold, 9, true, ColorConstants.LIGHT_GRAY));
            table.AddCell(CriarCellTexto(p.Capa.Observacoes, 1, 7, fontRegular));

            // Checklist
            AddCheckItem(table, "Tela do Canal", p.Capa.TelaDoCanal, 3);
            AddCheckItemDuasLinhas(table, "AVERBAR", p.Capa.Averbar, 3, p.Capa.AverbarData);
            AddCheckItemDuasLinhas(table, "LIBERAR B/L", p.Capa.LiberarBL, 2, p.Capa.LiberarBLData);

            AddCheckItemDuasLinhas(table, "MARINHA MERCANTE/ISENÇÃO", p.Capa.MarinhaMercante_Isencao, 3, p.Capa.MarinhaMercante_IsencaoData);
            AddCheckItemDuasLinhas(table, "I.C.M.S. OU EXONERAÇÃO", p.Capa.ICMS_Exoneracao, 3, p.Capa.ICMS_ExoneracaoData);
            AddCheckItem(table, "Lançado", p.Capa.Lancado, 2);

            AddCheckItem(table, "Consulta SEFAZ", p.Capa.ConsultaSEFAZ, 3);
            AddCheckItem(table, "DAT II Deferida", p.Capa.DAT_IIDeferida, 3);
            AddCheckItemDuasLinhas(table, "SISCARGA LIBERADO", p.Capa.SISCargaLiberado, 2, p.Capa.SISCargaLiberadoData);

            AddCheckItem(table, "DANFE", p.Capa.DANFE, 3);

            string linha1Arm = $"{Check(p.Capa.Armazenagem)} Armazenagem - {Check(p.Capa.Faturado)} Faturado";
            string linha2Pago = $"Pago por: {p.Capa.PagoPor}";

            var cellArm = new Cell(1, 3).SetPadding(2).SetVerticalAlignment(VerticalAlignment.MIDDLE);
            cellArm.Add(new Paragraph(linha1Arm).SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA)).SetFontSize(10));
            cellArm.Add(new Paragraph(linha2Pago).SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA)).SetFontSize(9));
            table.AddCell(cellArm);

            string txtEntT = $"{Check(p.Capa.ENTTransporte)} ENT Transporte Nº {p.Capa.ENTTransporteN}";
            if (p.Capa.ENTTransporteData != null) txtEntT += $" - {DataStr(p.Capa.ENTTransporteData)}";
            table.AddCell(CriarCellTexto(txtEntT, 1, 2, fontRegular));

            string txtEntA = $"{Check(p.Capa.ENTAlfandega)} ENT Alfândega Dossiê {p.Capa.ENTAlfandegaDossie}";
            if (p.Capa.ENTAlfandegaData != null) txtEntA += $" - {DataStr(p.Capa.ENTAlfandegaData)}";
            table.AddCell(CriarCellTexto(txtEntA, 1, 3, fontRegular));

            AddCheckItemDuasLinhas(table, "CONFERENCIA FÍSICA", p.Capa.ConferenciaFisica, 3, p.Capa.ConferenciaFisicaData);

            table.AddCell(new Cell(1, 2));

            document.Add(table);
        }

        private void AddCheckItemDuasLinhas(Table t, string label, bool check, int colspan, DateTime? date)
        {
            var cell = new Cell(1, colspan).SetPadding(2).SetVerticalAlignment(VerticalAlignment.MIDDLE);
            cell.Add(new Paragraph($"{Check(check)} {label}").SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA)).SetFontSize(10));
            string dataTxt = date != null ? $"DATA: {DataStr(date)}" : "DATA:       /        /";
            cell.Add(new Paragraph(dataTxt).SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA)).SetFontSize(9));
            t.AddCell(cell);
        }

        private void AddCheckItem(Table t, string label, bool check, int colspan, string extraLabel = null, DateTime? date = null)
        {
            string txt = $"{Check(check)} {label}";
            if (extraLabel != null) txt += $" {extraLabel}";
            if (date != null) txt += $" - {DataStr(date)}";
            else if (extraLabel == "DATA") txt += " -       /       /";

            t.AddCell(CriarCellTexto(txt, 1, colspan, PdfFontFactory.CreateFont(StandardFonts.HELVETICA), 10));
        }

        // Helpers PDF
        private Cell CriarCellTexto(string texto, int rowspan, int colspan, PdfFont font, int fontSize = 10, bool center = false, Color bg = null)
        {
            var p = new Paragraph(texto ?? "").SetFont(font).SetFontSize(fontSize);
            var cell = new Cell(rowspan, colspan).Add(p).SetPadding(2);
            if (center) cell.SetTextAlignment(TextAlignment.CENTER).SetVerticalAlignment(VerticalAlignment.MIDDLE);
            else cell.SetVerticalAlignment(VerticalAlignment.MIDDLE);
            if (bg != null) cell.SetBackgroundColor(bg);
            return cell;
        }

        private void AddLabelValue(Table t, string label, string value, PdfFont fb, PdfFont fr, int rowS = 1, int colS_Val = 1)
        {
            t.AddCell(CriarCellTexto(label, rowS, 1, fb));
            t.AddCell(CriarCellTexto(value, rowS, colS_Val, fr));
        }
        #endregion
    }
}
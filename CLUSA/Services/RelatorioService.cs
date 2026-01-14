using ClosedXML.Excel;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CLUSA.Models; // Importante para reconhecer as novas classes
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
    public class RelatorioService
    {
        private const string Colecao = "PROCESSO";
        private readonly string _pastaDestino = @"C:\UsaDespachos\Docs\Relatorios";
        private readonly string _caminhoLogo = @"C:\UsaDespachos\Exportador\logo.png";

        public RelatorioService()
        {
            if (!Directory.Exists(_pastaDestino)) Directory.CreateDirectory(_pastaDestino);
        }

        public async Task<string> GerarRelatorioAsync(string refUsa)
        {
            var processo = await BuscarProcessoAsync(refUsa);

            string refFormatada = refUsa.Replace("/", "-").Replace("\\", "-");
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string nomeBase = $"relatorio_{refFormatada}_{timestamp}";

            // 1. Gera Excel (ClosedXML)
            string caminhoExcel = System.IO.Path.Combine(_pastaDestino, $"{nomeBase}.xlsx");
            GerarExcel(caminhoExcel, processo);

            // 2. Gera PDF (iText7)
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

            // Inicializações de segurança
            processo.LI ??= new List<LicencaImportacao>();
            processo.Capa ??= new Capa();

            return processo;
        }

        #region Geração Excel (ClosedXML)

        private void GerarExcel(string caminho, Processo p)
        {
            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Relatório");

            // Configuração Pagina
            ws.PageSetup.PageOrientation = XLPageOrientation.Portrait;
            ws.PageSetup.FitToPages(1, 1);
            ws.PageSetup.Margins.SetLeft(0.25).SetRight(0.25).SetTop(0.3).SetBottom(0.3);

            // Larguras das colunas
            ws.Column(1).Width = 5;  // A
            ws.Column(2).Width = 10; // B
            ws.Column(3).Width = 10; // C
            ws.Column(4).Width = 5;  // D
            ws.Column(5).Width = 5;  // E
            ws.Column(6).Width = 5;  // F
            ws.Column(7).Width = 5;  // G
            ws.Column(8).Width = 5;  // H
            ws.Column(9).Width = 5;  // I
            ws.Column(10).Width = 5; // J
            ws.Column(11).Width = 17;// K
            ws.Column(12).Width = 5; // L
            ws.Column(13).Width = 15;// M
            ws.Column(14).Width = 10;// N
            ws.Column(15).Width = 5; // O
            ws.Column(16).Width = 5; // P
            ws.Column(17).Width = 10;// Q
            ws.Column(18).Width = 5; // R

            // Altura das linhas iniciais
            ws.Row(1).Height = 35;
            ws.Row(2).Height = 35;
            ws.Row(3).Height = 35;

            // --- CABEÇALHO ---
            ws.Range("A1:R1").Merge();
            if (File.Exists(_caminhoLogo))
            {
                var pic = ws.AddPicture(_caminhoLogo).MoveTo(ws.Cell(1, 1));
                pic.Width = 250; pic.Height = 85;
            }
            ws.Cell("A1").Value = "U.S.A";
            ws.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A1").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Cell("A1").Style.Font.FontName = "Times New Roman";
            ws.Cell("A1").Style.Font.FontSize = 28;

            ws.Range("A2:R2").Merge().Value = "Despachos Aduaneiros Ltda.";
            ws.Cell("A2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A2").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Cell("A2").Style.Font.FontName = "Times New Roman";
            ws.Cell("A2").Style.Font.FontSize = 28;

            ws.Range("A3:R3").Merge().Value = "Relatório Processo";
            EstilizarTitulo(ws.Cell("A3"), 26);

            // --- BLOCOS DE DADOS ---
            SetLabelVal(ws, 5, 2, 4, "Ref. USA:", p.Ref_USA);
            SetLabelVal(ws, 6, 2, 4, "Ref. S.:", p.SR);
            SetLabelVal(ws, 7, 2, 4, "Importador:", p.Importador);
            SetLabelVal(ws, 8, 2, 4, "Exportador:", p.Exportador);
            SetLabelVal(ws, 9, 2, 4, "Porto Destino:", p.PortoDestino);
            SetLabelVal(ws, 10, 2, 4, "Terminal:", p.Terminal);
            SetLabelVal(ws, 11, 2, 4, "Veículo:", p.Veiculo);
            SetLabelVal(ws, 12, 2, 4, "Armador:", p.Armador);

            SetLabelVal(ws, 5, 11, 12, "Produto:", p.Produto);
            SetLabelVal(ws, 6, 11, 12, "FLO:", p.FLO);
            SetLabelVal(ws, 7, 11, 12, "Origem:", p.Origem);
            SetLabelVal(ws, 8, 11, 12, "Conhecimento:", p.Conhecimento);
            SetLabelVal(ws, 9, 11, 12, "Free Time:", p.FreeTime.ToString());
            SetLabelVal(ws, 10, 11, 12, "Marca:", p.Marca);

            ws.Cell("K11").Value = "Amostra:"; ws.Cell("K11").Style.Font.Bold = true;
            ws.Cell("L11").Value = Check(p.Amostra); ws.Cell("L11").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            ws.Cell("K12").Value = "Desovado:"; ws.Cell("K12").Style.Font.Bold = true;
            ws.Cell("L12").Value = Check(p.Desovado); ws.Cell("L12").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            MergeBlocoDados(ws);

            // Bloco Docs Recebidos
            ws.Range("M14:R15").Merge().Value = "Doc. Recebidos";
            EstilizarTitulo(ws.Cell("M14"), 20);

            // Checklist Docs (Agora usa o array p.DocRecebidos diretamente)
            string[] docs = p.DocRecebidos ?? Array.Empty<string>();
            AddDocCheck(ws, 16, "BL", docs);
            AddDocCheck(ws, 17, "Fatura", docs);
            AddDocCheck(ws, 18, "Packing List", docs);
            AddDocCheck(ws, 19, "CO", docs);
            AddDocCheck(ws, 20, "Fito", docs);
            AddDocCheck(ws, 21, "CSI", docs);
            AddDocCheck(ws, 22, "CA", docs);
            AddDocCheck(ws, 23, "CF", docs);

            // Forma Rec e Data
            ws.Cell("M24").Value = "Forma Rec."; ws.Cell("M24").Style.Font.Bold = true;
            ws.Cell("O24").Value = p.FormaRecOriginais;
            ws.Range("O24:R24").Merge();

            ws.Cell("M25").Value = "Data Rec. Originais"; ws.Cell("M25").Style.Font.Bold = true;
            ws.Cell("O25").Value = DataStr(p.DataRecebOriginais);
            ws.Range("O25:R25").Merge();

            // Pendência
            ws.Range("M26:R26").Merge().Value = "Pendência";
            ws.Cell("M26").Style.Fill.BackgroundColor = XLColor.FromHtml("#D0D0D0");
            ws.Cell("M26").Style.Font.Bold = true;
            ws.Cell("M26").Style.Font.Underline = XLFontUnderlineValues.Single;
            ws.Cell("M26").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            ws.Range("M27:R30").Merge().Value = p.Pendencia;
            ws.Cell("M27").Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
            ws.Cell("M27").Style.Alignment.WrapText = true;

            // Bloco DI
            SetLabelVal(ws, 15, 2, 6, "Num. DI:", p.DI);
            SetLabelVal(ws, 16, 2, 6, "Parametrização DI:", p.ParametrizacaoDI);
            SetLabelVal(ws, 17, 2, 6, "Data de Registro:", DataStr(p.DataRegistroDI));
            SetLabelVal(ws, 18, 2, 6, "Data de Desembaraço:", DataStr(p.DataDesembaracoDI));
            SetLabelVal(ws, 19, 2, 6, "Data de Carregamento:", DataStr(p.DataCarregamentoDI));
            SetLabelVal(ws, 20, 2, 6, "Data de Inspeção:", DataStr(p.Inspecao));
            SetLabelVal(ws, 21, 2, 6, "Data de Minuta:", DataStr(p.DataMinutaDI));
            MergeBlocoDI(ws);

            // Bloco Datas
            ws.Range("B24:F24").Merge().Value = "Datas";
            ws.Cell("B24").Style.Fill.BackgroundColor = XLColor.FromHtml("#D0D0D0");
            ws.Cell("B24").Style.Font.Bold = true;
            ws.Cell("B24").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            SetLabelVal(ws, 25, 2, 4, "Data de Embarque:", DataStr(p.DataEmbarque));
            SetLabelVal(ws, 26, 2, 4, "Data de Chegada:", DataStr(p.DataDeAtracacao));
            SetLabelVal(ws, 27, 2, 4, "Vencimento Free Time:", DataStr(p.VencimentoFreeTime));
            SetLabelVal(ws, 28, 2, 4, "Vencimento FMA:", DataStr(p.VencimentoFMA));
            SetLabelVal(ws, 29, 2, 4, "Vencimento LI/LPCO:", DataStr(p.VencimentoLI_LPCO));
            MergeBlocoDatas(ws);

            // Bloco Órgãos Anuentes (ATUALIZADO PARA NOVA LÓGICA)
            // Agora verificamos se o nome do órgão está na lista calculada
            ws.Range("H24:K24").Merge().Value = "Órgãos Anuentes";
            ws.Cell("H24").Style.Fill.BackgroundColor = XLColor.FromHtml("#D0D0D0");
            ws.Cell("H24").Style.Font.Bold = true;
            ws.Cell("H24").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            bool HasOrgao(string nome) => p.OrgaosAnuentesString != null && p.OrgaosAnuentesString.ToUpper().Contains(nome.ToUpper());

            AddAnuenteCheck(ws, 25, "Mapa", HasOrgao("MAPA"));
            AddAnuenteCheck(ws, 26, "Anvisa", HasOrgao("ANVISA"));
            AddAnuenteCheck(ws, 27, "Decex", HasOrgao("DECEX"));
            AddAnuenteCheck(ws, 28, "Inmetro", HasOrgao("INMETRO"));
            AddAnuenteCheck(ws, 29, "Ibama", HasOrgao("IBAMA"));

            // Cabeçalho LI
            int row = 31;
            ws.Range(row, 1, row, 2).Merge().Value = "LI";
            ws.Range(row, 3, row, 4).Merge().Value = "Data Registro LI";
            ws.Range(row, 5, row, 7).Merge().Value = "NCM";
            ws.Range(row, 8, row, 10).Merge().Value = "LPCO";
            ws.Cell(row, 11).Value = "Data Registro LPCO";
            ws.Range(row, 12, row, 13).Merge().Value = "Parametrização";
            ws.Range(row, 14, row, 15).Merge().Value = "Data Deferimento";
            ws.Range(row, 16, row, 18).Merge().Value = "Órgãos Anuentes";

            for (int c = 1; c <= 18; c++)
            {
                ws.Cell(row, c).Style.Fill.BackgroundColor = XLColor.FromHtml("#D0D0D0");
                ws.Cell(row, c).Style.Font.Bold = true;
                ws.Cell(row, c).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }

            // Dados LIs (ATUALIZADO PARA NOVOS NOMES DE PROPRIEDADE)
            row++;
            if (p.LI != null && p.LI.Any())
            {
                foreach (var li in p.LI)
                {
                    // Caso haja múltiplos LPCOs na LI, precisamos iterar ou pegar o primeiro.
                    // Se a LI tiver múltiplos LPCOs, o ideal é listar um por linha ou juntar.
                    // Aqui vamos iterar para garantir que todos apareçam.

                    var listaLpcos = li.LPCO != null && li.LPCO.Any() ? li.LPCO : new List<LpcoInfo> { new LpcoInfo() };

                    foreach (var lpco in listaLpcos)
                    {
                        ws.Range(row, 1, row, 2).Merge().Value = li.Numero;
                        ws.Range(row, 3, row, 4).Merge().Value = DataStr(li.DataRegistro); // Era DataRegistroLI
                        ws.Range(row, 5, row, 7).Merge().Value = li.NCM;
                        ws.Range(row, 8, row, 10).Merge().Value = lpco.LPCO;
                        ws.Cell(row, 11).Value = DataStr(lpco.DataRegistroLPCO);
                        ws.Range(row, 12, row, 13).Merge().Value = lpco.ParametrizacaoLPCO;
                        ws.Range(row, 14, row, 15).Merge().Value = DataStr(lpco.DataDeferimentoLPCO);
                        ws.Range(row, 16, row, 18).Merge().Value = lpco.NomeOrgao; // Pega direto do LPCO

                        ws.Range(row, 1, row, 18).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        row++;
                    }
                }
            }

            // Status do Processo (Usando StatusDoProcesso para texto longo)
            ws.Range(row, 1, row, 18).Merge().Value = "Status do Processo";
            EstilizarTitulo(ws.Cell(row, 1), 20);
            row++;

            ws.Range(row, 1, row, 18).Merge().Value = p.HistoricoDoProcesso ?? ""; // Ou p.StatusDoProcesso
            ws.Cell(row, 1).Style.Alignment.WrapText = true;
            ws.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell(row, 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
            ws.Row(row).Height = 50;
            row++;

            // Rodapé
            ws.Range(row, 1, row, 15).Merge().Value = "Matriz: Rua Comendador Martins nº 55 Altos - Sala 22 - Vila Mathias - CEP 11015-530 - Santos - S.P.";
            ws.Cell(row, 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#ADADAD");
            ws.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            ws.Range(row, 16, row, 18).Merge().Value = "Duração do Processo";
            ws.Cell(row, 16).Style.Fill.BackgroundColor = XLColor.FromHtml("#D0D0D0");
            ws.Cell(row, 16).Style.Font.Bold = true;
            ws.Cell(row, 16).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            row++;

            ws.Range(row, 1, row, 15).Merge().Value = " Fone: (13)3222.8899 - 2202.8369  - e-mail: josecarlos@usadespachos.com.br - usa@bignet.com.br";
            ws.Cell(row, 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#ADADAD");
            ws.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            double dias = 0;
            if (p.DataDeAtracacao.HasValue && p.DataCarregamentoDI.HasValue)
            {
                dias = (p.DataDeAtracacao.Value - p.DataCarregamentoDI.Value).TotalDays;
                ws.Range(row, 16, row, 18).Merge().Value = $"{Math.Round(dias)} dias";
            }
            else
            {
                ws.Range(row, 16, row, 18).Merge().Value = "N/A";
            }
            ws.Cell(row, 16).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            var rangeTotal = ws.Range(3, 1, row, 18);
            rangeTotal.Style.Border.OutsideBorder = XLBorderStyleValues.Thick;

            var allCells = ws.Range(1, 1, row, 18);
            allCells.Style.Font.FontName = "Aptos Narrow";
            allCells.Style.Font.FontSize = 11;

            ws.Cell("A1").Style.Font.FontName = "Times New Roman";
            ws.Cell("A1").Style.Font.FontSize = 28;
            ws.Cell("A2").Style.Font.FontName = "Times New Roman";
            ws.Cell("A2").Style.Font.FontSize = 28;

            wb.SaveAs(caminho);
        }

        #region Helpers Excel
        private void SetLabelVal(IXLWorksheet ws, int r, int cLab, int cVal, string label, string val)
        {
            ws.Cell(r, cLab).Value = label;
            ws.Cell(r, cLab).Style.Font.Bold = true;
            ws.Cell(r, cVal).Value = val;
            ws.Cell(r, cVal).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        }

        private void AddDocCheck(IXLWorksheet ws, int r, string label, string[] docs)
        {
            ws.Cell(r, 13).Value = label;
            ws.Cell(r, 13).Style.Font.Bold = true;
            ws.Cell(r, 13).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // Verifica no array
            bool exists = docs.Contains(label);
            ws.Cell(r, 15).Value = Check(exists);
            ws.Cell(r, 15).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Range(r, 15, r, 16).Merge();
        }

        private void AddAnuenteCheck(IXLWorksheet ws, int r, string label, bool val)
        {
            ws.Cell(r, 8).Value = label;
            ws.Cell(r, 8).Style.Font.Bold = true;
            ws.Cell(r, 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell(r, 11).Value = Check(val);
            ws.Cell(r, 11).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        }

        private void EstilizarTitulo(IXLCell cell, double size)
        {
            cell.Style.Font.FontSize = size;
            cell.Style.Font.Bold = true;
            cell.Style.Font.Underline = XLFontUnderlineValues.Single;
            cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#D0D0D0");
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        }

        private void MergeBlocoDados(IXLWorksheet ws)
        {
            for (int r = 5; r <= 12; r++) { ws.Range(r, 2, r, 3).Merge(); ws.Range(r, 4, r, 8).Merge(); }
            for (int r = 5; r <= 10; r++) { ws.Range(r, 12, r, 12).Merge(); }
        }
        private void MergeBlocoDI(IXLWorksheet ws)
        {
            for (int r = 15; r <= 21; r++) { ws.Range(r, 2, r, 5).Merge(); ws.Range(r, 6, r, 11).Merge(); }
        }
        private void MergeBlocoDatas(IXLWorksheet ws)
        {
            for (int r = 25; r <= 29; r++) { ws.Range(r, 2, r, 3).Merge(); ws.Range(r, 4, r, 6).Merge(); }
        }
        #endregion

        #endregion

        #region Geração PDF (iText7)

        private void GerarPdf(string caminho, Processo p)
        {
            using var writer = new PdfWriter(caminho);
            using var pdf = new PdfDocument(writer);
            var document = new Document(pdf, PageSize.A4);
            document.SetMargins(20, 20, 20, 20);

            var fontBold = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
            var fontRegular = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

            float[] widths = { 5, 10, 10, 5, 5, 5, 5, 5, 5, 5, 17, 5, 15, 10, 5, 5, 10, 5 };
            var table = new Table(UnitValue.CreatePercentArray(widths)).UseAllAvailableWidth();

            // --- CABEÇALHO ---
            var cellHeader = new Cell(1, 18).SetBorder(Border.NO_BORDER).SetPadding(5);
            if (File.Exists(_caminhoLogo))
            {
                try
                {
                    ImageData imgData = ImageDataFactory.Create(_caminhoLogo);
                    Image img = new Image(imgData).ScaleToFit(150, 60);
                    cellHeader.Add(img.SetHorizontalAlignment(HorizontalAlignment.CENTER));
                }
                catch { }
            }
            cellHeader.Add(new Paragraph("U.S.A").SetFont(fontBold).SetFontSize(24).SetTextAlignment(TextAlignment.CENTER));
            cellHeader.Add(new Paragraph("Despachos Aduaneiros Ltda.").SetFont(fontBold).SetFontSize(20).SetTextAlignment(TextAlignment.CENTER));
            table.AddCell(cellHeader);

            table.AddCell(CriarCellTitulo("Relatório Processo", 18, fontBold, 22));

            // --- LAYOUT EM BLOCOS ---
            var layoutTable = new Table(UnitValue.CreatePercentArray(new float[] { 60, 40 })).UseAllAvailableWidth();

            var leftCell = new Cell().SetBorder(Border.NO_BORDER).SetPadding(2);
            leftCell.Add(TxtPair("Ref. USA:", p.Ref_USA));
            leftCell.Add(TxtPair("Ref. S.:", p.SR));
            leftCell.Add(TxtPair("Importador:", p.Importador));
            leftCell.Add(TxtPair("Exportador:", p.Exportador));
            leftCell.Add(TxtPair("Porto Destino:", p.PortoDestino));
            leftCell.Add(TxtPair("Terminal:", p.Terminal));
            leftCell.Add(TxtPair("Veículo:", p.Veiculo));
            leftCell.Add(TxtPair("Armador:", p.Armador));

            leftCell.Add(new Paragraph("\n"));
            leftCell.Add(new Paragraph("Dados da DI").SetFont(fontBold).SetBackgroundColor(ColorConstants.LIGHT_GRAY));
            leftCell.Add(TxtPair("Num. DI:", p.DI));
            leftCell.Add(TxtPair("Parametrização:", p.ParametrizacaoDI));
            leftCell.Add(TxtPair("Registro:", DataStr(p.DataRegistroDI)));
            leftCell.Add(TxtPair("Desembaraço:", DataStr(p.DataDesembaracoDI)));
            leftCell.Add(TxtPair("Carregamento:", DataStr(p.DataCarregamentoDI)));
            leftCell.Add(TxtPair("Inspeção:", DataStr(p.Inspecao)));
            leftCell.Add(TxtPair("Minuta:", DataStr(p.DataMinutaDI)));

            leftCell.Add(new Paragraph("\n"));
            leftCell.Add(new Paragraph("Datas Importantes").SetFont(fontBold).SetBackgroundColor(ColorConstants.LIGHT_GRAY));
            leftCell.Add(TxtPair("Embarque:", DataStr(p.DataEmbarque)));
            leftCell.Add(TxtPair("Chegada:", DataStr(p.DataDeAtracacao)));
            leftCell.Add(TxtPair("Venc. Free Time:", DataStr(p.VencimentoFreeTime)));
            leftCell.Add(TxtPair("Venc. FMA:", DataStr(p.VencimentoFMA)));
            leftCell.Add(TxtPair("Venc. LI/LPCO:", DataStr(p.VencimentoLI_LPCO)));

            var rightCell = new Cell().SetBorder(Border.NO_BORDER).SetPadding(2);
            rightCell.Add(TxtPair("Produto:", p.Produto));
            rightCell.Add(TxtPair("FLO:", p.FLO));
            rightCell.Add(TxtPair("Origem:", p.Origem));
            rightCell.Add(TxtPair("Conhecimento:", p.Conhecimento));
            rightCell.Add(TxtPair("Free Time:", p.FreeTime.ToString()));
            rightCell.Add(TxtPair("Marca:", p.Marca));
            rightCell.Add(TxtPair("Amostra:", Check(p.Amostra)));
            rightCell.Add(TxtPair("Desovado:", Check(p.Desovado)));

            rightCell.Add(new Paragraph("\n"));
            rightCell.Add(new Paragraph("Documentos Recebidos").SetFont(fontBold).SetBackgroundColor(ColorConstants.LIGHT_GRAY));
            string[] docsArr = p.DocRecebidos ?? Array.Empty<string>();
            rightCell.Add(TxtCheck("BL", docsArr));
            rightCell.Add(TxtCheck("Fatura", docsArr));
            rightCell.Add(TxtCheck("Packing List", docsArr));
            rightCell.Add(TxtCheck("CO", docsArr));
            rightCell.Add(TxtCheck("Fito", docsArr));

            rightCell.Add(new Paragraph("\n"));
            rightCell.Add(new Paragraph("Órgãos Anuentes").SetFont(fontBold).SetBackgroundColor(ColorConstants.LIGHT_GRAY));

            // Lógica atualizada para checar órgãos no PDF
            bool HasOrgao(string nome) => p.OrgaosAnuentesString != null && p.OrgaosAnuentesString.ToUpper().Contains(nome.ToUpper());

            rightCell.Add(TxtBool("Mapa", HasOrgao("MAPA")));
            rightCell.Add(TxtBool("Anvisa", HasOrgao("ANVISA")));
            rightCell.Add(TxtBool("Decex", HasOrgao("DECEX")));
            rightCell.Add(TxtBool("Inmetro", HasOrgao("INMETRO")));
            rightCell.Add(TxtBool("Ibama", HasOrgao("IBAMA")));

            rightCell.Add(new Paragraph("\n"));
            rightCell.Add(new Paragraph("Pendência").SetFont(fontBold).SetBackgroundColor(ColorConstants.LIGHT_GRAY));
            rightCell.Add(new Paragraph(p.Pendencia ?? "-").SetFontSize(9));

            layoutTable.AddCell(leftCell);
            layoutTable.AddCell(rightCell);

            var mainCell = new Cell(1, 18).Add(layoutTable).SetBorder(Border.NO_BORDER);
            table.AddCell(mainCell);

            // --- LIs ---
            var cellLiHeader = new Cell(1, 18).Add(new Paragraph("LIs").SetFont(fontBold).SetFontSize(14))
                .SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetTextAlignment(TextAlignment.CENTER);
            table.AddCell(cellLiHeader);

            float[] liWidths = { 15, 10, 10, 10, 10, 15, 10, 20 };
            var liTable = new Table(UnitValue.CreatePercentArray(liWidths)).UseAllAvailableWidth();

            string[] headers = { "LI", "Data Reg.", "NCM", "LPCO", "Data LPCO", "Param.", "Deferimento", "Anuentes" };
            foreach (var h in headers) liTable.AddHeaderCell(new Cell().Add(new Paragraph(h).SetFont(fontBold).SetFontSize(8)).SetBackgroundColor(ColorConstants.LIGHT_GRAY));

            if (p.LI != null && p.LI.Any())
            {
                foreach (var li in p.LI)
                {
                    // Itera LPCOs dentro da LI
                    var listaLpcos = li.LPCO != null && li.LPCO.Any() ? li.LPCO : new List<LpcoInfo> { new LpcoInfo() };
                    foreach (var lpco in listaLpcos)
                    {
                        liTable.AddCell(TxtCell(li.Numero));
                        liTable.AddCell(TxtCell(DataStr(li.DataRegistro))); // Atualizado
                        liTable.AddCell(TxtCell(li.NCM));
                        liTable.AddCell(TxtCell(lpco.LPCO));
                        liTable.AddCell(TxtCell(DataStr(lpco.DataRegistroLPCO)));
                        liTable.AddCell(TxtCell(lpco.ParametrizacaoLPCO));
                        liTable.AddCell(TxtCell(DataStr(lpco.DataDeferimentoLPCO)));
                        liTable.AddCell(TxtCell(lpco.NomeOrgao)); // Atualizado
                    }
                }
            }
            table.AddCell(new Cell(1, 18).Add(liTable));

            // Status e Rodapé
            table.AddCell(CriarCellTitulo("Status do Processo", 18, fontBold, 16));
            table.AddCell(new Cell(1, 18).Add(new Paragraph(p.HistoricoDoProcesso ?? "").SetFontSize(10)).SetMinHeight(40));

            var footer = new Cell(1, 18).SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetTextAlignment(TextAlignment.CENTER).SetFontSize(8);
            footer.Add(new Paragraph("Matriz: Rua Comendador Martins nº 55 Altos - Santos - SP | Fone: (13)3222.8899"));

            double dias = 0;
            if (p.DataDeAtracacao.HasValue && p.DataCarregamentoDI.HasValue)
                dias = (p.DataDeAtracacao.Value - p.DataCarregamentoDI.Value).TotalDays;

            footer.Add(new Paragraph($"Duração do Processo: {Math.Round(dias)} dias").SetFont(fontBold));
            table.AddCell(footer);

            document.Add(table);
            document.Close();
        }

        // Helpers PDF
        // --- Helpers PDF Corrigidos ---

        // Cria a fonte negrito dentro do helper para aplicar no Text
        private Paragraph TxtPair(string label, string val)
        {
            var fontBold = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
            return new Paragraph()
                .Add(new Text(label).SetFont(fontBold).SetFontSize(9)) // Usa SetFont em vez de SetBold
                .Add(new Text($" {val ?? ""}").SetFontSize(9));
        }

        private Paragraph TxtCheck(string label, string[] docs)
        {
            var fontBold = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
            return new Paragraph()
                .Add(new Text(label).SetFont(fontBold).SetFontSize(9))
                .Add(new Text($" {Check(docs.Any(d => d.Contains(label)))}").SetFontSize(9));
        }

        private Paragraph TxtBool(string label, bool val)
        {
            var fontBold = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
            return new Paragraph()
                .Add(new Text(label).SetFont(fontBold).SetFontSize(9))
                .Add(new Text($" {Check(val)}").SetFontSize(9));
        }

        private Cell TxtCell(string txt) => new Cell().Add(new Paragraph(txt ?? "").SetFontSize(8)).SetTextAlignment(TextAlignment.CENTER);

        private Cell CriarCellTitulo(string txt, int colspan, PdfFont font, float size)
        {
            return new Cell(1, colspan)
                .Add(new Paragraph(txt).SetFont(font).SetFontSize(size))
                .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
        }

        #endregion

        // Helpers Gerais
        private string DataStr(DateTime? d) => d.HasValue ? d.Value.ToString("dd/MM/yyyy") : "";
        private string Check(bool b) => b ? "☑" : "☐";
    }
}
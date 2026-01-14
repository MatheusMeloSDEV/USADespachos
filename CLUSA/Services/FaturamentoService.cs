using ClosedXML.Excel;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
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
    // Classe interna para despesas dinâmicas
    public class DespesaItem
    {
        public string Nome { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
        public decimal Valor { get; set; }
    }

    // Classe para resposta da API de CNPJ
    public class CnpjResponse
    {
        public string cnpj { get; set; }
        public string razao_social { get; set; }
        public string logradouro { get; set; }
        public string numero { get; set; }
        public string complemento { get; set; }
        public string bairro { get; set; }
        public string municipio { get; set; }
        public string uf { get; set; }
        public string cep { get; set; }
    }

    public class FaturamentoService
    {
        private const string Colecao = "Fatura";
        private readonly string _pastaDestino = @"C:\UsaDespachos\Docs\Faturamento";
        private readonly string _caminhoLogo = @"C:\UsaDespachos\Exportador\logo.png";
        private readonly HttpClient _httpClient;

        public FaturamentoService()
        {
            if (!Directory.Exists(_pastaDestino)) Directory.CreateDirectory(_pastaDestino);
            _httpClient = new HttpClient();
        }

        public async Task<string> GerarFaturamentoAsync(string refUsa)
        {
            var fatura = await BuscarFaturaAsync(refUsa);

            CnpjResponse? dadosCnpj = null;
            if (!string.IsNullOrWhiteSpace(fatura.Endereco_Importador))
            {
                dadosCnpj = await ConsultarCnpjBrasilApi(fatura.Endereco_Importador);
            }

            string refFormatada = refUsa.Replace("/", "-").Replace("\\", "-");
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string nomeBase = $"Faturamento_{fatura.Importador}_{refFormatada}_{timestamp}";

            string caminhoExcel = System.IO.Path.Combine(_pastaDestino, $"{nomeBase}.xlsx");
            GerarExcel(caminhoExcel, fatura, dadosCnpj);

            string caminhoPdf = System.IO.Path.Combine(_pastaDestino, $"{nomeBase}.pdf");
            GerarPdf(caminhoPdf, fatura, dadosCnpj);

            return caminhoPdf;
        }

        private async Task<Fatura> BuscarFaturaAsync(string refUsa)
        {
            var db = ConfigDatabase.GetDatabase();
            var collection = db.GetCollection<Fatura>(Colecao);
            var filtro = Builders<Fatura>.Filter.Eq(f => f.Ref_USA, refUsa.Trim());
            var fatura = await collection.Find(filtro).FirstOrDefaultAsync();
            if (fatura == null) throw new Exception($"Fatura para '{refUsa}' não encontrada.");
            return fatura;
        }

        private async Task<CnpjResponse?> ConsultarCnpjBrasilApi(string cnpj)
        {
            if (string.IsNullOrWhiteSpace(cnpj)) return null;
            string cnpjLimpo = new string(cnpj.Where(char.IsDigit).ToArray());
            if (cnpjLimpo.Length != 14) return null;
            try
            {
                var response = await _httpClient.GetAsync($"https://brasilapi.com.br/api/cnpj/v1/{cnpjLimpo}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<CnpjResponse>(json);
                }
            }
            catch { }
            return null;
        }

        private List<DespesaItem> ObterDespesas(Fatura f)
        {
            var lista = new List<DespesaItem>();

            if (f.Agencias != null)
            {
                foreach (var ag in f.Agencias)
                {
                    if (ag.Custo > 0 || !string.IsNullOrEmpty(ag.Numero))
                        lista.Add(new DespesaItem { Nome = "Agência", Numero = ag.Numero, Valor = ag.Custo });
                }
            }

            AddDespesa(lista, "Armazenagem", f.ArmazenagemN, f.ArmazenagemP);
            AddDespesa(lista, "Frete Marítimo", f.FreteMaritimoN, f.FreteMaritimoP);
            AddDespesa(lista, "Marinha Mercante", f.Marinha_MercanteN, f.Marinha_MercanteP);
            AddDespesa(lista, "GRU ANVISA", f.GRUANVISAN, f.GRUANVISAP);
            AddDespesa(lista, "LI Cancelada/ Indeferida", f.LiCancelada_IndeferidaN, f.LiCancelada_IndeferidaP);
            AddDespesa(lista, "Expediente LI Cancelada", f.ExpedienteLiCanceladaN, f.ExpedienteLiCanceladaP);
            AddDespesa(lista, "Encaminhamento Amostras", f.EncaminhamentoAmostrasN, f.EncaminhamentoAmostrasP);
            AddDespesa(lista, "Darf Anvisa", f.DarfAnvisaN, f.DarfAnvisaP);
            AddDespesa(lista, "Motoboy", f.MotoboyN, f.MotoboyP);
            if (f.LiP > 0) lista.Add(new DespesaItem { Nome = "L.I.", Valor = f.LiP });
            if (f.Expediente > 0) lista.Add(new DespesaItem { Nome = "Expediente", Valor = f.Expediente });
            AddDespesa(lista, "Despesas com Desembaraço", f.DespesasDesembaracoN, f.DespesasDesembaracoP);
            if (f.HD > 0) lista.Add(new DespesaItem { Nome = "HD", Valor = f.HD });
            if (f.Cartorio > 0) lista.Add(new DespesaItem { Nome = "Cartório", Valor = f.Cartorio });
            return lista;
        }

        private void AddDespesa(List<DespesaItem> lista, string nome, string num, decimal val)
        {
            if (!string.IsNullOrEmpty(num) || val > 0)
                lista.Add(new DespesaItem { Nome = nome, Numero = num, Valor = val });
        }

        #region Geração Excel (ClosedXML) - CORRIGIDO E ALINHADO COM PDF
        private void GerarExcel(string caminho, Fatura f, CnpjResponse? cnpj)
        {
            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Faturamento");

            ws.PageSetup.PageOrientation = XLPageOrientation.Portrait;
            ws.PageSetup.FitToPages(1, 1);
            ws.PageSetup.Margins.SetLeft(0.25).SetRight(0.25).SetTop(0.3).SetBottom(0.3);

            // Larguras exatas
            ws.Column(1).Width = 30;
            ws.Column(2).Width = 24.14;
            ws.Column(3).Width = 13.71;
            ws.Column(4).Width = 6.86;
            ws.Column(5).Width = 4.71;
            ws.Column(6).Width = 9.43;
            ws.Column(7).Width = 11.86;
            ws.Column(8).Width = 19.57;
            ws.Column(9).Width = 5.86;
            ws.Column(10).Width = 12.86;

            ws.Row(12).Height = 25;
            ws.Row(15).Height = 25;

            // Cabeçalho
            ws.Range("A1:J1").Merge(); ws.Range("A2:J2").Merge(); ws.Range("A3:J3").Merge();

            if (File.Exists(_caminhoLogo))
            {
                var pic = ws.AddPicture(_caminhoLogo).MoveTo(ws.Cell("A1"));
                pic.Width = 275; pic.Height = 85;
            }

            SetVal(ws, "A1", "U.S.A", "Times New Roman", 30, true);
            SetVal(ws, "A2", "Despachos Aduaneiros Ltda.", "Times New Roman", 25, true);
            ws.Row(3).Style.Fill.BackgroundColor = XLColor.FromHtml("#D9D9D9");

            // Merges do Cabeçalho
            ws.Range("A4:F4").Merge(); ws.Range("G4:J4").Merge();
            ws.Range("A5:F5").Merge(); ws.Range("H5:J5").Merge();
            ws.Range("A6:F6").Merge(); ws.Range("H6:J6").Merge();
            ws.Range("A7:F7").Merge(); ws.Range("H7:J7").Merge();
            ws.Range("A8:F8").Merge(); ws.Range("G8:J8").Merge();

            if (cnpj != null)
            {
                SetVal(ws, "A5", cnpj.razao_social, "Arial", 10, true, XLAlignmentHorizontalValues.Left);
                SetVal(ws, "A6", $"{cnpj.logradouro}, {cnpj.numero} {cnpj.complemento}".Trim(), "Arial", 10, true, XLAlignmentHorizontalValues.Left);
                SetVal(ws, "A7", $"{cnpj.bairro} - {cnpj.municipio} - {cnpj.uf} - CEP: {cnpj.cep}", "Arial", 10, true, XLAlignmentHorizontalValues.Left);
            }
            else
            {
                SetVal(ws, "A5", f.Importador, "Arial", 10, true, XLAlignmentHorizontalValues.Left);
                SetVal(ws, "A6", f.Endereco_Importador, "Arial", 10, true, XLAlignmentHorizontalValues.Left);
            }

            string dataHoje = $"Santos, {DateTime.Now.ToString("dd 'de' MMMM 'de' yyyy", new System.Globalization.CultureInfo("pt-BR"))}";
            SetVal(ws, "G4", dataHoje, "Arial", 10, false, XLAlignmentHorizontalValues.Center);

            ws.Cell("G5").Value = "Fatura N/Ref.:"; ws.Cell("G5").Style.Font.FontName = "Arial"; ws.Cell("G5").Style.Font.FontSize = 10;
            ws.Cell("H5").Value = f.Ref_USA; ws.Cell("H5").Style.Font.FontName = "Arial"; ws.Cell("H5").Style.Font.FontSize = 10;
            ws.Cell("G6").Value = "FLO.:"; ws.Cell("G6").Style.Font.FontName = "Arial"; ws.Cell("G6").Style.Font.FontSize = 10;
            ws.Cell("H6").Value = f.FLO; ws.Cell("H6").Style.Font.FontName = "Arial"; ws.Cell("H6").Style.Font.FontSize = 10;
            ws.Cell("G7").Value = "S/Ref.:"; ws.Cell("G7").Style.Font.FontName = "Arial"; ws.Cell("G7").Style.Font.FontSize = 10;
            ws.Cell("H7").Value = f.SR; ws.Cell("H7").Style.Font.FontName = "Arial"; ws.Cell("H7").Style.Font.FontSize = 10;
            SetVal(ws, "G8", "Importação", "Arial", 16, true, XLAlignmentHorizontalValues.Center);

            // Corpo
            ws.Range("A9:J9").Merge(); ws.Range("A10:J10").Merge();
            ws.Cell("A9").Value = "Despesas efetuadas, pôr sua ordem e conta, com as mercadorias abaixo discriminadas, expedidas pelo veículo ";
            ws.Cell("A9").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            ws.Cell("A10").Value = $"{f.Veiculo}   atracado em   {DataStr(f.DataAtracacao)}";
            ws.Cell("A10").Style.Font.Bold = true;
            ws.Cell("A10").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            ws.Range("B11:E11").Merge(); ws.Range("F11:J11").Merge(); ws.Range("B12:E12").Merge(); ws.Range("F12:J12").Merge();
            SetVal(ws, "A11", "MARCA", "Arial", 10, false); SetVal(ws, "A12", f.Marca, "Arial", 10, false);
            SetVal(ws, "B11", "QUANTIDADE", "Arial", 10, false); SetVal(ws, "B12", f.Quantidade.ToString(), "Arial", 10, false);
            SetVal(ws, "F11", "MERCADORIA", "Arial", 10, false); SetVal(ws, "F12", f.Mercadoria, "Arial", 10, false);

            ws.Range("A14:C14").Merge(); ws.Range("D14:E14").Merge(); ws.Range("F14:J14").Merge(); ws.Range("A15:C15").Merge(); ws.Range("D15:E15").Merge(); ws.Range("F15:J15").Merge();
            SetVal(ws, "A14", "VALORES RECEBIDOS  EM R$", "Arial", 10, false); SetVal(ws, "A15", f.ValRecebidos.ToString("N2"), "Arial", 10, false);
            SetVal(ws, "D14", "DATA", "Arial", 10, false); SetVal(ws, "D15", DataStr(f.DataRecebimento), "Arial", 10, false);
            SetVal(ws, "F14", "LANÇAMENTOS", "Arial", 10, false); SetVal(ws, "F15", "VALORES PARA DESEMBARAÇO", "Arial", 10, false);

            // Despesas Aduaneiras (Header Cinza com Borda)
            ws.Range("A16:J16").Merge();
            SetVal(ws, "A16", "DESPESAS ADUANEIRAS", "Arial", 10, true);
            ws.Cell("A16").Style.Fill.BackgroundColor = XLColor.FromHtml("#D9D9D9");
            ws.Range("A16:J16").Style.Border.OutsideBorder = XLBorderStyleValues.Thin; // Borda adicionada

            // Linha DI
            ws.Range("C17:F17").Merge();
            ws.Cell("A17").Value = "   Decl. De Importação nº ";
            ws.Cell("C17").Value = $"{f.DI} de {DataStr(f.DAtaDI)}";
            ws.Cell("C17").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Range("A17:J17").Style.Border.OutsideBorder = XLBorderStyleValues.Thin; // Borda adicionada

            var impostos = new List<(string, decimal)> { (" Imposto de Importação", f.ImpostoImportacao), (" I.P.I.", f.IPI), (" DI/ADIÇÃO", f.DI_ADICAO), (" P.I.S./P.A.S.E.P.", f.PIS_PASEP), (" C.O.F.I.N.S.", f.COFINS), (" MULTA L.I.", f.MULTA_LI), (" ICMS   -", f.ICMS) };
            int row = 18;
            foreach (var imp in impostos)
            {
                ws.Range(row, 2, row, 9).Merge();
                ws.Cell(row, 1).Value = imp.Item1;
                SetMoney(ws, row, 10, imp.Item2);

                // Bordas internas nos itens
                ws.Range(row, 1, row, 10).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                row++;
            }

            // Headers de Seção (Portuárias e Outras)
            ws.Range("A25:J25").Merge();
            SetVal(ws, "A25", "DESPESAS PORTUÁRIAS", "Arial", 10, true);
            ws.Cell("A25").Style.Fill.BackgroundColor = XLColor.FromHtml("#D9D9D9");
            ws.Range("A25:J25").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            ws.Range("A29:J29").Merge();
            SetVal(ws, "A29", "OUTRAS DESPESAS", "Arial", 10, true);
            ws.Cell("A29").Style.Fill.BackgroundColor = XLColor.FromHtml("#D9D9D9");
            ws.Range("A29:J29").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            // --- LÓGICA DE DESPESAS DINÂMICAS ---
            var despesas = ObterDespesas(f);
            var despesasLayout = new List<(int Row, string Label, string Chave)> {
                (26, " Agência n°", "Agência"), (27, " Agência n°", "Agência"), (28, " Armazenagem nº", "Armazenagem"),
                (30, " Frete Marítimo nº", "Frete Marítimo"), (31, " Marinha Mercante nº", "Marinha Mercante"), (32, " GRU ANVISA nº", "GRU ANVISA"),
                (33, " LI Cancelada/ Indeferida nº", "LI Cancelada/ Indeferida"), (34, " Expediente LI Cancelada nº", "Expediente LI Cancelada"),
                (35, " Encaminhamento Amostras nº", "Encaminhamento Amostras"), (36, " Darf Anvisa nº", "Darf Anvisa"), (37, " Motoboy nº", "Motoboy"),
                (38, " L.I.", "L.I."), (39, " Expediente", "Expediente"), (40, " Desp. Desembaraço nº", "Despesas com Desembaraço"), // Alterado aqui
                (41, " HD", "HD"), (42, " Cartorio", "Cartório")
            };

            var despesasUsadas = new HashSet<int>();

            foreach (var itemLayout in despesasLayout)
            {
                bool encontrou = false;
                for (int i = 0; i < despesas.Count; i++)
                {
                    if (despesas[i].Nome == itemLayout.Chave && !despesasUsadas.Contains(i))
                    {
                        ws.Range($"B{itemLayout.Row}:I{itemLayout.Row}").Merge();
                        ws.Cell($"A{itemLayout.Row}").Value = itemLayout.Label;
                        ws.Cell($"B{itemLayout.Row}").Value = despesas[i].Numero;
                        SetMoney(ws, itemLayout.Row, 10, despesas[i].Valor);

                        despesasUsadas.Add(i);
                        encontrou = true;
                        break;
                    }
                }
                if (!encontrou)
                {
                    ws.Cell($"A{itemLayout.Row}").Value = itemLayout.Label;
                    ws.Range($"B{itemLayout.Row}:I{itemLayout.Row}").Merge();
                    SetMoney(ws, itemLayout.Row, 10, 0);
                }
                // Borda em cada linha de despesa
                ws.Range(itemLayout.Row, 1, itemLayout.Row, 10).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            }

            // --- DOCUMENTOS ANEXOS ---
            ws.Range("A43:J43").Merge();
            SetVal(ws, "A43", "DOCUMENTOS ANEXOS", "Arial", 10, true);
            ws.Cell("A43").Style.Fill.BackgroundColor = XLColor.FromHtml("#D9D9D9");
            ws.Range("A43:J43").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            int docRow = 44;
            int anexosCount = Math.Min(f.NomesDocumentosAnexos.Length, f.NumeroDocumentosAnexos.Length);

            for (int i = 0; i < anexosCount; i++)
            {
                ws.Cell(docRow, 1).Value = f.NumeroDocumentosAnexos[i];
                ws.Cell(docRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell(docRow, 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                ws.Range(docRow, 2, docRow, 10).Merge().Value = f.NomesDocumentosAnexos[i];
                ws.Range(docRow, 2, docRow, 10).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Range(docRow, 2, docRow, 10).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                // Bordas internas
                ws.Cell(docRow, 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws.Range(docRow, 2, docRow, 10).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                docRow++;
            }

            // --- TOTAIS E SALDO ---
            ws.Range("A53:E53").Merge();
            SetVal(ws, "A53", "NÃO VALE COMO RECIBO", "Arial", 10, true);

            SetTotalRow(ws, 44, "Total de Despesas", f.TotalDespesas);
            SetTotalRow(ws, 45, "N/Comissão", f.NComissao);
            SetTotalRow(ws, 46, "Sub-Total  DÉBITO", f.SubTotal);
            SetTotalRow(ws, 47, "S/Adiantamento (CRÉDITO)", f.Adiantamento);
            SetTotalRow(ws, 48, "Total", f.Saldo);

            ws.Cell("F53").Value = "SALDO";
            ws.Cell("F53").Style.Font.Bold = true;
            ws.Cell("G53").Value = f.TipoFinalizacao;
            SetMoney(ws, 53, 10, f.Saldo);
            ws.Cell(53, 10).Style.Font.Bold = true;

            // --- RODAPÉ ---
            ws.Range("A54:J54").Merge(); ws.Range("A55:J55").Merge();
            ws.Range("A57:J57").Merge(); ws.Range("A58:J58").Merge();

            SetVal(ws, "A54", "Matriz: Rua Comendador Martins nº 55 Altos - Sala 22 - Vila Mathias - CEP 11015-530 - Santos - S.P.", "Arial", 10, true);
            SetVal(ws, "A55", " Fone: (13)3222.8899 - 2202.8369  - e-mail: josecarlos@usadespachos.com.br - usa@bignet.com.br", "Arial", 10, true);
            SetVal(ws, "A57", "Filial: Rua Manoel Dono Morgado nº 100 -  CEP 88301-462 - Fazenda – Itajaí - S.C.", "Arial", 10, true);
            SetVal(ws, "A58", "Fone: (47)3045.1439 - 3083.1430  - e-mail: nestor@usadespachos.com.br", "Arial", 10, true);

            // Bordas Externas Grossas
            AplicarBorda(ws, 1, 58, 1, 10, XLBorderStyleValues.Thick);
            AplicarBorda(ws, 53, 53, 6, 10, XLBorderStyleValues.Thick);

            // Grades
            var grades = new[] { "A4:F8", "G4:J7", "A9:J10", "A11:J12", "A14:J15", "A18:J24", "A26:J28", "A30:J42" };
            foreach (var g in grades) AplicarGrade(ws, g);

            AplicarGrade(ws, $"A44:E{Math.Max(52, docRow)}");
            AplicarGrade(ws, $"F43:J{Math.Max(52, docRow)}");

            ws.Range("B17:J17").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            ws.Range("A53:E53").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            wb.SaveAs(caminho);
        }
        #endregion

        #region Geração PDF (iText7)
        private void GerarPdf(string caminho, Fatura f, CnpjResponse? cnpj)
        {
            using var writer = new PdfWriter(caminho);
            using var pdf = new PdfDocument(writer);
            var document = new Document(pdf, PageSize.A4);
            document.SetMargins(20, 20, 20, 20);

            var fontTimes = PdfFontFactory.CreateFont(StandardFonts.TIMES_BOLD);
            var fontArial = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
            var fontArialBold = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

            // Larguras (Col 7 = 20)
            float[] widths = { 30, 24, 14, 7, 5, 9, 20, 20, 6, 13 };
            var table = new Table(UnitValue.CreatePercentArray(widths)).UseAllAvailableWidth();

            table.SetBorder(new SolidBorder(ColorConstants.BLACK, 2.0f));

            var border = new SolidBorder(ColorConstants.BLACK, 0.5f);
            var borderThick = new SolidBorder(ColorConstants.BLACK, 1.5f);

            // 1. TÍTULO / LOGO
            if (File.Exists(_caminhoLogo))
            {
                ImageData imgData = ImageDataFactory.Create(_caminhoLogo);
                Image logo = new Image(imgData);
                logo.ScaleToFit(140, 60);
                logo.SetFixedPosition(1, 30, 770);
                document.Add(logo);
            }

            var cellHeader = new Cell(1, 10).SetBorder(borderThick).SetPadding(1).SetMinHeight(50);
            cellHeader.Add(new Paragraph("U.S.A").SetFont(fontTimes).SetFontSize(24).SetTextAlignment(TextAlignment.CENTER));
            cellHeader.Add(new Paragraph("Despachos Aduaneiros Ltda.").SetFont(fontTimes).SetFontSize(18).SetTextAlignment(TextAlignment.CENTER));
            table.AddCell(cellHeader);

            // --- CLIENTE E FATURA ---
            // --- CLIENTE E FATURA ---
            string dataHoje = $"Santos, {DateTime.Now:dd 'de' MMMM 'de' yyyy}";

            // 1. CÉLULA CLIENTE (Lado Esquerdo)
            // Agora começa na linha 4 (junto com a data) e vai até a 8. Total Rowspan = 5.
            var cellClient = new Cell(5, 6).SetBorder(border).SetPadding(1).SetVerticalAlignment(VerticalAlignment.TOP);

            string razao = cnpj?.razao_social ?? f.Importador;
            string end1 = cnpj != null ? $"{cnpj.logradouro}, {cnpj.numero} {cnpj.complemento}".Trim() : f.Endereco_Importador;
            string end2 = cnpj != null ? $"{cnpj.bairro} - {cnpj.municipio} - {cnpj.uf} - CEP: {cnpj.cep}" : "";

            cellClient.Add(new Paragraph(razao).SetFont(fontArialBold).SetFontSize(8));
            cellClient.Add(new Paragraph(end1).SetFont(fontArialBold).SetFontSize(8));
            cellClient.Add(new Paragraph(end2).SetFont(fontArialBold).SetFontSize(8));
            table.AddCell(cellClient); // Adiciona primeiro (Colunas A-F)

            // 2. LADO DIREITO (Linhas 4 a 8)

            // Row 4 Right: Data (Colunas G-J)
            table.AddCell(new Cell(1, 4).Add(new Paragraph(dataHoje).SetFont(fontArial).SetFontSize(8).SetTextAlignment(TextAlignment.CENTER)).SetBorder(border));

            // Row 5 Right: Ref
            table.AddCell(new Cell(1, 1).Add(new Paragraph("Fatura N/Ref.:").SetFont(fontArial).SetFontSize(8)).SetBorder(border).SetPadding(1));
            table.AddCell(new Cell(1, 3).Add(new Paragraph(f.Ref_USA).SetFont(fontArial).SetFontSize(8)).SetBorder(border).SetPadding(1));

            // Row 6 Right: FLO
            table.AddCell(new Cell(1, 1).Add(new Paragraph("FLO.:").SetFont(fontArial).SetFontSize(8)).SetBorder(border).SetPadding(1));
            table.AddCell(new Cell(1, 3).Add(new Paragraph(f.FLO).SetFont(fontArial).SetFontSize(8)).SetBorder(border).SetPadding(1));

            // Row 7 Right: SR
            table.AddCell(new Cell(1, 1).Add(new Paragraph("S/Ref.:").SetFont(fontArial).SetFontSize(8)).SetBorder(border).SetPadding(1));
            table.AddCell(new Cell(1, 3).Add(new Paragraph(f.SR).SetFont(fontArial).SetFontSize(8)).SetBorder(border).SetPadding(1));

            // Row 8 Right: Importação
            // Nota: O lado esquerdo (A-F) desta linha já está ocupado pelo cellClient (rowspan 5), então só adicionamos a direita.
            table.AddCell(new Cell(1, 4).Add(new Paragraph("Importação").SetFont(fontArialBold).SetFontSize(12).SetTextAlignment(TextAlignment.CENTER)).SetBorder(border).SetPadding(1));

            // --- VEÍCULO (Centralizado) ---
            var cellVeiculo = new Cell(1, 10).SetBorder(border).SetPadding(1);
            cellVeiculo.Add(new Paragraph("Despesas efetuadas, pôr sua ordem e conta, com as mercadorias abaixo discriminadas, expedidas pelo veículo")
                .SetFont(fontArial).SetFontSize(8).SetTextAlignment(TextAlignment.CENTER));
            cellVeiculo.Add(new Paragraph($"{f.Veiculo} atracado em {DataStr(f.DataAtracacao)}")
                .SetFont(fontArialBold).SetFontSize(8).SetTextAlignment(TextAlignment.CENTER));
            table.AddCell(cellVeiculo);

            // --- MERCADORIA ---
            table.AddCell(CellTxtCenter(1, "MARCA", fontArial, border));
            table.AddCell(CellTxtCenter(4, "QUANTIDADE", fontArial, border));
            table.AddCell(CellTxtCenter(5, "MERCADORIA", fontArial, border));
            table.AddCell(CellTxtCenter(1, f.Marca, fontArial, border));
            table.AddCell(CellTxtCenter(4, f.Quantidade.ToString(), fontArial, border));
            table.AddCell(CellTxtCenter(5, f.Mercadoria, fontArial, border));

            // --- ROW VAZIA COM BORDA ---
            table.AddCell(new Cell(1, 10).SetBorder(border).SetHeight(2));

            // --- VALORES ---
            table.AddCell(CellTxtCenter(3, "VALORES RECEBIDOS EM R$", fontArial, border));
            table.AddCell(CellTxtCenter(2, "DATA", fontArial, border));
            table.AddCell(CellTxtCenter(5, "LANÇAMENTOS", fontArial, border));
            table.AddCell(CellTxtCenter(3, f.ValRecebidos.ToString("N2"), fontArial, border));
            table.AddCell(CellTxtCenter(2, DataStr(f.DataRecebimento), fontArial, border));
            table.AddCell(CellTxtCenter(5, "VALORES PARA DESEMBARAÇO", fontArial, border));

            // DI
            var cellDi = new Cell(1, 10).SetBorder(border).SetPadding(1)
                .Add(new Paragraph($"   Decl. De Importação nº {f.DI} de {DataStr(f.DAtaDI)}").SetFont(fontArial).SetFontSize(8).SetTextAlignment(TextAlignment.CENTER));
            table.AddCell(cellDi);

            // IMPOSTOS
            table.AddCell(CellHeaderGray("DESPESAS ADUANEIRAS", 10, fontArialBold, border));
            AddTaxRow(table, "Imposto de Importação", f.ImpostoImportacao, fontArial, border);
            AddTaxRow(table, "I.P.I.", f.IPI, fontArial, border);
            AddTaxRow(table, "DI/ADIÇÃO", f.DI_ADICAO, fontArial, border);
            AddTaxRow(table, "PIS/PASEP", f.PIS_PASEP, fontArial, border);
            AddTaxRow(table, "COFINS", f.COFINS, fontArial, border);
            AddTaxRow(table, "MULTA LI", f.MULTA_LI, fontArial, border);
            AddTaxRow(table, "ICMS", f.ICMS, fontArial, border);

            // DESPESAS
            table.AddCell(CellHeaderGray("DESPESAS PORTUÁRIAS", 10, fontArialBold, border));
            var despesas = ObterDespesas(f);
            var despesasLayout = new List<(string Label, string Chave)> {
                (" Agência n°", "Agência"), (" Agência n°", "Agência"), (" Armazenagem nº", "Armazenagem"),
                (" Frete Marítimo nº", "Frete Marítimo"), (" Marinha Mercante nº", "Marinha Mercante"), (" GRU ANVISA nº", "GRU ANVISA"),
                (" LI Cancelada/ Indeferida nº", "LI Cancelada/ Indeferida"), (" Expediente LI Cancelada nº", "Expediente LI Cancelada"),
                (" Encaminhamento Amostras nº", "Encaminhamento Amostras"), (" Darf Anvisa nº", "Darf Anvisa"), (" Motoboy nº", "Motoboy"),
                (" L.I.", "L.I."), (" Expediente", "Expediente"), (" Desp. Desembaraço nº", "Despesas com Desembaraço"),
                (" HD", "HD"), (" Cartorio", "Cartório")
            };

            var despesasUsadas = new HashSet<int>();
            for (int k = 0; k < 3; k++) AddExpenseLayout(table, despesasLayout[k], despesas, despesasUsadas, fontArial, border);

            table.AddCell(CellHeaderGray("OUTRAS DESPESAS", 10, fontArialBold, border));
            for (int k = 3; k < despesasLayout.Count; k++) AddExpenseLayout(table, despesasLayout[k], despesas, despesasUsadas, fontArial, border);

            // --- TOTAIS ---
            table.AddCell(CellHeaderGray("DOCUMENTOS ANEXOS", 10, fontArialBold, border));

            var totaisData = new List<(string, decimal)> {
                ("Total de Despesas", f.TotalDespesas),
                ("N/Comissão", f.NComissao),
                ("Sub-Total  DÉBITO", f.SubTotal),
                ("S/Adiantamento (CRÉDITO)", f.Adiantamento),
                ("Total", f.Saldo)
            };

            int maxRows = Math.Max(f.NomesDocumentosAnexos.Length, 5);
            for (int i = 0; i < maxRows; i++)
            {
                // DOCUMENTOS (Lado Esquerdo)
                if (i < f.NomesDocumentosAnexos.Length)
                {
                    string num = i < f.NumeroDocumentosAnexos.Length ? f.NumeroDocumentosAnexos[i] : "";
                    string nome = f.NomesDocumentosAnexos[i];

                    // Col A: Numero (Center)
                    table.AddCell(new Cell(1, 1).Add(new Paragraph(num).SetFont(fontArial).SetFontSize(8).SetTextAlignment(TextAlignment.CENTER)).SetBorder(border).SetPadding(1));

                    // Col B-E: Nome (Left) - CORRIGIDO: Colspan 4 (B,C,D,E)
                    table.AddCell(new Cell(1, 4).Add(new Paragraph(nome).SetFont(fontArial).SetFontSize(8).SetTextAlignment(TextAlignment.LEFT)).SetBorder(border).SetPadding(1));
                }
                else
                {
                    // Células vazias
                    table.AddCell(new Cell(1, 1).SetBorder(border).SetHeight(8));
                    table.AddCell(new Cell(1, 4).SetBorder(border).SetHeight(8));
                }

                // TOTAIS (Lado Direito F-J)
                if (i < totaisData.Count)
                {
                    var item = totaisData[i];
                    // Se for a linha "Total" (índice 4), usa negrito? O Python usa negrito no "Total Geral Label"? Sim (font_arial_10_b).
                    // Mas no Python "Total Geral" é a linha 48. Aqui estamos iterando.

                    // Col F-I (4 cols): Label
                    table.AddCell(new Cell(1, 4).Add(new Paragraph(item.Item1).SetFont(fontArial).SetFontSize(8)).SetTextAlignment(TextAlignment.RIGHT).SetBorder(border).SetPadding(1));
                    // Col J (1 col): Valor
                    table.AddCell(new Cell(1, 1).Add(new Paragraph(item.Item2.ToString("N2")).SetFont(fontArial).SetFontSize(8)).SetTextAlignment(TextAlignment.RIGHT).SetBorder(border).SetPadding(1));
                }
                else
                {
                    table.AddCell(new Cell(1, 4).SetBorder(border));
                    table.AddCell(new Cell(1, 1).SetBorder(border));
                }
            }

            // SALDO FINAL
            // SALDO FINAL
            // Col 1-5 (A-E): Aviso
            table.AddCell(new Cell(1, 5).Add(new Paragraph("NÃO VALE COMO RECIBO").SetFont(fontArialBold).SetFontSize(8)).SetBorder(border).SetPadding(1));

            table.AddCell(new Cell(1, 3).Add(new Paragraph(f.TipoFinalizacao).SetFont(fontArial).SetFontSize(8)).SetBorder(borderThick).SetTextAlignment(TextAlignment.CENTER).SetPadding(1));
            // Col 6 (F): Label SALDO
            table.AddCell(new Cell(1, 1).Add(new Paragraph("SALDO").SetFont(fontArialBold).SetFontSize(8)).SetBorder(borderThick).SetTextAlignment(TextAlignment.CENTER).SetPadding(1));

            // Col 7-9 (G-I): Valor do Saldo (3 colunas de espaço)
            table.AddCell(new Cell(1, 1).Add(new Paragraph(f.Saldo.ToString("N2")).SetFont(fontArialBold).SetFontSize(8)).SetBorder(borderThick).SetTextAlignment(TextAlignment.RIGHT).SetPadding(1));

            // Col 10 (J): Tipo (N/Favor ou S/Favor) - Reduzido para 1 coluna

            // FOOTER (TEXTOS ATUALIZADOS)
            var cellFooter = new Cell(1, 10).SetBorder(borderThick).SetPadding(1).SetMarginTop(5);
            cellFooter.Add(new Paragraph("Matriz: Rua Comendador Martins nº 55 Altos - Sala 22 - Vila Mathias - CEP 11015-530 - Santos - S.P.").SetFont(fontArialBold).SetFontSize(7).SetTextAlignment(TextAlignment.CENTER));
            cellFooter.Add(new Paragraph(" Fone: (13)3222.8899 - 2202.8369  - e-mail: josecarlos@usadespachos.com.br - usa@bignet.com.br").SetFont(fontArialBold).SetFontSize(7).SetTextAlignment(TextAlignment.CENTER));

            // Quebra de linha visual (Espaço) ou apenas sequência
            cellFooter.Add(new Paragraph("Filial: Rua Manoel Dono Morgado nº 100 -  CEP 88301-462 - Fazenda – Itajaí - S.C.").SetFont(fontArialBold).SetFontSize(7).SetTextAlignment(TextAlignment.CENTER));
            cellFooter.Add(new Paragraph("Fone: (47)3045.1439 - 3083.1430  - e-mail: nestor@usadespachos.com.br").SetFont(fontArialBold).SetFontSize(7).SetTextAlignment(TextAlignment.CENTER));

            table.AddCell(cellFooter);

            document.Add(table);
            document.Close();
        }

        // Helpers PDF
        private void AddInfoRow(Table t, string label, string val, PdfFont f) { t.AddCell(new Cell().Add(new Paragraph(label).SetFont(f).SetFontSize(8)).SetBorder(Border.NO_BORDER)); t.AddCell(new Cell().Add(new Paragraph(val).SetFont(f).SetFontSize(8)).SetBorder(Border.NO_BORDER).SetTextAlignment(TextAlignment.RIGHT)); }

        private Cell CellTxt(int colspan, string txt, PdfFont font, Border b) => new Cell(1, colspan).Add(new Paragraph(txt).SetFont(font).SetFontSize(8)).SetBorder(b).SetPadding(1).SetVerticalAlignment(VerticalAlignment.MIDDLE);

        private Cell CellTxtCenter(int colspan, string txt, PdfFont font, Border b) =>
            new Cell(1, colspan)
                .Add(new Paragraph(txt).SetFont(font).SetFontSize(8).SetTextAlignment(TextAlignment.CENTER))
                .SetBorder(b).SetPadding(1)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE);

        private Cell CellHeaderGray(string txt, int colspan, PdfFont font, Border b) => new Cell(1, colspan).Add(new Paragraph(txt).SetFont(font).SetFontSize(8)).SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetBorder(b).SetPadding(1);
        private void AddTaxRow(Table t, string label, decimal val, PdfFont font, Border b)
        {
            // Célula do Nome (Header do nome)
            t.AddCell(new Cell(1, 9)
                .Add(new Paragraph(label).SetFont(font).SetFontSize(8))
                .SetBorder(b) // <--- Borda aqui
                .SetPadding(1));

            // Célula do Valor
            t.AddCell(new Cell(1, 1)
                .Add(new Paragraph(val.ToString("N2")).SetFont(font).SetFontSize(8))
                .SetTextAlignment(TextAlignment.RIGHT)
                .SetBorder(b) // <--- Borda aqui
                .SetPadding(1));
        }
        private void AddExpenseLayout(Table t, (string Label, string Chave) itemLayout, List<DespesaItem> despesas, HashSet<int> usados, PdfFont font, Border b)
        {
            bool encontrou = false;
            for (int i = 0; i < despesas.Count; i++)
            {
                if (despesas[i].Nome == itemLayout.Chave && !usados.Contains(i))
                {
                    t.AddCell(new Cell(1, 1).Add(new Paragraph(itemLayout.Label).SetFont(font).SetFontSize(8)).SetBorder(b).SetPadding(1));
                    t.AddCell(new Cell(1, 8).Add(new Paragraph(despesas[i].Numero).SetFont(font).SetFontSize(8)).SetBorder(b).SetPadding(1));
                    t.AddCell(new Cell(1, 1).Add(new Paragraph(despesas[i].Valor.ToString("N2")).SetFont(font).SetFontSize(8)).SetTextAlignment(TextAlignment.RIGHT).SetBorder(b).SetPadding(1));
                    usados.Add(i);
                    encontrou = true;
                    break;
                }
            }
            if (!encontrou)
            {
                t.AddCell(new Cell(1, 1).Add(new Paragraph(itemLayout.Label).SetFont(font).SetFontSize(8)).SetBorder(b).SetPadding(1));
                t.AddCell(new Cell(1, 8).SetBorder(b).SetPadding(1));
                t.AddCell(new Cell(1, 1).Add(new Paragraph("0,00").SetFont(font).SetFontSize(8)).SetTextAlignment(TextAlignment.RIGHT).SetBorder(b).SetPadding(1));
            }
        }

        private string DataStr(DateTime? d) => d?.ToString("dd/MM/yyyy") ?? "";

        // Helpers Excel
        private void SetVal(IXLWorksheet ws, string cell, string val, string font, double size, bool bold, XLAlignmentHorizontalValues align = XLAlignmentHorizontalValues.Center) { ws.Cell(cell).Value = val; ws.Cell(cell).Style.Font.FontName = font; ws.Cell(cell).Style.Font.FontSize = size; ws.Cell(cell).Style.Font.Bold = bold; ws.Cell(cell).Style.Alignment.Horizontal = align; ws.Cell(cell).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center; }
        private void SetMoney(IXLWorksheet ws, int r, int c, decimal val) { ws.Cell(r, c).Value = val; ws.Cell(r, c).Style.NumberFormat.Format = "\"R$ \"#,##0.00"; ws.Cell(r, c).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right; }
        private void SetTotalRow(IXLWorksheet ws, int r, string label, decimal val) { ws.Range(r, 6, r, 9).Merge(); ws.Cell(r, 6).Value = label; SetMoney(ws, r, 10, val); }
        private void AplicarBorda(IXLWorksheet ws, int r1, int r2, int c1, int c2, XLBorderStyleValues style) { ws.Range(r1, c1, r2, c2).Style.Border.OutsideBorder = style; }
        private void AplicarGrade(IXLWorksheet ws, string range) { ws.Range(range).Style.Border.OutsideBorder = XLBorderStyleValues.Thin; ws.Range(range).Style.Border.InsideBorder = XLBorderStyleValues.Thin; }
        #endregion
    }
}
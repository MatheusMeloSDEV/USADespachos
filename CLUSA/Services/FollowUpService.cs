using ClosedXML.Excel;
using CLUSA.Models;
using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
// Bibliotecas do iText7
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
    public class FollowUpService
    {
        // --- CONFIGURAÇÕES ---
        private const string Colecao = "PROCESSO";

        private readonly string _pastaDestino = @"C:\UsaDespachos\Docs\FollowUp";
        private readonly string _caminhoLogo = @"C:\UsaDespachos\Exportador\logo.png";

        public FollowUpService()
        {
            if (!Directory.Exists(_pastaDestino))
                Directory.CreateDirectory(_pastaDestino);
        }

        public async Task<string> GerarRelatoriosAsync(string nomeImportador)
        {
            var todosDados = await BuscarDadosOrdenadosAsync(nomeImportador);

            // SEPARAÇÃO DAS LISTAS (Normal vs Finalizado)
            var ativos = todosDados
                .Where(p => !string.Equals(p.Status, "Finalizado", StringComparison.OrdinalIgnoreCase))
                .ToList();

            var finalizados = todosDados
                .Where(p => string.Equals(p.Status, "Finalizado", StringComparison.OrdinalIgnoreCase))
                .ToList();

            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            // Correção do Path.GetInvalidFileNameChars
            string nomeLimpo = string.Join("_", nomeImportador.Split(System.IO.Path.GetInvalidFileNameChars())).Replace(" ", "_");
            string nomeBase = $"{nomeLimpo}_{timestamp}";

            // 1. Gera Excel (Com duas abas)
            string caminhoExcel = System.IO.Path.Combine(_pastaDestino, $"{nomeBase}.xlsx");
            GerarExcel(caminhoExcel, ativos, finalizados, nomeImportador);

            // 2. Gera PDF (Com duas seções)
            string caminhoPdf = System.IO.Path.Combine(_pastaDestino, $"{nomeBase}.pdf");
            GerarPdf(caminhoPdf, ativos, finalizados, nomeImportador);

            return caminhoPdf;
        }

        private async Task<List<Processo>> BuscarDadosOrdenadosAsync(string nomeImportador)
        {
            var db = CLUSA.Repositories.ConfigDatabase.GetDatabase();
            var collection = db.GetCollection<Processo>(Colecao);

            var filter = Builders<Processo>.Filter.Eq(p => p.Importador, nomeImportador);
            var lista = await collection.Find(filter).ToListAsync();

            if (lista.Count == 0)
                throw new Exception($"Nenhum processo encontrado para: {nomeImportador}");

            return lista
                .OrderByDescending(p => p.DataDeAtracacao.HasValue)
                .ThenBy(p => p.DataDeAtracacao)
                .ToList();
        }

        // --- GERAÇÃO DO PDF ---
        // --- GERAÇÃO DO PDF ---
        private void GerarPdf(string caminhoArquivo, List<Processo> ativos, List<Processo> finalizados, string nomeImportador)
        {
            using var writer = new PdfWriter(caminhoArquivo);
            using var pdf = new PdfDocument(writer);
            var document = new Document(pdf, PageSize.A3.Rotate());
            document.SetMargins(20, 20, 20, 20);

            var fontBold = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
            var fontRegular = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
            var fontOblique = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_OBLIQUE);

            // Logo e Título (Igual ao anterior)
            if (File.Exists(_caminhoLogo))
            {
                try
                {
                    ImageData imageData = ImageDataFactory.Create(_caminhoLogo);
                    Image logo = new Image(imageData).ScaleToFit(120, 80);
                    document.Add(logo);
                }
                catch { }
            }

            document.Add(new Paragraph($"Follow-Up: {nomeImportador}")
                .SetFont(fontBold).SetFontSize(18).SetTextAlignment(TextAlignment.CENTER));

            document.Add(new Paragraph($"Gerado em: {DateTime.Now:dd/MM/yyyy HH:mm}")
                .SetFont(fontRegular).SetFontSize(10).SetTextAlignment(TextAlignment.CENTER));
            document.Add(new Paragraph("\n"));

            // 1. TABELA DE ATIVOS
            if (ativos.Any())
            {
                document.Add(new Paragraph("PROCESSOS EM ANDAMENTO")
                    .SetFont(fontBold).SetFontSize(12).SetFontColor(ColorConstants.BLUE));

                var tabelaAtivos = CriarTabelaPdf(ativos, nomeImportador, fontBold, fontRegular, fontOblique);
                document.Add(tabelaAtivos);
            }

            // 2. TABELA DE FINALIZADOS (Com Quebra de Página)
            if (finalizados.Any())
            {
                // SEPARAÇÃO DE PÁGINA: Se já escrevemos ativos antes, pula a página
                if (ativos.Any())
                {
                    document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
                }

                // Agora o título sai no topo da nova página
                document.Add(new Paragraph("PROCESSOS FINALIZADOS")
                    .SetFont(fontBold).SetFontSize(12).SetFontColor(ColorConstants.DARK_GRAY));

                var tabelaFinalizados = CriarTabelaPdf(finalizados, nomeImportador, fontBold, fontRegular, fontOblique);
                document.Add(tabelaFinalizados);
            }

            document.Close();
        }

        private Table CriarTabelaPdf(List<Processo> dados, string nomeImportador, PdfFont fontBold, PdfFont fontRegular, PdfFont fontOblique)
        {
            bool isCasaFlora = nomeImportador.Trim().ToUpper() == "CASA FLORA";

            // Lista de colunas
            var headers = new List<string> {
        "Ref. USA", "Exportador", "Ref. Imp", "Produto", "Free Time",
        "Venc. FT", "Venc. FMA", "Veículo", "Atracação", "Embarque",
        "Docs Recebidos", "Rec. Originais", "DI", "Param. DI", "Status"
    };
            if (isCasaFlora) headers.Insert(3, "FLO");

            // --- DEFINIÇÃO DE LARGURAS ---
            // Para garantir que o cabeçalho alinhe com os dados, definimos pesos fixos para as colunas
            // 1 = Estreito, 2 = Médio, 3 = Largo
            var pesos = new List<float>();

            // Ajuste fino dos pesos conforme o conteúdo típico
            pesos.Add(1.5f); // Ref USA
            pesos.Add(2.5f); // Exportador
            pesos.Add(1.5f); // Ref Imp
            if (isCasaFlora) pesos.Add(1.5f); // FLO
            pesos.Add(3.5f); // Produto (Largo)
            pesos.Add(1.0f); // FT
            pesos.Add(1.5f); // Venc FT
            pesos.Add(1.5f); // Venc FMA
            pesos.Add(2.0f); // Veiculo
            pesos.Add(1.5f); // Atracação
            pesos.Add(1.5f); // Embarque
            pesos.Add(3.0f); // Docs (Largo)
            pesos.Add(1.5f); // Rec Originais
            pesos.Add(1.5f); // DI
            pesos.Add(2.0f); // Param DI
            pesos.Add(1.5f); // Status

            // Converte lista de pesos para array
            float[] larguraColunas = pesos.ToArray();

            // 1. TABELA MESTRA (Container Principal)
            // Ela tem apenas 1 coluna que ocupa 100%
            var masterTable = new Table(1).UseAllAvailableWidth();

            // 2. CABEÇALHO (Criamos uma tabela só para o cabeçalho)
            var headerTable = new Table(UnitValue.CreatePercentArray(larguraColunas)).UseAllAvailableWidth();
            foreach (var h in headers)
            {
                headerTable.AddCell(new Cell().Add(new Paragraph(h).SetFont(fontBold).SetFontSize(9))
                    .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetBorder(Border.NO_BORDER)); // Borda controlada pela célula externa se quiser
            }

            // Adiciona o cabeçalho na Tabela Mestra como Header
            // Isso garante que o cabeçalho repita se a tabela quebrar página (opcional)
            var headerCell = new Cell().Add(headerTable).SetPadding(0).SetBorder(Border.NO_BORDER);
            masterTable.AddHeaderCell(headerCell);

            // 3. DADOS (Cada processo é uma sub-tabela)
            foreach (var item in dados)
            {
                // Cria uma tabela individual para ESTE processo
                // Usa as mesmas larguras do cabeçalho para garantir alinhamento visual
                var itemTable = new Table(UnitValue.CreatePercentArray(larguraColunas)).UseAllAvailableWidth();

                // --- Prepara Dados ---
                var valores = new List<string>();
                string Dt(DateTime? d) => d.HasValue ? d.Value.ToString("dd/MM/yyyy") : "";
                string Str(string s) => s ?? "";

                valores.Add(Str(item.Ref_USA));
                valores.Add(Str(item.Exportador));
                valores.Add(Str(item.SR));
                if (isCasaFlora) valores.Add(Str(item.FLO));
                valores.Add(Str(item.Produto));
                valores.Add(item.FreeTime.ToString());
                valores.Add(Dt(item.VencimentoFreeTime));
                valores.Add(Dt(item.VencimentoFMA));
                valores.Add(Str(item.Veiculo));
                valores.Add(Dt(item.DataDeAtracacao));
                valores.Add(Dt(item.DataEmbarque));
                string docs = item.DocRecebidos != null ? string.Join(", ", item.DocRecebidos) : "";
                valores.Add(docs);
                valores.Add(Dt(item.DataRecebOriginais));
                valores.Add(Str(item.DI));
                valores.Add(Str(item.ParametrizacaoDI));
                valores.Add(Str(item.Status));

                // Adiciona Linha de Cima (Dados)
                foreach (var valor in valores)
                {
                    var cell = new Cell().Add(new Paragraph(valor).SetFont(fontRegular).SetFontSize(8))
                        .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                        .SetBorderTop(Border.NO_BORDER)
                        .SetBorderBottom(new SolidBorder(ColorConstants.BLACK, 0.5f)); // Linha fina separando do histórico
                    itemTable.AddCell(cell);
                }

                // Adiciona Linha de Baixo (Histórico)
                string textoHistorico = Str(item.HistoricoDoProcesso).Replace("\r\n", " | ").Replace("\n", " | ");
                if (string.IsNullOrWhiteSpace(textoHistorico)) textoHistorico = "-";

                var cellHistorico = new Cell(1, headers.Count)
                    .Add(new Paragraph($"Histórico: {textoHistorico}")
                        .SetFont(fontOblique).SetFontSize(11).SetFontColor(ColorConstants.DARK_GRAY))
                    .SetBackgroundColor(new DeviceRgb(250, 250, 250))
                    .SetBorderBottom(Border.NO_BORDER); // Sem borda na sub-tabela, a borda virá do container

                itemTable.AddCell(cellHistorico);

                // --- A MÁGICA DE AGRUPAMENTO ---
                // Criamos uma Célula Container para guardar essa tabela inteira
                var containerCell = new Cell().Add(itemTable);

                // Removemos paddings e bordas internas para parecer uma tabela única
                containerCell.SetPadding(0);
                containerCell.SetBorder(new SolidBorder(ColorConstants.BLACK, 1.5f)); // Borda grossa ao redor do BLOCO inteiro

                // ** ISSO MANTÉM OS DADOS JUNTOS **
                containerCell.SetKeepTogether(true);

                masterTable.AddCell(containerCell);

                // Espaçamento entre processos (opcional, cria uma linha branca)
                // masterTable.AddCell(new Cell().SetHeight(5).SetBorder(Border.NO_BORDER));
            }

            return masterTable;
        }

        private void GerarExcel(string caminhoArquivo, List<Processo> ativos, List<Processo> finalizados, string nomeImportador)
        {
            using var wb = new XLWorkbook();

            // Aba 1: Em Andamento
            if (ativos.Any())
            {
                PreencherAbaExcel(wb, "Em Andamento", ativos, nomeImportador);
            }
            else
            {
                wb.Worksheets.Add("Em Andamento");
            }

            // Aba 2: Finalizados
            if (finalizados.Any())
            {
                PreencherAbaExcel(wb, "Finalizados", finalizados, nomeImportador);
            }

            wb.SaveAs(caminhoArquivo);
        }

        // Método auxiliar para não duplicar código entre as abas
        private void PreencherAbaExcel(XLWorkbook wb, string nomeAba, List<Processo> dados, string nomeImportador)
        {
            var ws = wb.Worksheets.Add(nomeAba);
            bool isCasaFlora = nomeImportador.Trim().ToUpper() == "CASA FLORA";

            // --- Configuração Título/Logo (Igual) ---
            ws.Cell("C1").Value = $"{nomeImportador} - {nomeAba}";
            ws.Cell("C1").Style.Font.FontSize = 20;
            ws.Cell("C1").Style.Font.Bold = true;
            ws.Cell("C1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            if (File.Exists(_caminhoLogo))
            {
                var pic = ws.AddPicture(_caminhoLogo).MoveTo(ws.Cell(1, 1));
                pic.Height = 60; pic.Width = 100;
                ws.Range("C1:H1").Merge();
            }

            var headers = new List<string> {
        "Ref. USA", "Exportador", "Ref. Imp", "Produto", "Free Time",
        "Venc. Free Time", "Venc. FMA", "Veiculo", "Data de Atracação", "Data de Embarque",
        "Documentos Recebidos", "Data Rec. Org.", "Histórico do Processo",
        "DI", "Parametrização DI", "Status"
    };
            if (isCasaFlora) headers.Insert(3, "FLO");

            // Cabeçalhos
            for (int i = 0; i < headers.Count; i++)
            {
                var cell = ws.Cell(2, i + 1);
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.LightGray;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            }

            // --- DADOS ---
            int row = 3;
            foreach (var item in dados)
            {
                int col = 1;
                string Str(string s) => s ?? "";

                ws.Cell(row, col++).Value = Str(item.Ref_USA);
                ws.Cell(row, col++).Value = Str(item.Exportador);
                ws.Cell(row, col++).Value = Str(item.SR);
                if (isCasaFlora) ws.Cell(row, col++).Value = Str(item.FLO);
                ws.Cell(row, col++).Value = Str(item.Produto);
                ws.Cell(row, col++).Value = item.FreeTime;
                ws.Cell(row, col++).Value = item.VencimentoFreeTime;
                ws.Cell(row, col++).Value = item.VencimentoFMA;
                ws.Cell(row, col++).Value = Str(item.Veiculo);
                ws.Cell(row, col++).Value = item.DataDeAtracacao;
                ws.Cell(row, col++).Value = item.DataEmbarque;

                string docs = item.DocRecebidos != null ? string.Join(", ", item.DocRecebidos) : "";
                ws.Cell(row, col++).Value = docs;

                ws.Cell(row, col++).Value = item.DataRecebOriginais;

                // NO EXCEL: Mantemos \n para quebra de linha dentro da célula
                // Não usamos Split aqui porque o Excel entende o \n se o WrapText estiver ligado
                string historico = Str(item.HistoricoDoProcesso).Replace("\r\n", "\n").Trim();
                ws.Cell(row, col++).Value = historico;

                ws.Cell(row, col++).Value = Str(item.DI);
                ws.Cell(row, col++).Value = Str(item.ParametrizacaoDI);
                ws.Cell(row, col++).Value = Str(item.Status);

                row++;
            }

            // --- FORMATAÇÃO VISUAL ---

            // Lista de colunas que devem quebrar linha
            var colunasWrap = new List<string> { "Produto", "Documentos Recebidos", "Histórico do Processo", "Parametrização DI" };

            // Larguras Fixas
            var larguras = new Dictionary<string, double> {
        { "Ref. USA", 15 }, { "Exportador", 30 }, { "Ref. Imp", 15 },
        { "FLO", 15 }, { "Produto", 40 }, { "Free Time", 12 },
        { "Venc. Free Time", 18 }, { "Venc. FMA", 15 }, { "Veiculo", 25 },
        { "Data de Atracação", 18 }, { "Data de Embarque", 18 },
        { "Documentos Recebidos", 45 }, { "Data Rec. Org.", 18 },
        
        // Histórico travado em 60 (não fica gigante para o lado)
        { "Histórico do Processo", 60 },

        { "DI", 18 }, { "Parametrização DI", 30 }, 
        
        // Status largo conforme pedido
        { "Status", 40 }
    };

            for (int i = 0; i < headers.Count; i++)
            {
                var nomeColuna = headers[i];
                var colExcel = ws.Column(i + 1);

                colExcel.Width = larguras.ContainsKey(nomeColuna) ? larguras[nomeColuna] : 20;

                if (colunasWrap.Contains(nomeColuna))
                {
                    colExcel.Style.Alignment.WrapText = true;
                }

                colExcel.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                if (!colunasWrap.Contains(nomeColuna))
                    colExcel.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                else
                    colExcel.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left; // Texto longo fica melhor à esquerda
            }

            // Cria Tabela
            var range = ws.Range(2, 1, row - 1, headers.Count);
            var table = range.CreateTable();
            table.Name = $"Tabela_{Guid.NewGuid().ToString("N").Substring(0, 8)}";
            table.Theme = XLTableTheme.TableStyleMedium9;
            table.ShowAutoFilter = true;

            // Datas
            var colunasData = new List<string> { "Venc. Free Time", "Venc. FMA", "Data de Atracação", "Data de Embarque", "Data Rec. Org." };
            foreach (var h in colunasData)
            {
                int idx = headers.IndexOf(h) + 1;
                if (idx > 0) ws.Column(idx).Style.DateFormat.Format = "dd/MM/yyyy";
            }

            // --- CORREÇÃO FINAL PARA O HISTÓRICO ---
            // Isso força o Excel a calcular a altura necessária para exibir todo o texto que foi quebrado
            // Sem isso, o texto fica cortado visualmente.
            ws.Rows(3, row - 1).AdjustToContents();
        }
    }
}
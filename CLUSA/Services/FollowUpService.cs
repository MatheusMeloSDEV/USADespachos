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
using System.Drawing;   
using System.Drawing.Imaging;

namespace CLUSA.Services
{
    public class FollowUpService
    {
        // --- CONFIGURAÇÕES ---
        private const string Colecao = "PROCESSO";
        private readonly string _pastaDestino;
        public System.Drawing.Image LogoParaRelatorio { get; set; }

        public FollowUpService(System.Drawing.Image logoExterna = null)
        {
            // Se passarmos a logo no construtor, ela já fica salva
            LogoParaRelatorio = logoExterna;

            // Ajuste do Path (Sempre use System.IO.Path para evitar erro com iText)
            if (Directory.Exists(@"C:\UsaDespachos"))
            {
                _pastaDestino = @"C:\UsaDespachos\Docs\FollowUp";
            }
            else
            {
                _pastaDestino = System.IO.Path.Combine(AppContext.BaseDirectory, "Docs", "FollowUp");
            }

            if (!Directory.Exists(_pastaDestino))
                Directory.CreateDirectory(_pastaDestino);
        }
        public async Task ExecutarFluxoAutomaticoAsync(string nomeImportador)
        {
            try
            {
                // 1. Gera os bytes do PDF em memória
                byte[] pdfBytes = await GerarPdfBytesAsync(nomeImportador);

                // 2. Define o nome do arquivo e o assunto
                string nomeArquivo = $"FollowUp_{nomeImportador}_{DateTime.Now:yyyyMMdd}.pdf";
                string assunto = $"Follow-Up Diário - {nomeImportador} - {DateTime.Now:dd/MM/yyyy}";
                string corpo = $"<p>Segue em anexo o Follow-Up atualizado de <b>{nomeImportador}</b>.</p>";

                // 3. Envia o e-mail usando o EmailService
                await EmailService.EnviarFollowUpAsync(assunto, corpo, pdfBytes, nomeArquivo);
            }
            catch (Exception ex)
            {
                // No GitHub Actions, isso aparecerá no log do console
                Console.WriteLine($"Erro no fluxo automático: {ex.Message}");
                throw; // Re-lança para o sistema saber que falhou
            }
        }
        // ===================================================================================
        // CENÁRIO 1: GERAÇÃO EM DISCO (Para uso interno/botão de relatório)
        // Gera Excel (.xlsx) + PDF (.pdf) e salva na pasta C:\UsaDespachos\Docs\FollowUp
        // ===================================================================================
        public async Task<string> GerarArquivosEmDiscoAsync(string nomeImportador)
        {
            // 1. Busca e separa os dados
            var (ativos, finalizados) = await BuscarESepararDadosAsync(nomeImportador);

            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            // Limpa caracteres inválidos do nome do arquivo
            string nomeLimpo = string.Join("_", nomeImportador.Split(System.IO.Path.GetInvalidFileNameChars())).Replace(" ", "_");
            string nomeBase = $"{nomeLimpo}_{timestamp}";

            // 2. Gera Excel no Disco
            string caminhoExcel = System.IO.Path.Combine(_pastaDestino, $"{nomeBase}.xlsx");
            GerarExcel(caminhoExcel, ativos, finalizados, nomeImportador);

            // 3. Gera PDF no Disco
            string caminhoPdf = System.IO.Path.Combine(_pastaDestino, $"{nomeBase}.pdf");

            using (var writer = new PdfWriter(caminhoPdf))
            using (var pdf = new PdfDocument(writer))
            using (var document = new Document(pdf, PageSize.A3.Rotate()))
            {
                // Chama a lógica de desenho compartilhada
                MontarEstruturaPdf(document, ativos, finalizados, nomeImportador);
            }

            // Retorna o caminho do PDF gerado para abrir se quiser
            return caminhoPdf;
        }

        // ===================================================================================
        // CENÁRIO 2: GERAÇÃO EM MEMÓRIA (Para anexar no E-mail)
        // Não salva nada no disco (HD), apenas retorna os bytes do PDF
        // ===================================================================================
        public async Task<byte[]> GerarPdfBytesAsync(string nomeImportador)
        {
            var (ativos, finalizados) = await BuscarESepararDadosAsync(nomeImportador);

            using (var stream = new MemoryStream())
            {
                using (var writer = new PdfWriter(stream))
                using (var pdf = new PdfDocument(writer))
                using (var document = new Document(pdf, PageSize.A3.Rotate()))
                {
                    // Chama a MESMA lógica de desenho (garante que o PDF do e-mail é igual ao do disco)
                    MontarEstruturaPdf(document, ativos, finalizados, nomeImportador);
                }
                return stream.ToArray();
            }
        }

        // ===================================================================================
        // MÉTODOS AUXILIARES DE DADOS
        // ===================================================================================

        private async Task<(List<Processo> Ativos, List<Processo> Finalizados)> BuscarESepararDadosAsync(string nomeImportador)
        {
            var todosDados = await BuscarDadosOrdenadosAsync(nomeImportador);

            var ativos = todosDados
                .Where(p => !string.Equals(p.Status, "Finalizado", StringComparison.OrdinalIgnoreCase))
                .ToList();

            var finalizados = todosDados
                .Where(p => string.Equals(p.Status, "Finalizado", StringComparison.OrdinalIgnoreCase))
                .ToList();

            return (ativos, finalizados);
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

        // ===================================================================================
        // LÓGICA DE GERAÇÃO DO PDF (COMPARTILHADA)
        // ===================================================================================

        private void MontarEstruturaPdf(Document document, List<Processo> ativos, List<Processo> finalizados, string nomeImportador)
        {
            document.SetMargins(20, 20, 20, 20);

            var fontBold = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
            var fontRegular = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
            var fontOblique = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_OBLIQUE);

            // 1. Logo
            if (LogoParaRelatorio != null)
            {
                try
                {
                    using (var ms = new MemoryStream())
                    {
                        LogoParaRelatorio.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        var imageData = iText.IO.Image.ImageDataFactory.Create(ms.ToArray());
                        var logo = new iText.Layout.Element.Image(imageData).ScaleToFit(120, 80);
                        document.Add(logo);
                    }
                }
                catch { }
            }

            // 2. Título e Data
            document.Add(new Paragraph($"Follow-Up: {nomeImportador}")
                .SetFont(fontBold).SetFontSize(18).SetTextAlignment(TextAlignment.CENTER));

            document.Add(new Paragraph($"Gerado em: {DateTime.Now:dd/MM/yyyy HH:mm}")
                .SetFont(fontRegular).SetFontSize(10).SetTextAlignment(TextAlignment.CENTER));
            document.Add(new Paragraph("\n"));

            // 3. Tabela Ativos
            if (ativos.Any())
            {
                document.Add(new Paragraph("PROCESSOS EM ANDAMENTO")
                    .SetFont(fontBold).SetFontSize(12).SetFontColor(ColorConstants.BLUE));

                document.Add(CriarTabelaPdf(ativos, nomeImportador, fontBold, fontRegular, fontOblique));
            }

            // 4. Tabela Finalizados
            if (finalizados.Any())
            {
                // Se já imprimiu ativos, quebra a página
                if (ativos.Any()) document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));

                document.Add(new Paragraph("PROCESSOS FINALIZADOS")
                    .SetFont(fontBold).SetFontSize(12).SetFontColor(ColorConstants.DARK_GRAY));

                document.Add(CriarTabelaPdf(finalizados, nomeImportador, fontBold, fontRegular, fontOblique));
            }
        }

        private Table CriarTabelaPdf(List<Processo> dados, string nomeImportador, PdfFont fontBold, PdfFont fontRegular, PdfFont fontOblique)
        {
            bool isCasaFlora = nomeImportador.Trim().ToUpper() == "CASA FLORA";

            // Definição de Colunas
            var headers = new List<string> {
                "Ref. USA", "Exportador", "Ref. Imp", "Produto", "Free Time",
                "Venc. FT", "Venc. FMA", "Veículo", "Atracação", "Embarque",
                "Docs Recebidos", "Rec. Originais", "DI", "Param. DI", "Status"
            };
            if (isCasaFlora) headers.Insert(4, "FLO");

            // Pesos das colunas (Largura visual)
            var pesos = new List<float> { 1.5f, 2.5f, 1.5f };
            if (isCasaFlora) pesos.Add(1.5f);
            // Restante dos pesos na ordem das colunas
            pesos.AddRange(new[] { 3.5f, 1.0f, 1.5f, 1.5f, 2.0f, 1.5f, 1.5f, 3.0f, 1.5f, 1.5f, 2.0f, 1.5f });

            // Tabela Mestra (Container)
            var masterTable = new Table(1).UseAllAvailableWidth();

            // Tabela de Cabeçalho
            var headerTable = new Table(UnitValue.CreatePercentArray(pesos.ToArray())).UseAllAvailableWidth();
            foreach (var h in headers)
            {
                headerTable.AddCell(new Cell().Add(new Paragraph(h).SetFont(fontBold).SetFontSize(9))
                    .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetBorder(Border.NO_BORDER));
            }
            masterTable.AddHeaderCell(new Cell().Add(headerTable).SetPadding(0).SetBorder(Border.NO_BORDER));

            // Loop dos Dados
            foreach (var item in dados)
            {
                var itemTable = new Table(UnitValue.CreatePercentArray(pesos.ToArray())).UseAllAvailableWidth();
                var valores = new List<string>();

                string Dt(DateTime? d) => d.HasValue ? d.Value.ToString("dd/MM/yyyy") : "";
                string Str(string s) => s ?? "";

                valores.Add(Str(item.Ref_USA));
                valores.Add(Str(item.Exportador));
                valores.Add(Str(item.SR));
                valores.Add(Str(item.Produto));
                if (isCasaFlora) valores.Add(Str(item.FLO));
                valores.Add(item.FreeTime.ToString());
                valores.Add(Dt(item.VencimentoFreeTime));
                valores.Add(Dt(item.VencimentoFMA));
                valores.Add(Str(item.Veiculo));
                valores.Add(Dt(item.DataDeAtracacao));
                valores.Add(Dt(item.DataEmbarque));
                valores.Add(item.DocRecebidos != null ? string.Join(", ", item.DocRecebidos) : "");
                valores.Add(Dt(item.DataRecebOriginais));
                valores.Add(Str(item.DI));
                valores.Add(Str(item.ParametrizacaoDI));
                valores.Add(Str(item.Status));

                // Adiciona linha de dados
                foreach (var valor in valores)
                {
                    itemTable.AddCell(new Cell().Add(new Paragraph(valor).SetFont(fontRegular).SetFontSize(8))
                        .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                        .SetBorderTop(Border.NO_BORDER)
                        .SetBorderBottom(new SolidBorder(ColorConstants.BLACK, 0.5f)));
                }

                // Adiciona linha de Histórico
                string textoHistorico = Str(item.HistoricoDoProcesso).Replace("\r\n", " | ").Replace("\n", " | ");
                if (string.IsNullOrWhiteSpace(textoHistorico)) textoHistorico = "-";

                itemTable.AddCell(new Cell(1, headers.Count)
                    .Add(new Paragraph($"Histórico: {textoHistorico}")
                        .SetFont(fontOblique).SetFontSize(11).SetFontColor(ColorConstants.DARK_GRAY))
                    .SetBackgroundColor(new DeviceRgb(250, 250, 250))
                    .SetBorderBottom(Border.NO_BORDER));

                // Agrupamento Visual (KeepTogether)
                var containerCell = new Cell().Add(itemTable).SetPadding(0).SetBorder(new SolidBorder(ColorConstants.BLACK, 1.5f));
                containerCell.SetKeepTogether(true);
                masterTable.AddCell(containerCell);
            }

            return masterTable;
        }

        // ===================================================================================
        // LÓGICA DE GERAÇÃO DO EXCEL (APENAS DISCO)
        // ===================================================================================

        private void GerarExcel(string caminhoArquivo, List<Processo> ativos, List<Processo> finalizados, string nomeImportador)
        {
            using var wb = new XLWorkbook();

            // Aba 1
            if (ativos.Any())
                PreencherAbaExcel(wb, "Em Andamento", ativos, nomeImportador);
            else
                wb.Worksheets.Add("Em Andamento");

            // Aba 2
            if (finalizados.Any())
                PreencherAbaExcel(wb, "Finalizados", finalizados, nomeImportador);

            wb.SaveAs(caminhoArquivo);
        }

        private void PreencherAbaExcel(XLWorkbook wb, string nomeAba, List<Processo> dados, string nomeImportador)
        {
            var ws = wb.Worksheets.Add(nomeAba);
            bool isCasaFlora = nomeImportador.Trim().ToUpper() == "CASA FLORA";

            // 1. Título e Logo
            ws.Cell("C1").Value = $"{nomeImportador} - {nomeAba}";
            ws.Cell("C1").Style.Font.FontSize = 20;
            ws.Cell("C1").Style.Font.Bold = true;
            ws.Cell("C1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            if (LogoParaRelatorio != null)
            {
                try
                {
                    // Note que aqui NÃO usamos mais Properties.Resources
                    // Usamos direto a variável que você preencheu no Program.cs
                    using (var ms = new MemoryStream())
                    {
                        // Salva a imagem no stream para o Excel conseguir ler
                        LogoParaRelatorio.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

                        var pic = ws.AddPicture(ms).MoveTo(ws.Cell(1, 1));
                        pic.Height = 60;
                        pic.Width = 100;
                    }
                }
                catch{}
            }

            // 2. Definição de Colunas
            var headers = new List<string> {
                "Ref. USA", "Exportador", "Ref. Imp", "Produto", "Free Time",
                "Venc. Free Time", "Venc. FMA", "Veiculo", "Data de Atracação", "Data de Embarque",
                "Documentos Recebidos", "Data Rec. Org.", "Histórico do Processo",
                "DI", "Parametrização DI", "Status"
            };
            if (isCasaFlora) headers.Insert(4, "FLO");

            // 3. Estilização do Cabeçalho
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

            // 4. Preenchimento dos Dados
            int row = 3;
            foreach (var item in dados)
            {
                int col = 1;
                string Str(string s) => s ?? "";

                ws.Cell(row, col++).Value = Str(item.Ref_USA);
                ws.Cell(row, col++).Value = Str(item.Exportador);
                ws.Cell(row, col++).Value = Str(item.SR);
                ws.Cell(row, col++).Value = Str(item.Produto);
                if (isCasaFlora) ws.Cell(row, col++).Value = Str(item.FLO);
                ws.Cell(row, col++).Value = item.FreeTime;
                ws.Cell(row, col++).Value = item.VencimentoFreeTime;
                ws.Cell(row, col++).Value = item.VencimentoFMA;
                ws.Cell(row, col++).Value = Str(item.Veiculo);
                ws.Cell(row, col++).Value = item.DataDeAtracacao;
                ws.Cell(row, col++).Value = item.DataEmbarque;

                string docs = item.DocRecebidos != null ? string.Join(", ", item.DocRecebidos) : "";
                ws.Cell(row, col++).Value = docs;

                ws.Cell(row, col++).Value = item.DataRecebOriginais;

                // Quebra de linha no histórico para ficar legível
                ws.Cell(row, col++).Value = Str(item.HistoricoDoProcesso).Replace("\r\n", "\n").Trim();

                ws.Cell(row, col++).Value = Str(item.DI);
                ws.Cell(row, col++).Value = Str(item.ParametrizacaoDI);
                ws.Cell(row, col++).Value = Str(item.Status);
                row++;
            }

            // 5. Configuração Visual das Colunas (Larguras e WrapText)
            var colunasWrap = new List<string> { "Produto", "Documentos Recebidos", "Histórico do Processo", "Parametrização DI" };

            // Mapeamento Nome -> Largura
            var larguras = new Dictionary<string, double> {
                { "Ref. USA", 15 }, { "Exportador", 30 }, { "Ref. Imp", 15 },
                { "FLO", 15 }, { "Produto", 40 }, { "Free Time", 12 },
                { "Venc. Free Time", 18 }, { "Venc. FMA", 15 }, { "Veiculo", 25 },
                { "Data de Atracação", 18 }, { "Data de Embarque", 18 },
                { "Documentos Recebidos", 45 }, { "Data Rec. Org.", 18 },
                { "Histórico do Processo", 60 },
                { "DI", 18 }, { "Parametrização DI", 30 },
                { "Status", 40 }
            };

            for (int i = 0; i < headers.Count; i++)
            {
                var nomeColuna = headers[i];
                var colExcel = ws.Column(i + 1);

                // Define largura
                colExcel.Width = larguras.ContainsKey(nomeColuna) ? larguras[nomeColuna] : 20;

                // Define WrapText e Alinhamento
                if (colunasWrap.Contains(nomeColuna))
                {
                    colExcel.Style.Alignment.WrapText = true;
                    colExcel.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                }
                else
                {
                    colExcel.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                }
                colExcel.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            }

            // 6. Formatação de Datas
            var colunasData = new List<string> { "Venc. Free Time", "Venc. FMA", "Data de Atracação", "Data de Embarque", "Data Rec. Org." };
            foreach (var h in colunasData)
            {
                int idx = headers.IndexOf(h) + 1;
                if (idx > 0) ws.Column(idx).Style.DateFormat.Format = "dd/MM/yyyy";
            }

            // 7. Tabela Oficial do Excel e Ajuste de Altura
            if (row > 3) // Só cria tabela se tiver dados
            {
                var range = ws.Range(2, 1, row - 1, headers.Count);
                var table = range.CreateTable();
                table.Name = $"Tabela_{Guid.NewGuid().ToString("N").Substring(0, 8)}"; // Nome único
                table.Theme = XLTableTheme.TableStyleMedium9;
                table.ShowAutoFilter = true;
            }

            // Ajusta altura das linhas baseado no conteúdo (vital para o histórico)
            ws.Rows(3, row - 1).AdjustToContents();
        }
    }
}
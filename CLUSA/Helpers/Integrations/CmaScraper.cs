using Microsoft.Playwright;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace CLUSA.Helpers.Integrations
{
    public class CmaScraper : IDisposable
    {
        private IPlaywright _playwright;
        private IBrowser _browser;

        public async Task InitializeAsync()
        {
            if (_playwright != null) return;
            _playwright = await Playwright.CreateAsync();
            _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false,
                Channel = "chrome",
                Args = new[] { "--disable-blink-features=AutomationControlled", "--window-position=-2000,-2000" }
            });
        }

        public async Task<ResultadoRastreioCma> RastrearContainerAsync(string containerId)
        {
            if (_browser == null) await InitializeAsync();

            var context = await _browser.NewContextAsync();
            var page = await context.NewPageAsync();

            try
            {
                // Bloqueia recursos visuais pesados
                await page.RouteAsync("**/*", async route =>
                {
                    var type = route.Request.ResourceType;
                    if (type == "image" || type == "media" || type == "font") await route.AbortAsync();
                    else await route.ContinueAsync();
                });

                await page.GotoAsync("https://www.cma-cgm.com/ebusiness/tracking", new PageGotoOptions { WaitUntil = WaitUntilState.DOMContentLoaded, Timeout = 60000 });

                // Cookies
                try
                {
                    var cookieBtn = page.Locator("#onetrust-accept-btn-handler");
                    if (await cookieBtn.IsVisibleAsync()) await cookieBtn.ClickAsync();
                }
                catch { }

                // Pesquisa
                var inputSelector = ".k-input-inner";
                await page.WaitForSelectorAsync(inputSelector);
                await page.Locator(inputSelector).First.FillAsync(containerId);

                // Botão de busca (ID é mais seguro)
                await page.Locator("#btnTracking").ClickAsync();

                // --- LÓGICA NOVA BASEADA NO SEU HTML ---

                // O seletor da tabela principal (Grid Content)
                var gridSelector = "#gridTrackingDetails .k-grid-content tbody";

                try
                {
                    await page.WaitForSelectorAsync($"{gridSelector} tr.k-master-row", new PageWaitForSelectorOptions { Timeout = 20000 });
                }
                catch
                {
                    return new ResultadoRastreioCma { Container = containerId, Sucesso = false, Erro = "Tabela de resultados não carregou." };
                }

                // Pega a ÚLTIMA linha MASTER
                var lastRow = page.Locator($"{gridSelector} tr.k-master-row").Last;

                // 1. Pega a Data como STRING primeiro
                string dataStringRaw = await SafeGetText(lastRow.Locator(".date"));
                // Limpa quebras de linha que o site traz (ex: "Saturday\n21-FEB...")
                dataStringRaw = dataStringRaw.Replace("\n", " ").Replace("\r", "").Trim();

                // 2. Converte para DateTime (O PULO DO GATO)
                DateTime? dataFinal = null;

                // O formato do site: "Saturday, 21-FEB-2026 07:00 AM"
                // Traduzindo para código: "dddd, dd-MMM-yyyy hh:mm tt"
                if (DateTime.TryParseExact(dataStringRaw,
                    "dddd, dd-MMM-yyyy hh:mm tt",
                    new CultureInfo("en-US"), // Força inglês por causa de "FEB/Saturday"
                    DateTimeStyles.None,
                    out DateTime dataConvertida))
                {
                    dataFinal = dataConvertida;
                }

                // 2. Navio (Coluna .vesselVoyage -> tag 'a')
                string navio = await SafeGetText(lastRow.Locator(".vesselVoyage a").First);

                // 3. Porto (Cidade) (Coluna .location -> span)
                string portoCidade = await SafeGetText(lastRow.Locator(".location span").First);

                // 4. Terminal (O Segredo!)
                string terminalDetalhe = "";
                var scriptTerminal = lastRow.Locator(".location script");

                if (await scriptTerminal.CountAsync() > 0)
                {
                    // OPÇÃO 1: Corrigindo para InnerHTMLAsync (HTML maiúsculo)
                    // terminalDetalhe = await scriptTerminal.InnerHTMLAsync(); 

                    // OPÇÃO 2 (RECOMENDADA): TextContentAsync é mais robusto para ler scripts
                    terminalDetalhe = await scriptTerminal.TextContentAsync();
                }
                // 5. Último Status (Coluna do meio, geralmente a 3ª td ou classe .capsule se existir)
                // No seu HTML, a classe é 'capsule' dentro da td
                string status = await SafeGetText(lastRow.Locator(".capsule"));

                return new ResultadoRastreioCma
                {
                    Container = containerId,
                    Sucesso = true,
                    Navio = navio,
                    PortoDestino = portoCidade,
                    LocalChegada = terminalDetalhe.Trim(), // O nome exato do terminal (BTP, etc)
                    DataChegada = dataFinal,
                    UltimoStatus = status
                };
            }
            catch (Exception ex)
            {
                return new ResultadoRastreioCma { Container = containerId, Sucesso = false, Erro = ex.Message };
            }
            finally
            {
                await page.CloseAsync();
                await context.CloseAsync();
            }
        }

        private async Task<string> SafeGetText(ILocator locator)
        {
            try
            {
                if (await locator.CountAsync() > 0)
                    return (await locator.InnerTextAsync()).Trim();
            }
            catch { }
            return "";
        }

        public void Dispose()
        {
            _browser?.CloseAsync();
            _playwright?.Dispose();
        }
    }

    public class ResultadoRastreioCma
    {
        public string Container { get; set; }
        public bool Sucesso { get; set; }
        public string Erro { get; set; }
        public string Navio { get; set; }
        public string PortoDestino { get; set; }
        public string LocalChegada { get; set; } // Terminal específico
        public DateTime? DataChegada { get; set; }
        public string UltimoStatus { get; set; }
    }
}
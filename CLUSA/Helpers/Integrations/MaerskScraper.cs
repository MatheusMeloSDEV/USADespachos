using System;
using System.Threading.Tasks;
using Microsoft.Playwright;

namespace CLUSA.Helpers.Integrations
{
    public class MaerskScraper : IDisposable
    {
        private IPlaywright _playwright;
        private IBrowser _browser;

        // Inicializa o navegador UMA vez (reaproveitamento = performance)
        public async Task InitializeAsync()
        {
            if (_playwright != null) return;

            _playwright = await Playwright.CreateAsync();

            // TRUQUE DE MESTRE:
            // Headless = false (para o site achar que é humano)
            // Args = joga a janela para a posição -2000 (fora do monitor) para não te atrapalhar
            _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions

            {
                Headless = false,
                Channel = "chrome", // Usa o Chrome real instalado se tiver (menos chance de bloqueio)
                Args = new[] {
                    "--disable-blink-features=AutomationControlled",
                    "--window-position=-2000,-2000"
                }
            });
        }

        public async Task<ResultadoRastreio> RastrearContainerAsync(string containerId)
        {
            // Garante que o browser está aberto
            if (_browser == null) await InitializeAsync();

            var context = await _browser.NewContextAsync(new BrowserNewContextOptions
            {
                ViewportSize = new ViewportSize { Width = 1280, Height = 720 },
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36"
            });

            var page = await context.NewPageAsync();

            try
            {
                // --- OTIMIZAÇÃO MAXIMA ---
                // Bloqueia imagens, fontes e CSS. O site carrega em < 1 segundo.
                await page.RouteAsync("**/*", async route =>
                {
                    var type = route.Request.ResourceType;
                    if (type == "image" || type == "media" || type == "stylesheet" || type == "font")
                        await route.AbortAsync();
                    else
                        await route.ContinueAsync();
                });

                // Atalho: Vai direto para a URL com o container (pula digitação e clicks extras)
                await page.GotoAsync($"https://www.maersk.com/tracking/{containerId}", new PageGotoOptions
                {
                    WaitUntil = WaitUntilState.DOMContentLoaded,
                    Timeout = 30000
                });

                // Tenta fechar o Cookie Banner se aparecer (rápido)
                try
                {
                    await page.ClickAsync("button#onetrust-accept-btn-handler", new PageClickOptions { Timeout = 2000 });
                }
                catch { /* Ignora se não aparecer */ }

                // Espera a lista de resultados aparecer
                var listaSelector = "ul[data-test='transport-plan-list']";
                await page.WaitForSelectorAsync(listaSelector, new PageWaitForSelectorOptions { Timeout = 10000 });

                // Lógica de Extração (Baseada no seu Python)
                var ultimoItem = page.Locator(listaSelector).Locator("li").Last;

                string data = await ultimoItem.Locator("[data-test='milestone-date']").InnerTextAsync();

                string local = "N/A";
                var localEl = ultimoItem.Locator("[data-test='location-name']");
                if (await localEl.CountAsync() > 0) local = await localEl.InnerTextAsync();

                string status = "N/A";
                var statusEl = ultimoItem.Locator("[data-test='milestone'] span").First;
                if (await statusEl.CountAsync() > 0) status = await statusEl.InnerTextAsync();

                return new ResultadoRastreio
                {
                    Container = containerId,
                    DataPrevista = data,
                    Destino = local.Replace("\n", " - "),
                    UltimoStatus = status,
                    Sucesso = true
                };
            }
            catch (Exception ex)
            {
                return new ResultadoRastreio { Container = containerId, Sucesso = false, Erro = ex.Message };
            }
            finally
            {
                await page.CloseAsync();
                await context.CloseAsync();
            }
        }

        public void Dispose()
        {
            _browser?.CloseAsync();
            _playwright?.Dispose();
        }
    }

    // Modelo simples para o retorno
    public class ResultadoRastreio
    {
        public string Container { get; set; }
        public string DataPrevista { get; set; }
        public string Destino { get; set; }
        public string UltimoStatus { get; set; }
        public bool Sucesso { get; set; }
        public string Erro { get; set; }
    }
}
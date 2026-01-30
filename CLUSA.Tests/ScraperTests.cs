using CLUSA.Helpers.Integrations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public class ScraperTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public async Task Deve_Rastrear_Container_E_Mostrar_Json()
        {
            // 1. Arrange
            string containerId = "TTNU1259910"; // Use um container real
            using var scraper = new MaerskScraper();

            // 2. Act
            await scraper.InitializeAsync();
            var resultado = await scraper.RastrearContainerAsync(containerId);

            // 3. Imprimir o Resultado Completo (Isso vai aparecer no link "Output")
            var jsonBonito = JsonSerializer.Serialize(resultado, new JsonSerializerOptions { WriteIndented = true });

            TestContext.WriteLine("--------------------------------------------------");
            TestContext.WriteLine("RESULTADO DA BUSCA:");
            TestContext.WriteLine(jsonBonito);
            TestContext.WriteLine("--------------------------------------------------");

            // 4. Assert (Validações)
            Assert.IsTrue(resultado.Sucesso, $"O scraper falhou: {resultado.Erro}");
            Assert.IsNotNull(resultado.DataPrevista, "Data não pode ser nula");
        }

        [TestMethod]
        public async Task Deve_Retornar_Erro_Para_Container_Inexistente()
        {
            // 1. Arrange
            string containerFalso = "ABCD1234567";
            using var scraper = new MaerskScraper();

            // 2. Act
            await scraper.InitializeAsync();
            var resultado = await scraper.RastrearContainerAsync(containerFalso);

            // 3. Assert
            // Esperamos que ele NÃO ache dados, mas não queremos que o código quebre (Exception)
            // Dependendo da sua lógica, ou Sucesso é false, ou os dados vêm vazios.
            // Ajuste conforme sua regra de negócio.

            // Se sua lógica retorna erro na mensagem quando não acha:
            if (!resultado.Sucesso)
            {
                StringAssert.Contains(resultado.Erro, "não encontrado", "Deveria avisar que não achou");
            }
        }
        [TestMethod]
        public async Task Teste_Visual_CMA_Com_Xpath()
        {
            // 1. Use um container que você sabe que tem histórico longo (já que estamos pegando tr[5])
            string containerId = "AMCU9391146"; // <--- TROQUE PELO SEU CONTAINER DE TESTE

            using var scraper = new CmaScraper();
            await scraper.InitializeAsync();

            // 2. Executa
            var resultado = await scraper.RastrearContainerAsync(containerId);

            // 3. Formata o JSON para leitura
            var json = JsonSerializer.Serialize(resultado, new JsonSerializerOptions { WriteIndented = true });

            // 4. IMPRIME O RESULTADO (Clique em "Output" no Test Explorer após rodar)
            TestContext.WriteLine("==========================================");
            TestContext.WriteLine($"BUSCA PARA: {containerId}");
            TestContext.WriteLine("==========================================");
            TestContext.WriteLine(json);
            TestContext.WriteLine("==========================================");

            // Valida se não deu erro
            Assert.IsTrue(resultado.Sucesso, $"Erro: {resultado.Erro}");
        }
    }
}
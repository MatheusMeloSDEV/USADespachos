using Microsoft.VisualStudio.TestTools.UnitTesting;
using CLUSA.Services;
using CLUSA.Repositories;
using System.Threading.Tasks;
using System.Drawing;

namespace Tests
{
    [TestClass] // Identifica que esta classe contém testes
    public class FollowUpTests
    {
        [TestMethod] // Identifica o método de teste
        public async Task TestarEnvioFluxoCompleto_AmbienteTeste()
        {
            // --- ARRANGE (Preparação) ---

            // 1. Forçamos o banco de dados para o ambiente de teste
            ConfigDatabase.ConfigurarParaTeste();

            // 2. Pegamos a logo do projeto Trabalho (Injeção)
            // Nota: Se o teste não achar Trabalho.Properties, 
            // verifique se adicionou a referência do projeto.
            Image logoTeste = Trabalho.Properties.Resources.FollowUpLogo;

            // 3. Instanciamos o serviço
            var service = new FollowUpService(logoTeste);

            // Defina um cliente que você SABE que tem dados no banco 'testeusa'
            string clienteParaTeste = "LEITESOL";

            // --- ACT (Execução) ---

            // Tentamos executar o fluxo completo (Gerar PDF + Enviar E-mail)
            // Se algo falhar (banco, iText, MailKit), ele vai estourar um erro aqui
            await service.ExecutarFluxoAutomaticoAsync(clienteParaTeste);

            // --- ASSERT (Verificação) ---

            // Se o código chegou até aqui sem dar Exception, o teste passou!
            Assert.IsTrue(true, "O fluxo foi executado até o fim sem erros.");
        }
    }
}

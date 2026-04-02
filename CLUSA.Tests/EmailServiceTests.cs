using CLUSA.Services;

namespace Tests;

[TestClass]
public class EmailServiceTests
{
    [TestMethod]
    public async Task EnviarNotificacaoVencimentoAsync_DeveEnviarComSucesso()
    {
        // Arrange
        string assunto = "[TESTE AUTOMATIZADO MSTest] - Vencimento";
        string corpo = "<h2>Teste de Integração</h2><p>Este e-mail foi disparado via MSTest (Vencimento).</p>";

        // Act & Assert
        // Como o método retorna Task e não devolve um valor, consideramos sucesso se não lançar exceção.
        try
        {
            await EmailService.EnviarNotificacaoVencimentoAsync(assunto, corpo);
        }
        catch (Exception ex)
        {
            Assert.Fail($"O envio falhou e lançou uma exceção: {ex.Message}");
        }
    }

    [TestMethod]
    public async Task EnviarFollowUpTextoAsync_DeveEnviarComSucesso()
    {
        // Arrange
        string assunto = "[TESTE AUTOMATIZADO MSTest] - Follow-Up Texto";
        string corpo = "<h2>Teste de Integração</h2><p>Este e-mail foi disparado via MSTest (Follow-Up sem anexo).</p>";

        // Act & Assert
        try
        {
            await EmailService.EnviarFollowUpTextoAsync(assunto, corpo);
        }
        catch (Exception ex)
        {
            Assert.Fail($"O envio falhou e lançou uma exceção: {ex.Message}");
        }
    }

    [TestMethod]
    public async Task EnviarFollowUpAsync_ComAnexoValido_DeveEnviarComSucesso()
    {
        // Arrange
        string assunto = "[TESTE AUTOMATIZADO MSTest] - Follow-Up Com Anexo";
        string corpo = "<h2>Teste de Integração</h2><p>Este e-mail foi disparado via MSTest contendo um anexo de teste.</p>";

        // Criando um array de bytes falso simulando um arquivo PDF
        byte[] pdfFalsoBytes = System.Text.Encoding.UTF8.GetBytes("Conteudo falso simulando um PDF.");
        string nomeArquivo = "documento_teste_mstest.pdf";

        // Act & Assert
        try
        {
            await EmailService.EnviarFollowUpAsync(assunto, corpo, pdfFalsoBytes, nomeArquivo);
        }
        catch (Exception ex)
        {
            Assert.Fail($"O envio falhou e lançou uma exceção: {ex.Message}");
        }
    }

    [TestMethod]
    public async Task EnviarFollowUpAsync_ComAnexoNulo_DeveLancarArgumentException()
    {
        // Arrange
        string assunto = "Teste Falha Esperada";
        string corpo = "Este e-mail não deve chegar.";
        byte[]? anexoInvalido = null; // Simulando esquecimento do anexo
        string nomeArquivo = "falha.pdf";

        // Act & Assert
        try
        {
            await EmailService.EnviarFollowUpAsync(assunto, corpo, anexoInvalido, nomeArquivo);

            // Se a execução passar da linha acima sem dar erro, o teste deve falhar
            Assert.Fail("A exceção ArgumentException era esperada, mas nenhuma exceção foi lançada.");
        }
        catch (ArgumentException)
        {
            // Sucesso! A exceção correta foi disparada pela sua validação.
        }
        catch (Exception ex)
        {
            // Se lançar outro tipo de erro (ex: erro de conexão), o teste falha
            Assert.Fail($"Era esperada uma ArgumentException, mas foi lançada a exceção: {ex.GetType().Name} - {ex.Message}");
        }
    }
}

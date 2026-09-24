using System;
using System.Threading.Tasks;
using CLUSA.Services;
using FluentAssertions;
using Xunit;

namespace CLUSA.Tests
{
    public class CeAutomacaoServiceTests
    {
        [Fact]
        public void CeAutomacaoService_Instanciacao_DeveCriarComUrlPadrao()
        {
            // Act
            var service = new CeAutomacaoService();

            // Assert
            service.Should().NotBeNull();
        }

        [Fact]
        public async Task VerificarStatusApiAsync_QuandoEndpointOffline_DeveRetornarNuloSemLancarExcecao()
        {
            // Arrange - Usa uma porta fechada para simular offline
            var service = new CeAutomacaoService("http://127.0.0.1:59999");

            // Act
            var status = await service.VerificarStatusApiAsync();

            // Assert
            status.Should().BeNull();
        }

        [Fact]
        public async Task SincronizarProcessosComBancoAsync_QuandoApiOffline_DeveRetornarRelatorioComFalha()
        {
            // Arrange
            var service = new CeAutomacaoService("http://127.0.0.1:59999");

            // Act
            var relatorio = await service.SincronizarProcessosComBancoAsync("TesteUnitario");

            // Assert
            relatorio.Should().NotBeNull();
            relatorio.Sucesso.Should().BeFalse();
            relatorio.Mensagem.Should().Contain("offline");
        }
    }
}
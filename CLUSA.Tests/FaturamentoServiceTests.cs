using System;
using System.Collections.Generic;
using CLUSA.Models;
using FluentAssertions;
using Xunit;

namespace CLUSA.Tests
{
    public class FaturamentoServiceTests
    {
        [Fact]
        public void Fatura_CalculoTotal_DeveSomarItensCorretamente()
        {
            // Arrange
            var fatura = new Fatura
            {
                Ref_USA = "USA-FAT-001",
                Agencias = new List<Agencia>
                {
                    new Agencia { Numero = "AG-01", Custo = 1500.50m },
                    new Agencia { Numero = "AG-02", Custo = 2499.50m }
                }
            };

            // Act
            decimal total = 0;
            foreach (var ag in fatura.Agencias)
            {
                total += ag.Custo;
            }

            // Assert
            total.Should().Be(4000.00m);
        }

        [Fact]
        public void Recibo_Instanciacao_DeveConterValoresPadraoValidos()
        {
            // Arrange & Act
            var recibo = new Recibo
            {
                Ref_USA = "USA-REC-001",
                Importador = "Cliente Teste",
                EmissaoLicenca = 250.00m,
                Expediente = 150.00m,
                HonorariosDespachante = 800.00m,
                Total = 1200.00m
            };

            // Assert
            recibo.Ref_USA.Should().Be("USA-REC-001");
            recibo.Importador.Should().Be("Cliente Teste");
            recibo.Total.Should().Be(1200.00m);
            recibo.Datahoje.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void Recibo_ConstrutorComProcesso_DeveCopiarCampos()
        {
            // Arrange
            var processo = new Processo
            {
                Ref_USA = "USA-PROC-99",
                SR = "SR-123",
                Importador = "Importador X",
                Exportador = "Exportador Y",
                Veiculo = "Navio Alfa",
                Produto = "Mercadoria Z"
            };

            // Act
            var recibo = new Recibo(processo);

            // Assert
            recibo.Ref_USA.Should().Be("USA-PROC-99");
            recibo.SR.Should().Be("SR-123");
            recibo.Importador.Should().Be("Importador X");
            recibo.Exportador.Should().Be("Exportador Y");
            recibo.Veiculo.Should().Be("Navio Alfa");
            recibo.Mercadoria.Should().Be("Mercadoria Z");
        }
    }
}
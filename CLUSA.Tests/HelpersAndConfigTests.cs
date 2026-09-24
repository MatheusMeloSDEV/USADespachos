using System;
using System.Collections.Generic;
using CLUSA.Helpers;
using CLUSA.Models;
using FluentAssertions;
using Xunit;

namespace CLUSA.Tests
{
    public class HelpersAndConfigTests
    {
        [Fact]
        public void DataHelper_CalcularVencimento_QuandoDataValida_DeveSomarDiasCorretamente()
        {
            // Arrange
            var dataBase = new DateTime(2026, 9, 23);
            int dias = 10;

            // Act
            var resultado = DataHelper.CalcularVencimento(dataBase, dias);

            // Assert
            resultado.Should().Be(new DateTime(2026, 10, 3));
        }

        [Fact]
        public void DataHelper_CalcularVencimento_QuandoDataNula_DeveRetornarNulo()
        {
            // Act
            var resultado = DataHelper.CalcularVencimento(null, 15);

            // Assert
            resultado.Should().BeNull();
        }

        [Fact]
        public void Catalogo_Instanciacao_DeveConterOrgaosValidos()
        {
            // Arrange
            var catalogo = new Catalogo
            {
                Mercadoria = "Leite em Pó",
                NCM = "0402.10.10",
                Orgaos = new List<Orgao>
                {
                    new Orgao("MAPA", "CANAL VERDE", DateTime.Today, DateTime.Today, "Sem pendências")
                }
            };

            // Assert
            catalogo.Mercadoria.Should().Be("Leite em Pó");
            catalogo.NCM.Should().Be("0402.10.10");
            catalogo.Orgaos.Should().HaveCount(1);
            catalogo.Orgaos[0].OrgaoId.Should().Be("MAPA");
            catalogo.Orgaos[0].Parametrizacao.Should().Be("CANAL VERDE");
        }

        [Fact]
        public void Vistoria_Instanciacao_DeveConterStatusPadrao()
        {
            // Arrange
            var vistoria = new Vistoria
            {
                Ref_USA = "USA-VIST-01",
                LPCO = "LPCO-998877",
                Status = StatusVistoria.SolicitarDataVistoria,
                ParametrizacaoLPCO = "MAPA"
            };

            // Assert
            vistoria.Ref_USA.Should().Be("USA-VIST-01");
            vistoria.LPCO.Should().Be("LPCO-998877");
            vistoria.Status.Should().Be(StatusVistoria.SolicitarDataVistoria);
            vistoria.ParametrizacaoLPCO.Should().Be("MAPA");
        }

        [Fact]
        public void VistoriaDUIMP_Instanciacao_DeveManterCamposCorretos()
        {
            // Arrange
            var vistoriaDuimp = new VistoriaDUIMP
            {
                Ref_USA = "USA-DUIMP-01",
                DUIMP = "26BR0000001234",
                Status = "Aguardando Chegada",
                OrgaosAnuentesString = "ANVISA, MAPA"
            };

            // Assert
            vistoriaDuimp.Ref_USA.Should().Be("USA-DUIMP-01");
            vistoriaDuimp.DUIMP.Should().Be("26BR0000001234");
            vistoriaDuimp.Status.Should().Be("Aguardando Chegada");
            vistoriaDuimp.OrgaosAnuentesString.Should().Be("ANVISA, MAPA");
        }

        [Fact]
        public void Notificacao_Instanciacao_DeveConterDataCriacao()
        {
            // Arrange
            var notif = new Notificacao
            {
                RefUsa = "USA-NOTIF-01",
                Mensagem = "Alerta de teste",
                Visualizado = false,
                DataCriacao = DateTime.Now
            };

            // Assert
            notif.RefUsa.Should().Be("USA-NOTIF-01");
            notif.Mensagem.Should().Be("Alerta de teste");
            notif.Visualizado.Should().BeFalse();
            notif.DataCriacao.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(5));
        }
    }
}
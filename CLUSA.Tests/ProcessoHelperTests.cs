using System;
using System.Collections.Generic;
using CLUSA.Helpers;
using CLUSA.Models;
using FluentAssertions;
using Xunit;

namespace CLUSA.Tests
{
    public class ProcessoHelperTests
    {
        [Fact]
        public void AtualizarCondicaoProcesso_QuandoPossuiDataRegistroDI_DeveRetornarFinalizado()
        {
            // Arrange
            var processo = new Processo
            {
                Ref_USA = "USA-1001",
                DataRegistroDI = DateTime.Today
            };

            // Act
            ProcessoHelper.AtualizarCondicaoProcesso(processo);

            // Assert
            processo.CondicaoProcesso.Should().Be("Finalizado");
        }

        [Fact]
        public void AtualizarCondicaoProcesso_QuandoDataEmbarquePassouESemDI_DeveRetornarDIDUIMPParaDigitacao()
        {
            // Arrange
            var processo = new Processo
            {
                Ref_USA = "USA-1002",
                DataEmbarque = DateTime.Today.AddDays(-2),
                RascunhoDI = "",
                DI = "",
                Numerario = true
            };

            // Act
            ProcessoHelper.AtualizarCondicaoProcesso(processo);

            // Assert
            processo.CondicaoProcesso.Should().Be("DIDUIMPParaDigitacao");
        }

        [Fact]
        public void AtualizarCondicaoProcesso_QuandoDataEmbarqueDefinidaENumerarioFalso_DeveRetornarSolicitarNumerario()
        {
            // Arrange
            var processo = new Processo
            {
                Ref_USA = "USA-1003",
                DataEmbarque = DateTime.Today.AddDays(5),
                Numerario = false
            };

            // Act
            ProcessoHelper.AtualizarCondicaoProcesso(processo);

            // Assert
            processo.CondicaoProcesso.Should().Be("SolicitarNumerario");
        }

        [Fact]
        public void AtualizarCondicaoProcesso_QuandoPresencaDeCargaAtiva_DeveRetornarAtracadosComPresencaCarga()
        {
            // Arrange
            var processo = new Processo
            {
                Ref_USA = "USA-1004",
                PresencaDeCarga = true,
                Numerario = true
            };

            // Act
            ProcessoHelper.AtualizarCondicaoProcesso(processo);

            // Assert
            processo.CondicaoProcesso.Should().Be("AtracadosComPresencaCarga");
        }

        [Fact]
        public void AtualizarCondicaoProcesso_QuandoAtracadoESemSigVig_DeveRetornarSituacaoSIGVIG()
        {
            // Arrange
            var processo = new Processo
            {
                Ref_USA = "USA-1005",
                DataDeAtracacao = DateTime.Today.AddDays(-1),
                SigVig = false,
                Numerario = true
            };

            // Act
            ProcessoHelper.AtualizarCondicaoProcesso(processo);

            // Assert
            processo.CondicaoProcesso.Should().Be("SituacaoSIGVIG");
        }

        [Fact]
        public void AtualizarCondicaoProcesso_QuandoAtracadoESemPresencaDeCarga_DeveRetornarAtracadosSemPresencaCarga()
        {
            // Arrange
            var processo = new Processo
            {
                Ref_USA = "USA-1006",
                DataDeAtracacao = DateTime.Today.AddDays(-1),
                SigVig = true,
                PresencaDeCarga = false,
                Numerario = true
            };

            // Act
            ProcessoHelper.AtualizarCondicaoProcesso(processo);

            // Assert
            processo.CondicaoProcesso.Should().Be("AtracadosSemPresencaCarga");
        }

        [Fact]
        public void AtualizarCondicaoProcesso_QuandoPossuiRedestinacao_DeveRetornarRedestinados()
        {
            // Arrange
            var processo = new Processo
            {
                Ref_USA = "USA-1007",
                DataDeAtracacao = DateTime.Today.AddDays(5),
                Redestinacao = true,
                Numerario = true
            };

            // Act
            ProcessoHelper.AtualizarCondicaoProcesso(processo);

            // Assert
            processo.CondicaoProcesso.Should().Be("Redestinados");
        }
    }
}
using System;
using System.Collections.Generic;
using CLUSA.Models;
using FluentAssertions;
using Trabalho;
using Xunit;

namespace CLUSA.Tests
{
    public class GridColumnManagerTests
    {
        [Fact]
        public void RegistrarCatalogo_DeveArmazenarERetornarColunasRegistradas()
        {
            // Arrange
            string gridName = "GridTeste_" + Guid.NewGuid().ToString();
            var colunas = new List<DefinicaoColuna>
            {
                new DefinicaoColuna("Ref_USA", "Ref USA", minimumWidth: 100),
                new DefinicaoColuna("Importador", "Importador", minimumWidth: 150)
            };

            // Act
            GridColumnManager.RegistrarCatalogo(gridName, colunas);
            var resultado = GridColumnManager.ObterCatalogo(gridName);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().HaveCount(2);
            resultado[0].NomePropriedade.Should().Be("Ref_USA");
        }

        [Fact]
        public void ObterCatalogo_QuandoNaoExiste_DeveRetornarListaVazia()
        {
            // Act
            var resultado = GridColumnManager.ObterCatalogo("GridInexistente_123");

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEmpty();
        }

        [Fact]
        public void RegistrarCatalogosPadrao_DeveRegistrarGridsPrincipais()
        {
            // Act
            GridColumnManager.RegistrarCatalogosPadrao();
            var gridProcesso = GridColumnManager.ObterCatalogo("DGVAguardandoCE");
            var gridVistorias = GridColumnManager.ObterCatalogo("DGVVistorias");

            // Assert
            gridProcesso.Should().NotBeEmpty();
            gridVistorias.Should().NotBeEmpty();
        }
    }
}
using System;
using System.Collections.Generic;
using CLUSA.Models;
using FluentAssertions;
using MongoDB.Bson;
using Xunit;

namespace CLUSA.Tests
{
    public class ModelsTests
    {
        [Fact]
        public void Processo_OrgaosAnuentesString_DeveRetornarOrgaosDistintosConcatenados()
        {
            // Arrange
            var processo = new Processo
            {
                Ref_USA = "USA-2001",
                LI = new List<LicencaImportacao>
                {
                    new LicencaImportacao
                    {
                        Numero = "LI-001",
                        LPCO = new List<LpcoInfo>
                        {
                            new LpcoInfo { LPCO = "LPCO-1", NomeOrgao = "MAPA" },
                            new LpcoInfo { LPCO = "LPCO-2", NomeOrgao = "ANVISA" }
                        }
                    },
                    new LicencaImportacao
                    {
                        Numero = "LI-002",
                        LPCO = new List<LpcoInfo>
                        {
                            new LpcoInfo { LPCO = "LPCO-3", NomeOrgao = "MAPA" }
                        }
                    }
                }
            };

            // Act
            var resultado = processo.OrgaosAnuentesString;

            // Assert
            resultado.Should().Be("MAPA, ANVISA");
        }

        [Fact]
        public void Processo_OrgaosAnuentesString_QuandoSemLIs_DeveRetornarTraco()
        {
            // Arrange
            var processo = new Processo { Ref_USA = "USA-2002", LI = new List<LicencaImportacao>() };

            // Act
            var resultado = processo.OrgaosAnuentesString;

            // Assert
            resultado.Should().Be("-");
        }

        [Fact]
        public void Capa_Instanciacao_DeveConterValoresPadraoValidos()
        {
            // Arrange & Act
            var capa = new Capa();

            // Assert
            capa.Master.Should().BeEmpty();
            capa.Container.Should().BeEmpty();
            capa.Lancado.Should().BeFalse();
            capa.DANFE.Should().BeFalse();
            capa.Armazenagem.Should().BeFalse();
        }

        [Fact]
        public void Vencimento_Instanciacao_DeveConterPropriedadesValidas()
        {
            // Arrange
            var vencimento = new Vencimento
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Importador = "Empresa Importadora LTDA",
                Cnpjs = new List<string> { "12.345.678/0001-90" },
                Eventos = new List<EventoVencimento>
                {
                    new EventoVencimento
                    {
                        Tag = "Radar",
                        Data = DateTime.Today.AddDays(15)
                    }
                }
            };

            // Assert
            vencimento.Id.Should().NotBeNullOrEmpty();
            vencimento.Importador.Should().Be("Empresa Importadora LTDA");
            vencimento.Eventos.Should().HaveCount(1);
            vencimento.Eventos[0].Data.Should().BeAfter(DateTime.Today);
        }

        [Fact]
        public void User_Propriedades_DevemSerAtribuidasCorretamente()
        {
            // Arrange
            var user = new Users
            {
                Username = "admin_user",
                Password = "hashed_password",
                Admin = true
            };

            // Assert
            user.Username.Should().Be("admin_user");
            user.Password.Should().Be("hashed_password");
            user.Admin.Should().BeTrue();
        }
    }
}
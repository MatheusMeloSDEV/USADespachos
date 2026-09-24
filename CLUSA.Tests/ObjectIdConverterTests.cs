using System;
using FluentAssertions;
using MongoDB.Bson;
using Newtonsoft.Json;
using Trabalho;
using Xunit;

namespace CLUSA.Tests
{
    public class ObjectIdConverterTests
    {
        private class TestClass
        {
            [JsonConverter(typeof(ObjectIdConverter))]
            public ObjectId Id { get; set; }
            public string Name { get; set; } = string.Empty;
        }

        [Fact]
        public void WriteJson_DeveSerializarObjectIdComoString()
        {
            // Arrange
            var objId = ObjectId.GenerateNewId();
            var objeto = new TestClass { Id = objId, Name = "Teste" };

            // Act
            var json = JsonConvert.SerializeObject(objeto);

            // Assert
            json.Should().Contain(objId.ToString());
        }

        [Fact]
        public void ReadJson_DeveDeserializarStringParaObjectIdValido()
        {
            // Arrange
            var originalId = ObjectId.GenerateNewId();
            var json = $"{{\"Id\":\"{originalId}\",\"Name\":\"Teste\"}}";

            // Act
            var objeto = JsonConvert.DeserializeObject<TestClass>(json);

            // Assert
            objeto.Should().NotBeNull();
            objeto!.Id.Should().Be(originalId);
            objeto.Name.Should().Be("Teste");
        }

        [Fact]
        public void ReadJson_QuandoValorInvalido_DeveRetornarObjectIdEmpty()
        {
            // Arrange
            var json = "{\"Id\":null,\"Name\":\"Teste\"}";

            // Act
            var objeto = JsonConvert.DeserializeObject<TestClass>(json);

            // Assert
            objeto.Should().NotBeNull();
            objeto!.Id.Should().Be(ObjectId.Empty);
        }
    }
}
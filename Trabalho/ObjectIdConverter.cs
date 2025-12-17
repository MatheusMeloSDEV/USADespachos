using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MongoDB.Bson;
using System;

namespace Trabalho
{
    public class ObjectIdConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is ObjectId objectId)
            {
                // Escreve o ObjectId como uma string simples (ex: "507f1f77bcf86cd799439011")
                writer.WriteValue(objectId.ToString());
            }
            else
            {
                writer.WriteNull();
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.String)
            {
                // Se não for string, retorna um ObjectId vazio ou lança erro
                return ObjectId.Empty;
            }

            var objectIdString = (string)reader.Value;
            return new ObjectId(objectIdString);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(ObjectId);
        }
    }
}
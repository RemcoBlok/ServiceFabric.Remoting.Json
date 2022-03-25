using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ServiceFabric.Remoting.Json
{
    public class MessageConverter : JsonConverter<object>
    {
        private static readonly JsonSerializerOptions Options = new()
        {
            AllowTrailingCommas = true,
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            ReadCommentHandling = JsonCommentHandling.Skip,
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters =
            {
                new DateOnlyConverter(),
                new DateOnlyNullableConverter()
            }
        };

        public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            if (!reader.Read()
                || reader.TokenType != JsonTokenType.PropertyName
                || reader.GetString() != "$type")
            {
                throw new JsonException();
            }

            if (!reader.Read() || reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException();
            }

            var typeName = reader.GetString();
            var type = Type.GetType(typeName);

            if (!reader.Read() || reader.GetString() != "Value")
            {
                throw new JsonException();
            }

            var message = JsonSerializer.Deserialize(ref reader, type, Options);

            if (!reader.Read() || reader.TokenType != JsonTokenType.EndObject)
            {
                throw new JsonException();
            }

            return message;
        }

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            var type = value.GetType();
            var typeName = type.GetSimpleAssemblyQualifiedName();

            writer.WriteStartObject();
            writer.WriteString("$type", typeName);
            writer.WritePropertyName("Value");
            JsonSerializer.Serialize(writer, value, type, Options);
            writer.WriteEndObject();
        }
    }
}

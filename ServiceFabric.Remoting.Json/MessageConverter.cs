using System.Text.Json;
using System.Text.Json.Serialization;

namespace ServiceFabric.Remoting.Json
{
    public class MessageConverter : JsonConverter<object>
    {
        private static readonly JsonEncodedText TypePropertyName = JsonEncodedText.Encode("$type");
        private static readonly JsonEncodedText ValuePropertyName = JsonEncodedText.Encode("value");

        private static readonly JsonSerializerOptions Options = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters =
            {
                new DateOnlyConverter()
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
                || !reader.ValueTextEquals(TypePropertyName.EncodedUtf8Bytes))
            {
                throw new JsonException();
            }

            if (!reader.Read() || reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException();
            }

            var typeName = reader.GetString();
            var type = TypeCache.GetType(typeName);

            if (!reader.Read()
                || reader.TokenType != JsonTokenType.PropertyName
                || !reader.ValueTextEquals(ValuePropertyName.EncodedUtf8Bytes))
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
            writer.WriteString(TypePropertyName, typeName);
            writer.WritePropertyName(ValuePropertyName);
            JsonSerializer.Serialize(writer, value, type, Options);
            writer.WriteEndObject();
        }
    }
}

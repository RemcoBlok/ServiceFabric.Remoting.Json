using Microsoft.ServiceFabric.Actors;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ServiceFabric.Remoting.Json
{
    public class ActorConverter : JsonConverter<IActor>
    {
        private static readonly JsonSerializerOptions Options = new()
        {
            AllowTrailingCommas = true,
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            ReadCommentHandling = JsonCommentHandling.Skip,
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public override bool CanConvert(Type typeToConvert)
        {
            return typeof(IActor).IsAssignableFrom(typeToConvert);
        }

        public override IActor Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var actorReference = JsonSerializer.Deserialize<ActorReference>(ref reader, Options);
            var actor = (IActor)actorReference.Bind(typeToConvert);
            return actor;
        }

        public override void Write(Utf8JsonWriter writer, IActor value, JsonSerializerOptions options)
        {
            var actorReference = ActorReference.Get(value);
            JsonSerializer.Serialize(writer, actorReference, Options);
        }
    }
}

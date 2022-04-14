using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ServiceFabric.Remoting.Json
{
    public class DateOnlyConverter : JsonConverter<DateOnly>
    {
        private const string DateFormat = "yyyy-MM-dd";

        public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            try
            {
                return DateOnly.ParseExact(reader.GetString(), DateFormat, CultureInfo.InvariantCulture);
            }
            catch (FormatException)
            {
                throw new JsonException();
            }
        }

        public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(DateFormat, CultureInfo.InvariantCulture));
        }
    }
}

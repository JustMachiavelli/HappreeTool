using System.Text.Json;
using System.Text.Json.Serialization;

namespace HappreeTool.Converters
{
    public class DateOnlyConverter : JsonConverter<DateOnly>
    {
        private readonly string _format;

        public DateOnlyConverter(string format = "yyyy-MM-dd")
        {
            _format = format;
        }

        public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            return DateOnly.ParseExact(value!, _format);
        }

        public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(_format));
        }
    }

    public class DateOnlyYMDConverter : DateOnlyConverter
    {
        public DateOnlyYMDConverter() : base("yyyy-MM-dd") { }
    }

    public class DateOnlyYMDNoDashConverter : DateOnlyConverter
    {
        public DateOnlyYMDNoDashConverter() : base("yyyyMMdd") { }
    }

}

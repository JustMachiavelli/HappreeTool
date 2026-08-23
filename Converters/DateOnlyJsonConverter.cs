using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HappreeTool.Converters
{
    public class DateTimeConverter : JsonConverter<DateTime>
    {
        private readonly string _format;

        public DateTimeConverter(string format = "yyyy-MM-dd HH:mm:ss")
        {
            _format = format;
        }

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();

            if (DateTime.TryParseExact(value, _format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
                return result;

            throw new JsonException($"Cannot parse '{value}' as DateTime with format '{_format}'.");
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(_format));
        }
    }

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

            if (DateOnly.TryParseExact(value, _format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
                return result;

            throw new JsonException($"Cannot parse '{value}' as DateOnly with format '{_format}'.");
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
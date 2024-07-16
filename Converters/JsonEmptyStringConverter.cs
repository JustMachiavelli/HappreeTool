using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;

namespace HappreeTool.Converters
{
    /// <summary>
    /// 反序列化json字符串是，自定义转化空字符串
    /// </summary>
    public class JsonEmptyStringConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null) return null;

            string? text = reader.Value.ToString();

            if (string.IsNullOrWhiteSpace(text))
            {
                return null;
            }

            return text;
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

    }

    public class NullToEmptyStringResolver : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            return type.GetProperties()
            .Select(p => {
                var jp = base.CreateProperty(p, memberSerialization);
                jp.ValueProvider = new EmptyToNullStringValueProvider(p);
                return jp;
            }).ToList();
        }
    }

    public class EmptyToNullStringValueProvider : IValueProvider
    {
        PropertyInfo _MemberInfo;

        public EmptyToNullStringValueProvider(PropertyInfo memberInfo)
        {
            _MemberInfo = memberInfo;
        }

        public object? GetValue(object target)
        {
            object? result = _MemberInfo.GetValue(target);

            if (_MemberInfo.PropertyType == typeof(string) && result != null && string.IsNullOrWhiteSpace(result.ToString()))
            {
                result = null;
            }

            return result;
        }

        public void SetValue(object target, object? value)
        {
            if (_MemberInfo.PropertyType == typeof(string) && value != null && string.IsNullOrWhiteSpace(value.ToString()))
            {
                value = null;
            }

            _MemberInfo.SetValue(target, value);
        }
    }

}

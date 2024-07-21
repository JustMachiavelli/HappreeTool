using System.Text.Json;

namespace HappreeTool.CommonUtils
{
    public static class SettingsManager<T> where T : new()
    {
        public static async Task<T> LoadSettingsAsync(string settingsPath)
        {
            if (File.Exists(settingsPath))
            {
                var json = await File.ReadAllTextAsync(settingsPath);
                var settings = JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (settings == null)
                {
                    throw new ArgumentNullException(nameof(settings));
                }
                return settings;
            }
            return new T();
        }

        public static async Task SaveSettingsAsync(T settings, string settingsPath)
        {
            var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(settingsPath, json);
        }

        public static async Task UpdateSettingAsync(string path, object value, string settingsPath)
        {
            var settings = await LoadSettingsAsync(settingsPath);
            UpdateNestedProperty(settings!, path.Split('.'), value);
            await SaveSettingsAsync(settings, settingsPath);
        }

        private static void UpdateNestedProperty(object obj, string[] pathSegments, object value, int index = 0)
        {
            if (obj == null || pathSegments == null || index >= pathSegments.Length)
            {
                throw new ArgumentException("Invalid path or object.");
            }

            var propertyName = pathSegments[index];
            var propertyInfo = obj.GetType().GetProperty(propertyName);
            if (propertyInfo == null)
            {
                throw new ArgumentException($"Property '{propertyName}' not found on '{obj.GetType()}'.");
            }

            if (index == pathSegments.Length - 1)
            {
                propertyInfo.SetValue(obj, Convert.ChangeType(value, propertyInfo.PropertyType));
            }
            else
            {
                var nestedObject = propertyInfo.GetValue(obj);
                if (nestedObject == null)
                {
                    // 如果属性值为 null，实例化该属性的类型的一个实例
                    nestedObject = Activator.CreateInstance(propertyInfo.PropertyType);
                    propertyInfo.SetValue(obj, nestedObject);
                }
                UpdateNestedProperty(nestedObject!, pathSegments, value, index + 1);
            }
        }
    }

}

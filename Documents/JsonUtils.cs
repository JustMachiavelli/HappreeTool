using System.Text;
using System.Text.Json;

namespace HappreeTool.Documents
{
    public class JsonUtils
    {

        static Dictionary<string, object> ReadJsonToDict(string path)
        {
            using (StreamReader sr = new StreamReader(path, Encoding.UTF8))
            {
                string json = sr.ReadToEnd();
                Dictionary<string, object> dictJson = JsonSerializer.Deserialize<Dictionary<string, object>>(json)!;
                return dictJson;
            }
        }

        /// <summary>
        /// 写json
        /// </summary>
        /// <param name="path">json路径</param>
        /// <param name="dictJson">内容dict</param>
        static void WriteJson(string path, Dictionary<string, object> dictJson)
        {
            using (StreamWriter sw = new StreamWriter(path, false, Encoding.UTF8))
            {
                string json = JsonSerializer.Serialize(dictJson, new JsonSerializerOptions { WriteIndented = true });
                sw.Write(json);
            }
        }

        /// <summary>
        /// 更新json文件的某个key的值
        /// </summary>
        /// <param name="path">json路径</param>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns>是否改变</returns>
        static bool UpdateJsonValue(string path, string key, string value)
        {
            if (File.Exists(path))
            {
                Dictionary<string, object> dictJson = ReadJsonToDict(path);
                dictJson[key] = value;
                WriteJson(path, dictJson);
                return true;
            }
            return false;
        }
    }
}

using System.Text.RegularExpressions;

namespace HappreeTool.CommonUtils
{
    public class MyStringUtils
    {
        /// <summary>
        /// 去除字符串中某些文字
        /// </summary>
        /// <param name="toReplace"></param>
        /// <param name="stringsToRemove"></param>
        /// <returns></returns>
        public static string ReplaceByArray(string toReplace, IEnumerable<string> stringsToRemove)
        {
            // 遍历数组，逐个替换字符串
            foreach (string str in stringsToRemove)
            {
                toReplace = toReplace.Replace(str, "");
            }

            return toReplace;
        }

        /// <summary>
        /// 替换字符串中某些文字为其他字符串
        /// </summary>
        /// <param name="toReplace"></param>
        /// <param name="stringsToReplace"></param>
        /// <returns></returns>
        public static string ReplaceByDict(string toReplace, Dictionary<string, string> stringsToReplace)
        {

            foreach (KeyValuePair<string, string> kvp in stringsToReplace)
            {
                toReplace = toReplace.Replace(kvp.Key, kvp.Value);
            }
            return toReplace;
        }

        /// <summary>
        /// 将文件命令的非法字符替换的字典
        /// </summary>
        static readonly Dictionary<string, string> dictOsInvalidChars = new()
        {
            { "\\", "#" },
            { "/", "#" },
            { ":", "：" },
            { "*", "#" },
            { "?", "？" },
            { "\"", "#" },
            { "<", "《" },
            { ">", "》" },
            { "|", "#" }
        };

        /// <summary>
        /// 替换windows路径不允许的特殊字符 \/:*?"<>|
        /// </summary>
        /// <param name="src">原字符串，文件名、简介、标题、导演姓名等</param>
        /// <returns>替换后字符串</returns>
        public static string ReplaceWindowsOsInvalidChar(string src)
        {
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
            {
                // 不是 Windows 系统
                return src;
            }
            //if (src == "\\") return src;
            return ReplaceByDict(src, dictOsInvalidChars);
        }

        /// <summary>
        /// XML转义
        /// </summary>
        public static readonly Dictionary<string, string> XmlInvalidCharsDict = new()
        {
            { "\r\n", "&NewLine" },
            { "\n", "&NewLine" },
            { "\t", "&Tab;" },
        };

        /// <summary>
        /// 替换xml中的不允许的特殊字符，替换制表符，去除首尾空格
        /// </summary>
        /// <remarks>xml需要转义【&<>"'】，双引号和单引号似乎被什么操作变得不影响。</remarks>
        /// <param name="src">原字符串，例如文件名、简介、标题、导演姓名等</param>
        /// <returns>替换后字符串</returns>
        public static string EscapeXml(string src)
        {
            //return System.Security.SecurityElement.Escape(ReplaceByDict(src, XmlInvalidCharsDict));
            return System.Security.SecurityElement.Escape(src).Trim();
        }

        /// <summary>
        /// XML转义
        /// </summary>
        public static readonly Dictionary<string, string> HtmlSpecialCharsDict = new()
        {
            { "<br>", "&NewLine" },
        };

        /// <summary>
        /// 去除html中一段文字中的<br>换行
        /// </summary>
        /// <param name="src">一段网页</param>
        /// <returns>替换后的字符串</returns>
        public static string ReplaceHtmlBr(string src)
        {
            return ReplaceByDict(src, HtmlSpecialCharsDict);
        }

        /// <summary>
        /// 提取字符串中指定两个节点内容
        /// </summary>
        /// <param name="src">原字符串</param>
        /// <param name="begin">开始节点字符串</param>
        /// <param name="end">结束节点字符串</param>
        /// <returns></returns>
        /// <remarks>
        /// <para>begin（或end）传null，则从头（从尾）取</para>；
        /// <para>begin或end不传null，且找不到，则返回null</para>
        /// </remarks>
        public static string? ExtractByBeginEndChar(string? src, string? begin = null, string? end = null)
        {
            if (string.IsNullOrEmpty(src))
                return null;

            int bIndex = begin != null ? src.IndexOf(begin, StringComparison.Ordinal) : 0;
            if (bIndex == -1) // 如果 begin 不存在于 src 中，则直接返回null
                return null;

            int startIndex = bIndex + (begin?.Length ?? 0);

            if (end == null) // 如果 end 为 null，则从 bIndex 开始到 src 结尾的部分
                return src.Substring(startIndex);

            int eIndex = src.IndexOf(end, startIndex, StringComparison.Ordinal);
            if (eIndex == -1) // 如果 end 不存在于 src 中，则直接返回null
                return null;

            return src.Substring(startIndex, eIndex - startIndex);
        }

        /// <summary>
        /// 正则表达式匹配，匹配不到返回null
        /// </summary>
        /// <param name="src"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static string? ExtractFirstMatch(string? src, string pattern)
        {
            if (string.IsNullOrEmpty(src))
            {
                return null;
            }

            Regex regex = new Regex(pattern);
            Match match = regex.Match(src);
            if (match.Success)
            {
                return match.Groups[1].Value;
            }

            return null;
        }

    }
}

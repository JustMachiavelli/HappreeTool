namespace HappreeTool.Utils.Documents
{
    public class VideoUtils
    {
        public static readonly HashSet<string> VIDEO_EXTENSIONS = new(StringComparer.OrdinalIgnoreCase)
        {
            ".mp4",
            ".mov",
            ".avi",
            ".wmv",
            ".mkv",
            ".flv",
            ".webm",
            ".m4v",
            ".rmvb",
            ".ts",
            ".iso",
        };

        public static readonly HashSet<string> SUBTITLE_EXTENSIONS = new(StringComparer.OrdinalIgnoreCase)
        {
            ".srt",
            ".sub",
            ".ass",
            ".ssa",
            ".vtt",
            ".smi",
        };

        /// <summary>
        /// 通用文件后缀判定
        /// </summary>
        public static bool IsMatchExtension(string filePath, IEnumerable<string> extensions)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return false;
            }

            string fileName = Path.GetFileName(filePath);

            // 排除隐藏文件
            if (fileName.StartsWith("."))
            {
                return false;
            }

            string extension = Path.GetExtension(filePath);

            return extensions.Contains(
                extension,
                StringComparer.OrdinalIgnoreCase);
        }

        public static bool IsVideoFile(string filePath)
        {
            return IsMatchExtension(filePath, VIDEO_EXTENSIONS);
        }

        public static bool IsSubtitleFile(string filePath)
        {
            return IsMatchExtension(filePath, SUBTITLE_EXTENSIONS);
        }
    }
}
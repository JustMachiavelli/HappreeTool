namespace HappreeTool.Documents
{
    public class VideoUtils
    {

        public static readonly HashSet<string> VIDEO_EXTENSIONS = new HashSet<string>
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
            // 添加其他视频后缀名...
        };


        public static bool IsVideoFile(string filePath)
        {
            if (Path.GetFileName(filePath).StartsWith("."))
            {
                return false;
            }
            return VIDEO_EXTENSIONS.Contains(Path.GetExtension(filePath), StringComparer.OrdinalIgnoreCase);
        }

        public static readonly HashSet<string> SUBTITLE_EXTENSIONS = new HashSet<string>
        {
            ".srt",
            ".sub",
            ".ass",
            ".ssa",
            ".vtt",
            ".smi",
            // 添加其他字幕文件后缀名...
        };

        public static bool IsSubtitleFile(string filePath)
        {
            if (Path.GetFileName(filePath).StartsWith("."))
            {
                return false;
            }
            return SUBTITLE_EXTENSIONS.Contains(Path.GetExtension(filePath), StringComparer.OrdinalIgnoreCase);
        }


    }
}

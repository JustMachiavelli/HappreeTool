namespace HappreeTool.Constants.Exceptions.Process
{
    /// <summary>
    /// 处理过程中，可容忍的错误，可以继续对当前项的整理
    /// </summary>
    /// <remarks>可能只是放弃某个网站对当前项的整理。</remarks>
    /// <param name="message"></param>
    public class TolerantException(string message) : Exception(message) { }
}

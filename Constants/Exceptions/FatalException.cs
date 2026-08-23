namespace HappreeTool.Constants.Exceptions
{
    /// <summary>
    /// 处理过程中，发生致命错误，必须停止整个应用程序，否则会大量报错或污染数据。
    /// </summary>
    public class FatalException(string message) : Exception(message) { }
}
namespace HappreeTool.Constants.Exceptions.Process
{
    /// <summary>
    /// 需要手动处理的错误，用于提示管理员（等当前流程结束后）进行人工干预
    /// </summary>
    /// <param name="msg"></param>
    public class ManualHandlingException(string msg) : Exception(msg) { }
}

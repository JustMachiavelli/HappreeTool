namespace HappreeTool.Constants.Exceptions.Process;

/// <summary>
/// 
/// </summary>
/// <param name="msg"></param>
public class BatchException(string msg) : Exception(msg) { }
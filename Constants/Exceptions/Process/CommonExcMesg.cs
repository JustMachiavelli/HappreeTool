namespace HappreeTool.Constants.Exceptions.Process;

public class CommonExcMesg
{
    public static string ResourceNotExist(string resourceType, string property)
        => $"{nameof(property)}为【{resourceType}】的【{property}】资源还不存在";
}
using HappreeTool.Constants.Exceptions.Process;

namespace AVPI.Shared.Exceptions
{
    /// <summary>
    /// 必要资源不存在
    /// </summary>
    /// <param name="resourceType"></param>
    /// <param name="property"></param>
    public class ResourceNotExistException(string resourceType, string property)
        : FatalException($"{nameof(property)}为【{property}】的【{resourceType}】资源还不存在")
    {
    }
}

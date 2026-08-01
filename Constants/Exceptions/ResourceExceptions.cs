using HappreeTool.Constants.Exceptions.Process;

namespace HappreeTool.Constants.Exceptions;

/// <summary>
/// 必要资源不存在
/// </summary>
/// <param name="resourceType"></param>
/// <param name="property"></param>
public class ResourceNotExistException(string resourceType, string property)
    : Exception($"{nameof(property)}为【{resourceType}】的【{property}】资源还不存在")
{
}

/// <summary>
/// 重复创建资源
/// </summary>
/// <param name="resourceType">资源名称</param>
/// <param name="id">唯一标识</param>
public class ResourceAlreadyExistException(string resourceType, string id)
    : Exception($"{nameof(id)}为【{resourceType}】资源【{id}】已存在")
{
}
namespace AVPI.Shared.Exceptions
{
    public class ResourceNotFoundException(string resourceType, string property)
        : Exception($"{nameof(property)}为【{property}】的【{resourceType}】资源未找到")
    {
    }
}

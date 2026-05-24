namespace HappreeTool.Utils.CommonUtils;

public static class ProjectNamespaceUtils
{
    /// <summary>
    /// 获取命名空间中的指定层级名称
    /// </summary>
    /// <param name="type">类型</param>
    /// <param name="index">层级索引</param>
    /// <returns></returns>
    public static string GetNamespacePart(Type type, int no)
    {
        string? ns = type.Namespace;

        if (string.IsNullOrWhiteSpace(ns))
        {
            throw new ArgumentException();
        }

        int index = no - 1;
        string[] parts = ns.Split('.');
        if (index < 0 || index >= parts.Length)
        {
            throw new ArgumentException();
        }

        return parts[index];
    }

    /// <summary>
    /// 获取当前项目层的简单名称
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static string GetSimpleModuleName(Type type)
    {
        return GetNamespacePart(type, 2);
    }
}
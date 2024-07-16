using System.Runtime.InteropServices;
using System.Text;

namespace HappreeTool.Documents
{
    public static class FileUtils
    {
        /// <summary>
        /// 递归处理文件夹的示例
        /// </summary>
        /// <param name="rootFolder"></param>
        public static void ProcessFiles(string rootFolder)
        {
            Stack<string> folderStack = new Stack<string>(); // 使用栈模拟递归的文件夹堆栈

            folderStack.Push(rootFolder); // 将根文件夹入栈

            while (folderStack.Count > 0)
            {
                string currentFolder = folderStack.Pop(); // 弹出栈顶文件夹

                // 处理当前文件夹下的文件
                string[] files = Directory.GetFiles(currentFolder);
                foreach (string file in files)
                {
                    // 处理文件逻辑...
                    Console.WriteLine($"处理文件: {file}");
                }

                // 获取当前文件夹的子文件夹并入栈
                string[] subFolders = Directory.GetDirectories(currentFolder);
                foreach (string subFolder in subFolders)
                {
                    folderStack.Push(subFolder);
                }
            }
        }

        /// <summary>
        /// 获取所给路径文件夹的所有子文件
        /// </summary>
        /// <param name="directoryPath">要处理的文件夹路径</param>
        /// <param name="searchPattern">要匹配的文件格式</param>
        /// <param name="files">已获取的子文件们完整路径</param>
        /// <returns></returns>
        public static List<string> GetAllSubFilesPaths(string directoryPath, string searchPattern, List<string>? files = null)
        {
            if (files == null)
            {
                files = new List<string>();
            }

            DirectoryInfo currentDirectoryInfo = new DirectoryInfo(directoryPath);

            // 当前一级文件夹内的子文件们
            files.AddRange(currentDirectoryInfo.GetFiles(searchPattern).Select(fi => fi.FullName).ToList());

            // 当前一级文件夹内的子文件夹们
            DirectoryInfo[] subDirectoryInfos = currentDirectoryInfo.GetDirectories();
            //递归
            foreach (DirectoryInfo di in subDirectoryInfos)
            {
                GetAllSubFilesPaths(di.FullName, searchPattern, files);
            }

            return files;
        }

        /// <summary>
        /// 以append形式向指定txt写入内容
        /// </summary>
        /// <param name="path">文本路径</param>
        /// <param name="content">要写入内容，注意加换行符</param>
        public static void WriteTxtAppend(string path, string content)
        {
            new StreamWriter(path, true, Encoding.UTF8).WriteLine(content);
        }

        /// <summary>
        /// 确保当前目录存在
        /// </summary>
        /// <remarks>不存在则创建</remarks>
        /// <param name="dirPath">目录路径</param>
        /// <returns>目录路径</returns>
        public static void ConfirmDirExist(string dirPath)
        {
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
        }

        /// <summary>
        /// 确保当前目录或文件不存在
        /// </summary>
        /// <remarks>存在则删去</remarks>
        /// <param name="path">路径</param>
        /// <returns>路径</returns>
        public static string ConfirmFileNotExist(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            return path;
        }

        /// <summary>
        /// 读取文本内容为Array【待定】
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string[] ReadTxtAsList(string path)
        {
            return File.ReadAllLines(path).ToArray();
        }

        /// <summary>
        /// 获取父目录路径【待定】
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string DirFather(string path)
        {
            return Path.GetDirectoryName(path.TrimEnd('\\')) ?? path;
        }

        /// <summary>
        /// 判定文件路径是否过长
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool IsPathTooLong(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (path.StartsWith(@"\\?\") || path.StartsWith(@"\\?\UNC\"))
            {
                return false;
            }

            int pathLength = path.Length;

            if (pathLength >= 260)
            {
#if NETCOREAPP
                return true; // 在.NET Core中，路径长度受限制为260个字符
#else
                string root = Path.GetPathRoot(path);
                if (root.Length <= 3) // 例如，C:\，D:\ 等
                {
                    return pathLength >= 259;
                }
                else
                {
                    return pathLength >= 248;
                }
#endif
            }

            return false;
        }

        /// <summary>
        /// 重命名文件
        /// </summary>
        /// <param name="oldFilePath"></param>
        /// <param name="newFilePath"></param>
        /// <param name="errorMessage"></param>
        /// <returns>确保oldFilePath变成newFilePath</returns>
        public static void MoveFile(string oldFilePath, string newFilePath)
        {
            if (!File.Exists(oldFilePath))
            {
                throw new FileNotFoundException($"原文件不存在【 {oldFilePath}】");
            }

            if (IsPathTooLong(newFilePath))
            {
                throw new PathTooLongException($"无法重命名文件，新文件路径太长【{newFilePath}】");
            }

            // 目标路径不存在，直接重命名
            if (!File.Exists(newFilePath))
            {
                File.Move(oldFilePath, newFilePath);
                return;
            }

            // 目标路径存在，且大小写完全相同，就是自己，不用动
            if (string.Equals(oldFilePath, newFilePath, StringComparison.Ordinal))
            {
                // Windows&linux系统，大小写敏感，相同
                return;
            }

            // 目标路径已存在，特定于操作系统的处理
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // Windows系统
                if (!string.Equals(oldFilePath, newFilePath, StringComparison.OrdinalIgnoreCase))
                {
                    // 大小写不敏感，不同  => 不是自己 => 重复文件 => 抛错
                    throw new IOException($"无法重命名文件【{oldFilePath}】，目标路径已存在【{newFilePath}】");
                }

                // 大小写不敏感，相同 => 是自己，但大小写不同 => 重命名 true => 用一个临时文件move两次（直接重命名大小写，可能失败）
                string tempFilePath = Path.Combine(Path.GetDirectoryName(oldFilePath) ?? oldFilePath, Path.GetRandomFileName());
                File.Move(oldFilePath, tempFilePath);
                File.Move(tempFilePath, newFilePath);
                return;
            }

            // 非Windows系统，大小写敏感，不同  => 不是自己，是另一个重复文件 => 抛错
            throw new IOException($"无法重命名文件【{oldFilePath}】，目标路径已存在【{newFilePath}】");
        }

        /// <summary>
        /// 移动文件夹
        /// </summary>
        /// <param name="oldDir"></param>
        /// <param name="newDir"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public static void MoveDirectory(string oldDir, string newDir)
        {
            if (!Directory.Exists(oldDir))
            {
                throw new DirectoryNotFoundException($"原目录不存在【{oldDir}】");
            }

            if (IsPathTooLong(newDir))
            {
                throw new PathTooLongException($"无法重命名文件，新文件夹路径太长【{newDir}】");
            }

            // 目标路径不存在，直接重命名
            if (!Directory.Exists(newDir))
            {
                Directory.Move(oldDir, newDir);
                return;
            }

            // 目标路径存在，且大小写完全相同，就是自己，不用动
            if (string.Equals(oldDir, newDir, StringComparison.Ordinal))
            {
                // Windows&Linux系统，大小写敏感，相同
                return;
            }

            // 目标路径已存在，特定于操作系统的处理
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // Windows系统
                if (!string.Equals(oldDir, newDir, StringComparison.OrdinalIgnoreCase))
                {
                    // 大小写不敏感，不同  => 不是自己 => 重复目录 => 不动 false
                    throw new IOException($"无法重命名目录【{oldDir}】，目标路径已存在【{newDir}】");
                }

                // 大小写不敏感，相同 => 是自己，但大小写不同 => 重命名 true => 用一个临时目录move两次（直接重命名大小写，可能失败）
                string tempDir = Path.Combine(Path.GetDirectoryName(oldDir) ?? oldDir, Path.GetRandomFileName());
                Directory.Move(oldDir, tempDir);
                Directory.Move(tempDir, newDir);
                return;
            }

            // 非Windows系统，大小写敏感，不同  => 不是自己，是另一个重复目录 => 不动 false
            throw new IOException($"无法重命名目录【{oldDir}】，目标路径已存在【{newDir}】");
        }

        /// <summary>
        /// 确保C:\a.jpg变为C:\A.jpg
        /// </summary>
        /// <remarks>在Windows系统大小写不敏感，File.Move("C:\a.jpg", "C:\A.jpg")不会生效</remarks>
        /// <param name="filePath"></param>
        /// <param name="desiredPath"></param>
        public static void EnsureCaseSensitivePath(string desiredPath)
        {
            var directory = Path.GetDirectoryName(desiredPath);
            var fileNameLower = Path.GetFileName(desiredPath).ToLower();

            if (!Directory.Exists(directory))
            {
                throw new DirectoryNotFoundException($"文件夹'{directory}'不存在");
            }

            var file = Directory.GetFiles(directory)
                                .FirstOrDefault(f => string.Equals(Path.GetFileName(f).ToLower(), fileNameLower, StringComparison.Ordinal));

            if (file == null)
            {
                throw new FileNotFoundException($"The file '{desiredPath}' was not found with case-sensitive match.");
            }

            string tempPath = Path.Combine(directory, Path.GetRandomFileName() + "_" + fileNameLower);
            File.Move(desiredPath, tempPath);
            File.Move(tempPath, desiredPath);
        }

    }
}

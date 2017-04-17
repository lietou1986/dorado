using System;
using System.Collections.Generic;
using System.IO;

namespace Dorado.Utils
{
    /// <summary>
    /// 关于路径的工具
    /// </summary>
    [System.Diagnostics.DebuggerStepThrough]
    public static class IOUtility
    {
        /// <summary>
        /// 将多个路径合并在一起，并忽略其中的“..”与“.”
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        public static string Combine(params string[] paths)
        {
            return Revise(Path.Combine(paths));
        }

        /// <summary>
        /// 修正路径，将其中的"."及".."合并
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string Revise(string path)
        {
            if (string.IsNullOrEmpty(path))
                return path;

            if (path.IndexOf('.') < 0)
                return path;

            string[] parts = path.Split(new char[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
            string[] newParts = new string[parts.Length];
            bool isPathRoot = Path.IsPathRooted(parts[0]);

            int newIndex = 0;
            for (int index = 0; index < parts.Length; index++)
            {
                string part = parts[index];
                if (part == ".")
                    continue;

                if (part == "..")
                {
                    if (newIndex == 0 || newIndex == 1 && isPathRoot)
                        throw new FormatException("路径格式不正确");

                    newIndex--;
                }
                else
                {
                    newParts[newIndex++] = part;
                }
            }

            return string.Join("\\", newParts, 0, newIndex);
        }

        /// <summary>
        /// 遍历目录
        /// </summary>
        /// <param name="rootDirectory"></param>
        /// <param name = "fileHandler"></param>
        public static void Traversing(IList<string> rootDirectory, Action<FileInfo> fileHandler)
        {
            Queue<string> pathQueue = new Queue<string>();

            foreach (string directory in rootDirectory)
            {
                pathQueue.Enqueue(directory);
            }

            while (pathQueue.Count > 0)
            {
                DirectoryInfo diParent = new DirectoryInfo(pathQueue.Dequeue());
                foreach (DirectoryInfo diChild in diParent.GetDirectories())
                {
                    pathQueue.Enqueue(diChild.FullName);
                }

                foreach (FileInfo fi in diParent.GetFiles())
                    fileHandler?.Invoke(fi);
            }
        }

        public static void Traversing(string rootDirectory, Action<FileInfo> fileHandler)
        {
            Traversing(new List<string>() { rootDirectory }, fileHandler);
        }

        /// <summary>
        /// 删除目录
        /// </summary>
        public static void DeleteDirectory(bool isReCreate, params string[] paths)
        {
            foreach (string path in paths)
            {
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                }
                if (isReCreate)
                    Directory.CreateDirectory(path);
            }
        }

        /// <summary>
        /// 删除目录
        /// </summary>
        public static void DeleteDirectory(params string[] paths)
        {
            DeleteDirectory(false, paths);
        }

        /// <summary>
        /// 创建目录
        /// </summary>
        public static void CreateDirectory(params string[] paths)
        {
            foreach (string path in paths)
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
            }
        }
    }
}
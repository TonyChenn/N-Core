using System;
using System.Collections.Generic;
using System.IO;

namespace NCore
{
    public static class FileHelper
    {
        /// <summary>
        /// 获取文件夹中所有文件
        /// </summary>
        /// <param name="fileList">文件所装载的列表</param>
        /// <param name="dir"></param>
        public static void GetAllFiles(List<string> fileList, string dir)
        {
            string[] files = Directory.GetFiles(dir);
            for (int i = 0, iMax = files.Length; i < iMax; i++)
                fileList.Add(files[i]);

            string[] subDirs = Directory.GetDirectories(dir);
            for (int i = 0, iMax = subDirs.Length; i < iMax; i++)
                GetAllFiles(fileList, subDirs[i]);
        }

        /// <summary>
        /// 删除文件夹(包括子文件)
        /// </summary>
        /// <param name="dir"></param>
        public static void DeleteDirectory(string dir)
        {
            foreach (string subdir in Directory.GetDirectories(dir))
                Directory.Delete(subdir, true);

            foreach (string subFile in Directory.GetFiles(dir))
                File.Delete(subFile);
        }

        /// <summary>
        /// 将src文件夹下文件及文件夹Copy到目标文件夹下
        /// </summary>
        /// <param name="srcDir"></param>
        /// <param name="tgtDir"></param>
        public static void CopyDirectory(string srcDir, string tgtDir)
        {
            DirectoryInfo source = new DirectoryInfo(srcDir);
            DirectoryInfo target = new DirectoryInfo(tgtDir);

            if (target.FullName.StartsWith(source.FullName, StringComparison.CurrentCultureIgnoreCase))
                throw new Exception("父目录不能拷贝到子目录！");

            if (!source.Exists) return;

            if (!target.Exists)
                target.Create();

            FileInfo[] files = source.GetFiles();
            for (int i = 0, iMax = files.Length; i < iMax; i++)
                File.Copy(files[i].FullName, Path.Combine(target.FullName, files[i].Name), true);

            DirectoryInfo[] dirs = source.GetDirectories();
            for (int j = 0, jMax = dirs.Length; j < jMax; j++)
                CopyDirectory(dirs[j].FullName, Path.Combine(target.FullName, dirs[j].Name));
        }
    }
}

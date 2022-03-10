using System;
using System.IO;
using UnityEngine;

namespace SFramework.Core
{
    public static class StringExtention
    {
        #region 路径相关

        /// <summary>
        /// 路径标准化
        /// </summary>
        /// 将路径中"\\" 转换为"/",去除末尾"/"
        public static string MakeStandardPath(this string path)
        {
            return path.Replace("\\", "/").TrimEnd('/');
        }

        /// <summary>
        /// AssetPath 转化为 FullPath
        /// </summary>
        public static string AssetPathToFullPath(this string assetPath)
        {
            if (string.IsNullOrEmpty(assetPath) || !assetPath.StartsWith("Assets"))
                throw new Exception(assetPath);
            
            string temp = Application.dataPath + assetPath.Substring("Assets".Length);
            return temp.MakeStandardPath();
        }

        /// <summary>
        /// FullPath 转化为 AssetPath
        /// </summary>
        public static string FullPathToAssetPath(this string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath))
                return null;
            
            return $"Assets/{fullPath.Substring(Application.dataPath.Length)}";
        }
        #endregion

        #region 文件夹

        /// <summary>
        /// 文件夹是否存在
        /// </summary>
        public static bool ExistFolder(this string folderPath)
        {
            if (string.IsNullOrEmpty(folderPath))
                throw new Exception($"{folderPath} is null or empty");
            
            return Directory.Exists(folderPath);
        }

        /// <summary>
        /// 创建文件夹（如果不存在）
        /// </summary>
        public static void CreateFolder(this string folderPath)
        {
            if (!folderPath.ExistFolder())
            {
                Directory.CreateDirectory(folderPath);
            }
        }

        /// <summary>
        /// 删除文件夹（存在并且为空的话）
        /// </summary>
        public static void DeleteFolderIfEmpty(this string folderPath)
        {
            if (folderPath.ExistFolder())
            {
                if (Directory.GetDirectories(folderPath).Length == 0 && Directory.GetFiles(folderPath).Length == 0)
                    Directory.Delete(folderPath);
            }
        }

        /// <summary>
        /// 删除文件夹（包括子文件/子文件夹也会删除）
        /// </summary>
        public static void DeleteFolderForce(this string folderPath)
        {
            if (folderPath.ExistFolder())
            {
                Directory.Delete(folderPath, true);
            }
        }
        #endregion

        #region 文件相关

        /// <summary>
        /// 判断文件是否存在
        /// </summary>
        public static bool ExistFile(this string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return false;
            return File.Exists(filePath);
        }

        /// <summary>
        /// 删除文件（如果存在）
        /// </summary>
        /// <param name="filePath"></param>
        public static void DeleteFile(this string filePath)
        {
            if(filePath.ExistFile())
                File.Delete(filePath);
        }

        /// <summary>
        /// 创建文件的文件夹
        /// </summary>
        /// <param name="filePath"></param>
        public static void CreateFileFolder(this string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return;

            string folderName = Path.GetDirectoryName(filePath);
            folderName.CreateFolder();
        }
        #endregion
    }
}
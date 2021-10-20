using System;
using UnityEngine;

namespace NextFramework.Core
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
    }
}
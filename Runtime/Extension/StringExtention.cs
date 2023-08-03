using System;
using System.IO;
using UnityEngine;

namespace NCore
{
    public static class StringExtention
    {
        #region 字符串相关
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static int ToInt(this string str, int defaultValue = 0)
        {
            var result = defaultValue;
            int.TryParse(str, out result);
            return result;
        }

        public static DateTime ToDateTime(this string str, DateTime defaultValue = default)
        {
            var result = defaultValue;
            DateTime.TryParse(str, out result);
            return result;
        }

        #endregion
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

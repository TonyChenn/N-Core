using System;
using UnityEditor;
using UnityEngine;

namespace NCore.Editor
{
    public class EditorUtil
    {
        [MenuItem("Tools/Editor/导出Package")]
        private static void ExportPackage()
        {
            ExportPackage("Assets/SFramework",GenPackageName());
        }


        [MenuItem("Tools/Open../StreammingAsset")]
        private static void OpenStreammingAssetFolder() { OpenFolder(Application.streamingAssetsPath); }
        [MenuItem("Tools/Open../可读写目录")]
        private static void OpenPersistentFolder() { OpenFolder(Application.persistentDataPath); }


        /// <summary>
        /// 打开文件夹
        /// </summary>
        public static void OpenFolder(string folderPath)
        {
            Application.OpenURL($"file:///{folderPath}");
        }

        /// <summary>
        /// 导出UnityPackage
        /// </summary>
        public static void ExportPackage(string assetPathName, string fileName)
        {
            AssetDatabase.ExportPackage(assetPathName, fileName, ExportPackageOptions.Recurse);
            OpenFolder($"{Application.dataPath}/../../");
        }
        private static string GenPackageName()
        {
            string time = DateTime.Now.ToString("yyyy.MM.dd.HH");
            return $"../SFramework_{time}.unitypackage";
        }
    }
}
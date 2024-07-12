using System;
using UnityEngine;
using UnityEngine.Networking;

namespace NCore
{
    public static class UnityWebRequestExtention
    {
        /// <summary>
        /// TextInfo
        /// </summary>
        public static string GetTxt(this UnityWebRequest request)
        {
            return request.downloadHandler.text;
        }

        /// <summary>
        /// AssetBundle
        /// </summary>
        public static AssetBundle GetAssetBundle(this UnityWebRequest request)
        {
            return DownloadHandlerAssetBundle.GetContent(request);
        }

        /// <summary>
        /// AudioClip
        /// </summary>
        public static AudioClip GetAudioClip(this UnityWebRequest request)
        {
            return DownloadHandlerAudioClip.GetContent(request);
        }

        /// <summary>
        /// Texture
        /// </summary>
        public static Texture GetTexture(this UnityWebRequest request)
        {
            return DownloadHandlerTexture.GetContent(request);
        }

        /// <summary>
        /// 二进制
        /// </summary>
        public static byte[] GetBytes(this UnityWebRequest request)
        {
            return request.downloadHandler.data;
        }

        /// <summary>
        /// 把下载的文件保存到本地
        /// </summary>
        /// <param name="request"></param>
        /// <param name="path"></param>
        public static void SaveFile(this UnityWebRequest request, string path)
        {
            using (System.IO.MemoryStream memory = new(request.GetBytes()))
            {
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);

                var folder = System.IO.Path.GetDirectoryName(path);
                if (!System.IO.Directory.Exists(folder)) { System.IO.Directory.CreateDirectory(folder); }

                byte[] buffer = new byte[1024 * 1024];
                System.IO.FileStream fs = System.IO.File.Open(path, System.IO.FileMode.OpenOrCreate);
                int readBytes;
                while ((readBytes = memory.Read(buffer, 0, buffer.Length)) > 0)
                {
                    fs.Write(buffer, 0, readBytes);
                }
                fs.Close();
            }
        }
    }
}

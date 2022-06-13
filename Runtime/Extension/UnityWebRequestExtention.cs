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
        public static AssetBundle GetBundle(this UnityWebRequest request)
        {
            return DownloadHandlerAssetBundle.GetContent(request);
        }
        
        /// <summary>
        /// AudioClip
        /// </summary>
        public static AudioClip GetClip(this UnityWebRequest request)
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
        /// JsonNode
        /// </summary>
        /// <returns></returns>
        public static JSONNode GetJsonNode(this UnityWebRequest request)
        {
            string json = request.GetTxt();
            return JSON.Parse(json);
        }
    }
}
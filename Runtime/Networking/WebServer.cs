using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace NCore
{
    /// <summary>
    /// 用作http请求,小文件下载
    /// </summary>
    public static class WebServer
    {
        #region Get Data
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// 如果已http开头就是绝对路径，否则是相对路径
        /// <returns></returns>
        public static async Task<UnityWebRequest> Get(string url)
        {
            //if (!url.StartsWith("http"))
            //    url = ChannelConfig.Singleton.CurChannel.ServerURL + url;

            UnityWebRequest request = UnityWebRequest.Get(url);
            await request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success)
                Debug.LogError($"[WebServer] fail to get ---- {url}");

            return request;
        }
        #endregion

        #region Post Data

        public static async Task<UnityWebRequest> Post(string url, Hashtable data, byte[] file = null)
        {
            WWWForm form = new WWWForm();
            foreach (string key in data.Keys)
                form.AddField(key, data[key].ToString());

            if (file != null)
                form.AddBinaryData("file", file);

            var request = UnityWebRequest.Post(url, form);
            await request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
                Debug.LogError($"[WebServer] fail to post ---- {url}");

            return request;
        }

        #endregion

        #region Put
        public static async Task<UnityWebRequest> Put(string url, string data)
        {
            return await Put(url,System.Text.Encoding.UTF8.GetBytes(data));
        }
        public static async Task<UnityWebRequest> Put(string url, byte[] data)
        {
            UnityWebRequest request = UnityWebRequest.Put(url, data);
            await request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
                Debug.LogError($"[WebServer] fail to put ---- {url}");

            return request;
        }
        #endregion
    }
}


/// <summary>
/// UNITY_2017_1以前的使用此方法
/// </summary>

/*
public class WebServer : MonoSinglton<WebServer>, ISingleton
{
    public void InitSingleton()
    {
        Singlton.gameObject.name = "[WebServer]";
    }
    #region Get Data

    public void Get(string url, Action<UnityWebRequest> result)
    {
        StartCoroutine(GetData(url, result));
    }

    IEnumerator GetData(string url, Action<UnityWebRequest> result)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ProtocolError
            || request.result == UnityWebRequest.Result.ConnectionError)
        {
            string info = string.Format($"Get error: {request.error}, url: {url}");
            Debug.Log(info);
        }
        else
        {
            if (result != null) result(request);
        }
    }

    #endregion


    #region Post Data
    public void Post(string url, Hashtable data, byte[] file, Action<UnityWebRequest> result)
    {
        WWWForm form = new WWWForm();
        foreach (string key in data)
            form.AddField(key, data[key].ToString());

        if (file != null)
            form.AddBinaryData("file", file);

        StartCoroutine(PostData(url, form, result));
    }

    IEnumerator PostData(string url, WWWForm form, Action<UnityWebRequest> result)
    {
        var request = UnityWebRequest.Post(url, form);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ProtocolError
            || request.result == UnityWebRequest.Result.ConnectionError)
            Debug.Log(string.Format("Post error: {0}, url: {1}", request.error, url));
        else
        {
            if (result != null) result(request);
        }
    }

    #endregion
}
*/
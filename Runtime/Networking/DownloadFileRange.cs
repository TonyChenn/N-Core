using System.IO;
using UnityEngine.Networking;

namespace NCore.Networking
{
    /// <summary>
    /// Unity2018+
    /// 断点续传下载(用于大文件下载)
    /// </summary>
    [APIInfo("N-Core", "DownloadFileRange", @"
基于UnityWebRequest的断点续传下载支持,使用简单，适合大文件下载。
# 主要接口：
```csharp
// 开始下载事件
public event System.Action StartDownloadEvent;
// 下载过程事件(文件总大小, 已下载大小,下载速度)
public event System.Action<ulong, ulong, float> OnDownloadEvent;
public event System.Action DownloadCompleteEvent;  
```
# 使用方法：
```csharp
using (UnityWebRequest request = new UnityWebRequest(""https://xxx/yyy.apk"", UnityWebRequest.kHttpVerbGET))
{
    var downloadHandler = new DownloadFileRange(Application.persistentDataPath + ""/base.apk"", request);
    downloadHandler.OnDownloadEvent += OnRefreshProgress;
    request.downloadHandler = downloadHandler;
    await request.SendWebRequest();

    if (request.result != UnityWebRequest.Result.Success)
        Debug.LogError(""下载失败"");
    else
        Debug.Log(""下载成功"");
    downloadHandler.OnDispose();
}
```")]
    public class DownloadFileRange : DownloadHandlerScript
    {
        private ulong m_TotalFileSize = 0;               // 文件总大小
        private ulong m_LocalFileSize = 0;               // 本地文件大小
        private ulong m_CurFileSize = 0;                 // 当前文件大小
        private float m_DownloadSpeed = 0;               // 下载速度

        private string m_FileSavePath;                  // 文件保存路径
        private string m_TempSavePath;                  // 临时保存目录
        private UnityWebRequest m_WebRequest;
        private FileStream m_Fs;

        private float m_LastTime = 0;
        private ulong m_LastSize = 0;

        public event System.Action StartDownloadEvent;                          // 开始下载事件
        public event System.Action<ulong, ulong, float> OnDownloadEvent;        // 下载过程事件(文件总大小, 已下载大小,下载速度)
        public event System.Action DownloadCompleteEvent;                       // 下载结束事件

        public DownloadFileRange(string savePath, UnityWebRequest request) : base(new byte[1024 * 1024])
        {
            m_FileSavePath = savePath;
            m_TempSavePath = savePath + ".temp";
            m_WebRequest = request;

            string folder = Path.GetDirectoryName(savePath);
            if (!Directory.Exists(folder)) { Directory.CreateDirectory(folder); }

            m_Fs = new FileStream(m_TempSavePath, FileMode.Append, FileAccess.Write);
            m_LocalFileSize = (ulong)m_Fs.Length;
            m_CurFileSize = m_LocalFileSize;

            // "bytes =< range - start > -" 请求从range - start到文件结尾的所有bytes
            //  https://developer.mozilla.org/zh-CN/docs/Web/HTTP/Headers/Range
            request.SetRequestHeader("Range", "bytes=" + m_LocalFileSize + "-");
        }
        public new void Dispose()
        {
            OnDispose();
        }
        ~DownloadFileRange()
        {
            OnDispose();
        }

        public void OnDispose()
        {
            m_DownloadSpeed = 0;
            if (m_Fs != null)
            {
                m_Fs.Flush();
                m_Fs.Dispose();
                m_Fs.Close();
                m_Fs = null;
            }
        }


        #region override
        protected override byte[] GetData() { return null; }
        protected override string GetText() { return null; }
        protected override float GetProgress() { return m_TotalFileSize == 0 ? 0 : m_CurFileSize / m_TotalFileSize; }
        protected override bool ReceiveData(byte[] data, int dataLength)
        {
            if (data == null || dataLength == 0 || m_WebRequest.responseCode >= 400)
            {
				UpdateLog.Info("[DownloadFileRange error] ReceiveData return false");
                return false;
            }
            m_Fs.Write(data, 0, dataLength);
            m_CurFileSize += (ulong)dataLength;
            if (UnityEngine.Time.time - m_LastTime >= 0.5f)
            {
                m_DownloadSpeed = (m_CurFileSize - m_LastSize) / (UnityEngine.Time.time - m_LastTime);
                m_LastTime = UnityEngine.Time.time;
                m_LastSize = m_CurFileSize;
                OnDownloadEvent?.Invoke(m_TotalFileSize, m_CurFileSize, m_DownloadSpeed);
            }
            return true;
        }
        protected override void ReceiveContentLengthHeader(ulong contentLength)
        {
            string content_length = m_WebRequest.GetResponseHeader("Content-Length");
			UpdateLog.Info(content_length);
            if (!string.IsNullOrEmpty(content_length))
            {
                try
                {
                    m_TotalFileSize = ulong.Parse(content_length);
                }
                catch (System.Exception e)
                {
					UpdateLog.Info("获取文件长度失败:" + e.Message);
                    m_TotalFileSize = contentLength;
                }
            }
            else
            {
                m_TotalFileSize = contentLength;
            }
            m_TotalFileSize += m_LocalFileSize;
            m_LastTime = UnityEngine.Time.time;
            m_LastSize = m_CurFileSize;

            StartDownloadEvent?.Invoke();
        }
        
        /// <summary>
        /// 下载完成
        /// </summary>
        protected override void CompleteContent()
        {
            base.CompleteContent();
            OnDispose();

            if (File.Exists(m_FileSavePath))
                File.Delete(m_FileSavePath);

            if (File.Exists(m_TempSavePath))
                File.Move(m_TempSavePath, m_FileSavePath);

            DownloadCompleteEvent?.Invoke();
        }

        #endregion
    }
}

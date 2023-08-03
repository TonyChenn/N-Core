using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using NDebug;
using UnityEngine;

namespace NCore.Networking
{
	[APIInfo("N-Core", "Downloader", @"
> 基于HttpWebRequest的多线程断点续传下载器。使用比DownloadFileRange更加复杂，但功能更强大。
> 适用于多文件/大文件多线程断点续传下载
# 主要API
```csharp
// 添加/移除下载单元
void Add(DownloadUnit downloadUnit);
void Add(DownloadUnit[] downloadUnit);
void DeleteDownload(DownloadUnit unit);
void DeleteAll();

// 开始下载任务
void StartAsync();
```
# 使用方式：
```csharp
// 清理下载器
Downloader.Singleton.ClearAllDownload();

// 添加下载文件
for (int i = 0, iMax = modifyList.Count; i < iMax; i++)
{
    // 创建下载单元
    DownloadUnit unit = new DownloadUnit(bundleName, url, savePath, bundleMd5);
    // 添加下载失败回调
    unit.ErrorFun = (msg) =>{};
    // 下载过程回调
    unit.ProgressFun = (curSize, totalSize) =>{};
    // 下载结束回调
    unit.CompleteFun = () =>{};
    
    // 开始异步下载
    Downloader.Singleton.Add(unit);
}

// 开始下载
Downloader.Singleton.StartAsync();
```
")]
	public class Downloader : MonoBehaviour
	{
		private static readonly object _lock = new();     // 线程锁
		public static int MaxRetryCount = 3;            // 最大尝试次数
		public static int MaxThreadsCount = 10;         // 同时最大下载量
		public static int TimeOut = 30;

		private Queue<DownloadFileMac> downloadQueue;   // 下载作业队列
		private List<DownloadUnit> completeList;        // 下载完成列表
		private List<DownloadFileMac> errorList;        // 下载失败列表

		private Dictionary<Thread, DownloadFileMac> threadDict;

		private static Downloader _instance = null;
		public static Downloader Singleton { get { return _instance; } }

		private void Awake()
		{
			_instance = this;
			// 突破Http连接并发数量
			ServicePointManager.DefaultConnectionLimit = 512;
			// https证书相关
			ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidationCallback;
		}
		private void Start()
		{
			downloadQueue = new Queue<DownloadFileMac>(8);
			completeList = new List<DownloadUnit>(16);
			errorList = new List<DownloadFileMac>();
			threadDict = new Dictionary<Thread, DownloadFileMac>(MaxThreadsCount);
		}

		private void Update()
		{
			UpdateComplete();
			UpdateProgress();
			UpdateError();
			UpdateThread();
		}

#if UNITY_EDITOR
		private Vector3 scrollPos;
		private void OnGUI()
		{
			GUILayout.BeginVertical();
			GUILayout.Space(Screen.height / 2);
			GUILayout.Label("下载器任务：");
			GUILayout.Label("线程ID\t资源名\t下载进度");
			GUILayout.BeginScrollView(scrollPos, GUILayout.Height(Screen.height / 2 - 15));
			foreach (var item in threadDict)
			{
				if (item.Key.IsAlive)
				{
					GUILayout.Label($"{item.Key.ManagedThreadId}\t{item.Value.Unit.Name}\t{item.Value.CurSize}/{item.Value.TotalSize}");
				}
			}
			GUILayout.EndScrollView();
			GUILayout.EndVertical();
		}
#endif

		// 添加下载任务
		public void Add(DownloadUnit unit) { lock (_lock) { downloadQueue.Enqueue(new DownloadFileMac(unit)); } }
		public void Add(DownloadUnit[] unit)
		{
			lock (_lock)
			{
				for (int i = 0, iMax = unit.Length; i < iMax; i++)
				{
					downloadQueue.Enqueue(new DownloadFileMac(unit[i]));
				}
			}
		}
		// 停止下载任务
		public void Delete(DownloadUnit unit)
		{
			lock (_lock)
			{
				unit.IsDelete = true;
			}
		}
		// 清理所有下载任务
		public void DeleteAll()
		{
			lock (_lock)
			{
				foreach (var item in downloadQueue)
				{
					if (item != null)
						item.Unit.IsDelete = true;
				}

				foreach (var item in threadDict)
				{
					if (item.Value != null)
					{
						item.Value.Unit.IsDelete = true;
					}
				}

				for (int i = 0, iMax = completeList.Count; i < iMax; i++)
				{
					if (completeList[i] != null)
					{
						completeList[i].IsDelete = true;
					}
				}
			}
		}
		// 开始下载任务
		public void StartAsync()
		{
			if (threadDict.Count < MaxThreadsCount)
			{
				Thread thread = new Thread(DownloadThreadLoop);
				lock (_lock)
				{
					threadDict.Add(thread, null);
				}
				thread.Start();
			}
		}

		private void DownloadThreadLoop()
		{
			while (true)
			{
				DownloadFileMac downloadItem = null;
				lock (_lock)
				{
					if (downloadQueue.Count == 0) break;

					downloadItem = downloadQueue.Dequeue();
					// 没有或取消
					if (downloadItem == null || downloadItem.Unit.IsDelete) continue;

					threadDict[Thread.CurrentThread] = downloadItem;
				}

				downloadItem.Run();

				if (downloadItem.State == DownloadFileState.Complete)
				{
					lock (_lock)
					{
						completeList.Add(downloadItem.Unit);
						threadDict[Thread.CurrentThread] = null;
					}
				}
				else if (downloadItem.State == DownloadFileState.Error)
				{
					lock (_lock)
					{
						downloadQueue.Enqueue(downloadItem);
						if (downloadItem.NeedShowError())
							errorList.Add(downloadItem);
					}
					break;
				}
				else
				{
					Log.Error($"Downloader: Download error State: {downloadItem.State}\t\t{downloadItem.Unit.Name}\t\t{downloadItem.TryCount}");
					Log.Error($"错误原因：{downloadItem.Error}");
					break;
				}
			}
		}

		/// <summary>
		/// 更新线程状态
		/// </summary>
		private void UpdateThread()
		{
			if (downloadQueue.Count == 0 || threadDict.Count == 0) return;

			lock (_lock)
			{
				List<Thread> list = new List<Thread>();
				foreach (var item in threadDict)
				{
					if (item.Key.IsAlive) continue;
					if (item.Value != null)
						downloadQueue.Enqueue(item.Value);

					list.Add(item.Key);
				}

				for (int i = 0, iMax = list.Count; i < iMax; i++)
				{
					threadDict.Remove(list[i]);
					list[i].Abort();
				}
			}
			//如果没有网络，不开启新线程，旧线程会逐个关闭
			if (!HasNetwork()) return;
			if (threadDict.Count >= MaxThreadsCount) return;

			if (downloadQueue.Count > 0)
			{
				var thread = new Thread(DownloadThreadLoop);
				lock (_lock)
				{
					threadDict[thread] = null;
				}
				thread.Start();
			}

		}

		private void UpdateComplete()
		{
			//回调完成
			if (completeList.Count == 0) return;

			DownloadUnit[] completeArr = null;
			lock (_lock)
			{
				completeArr = completeList.ToArray();
				completeList.Clear();
			}

			foreach (var info in completeArr)
			{
				if (info.IsDelete) continue; //已经销毁，不进行回调
				info.IsDelete = true;

				info.ProgressFun?.Invoke(info.Size, info.Size);
				info.CompleteFun?.Invoke();
			}
		}


		private List<DownloadFileMac> tmpDownloadItems;
		private void UpdateProgress()
		{
			//回调进度
			if (threadDict.Count == 0) return;
			tmpDownloadItems ??= new List<DownloadFileMac>(MaxThreadsCount);
			tmpDownloadItems.Clear();
			lock (_lock)
			{
				foreach (DownloadFileMac mac in threadDict.Values)
				{
					if (mac != null && !mac.Unit.IsDelete)
						tmpDownloadItems.Add(mac);
				}
			}

			foreach (DownloadFileMac mac in tmpDownloadItems)
			{
				mac.Unit.ProgressFun?.Invoke(mac.CurSize, mac.TotalSize);
			}
		}
		private void UpdateError()
		{
			//回调error
			if (errorList.Count == 0) return;

			DownloadFileMac[] errorArr = null;
			lock (_lock)
			{
				errorArr = errorList.ToArray();
				errorList.Clear();
			}

			foreach (DownloadFileMac mac in errorArr)
			{
				if (mac.Unit.IsDelete) continue; //已经销毁，不进行回调

				mac.Unit.ErrorFun?.Invoke(mac.Error);
				mac.Error = "";
			}
		}

		/// <summary>
		/// 是否有网络连接
		/// </summary>
		private bool HasNetwork()
		{
			return (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork ||       // 2/3/4G
					Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork);          // wifi
		}

		#region https 证书异常、https连接数量
		private bool RemoteCertificateValidationCallback(System.Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			bool isOk = true;
			// If there are errors in the certificate chain, look at each error to determine the cause.
			if (sslPolicyErrors != SslPolicyErrors.None)
			{
				for (int i = 0; i < chain.ChainStatus.Length; i++)
				{
					if (chain.ChainStatus[i].Status != X509ChainStatusFlags.RevocationStatusUnknown)
					{
						chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
						chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
						chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
						chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
						bool chainIsValid = chain.Build((X509Certificate2)certificate);
						if (!chainIsValid)
						{
							isOk = false;
						}
					}
				}
			}
			return isOk;
		}
		#endregion
	}

	internal class DownloadFileMac
	{
		const int SectionSize = 16 * 1024;          // 下载每一份大小：16KB
		const int TimeOut = 5 * 1000;               // 5秒
		const int ReadWriteTimeout = 2 * 1000;      // 2秒

		public DownloadUnit Unit;

		public ulong CurSize = 0;
		public ulong TotalSize = 0;
		public DownloadFileState State = DownloadFileState.None;
		public int TryCount = 0;
		public string Error;

		public DownloadFileMac(DownloadUnit downloadUnit)
		{
			Unit = downloadUnit;
		}

		public bool NeedShowError()
		{
			return TryCount >= Downloader.MaxRetryCount;
		}

		public async void Run()
		{
			++TryCount;
			// 获取下载资源大小
			State = DownloadFileState.ResetSize;
			if (!ResetSize()) return;

			State = DownloadFileState.Download;
			if (!Download()) return;

			State = DownloadFileState.Md5;
			if (CheckMD5()) //校验失败，重下一次
			{
				State = DownloadFileState.Complete;
			}
			else
			{
				State = DownloadFileState.Download;
				if (!Download()) return;

				State = DownloadFileState.Md5;
				if (!CheckMD5()) return; //两次都失败，文件有问题
			}
			await System.Threading.Tasks.Task.Delay(100);
		}

		// 断点续传文件下载
		public bool Download()
		{
			string tmpFile = $"{Unit.SavePath}.tmp";
			if (!File.Exists(tmpFile))
			{
				string folder = Path.GetDirectoryName(tmpFile);
				if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

				File.Create(tmpFile).Close();
			}

			if (MD5Helper.GetFileMD5(tmpFile) == Unit.MD5)
			{
				SaveTmpFile(tmpFile, Unit.SavePath);
				return true;
			}

			FileStream fs = File.OpenWrite(tmpFile);
			long startPos = fs.Length;
			fs.Seek(startPos, SeekOrigin.Current);
			CurSize = (ulong)startPos;

			HttpWebRequest request = null;
			HttpWebResponse response = null;
			try
			{
				request = WebRequest.Create(Unit.DownloadUrl) as HttpWebRequest;
				request.ReadWriteTimeout = ReadWriteTimeout;
				request.Timeout = TimeOut;
				request.Method = "GET";

				if (startPos > 0) request.AddRange(startPos);

				response = (HttpWebResponse)request.GetResponse();
				using Stream stream = response.GetResponseStream();
				stream.ReadTimeout = TimeOut;
				long totalSize = response.ContentLength;

				byte[] bytes = new byte[SectionSize];
				int offset = -1;
				while ((offset = stream.Read(bytes, 0, SectionSize)) > 0)
				{
					fs?.Write(bytes, 0, offset);
					startPos += offset;

					// 判断是否下载完成
					// 下载完成将temp文件，改成正式文件
					if (startPos == totalSize)
					{
						fs?.Flush(); fs?.Dispose();

						SaveTmpFile(tmpFile, Unit.SavePath);
					}
					CurSize = (ulong)startPos;
				}
			}
			catch (Exception ex)
			{
				fs?.Flush(); fs?.Dispose();
				if (File.Exists(tmpFile)) File.Delete(tmpFile);

				State = DownloadFileState.Error;
				Error = $"Download Error: {ex.Message}";
			}
			finally
			{
				fs?.Dispose();
				request?.Abort();
				response?.Close();
			}

			return State != DownloadFileState.Error;
		}

		/// <summary>
		/// 获取远程文件大小
		/// </summary>
		private long GetRemoteFileSize(string url)
		{
			HttpWebRequest request = null;
			WebResponse response = null;
			long length = 0;
			try
			{
				request = WebRequest.Create(url) as HttpWebRequest;
				request.Timeout = TimeOut;
				request.ReadWriteTimeout = ReadWriteTimeout;
				response = request.GetResponse();
				length = response.ContentLength;
			}
			catch (Exception ex)
			{
				State = DownloadFileState.Error;
				Error = "Request Remote File Size Error: " + ex.Message;
			}
			finally
			{
				request?.Abort(); request = null;
				response?.Close(); response = null;
			}
			return length;
		}

		private bool CheckMD5()
		{
			if (string.IsNullOrEmpty(Unit.MD5)) return true;

			string realMD5 = MD5Helper.GetFileMD5(Unit.SavePath);
			if (realMD5 != Unit.MD5)
			{
				File.Delete(Unit.SavePath);
				State = DownloadFileState.Error;
				Error = $"Check File MD5 Error:{Unit.Name}\t{realMD5}\t{Unit.MD5}";
				return false;
			}
			return true;
		}

		private bool ResetSize()
		{
			if (Unit.Size <= 0)
			{
				Unit.Size = (ulong)GetRemoteFileSize(Unit.DownloadUrl);
				if (Unit.Size == 0) return false;
			}

			CurSize = 0;
			TotalSize = Unit.Size;

			return true;
		}

		private void SaveTmpFile(string tempPath, string savePath)
		{
			if (File.Exists(savePath))
				File.Delete(savePath);

			File.Move(tempPath, savePath);
		}
	}

	internal enum DownloadFileState
	{
		None,
		ResetSize,
		Download,
		Md5,
		Complete,
		Error,
	}
}

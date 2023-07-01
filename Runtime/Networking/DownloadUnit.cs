using System;

namespace NCore.Networking
{
	public class DownloadUnit
	{
		public string Name { get; private set; }
		public string DownloadUrl { get; private set; }
		public string SavePath { get; private set; }
		public string MD5 { get; private set; }
		public ulong Size { get; set; }

		public bool IsDelete { get; set; }

		public DownloadUnit(string name, string url, string savePath, string md5 = null, ulong size = 0)
		{
			Name = name;
			DownloadUrl = url;
			SavePath = savePath;
			MD5 = md5;
			Size = size;
		}

		public Action<string> ErrorFun;
		public Action<ulong, ulong> ProgressFun;
		public Action CompleteFun;
	}
}


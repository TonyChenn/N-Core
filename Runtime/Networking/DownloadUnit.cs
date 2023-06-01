using System;

namespace NCore.Networking
{
    public class DownloadUnit
    {
        public string Name { get; private set; }
        public string SavePath { get; private set; }

        public string MD5 { get; private set; }

        public ulong Size { get; set; }

        public string DownloadUrl { get; private set; }

        public bool IsDelete { get; set; }

        public DownloadUnit(string name, string url, string savePath, string md5 = null)
        {
            Name = name;
            DownloadUrl = url;
            SavePath = savePath;
            MD5 = md5;
        }

        public DownloadErrorCallback ErrorFun;
        public DownloadProgressCallback ProgressFun;
        public DonwloadCompleteCallback CompleteFun;

        public delegate void DownloadErrorCallback(string msg);
        public delegate void DownloadProgressCallback(ulong curSize, ulong totalSize);
        public delegate void DonwloadCompleteCallback();
    }
}


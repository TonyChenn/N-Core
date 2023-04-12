using System.Collections.Generic;

namespace NCore
{
    public class Downloader:NormalSingleton<Downloader>
    {
        public static int MaxRetryCount = 3;
        public static int MaxDownloadThreadsCount = 5;
        public static int MaxDecompressThreadsCount = 5;
        public static int TimeOut = 30;

        // 下载作业队列
        private Queue<BundleUnit> queue = new Queue<BundleUnit>(32);
        private List<BundleUnit> completeList = new List<BundleUnit>(32);
        private List<BundleUnit> errorList = new List<BundleUnit>(8);

        private Downloader() { }

        public void AddDownLoad(BundleUnit item)
        {
            if (item == null) return;

            queue.Enqueue(item);
        }

    }


    enum DownLoadState
    {
        Checking,               // 检查是否有更新
        CheckSucceed,           // 检查完毕
        CheckFailed,            // 检查失败

        Downloading,            // 下载中
        DownloadFailed,         // 下载失败
        DownloadSucceed,        // 所有文件下载完成
        WriteFileFailed,        // 写文件失败

        Decompressing,          // 解压中
        DecompressFinished,     // 解压完成

        Retry,                  // 重新连接中
    }
}
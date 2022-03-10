using System;
using System.Threading;
using UnityEngine;

namespace NCore
{
    public static class SyncContextUtil
    {
        #region API

        /// <summary>
        /// 委托抛到Unity线程运行
        /// </summary>
        public static void RunOnUnityScheduler(Action action)
        {
            if (SynchronizationContext.Current == UnitySynchronizationContext)
            {
                action();
            }
            else
            {
                UnitySynchronizationContext.Post(_ => action(), null);
            }
        }

        #endregion

        // 该属性：使该方法再Awake之前执行
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Install()
        {
            UnitySynchronizationContext = SynchronizationContext.Current;
            UnityThreadId = Thread.CurrentThread.ManagedThreadId;
        }

        /// <summary>
        /// Unity线程ID
        /// </summary>
        private static int UnityThreadId { get; set; }

        /// <summary>
        /// 啥意思？（作用是将一个线程的代码到另一个线程来执行）
        /// </summary>
        private static SynchronizationContext UnitySynchronizationContext { get; set; }
    }
}
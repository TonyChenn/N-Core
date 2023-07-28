using System;
using System.Threading;
using UnityEngine;

namespace NCore
{
    public static class SyncContextUtil
    {
		// 该属性：使该方法再Awake之前执行
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Initalize()
		{
			UnitySynchronizationContext = SynchronizationContext.Current;
			UnityThreadId = Thread.CurrentThread.ManagedThreadId;
		}
		/// <summary>
		/// Unity线程ID
		/// </summary>
		public static int UnityThreadId { get; private set; }

		/// <summary>
		///Unity同步上下文
		/// </summary>
		public static SynchronizationContext UnitySynchronizationContext { get; private set; }


		#region API
		/// <summary>
		/// 委托抛到Unity线程运行(Post 异步)
		/// </summary>
		public static void PostToUnityThread(Action action)
        {
			UnitySynchronizationContext.Post(_ => action(), null);
		}
		/// <summary>
		/// 委托抛到Unity线程运行(Send 同步)
		/// </summary>
		public static void SendToUnityThread(Action action)
		{
			if (SynchronizationContext.Current == UnitySynchronizationContext)
			{
				action();
			}
			else
			{
				UnitySynchronizationContext.Send(_ => action(), null);
			}
		}
		#endregion


	}
}

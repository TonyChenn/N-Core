using NCore;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ThreadHelper
{
	private static ThreadHelper instance = null;
	private static ThreadHelper threadHelper
	{
		get
		{
			if (instance == null)
			{
				instance = new ThreadHelper();
			}
			return instance;
		}
	}

	#region Unity Thread ID
	private int unityThreadID;
	public static int UnityThreadID => threadHelper.unityThreadID;
	#endregion

	#region Unity Thread Context
	private SynchronizationContext unityThreadContext;
	public static SynchronizationContext UnityThreadContext => threadHelper.unityThreadContext;
	#endregion

	private ThreadHelper()
	{
		unityThreadID = Thread.CurrentThread.ManagedThreadId;
		unityThreadContext = SynchronizationContext.Current;
	}

}

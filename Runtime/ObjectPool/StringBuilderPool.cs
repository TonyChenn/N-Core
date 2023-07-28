using System;
using System.Collections.Generic;
using System.Text;

namespace NCore
{
	public class StringBuilderPool
	{
		private static readonly DefaultObjectPool<StringBuilder> pool = new((_builder) =>
		{
			_builder.Remove(0, _builder.Length);
		}, 16);

		private static int MaxCount = 32;

		public static StringBuilder Alloc() => pool.Alloc();

		public static void Recycle(StringBuilder builder)
		{
			if (builder == null) return;
			if (pool.Count >= MaxCount) { builder = null; return; }
			pool.Recycle(builder);
		}
	}

	/// <summary>
	/// StringBuilder 静态拓展
	/// </summary>
	public static class StringBuilderExtention
	{
		public static void Recycle(this StringBuilder builder)
		{
			StringBuilderPool.Recycle(builder);
		}
	}
}

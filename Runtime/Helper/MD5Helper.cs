using System.Collections.Generic;
using UnityEngine.Networking;

namespace NCore
{
	public static class MD5Helper
	{
		#region 计算Hash API
		/// <summary>
		/// 计算字符串的MD5值
		/// </summary>
		public static string GetStrMD5(string source)
		{
			byte[] buffer = System.Text.Encoding.UTF8.GetBytes(source);
			return ComputeHash(buffer);
		}

		/// <summary>
		/// 计算文件的MD5值
		/// </summary>
		public static string GetFileMD5(string assetPath)
		{
			byte[] buffer = ReadFileBytes(assetPath);
			return buffer != null ? ComputeHash(buffer) : null;
		}

		/// <summary>
		/// 计算多个文件的MD5值
		/// </summary>
		public static string GetFilesMD5(string[] assetPath)
		{
			List<byte> list = new List<byte>();
			foreach (string item in assetPath)
			{
				byte[] buffer = ReadFileBytes(item);
				if (buffer != null)
					list.AddRange(buffer);
			}
			return ComputeHash(list.ToArray());
		}

		public static string ComputeHash(byte[] buffer)
		{
			if (buffer == null || buffer.Length < 1) return null;

			var builder = StringBuilderPool.Alloc();
			byte[] hash = System.Security.Cryptography.MD5.Create().ComputeHash(buffer);
			foreach (var b in hash)
			{
				builder.Append(b.ToString("x2"));
			}

			string result = builder.ToString();
			builder.Recycle();
			return result;
		}
		#endregion


		static byte[] ReadFileBytes(string filePath)
		{
			var request = UnityWebRequest.Get(filePath);
			request.SendWebRequest();
			while (!request.isDone) { if (request.error != null) { return null; } }
			if (request.error != null) { return null; }
			return request.downloadHandler.data;
		}
	}
}

using NCore;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;


public static class MD5Helper
{
    private static MD5 md5 = null;
    private static StringBuilder builder = null;

    private static MD5 Md5
    {
        get
        {
            if (md5 == null)
                md5 = MD5.Create();
            return md5;
        }
    }
    private static StringBuilder Builder
    {
        get
        {
            if (builder == null)
                builder = new StringBuilder();
            return builder;
        }
    }

    #region 计算Hash API
    /// <summary>
    /// 计算字符串的MD5值
    /// </summary>
    public static string GetStrMD5(string source)
    {
        byte[] buffer = Encoding.UTF8.GetBytes(source);
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
    public static string GetFilesMD5(string[] assetPathArray)
    {
        List<byte> list = new List<byte>();
        foreach (string assetPath in assetPathArray)
        {
            byte[] buffer = ReadFileBytes(assetPath);
            if (buffer != null)
                list.AddRange(buffer);
        }
        return ComputeHash(list.ToArray());
    }

    public static string ComputeHash(byte[] buffer)
    {
        if (buffer == null || buffer.Length < 1) return null;

        var builder = StringBuilderPool.Alloc();
        byte[] hash = Md5.ComputeHash(buffer);
        foreach (var b in hash)
        {
            string temp = b.ToString("x2");
            builder.Append(temp);

        }

        string result = builder.ToString();
        builder.Recycle();
        return result;
    }
    #endregion


    static byte[] ReadFileBytes(string filePath)
    {
        if (!File.Exists(filePath)) return null;

        List<byte> list = new List<byte>();
        byte[] data = File.ReadAllBytes(filePath);
        list.AddRange(data);

        return list.ToArray();
    }
}
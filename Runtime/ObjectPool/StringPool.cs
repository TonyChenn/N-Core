using NCore;
using System;
using System.Collections.Generic;

[APIInfo("N-Core", "StringPool", @"
## 字符串连接工具
> unsafe

### 使用须知：
需开启不安全代码。是利用指针直接操作内存。
建议新字符串不用后都给`Recycle()`一波。同样长度字符串最多缓存 {MAX_SIZE} 个。

### 优点
1. 优化字符串连接时频繁分配内存问题。
	- 对于已经缓存长度的字符串，利用指针，直接操作内存。
	- 对于未存缓存长度的字符串，使用StringBuilder 进项连接。

### 注意
`Recycle` 的字符串请千万不要再次使用。

`Recycle` 的字符串请千万不要再次使用。

`Recycle` 的字符串请千万不要再次使用。

重要的事情说三遍。

### 主要API
```csharp
// 连接n个字符串
string Concat(params string[] strs);

// 回收缓存字符串
void Recycle(string str);
```

### 示例
```csharp
string origin = ""hello"";
Debug.Log($""回收前：${origin}"");
// origin.Recycle();
StringPool.Recycle(origin);

string str1 = ""oll"";
string str2 = ""eh"";
string result = StringPool.Concat(str1, str2);

Debug.Log($""回收修改后：{origin}"");
Debug.Log($""连接结果：{result}"");

> 回收前：hello
> 回收修改后：olleh
> 连接结果：olleh
```
")]
public static class StringPool
{
	// 每个池子最大数量
	private const int MAX_SIZE = 3;

	private static readonly Lazy<Dictionary<int, Stack<string>>> dict = new Lazy<Dictionary<int, Stack<string>>>();

	public static unsafe string Concat(params string[] strs)
	{
		if (strs == null || strs.Length == 0) return "";
		if (strs.Length == 1) return strs[0];

		int len = 0;
		for (int i = 0, iMax = strs.Length; i < iMax; i++) { len += strs[i].Length; }
		if (len == 0) return "";

		if (!dict.Value.ContainsKey(len)) { dict.Value[len] = new Stack<string>(MAX_SIZE); }

		var stack = dict.Value[len];
		string result = null;

		if (stack.Count == 0)
		{
			var builder = StringBuilderPool.Alloc();
			for (int i = 0, iMax = strs.Length; i < iMax; i++)
			{
				builder.Append(strs[i]);
			}
			result = builder.ToString();
			builder.Recycle();
			return result;
		}
		result = stack.Pop();

		fixed (char* ptr = result)
		{
			char* dest = ptr;
			for (int i = 0, iMax = strs.Length; i < iMax; i++)
			{
				fixed (char* strPtr = strs[i])
				{
					char* tmp = strPtr;
					while (*tmp != '\0')
					{
						*dest = *tmp;
						tmp++;
						dest++;
					}
				}
			}
		}

		return result;
	}

	public static void Recycle(string str)
	{
		if (str.IsNullOrEmpty()) return;

		int len = str.Length;
		if (!dict.Value.ContainsKey(len)) { dict.Value[len] = new Stack<string>(MAX_SIZE); }
		Stack<string> stack = dict.Value[len];

		if (stack.Count < MAX_SIZE) { dict.Value[len].Push(str); }
		else
		{
			// 要不要手动删除对应内存
			//unsafe { }
		}
	}
}

public static class StringPoolEx
{
	public static void Recycle(this string str)
	{
		StringPool.Recycle(str);
	}
}

using System.Collections;
using System.Reflection;
using System.Text;
using UnityEngine;


public class Log
{
	public enum LevelEum { None, OnlyError, Error_Warn, All }

	public static LevelEum Level = LevelEum.All;

	private const string SPACE = " ";

	private static readonly StringBuilder builder = new StringBuilder("", 1024);

	private static int mMaxLayer = 5;

	[System.Diagnostics.DebuggerHidden]
	public static void Info(object message, Object context = null)
	{
		if (Level > LevelEum.None)
		{
			Debug.Log(message, context);
		}
	}

	[System.Diagnostics.DebuggerHidden]
	public static void Info(object message, Color color, Object context = null)
	{
		Info(string.Format("<color=#{0}>{1}</color>", getColorHex(color), message), context);
	}

	[System.Diagnostics.DebuggerHidden]
	public static void RedInfo(object message, Object context = null)
	{
		Info(string.Format("<color=red>{0}</color>", message), context);
	}

	[System.Diagnostics.DebuggerHidden]
	public static void GreenInfo(object message, Object context = null)
	{
		Info(string.Format("<color=green>{0}</color>", message), context);
	}

	[System.Diagnostics.DebuggerHidden]
	public static void Warning(object message, Object context = null)
	{
		if (Level >= LevelEum.Error_Warn)
		{
			Debug.LogWarning(message, context);
		}
	}

	[System.Diagnostics.DebuggerHidden]
	public static void Error(object message, Object context = null)
	{
		if (Level >= LevelEum.OnlyError)
		{
			Debug.LogError(message, context);
		}
	}

	[System.Diagnostics.DebuggerHidden]
	public static void Dump(object obj, int maxLayer = 5)
	{
		mMaxLayer = maxLayer;
		builder.Remove(0, builder.Length);
		builder.AppendLine("dump info ");
		dumpHandler(obj, 1);
		Info(builder.ToString());
	}

	[System.Diagnostics.DebuggerHidden]
	private static void dumpHandler(object obj, int layer)
	{
		if (obj == null)
		{
			builder.Append("null");
			return;
		}
		System.Type type = obj.GetType();
		if (obj is string)
		{
			builder.Append("\"");
			builder.Append(obj);
			builder.Append("\",");
			return;
		}
		if (type.IsValueType)
		{
			builder.Append(obj);
			builder.Append(",");
			return;
		}
		IList list = obj as IList;
		if (list != null)
		{
			builder.AppendLine("[");
			if (layer <= mMaxLayer)
			{
				for (int i = 0; i < list.Count; i++)
				{
					appendSpace(layer);
					builder.Append(i);
					builder.Append(":");
					dumpHandler(list[i], layer + 1);
					builder.AppendLine();
				}
			}
			else
			{
				builder.Append("List Data...");
			}
			builder.Append("]");
			return;
		}
		IDictionary dictionary = obj as IDictionary;
		if (dictionary != null)
		{
			builder.AppendLine("{");
			if (layer <= mMaxLayer)
			{
				foreach (object key in dictionary.Keys)
				{
					builder.Append(key.ToString());
					builder.AppendLine(":");
					dumpHandler(dictionary[key], layer + 1);
					builder.AppendLine();
				}
			}
			else
			{
				builder.Append("Dictionary Data...");
			}
			builder.AppendLine("}");
		}
		else if (type.IsArray)
		{
			System.Array array = obj as System.Array;
			builder.Append("[");
			if (layer <= mMaxLayer)
			{
				for (int j = 0; j < array.Length; j++)
				{
					builder.Append(j);
					builder.Append(" : ");
					dumpHandler(array.GetValue(j), layer + 1);
				}
			}
			else
			{
				builder.Append("Array Data...");
			}
			builder.Append("]");
		}
		else if (type.IsClass)
		{
			builder.AppendLine("{");
			FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
			appendSpace(layer);
			builder.AppendLine("// 字段如下：");
			if (fields.Length != 0)
			{
				if (layer <= mMaxLayer)
				{
					FieldInfo[] array2 = fields;
					foreach (FieldInfo fieldInfo in array2)
					{
						appendSpace(layer);
						builder.Append(fieldInfo.Name);
						builder.Append(" : ");
						dumpHandler(fieldInfo.GetValue(obj), layer + 1);
						builder.AppendLine();
					}
				}
				else
				{
					builder.Append("Class...");
				}
			}
			builder.AppendLine();
			appendSpace(layer);
			builder.AppendLine("// 属性如下：");
			PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
			if (properties.Length != 0)
			{
				if (layer <= mMaxLayer)
				{
					PropertyInfo[] array3 = properties;
					foreach (PropertyInfo propertyInfo in array3)
					{
						builder.Append(propertyInfo.Name);
						builder.Append(" : ");
						dumpHandler(propertyInfo.GetGetMethod().Invoke(obj, null), layer + 1);
						builder.AppendLine();
					}
				}
				else
				{
					builder.Append("Class...");
				}
			}
			builder.Append("}");
		}
		else
		{
			RedInfo("unsupport type:" + type.FullName);
			builder.Append(type.FullName);
			builder.Append(",");
		}
	}

	[System.Diagnostics.DebuggerHidden]
	private static void appendSpace(int layer)
	{
		int i = 0;
		for (int num = layer * 3; i < num; i++)
		{
			builder.Append(" ");
		}
	}

	[System.Diagnostics.DebuggerHidden]
	private static string getColorHex(Color color)
	{
		return ColorUtility.ToHtmlStringRGBA(color);
	}
}

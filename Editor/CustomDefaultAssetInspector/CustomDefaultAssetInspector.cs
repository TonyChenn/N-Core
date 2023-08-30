using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[APIInfo("N-Core", "ICustomDefaultAssetInspector", @"
对DefaultAsset类型资源进行自定义 Inspector 面板，基于反射实现，详细实现参看：CustomDefaultAssetInspector

只需实现接口：ICustomDefaultAssetInspector

示例1：版控文件的预览：
```csharp
public class VersionFileInspector : ICustomDefaultAssetInspector
{
	public bool CanDraw(string path)
	{
		string fileName = Path.GetFileName(path);
		return fileName.StartsWith(""version"") && fileName.EndsWith("".data"");
	}

	public void Draw(string path){ }
}
```
![](https://raw.githubusercontent.com/TonyChenn/BlogPicture/master/2023/q3/custom_default_asset_data.jpg)

示例2：lua文件支持：
```csharp
public class LuaInspector : ICustomDefaultAssetInspector
{
	public bool CanDraw(string path)
	{
		return path.EndsWith("".lua"");
	}

	public void Draw(string path)
	{
		string text = File.ReadAllText(path);
		GUILayout.TextArea(text);
	}
}
```
![](https://raw.githubusercontent.com/TonyChenn/BlogPicture/master/2023/q3/custom_default_asset_lua.jpg)
")]
public interface ICustomDefaultAssetInspector
{
	bool CanDraw(string path);
	void Draw(string path);
}

[CanEditMultipleObjects, CustomEditor(typeof(DefaultAsset))]
public class CustomDefaultAssetInspector : Editor
{
	private readonly Type[] STRING_TYPE = new Type[] { typeof(string) };
	Dictionary<object, Type> dict = new Dictionary<object, Type>();
	private void OnEnable()
	{
		dict.Clear();

		Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
		for (int i = 0; i < assemblies.Length; i++)
		{
			Type[] classArray = assemblies[i].GetTypes();
			foreach (Type clazz in classArray)
			{
				if (clazz.IsInterface || clazz.IsAbstract) continue;
				foreach (Type _interface in clazz.GetInterfaces())
				{
					if (_interface == typeof(ICustomDefaultAssetInspector))
					{
						dict[Activator.CreateInstance(clazz)] = clazz;
						break;
					}
				}
			}
		}
	}
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		string path = AssetDatabase.GetAssetPath(Selection.activeInstanceID);

		foreach (var item in dict)
		{
			object _ins = item.Key;
			Type clazz = item.Value;

			MethodInfo method1 = clazz.GetMethod("CanDraw", STRING_TYPE);
			bool canDraw = (bool)method1.Invoke(_ins, new object[] { path });
			if (canDraw)
			{
				bool enabled = GUI.enabled;
				GUI.enabled = true;
				
				MethodInfo method2 = clazz.GetMethod("Draw", STRING_TYPE);
				method2.Invoke(_ins, new object[] { path });

				GUI.enabled = enabled;
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Video;

[APIInfo("N-Core", "Selector.AssetSelector", @"Editor模式下资源选择工具
接口信息如下：
```csharp
// folder 是筛选的目录,是以""Assets/""开头的相对路径
// filter 过滤规则。必须以""t:""开头。如：t:sprite, t:texture, t:gameobject 等等
// callback 选择之后的回调

static void Show(string filter, Action<string> callback);
static void Show(string folder, string filter, Action<string> callback);
```")]
public class AssetSelector : EditorWindow
{
	private string m_Folder;
	private string m_Filter;
	private Action<string> m_SelectedCallback;

	private static string SelectedItem { get; set; }
	private static string SearchString { get; set; }


	public static void Show(string filter, Action<string> callback)
	{
		Show("Assets/", filter, callback);
	}
	public static void Show(string folder, string filter, Action<string> callback)
	{

		if (!Directory.Exists(folder)) return;
		if (!folder.StartsWith("Assets/")) return;
		if (!filter.StartsWith("t:")) return;

		AssetSelector selector = GetWindow<AssetSelector>(true, "资源选择");
		selector.m_Folder = folder;
		selector.m_Filter = filter;
		selector.m_SelectedCallback = callback;
	}

	private List<string> items;
	private Vector2 scrollPos;
	private void OnGUI()
	{
		if (items == null)
		{
			items = new List<string>();
			RefreshItems();
		}

		Type type = GetAssetType();

		EditorGUIUtility.labelWidth = 80f;
		GUILayout.Label($"请选择{type?.Name}", "LODLevelNotifyText");
		GUILayout.Space(6f);

		GUILayout.BeginHorizontal();
		GUILayout.Space(84f);
		string before = SearchString ?? "";
		string after = EditorGUILayout.TextField("", before, "SearchTextField");
		//搜索框
		if (GUILayout.Button("", "SearchCancelButton", GUILayout.Width(18f)))
		{
			after = "";
			GUIUtility.keyboardControl = 0;
		}
		if (before != after)
		{
			SearchString = after;
			RefreshItems();
		}

		GUILayout.Space(84f);
		GUILayout.EndHorizontal();

		GUILayout.Space(10f);

		if (items.Count > 0)
		{
			scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
			if (type != null)
			{
				DrawItems(type);
			}
			else
			{
				DrawDefault();
			}
			EditorGUILayout.EndScrollView();
		}
	}

	private Type GetAssetType()
	{
		switch (m_Filter)
		{
			case "t:texture":
				return typeof(Texture);
			case "t:sprite":
				return typeof(Sprite);
			case "t:videoclip":
				return typeof(VideoClip);
			default:
				return null;
		}
	}

	private float mClickTime = 0f;
	private void DrawItems(Type type)
	{
		float size = 80f;
		float padded = size + 10f;
		int columns = Mathf.FloorToInt(Screen.width / padded);
		if (columns < 1) columns = 1;

		int offset = 0;
		Rect rect = new Rect(10f, 0, size, size);

		int rows = 1;
		while (offset < items.Count)
		{
			GUILayout.BeginHorizontal();
			{
				int col = 0;
				rect.x = 10f;

				for (; offset < items.Count; ++offset)
				{
					string itemPath = items[offset];
					var item = AssetDatabase.LoadAssetAtPath(itemPath, type);
					if (item == null) continue;

					string itemName = "";

					// Button comes first
					if (GUI.Button(rect, ""))
					{
						if (Event.current.button == 0)
						{
							float delta = Time.realtimeSinceStartup - mClickTime;
							mClickTime = Time.realtimeSinceStartup;

							if (SelectedItem != itemPath)
							{
								SelectedItem = itemPath;
								m_SelectedCallback?.Invoke(itemPath);
							}
						}
					}

					if (Event.current.type == EventType.Repaint)
					{
						EditorUtil.DrawTiledTexture(rect, EditorUtil.backdropTexture);
						Rect clipRect = rect;

						if (item is Texture texture)
						{
							GUI.DrawTexture(clipRect, texture);
							itemName = texture.name;
						}
						else if (item is Sprite sprite)
						{
							GUI.DrawTexture(clipRect, sprite.texture);
							itemName = sprite.texture.name;
						}
						else if (item is VideoClip video)
						{
							Texture2D tex = AssetPreview.GetMiniThumbnail(video);
							GUI.DrawTexture(clipRect, tex);
							itemName = video.name;
						}

						// Draw the selection
						if (SelectedItem == itemPath)
						{
							EditorUtil.DrawOutline(rect, new Color(0.4f, 1f, 0f, 1f));
						}
					}

					GUI.backgroundColor = new Color(1f, 1f, 1f, 0.5f);
					GUI.contentColor = new Color(1f, 1f, 1f, 0.7f);
					GUI.Label(new Rect(rect.x, rect.y + rect.height, rect.width, 32f), itemName, "ProgressBarBack");
					GUI.contentColor = Color.white;
					GUI.backgroundColor = Color.white;

					if (++col >= columns)
					{
						++offset;
						break;
					}
					rect.x += padded;
				}
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(padded);
			rect.y += padded + 26;
			++rows;
		}
		GUILayout.Space(rows * 26);
	}

	private void DrawDefault()
	{
		for (int i = 0, iMax = items.Count; i < iMax; i++)
		{
			string assetName = Path.GetFileName(items[i]);
			GUILayout.BeginHorizontal();
			{
				GUI.contentColor = new Color(1f, 1f, 1f);
				GUILayout.Button(assetName, "TextArea", GUILayout.Width(160f), GUILayout.Height(20f));
				GUILayout.Button(items[i].Replace("Assets/", ""), "TextArea", GUILayout.Height(20f));
				GUI.contentColor = Color.white;
				if (GUILayout.Button("选择", GUILayout.Width(100)))
				{
					m_SelectedCallback?.Invoke(items[i]);
				}
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(1);
		}
	}

	private void RefreshItems()
	{
		items.Clear();

		string searchString = SearchString ?? "";
		string[] keywords = searchString.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
		for (int i = 0, iMax = keywords.Length; i < iMax; i++) keywords[i] = keywords[i].ToLower();

		string[] guids = AssetDatabase.FindAssets(m_Filter, new string[] { m_Folder });
		foreach (var item in guids)
		{
			string assetPath = AssetDatabase.GUIDToAssetPath(item);
			string assetName = Path.GetFileNameWithoutExtension(assetPath);
			if (string.Equals(searchString, assetName, StringComparison.OrdinalIgnoreCase))
			{
				items.Add(assetPath);
				continue;
			}
			int matchs = 0;
			foreach (var keyword in keywords) { if (assetName.Contains(keyword)) ++matchs; }
			if (matchs == keywords.Length) { items.Add(assetPath); }
		}
	}
}

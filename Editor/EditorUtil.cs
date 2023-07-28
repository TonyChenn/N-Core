using System;
using UnityEditor;
using UnityEngine;


public class EditorUtil
{

	[MenuItem("Tools/Open../StreammingAsset")]
	private static void OpenStreammingAssetFolder() { OpenFolder(Application.streamingAssetsPath); }
	[MenuItem("Tools/Open../可读写目录")]
	private static void OpenPersistentFolder() { OpenFolder(Application.persistentDataPath); }


	/// <summary>
	/// 打开文件夹
	/// </summary>
	public static void OpenFolder(string folderPath)
	{
		Application.OpenURL($"file:///{folderPath}");
	}


	/// <summary>
	/// Draws the tiled texture. Like GUI.DrawTexture() but tiled instead of stretched.
	/// </summary>
	public static void DrawTiledTexture(Rect rect, Texture texture)
	{
		GUI.BeginGroup(rect);
		{
			int width = Mathf.RoundToInt(rect.width);
			int height = Mathf.RoundToInt(rect.height);

			for (int y = 0; y < height; y += texture.height)
			{
				for (int x = 0; x < width; x += texture.width)
				{
					GUI.DrawTexture(new Rect(x, y, texture.width, texture.height), texture);
				}
			}
		}
		GUI.EndGroup();
	}



	private static Texture2D mBackdropTex;
	/// <summary>
	/// 返回一个看起来像深色棋盘的可用纹理。
	/// </summary>
	public static Texture2D backdropTexture
	{
		get
		{
			if (mBackdropTex == null) mBackdropTex = CreateCheckerTex(
				new Color(0.1f, 0.1f, 0.1f, 0.5f),
				new Color(0.2f, 0.2f, 0.2f, 0.5f));
			return mBackdropTex;
		}
	}
	/// <summary>
	/// Create a checker-background texture
	/// </summary>
	private static Texture2D CreateCheckerTex(Color c0, Color c1)
	{
		Texture2D tex = new Texture2D(16, 16);
		tex.name = "[Generated] Checker Texture";
		tex.hideFlags = HideFlags.DontSave;

		for (int y = 0; y < 8; ++y) for (int x = 0; x < 8; ++x) tex.SetPixel(x, y, c1);
		for (int y = 8; y < 16; ++y) for (int x = 0; x < 8; ++x) tex.SetPixel(x, y, c0);
		for (int y = 0; y < 8; ++y) for (int x = 8; x < 16; ++x) tex.SetPixel(x, y, c0);
		for (int y = 8; y < 16; ++y) for (int x = 8; x < 16; ++x) tex.SetPixel(x, y, c1);

		tex.Apply();
		tex.filterMode = FilterMode.Point;
		return tex;
	}


	/// <summary>
	/// Draw a single-pixel outline around the specified rectangle.
	/// </summary>
	public static void DrawOutline(Rect rect, Color color)
	{
		if (Event.current.type == EventType.Repaint)
		{
			Texture2D tex = EditorGUIUtility.whiteTexture;
			GUI.color = color;
			GUI.DrawTexture(new Rect(rect.xMin, rect.yMin, 1f, rect.height), tex);
			GUI.DrawTexture(new Rect(rect.xMax, rect.yMin, 1f, rect.height), tex);
			GUI.DrawTexture(new Rect(rect.xMin, rect.yMin, rect.width, 1f), tex);
			GUI.DrawTexture(new Rect(rect.xMin, rect.yMax, rect.width, 1f), tex);
			GUI.color = Color.white;
		}
	}
}

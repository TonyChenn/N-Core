using System.IO;
using UnityEngine;

/// <summary>
/// CSV文件Inspector支持
/// </summary>
public class CsvFileInspector : ICustomDefaultAssetInspector
{
	public bool CanDraw(string path)
	{
		return path.ToLower().EndsWith(".csv");
	}

	public void Draw(string path)
	{
		int line = 0;
		using var sr = new StreamReader(path);
		string str_line = "";
		while ((str_line = sr.ReadLine()) != null && line < 100)
		{
			string[] cells = str_line.Split(',');
			GUILayout.BeginHorizontal();
			for (int i = 0; i < cells.Length; i++)
			{
				GUILayout.Label(cells[i],GUILayout.Width(180));
			}
			GUILayout.EndHorizontal();
			line++;
		}
	}
}

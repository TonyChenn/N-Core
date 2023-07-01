using NCore.Editor;
using UnityEditor;
using UnityEngine;

public class Path_NCore : IPathConfig, IEditorPrefs
{
	#region IPathConfig
	private const string TAG = "NCore";
	public string GetModuleName() => TAG;
	#endregion


	#region IEditorPrefs
	private static readonly string default_script_folder = $"{Application.dataPath}/Scripts";

	[SettingProperty(FieldType.Folder, "Script目录")]
	public static string DefaultScriptFolder
	{
		get { return EditorPrefsHelper.GetString("Path_NCore_DefaultScriptFolder", default_script_folder); }
		set
		{
			if (!value.StartsWith(Application.dataPath))
			{
				EditorUtility.DisplayDialog("提示", "目录不合法!!!", "好的");
				return;
			}
			EditorPrefsHelper.SetString("Path_NCore_DefaultScriptFolder", value);
		}
	}

	public void ReleaseEditorPrefs()
	{
		EditorPrefsHelper.DeleteKey("Path_NCore_DefaultScriptFolder");
	}
	#endregion
}

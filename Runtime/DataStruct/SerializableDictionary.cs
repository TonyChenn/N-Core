using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// SerializableSDictionary，可序列化的Dictionary类
/// </summary>
[Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
	[Serializable] public struct Pair { public TKey key; public TValue value; }
	/// <summary>
	/// 是否允许在Inspector里编辑Map的内容<br/>
	/// （此变量可以从Inspector里修改）
	/// </summary>
	[SerializeField] private bool editable = false;

	/// <summary>
	/// 用于存储Dictionary数据的列表
	/// </summary>
	[SerializeField] private List<Pair> pairs = new();

	public void OnAfterDeserialize()
	{
		if (!editable) return;

		Clear();

		foreach (Pair el in pairs)
		{
#if UNITY_EDITOR

#endif
			this[el.key] = el.value;
		}
	}

	public void OnBeforeSerialize()
	{
		pairs.Clear();

		foreach (var kv in this)
			pairs.Add(new Pair { key = kv.Key, value = kv.Value });
	}

}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(SerializableDictionary<,>.Pair))]
public class SerializableDictionaryPairEditor : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		var TKey = property.FindPropertyRelative("key");
		var TValue = property.FindPropertyRelative("value");

		Rect rectKey	= new(position.x, position.y, 100, position.height);
		Rect rectValue	= new(position.x + 120, position.y, position.width-120, position.height);
		EditorGUI.PropertyField(rectKey, TKey, GUIContent.none);
		EditorGUI.PropertyField(rectValue, TValue, GUIContent.none);
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		var key = EditorGUI.GetPropertyHeight(property.FindPropertyRelative("key"), true);
		var value = EditorGUI.GetPropertyHeight(property.FindPropertyRelative("value"), true);
		return Mathf.Max(key, value);
	}
}

[CustomPropertyDrawer(typeof(SerializableDictionary<,>))]
public class SerializableDictionaryEditor : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		var editable = property.FindPropertyRelative("editable");
		var pairs = property.FindPropertyRelative("pairs");

		var rect = new Rect(position.x,position.y, position.width, position.height);
		EditorGUI.PropertyField(rect, editable);

		var field = new GUIContent(label.text, null, label.tooltip);
		EditorGUI.PropertyField(rect, pairs, field, true);
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		return EditorGUI.GetPropertyHeight(property.FindPropertyRelative("pairs"));
	}
}
#endif

using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;
using Object = UnityEngine.Object;

namespace NCore.Editor
{
    public interface IEditorPrefs
    {
        /// <summary>
        /// 释放EditorPrefs
        /// </summary>
        void ReleaseEditorPrefs();
    }

    public class EditorPrefsHelper : IEditorPrefs
    {
        private EditorPrefsHelper(){}

        [MenuItem("Tools/Prefs/Clear All EditorPrefs")]
        static void ClearAllEditorPrefs()
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0, iMax = assemblies.Length; i < iMax; i++)
            {
                ClearAssemblyEditorPrefs(assemblies[i]);
            }
        }

        /// <summary>
        /// 删除一个程序集中的EditorPrefs
        /// </summary>
        /// <param name="assembly"></param>
        static void ClearAssemblyEditorPrefs(Assembly assembly)
        {
            Type[] types = assembly.GetTypes();
            for (int i = 0, iMax = types.Length; i < iMax; i++)
            {
                //如果是接口
                if (types[i].IsInterface) continue;

                Type[] ins = types[i].GetInterfaces();
                foreach (var item in ins)
                {
                    if (item == typeof(IEditorPrefs))
                    {
                        object o = Activator.CreateInstance(types[i]);
                        MethodInfo method = item.GetMethod("ReleaseEditorPrefs");
                        method?.Invoke(o, null);
                        break;
                    }
                }
            }
        }

        #region Key

        static List<string> key_obj_list = new List<string>();

        #endregion


        #region Generic Get and Set methods

        // bool
        public static bool GetBool(string prefsKey, bool defaultValue)
        {
            return EditorPrefs.GetBool(prefsKey, defaultValue);
        }

        public static void SetBool(string prefsKey, bool val)
        {
            EditorPrefs.SetBool(prefsKey, val);
        }

        // int
        public static int GetInt(string prefsKey, int defaultValue)
        {
            return EditorPrefs.GetInt(prefsKey, defaultValue);
        }

        public static void SetInt(string prefsKey, int val)
        {
            EditorPrefs.SetInt(prefsKey, val);
        }

        // float
        public static float GetFloat(string prefsKey, float defaultValue)
        {
            return EditorPrefs.GetFloat(prefsKey, defaultValue);
        }

        public static void SetFloat(string prefsKey, float val)
        {
            EditorPrefs.SetFloat(prefsKey, val);
        }

        // color
        public static Color GetColor(string prefsKey, Color c)
        {
            string strVal = GetString(prefsKey, c.r + " " + c.g + " " + c.b + " " + c.a);
            string[] parts = strVal.Split(' ');

            if (parts.Length != 4) return c;
            float.TryParse(parts[0], out c.r);
            float.TryParse(parts[1], out c.g);
            float.TryParse(parts[2], out c.b);
            float.TryParse(parts[3], out c.a);

            return c;
        }

        public static void SetColor(string prefsKey, Color c)
        {
            SetString(prefsKey, c.r + " " + c.g + " " + c.b + " " + c.a);
        }

        // enum
        public static T GetEnum<T>(string prefsKey, T defaultValue)
        {
            string val = GetString(prefsKey, defaultValue.ToString());
            string[] names = System.Enum.GetNames(typeof(T));
            System.Array values = System.Enum.GetValues(typeof(T));

            for (int i = 0; i < names.Length; ++i)
            {
                if (names[i] == val)
                    return (T) values.GetValue(i);
            }

            return defaultValue;
        }

        public static void SetEnum(string prefsKey, System.Enum val)
        {
            SetString(prefsKey, val.ToString());
        }

        // string
        public static string GetString(string prefsKey, string defaultValue)
        {
            return EditorPrefs.GetString(prefsKey, defaultValue);
        }

        public static void SetString(string prefsKey, string val)
        {
            EditorPrefs.SetString(prefsKey, val);
        }

        //Object
        public static T Get<T>(string prefsKey, T defaultValue) where T : Object
        {
            if (!key_obj_list.Contains(prefsKey))
                key_obj_list.Add(prefsKey);

            string path = EditorPrefs.GetString(prefsKey);
            if (string.IsNullOrEmpty(path)) return null;

            T retVal = LoadAsset<T>(path);

            if (retVal == null)
            {
                int id;
                if (int.TryParse(path, out id))
                    return EditorUtility.InstanceIDToObject(id) as T;
            }

            return retVal;
        }

        /// <summary>
        /// 加载Asset
        /// </summary>
        static T LoadAsset<T>(string path) where T : Object
        {
            if (string.IsNullOrEmpty(path)) return null;

            Object obj = AssetDatabase.LoadMainAssetAtPath(path);
            if (obj == null) return null;

            T val = obj as T;
            if (val != null) return val;

            if (typeof(T).IsSubclassOf(typeof(Component)))
            {
                if (obj is GameObject)
                {
                    GameObject go = obj as GameObject;
                    return go.GetComponent(typeof(T)) as T;
                }
            }

            return null;
        }

        public static void SetObject(string prefsKey, Object obj)
        {
            if (!key_obj_list.Contains(prefsKey))
                key_obj_list.Add(prefsKey);

            if (obj == null)
            {
                EditorPrefs.DeleteKey(prefsKey);
            }
            else
            {
                if (obj != null)
                {
                    string path = AssetDatabase.GetAssetPath(obj);
                    EditorPrefs.SetString(prefsKey,
                        string.IsNullOrEmpty(path) ? obj.GetInstanceID().ToString() : path);
                }
                else EditorPrefs.DeleteKey(prefsKey);
            }
        }

        #endregion


        public static bool HasKey(string key)
        {
            return EditorPrefs.HasKey(key);
        }

        public static void DeleteKey(string prefsKey)
        {
            EditorPrefs.DeleteKey(prefsKey);
        }

        #region IEditorPrefs

        public void ReleaseEditorPrefs()
        {
            for (int i = 0, iMax = key_obj_list.Count; i < iMax; i++)
                EditorPrefs.DeleteKey(key_obj_list[i]);
        }

        #endregion
    }
}
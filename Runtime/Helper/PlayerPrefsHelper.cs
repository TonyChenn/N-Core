using UnityEngine;

namespace NCore
{
    public static class PlayerPrefsHelper
    {
        #region Generic Get and Set methods

        // bool
        public static bool GetBool(string prefsKey, bool defaultValue)
        {
            if (!PlayerPrefs.HasKey(prefsKey)) return defaultValue;
            return PlayerPrefs.GetInt(prefsKey) == 1;
        }
        public static void SetBool(string prefsKey, bool val)
        {
            int v = val ? 1 : 0;
            PlayerPrefs.SetInt(prefsKey, v);
        }

        // int
        public static int GetInt(string prefsKey, int defaultValue)
        {
            return PlayerPrefs.GetInt(prefsKey, defaultValue);
        }
        public static void SetInt(string prefsKey, int val)
        {
            PlayerPrefs.SetInt(prefsKey, val);
        }

        // float
        public static float GetFloat(string prefsKey, float defaultValue)
        {
            return PlayerPrefs.GetFloat(prefsKey, defaultValue);
        }
        public static void SetFloat(string prefsKey, float val)
        {
            PlayerPrefs.SetFloat(prefsKey, val);
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
            return PlayerPrefs.GetString(prefsKey, defaultValue);
        }
        public static void SetString(string prefsKey, string val)
        {
            PlayerPrefs.SetString(prefsKey, val);
        }
        #endregion
        
        public static bool HasKey(string key)
        {
            return PlayerPrefs.HasKey(key);
        }

        public static void DeleteKey(string prefsKey)
        {
            PlayerPrefs.DeleteKey(prefsKey);
        }
    }
}
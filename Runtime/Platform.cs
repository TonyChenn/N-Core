namespace NCore
{
    public class Platform
    {
        /// <summary>
        /// 是否是编辑器模式
        /// </summary>
        public static bool IsEditor
        {
            get
            {
#if UNITY_EDITOR
                return true;
#else
                return false;
#endif
            }
        }

        /// <summary>
        /// 是否是 Android 平台
        /// </summary>
        public static bool IsAndroid
        {
            get
            {
#if UNITY_ANDROID
				return true;
#else
                return false;
#endif
            }
        }

        /// <summary>
        /// 是否是 IOS 平台
        /// </summary>
        public static bool IsIOS
        {
            get
            {
#if UNITY_IOS
				return true;
#else
                return false;
#endif
            }
        }
    }
}
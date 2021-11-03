using System;
using System.Reflection;
using UnityEngine;
using UObject = UnityEngine.Object;

namespace NextFramework.Core
{
    public interface ISingleton
    {
        void InitSingleton();
    }

    public abstract class NormalSingleton<T> : ISingleton
        where T : NormalSingleton<T>
    {
        private static object mLock = new object();
        protected static T mInstance = null;

        public static T Singlton
        {
            get
            {
                if (mInstance == null)
                {
                    lock (mLock)
                    {
                        if (mInstance == null)
                        {
                            mInstance = SingletonCreater.CreateSingleton<T>();
                        }
                    } 
                }
                return mInstance;
            }
        }

        public virtual void InitSingleton()
        {
        }

        public virtual void Dispose()
        {
            mInstance = null;
        }
    }

    public static class SingletonCreater
    {
        public static T CreateSingleton<T>() where T : class, ISingleton
        {
#if UNITY_EDITOR
            // 获取私有构造函数
            var ctors = typeof(T).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);
            // 获取无参构造函数
            var ctor = Array.Find(ctors, c => c.GetParameters().Length == 0);
            if (ctor == null)
            {
                throw new Exception("Non-Public Constructor() not found! in " + typeof(T));
            }

            // 通过构造函数，常见实例
            var ins = ctor.Invoke(null) as T;
            ins?.InitSingleton();
            return ins;
#else
            var ins = (T)Activator.CreateInstance(typeof(T), true);
            ins?.InitSingleton();
            return ins;
#endif
        }

        public static class NormalSingltonProperty<T> where T : class, ISingleton
        {
            static T mInstance;
            static readonly object mLock = new object();

            public static T Singlton
            {
                get
                {
                    lock (mLock)
                    {
                        if (mInstance == null)
                            mInstance = SingletonCreater.CreateSingleton<T>();
                    }

                    return mInstance;
                }
            }

            public static void Dispose()
            {
                mInstance = null;
            }
        }

        public static T CreateMonoSingleton<T>() where T : MonoBehaviour, ISingleton
        {
            T _instence = UObject.FindObjectOfType(typeof(T)) as T;
            if (_instence != null)
                return _instence;

            string name = typeof(T).ToString();
            GameObject obj = new GameObject(name);
            _instence = obj.AddComponent<T>();
            UObject.DontDestroyOnLoad(obj);

            _instence.InitSingleton();
            return _instence;
        }
    }

    /// <summary>
    /// MonoSinglton
    /// </summary>
    public abstract class MonoSinglton<T> : MonoBehaviour where T : MonoBehaviour, ISingleton
    {
        static T _instance = null;

        public static T Singlton
        {
            get
            {
                if (_instance == null)
                    _instance = SingletonCreater.CreateMonoSingleton<T>();

                return _instance;
            }
        }

        protected virtual void Dispose()
        {
            Destroy(this);
            _instance = null;
        }

        protected virtual void OnDestroy()
        {
            Dispose();
        }
    }
}
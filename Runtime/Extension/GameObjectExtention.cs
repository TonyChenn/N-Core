using System;
using UnityEngine;

namespace NextFramework.Core
{
    public static class GameObjectExtention
    {
        public static GameObject SetLocalPosX(this GameObject gameObject, float x)
        {
            return gameObject.transform.SetLocalPosX(x).gameObject;
        }
        public static GameObject SetLocalPosY(this GameObject gameObject, float y)
        {
            return gameObject.transform.SetLocalPosY(y).gameObject;
        }
        public static GameObject SetLocalPosZ(this GameObject gameObject, float z)
        {
            return gameObject.transform.SetLocalPosZ(z).gameObject;
        }
        

        public static GameObject SetPosX(this GameObject gameObject, float x)
        {
            return gameObject.transform.SetPosX(x).gameObject;
        }
        public static GameObject SetPosY(this GameObject gameObject, float y)
        {
            return gameObject.transform.SetPosY(y).gameObject;
        }
        public static GameObject SetPosZ(this GameObject gameObject, float z)
        {
            return gameObject.transform.SetPosZ(z).gameObject;
        }

        
        public static GameObject SetLocalScaleX(this GameObject gameObject, float xScale)
        {
            return gameObject.transform.SetLocalScaleX(xScale).gameObject;
        }
        public static GameObject SetLocalScaleY(this GameObject gameObject, float yScale)
        {
            return gameObject.transform.SetLocalScaleY(yScale).gameObject;
        }
        public static GameObject SetLocalScaleZ(this GameObject gameObject, float zScale)
        {
            return gameObject.transform.SetLocalScaleZ(zScale).gameObject;
        }

        /// <summary>
        /// 重置Trans
        /// </summary>
        [Obsolete("方法过时，使用 Identity 代替")]
        public static GameObject Reset(this GameObject gameObject)
        {
            return gameObject.transform.Identity().gameObject;
        }
        public static GameObject Identity(this GameObject gameObject)
        {
            return gameObject.transform.Identity().gameObject;
        }

        
        public static GameObject SetParent(this GameObject gameObject, GameObject parent)
        {
            gameObject.transform.SetParent(parent.transform);
            return gameObject;
        }

        
        /// <summary>
        /// 获取Compoment,如果不存在就添加
        /// </summary>
        public static T GetOrAddCompoment<T>(this GameObject gameObject) where T : Component
        {
            T comp = gameObject.GetComponent<T>();
            if (comp == null)
                comp = gameObject.AddComponent<T>();
            return comp;
        }
        /// <summary>
        /// 获取Compoment,如果不存在就添加
        /// </summary>
        public static Component GetOrAddCompoment(this GameObject gameObject, Type type)
        {
            Component comp = gameObject.GetComponent(type);
            if (comp == null)
                comp = gameObject.AddComponent(type);
            return comp;
        }
    }
}
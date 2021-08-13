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
        public static GameObject SetPosX(this GameObject gameObject, float x)
        {
            return gameObject.transform.SetPosX(x).gameObject;
        }
        public static GameObject SetPosY(this GameObject gameObject, float y)
        {
            return gameObject.transform.SetPosY(y).gameObject;
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
    }
}
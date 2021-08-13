using System;
using UnityEngine;

namespace NextFramework.Core
{
    /// <summary>
    /// Transform 拓展方法
    /// </summary>
    public static class TransformExtension
    {
        public static Transform SetLocalPosX(this Transform trans, float x)
        {
            var pos = trans.localPosition;
            pos.x = x;
            trans.localPosition = pos;
            return trans;
        }
        public static Transform SetLocalPosY(this Transform trans, float y)
        {
            var pos = trans.localPosition;
            pos.y = y;
            trans.localPosition = pos;
            return trans;
        }
        public static Transform SetPosX(this Transform trans, float x)
        {
            var pos = trans.position;
            pos.x = x;
            trans.position = pos;
            return trans;
        }
        public static Transform SetPosY(this Transform trans, float y)
        {
            var pos = trans.position;
            pos.y = y;
            trans.position = pos;
            return trans;
        }

        /// <summary>
        /// 重置Trans
        /// </summary>
        [Obsolete("方法过时，使用 Identity 代替")]
        public static Transform Reset(this Transform trans)
        {
            return trans.Identity();
        }

        public static Transform Identity(this Transform trans)
        {
            trans.localPosition=Vector3.zero;
            trans.localRotation=Quaternion.identity;
            trans.localScale=Vector3.one;
            return trans;
        }
    }
}

using System;
using UnityEngine;

namespace SFramework.Core
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
        public static Transform SetLocalPosZ(this Transform trans, float z)
        {
            var pos = trans.localPosition;
            pos.z = z;
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
        public static Transform SetPosZ(this Transform trans, float z)
        {
            var pos = trans.position;
            pos.z = z;
            trans.position = pos;
            return trans;
        }
        
        
        public static Transform SetLocalScaleX(this Transform trans, float xScale)
        {
            Vector3 v3 = trans.localScale;
            v3.x = xScale;
            trans.localScale = v3;
            return trans;
        }
        public static Transform SetLocalScaleY(this Transform trans, float yScale)
        {
            Vector3 v3 = trans.localScale;
            v3.y = yScale;
            trans.localScale = v3;
            return trans;
        }
        public static Transform SetLocalScaleZ(this Transform trans, float zScale)
        {
            Vector3 v3 = trans.localScale;
            v3.z = zScale;
            trans.localScale = v3;
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
        
        /// <summary>
        /// 获取Compoment,如果不存在就添加
        /// </summary>
        public static T GetOrAddCompoment<T>(this Transform trans) where T : Component
        {
            return trans.gameObject.GetOrAddCompoment<T>();
        }
        /// <summary>
        /// 获取Compoment,如果不存在就添加
        /// </summary>
        public static Component GetOrAddCompoment(this Transform trans, Type type)
        {
            return trans.gameObject.GetOrAddCompoment(type);
        }
    }
}

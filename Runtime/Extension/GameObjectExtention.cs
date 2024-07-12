using System;
using UnityEngine;

namespace NCore
{
	public static class GameObjectExtention
	{
		public static GameObject SetLocalPosX(this GameObject gameObject, float x)
		{
			gameObject.transform.SetLocalPosX(x);
			return gameObject;
		}
		public static GameObject SetLocalPosY(this GameObject gameObject, float y)
		{
			gameObject.transform.SetLocalPosY(y);
			return gameObject;
		}
		public static GameObject SetLocalPosZ(this GameObject gameObject, float z)
		{
			gameObject.transform.SetLocalPosZ(z);
			return gameObject;
		}


		public static GameObject SetPosX(this GameObject gameObject, float x)
		{
			gameObject.transform.SetPosX(x);
			return gameObject;
		}
		public static GameObject SetPosY(this GameObject gameObject, float y)
		{
			gameObject.transform.SetPosY(y);
			return gameObject;
		}
		public static GameObject SetPosZ(this GameObject gameObject, float z)
		{
			gameObject.transform.SetPosZ(z);
			return gameObject;
		}


		public static GameObject SetLocalScaleX(this GameObject gameObject, float xScale)
		{
			gameObject.transform.SetLocalScaleX(xScale);
			return gameObject;
		}
		public static GameObject SetLocalScaleY(this GameObject gameObject, float yScale)
		{
			gameObject.transform.SetLocalScaleY(yScale);
			return gameObject;
		}
		public static GameObject SetLocalScaleZ(this GameObject gameObject, float zScale)
		{
			gameObject.transform.SetLocalScaleZ(zScale);
			return gameObject;
		}

		/// <summary>
		/// 重置Trans
		/// </summary>
		[Obsolete("方法过时，使用 Identity 代替")]
		public static GameObject Reset(this GameObject gameObject)
		{
			gameObject.transform.Identity();
			return gameObject;
		}
		public static GameObject Identity(this GameObject gameObject)
		{
			gameObject.transform.Identity();
			return gameObject;
		}

		public static GameObject SetParent(this GameObject gameObject, GameObject parent)
		{
			gameObject.transform.SetParent(parent.transform);
			return gameObject;
		}

		public static void Destory(this GameObject gameObject)
		{
			GameObject.Destroy(gameObject);
		}


		/// <summary>
		/// 获取Compoment,如果不存在就添加
		/// </summary>
		public static T GetOrAddCompoment<T>(this GameObject gameObject) where T : Component
		{
			return gameObject.GetComponent<T>() ?? gameObject.AddComponent<T>();
		}
		/// <summary>
		/// 获取Compoment,如果不存在就添加
		/// </summary>
		public static Component GetOrAddCompoment(this GameObject gameObject, Type type)
		{
			return gameObject.GetComponent(type) ?? gameObject.AddComponent(type);
		}
	}
}

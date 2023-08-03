using UnityEngine;

namespace NCore
{
    public class AsyncCoroutine : MonoSinglton<AsyncCoroutine>
    {
		private void Awake()
		{
			gameObject.name = "[AsyncCoroutine]";
		}
    }
}

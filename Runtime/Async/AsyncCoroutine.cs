using UnityEngine;

namespace NextFramework.Core
{
    public class AsyncCoroutine : MonoSinglton<AsyncCoroutine>, ISingleton
    {
        public void InitSingleton()
        {
            gameObject.name = "[AsyncCoroutine]";
        }
    }
}
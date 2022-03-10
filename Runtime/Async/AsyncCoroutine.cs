using UnityEngine;

namespace NCore
{
    public class AsyncCoroutine : MonoSinglton<AsyncCoroutine>, ISingleton
    {
        public void InitSingleton()
        {
            gameObject.name = "[AsyncCoroutine]";
        }
    }
}
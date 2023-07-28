using System;
using UnityEngine;

namespace NCore
{
    /// <summary>
    /// GameObject对象池
    /// </summary>

    internal class GameObjectPool<T> : PoolBase<T> where T: Component
    {
        private readonly Action<T> resetAction;
        private readonly Func<T> createFunc;

        public GameObjectPool(Func<T> createFunc, Action<T> resetAction, int initCount = 0)
            :base(initCount)
        {
            this.createFunc = createFunc;
            this.resetAction = resetAction;
        }

        public override T Create()
        {
            if (createFunc == null) return default(T);

            return createFunc();
        }

        public override bool Recycle(T obj)
        {
            resetAction?.Invoke(obj);
            dataStack.Push(obj);

            return true;
        }


        public override void Destory()
        {
            while (dataStack.Count > 0)
            {
                T obj = dataStack.Pop();
                GameObject.Destroy(obj as GameObject);
            }

            dataStack = null;
        }
    }
}
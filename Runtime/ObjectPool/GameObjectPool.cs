using System;

namespace NCore
{
    /// <summary>
    /// GameObject对象池
    /// </summary>
    public class GameObjectPool<T> : PoolBase<T>
    {
        Action<T> resetAction;

        public GameObjectPool(Func<T> factoryFunc, Action<T> resetAction, int initCount = 0)
        {
            factory = new CustomObjectFactory<T>(factoryFunc);
            this.resetAction = resetAction;
            for (int i = 0, iMax = initCount; i < iMax; i++)
            {
                Recycle(factory.Create());
            }
        }

        public override bool Recycle(T obj)
        {
            resetAction?.Invoke(obj);

            dataStack.Push(obj);

            return true;
        }
    }
}
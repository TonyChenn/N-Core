using System;

namespace NextFramework.Core
{
    /// <summary>
    /// 只能用作new() 创建的类型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DefaultObjectPool<T> : PoolBase<T> where T : new()
    {
        private Action<T> resetAction; 
        public DefaultObjectPool(Action<T> resetAction, int initCount = 0)
        {
            factory = new DefaultObjectFactory<T>();
            this.resetAction = resetAction;
            if (initCount > 0)
            {
                for (int i = 0; i < initCount; i++)
                {
                    Recycle(factory.Create());
                }
            }
        }

        public override bool Recycle(T obj)
        {
            if (obj == null) return false;
            
            resetAction?.Invoke(obj);
            dataStack.Push(obj);
            return true;
        }
    }
}
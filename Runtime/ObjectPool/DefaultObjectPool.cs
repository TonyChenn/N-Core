using System;

namespace NCore
{
    /// <summary>
    /// 只能用作new() 创建的类型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DefaultObjectPool<T> : PoolBase<T> where T : new()
    {
        private readonly Action<T> resetAction;


        public DefaultObjectPool(Action<T> resetAction, int initCount = 0) : base(initCount)
        {
            this.resetAction = resetAction;
        }

        public override T Create()
        {
            return new T();
        }

        public override bool Recycle(T obj)
        {
            if (obj == null) return false;

            resetAction?.Invoke(obj);
            dataStack.Push(obj);
            return true;
        }

        public override void Destory()
        {
            dataStack = null;
        }
    }
}

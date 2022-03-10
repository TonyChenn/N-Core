using System.Collections.Generic;

namespace NCore
{
    public abstract class PoolBase<T> : IPool<T>
    {
        protected Stack<T> dataStack = new Stack<T>();
        protected int maxCount = 10;
        protected IObjectFactory<T> factory;

        public int HasCount { get { return dataStack.Count; } }
        
        /// <summary>
        /// 获取
        /// </summary>
        public virtual T Alloc()
        {
            return HasCount == 0 ? factory.Create() : dataStack.Pop();
        }
        /// <summary>
        /// 回收
        /// </summary>
        public abstract bool Recycle(T obj);
    }
}



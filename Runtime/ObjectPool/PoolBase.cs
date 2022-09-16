using System.Collections.Generic;

namespace NCore
{
    public abstract class PoolBase<T> : IPool<T>
    {
        protected readonly Stack<T> dataStack = new Stack<T>();
        protected int maxCount = 10;

        public int HasCount { get { return dataStack.Count; } }

        public PoolBase(int initCount = 4)
        {
            dataStack = new Stack<T>(initCount);
        }

        /// <summary>
        /// 获取
        /// </summary>
        public virtual T Alloc()
        {
            return HasCount == 0 ? Create() : dataStack.Pop();
        }
        /// <summary>
        /// 回收
        /// </summary>
        public abstract bool Recycle(T obj);

        public abstract T Create();
    }
}



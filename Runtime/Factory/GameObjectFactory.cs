using System;

namespace NCore
{
    /// <summary>
    /// 对象是自定义生成的(如：GameObject)
    /// </summary>
    public class GameObjectFactory<T> : IObjectFactory<T>
    {
        protected readonly Func<T> m_createMethod;
        protected readonly Action<T> m_resetAction;

        public GameObjectFactory(Func<T> creatMethod, Action<T> resetAction)
        {
            m_createMethod = creatMethod;
            m_resetAction = resetAction;
        }
        public T Create()
        {
            return m_createMethod();
        }

        public void Reset(T item)
        {
            m_resetAction(item);
        }
    }
}


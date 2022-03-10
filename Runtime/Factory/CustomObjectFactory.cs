using System;

namespace SFramework.Core
{
    /// <summary>
    /// 对象是自定义生成的(如：GameObject)
    /// </summary>
    public class CustomObjectFactory<T> : IObjectFactory<T>
    {
        protected Func<T> mMethod;

        public CustomObjectFactory(Func<T> creatMethod)
        {
            this.mMethod = creatMethod;
        }
        public T Create()
        {
            return mMethod();
        }
    }
}


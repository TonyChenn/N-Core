namespace NCore
{
    /// <summary>
    /// 对象是通过New()生成的
    /// </summary>
    public class DefaultObjectFactory<T> : IObjectFactory<T> where T : new()
    {
        public DefaultObjectFactory()
        {
        }

        public T Create()
        {
            return new T();
        }

        public void Reset(T item)
        {
            throw new System.NotImplementedException();
        }
    }
}


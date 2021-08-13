namespace NextFramework.Core
{
    public interface IPool<T>
    {
        T Alloc();
        bool Recycle(T obj);
    }
}

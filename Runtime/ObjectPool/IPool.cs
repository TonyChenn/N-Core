namespace SFramework.Core
{
    public interface IPool<T>
    {
        T Alloc();
        bool Recycle(T obj);
    }
}

namespace NCore
{
    public interface IObjectFactory<T>
    {
        T Create();

        void Reset(T item);
    }
}

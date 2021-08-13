namespace NextFramework.Core
{
    public interface IObjectFactory<T>
    {
        T Create();
    }
}

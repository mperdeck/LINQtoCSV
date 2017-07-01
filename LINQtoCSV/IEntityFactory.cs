namespace LINQtoCSV
{
    public interface IEntityFactory
    {
        T AllocateObject<T>() where T : class, new();
        void ReleaseObject<T>(T entity) where T : class, new();
    }
}

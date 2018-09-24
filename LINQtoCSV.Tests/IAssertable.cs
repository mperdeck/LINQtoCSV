namespace LINQtoCSV.Tests
{
    public interface IAssertable<T>
    {
        void AssertEqual(T other);
    }
}

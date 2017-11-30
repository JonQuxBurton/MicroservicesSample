namespace Infrastructure.Rest
{
    public interface IRestGetterFactory
    {
        IRestGetter Create(string url);
    }
}
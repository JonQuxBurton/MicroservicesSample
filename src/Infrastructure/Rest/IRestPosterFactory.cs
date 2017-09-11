namespace Infrastructure.Rest
{
    public interface IRestPosterFactory
    {
        IRestPoster Create(string url);
    }
}

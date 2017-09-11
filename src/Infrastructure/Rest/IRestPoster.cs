using RestSharp;

namespace Infrastructure.Rest
{
    public interface IRestPoster
    {
        IRestResponse Post(string resource, object body);
    }
}
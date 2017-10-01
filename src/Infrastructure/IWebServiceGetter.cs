using RestSharp;

namespace Infrastructure.Rest
{
    public interface IWebServiceGetter
    {
        IRestResponse Get(string resource);
    }
}
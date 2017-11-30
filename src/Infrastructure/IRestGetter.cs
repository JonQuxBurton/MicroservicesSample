using RestSharp;

namespace Infrastructure.Rest
{
    public interface IRestGetter
    {
        IRestResponse Get(IRestRequest restRequest);
    }
}
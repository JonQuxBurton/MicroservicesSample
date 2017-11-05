using System.Threading.Tasks;
using RestSharp;

namespace Infrastructure.Rest
{
    public interface IRestGetter
    {
        Task<IRestResponse> Get(IRestRequest restRequest);
    }
}
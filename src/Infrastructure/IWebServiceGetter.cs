using RestSharp;
using System.Threading.Tasks;

namespace Infrastructure.Rest
{
    public interface IWebServiceGetter
    {
        Task<IRestResponse> Get(string resource);
    }
}
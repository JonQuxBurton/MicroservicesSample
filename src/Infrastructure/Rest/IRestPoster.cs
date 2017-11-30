using RestSharp;
using System.Threading.Tasks;

namespace Infrastructure.Rest
{
    public interface IRestPoster
    {
        Task<IRestResponse> Post(string resource, object body);
    }
}
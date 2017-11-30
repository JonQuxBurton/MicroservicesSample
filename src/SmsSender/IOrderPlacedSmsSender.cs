using System.Threading.Tasks;

namespace SmsSender
{
    public interface IOrderPlacedSmsSender
    {
        Task<bool> Send(int phoneLineId);
    }
}
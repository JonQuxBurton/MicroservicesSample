using System.Threading.Tasks;

namespace SmsSender
{
    public interface IOrderCompletedSmsSender
    {
        Task<bool> Send(int phoneLineId);
    }
}
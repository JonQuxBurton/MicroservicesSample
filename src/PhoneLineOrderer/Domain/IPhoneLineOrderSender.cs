using System.Threading.Tasks;

namespace PhoneLineOrderer.Domain
{
    public interface IPhoneLineOrderSender
    {
        Task<bool> Send(Resources.PhoneLineOrder phoneLineOrder);
    }
}

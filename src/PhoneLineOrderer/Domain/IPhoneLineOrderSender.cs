using Infrastructure.Rest;

namespace PhoneLineOrderer.Domain
{
    public interface IPhoneLineOrderSender
    {
        bool Send(Resources.PhoneLineOrder phoneLineOrder);
    }
}

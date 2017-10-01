namespace SmsSender
{
    public interface IOrderPlacedSmsSender
    {
        bool Send(int phoneLineId);
    }
}
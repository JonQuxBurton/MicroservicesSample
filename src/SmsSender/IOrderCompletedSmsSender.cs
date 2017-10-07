namespace SmsSender
{
    public interface IOrderCompletedSmsSender
    {
        bool Send(int phoneLineId);
    }
}
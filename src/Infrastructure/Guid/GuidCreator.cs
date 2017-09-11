namespace Infrastructure.Guid
{
    public class GuidCreator : IGuidCreator
    {
        public System.Guid Create()
        {
            return System.Guid.NewGuid();
        }
    }
}

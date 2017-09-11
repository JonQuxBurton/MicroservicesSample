namespace Infrastructure.Serialization
{
    public interface ISerializer
    {
        string Serialize(object value);
    }
}

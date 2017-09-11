namespace Infrastructure.Serialization
{
    public interface IDeserializer
    {
        T DeserializeBytes<T>(byte[] serialized);
    }
}

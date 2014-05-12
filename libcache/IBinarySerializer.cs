namespace Kontur.Cache
{
    using System.IO;

    public interface IBinarySerializer<T>
    {
        void Serialize(Stream stream, T value);

        T Deserialize(Stream stream);
    }
}
namespace Kontur.Cache
{
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;

    public class DefaultBinarySerializer<T> : IBinarySerializer<T>
    {
        public void Serialize(Stream stream, T value)
        {
            var formatter = new BinaryFormatter();
            formatter.Serialize(stream, value);
        }

        public T Deserialize(Stream stream)
        {
            var formatter = new BinaryFormatter();
            return (T)formatter.Deserialize(stream);
        }
    }
}
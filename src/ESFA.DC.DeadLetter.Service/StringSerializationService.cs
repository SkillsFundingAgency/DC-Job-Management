using System.IO;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.DeadLetter.Service
{
    public class StringSerializationService : ISerializationService
    {
        public T Deserialize<T>(string serializedObject)
        {
            if (typeof(T) == typeof(string))
            {
                T result = (T)System.Convert.ChangeType(serializedObject, typeof(T));
                return result;
            }

            return default(T);
        }

        public T Deserialize<T>(Stream stream)
        {
            throw new System.NotImplementedException();
        }

        public string Serialize<T>(T objectToSerialize)
        {
            throw new System.NotImplementedException();
        }

        public void Serialize<T>(T objectToSerialize, Stream stream)
        {
            throw new System.NotImplementedException();
        }
    }
}
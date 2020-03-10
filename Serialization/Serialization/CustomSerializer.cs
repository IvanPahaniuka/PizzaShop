using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pizza.Serialization
{
    public class CustomSerializer : ISerializer
    {
        public T Deserialize<T>(Stream stream) where T : class
        {
            var serializer = new Pizza.CustomSerializer.CustomSerializer();
            return serializer.Deserialize<T>(stream);
        }

        public void Serialize<T>(Stream stream, T obj) where T : class
        {
            var serializer = new Pizza.CustomSerializer.CustomSerializer();
            serializer.Serialize(stream, obj);
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Pizza.Serialization
{
    public class BinarySerializer: ISerializer
    {
        public T Deserialize<T>(Stream stream) where T:class
        {
            var serializer = new BinaryFormatter();
            return serializer.Deserialize(stream) as T;
        }

        public void Serialize<T>(Stream stream, T obj) where T:class
        {
            var serializer = new BinaryFormatter();
            serializer.Serialize(stream, obj);
        }
    }
}

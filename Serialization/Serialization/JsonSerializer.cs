using Newtonsoft.Json;
using Pizza.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pizza.Serialization
{
    public class JsonSerializer: ISerializer
    {
        public T Deserialize<T>(Stream stream) where T:class
        {
            T obj;

            using (var SR = new StreamReader(stream))
            using (var JR = new JsonTextReader(SR))
            {
                var serializer = new Newtonsoft.Json.JsonSerializer();
                serializer.TypeNameHandling = TypeNameHandling.All;
                obj = serializer.Deserialize<T>(JR);
            }

            return obj;
        }

        public void Serialize<T>(Stream stream, T obj) where T:class
        {
            using (var SW = new StreamWriter(stream))
            using (var JW = new JsonTextWriter(SW))
            {
                var serializer = new Newtonsoft.Json.JsonSerializer();
                serializer.TypeNameHandling = TypeNameHandling.All;
                serializer.Serialize(JW, obj, typeof(T));
            }
        }
    }
}

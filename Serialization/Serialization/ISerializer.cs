using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pizza.Serialization
{
    public interface ISerializer
    {
        void Serialize<T>(Stream stream, T obj) where T:class;
        T Deserialize<T>(Stream stream) where T:class;
    }
}

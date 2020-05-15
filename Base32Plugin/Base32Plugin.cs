using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pizza.Plugin
{
    public class Base32Plugin : Plugin, IDataPlugin
    {
        public byte[] Demodify(byte[] data)
        {
            var str = Encoding.ASCII.GetString(data);
            return Base32Encoding.ToBytes(str);
        }

        public byte[] Modify(byte[] data)
        {
            var str = Base32Encoding.ToString(data);
            return Encoding.ASCII.GetBytes(str);
        }
    }
}

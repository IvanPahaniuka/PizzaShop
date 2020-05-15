using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pizza.Plugin
{
    public class Base64Plugin : Plugin, IDataPlugin
    {
        public byte[] Demodify(byte[] data)
        {
            var str = Encoding.ASCII.GetString(data);
            return Convert.FromBase64String(str);
        }

        public byte[] Modify(byte[] data)
        {
            var str = Convert.ToBase64String(data);
            return Encoding.ASCII.GetBytes(str);
        }
    }
}

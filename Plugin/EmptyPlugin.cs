using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pizza.Plugin
{
    public class EmptyPlugin : Plugin, IDataPlugin
    {
        public byte[] Demodify(byte[] data)
        {
            return data;
        }

        public byte[] Modify(byte[] data)
        {
            return data;
        }
    }
}

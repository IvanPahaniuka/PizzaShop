using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pizza.Plugin
{
    public interface IDataPlugin
    {
        byte[] Modify(byte[] data);
        byte[] Demodify(byte[] data);
    }
}

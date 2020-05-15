using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pizza.UI
{
    public class ValueDescription
    {
        public virtual object Value { get; set; }
        public virtual string Description { get; set; }
    }

    public class ValueDescription<T> : ValueDescription
    {
        public virtual T ValueTyped { get; set; }
        public override object Value
        {
            get => ValueTyped;
            set => ValueTyped = (T)value;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pizza.Models.Builders
{
    public class PizzaBuilder<T> where T: Pizza
    {
        public T Pizza { get; protected set; }
            
        public PizzaBuilder(T pizza)
        {
            Pizza = pizza ?? throw new ArgumentNullException();
        }

        public virtual T Build()
        {
            return (T)Pizza.Clone();
        }
    }
}
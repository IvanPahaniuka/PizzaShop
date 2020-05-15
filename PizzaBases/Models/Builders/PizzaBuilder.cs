using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pizza.Models.Builders
{
    public class PizzaBuilder
    {
        public Pizza Pizza { get; protected set; }
            
        public PizzaBuilder(Pizza pizza)
        {
            Pizza = pizza ?? throw new ArgumentNullException();
        }

        public virtual Pizza Build()
        {
            return Pizza.Clone();
        }
    }

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

        public static implicit operator PizzaBuilder(PizzaBuilder<T> builder)
        {
            return new PizzaBuilder(builder.Pizza);
        }
    }
}
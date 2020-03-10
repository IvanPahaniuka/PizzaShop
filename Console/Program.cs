using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pizza.Models;
using Pizza.Models.Builders;
using Pizza.CustomSerializer;

namespace Pizza
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new PizzaBuilder<CheesePizza>(new CheesePizza{ 
                Box = new Box(new Size(10), Box.BoxColor.Red),
                Name = "Cheese",
                CheeseMass = 1.1f,
                Cost = 10.1M});

            var pizza = builder.Build();
            pizza.CheeseMass = 1.3f;
            pizza.Box.Size = new Size(11);
            pizza.Cost = 10.5M;

            Console.WriteLine($"Big cheese pizza: {pizza}");
            Console.WriteLine($"Medium cheese pizza: {builder.Build()}");

            var dict = new Dictionary<Models.Pizza, int>();
            dict.Add(new Models.Pizza(), 123);
            dict.Add(new Models.CheesePizza(), 1223);

            var serializer =  new CustomSerializer.CustomSerializer();

            var str = serializer.Serialize(dict);
            Console.WriteLine($"Big cheese pizza: {str}");
            dict = serializer.Deserialize<Dictionary<Models.Pizza, int>>(str);

        
            Console.ReadLine();
        }
    }
}

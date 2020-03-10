using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Pizza.Models
{
    [Serializable]
    public class ItalianPizza: PepperoniPizza
    {
        private float vegetablesMass;
        
        [Description("Вес овощей")]
        public float VegetablesMass
        {
            get => vegetablesMass;
            set
            {
                vegetablesMass = value;
                OnPropertyChanged();
            }
        }
    }
}
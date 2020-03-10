using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Pizza.Models
{
    [Serializable]
    public class PepperoniPizza: CheesePizza
    {
        private float sausageMass;

        [Description("Вес сосисок")]
        public float SausageMass
        {
            get => sausageMass;
            set
            {
                sausageMass = value;
                OnPropertyChanged();
            }
        }
    }
}
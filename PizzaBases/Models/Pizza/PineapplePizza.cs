using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Pizza.Models
{
    [Serializable]
    public class PineapplePizza: CheesePizza
    {
        private float pinappleMass;

        [Description("Вес ананаса")]
        public float PineappleMass
        {
            get => pinappleMass;
            set
            {
                pinappleMass = value;
                OnPropertyChanged();
            }
        }
    }
}
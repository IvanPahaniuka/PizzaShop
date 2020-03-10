using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Pizza.Models
{
    [Serializable]
    public class CheesePizza: Pizza
    {
        private float cheeseMass;

        [Description("Вес сыра")]
        public float CheeseMass
        {
            get => cheeseMass;
            set
            {
                cheeseMass = value;
                OnPropertyChanged();
            }
        }
    }
}
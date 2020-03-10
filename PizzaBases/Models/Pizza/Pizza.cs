using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Pizza.Models
{
    [Serializable]
    public class Pizza : INotifyPropertyChanged
    {

        private string name;
        private Box box;
        private decimal cost;

        [Description("Название")]
        public string Name
        {
            get => name;
            set
            {
                name = value;
                OnPropertyChanged();
            }
        }
        [Description("Коробка")]
        public Box Box
        {
            get => box;
            set
            {
                box = value;
                OnPropertyChanged();
            }
        }
        [Description("Цена")]
        public decimal Cost
        {
            get => cost;
            set
            {
                cost = value;
                OnPropertyChanged();
            }
        }

        [field:NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public virtual Pizza Clone()
        {
            Pizza clone = (Pizza)MemberwiseClone();
            clone.Box = clone.Box.Clone();
            return clone;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            var type = GetType();
            foreach (var prop in type.GetProperties())
            {
                builder.Append($"{prop.Name}: {prop.GetValue(this)}; ");
            }


            return builder.ToString();
        }
    }
}
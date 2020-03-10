using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Pizza.Models
{
    [Serializable]
    public class Box: INotifyPropertyChanged
    {
        public enum BoxColor {
            [Description("Красный")]
            Red,
            [Description("Белый")]
            White,
            [Description("Желтый")]
            Yellow,
            [Description("Зеленый")]
            Green,
            [Description("Розовый")]
            Pink
        }

        private Size size;
        private BoxColor color;

        [Description("Размеры")]
        public Size Size 
        {
            get => size;
            set
            {
                size = value;
                OnPropertyChanged();
            }
        }
        [Description("Цвет")]
        public BoxColor Color
        {
            get => color;
            set 
            {
                color = value;
                OnPropertyChanged();
            }
        }

        [field:NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public Box()
            :this(new Size())
        {
        }

        public Box(Size size)
            :this(size, BoxColor.White)
        {

        }

        public Box(Size size, BoxColor color)
        {
            Size = size;
            Color = color;
        }

        public Box Clone()
        {
            return new Box(new Size(size.X, size.Y), Color);
        }
    }
}
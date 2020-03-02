using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Pizza.Models
{
    public class Size: INotifyPropertyChanged
    {
        private float x, y;

        public float X 
        {
            get => x;
            set
            {
                x = value;
                OnPropertyChanged();
            }
        }

        public float Y
        {
            get => y;
            set
            {
                y = value;
                OnPropertyChanged();
            }
        }

        public Size()
            :this(0)
        {
        }

        public Size(float a)
            : this(a, a)
        {

        }

        public Size(float x, float y)
        {
            X = x;
            Y = y;
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}

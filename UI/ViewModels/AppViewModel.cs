using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Pizza.Models;

namespace Pizza.UI.ViewModels
{
    public class AppViewModel: INotifyPropertyChanged
    {
        private object selectedItem;

        public object SelectedItem
        {
            get => selectedItem;
            set
            {
                selectedItem = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<object> Items { get; set; }


        public AppViewModel()
        {
            Items = new ObservableCollection<object>();
        }

        public void AddItem(Models.Builders.PizzaBuilder<Models.Pizza> builder)
        {
            Items.Add(builder.Build());
        }

        public void RemoveItem(object item)
        {
            Items.Remove(item);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}

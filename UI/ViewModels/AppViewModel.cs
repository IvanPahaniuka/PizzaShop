using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Pizza.Models;
using Pizza.Serialization;

namespace Pizza.UI.ViewModels
{
    [Serializable]
    public class AppViewModel: INotifyPropertyChanged
    {
        public class FileDescription
        {
            public string Filter { get; }
            public ISerializer Serializator { get; }

            public FileDescription(string filter, ISerializer serializator)
            {
                Filter = filter ?? throw new ArgumentNullException();
                Serializator = serializator ?? throw new ArgumentNullException();
            }
        }

        private object selectedItem;
        private ObservableCollection<object> items;
        private List<FileDescription> filters;
        private string lastFile;
        private FileDescription lastFilter;

        public object SelectedItem
        {
            get => selectedItem;
            set
            {
                selectedItem = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<object> Items
        {
            get => items;
            set
            {
                items = value;
                OnPropertyChanged();
            }
        }
        public List<FileDescription> Filters
        {
            get => filters;
            set
            {
                filters = value;
                OnPropertyChanged();
            }
        }
        public string LastFile
        {
            get => lastFile;
            set
            {
                lastFile = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Title));
            }
        }
        public string Title{ 
            get => (LastFile != null ? Path.GetFileName(LastFile) + " - " : "") + 
                    "Главная";
        }
        public FileDescription LastFilter
        {
            get => lastFilter;
            set
            {
                lastFilter = value;
                OnPropertyChanged();
            }
        }
        

        public AppViewModel()
        {
            Items = new ObservableCollection<object>();

            Filters = new List<FileDescription>{
                new FileDescription("Бинарный файл (*.bin)|*.bin", new BinarySerializer()),
                new FileDescription("JSON файл (*.json)|*.json", new JsonSerializer()),
                new FileDescription("Текстовый формат (*.txt)|*.txt", new CustomSerializer())
                };
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

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
using Pizza.Plugin;
using Pizza.Serialization;
using Pizza.Models.Builders;

namespace Pizza.UI.ViewModels
{
    [Serializable]
    public class AppViewModel: INotifyPropertyChanged
    {
        public class FileDescription
        {
            public string Filter => $"{Name} (*.{FileExtension})|*.{FileExtension}";
            public string Name { get; }
            public string FileExtension { get; }
            public ISerializer Serializer { get; }

            public FileDescription(string name, string fileExtension, ISerializer serializer)
            {
                Name = name ?? throw new ArgumentNullException();
                FileExtension = fileExtension ?? throw new ArgumentNullException();
                Serializer = serializer ?? throw new ArgumentNullException();
            }
        }

        private object selectedItem;
        private ObservableCollection<object> items;
        private List<FileDescription> serializers;
        private string lastFile;
        private ISerializer lastSerializer;

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
        public List<FileDescription> Serializers
        {
            get => serializers;
            set
            {
                serializers = value;
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
        public ISerializer LastSerializer
        {
            get => lastSerializer;
            set
            {
                lastSerializer = value;
                OnPropertyChanged();
            }
        }


        public AppViewModel()
        {
            Items = new ObservableCollection<object>();
            
            var serializers = new List<FileDescription>{
                new FileDescription("Бинарный формат", "bin", new BinarySerializer()),
                new FileDescription("JSON формат", "json", new JsonSerializer()),
                new FileDescription("Текстовый формат", "txt", new CustomSerializer())};

            var pluginsSerializer = GetPluginsSerializer(serializers.Select(s => s.Serializer).ToArray());
            serializers.Add(new FileDescription("Модифицированный формат", "bmd", pluginsSerializer));

            Serializers = serializers;
        }

        public void AddItem(PizzaBuilder builder)
        {
            Items.Add(builder.Build());
        }
        public void RemoveItem(object item)
        {
            Items.Remove(item);
        }

        private PluginSerializer GetPluginsSerializer(IEnumerable<ISerializer> serializers)
        {
            var path = Path.Combine(Environment.CurrentDirectory, "Plugins");
            var plugins = new List<IDataPlugin>(PluginsLoader.LoadPlugins(path).OfType<IDataPlugin>());
            plugins.Insert(0, new EmptyPlugin());

            return new PluginSerializer(serializers, plugins);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}

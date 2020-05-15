using Pizza.Plugin;
using Pizza.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Pizza.UI.ViewModels 
{
    public class FilePluginViewModel: INotifyPropertyChanged
    {
        private IEnumerable<ValueDescription<IDataPlugin>> plugins;
        private IDataPlugin selectedPlugin;
        private IEnumerable<ValueDescription<ISerializer>> serialziers;
        private ISerializer selectedSerializer;

        public IEnumerable<ValueDescription<IDataPlugin>> Plugins
        {
            get => plugins;
            private set
            {
                plugins = value;
                OnPropertyChanged();
            }
        }
        public IDataPlugin SelectedPlugin
        {
            get => selectedPlugin;
            set
            {
                selectedPlugin = value;
                OnPropertyChanged();
            }
        }
        public IEnumerable<ValueDescription<ISerializer>> Serializers
        {
            get => serialziers;
            private set
            {
                serialziers = value;
                OnPropertyChanged();
            }
        }
        public ISerializer SelectedSerializer
        {
            get => selectedSerializer;
            set
            {
                selectedSerializer = value;
                OnPropertyChanged();
            }
        }

        public FilePluginViewModel(
            IEnumerable<ValueDescription<ISerializer>> serializers, 
            IEnumerable<ValueDescription<IDataPlugin>> plugins)
        {
            serializers = serializers.Where(s => !(s.ValueTyped is PluginSerializer)).ToArray();

            Serializers = serializers;
            Plugins = plugins;

            SelectedSerializer = Serializers.ElementAtOrDefault(0)?.ValueTyped;
            SelectedPlugin = Plugins.ElementAtOrDefault(0)?.ValueTyped;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}

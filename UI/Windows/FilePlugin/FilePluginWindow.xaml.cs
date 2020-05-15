using Pizza.Plugin;
using Pizza.Serialization;
using Pizza.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Pizza.UI
{
    /// <summary>
    /// Логика взаимодействия для FilePluginWindow.xaml
    /// </summary>
    public partial class FilePluginWindow : Window
    {
        private readonly FilePluginViewModel model;

        public ISerializer SelectedSerializer => model.SelectedSerializer;
        public IDataPlugin SelectedPlugin => model.SelectedPlugin;

        public FilePluginWindow(
            IEnumerable<ValueDescription<ISerializer>> serializers, 
            IEnumerable<ValueDescription<IDataPlugin>> plugins)
        {
            InitializeComponent();

            model = new FilePluginViewModel(serializers, plugins);
            DataContext = model;
        }
    }
}

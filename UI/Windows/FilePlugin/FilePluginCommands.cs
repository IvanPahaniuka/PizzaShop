using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Pizza.UI
{
    public partial class FilePluginWindow: Window
    {
        private void CommandBinding_Save(object sender, ExecutedRoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}

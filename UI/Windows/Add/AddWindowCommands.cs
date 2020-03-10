using System.Windows;
using System.Windows.Input;

namespace Pizza.UI
{
    /// <summary>
    /// Логика взаимодействия для AddWindow.xaml
    /// </summary>
    public partial class AddWindow : Window
    {

        private void CommandBinding_New(object sender, ExecutedRoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}

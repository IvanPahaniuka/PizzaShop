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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Pizza.UI
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly AppViewModel model;

        public MainWindow()
        {
            InitializeComponent();

            model = new AppViewModel();
            DataContext = model;
        }

        private void CommandBinding_Add(object sender, ExecutedRoutedEventArgs e)
        {
            var addWindow = new AddWindow();
            if (addWindow.ShowDialog() == true)
            {
                model.AddItem(addWindow.SelectedBuilder);
            }
        }

        private void CommandBinding_Delete(object sender, ExecutedRoutedEventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите удалить выбранный элемент?", "Удаление", 
                MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                model.RemoveItem(model.SelectedItem);
        }

        private void CommandBinding_Delete_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = model.SelectedItem != null;
        }
    }
}

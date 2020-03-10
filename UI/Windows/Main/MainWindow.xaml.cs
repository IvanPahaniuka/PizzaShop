using Pizza.UI.ViewModels;
using System.Windows;
using System.Windows.Input;

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
    }
}

using Pizza.Models.Builders;
using Pizza.UI.ViewModels;
using System.Windows;

namespace Pizza.UI
{

    /// <summary>
    /// Логика взаимодействия для AddWindow.xaml
    /// </summary>
    public partial class AddWindow : Window
    {
        private readonly AddViewModel model;

        public PizzaBuilder<Models.Pizza> SelectedBuilder => model.SelectedBuilder;

        public AddWindow()
        {
            InitializeComponent();
            
            model = new AddViewModel();
            DataContext = model;
        }
    }
}

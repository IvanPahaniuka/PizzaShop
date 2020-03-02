using Pizza.Models.Builders;
using Pizza.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Pizza.UI
{
    public class ValueDescription
    {
        public PizzaBuilder<Models.Pizza> Value { get; set; }
        public string Description { get; set; }
    }

    [ValueConversion(typeof(IEnumerable<PizzaBuilder<Models.Pizza>>), typeof(IEnumerable<ValueDescription>))]
    public class BuildersToDescriptionConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !(value is IEnumerable<PizzaBuilder<Models.Pizza>>))
                return new ValueDescription[]{ };

            var valueList = value as IEnumerable<PizzaBuilder<Models.Pizza>>;
            var valueDescr = new List<ValueDescription>(valueList.Count());

            foreach (var item in valueList)
                valueDescr.Add(new ValueDescription{ Value = item, Description = item.Pizza.Name });

            return valueDescr;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }

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

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}

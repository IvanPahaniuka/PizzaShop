using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Pizza.Controls
{
    public class ValueDescription
    {
        public Enum Value { get; set; }
        public string Description { get; set; }
    }

    public static class EnumHelper
    {
        public static string Description(this Enum value)
        {
            var attributes = value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes.Any())
                return (attributes.First() as DescriptionAttribute).Description;

            TextInfo ti = CultureInfo.CurrentCulture.TextInfo;
            return ti.ToTitleCase(ti.ToLower(value.ToString().Replace("_", " ")));
        }

        public static IEnumerable<ValueDescription> GetAllValuesAndDescriptions(Type t)
        {
            if (!t.IsEnum)
                throw new ArgumentException($"{nameof(t)} must be an enum type");

            return Enum.GetValues(t).Cast<Enum>().Select((e) => new ValueDescription() { Value = e, Description = e.Description() }).ToList();
        }
    }

    [ValueConversion(typeof(Enum), typeof(IEnumerable<ValueDescription>))]
    public class EnumToListConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !(value is Type))
                return new ValueDescription[]{ };

            return EnumHelper.GetAllValuesAndDescriptions((Type)value);
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
    /// Логика взаимодействия для EnumBox.xaml
    /// </summary>
    public partial class EnumBox : UserControl
    {
        public static DependencyProperty TypeProperty;
        public static readonly RoutedEvent TypeChangedEvent;

        public static DependencyProperty SelectedValueProperty;
        public static readonly RoutedEvent SelectedValueChangedEvent;

        public Type Type
        {
            get => (Type)GetValue(TypeProperty);
            set => SetValue(TypeProperty, value);
        }
        public Enum SelectedValue
        {
            get => (Enum)GetValue(SelectedValueProperty);
            set => SetValue(SelectedValueProperty, value);
        }

        public event RoutedPropertyChangedEventHandler<Type> TypeChanged
        {
            add => AddHandler(TypeChangedEvent, value);
            remove => RemoveHandler(TypeChangedEvent, value);
        }
        public event RoutedPropertyChangedEventHandler<Enum> SelectedValueChanged
        {
            add => AddHandler(SelectedValueChangedEvent, value);
            remove => RemoveHandler(SelectedValueChangedEvent, value);
        }

        static EnumBox()
        {
            TypeProperty = DependencyProperty.Register("Type", typeof(Type), typeof(EnumBox),
                new FrameworkPropertyMetadata(defaultValue: null));
            TypeChangedEvent = EventManager.RegisterRoutedEvent("TypeChanged", RoutingStrategy.Bubble,
                typeof(RoutedPropertyChangedEventHandler<Type>), typeof(EnumBox));

            SelectedValueProperty = DependencyProperty.Register("SelectedValue", typeof(Enum), typeof(EnumBox),
                new FrameworkPropertyMetadata(defaultValue: null));
            SelectedValueChangedEvent = EventManager.RegisterRoutedEvent("SelectedValueChanged", RoutingStrategy.Bubble,
                typeof(RoutedPropertyChangedEventHandler<Enum>), typeof(EnumBox));
        }

        public EnumBox()
        {
            InitializeComponent();
        }
    }
}

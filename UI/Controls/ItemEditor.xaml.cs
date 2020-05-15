using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
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

namespace Pizza.Controls
{
    public static class PropertyExtension
    {
        public static string GetDescription(this PropertyInfo property)
        {
            var attributes = property.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes.Any())
                return (attributes.First() as DescriptionAttribute).Description;

            TextInfo ti = CultureInfo.CurrentCulture.TextInfo;
            return ti.ToTitleCase(ti.ToLower(property.Name.Replace("_", " ")));
        }
    }

    /// <summary>
    /// Логика взаимодействия для ItemEditor.xaml
    /// </summary>
    public partial class ItemEditor : UserControl
    {
        public static DependencyProperty ItemProperty;
        public static readonly RoutedEvent ItemChangedEvent;

        public object Item
        {
            get => GetValue(ItemProperty);
            set => SetValue(ItemProperty, value);
        }

        public event RoutedPropertyChangedEventHandler<object> ItemChanged
        {
            add => AddHandler(ItemChangedEvent, value);
            remove => RemoveHandler(ItemChangedEvent, value);
        }

        static ItemEditor()
        {
            ItemProperty = DependencyProperty.Register("Item", typeof(object), typeof(ItemEditor),
                new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnItemChanged)));
            ItemChangedEvent = EventManager.RegisterRoutedEvent("ItemChanged", RoutingStrategy.Bubble,
                typeof(RoutedPropertyChangedEventHandler<object>), typeof(ItemEditor));
        }

        public ItemEditor()
        {
            InitializeComponent();
        }

        private static void OnItemChanged(DependencyObject sender,
            DependencyPropertyChangedEventArgs e)
        {
            var newItem = e.NewValue;
            var itemEditor = (ItemEditor)sender;

            ChangeItem(itemEditor, newItem);
        }

        private static void ChangeItem(ItemEditor editor, object newItem)
        {
            var stack = editor.stack;
            stack.Children.Clear();

            var properties = newItem.GetType().GetProperties();
            foreach (var property in properties)
            {
                AddPropertyEditor(stack, property);
            }
        }

        private static void AddPropertyEditor(StackPanel stack, PropertyInfo property, string path = "")
        {
            var propType = property.PropertyType;
            var name = property.GetDescription();

            if (propType == typeof(string) ||
                propType == typeof(decimal) ||
                propType == typeof(int) ||
                propType == typeof(float))
            {
                stack.Children.Add(new TextBlock { Text = name, Margin = new Thickness(5,5,5,0) });

                var textBox = new TextBox{ Margin = new Thickness(5) };
                var binding = new Binding
                {
                    Path = new PropertyPath(path + property.Name),
                    Mode = BindingMode.TwoWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                };
                textBox.SetBinding(TextBox.TextProperty, binding);

                stack.Children.Add(textBox);
                return;
            }

            if (propType.IsEnum)
            {
                stack.Children.Add(new TextBlock { Text = name, Margin = new Thickness(5,5,5,0) });

                var enumBox = new EnumBox{ Type = propType, Margin = new Thickness(5) };

                var binding = new Binding
                {
                    Path = new PropertyPath(path + property.Name),
                    Mode = BindingMode.TwoWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                };
                enumBox.SetBinding(EnumBox.SelectedValueProperty, binding);

                stack.Children.Add(enumBox);
                return;
            }

            if (propType.IsClass)
            {
                var expStack = new StackPanel();
                var expander = new Expander { Header = name, BorderThickness = new Thickness(1), BorderBrush = Brushes.LightGray, 
                    Margin = new Thickness(5), Padding = new Thickness(3) };
                expander.Content = expStack;

                stack.Children.Add(expander);


                var properties = propType.GetProperties();
                foreach (var _property in properties)
                {
                    AddPropertyEditor(expStack, _property, $"{path}{property.Name}.");
                }

                return;
            }
        }
    }
}

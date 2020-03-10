using Pizza.Models.Builders;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Markup;

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
}

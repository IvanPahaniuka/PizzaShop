using Pizza.Models.Builders;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Markup;

namespace Pizza.UI
{
    [ValueConversion(typeof(IEnumerable<PizzaBuilder>), typeof(IEnumerable<ValueDescription>))]
    public class BuildersToDescriptionConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !(value is IEnumerable<PizzaBuilder>))
                return new ValueDescription[]{ };

            var valueList = value as IEnumerable<PizzaBuilder>;
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

using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace School_Manager
{
    public class MultiValueConverter : MarkupExtension , IMultiValueConverter
    {
        private static MultiValueConverter mConverter = null;

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return mConverter ?? (mConverter = new MultiValueConverter());
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values.Clone();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Globalization;

namespace School_Manager
{
    public class InchToPixelConverter : BaseValueConverter<InchToPixelConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double i = 0;
            double.TryParse(value as string ?? "",out i);
            return i * 96;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

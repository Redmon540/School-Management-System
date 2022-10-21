using System;
using System.Globalization;

namespace School_Manager
{
    public class FeeOptionsVisibilityConverter : BaseValueConverter<FeeOptionsVisibilityConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((string)value == "Current Status")
                return false;
            else
                return true;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

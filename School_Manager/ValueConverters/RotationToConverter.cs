using System;
using System.Globalization;


namespace School_Manager
{
    public class RotationToConverter : BaseValueConverter<RotationToConverter>
    {
        
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)value == 0 ? 0 : 90;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

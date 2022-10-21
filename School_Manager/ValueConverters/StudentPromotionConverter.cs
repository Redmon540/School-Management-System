using System;
using System.Globalization;
using System.Windows.Data;

namespace School_Manager
{
    [ValueConversion(typeof(bool), typeof(string))]
    public class StudentPromotionConverter : BaseValueConverter<StudentPromotionConverter>
    {
        
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                if (value.GetType() == typeof(bool))
                {
                    if ((bool)value) return "PROMOTE";
                    return "FAIL";
                }
            }
            return null;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((string)value == "PROMOTE")
                return true;
            return false;
        }
    }
}

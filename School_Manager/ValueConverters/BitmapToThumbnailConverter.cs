using System;
using System.Globalization;

namespace School_Manager
{
    public class BitmapToThumbnailConverter : BaseValueConverter<BitmapToThumbnailConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Helper.CreateThumbnail(value as byte[]);
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

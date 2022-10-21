using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;

namespace School_Manager
{
    public class EmptyRecordMessageConverter : BaseValueConverter<EmptyRecordMessageConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Visibility.Collapsed;
            ObservableCollection<MonthlyFeeRecord> record = value as ObservableCollection<MonthlyFeeRecord>;
            return record.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

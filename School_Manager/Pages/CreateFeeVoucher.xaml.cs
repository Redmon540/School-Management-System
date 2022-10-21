using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace School_Manager
{
    /// <summary>
    /// Interaction logic for CreateFeeVoucher.xaml
    /// </summary>
    public partial class CreateFeeVoucher : BasePage<CreateFeeVoucherViewModel>
    {

        public CreateFeeVoucher()
        {
            InitializeComponent();
        }

        private Point _startPoint;

        private void ListBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // The initial mouse position
            _startPoint = e.GetPosition(null);
        }

        private void ListBox_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            // Get the current mouse position
            Point mousePos = e.GetPosition(null);
            Vector diff = _startPoint - mousePos;

            if (e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                // Get the dragged ListBoxItem
                var listBox = sender as ListBox;
                var item = listBox.SelectedItem as string;
                if (item.IsNullOrEmpty())
                    return;

                // Initialize the drag & drop operation
                if (item.ToLower().Contains("photo"))
                    DragDrop.DoDragDrop(listBox, new ImageControl { Value = item.ToString() }, DragDropEffects.Copy);
                else if (item.ToLower().Contains("qr code"))
                    DragDrop.DoDragDrop(listBox, new QRCodeControl { Value = item.ToString() }, DragDropEffects.Copy);
                else
                    DragDrop.DoDragDrop(listBox, new TextControl { Value = item.ToString() }, DragDropEffects.Copy);
            }
        }

        private void Fee_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // The initial mouse position
            _startPoint = e.GetPosition(null);
        }

        private void Fee_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            // Get the current mouse position
            Point mousePos = e.GetPosition(null);
            Vector diff = _startPoint - mousePos;

            if (e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                // Get the dragged ListBoxItem
                var textBlock = sender as TextBlock;
                var item = textBlock.Text as string;
                if (item.IsNullOrEmpty())
                    return;

                var fee = new FeeControl
                {
                    Value = item.ToString(),
                    Fee = (textBlock.DataContext as FeeEntity).Fee,
                    Month = (textBlock.DataContext as FeeEntity).Month,
                    Year = (textBlock.DataContext as FeeEntity).Year,
                    Background = (textBlock.DataContext as FeeEntity).Background
                };
                if (item == "Discount")
                    fee.FeeAttribite = FeeAttribite.Discount;
                else if (item == "Late Fee")
                    fee.FeeAttribite = FeeAttribite.LateFee;
                else if (item == "Due Date")
                    fee.FeeAttribite = FeeAttribite.DueDate;
                else if (item == "Total")
                    fee.FeeAttribite = FeeAttribite.Total;
                else if (item != "TOTAL FEE" && item != "TOTAL DISCOUNT"
                            && item != "GRAND TOTAL" && item != "TOTAL LATE FEE"
                            && item != "DUES" && item != "GRAND SUM")
                    fee.FeeAttribite = FeeAttribite.Fee;
                    // Initialize the drag & drop operation
                    DragDrop.DoDragDrop(textBlock, fee , DragDropEffects.Copy);
            }
        }
    }
}

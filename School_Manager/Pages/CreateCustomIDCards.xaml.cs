using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace School_Manager
{
    /// <summary>
    /// Interaction logic for CreateCustomIDCards.xaml
    /// </summary>
    public partial class CreateCustomIDCards : BasePage<CreateCustomIDCardsViewModel>
    {
        public CreateCustomIDCards()
        {
            InitializeComponent();
        }

        #region DragDrop Events

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

        #endregion
        
    }
}

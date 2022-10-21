using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace School_Manager
{
    /// <summary>
    /// Interaction logic for CardDesigner.xaml
    /// </summary>
    public partial class CardDesigner : UserControl
    {
        public CardDesigner()
        {
            InitializeComponent();
        }

        #region Dependency Properties

        public ObservableCollection<DesignerControls> ItemSource
        {
            get { return (ObservableCollection<DesignerControls>)GetValue(ItemSourceProperty); }
            set { SetValue(ItemSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemSourceProperty =
            DependencyProperty.Register("ItemSource", typeof(ObservableCollection<DesignerControls>), typeof(CardDesigner), new PropertyMetadata(new PropertyChangedCallback(OnItemSourceChanged)));
        private static void OnItemSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var searchableDataGrid = (CardDesigner)d;
            searchableDataGrid.ItemSource = e.NewValue as ObservableCollection<DesignerControls>;
        }


        public BitmapImage BackgroundImage
        {
            get { return (BitmapImage)GetValue(BackgroundImageProperty); }
            set { SetValue(BackgroundImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BackgroundImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BackgroundImageProperty =
            DependencyProperty.Register("BackgroundImage", typeof(BitmapImage), typeof(CardDesigner));



        #endregion

        private void ItemsControl_Drop(object sender, DragEventArgs e)
        {

            var qrControl = e.Data.GetData(typeof(QRCodeControl)) as QRCodeControl;
            if (qrControl != null)
            {
                ItemSource.Add(qrControl);
                return;
            }

            var image = e.Data.GetData(typeof(ImageControl)) as ImageControl;
            if (image != null)
            {
                ItemSource.Add(image);
                return;
            }

            var fee = e.Data.GetData(typeof(FeeControl)) as FeeControl;
            if (fee != null)
            {
                ItemSource.Add(fee);
                return;
            }

            var text = e.Data.GetData(typeof(TextControl)) as TextControl;
            if (text != null)
            {
                ItemSource.Add(text);
                return;
            }

            

        }

        private void ItemsControl_DragEnter(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.Text) ||
                    sender == e.Source)
            {
                e.Effects = DragDropEffects.None;
            }
        }
    }
}

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace School_Manager
{
    /// <summary>
    /// Interaction logic for PhotoTemplate.xaml
    /// </summary>
    public partial class PhotoTemplate : UserControl
    {
        public PhotoTemplate()
        {
            InitializeComponent();
        }


        public string BindingPath
        {
            get
            {
                return GetValue(BindingPathProperty) as string;
            }
            set
            {
                SetValue(BindingPathProperty, value);
                ImageContainer.SetBinding(Image.SourceProperty, new Binding(BindingPath) { Converter = new BitmapToThumbnailConverter()});
            }
        }
        public static readonly DependencyProperty BindingPathProperty =
        DependencyProperty.Register("BindingPath", typeof(string), typeof(PhotoTemplate), new PropertyMetadata(new PropertyChangedCallback(OnBindingPathChanged)));
        private static void OnBindingPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PhotoTemplate item = (PhotoTemplate)d;
            item.BindingPath = e.NewValue as string;
        }

    }
}

using System.Windows;

namespace School_Manager
{
    /// <summary>
    /// Interaction logic for ApplicationSetupWindow.xaml
    /// </summary>
    public partial class ApplicationSetupWindow : Window
    {
        public ApplicationSetupWindow()
        {
            InitializeComponent();
            DataContext = new WindowViewModel(this);
        }
    }
}

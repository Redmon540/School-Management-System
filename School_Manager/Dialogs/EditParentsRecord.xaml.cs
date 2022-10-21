using System.Windows.Controls;

namespace School_Manager
{
    /// <summary>
    /// Interaction logic for EditParentsRecord.xaml
    /// </summary>
    public partial class EditParentsRecord : UserControl
    {
        public EditParentsRecord(string ParentID)
        {
            InitializeComponent();

            //sets the view model
            Grid.DataContext = new EditParentsViewModel(ParentID);
        }
    }
}

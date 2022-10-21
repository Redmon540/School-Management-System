using System.Windows.Controls;

namespace School_Manager
{
    /// <summary>
    /// Interaction logic for ViewParentRecord.xaml
    /// </summary>
    public partial class ViewParentRecord : UserControl
    {
        public ViewParentRecord(string ParentID)
        {
            InitializeComponent();

            //sets the datacontext of the parent grid to viewmodel
            stackpanel.DataContext = new ViewParentRecordViewModel(ParentID);
        }
    }
}

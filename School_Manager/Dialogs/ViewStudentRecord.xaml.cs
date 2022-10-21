using System.Windows.Controls;

namespace School_Manager
{
    /// <summary>
    /// Interaction logic for ViewStudentRecord.xaml
    /// </summary>
    public partial class ViewStudentRecord : UserControl
    {
        #region Default Constructor

        public ViewStudentRecord(string StudentID, string ParentID, string ClassName)
        {
            InitializeComponent();

            //sets the datacontext of the parent grid to viewmodel
            stackpanel.DataContext = new ViewStudentRecordViewModel(StudentID, ParentID, ClassName);
        }

        #endregion
    }
}

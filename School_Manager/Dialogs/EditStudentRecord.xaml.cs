using System.Windows.Controls;

namespace School_Manager
{
    /// <summary>
    /// Interaction logic for EditStudentRecord.xaml
    /// </summary>
    public partial class EditStudentRecord : UserControl
    {
        #region Default Constructor

        public EditStudentRecord(string StudentID, string ParentID, string ClassName)
        {
            InitializeComponent();

            //sets the datacontext of the parent grid to viewmodel
            Grid.DataContext = new EditStudentRecordViewModel(StudentID, ParentID, ClassName);
        } 

        #endregion
    }
}

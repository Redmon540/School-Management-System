using System.Windows.Controls;

namespace School_Manager
{
    /// <summary>
    /// Interaction logic for EditTeacher.xaml
    /// </summary>
    public partial class EditTeacher : UserControl
    {
        public EditTeacher(string TeacherID)
        {
            InitializeComponent();

            //set the datacontext
            Grid.DataContext = new EditTeacherViewModel(TeacherID);
        }
    }
}

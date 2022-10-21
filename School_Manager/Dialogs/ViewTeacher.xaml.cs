using System.Windows.Controls;

namespace School_Manager
{
    /// <summary>
    /// Interaction logic for ViewTeacher.xaml
    /// </summary>
    public partial class ViewTeacher : UserControl
    {
        public ViewTeacher(string TeacherID)
        {
            InitializeComponent();

            //sets the datacontext
            stackpanel.DataContext = new ViewTeacherViewModel(TeacherID);
        }
    }
}

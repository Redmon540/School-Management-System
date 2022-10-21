using System.Windows.Controls;

namespace School_Manager
{
    /// <summary>
    /// Interaction logic for ViewFeeDetails.xaml
    /// </summary>
    public partial class ViewFeeDetails : UserControl
    {
        public ViewFeeDetails(string StudentID, string SelectedClass)
        {
            InitializeComponent();

            //sets the viewmodel binding
            DataContext = new ViewFeeDetailsViewModel(StudentID, SelectedClass);
        }
    }
}

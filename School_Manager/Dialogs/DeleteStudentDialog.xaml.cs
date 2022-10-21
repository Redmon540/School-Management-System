using System.Windows.Controls;

namespace School_Manager
{
    /// <summary>
    /// Interaction logic for DeleteStudentDialog.xaml
    /// </summary>
    public partial class DeleteStudentDialog : UserControl
    {
        public DeleteStudentDialog(string Message, string OKText, string CancelText, string CheckBoxContent, bool IsYesNoButtonVisible)
        {
            InitializeComponent();

            //make the view model
            var ViewModel = new DeleteStudentDialogViewModel(Message, OKText, CancelText, IsYesNoButtonVisible, CheckBoxContent);

            IsChecked = new ValueWrapper<bool>();

            //set the refrence
            IsYesPressed = ViewModel.IsYesPressed;
            IsChecked = ViewModel._IsChecked;

            //set the viewodel
            stackpanel.DataContext = ViewModel;
        }

        // <summary>
        /// A boolean that indicate if the YES button was pressed on dialog
        /// </summary>
        public ValueWrapper<bool> IsYesPressed { get; set; }

        // <summary>
        /// A boolean that indicate if the checkbox was checked on dialog
        /// </summary>
        public ValueWrapper<bool> IsChecked { get; set; }
    }
}

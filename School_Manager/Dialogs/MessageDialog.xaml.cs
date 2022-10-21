using System.Windows.Controls;

namespace School_Manager
{
    /// <summary>
    /// Interaction logic for MessageDialog.xaml
    /// </summary>
    public partial class MessageDialog : UserControl
    {
        public MessageDialog(string Message, string OKText, string CancelText, bool IsYesNoButtonVisible)
        {
            InitializeComponent();

            //make the view model
            var ViewModel = new MessageDialogViewModel(Message, OKText, CancelText, IsYesNoButtonVisible);

            //set the refrence
            IsYesPressed = ViewModel.IsYesPressed;

            //set the viewodel
            stackpanel.DataContext = ViewModel;
        }

        // <summary>
        /// A boolean that indicate if the YES button was pressed on dialog
        /// </summary>
        public ValueWrapper<bool> IsYesPressed { get; set; }
    }
}

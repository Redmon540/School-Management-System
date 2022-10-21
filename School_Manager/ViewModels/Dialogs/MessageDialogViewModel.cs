using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace School_Manager
{
    public class MessageDialogViewModel : BaseViewModel
    {
        #region Default Constructor

        public MessageDialogViewModel(string Message, string OkText, string CancelText, bool IsYesNoButtonVisible)
        {
            //Initialize the properties
            this.Message = Message;
            this.OkText = OkText;
            this.CancelText = CancelText;
            this.IsYesNoButtonVisible = IsYesNoButtonVisible;

            //Initialize the ref bool
            IsYesPressed = new ValueWrapper<bool>(false);

            //Initialize command
            OkCommand = new RelayCommand(OkCommandMethod);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Main Message of the dialog box
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Text for OK button
        /// </summary>
        public string OkText { get; set; }

        /// <summary>
        /// Text for Cancel Button 
        /// </summary>
        public string CancelText { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsYesNoButtonVisible { get; set; } = false;

        /// <summary>
        /// A boolean that indicate if the YES button was pressed on dialog
        /// </summary>
        public ValueWrapper<bool> IsYesPressed { get; set; }

        #endregion

        #region Public Command

        public ICommand OkCommand { get; set; }

        #endregion

        #region Command Methods

        private void OkCommandMethod()
        {
            IsYesPressed.Value = true;
            Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive).Close();
        }

        #endregion
    }
}

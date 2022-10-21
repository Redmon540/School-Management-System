namespace School_Manager
{
    /// <summary>
    /// View model for delete student record dialog
    /// </summary>
    public class DeleteStudentDialogViewModel : MessageDialogViewModel
    {
        #region Default Contructor

        public DeleteStudentDialogViewModel(string Message, string OkText, string CancelText, bool IsYesNoButtonVisible, string CheckBoxContent) : base(Message, OkText, CancelText, IsYesNoButtonVisible)
        {
            this.CheckBoxContent = CheckBoxContent;
            _IsChecked = new ValueWrapper<bool>();
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// text for checkbox
        /// </summary>
        public string CheckBoxContent { get; set; }

        /// <summary>
        /// refrence property
        /// </summary>
        public ValueWrapper<bool> _IsChecked { get; set; }

        /// <summary>
        /// reflects checkbox ischeked state
        /// </summary>
        public bool IsChecked
        {
            get => _IsChecked;
            set
            {
                if (value == _IsChecked)
                    return;

                    _IsChecked.Value = value;
            }
        }
        
        #endregion
    }
}

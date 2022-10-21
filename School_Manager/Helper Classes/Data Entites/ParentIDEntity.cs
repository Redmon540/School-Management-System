namespace School_Manager
{
    public class ParentIDEntity : TextEntity
    {
        /// <summary>
        /// Text to alert user when searched parent id doesn't found in the database
        /// </summary>
        public new string AlertText { get; set; } = "Invalid Parent ID.";

        private bool _IsValid = true;

        /// <summary>
        /// tells the alert text is visible or not
        /// </summary>
        public new bool IsValid
        {
            get
            {
                if (Value.IsNullOrEmpty())
                    return false;
                else if (IsEnabled)
                    return false;
                return _IsValid;
            }
            set
            {
                if (_IsValid != value)
                    _IsValid = value;
            }
        }
    }
}

using System.Data;
using System.Windows.Input;

namespace School_Manager
{
    public class SalarySheetViewModel : BaseViewModel
    {
        #region Constructor

        public SalarySheetViewModel()
        {
            EditCommand = new RelayCommand(Edit);
            EditButtonContent = App.Current.MainWindow.FindResource("EditIcon");
        }

        #endregion

        #region Properties

        public string Heading { get; set; }

        public DataTable ItemSource { get; set; }

        public bool IsEditButtonVisible { get; set; } = true;

        public bool CanEditHeading { get; set; } = false;

        public object EditButtonContent { get; set; }

        public bool CanAddRows { get; set; } = true;

        #endregion

        #region Commands

        public ICommand EditCommand { get; set; }

        #endregion

        #region Command Methods

        private void Edit()
        {
            if (CanEditHeading)
            {
                CanEditHeading = false;
                EditButtonContent = App.Current.MainWindow.FindResource("EditIcon");
            }
            else
            {
                CanEditHeading = true;
                EditButtonContent = App.Current.MainWindow.FindResource("CheckIcon");
            }
        }

        #endregion
    }
}

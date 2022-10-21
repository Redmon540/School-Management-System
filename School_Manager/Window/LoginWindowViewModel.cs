using System.Windows;
using System.Windows.Input;

namespace School_Manager
{
    public class LoginWindowViewModel : BaseViewModel
    {
        #region Constructor

        public LoginWindowViewModel(Window window)
        {
            _window = window;
            CloseCommand = new RelayCommand(() => Application.Current.MainWindow.Close());
            CurrentPage = ApplicationPage.Login;
            
        }

        #endregion

        #region Private members

        private Window _window;

        private ApplicationPage _CurrentPage;

        #endregion

        #region Public Properties

        public ApplicationPage CurrentPage
        {
            get
            {
                return _CurrentPage;
            }
            set
            {
                if (_CurrentPage == value)
                    return;
                _CurrentPage = value;
            }
        }

        #endregion

        #region Commands

        public ICommand CloseCommand { get; set; }

        #endregion

        
    }
}

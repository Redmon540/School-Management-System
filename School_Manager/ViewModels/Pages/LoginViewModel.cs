using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace School_Manager
{
    public class LoginViewModel : BaseViewModel
    {
        #region Constructor

        public LoginViewModel()
        {
            //set the properties
            Accounts = DataAccess.GetDataTable("SELECT [Account_ID] , [UserName] FROM Accounts").AsEnumerable().Select(s=> new AccountEntity() { Account_ID = s[0].ToString(), AccountName = s[1].ToString() }).ToList();

            //Create Commands
            LoginCommand = new RelayParameterizedCommand(async (parameter) => await Login(parameter));
            GotoSignUpCommand = new RelayCommand(GotoSignUpPage);
        }

        #endregion
       
        #region Public Properties

        public AccountEntity SelectedAccount { get; set; }
        
        public bool IsLoginRunning { get; set; }

        public List<AccountEntity> Accounts { get; set; }

        #endregion

        #region Public Commands

        public ICommand LoginCommand { get; set; }

        public ICommand GotoSignUpCommand { get; set; }

        #endregion

        #region Command Methods

        public void GotoSignUpPage()
        {
            (Application.Current.MainWindow.DataContext as LoginWindowViewModel).CurrentPage = ApplicationPage.SignUp;
        }

        /// <summary>
        /// Attempts to log the user in
        /// </summary>
        /// <param name="parameter">The <see cref="SecureString"/>passed in from the view for the user password</param>
        /// <returns></returns>
        public async Task Login(object parameter)
        {
            await RunCommandAsync(() => IsLoginRunning, async () =>
             {
                 string q1 = $"SELECT PasswordHash FROM Accounts WHERE Account_ID = {SelectedAccount.Account_ID}";
                 string q2 = $"SELECT Salt FROM Accounts WHERE Account_ID = {SelectedAccount.Account_ID}";
                 bool IsPasswordMatched = false;
                 await Task.Run(() =>
                 {
                     IsPasswordMatched = Helper.CompareHashes(
                         (parameter as IHavePassword).SecurePassword,
                        DataAccess.GetDataTable(q1).Rows[0][0] as byte[],
                        DataAccess.GetDataTable(q2).Rows[0][0] as byte[]
                        );
                 });
                 if (IsPasswordMatched)
                 {
                     var loginWindow = Application.Current.MainWindow;
                     var window = new MainWindow();
                     window.Show();
                     loginWindow.Close();
                 }
                 else
                 {
                     DialogManager.ShowMessageDialog("Warning", "Incorrect Password!",DialogTitleColor.Red);
                 }

             });
        } 

        #endregion
    }
}

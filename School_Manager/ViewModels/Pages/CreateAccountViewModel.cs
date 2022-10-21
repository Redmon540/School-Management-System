using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace School_Manager
{
    public class CreateAccountViewModel : BaseViewModel
    {
        #region Constructor

        public CreateAccountViewModel()
        {
            //Create Commands
            SignUpCommand = new RelayParameterizedCommand(async (parameter) => await SignUp(parameter));
            GotoLoginCommand = new RelayCommand(GotoLoginPage);

            //set the properties
            AccountTypes = DataAccess.GetDataTable("SELECT [Account Type] FROM AccountType").AsEnumerable().Select(x => x[0].ToString()).ToList();
        }

        #endregion

        #region Public Properties

        public string UserName { get; set; }

        public bool IsLoginRunning { get; set; }

        public List<string> AccountTypes { get; set; }

        public string SelectedAccountType { get; set; }

        #endregion

        #region Public Commands

        public ICommand SignUpCommand { get; set; }

        public ICommand GotoLoginCommand { get; set; }


        #endregion

        #region Command Methods

        public void GotoLoginPage()
        {
            (Application.Current.MainWindow.DataContext as LoginWindowViewModel).CurrentPage = ApplicationPage.Login;
        }

        /// <summary>
        /// Attempts to log the user in
        /// </summary>
        /// <param name="parameter">The <see cref="SecureString"/>passed in from the view for the user password</param>
        /// <returns></returns>
        public async Task SignUp(object parameter)
        {
            await RunCommandAsync(() => IsLoginRunning, async () =>
            {
                //if any feild is null or empty show message
                if (SelectedAccountType.IsNullOrEmpty() || UserName.IsNullOrEmpty() ||
                (parameter as IHavePassword).SecurePassword.Length < 0 ||
                (parameter as IHavePassword).SecureConfirmPassword.Length < 0 ||
                (parameter as IHavePassword).SecureAdminPassword.Length < 0)
                {
                    DialogManager.ShowMessageDialog("Warning!", "Inputs cannot be null.",DialogTitleColor.Red);
                    return;
                }
                //if password don't match show message
                if((parameter as IHavePassword).SecurePassword.Unsecure() != (parameter as IHavePassword).SecureConfirmPassword.Unsecure())
                {
                    DialogManager.ShowMessageDialog("Warning!", "Password & cofirmation password don't match.",DialogTitleColor.Red);
                    return;
                }
                bool match = false;
                bool IsSignUp = false;
                await Task.Run(() =>
                {
                    string q1 = "SELECT PasswordHash FROM Accounts WHERE Account_ID = 1";
                    string q2 = "SELECT Salt FROM Accounts WHERE Account_ID = 1";
                    match =  Helper.CompareHashes(
                        (parameter as IHavePassword).SecureAdminPassword,
                        DataAccess.GetDataTable(q1).Rows[0][0] as byte[],
                        DataAccess.GetDataTable(q2).Rows[0][0] as byte[]
                        );
                if (match)
                {
                    IsSignUp = DataAccess.InsertAccount(SelectedAccountType, UserName, (parameter as IHavePassword).SecurePassword);
                }
                });
                if (IsSignUp)
                {
                    DialogManager.ShowMessageDialog("Message", "Account Created Successfully!",DialogTitleColor.Green);
                    PageManager.GotoPage(ApplicationPage.Login);
                }
                if(!match)  
                    DialogManager.ShowMessageDialog("Warning!", "Please enter correct admin password.", DialogTitleColor.Red);
            });
        } 

        #endregion
    }
}

using System.Security;


namespace School_Manager
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : BasePage<LoginViewModel> , IHavePassword
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        public SecureString SecurePassword => PasswordText.SecurePassword;

        public SecureString SecureConfirmPassword => throw new System.NotImplementedException();

        public SecureString SecureAdminPassword => throw new System.NotImplementedException();
    }
}

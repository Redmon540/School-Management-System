using System.Security;


namespace School_Manager
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class CreateAccount : BasePage<CreateAccountViewModel> , IHavePassword
    {
        public CreateAccount()
        {
            InitializeComponent();
        }

        public SecureString SecurePassword => PasswordText.SecurePassword;
        public SecureString SecureConfirmPassword => ConfirmPasswordText.SecurePassword;
        public SecureString SecureAdminPassword => AdminPasswordText.SecurePassword;
    }
}

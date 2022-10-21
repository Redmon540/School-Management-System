using System.Windows;

namespace School_Manager
{
    public class RegistrationControlViewModel : RegistrationViewModel
    {
        protected override void Register()
        {
            if (!RegistrationKey.IsValid || !Type.IsValid)
            {
                DialogManager.ShowValidationMessage();
                return;
            }
            RegistrationType type = RegistrationType.Monthly;
            if (Type.Value == "Half Yearly")
                type = RegistrationType.HalfYearly;
            else if (Type.Value == "Yearly")
                type = RegistrationType.Yearly;
            else if (Type.Value == "Full Time")
                type = RegistrationType.FullTime;

            if (DataAccess.Register(RegistrationKey.Value, type, StartDate, EndDate))
            {
                DialogManager.ShowMessageDialog("Message", "Product has been registered successfully.",DialogTitleColor.Green);
                var regisWindow = Application.Current.MainWindow;
                var window = new MainWindow();
                window.Show();
                regisWindow.Close();
            }
            else
                DialogManager.ShowMessageDialog("Message", "Product registration failed.",DialogTitleColor.Red);
        }
    }
}

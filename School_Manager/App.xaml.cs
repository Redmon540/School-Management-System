using System;
using System.Configuration;
using System.Windows;

namespace School_Manager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            //starts the stop watch to know how much time user is spending on application
            Helper.StartTimer();

            //hook the exist event
            this.Exit += Application_Exit;

            //set the master connection string for setting up application
            DataAccess.connectionString = ConfigurationManager.ConnectionStrings["MasterConnectionString"].ConnectionString;

            //checks if the registration information is setup or not
            if (DataAccess.SetupApplication())
            {

                //set the default connection string
                DataAccess.connectionString = ConfigurationManager.ConnectionStrings["DefaultConnectionString"].ConnectionString;

                //set the product information
                DataAccess.SetProductInfo();

                //check if the registration haven't expired yet
                var registrationStatus = DataAccess.CheckRegistration();
                if (registrationStatus.Status == RegistrationWarning.Registered)
                {
                    //show the startup window of the application
                    Current.MainWindow = new LoginWindow();
                    MainWindow.Show();
                }
                else if (registrationStatus.Status == RegistrationWarning.Alert)
                {
                    //show the startup window of the application and registration alert
                    Current.MainWindow = new LoginWindow();
                    MainWindow.Show();
                    DialogManager.ShowMessageDialog("Warning", $"Please register your product it will expire in {registrationStatus.DaysLeft} days.", DialogTitleColor.Yellow);
                }
                else
                {
                    //show the registration window
                    Current.MainWindow = new RegistrationWindow();
                    MainWindow.Show();
                }
            }

        }
        
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            //stops the stopwatch
            Helper.StopTimer();
            //saves the spent time
            var time = (TimeSpan.Parse(DataAccess.GetDataTable("SELECT [Run_Time] FROM Registration").Rows[0][0]
                .ToString().Decrypt()) + Helper.Timer.Elapsed).ToString().Encrypt();
            DataAccess.ExecuteQuery($"UPDATE Registration SET [Run_Time] = '{time}'");
        }
    }
}

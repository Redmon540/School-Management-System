using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace School_Manager
{
    public class SetupApplicationViewModel : BaseViewModel
    {
        #region Default Constructor

        public SetupApplicationViewModel()
        {
            SetupCommand = new RelayCommand(Setup);
        }

        #endregion

        #region Properties

        public string ProductID { get; set; } = "0000";

        public string InstituteName { get; set; }

        #endregion

        #region Commands

        public ICommand SetupCommand { get; set; }

        #endregion

        #region Command Methods

        private void Setup()
        {
            if(ProductID.IsNullOrEmpty() || InstituteName.IsNullOrEmpty())
            {
                DialogManager.ShowValidationMessage();
                return;
            }

            var script = File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Scripts.txt"));
            
            //first create the database 
            DataAccess.ExecuteQuery("create database [ims_db]");

            SqlConnection conn = new SqlConnection(DataAccess.connectionString);

            // split script on GO command
            IEnumerable<string> commandStrings = Regex.Split(script, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);

            conn.Open();
            foreach (string commandString in commandStrings)
            {
                if (commandString.Trim() != "")
                {
                    using (var command = new SqlCommand(commandString, conn))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
            conn.Close();

            DataAccess.ExecuteQuery($"INSERT INTO [ims_db].[dbo].[Product_Information]([Product_ID],[Product_Version],[Registered_To],[Registered_Date]) " +
                $"VALUES('{ProductID}', 'v.1.0.0', '{InstituteName}', GETDATE())");

            //set the default connection string
            DataAccess.connectionString = ConfigurationManager.ConnectionStrings["DefaultConnectionString"].ConnectionString;

            //set the product information
            DataAccess.SetProductInfo();

            var currentWindow = Application.Current.MainWindow;

            //check if the registration haven't expired yet
            var registrationStatus = DataAccess.CheckRegistration();
            if (registrationStatus.Status == RegistrationWarning.Registered)
            {
                //show the startup window of the application
                Application.Current.MainWindow = new LoginWindow();
                Application.Current.MainWindow.Show();
                currentWindow.Close();
            }
            else if (registrationStatus.Status == RegistrationWarning.Alert)
            {
                //show the startup window of the application and registration alert
                Application.Current.MainWindow = new LoginWindow();
                Application.Current.MainWindow.Show();
                DialogManager.ShowMessageDialog("Warning", $"Please register your product it will expire in {registrationStatus.DaysLeft} days.", DialogTitleColor.Yellow);
                currentWindow.Close();
            }
            else
            {
                //show the registration window
                Application.Current.MainWindow = new LoginWindow();
                Application.Current.MainWindow.Show();
                currentWindow.Close();
            }
            
        }

        #endregion
    }
}

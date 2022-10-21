using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace School_Manager
{
    public class SetProductInfoViewModel : BaseViewModel
    {
        #region Constructor

        public SetProductInfoViewModel()
        {
            //set the commands
            SetInfoCommand = new RelayCommand(SetInfo);
            
            //set the properties
            StudentInfo = new List<Check>
            {
                new Check{ Content = "Photo" , IsChecked = true , IsEnabled = false},
                new Check{ Content = "Name" , IsChecked = true, IsEnabled = false},
                new Check{ Content = "Gender" , IsChecked = true},
                new Check{ Content = "Blood Group" , IsChecked = true},
                new Check{ Content = "CNIC" , IsChecked = true},
                new Check{ Content = "Date Of Birth" , IsChecked = true },
                new Check{ Content = "Language" , IsChecked = true },
                new Check{ Content = "Contact" , IsChecked = true },
                new Check{ Content = "Email" , IsChecked = true },
                new Check{ Content = "Address" , IsChecked = true },
            };
            ParentInfo = new List<Check>
            {
                new Check{ Content = "Father Photo" , IsChecked = true , IsEnabled = false},
                new Check{ Content = "Father Name" , IsChecked = true, IsEnabled = false},
                new Check{ Content = "Mother Photo" , IsChecked = true},
                new Check{ Content = "Mother Name" , IsChecked = true},
                new Check{ Content = "Father CNIC" , IsChecked = true },
                new Check{ Content = "Mother CNIC" , IsChecked = true },
                new Check{ Content = "Father Occupation" , IsChecked = true },
                new Check{ Content = "Mother Occupation" , IsChecked = true },
                new Check{ Content = "Mother Occupation" , IsChecked = true },
                new Check{ Content = "Father Contact" , IsChecked = true },
                new Check{ Content = "Mother Contact" , IsChecked = true },
                new Check{ Content = "Parent E-mail" , IsChecked = true },
            };
            TeacherInfo = new List<Check>
            {
                new Check{ Content = "Photo" , IsChecked = true , IsEnabled = false},
                new Check{ Content = "Name" , IsChecked = true, IsEnabled = false},
                new Check{ Content = "Father Name" , IsChecked = true, IsEnabled = false},
                new Check{ Content = "Gender" , IsChecked = true},
                new Check{ Content = "Blood Group" , IsChecked = true},
                new Check{ Content = "Date Of Birth" , IsChecked = true },
                new Check{ Content = "Language" , IsChecked = true },
                new Check{ Content = "CNIC" , IsChecked = true},
                new Check{ Content = "Date of Join" , IsChecked = true},
                new Check{ Content = "Rank" , IsChecked = true},
                new Check{ Content = "Contact" , IsChecked = true },
                new Check{ Content = "Email" , IsChecked = true },
                new Check{ Content = "Address" , IsChecked = true },
            };
        }

        #endregion

        #region Properties

        public TextEntity ProductID { get; set; } = new TextEntity { FeildName = "Product ID", ValidationType = ValidationType.NotEmpty };

        public TextEntity ProductVersion { get; set; } = new TextEntity { FeildName = "Product Version", ValidationType = ValidationType.NotEmpty };

        public TextEntity RegisteredTo { get; set; } = new TextEntity { FeildName = "Institute", ValidationType = ValidationType.NotEmpty };

        public TextEntity RegistrationDate { get; set; } = new TextEntity { FeildName = "Registration Date", ValidationType = ValidationType.NotEmpty };

        public List<Check> StudentInfo { get; set; }

        public List<Check> ParentInfo { get; set; }

        public List<Check> TeacherInfo { get; set; }

        #endregion

        #region Commands

        public ICommand SetInfoCommand { get; set; }

        #endregion

        #region Command Methods

        private void SetInfo()
        {
            if(ProductID.IsValid && ProductVersion.IsValid && RegisteredTo.IsValid && RegistrationDate.IsValid)
            {
                DataAccess.ExecuteQuery("INSERT INTO ProductInformation ( [Product_ID],[Product_Version],[Registered_To],[Registered_Date] ) " +
                    $"VALUES ('{ProductID.Value}' , '{ProductVersion.Value}' , '{RegisteredTo.Value}' , '{RegistrationDate.Value}')");

                Application.Current.MainWindow = new RegistrationWindow();
                Application.Current.MainWindow.Show();
            }
        }

        #endregion
    }
}

using System.Collections.ObjectModel;
using System.Windows.Input;

namespace School_Manager
{
    public class CreateTeachersViewModel : BaseViewModel
    {
        #region Default Constructor

        public CreateTeachersViewModel()
        {
            //initialize the properties
            TeachersEntities = new ObservableCollection<Check>()
            {
                new Check() { Content = "Photo" , IsChecked=true},
                new Check() { Content = "Name" , IsChecked=true},
                new Check() { Content = "Father Name" , IsChecked=true},
                new Check() { Content = "Gender" , IsChecked=true},
                new Check() { Content = "Date of Birth" , IsChecked=true},
                new Check() { Content = "CNIC" , IsChecked=true},
                new Check() { Content = "Religion" , IsChecked=true},
                new Check() { Content = "Language" , IsChecked=true},
                new Check() { Content = "Blood Group" , IsChecked=true},
                new Check() { Content = "Contact" , IsChecked=true},
                new Check() { Content = "Email" , IsChecked=true},
                new Check() { Content = "Address" , IsChecked=true},
                new Check() { Content = "Salary" , IsChecked=true},
            };
            CustomEntities = new ObservableCollection<Check>();

            //initialize commands
            CreateTeachersCommand = new RelayCommand(CreateTeachersMethod);
            AddCustomColumn = new RelayCommand(AddCustomColumnMethod);
            RemoveCustomEntity = new RelayParameterizedCommand(parameter => RemoveCustomEntityMethod(parameter));
        }

        #endregion


        #region Public Properties

        public ObservableCollection<Check> TeachersEntities { get; set; }

        public ObservableCollection<Check> CustomEntities { get; set; }

        #endregion

        #region Commands

        public ICommand CreateTeachersCommand { get; set; }

        public ICommand AddCustomColumn { get; set; }

        public ICommand RemoveCustomEntity { get; set; }

        #endregion

        #region Command Methods

        private void CreateTeachersMethod()
        {
            string query = "";
            foreach (Check item in TeachersEntities)
            {
                if(item.IsChecked)
                {
                    if (item.Content.ToLower().Contains("date"))
                        query += $", [{item.Content}] [date] NULL";
                    else if (item.Content.ToLower().Contains("photo"))
                        query += $", [{item.Content}] [image] NULL";
                    else if (item.Content.ToLower().Contains("salary"))
                        query += $", [{item.Content}] [int] NULL";
                    else query += $", [{item.Content}] [nvarchar] (100) NULL";
                }
            }
            foreach (Check item in CustomEntities)
            {
                if(!string.IsNullOrEmpty(item.Content))
                query += $", [{item.Content}] [nvarchar] (100) NULL";
            }
            query = $"CREATE TABLE [Teachers] ( [Teacher ID] [int] IDENTITY(1,1) NOT NULL  {query} , [Class Under Supervision] [nvarchar] (100) NULL )";
            DataAccess.ExecuteQuery(query);
            DialogManager.ShowMessageDialog("Message", "Teachers Record Created Successfully.",DialogTitleColor.Green);
        }

        private void AddCustomColumnMethod()
        {
            CustomEntities.Add(new Check());
        }

        private void RemoveCustomEntityMethod(object parameter)
        {
            Check entity = parameter as Check;
            foreach (Check i in CustomEntities)
            {
                if (entity == i)
                {
                    CustomEntities.Remove(i);
                    break;
                }
            }
        }

        #endregion
    }
}

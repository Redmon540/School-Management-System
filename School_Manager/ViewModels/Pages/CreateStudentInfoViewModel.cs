using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace School_Manager
{
    public class CreateStudentInfoViewModel : BaseViewModel
    {
        #region Constructor

        public CreateStudentInfoViewModel()
        {
            //set the properties
            StudentColumn = new List<Check>
            {
                new Check{ Content = "Name" , IsEnabled = false ,IsChecked = true},
                new Check{ Content = "Photo" ,IsChecked = true},
                new Check{ Content = "Gender" ,IsChecked = true},
                new Check{ Content = "Date of Birth" ,IsChecked = true},
                new Check{ Content = "Religion" ,IsChecked = true},
                new Check{ Content = "Language" ,IsChecked = true},
                new Check{ Content = "Blood Group" ,IsChecked = true},
                new Check{ Content = "CNIC/B-Form" ,IsChecked = true},
                new Check{ Content = "Contact" ,IsChecked = true},
                new Check{ Content = "Email" ,IsChecked = true},
                new Check{ Content = "Address" ,IsChecked = true},
            };
            ParentColumn = new List<Check>
            {
                new Check{ Content = "Father Name" , IsEnabled = false ,IsChecked = true},
                new Check{ Content = "Mother Name" ,IsChecked = true},
                new Check{ Content = "Father Photo" ,IsChecked = true},
                new Check{ Content = "Mother Photo" ,IsChecked = true},
                new Check{ Content = "Father CNIC" ,IsChecked = true},
                new Check{ Content = "Mother CNIC" ,IsChecked = true},
                new Check{ Content = "Father Occupation" ,IsChecked = true},
                new Check{ Content = "Father Contact" ,IsChecked = true},
                new Check{ Content = "Mother Contact" ,IsChecked = true},
                new Check{ Content = "Father Salary" ,IsChecked = true},
                new Check{ Content = "Parent Email" ,IsChecked = true},
            };
            CustomStudentColumns = new ObservableCollection<TextEntity>();
            CustomParentColumns = new ObservableCollection<TextEntity>();

            //set commands
            AddCustomStudentColumn = new RelayCommand(AddStudentColumn);
            AddCustomParentColumn = new RelayCommand(AddParentColumn);
            RemoveStudentColumnCommand = new RelayParameterizedCommand(parameter => RemoveStudentColumn(parameter));
            RemoveParentColumnCommand = new RelayParameterizedCommand(parameter => RemoveParentColumn(parameter));
            CreateStructureCommand = new RelayCommand(CreateStructure);
        }

        #endregion

        #region Properties

        public List<Check> StudentColumn { get; set; }

        public List<Check> ParentColumn { get; set; }

        public ObservableCollection<TextEntity> CustomStudentColumns { get; set; }

        public ObservableCollection<TextEntity> CustomParentColumns { get; set; }

        public TextEntity StudentID { get; set; } = new TextEntity { FeildName = "ID Name", Value = "Gr.No." , ValidationType = ValidationType.NotEmpty };

        public TextEntity ParentID { get; set; } = new TextEntity { FeildName = "ID Name", Value = "Parent ID" , ValidationType = ValidationType.NotEmpty };

        #endregion

        #region Commands

        public ICommand AddCustomStudentColumn { get; set; }

        public ICommand AddCustomParentColumn { get; set; }

        public ICommand RemoveStudentColumnCommand { get; set; }

        public ICommand RemoveParentColumnCommand { get; set; }

        public ICommand CreateStructureCommand { get; set; }

        #endregion

        #region Command Methods

        private void AddStudentColumn()
        {
            CustomStudentColumns.Add(new TextEntity() { ValidationType = ValidationType.NotEmpty });
        }

        private void AddParentColumn()
        {
            CustomParentColumns.Add(new TextEntity() { ValidationType = ValidationType.NotEmpty });
        }

        private void RemoveStudentColumn(object sender)
        {
            CustomStudentColumns.Remove(sender as TextEntity);
        }

        private void RemoveParentColumn(object sender)
        {
            CustomParentColumns.Remove(sender as TextEntity);
        }

        private void CreateStructure()
        {
            string columns1 = "";
            foreach (var item in StudentColumn)
            {
                columns1 += $",[{item}] varchar(100)";
            }
            foreach (var item in CustomStudentColumns)
            {
                columns1 += $",[{item}] varchar(100)";
            }

            DataAccess.ExecuteQuery($"CREATE TABLE Students([Student_ID] int primary key identity {columns1} , [Parent_ID] int , [Class_ID] int, [Section_ID] int)");

            string columns2 = "";
            foreach (var item in ParentColumn)
            {
                columns2 += $",[{item}] varchar(100)";
            }
            foreach (var item in CustomParentColumns)
            {
                columns2 += $",[{item}] varchar(100)";
            }
            DataAccess.ExecuteQuery($"CREATE TABLE Parents([Parent_ID] int primary key identity {columns1} )");
        }

        #endregion
    }
}

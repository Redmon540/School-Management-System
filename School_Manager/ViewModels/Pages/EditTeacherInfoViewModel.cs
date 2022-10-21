using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace School_Manager
{ 
    public class EditTeacherInfoViewModel : BaseViewModel
    {
        #region Constructor

        public EditTeacherInfoViewModel()
        {
            //set the properties
            CustomTeacherColumns = new ObservableCollection<TextEntity>();

            //set commands
            AddCustomTeacherColumn = new RelayCommand(AddTeacherColumn);
            RemoveStudentColumnCommand = new RelayParameterizedCommand(parameter => RemoveTeacherColumn(parameter));
            DeleteCommand = new RelayParameterizedCommand(parameter => DeleteColumn(parameter));
            EditCommand = new RelayParameterizedCommand(parameter => Edit(parameter));
            UpdateStructureCommand = new RelayCommand(UpdateStructure);

            SetColumns();
        }

        #endregion

        #region Properties

        public ObservableCollection<TextEntry> TeacherColumn { get; set; }

        public ObservableCollection<TextEntity> CustomTeacherColumns { get; set; }

        public TextEntity TeacherID { get; set; } = new TextEntity
        {
            FeildName = "ID Name",
            Value = DataAccess.GetTeacherID(),
            OriginalValue = DataAccess.GetTeacherID(),
            ValidationType = ValidationType.NotEmpty
        };

        #endregion

        #region Commands

        public ICommand AddCustomTeacherColumn { get; set; }

        public ICommand RemoveStudentColumnCommand { get; set; }

        public ICommand DeleteCommand { get; set; }

        public ICommand EditCommand { get; set; }

        public ICommand UpdateStructureCommand { get; set; }

        #endregion

        #region Command Methods

        private void SetColumns()
        {
            try
            {
                TeacherColumn = new ObservableCollection<TextEntry>(DataAccess.GetDataTable("SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Teachers'")
                .AsEnumerable().Where(x => !x[0].ToString().Contains("_")).
                Select(x => new TextEntry { Text = x[0].ToString(), IsEnabled = x[0].ToString() == "Name" || x[0].ToString() == "Photo" ? false : true }).ToList());
            }
            catch (Exception ex)
            {
                Logger.Log(ex, true);
            }
        }

        private void AddTeacherColumn()
        {
            CustomTeacherColumns.Add(new TextEntity() { ValidationType = ValidationType.NotEmpty });
        }

        private void RemoveTeacherColumn(object sender)
        {
            CustomTeacherColumns.Remove(sender as TextEntity);
        }

        private void Edit(object sender)
        {
            var item = sender as TextEntry;
            if (item.IsEditing)
            {
                item.EditIcon = Application.Current.FindResource("EditIcon") as string;
                item.DeleteIcon = Application.Current.FindResource("DeleteBinIcon") as string;
                if (item.Text != item.BackupText)
                {
                    DataAccess.ExecuteQuery($"EXEC sp_RENAME 'Teachers.{item.BackupText}' , '{item.Text}', 'COLUMN'");
                    DataAccess.ExecuteQuery($"UPDATE _ColumnInformation SET Custom_Columns = NULL , Search_Columns = NULL WHERE Table_Name = 'Teachers' ");
                }
                item.IsEditing = false;
            }
            else
            {
                item.EditIcon = Application.Current.FindResource("CheckIcon") as string;
                item.DeleteIcon = Application.Current.FindResource("CrossIcon") as string;
                item.BackupText = item.Text;
                item.IsEditing = true;
            }
        }

        private void DeleteColumn(object sender)
        {
            try
            {
                var item = sender as TextEntry;
                if (item.IsEditing)
                {
                    item.EditIcon = Application.Current.FindResource("EditIcon") as string;
                    item.DeleteIcon = Application.Current.FindResource("DeleteBinIcon") as string;
                    item.Text = item.BackupText;
                    item.IsEditing = false;
                }
                else if(DialogManager.ShowMessageDialog("Warning", "Are you sure you want to delete this feild? It will effect all teachers.", true))
                {
                    DataAccess.ExecuteQuery($"ALTER TABLE Teachers DROP COLUMN [{item.Text}]");
                    TeacherColumn.Remove(item);
                    DataAccess.ExecuteQuery($"UPDATE _ColumnInformation SET Custom_Columns = NULL , Search_Columns = NULL WHERE Table_Name = 'Teachers' ");
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex, true);
            }
        }

        private void UpdateStructure()
        {
            try
            {
                bool isValid = true;
                foreach (var item in CustomTeacherColumns)
                {
                    if (!item.IsValid)
                        isValid = false;
                }
                if (!TeacherID.IsValid)
                    isValid = false;

                if (!isValid)
                {
                    DialogManager.ShowValidationMessage();
                    return;
                }
                foreach (var item in CustomTeacherColumns)
                {
                    DataAccess.ExecuteQuery($"ALTER TABLE Teachers ADD [{item.Value}] varchar(100)");
                }

                DataAccess.ExecuteQuery($"UPDATE _ColumnInformation SET Custom_Columns = '{TeacherID.Value}' WHERE Table_Name = 'Teacher_ID_Name' ");
                DataAccess.ExecuteQuery($"UPDATE _ColumnInformation SET Custom_Columns = NULL , Search_Columns = NULL WHERE Table_Name = 'Teachers' ");

                CustomTeacherColumns = new ObservableCollection<TextEntity>();
                SetColumns();

                DialogManager.ShowMessageDialog("Message", "Information Structure Updated Successfully.",DialogTitleColor.Green);
            }
            catch (Exception ex)
            {
                Logger.Log(ex, true);
            }

        }

        #endregion
    }
}

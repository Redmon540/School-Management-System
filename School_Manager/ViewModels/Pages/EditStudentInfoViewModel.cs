using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace School_Manager
{
    public class EditStudentInfoViewModel : BaseViewModel
    {
        #region Constructor

        public EditStudentInfoViewModel()
        {
            //set the properties
            CustomStudentColumns = new ObservableCollection<TextEntity>();
            CustomParentColumns = new ObservableCollection<TextEntity>();

            //set commands
            AddCustomStudentColumn = new RelayCommand(AddStudentColumn);
            AddCustomParentColumn = new RelayCommand(AddParentColumn);
            RemoveStudentColumnCommand = new RelayParameterizedCommand(parameter => RemoveStudentColumn(parameter));
            RemoveParentColumnCommand = new RelayParameterizedCommand(parameter => RemoveParentColumn(parameter));
            DeleteCommand = new RelayParameterizedCommand(parameter => Delete(parameter));
            EditCommand = new RelayParameterizedCommand(parameter => Edit(parameter));
            UpdateStructureCommand = new RelayCommand(UpdateStructure);

            SetColumns();
        }

        #endregion

        #region Properties

        public ObservableCollection<TextEntry> StudentColumn { get; set; }

        public ObservableCollection<TextEntry> ParentColumn { get; set; }

        public ObservableCollection<TextEntity> CustomStudentColumns { get; set; }

        public ObservableCollection<TextEntity> CustomParentColumns { get; set; }

        public TextEntity StudentID { get; set; } = new TextEntity { FeildName = "ID Name",
            Value = DataAccess.GetStudentID(), OriginalValue = DataAccess.GetStudentID(),
            ValidationType = ValidationType.NotEmpty };

        public TextEntity ParentID { get; set; } = new TextEntity { FeildName = "ID Name",
            Value = DataAccess.GetParentID(), OriginalValue = DataAccess.GetParentID(),
            ValidationType = ValidationType.NotEmpty };

        #endregion

        #region Commands

        public ICommand AddCustomStudentColumn { get; set; }

        public ICommand AddCustomParentColumn { get; set; }

        public ICommand RemoveStudentColumnCommand { get; set; }

        public ICommand RemoveParentColumnCommand { get; set; }

        public ICommand DeleteCommand { get; set; }

        public ICommand EditCommand { get; set; }

        public ICommand UpdateStructureCommand { get; set; }

        #endregion

        #region Command Methods

        private void SetColumns()
        {
            try
            {
                StudentColumn = new ObservableCollection<TextEntry>(DataAccess.GetDataTable("SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Students'")
                .AsEnumerable().Where(x => !x[0].ToString().Contains("_")).
                Select(x => new TextEntry { Text = x[0].ToString() ,IsEnabled = x[0].ToString() == "Name" || x[0].ToString() == "Photo" ? false : true }).ToList());

                ParentColumn = new ObservableCollection<TextEntry>(DataAccess.GetDataTable("SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Parents'")
                    .AsEnumerable().Where(x => !x[0].ToString().Contains("_")).
                    Select(x => new TextEntry { Text = x[0].ToString(), IsEnabled = x[0].ToString().Contains("Father Name") || x[0].ToString().Contains("Photo") ? false : true }).ToList());
            }
            catch (Exception ex)
            {
                Logger.Log(ex, true);
            }
        }

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

        private void Edit(object sender)
        {
            var item = sender as TextEntry;
            if (item.IsEditing)
            {
                item.EditIcon = Application.Current.FindResource("EditIcon") as string;
                item.DeleteIcon = Application.Current.FindResource("DeleteBinIcon") as string;
                if(StudentColumn.Contains(item))
                {
                    if(item.Text != item.BackupText)
                    {
                        DataAccess.ExecuteQuery($"EXEC sp_RENAME 'Students.{item.BackupText}' , '{item.Text}', 'COLUMN'");
                        DataAccess.ExecuteQuery($"UPDATE ColumnInformation SET Custom_Columns = NULL , Search_Columns = NULL");
                    }
                }
                else if(ParentColumn.Contains(item))
                {
                    if (item.Text != item.BackupText)
                    {
                        DataAccess.ExecuteQuery($"EXEC sp_RENAME 'Parents.{item.BackupText}' , '{item.Text}', 'COLUMN'");
                        DataAccess.ExecuteQuery($"UPDATE _ColumnInformation SET Custom_Columns = NULL , Search_Columns = NULL WHERE Table_Name = 'Parents' ");
                    }
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

        private void Delete(object sender)
        {
            try
            {
                var item = sender as TextEntry;
                if(item.IsEditing)
                {
                    item.EditIcon = Application.Current.FindResource("EditIcon") as string;
                    item.DeleteIcon = Application.Current.FindResource("DeleteBinIcon") as string;
                    item.Text = item.BackupText;
                    item.IsEditing = false;
                }
                else if(DialogManager.ShowMessageDialog("Warning", "Are you sure you want to delete this feild? It will effect all classes.", true))
                {
                    if (StudentColumn.Contains(item))
                    {
                        DataAccess.ExecuteQuery($"ALTER TABLE Students DROP COLUMN [{item.Text}]");
                        DataAccess.ExecuteQuery($"UPDATE ColumnInformation SET Custom_Columns = NULL , Search_Columns = NULL");
                        StudentColumn.Remove(item);
                    }
                    else if (ParentColumn.Contains(item))
                    {
                        ParentColumn.Remove(item);
                        DataAccess.ExecuteQuery($"ALTER TABLE Parents DROP COLUMN [{item.Text}]");
                        DataAccess.ExecuteQuery($"UPDATE _ColumnInformation SET Custom_Columns = NULL , Search_Columns = NULL WHERE Table_Name = 'Parents' ");
                    }
                }
            }
            catch(Exception ex)
            {
                Logger.Log(ex, true);
            }
            
        }

        private void UpdateStructure()
        {
            try
            {
                bool isValid = true;
                foreach (var item in CustomStudentColumns)
                {
                    if (!item.IsValid)
                        isValid = false;
                }
                foreach (var item in CustomParentColumns)
                {
                    if (!item.IsValid)
                        isValid = false;
                }
                if (!StudentID.IsValid)
                    isValid = false;
                if (!ParentID.IsValid)
                    isValid = false;

                if (!isValid)
                {
                    DialogManager.ShowValidationMessage();
                    return;
                }
                foreach (var item in CustomStudentColumns)
                {
                    DataAccess.ExecuteQuery($"ALTER TABLE Students ADD  [{item.Value}] varchar(100)");
                }
                foreach (var item in CustomParentColumns)
                {
                    DataAccess.ExecuteQuery($"ALTER TABLE Parents ADD  [{item.Value}] varchar(100)");
                }
                DataAccess.ExecuteQuery($"UPDATE _ColumnInformation SET Custom_Columns = '{StudentID.Value}' WHERE Table_Name = 'Student_ID_Name' ");
                DataAccess.ExecuteQuery($"UPDATE ColumnInformation SET Custom_Columns = NULL , Search_Columns = NULL");
                DataAccess.ExecuteQuery($"UPDATE _ColumnInformation SET Custom_Columns = '{ParentID.Value}' WHERE Table_Name = 'Parent_ID_Name' ");
                DataAccess.ExecuteQuery($"UPDATE _ColumnInformation SET Custom_Columns = NULL , Search_Columns = NULL WHERE Table_Name = 'Parents' ");
                CustomStudentColumns = new ObservableCollection<TextEntity>();
                CustomParentColumns = new ObservableCollection<TextEntity>();
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

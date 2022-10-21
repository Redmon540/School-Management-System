using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows.Input;

namespace School_Manager
{
    public class TeachersViewModel : BaseViewModel
    {
        #region Default Constructor

        public TeachersViewModel()
        {
            //initialize the properties
            ColumnNames = new ObservableCollection<Check>();
            SearchColumns = new ObservableCollection<Check>();
            _teacherIDName = DataAccess.GetTeacherID();

            //initialize commands
            ApplyFilterCommand = new RelayCommand(ApplyFilter);
            ViewCommand = new RelayCommand(ViewRecord);
            EditCommand = new RelayCommand(EditRecord);
            DeleteCommand = new RelayCommand(DeleteRecord);

            //load data
            LoadTeachersData();
        }

        #endregion

        #region Public Properties

        private string _teacherIDName;

        public DataTable GridData { get; set; } 

        public bool IsDataLoading { get; set; }

        public DataRowView SelectedItem { get; set; }

        public ObservableCollection<Check> ColumnNames { get; set; }

        public ObservableCollection<Check> SearchColumns { get; set; }

        public string SelectedColumn { get; set; }

        #endregion

        public ICommand ApplyFilterCommand { get; set; }

        public ICommand ViewCommand { get; set; }

        public ICommand EditCommand { get; set; }

        public ICommand DeleteCommand { get; set; }

        #region Commands

        #endregion

        #region Command Methods

        private void LoadTeachersData()
        {
            try
            {
                IsDataLoading = true;
                using (new WaitCursor())
                {
                    ColumnNames = new ObservableCollection<Check>();
                    List<string> teacherColumns = DataAccess.GetDataTable("SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Teachers'").AsEnumerable().Select(s => s[0].ToString()).Where(x => !x.Contains("_")).ToList();
                    List<string> customColumns = DataAccess.GetDataTable($"SELECT Custom_Columns FROM _ColumnInformation WHERE Table_Name = 'Teachers'").Rows[0][0].ToString().Split(',').ToList();

                    string query = $"[Teacher_ID] AS [{_teacherIDName}]";
                    if (customColumns.Count == 0 || (customColumns.Count == 1 && customColumns[0].IsNullOrEmpty()))
                    {
                        foreach (var item in teacherColumns)
                        {
                            if (item.ToLower().Contains("date"))
                            {
                                query += $",CONVERT(varchar(10) , [{item}] , 103) as [{item}]";
                            }
                            else
                            {
                                query += $",[{item}]";
                            }
                            ColumnNames.Add(new Check() { Content = item, IsChecked = true });
                        }
                        //to insert teacher id
                        ColumnNames.Insert(0, new Check() { Content = _teacherIDName, IsChecked = true });
                    }
                    else
                    {
                        foreach (var item in customColumns) 
                        {
                            if (item.ToLower().Contains("date"))
                            {
                                query += $",CONVERT(varchar(10) , [{item}] , 103) as [{item}]";
                            }
                            else if (item != _teacherIDName)
                            {
                                query += $",[{item}]";
                            }
                        }
                        ColumnNames = new ObservableCollection<Check>(teacherColumns.Select(i => new Check() { Content = i, IsChecked = customColumns.Contains(i) }));
                        //to insert teacher id
                        ColumnNames.Insert(0, new Check() { Content = _teacherIDName, IsChecked = true });
                    }

                    //to populate datagrid
                    GridData = DataAccess.GetDataTable($"SELECT {query} , Class AS [{DataAccess.GetTeacherClassIDName()}] FROM Teachers LEFT JOIN Classes ON Classes.Class_ID = Teachers.Class_ID WHERE Teachers.Is_Active = 1");

                    //to set search columns
                    List<string> searchColumns = DataAccess.GetDataTable($"SELECT Search_Columns FROM _ColumnInformation WHERE Table_Name = 'Teachers'").Rows[0][0].ToString().Split(',').ToList();
                    if (searchColumns.Count == 0 || (searchColumns.Count == 1 && searchColumns[0].IsNullOrEmpty()))
                    {
                        SearchColumns = new ObservableCollection<Check>(ColumnNames.Where(x => x.IsChecked && !x.Content.ToLower().Contains("photo")).Select(i => new Check() { Content = i.Content, IsChecked = true }));
                    }
                    else
                    {
                        SearchColumns = new ObservableCollection<Check>(ColumnNames.Where(x => x.IsChecked && !x.Content.ToLower().Contains("photo")).Select(i => new Check() { Content = i.Content, IsChecked = searchColumns.Contains(i.Content) }));
                    }
                }
                IsDataLoading = false;
            }
            catch(Exception ex)
            {
                Logger.Log(ex, true);
            }
            finally
            {
                IsDataLoading = false;
            }
        }

        private void ApplyFilter()
        {
            try
            {
                SelectedColumn = "";
                IsDataLoading = true;
                string query = $"[Teacher_ID] AS [{_teacherIDName}]";
                string customQuery = _teacherIDName;

                foreach (Check checkBox in ColumnNames)
                {//to get only date from server in dd/mm/yyyy formate
                    if (checkBox.Content.ToLower().Contains("date"))
                    {
                        if (checkBox.IsChecked)
                        {
                            query += $",CONVERT(varchar(10) , [{checkBox.Content}] , 103) as [{checkBox.Content}]";
                            customQuery += $",{checkBox.Content}";
                        }
                    }
                    else if (checkBox.IsChecked && checkBox.Content != _teacherIDName)
                    {
                        query += $",[{checkBox.Content}]";
                        customQuery += $",{checkBox.Content}";
                    }
                }
                //update the custom_columns
                DataAccess.ExecuteQuery($"UPDATE _ColumnInformation SET Custom_Columns = '{customQuery}' WHERE Table_Name = 'Teachers'");

                //To Populate Datagrid
                GridData = DataAccess.GetDataTable($"SELECT {query} , Class AS [{DataAccess.GetTeacherClassIDName()}] FROM Teachers LEFT JOIN Classes ON Classes.Class_ID = Teachers.Class_ID WHERE Teachers.Is_Active = 1");

                //to set search columns
                List<string> searchColumns = DataAccess.GetDataTable($"SELECT Search_Columns FROM _ColumnInformation WHERE Table_Name = 'Teachers'").Rows[0][0].ToString().Split(',').ToList();
                if (searchColumns.Count == 0 || (searchColumns.Count == 1 && searchColumns[0].IsNullOrEmpty()))
                {
                    SearchColumns = new ObservableCollection<Check>(ColumnNames.Where(x => x.IsChecked && !x.Content.ToLower().Contains("photo")).Select(i => new Check() { Content = i.Content, IsChecked = true }));
                }
                else
                {
                    SearchColumns = new ObservableCollection<Check>(ColumnNames.Where(x => x.IsChecked && !x.Content.ToLower().Contains("photo")).Select(i => new Check() { Content = i.Content, IsChecked = searchColumns.Contains(i.Content) }));
                }
            }
            catch(Exception ex)
            {
                Logger.Log(ex, true);
            }
            finally
            {
                IsDataLoading = false;
            }
        }

        private void ViewRecord()
        {
            try
            {
                if (Account.CanViewTeacher)
                {
                    DataRowView _SelectedItem = SelectedItem;
                    var TeacherID = _SelectedItem[_teacherIDName].ToString();
                    DialogManager.ViewTeacherRecord(TeacherID);
                }
                else
                {
                    DialogManager.ShowMessageDialog("Warning!", "You don't have permission for this action.",DialogTitleColor.Red);
                }
            }
            catch(Exception ex)
            {
                Logger.Log(ex, true);
            }
        }

        private void EditRecord()
        {
            try
            {
                if (Account.CanEditTeacher)
                {
                    DataRowView _SelectedItem = SelectedItem;
                    var TeacherID = _SelectedItem[_teacherIDName].ToString();
                    DialogManager.EditTeacherRecord(TeacherID);
                    LoadTeachersData();
                }
                else
                {
                    DialogManager.ShowMessageDialog("Warning!", "You don't have permission for this action.",DialogTitleColor.Red);
                }
            }
            catch(Exception ex)
            {
                Logger.Log(ex, true);
            }
        }

        private void DeleteRecord()
        {
            try
            {
                if (Account.CanDeleteTeacher)
                {
                    bool IsYesPressed = DialogManager.ShowMessageDialog("Warning", "Are you sure you want to delete this Teacher ? ", true);
                    if (IsYesPressed)
                    {
                        DataRowView _SelectedItem = SelectedItem;
                        var teacherID = _SelectedItem[_teacherIDName].ToString();
                        DataAccess.ExecuteQuery($"UPDATE Teachers SET Is_Active = 0 WHERE Teacher_ID = {teacherID}");
                        DialogManager.ShowMessageDialog("Message", "Record Deleted Successfully",DialogTitleColor.Green);
                        LoadTeachersData();
                    }
                }
                else
                {
                    DialogManager.ShowMessageDialog("Warning!", "You don't have permission for this action.",DialogTitleColor.Red);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex, true);
            }
        }

        #endregion
    }
}

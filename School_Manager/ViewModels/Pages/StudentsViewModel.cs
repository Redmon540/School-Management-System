using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;

namespace School_Manager
{
    public class StudentsViewModel : BaseViewModel
    {
        #region Default Constructor

        public StudentsViewModel()
        {
            //Initialize Commands
            ClassSelectionChanged = new RelayCommand(ChangeClass);
            ApplyFilterCommand = new RelayCommand(ApplyFilter);
            ChangeSectionCommand = new RelayCommand(ChangeSection);
            ViewCommand = new RelayCommand(ViewRecord);
            EditCommand = new RelayCommand(EditRecord);
            DeleteCommand = new RelayCommand(DeleteRecord);

            //initialize the classes
            ClassNames = DataAccess.GetClassNames();

            //initialize properties
            ColumnNames = new ObservableCollection<Check>();
            SearchColumns = new ObservableCollection<Check>();
            SectionNames = new List<string>();
            StudentID = DataAccess.GetStudentID();
        }

        #endregion

        #region Public Properties

        public string StudentID { get; set; }

        public List<string> ClassNames { get; set; }

        public string SelectedClass { get; set; } = null;

        public List<string> SectionNames { get; set; }

        public ObservableCollection<Check> ColumnNames { get; set; }

        public ObservableCollection<Check> SearchColumns { get; set; }

        public string SearchText { get; set; }

        public DataTable GridData { get; set; }

        public object SelectedItem { get; set; }

        public string SelectedSection { get; set; }

        public string SelectedColumn { get; set; }

        public bool IsDataLoading { get; set; }

        #endregion

        #region Public Commands

        public ICommand ApplyFilterCommand { get; set; }

        public ICommand ChangeSectionCommand { get; set; }

        public ICommand ClassSelectionChanged { get; set; }

        public ICommand ViewCommand { get; set; }

        public ICommand EditCommand { get; set; }

        public ICommand DeleteCommand { get; set; }

        #endregion

        #region Command Methods

        private void ChangeClass()
        {
            try
            {
                IsDataLoading = true;
                ColumnNames = new ObservableCollection<Check>();
                using (new WaitCursor())
                {
                    if (SelectedClass != null)
                    {
                        //get all columns list
                        List<string> studentColumns = DataAccess.GetDataTable("SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Students'").AsEnumerable().Select(s => s[0].ToString()).Where(x => !x.Contains("_")).ToList();
                        List<string> parentColumns = DataAccess.GetDataTable("SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Parents'").AsEnumerable().Select(s => s[0].ToString()).Where(x => !x.Contains("_")).ToList();
                        List<string> allColumns = new List<string>();
                        allColumns.AddRange(studentColumns);
                        allColumns.AddRange(parentColumns);

                        //custom columns
                        List<string> customColumns = DataAccess.GetDataTable($"SELECT Custom_Columns FROM ColumnInformation WHERE Class_ID = {DataAccess.GetClassID(SelectedClass)}").Rows[0][0].ToString().Split(',').ToList();

                        string query = $"[Student_ID] AS [{StudentID}]";
                        if (customColumns.Count == 0 || (customColumns.Count == 1 && customColumns[0].IsNullOrEmpty()))
                        {
                            foreach (var item in allColumns)
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
                            //add the student id
                            ColumnNames.Insert(0, new Check() { Content = StudentID, IsChecked = true });
                        }
                        else
                        {
                            foreach (var item in customColumns)
                            {
                                if (item.ToLower().Contains("date"))
                                {
                                    query += $",CONVERT(varchar(10) , [{item}] , 103) as [{item}]";
                                }
                                else if(item != StudentID)
                                {
                                    query += $",[{item}]";
                                }
                            }
                            ColumnNames = new ObservableCollection<Check>(allColumns.Select(i => new Check() { Content = i, IsChecked = customColumns.Contains(i) }));
                            //add the student id
                            ColumnNames.Insert(0, new Check() { Content = StudentID, IsChecked = true });
                        }

                        //DON'T populate here cause when the selected value of section is changed the change section method will fire 
                        //and it will be populating twice
                        ////To Populate Datagrid
                        //GridData = DataAccess.GetDataTable($"SELECT {query} FROM [Students] LEFT JOIN [Parents] " +
                        //    $"ON [Students].[Parent_ID] = [Parents].[Parent_ID]  WHERE [Students].[Class_ID] = " +
                        //    $"{DataAccess.GetClassID(SelectedClass)} AND Students.Is_Active = 1 " +
                        //    $"ORDER BY [Student_ID]");

                        //to set search columns
                        List<string> searchCol = DataAccess.GetDataTable($"SELECT Search_Columns FROM ColumnInformation WHERE Class_ID = {DataAccess.GetClassID(SelectedClass)}").Rows[0][0].ToString().Split(',').ToList();
                        if (searchCol.Count == 0 || (searchCol.Count == 1 && searchCol[0].ToString().IsNullOrEmpty()))
                        {
                            SearchColumns = new ObservableCollection<Check>(ColumnNames.Where(x => x.IsChecked && !x.Content.ToLower().Contains("photo")).Select(x => new Check() { Content = x.Content, IsChecked = true }).ToList());
                        }
                        else
                        {
                            SearchColumns = new ObservableCollection<Check>(ColumnNames.Where(x => x.IsChecked && !x.Content.ToLower().Contains("photo")).Select(x => new Check() { Content = x.Content, IsChecked = searchCol.Contains(x.Content) }).ToList());
                        }

                        //To populate Section ComboBox
                        SectionNames = DataAccess.GetDataTable($"SELECT Section FROM Sections WHERE Class_ID = {DataAccess.GetClassID(SelectedClass)}").AsEnumerable().Select(x => x[0].ToString()).ToList();
                        SectionNames.Insert(0, "All");
                        SelectedSection = "All";
                        CollectionViewSource.GetDefaultView(SectionNames).Refresh();
                    }
                }
            }
            catch (Exception ex)
            {
                IsDataLoading = false;
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
                //to clear the column names combobox selected item
                SelectedColumn = "";
                IsDataLoading = true;
                string customQuery = StudentID;
                string query = $"[Student_ID] AS [{StudentID}]";
                if (SelectedClass != null)
                {
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
                        else if (checkBox.IsChecked && checkBox.Content != StudentID)
                        {
                            query += $",[{checkBox.Content}]";
                            customQuery += $",{checkBox.Content}";
                        }
                    }
                    //update the custom_columns
                    DataAccess.ExecuteQuery($"UPDATE ColumnInformation SET Custom_Columns = '{customQuery}' WHERE Class_ID = {DataAccess.GetClassID(SelectedClass)}");

                    //To Populate Datagrid
                    if(SelectedSection != "All")
                        GridData = DataAccess.GetDataTable($"SELECT  {query} FROM [Students] LEFT JOIN [Parents] ON [Students].[Parent_ID] = [Parents].[Parent_ID] " +
                        $"JOIN Sections ON Sections.Section_ID = Students.Section_ID WHERE [Students].[Class_ID] = {DataAccess.GetClassID(SelectedClass)} " +
                        $"AND Students.Section_ID = {DataAccess.GetSectionID(SelectedSection, DataAccess.GetClassID(SelectedClass))} AND Students.Is_Active = 1 ORDER BY Students.[Student_ID]");
                    else
                        GridData = DataAccess.GetDataTable($"SELECT {query} FROM [Students] LEFT JOIN [Parents] " +
                            $"ON [Students].[Parent_ID] = [Parents].[Parent_ID]  " +
                            $"WHERE [Students].[Class_ID] = {DataAccess.GetClassID(SelectedClass)} " +
                            $"AND Students.Is_Active = 1 ORDER BY [Student_ID]");

                    //to set search columns
                    List<string> searchCol = DataAccess.GetDataTable($"SELECT Search_Columns FROM ColumnInformation " +
                        $"WHERE Class_ID = {DataAccess.GetClassID(SelectedClass)}").Rows[0][0].ToString().Split(',').ToList();
                    if (searchCol.Count == 0 || (searchCol.Count == 1 && searchCol[0].ToString().IsNullOrEmpty()))
                    {
                        SearchColumns = new ObservableCollection<Check>(ColumnNames.
                            Where(x => x.IsChecked && !x.Content.ToLower().Contains("photo")).
                            Select(x => new Check() { Content = x.Content, IsChecked = true }).ToList());
                    }
                    else
                    {
                        SearchColumns = new ObservableCollection<Check>(ColumnNames
                            .Where(x => x.IsChecked && !x.Content.ToLower().Contains("photo"))
                            .Select(x => new Check() { Content = x.Content, IsChecked = searchCol.Contains(x.Content) }).ToList());
                    }
                }
            }
            catch (Exception ex)
            {
                IsDataLoading = false;
                Logger.Log(ex, true);
            }
            finally
            {
                IsDataLoading = false;
            }

        }

        private void ChangeSection()
        {
            try
            {
                IsDataLoading = true;
                string query = $"[Student_ID] AS [{StudentID}]";
                if (SelectedClass.IsNotNullOrEmpty() && SelectedSection.IsNotNullOrEmpty())
                {
                    foreach (Check checkBox in ColumnNames)
                    {//to get only date from server in dd/mm/yyyy formate
                        if (checkBox.Content.ToLower().Contains("date"))
                        {
                            if (checkBox.IsChecked)
                            {
                                query += $",CONVERT(varchar(10) , [{checkBox.Content}] , 103) as [{checkBox.Content}]";
                            }
                        }
                        else if (checkBox.IsChecked && checkBox.Content != StudentID)
                        {
                            query += $",[{checkBox.Content}]";
                        }
                    }

                    if(SelectedSection != "All")
                    GridData = DataAccess.GetDataTable($"SELECT  {query} FROM [Students] LEFT JOIN [Parents] ON [Students].[Parent_ID] = [Parents].[Parent_ID] " +
                        $"JOIN Sections ON Sections.Section_ID = Students.Section_ID WHERE [Students].[Class_ID] = {DataAccess.GetClassID(SelectedClass)} " +
                        $"AND Students.Section_ID = {DataAccess.GetSectionID(SelectedSection, DataAccess.GetClassID(SelectedClass))} AND Students.Is_Active = 1 ORDER BY Students.[Student_ID]");
                    else
                        GridData = DataAccess.GetDataTable($"SELECT {query} FROM [Students] FULL JOIN [Parents] " +
                         $"ON [Students].[Parent_ID] = [Parents].[Parent_ID]  WHERE [Students].[Class_ID] = " +
                         $"{DataAccess.GetClassID(SelectedClass)} AND Students.Is_Active = 1 " +
                         $" ORDER BY [Student_ID]");


                    //to set search columns
                    List<string> searchCol = DataAccess.GetDataTable($"SELECT Search_Columns FROM ColumnInformation WHERE Class_ID = {DataAccess.GetClassID(SelectedClass)}").Rows[0][0].ToString().Split(',').ToList();
                    if (searchCol.Count == 0 || (searchCol.Count == 1 && searchCol[0].ToString().IsNullOrEmpty()))
                    {
                        SearchColumns = new ObservableCollection<Check>(ColumnNames.Where(x => x.IsChecked && !x.Content.ToLower().Contains("photo")).Select(x => new Check() { Content = x.Content, IsChecked = true }).ToList());
                    }
                    else
                    {
                        SearchColumns = new ObservableCollection<Check>(ColumnNames.Where(x => x.IsChecked && !x.Content.ToLower().Contains("photo")).Select(x => new Check() { Content = x.Content, IsChecked = searchCol.Contains(x.Content) }).ToList());
                    }
                }
            }
            catch (Exception ex)
            {
                IsDataLoading = false;
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
                if (Account.CanViewStudentRecord)
                {
                    DataRowView _SelectedItem = (DataRowView)SelectedItem;
                    var studentID = _SelectedItem[StudentID].ToString();
                    DialogManager.ViewStudentRecord(studentID, DataAccess.GetParentID(studentID), SelectedClass);
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

        private void EditRecord()
        {
            try
            {
                if (Account.CanEditStudentRecord)
                {
                    DataRowView _SelectedItem = (DataRowView)SelectedItem;
                    var studentID = _SelectedItem[StudentID].ToString();
                    DialogManager.EditStudentRecord(studentID, DataAccess.GetParentID(studentID), SelectedClass);
                    ChangeClass();
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

        private void DeleteRecord()
        {
            try
            {
                if (Account.CanDeleteStudentRecord)
                {
                    (bool IsYesPressed, bool IsChecked) = DialogManager.ShowDeleteStudentDialog("Warning", "Are you sure you want to delete this Student ? ", "Also delete the parents record.", true);
                    if (IsYesPressed)
                    {
                        DataRowView _SelectedItem = (DataRowView)SelectedItem;
                        var studentID = _SelectedItem[StudentID].ToString();
                        var parentID = DataAccess.GetParentID(studentID);
                        DataAccess.ExecuteQuery($"UPDATE [Students] SET Is_Active = 0 WHERE [Student_ID] = '{studentID}'");

                        if (IsChecked)
                        {
                            DataAccess.ExecuteQuery($"UPDATE [Parents] SET Is_Active = 0 WHERE [Parent_ID] = '{parentID}'");
                        }

                        DialogManager.ShowMessageDialog("Message", "Record Deleted Successfully",DialogTitleColor.Green);
                        ChangeClass();
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
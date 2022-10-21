using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows.Input;

namespace School_Manager
{
    public class ParentsViewModel : BaseViewModel
    {

        #region Default Constructor

        public ParentsViewModel()
        {
            //Initialize Commands
            ApplyFilterCommand = new RelayCommand(ApplyFilter);
            ViewCommand = new RelayCommand(ViewRecord);
            EditCommand = new RelayCommand(EditRecord);
            DeleteCommand = new RelayCommand(DeleteRecord);

            //set the properties
            ColumnNames = new ObservableCollection<Check>();
            SearchColumns = new ObservableCollection<Check>();

            _parentIDName = DataAccess.GetParentID();

            //set the data
            LoadParentsData(); 
        }

        #endregion

        #region Public Properties

        private string _parentIDName;

        public ObservableCollection<Check> ColumnNames { get; set; }

        public ObservableCollection<Check> SearchColumns { get; set; }

        public string SearchText { get; set; }

        public DataTable GridData { get; set; }

        public object SelectedItem { get; set; }

        public string SelectedColumn { get; set; }

        public bool IsDataLoading { get; set; }

        #endregion

        #region Public Commands

        public ICommand ApplyFilterCommand { get; set; }

        public ICommand ViewCommand { get; set; }

        public ICommand EditCommand { get; set; }

        public ICommand DeleteCommand { get; set; }

        #endregion

        #region Command Methods

        private void LoadParentsData()
        {
            try
            {
                IsDataLoading = true;
                using (new WaitCursor())
                {
                    ColumnNames = new ObservableCollection<Check>();

                    //get all columns list
                    List<string> parentColumns = DataAccess.GetDataTable("SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Parents'").AsEnumerable().Select(s => s[0].ToString()).Where(x => !x.Contains('_')).ToList();

                    //custom columns
                    List<string> customColumns = DataAccess.GetDataTable
                        ($"SELECT Custom_Columns FROM _ColumnInformation WHERE Table_Name = 'Parents'").
                        Rows[0][0].ToString().Split(',').ToList();

                    string query = $"[Parent_ID] As [{_parentIDName}]";
                    if (customColumns.Count == 0 || (customColumns.Count == 1 && customColumns[0].IsNullOrEmpty()))
                    {
                        foreach (var item in parentColumns)
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
                        //inser the parent id
                        ColumnNames.Insert(0, new Check() { Content = _parentIDName, IsChecked = true });
                    }
                    else
                    {
                        foreach (var item in customColumns)
                        {
                            if (item.ToLower().Contains("date"))
                            {
                                query += $",CONVERT(varchar(10) , [{item}] , 103) as [{item}]";
                            }
                            else if (item != _parentIDName)
                            {
                                query += $",[{item}]";
                            }
                        }
                        ColumnNames = new ObservableCollection<Check>(parentColumns.Select(i => new Check() { Content = i, IsChecked = customColumns.Contains(i) }));
                        //inser the parent id
                        ColumnNames.Insert(0, new Check() { Content = _parentIDName, IsChecked = true });
                    }

                    //To Populate Datagrid
                    GridData = DataAccess.GetDataTable($"SELECT {query} FROM [Parents] WHERE Parents.Is_Active = 1 ORDER BY [Parent_ID]");

                    //to set search columns
                    List<string> searchCol = DataAccess.GetDataTable($"SELECT Search_Columns FROM _ColumnInformation WHERE Table_Name = 'Parents'").Rows[0][0].ToString().Split(',').ToList();
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

        private void ApplyFilter()
        {
            try
            {
                IsDataLoading = true;
                SelectedColumn = "";
                string query = $"[Parent_ID] AS [{_parentIDName}]";
                string customQuery = _parentIDName;
                foreach (Check checkBox in ColumnNames)
                {
                    //to get only date from server in dd/mm/yyyy formate
                    if (checkBox.Content.ToLower().Contains("date"))
                    {
                        if (checkBox.IsChecked)
                        {
                            query += $",CONVERT(varchar(10) , [{checkBox.Content}] , 103) as [{checkBox.Content}]";
                            customQuery += $",{checkBox.Content}";
                        }
                    }
                    else if (checkBox.IsChecked && checkBox.Content != _parentIDName)
                    {
                        query += $",[{checkBox.Content}]";
                        customQuery += $",{checkBox.Content}";
                    }
                }
                //update the custom_columns
                DataAccess.ExecuteQuery($"UPDATE _ColumnInformation SET Custom_Columns = '{customQuery}' WHERE Table_Name = 'Parents'");

                //To Populate Datagrid
                GridData = DataAccess.GetDataTable($"SELECT {query} FROM [Parents] WHERE Parents.Is_Active = 1 ORDER BY [Parent_ID]");

                //to set search columns
                List<string> searchCol = DataAccess.GetDataTable($"SELECT Search_Columns FROM _ColumnInformation WHERE Table_Name = 'Parents'").Rows[0][0].ToString().Split(',').ToList();
                if (searchCol.Count == 0 || (searchCol.Count == 1 && searchCol[0].ToString().IsNullOrEmpty()))
                {
                    SearchColumns = new ObservableCollection<Check>(ColumnNames.Where(x=> x.IsChecked && !x.Content.ToLower().Contains("photo")).Select(x => new Check() { Content = x.Content, IsChecked = x.IsChecked }).ToList());
                }
                else
                {
                    SearchColumns = new ObservableCollection<Check>(ColumnNames.Where(x => x.IsChecked && !x.Content.ToLower().Contains("photo")).Select(x => new Check() { Content = x.Content, IsChecked = searchCol.Contains(x.Content) }).ToList());
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
                if (Account.CanViewParent)
                {
                    DataRowView _SelectedItem = (DataRowView)SelectedItem;
                    var parentID = _SelectedItem[_parentIDName].ToString();
                    DialogManager.ViewParentRecord(parentID);
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
                if (Account.CanEditParent)
                {
                    DataRowView _SelectedItem = (DataRowView)SelectedItem;
                    var parentID = _SelectedItem[_parentIDName].ToString();
                    DialogManager.EditParents(parentID);
                    LoadParentsData();
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
                if (Account.CanDeleteParent)
                {
                    bool IsYesPressed = DialogManager.ShowMessageDialog("Warning", "Are you sure you want to delete parents record ? ", true);
                    if (IsYesPressed)
                    {
                        DataRowView _SelectedItem = (DataRowView)SelectedItem;
                        var parentID = _SelectedItem[_parentIDName].ToString();
                        if (DataAccess.GetDataTable($"SELECT * FROM Students WHERE Is_Active = 1 AND Parent_ID = {parentID}").Rows.Count == 0)
                        {
                            DataAccess.ExecuteQuery($"UPDATE [Parents] SET Is_Active = 0 WHERE [Parent_ID] = '{parentID}'");
                            DialogManager.ShowMessageDialog("Message", "Record Deleted Successfully",DialogTitleColor.Green);
                            LoadParentsData();
                        }
                        else
                        {
                            DialogManager.ShowMessageDialog("Warning", "You cannot delete parents before deleting there children.",DialogTitleColor.Red);
                        }
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

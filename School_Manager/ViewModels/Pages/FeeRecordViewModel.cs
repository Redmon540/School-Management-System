using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows.Controls;
using System.Windows.Input;

namespace School_Manager
{
    public class FeeRecordViewModel : BaseViewModel
    {
        #region Default Constructor

        public FeeRecordViewModel()
        {
            //Initialize Commands
            ApplyFilterCommand = new RelayCommand(ApplyFilter);
            ClassSelectionChanged = new RelayCommand(ChangeClass);
            ViewCommand = new RelayCommand(ViewRecord);
            SectionChangedCommand = new RelayCommand(ChangeSectionCommand);

            //initialize dates
            SelectedMonth = DateTime.Now.ToString("MMMM");
            SelectedYear = DateTime.Now.Year.ToString();
            ClassNames = new ObservableCollection<string>(DataAccess.GetClassNames());

        }

        #endregion

        #region Public Properties

        public ObservableCollection<string> ClassNames { get; set; }

        public string SelectedClass { get; set; } = null;

        public ObservableCollection<string> SectionNames { get; set; }

        public string SelectedSection { get; set; }

        public ObservableCollection<CheckBox> ColumnNames { get; set; }

        public ObservableCollection<Check> SearchColumns { get; set; } = new ObservableCollection<Check> { new Check { Content = "Name", IsChecked = true } };

        public ObservableCollection<string> Months { get; set; } = new ObservableCollection<string>(Helper.GetMonths());

        public ObservableCollection<string> Years { get; set; } = new ObservableCollection<string>(Helper.GetYears());

        public string SelectedMonth { get; set; }

        public string SelectedYear { get; set; }

        public string SearchText { get; set; }

        public DataTable GridData { get; set; }

        public object SelectedItem { get; set; }

        public bool IsDataLoading { get; set; } = false;

        public bool IsNoRecordVisible { get; set; } = false;

        #endregion

        #region Public Commands

        public ICommand ApplyFilterCommand { get; set; }

        public ICommand ClassSelectionChanged { get; set; }

        public ICommand SectionChangedCommand { get; set; }

        public ICommand ViewCommand { get; set; }

        #endregion

        #region Command Methods

        private void ApplyFilter()
        {
            string columnNames = "";
            if (SelectedClass != null)
            {
                foreach (CheckBox checkBox in ColumnNames)
                {
                    if (checkBox.IsChecked == true)
                    {
                        columnNames += $",[{checkBox.Content.ToString()}]";
                    }
                }

            }
        }

        private void ChangeClass()
        {
            if (SelectedClass != null)
            {
                GridData = null;
                var classID = DataAccess.GetClassID(SelectedClass);
                //if (DataAccess.GetDataTable($"SELECT * FROM [Created_Fee_Records] WHERE Class_ID = {classID} AND Month = {Helper.GetMonthNumber(SelectedMonth)} AND Year = {SelectedYear}").Rows.Count != 0)
                {
                    IsDataLoading = true;
                    GridData = DataAccess.GetFeeRecord(classID, Helper.GetMonthNumber(SelectedMonth).ToString() , SelectedYear, null);
                    SectionNames = new ObservableCollection<string>(DataAccess.GetSectionsNames(classID));
                    SectionNames.Insert(0, "All");
                    SelectedSection = "All";
                    IsDataLoading = false;
                    IsNoRecordVisible = false;
                }
                //else
                //{
                //    IsNoRecordVisible = true;
                //}
            }
        }

        private void ChangeSectionCommand()
        {
            var classID = DataAccess.GetClassID(SelectedClass);
            //if (DataAccess.GetDataTable($"SELECT * FROM [Created_Fee_Records] WHERE Class_ID = {classID} AND Month = {Helper.GetMonthNumber(SelectedMonth)} AND Year = {SelectedYear}").Rows.Count != 0)
            {
                GridData = null;
                IsDataLoading = true;
                if (SelectedSection.IsNullOrEmpty() || SelectedSection == "All")
                    GridData = DataAccess.GetFeeRecord(classID, Helper.GetMonthNumber(SelectedMonth).ToString(), SelectedYear, null);
                else
                    GridData = DataAccess.GetFeeRecord(classID, Helper.GetMonthNumber(SelectedMonth).ToString(), SelectedYear,  DataAccess.GetSectionID(SelectedSection, classID));
                IsDataLoading = false;
                IsNoRecordVisible = false;
            }
            //else
            //{
            //    IsNoRecordVisible = true;
            //}
        }

        private void ViewRecord()
        {
            try
            {
                DataRowView _SelectedItem = (DataRowView)SelectedItem;
                var studentID = _SelectedItem[DataAccess.GetStudentID()].ToString();
                DialogManager.ViewFeeDetails(studentID, SelectedClass);
                ChangeClass();
            }
            catch (Exception ex)
            {
                Logger.Log(ex, true);
            }
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows.Input;

namespace School_Manager
{
    public class StudentAttendenceViewModel : BaseViewModel
    {
        #region Constructor

        public StudentAttendenceViewModel()
        {
            //set commands
            GetAttendenceRecordCommand = new RelayCommand(GetAttendenceRecord);
        }

        #endregion

        #region Properties

        public List<string> Classes { get; set; } = DataAccess.GetClassNames();

        public string SelectedClass { get; set; }

        public List<string> Years { get; set; } = Helper.GetYears();

        public string SelectedYear { get; set; } = DateTime.Now.Year.ToString();

        public List<string> Months { get; set; } = Helper.GetMonths();

        public string SelectedMonth { get; set; } = Helper.GetMonthName(DateTime.Now.Month);

        public ObservableCollection<Check> SearchColumns { get; set; } = new ObservableCollection<Check> { new Check { Content = "Name", IsChecked = true } };

        public bool IsDataLoading { get; set; }

        public DataTable  GridData { get; set; }

        #endregion

        #region Commands

        public ICommand GetAttendenceRecordCommand { get; set; }
                        
        #endregion

        #region Command Methods

        private void GetAttendenceRecord()
        {
            try
            {
                if (SelectedClass.IsNullOrEmpty())
                    return;
                if (SelectedMonth == "All")
                {
                    GridData = DataAccess.GetDataTable($"SELECT Students.[Student_ID] as [{DataAccess.GetStudentID()}]  , Name " +
                        $",[1],[2],[3],[4],[5],[6],[7],[8],[9],[10]" +
                        $",[11],[12],[13],[14],[15],[16],[17],[18],[19],[20]" +
                        $",[21],[22],[23],[24],[25],[26],[27],[28],[29],[30],[31]" +
                        $"From Student_Attendence LEFT JOIN " +
                        $"Students ON Student_Attendence.Student_ID = Students.Student_ID " +
                        $"LEFT JOIN Classes ON Classes.Class_ID = Students.Class_ID " +
                        $"WHERE Year = {SelectedYear} " +
                        $"AND Students.Class_ID = { DataAccess.GetClassID(SelectedClass)}");
                }
                else
                {
                    GridData = DataAccess.GetDataTable($"SELECT Students.[Student_ID] as [{DataAccess.GetStudentID()}]  , Name " +
                        $",[1],[2],[3],[4],[5],[6],[7],[8],[9],[10]" +
                        $",[11],[12],[13],[14],[15],[16],[17],[18],[19],[20]" +
                        $",[21],[22],[23],[24],[25],[26],[27],[28],[29],[30],[31]" +
                        $"From Student_Attendence LEFT JOIN " +
                        $"Students ON Student_Attendence.Student_ID = Students.Student_ID " +
                        $"LEFT JOIN Classes ON Classes.Class_ID = Students.Class_ID " +
                        $"WHERE Year = {SelectedYear} " +
                        $"AND Month = {Helper.GetMonthNumber(SelectedMonth)} " +
                        $"AND Students.Class_ID = { DataAccess.GetClassID(SelectedClass)}");
                }
            }
            catch(Exception ex)
            {
                Logger.Log(ex, true);
            }
        }

        #endregion
    }
}

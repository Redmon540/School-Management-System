using System;
using System.Data;
using System.Globalization;
using System.Windows.Forms;
using System.Windows.Input;

namespace School_Manager
{
    public class TeacherSalaryViewModel : BaseViewModel
    {
        #region Constructor

        public TeacherSalaryViewModel()
        {
            var month = DateTime.Now.Month;
            string previousMonth = "December";
            if(month != 1)
            {
                previousMonth = Helper.GetMonthName(month - 1);
            }
            SalarySheetViewModel = new SalarySheetViewModel()
            {
                
                Heading = $"Salary Sheet For the Month of {previousMonth} Pay in {Helper.GetMonthName(month)}.",
            };
            CreateSalarySheetCommand = new RelayCommand(CreateSalarySheet);
            GetSalaryRecordCommand = new RelayCommand(GetData);
        }

        #endregion

        #region Properties

        public SalarySheetViewModel SalarySheetViewModel { get; set; }

        public ListEntity Years { get; set; } = new ListEntity { Items = Helper.GetYears(), FeildName = "Years", ValidationType = ValidationType.NotEmpty, Value = DateTime.Now.Year.ToString() };

        public ListEntity Months { get; set; } = new ListEntity { Items = Helper.GetMonths(), FeildName = "Months", ValidationType = ValidationType.NotEmpty, Value = Helper.GetMonthName(DateTime.Now.Month) };

        #endregion

        #region Commands

        public ICommand CreateSalarySheetCommand { get; set; }

        public ICommand GetSalaryRecordCommand { get; set; }

        #endregion

        #region Command Methods

        private void GetData()
        {
            try
            {
                if (!Years.IsValid || !Months.IsValid)
                    return;
                DataTable teachers = DataAccess.GetDataTable($"SELECT [Teacher_ID] AS [{DataAccess.GetTeacherID()}], [Name] , [Rank] , CONVERT(varchar(10) , [Date of Join] , 103) AS [Date of Join] , CONVERT(varchar(10), [Salary]) as [Salary] ," +
                    $" '' as [Presents] , '' as [Absents] , '' as [Leaves] , '' as [Deduction] , '' [Net Pay] ," +
                    $" '' as [Signature] FROM Teachers WHERE Is_Active = 1");
                DataTable table = DataAccess.GetDataTable($"SELECT * FROM Teacher_Attendence WHERE Month = {Helper.GetMonthNumber(Months.Value)} AND Year = {Years.Value}");
                int presents = 0;
                int absents = 0;
                int leaves = 0;
                int rowsCount = 0;
                foreach (DataRow row in table.Rows)
                {
                    presents = 0;
                    absents = 0;
                    leaves = 0;

                    for (int i = 1; i < DateTime.DaysInMonth(int.Parse(Years.Value), Helper.GetMonthNumber(Months.Value)); i++)
                    {
                        if (row[$"{i}"] != null)
                        {
                            if (row[$"{i}"].ToString() == "A")
                                absents++;
                            else if (row[$"{i}"].ToString() == "P")
                                presents++;
                            else if (row[$"{i}"].ToString() == "L")
                                leaves++;
                        }
                    }
                    teachers.Rows[rowsCount]["Presents"] = presents;
                    teachers.Rows[rowsCount]["Absents"] = absents;
                    teachers.Rows[rowsCount]["Leaves"] = leaves;
                    var a = teachers.Rows[rowsCount]["Salary"] == DBNull.Value || teachers.Rows[rowsCount]["Salary"] == null ? 0 : teachers.Rows[rowsCount]["Salary"];
                    var salary = Convert.ToInt32(teachers.Rows[rowsCount]["Salary"] == DBNull.Value || teachers.Rows[rowsCount]["Salary"] == null ? 0 : teachers.Rows[rowsCount]["Salary"]);
                    double deduction = ((double)salary / (presents + absents + leaves)) * absents;
                    double netPay = salary - deduction;
                    teachers.Rows[rowsCount]["Deduction"] = deduction.ToString("C0", CultureInfo.CreateSpecificCulture("ur-PK"));
                    teachers.Rows[rowsCount]["Net Pay"] = netPay.ToString("C0", CultureInfo.CreateSpecificCulture("ur-PK"));
                    teachers.Rows[rowsCount]["Salary"] = salary.ToString("C0", CultureInfo.CreateSpecificCulture("ur-PK"));
                    rowsCount++;
                }
                if (table.Rows.Count == 0)
                {
                    foreach(DataRow row in teachers.Rows)
                    {
                        double salary;
                        var success = double.TryParse(row["Salary"].ToString(),out salary);
                        if(success)
                        {
                            row["Salary"] = salary.ToString("C0", CultureInfo.CreateSpecificCulture("ur-PK"));
                        }
                    }
                }
                    SalarySheetViewModel.ItemSource = teachers;
            }
            catch (Exception ex)
            {
                Logger.Log(ex, true);
            }
        }

        private void CreateSalarySheet()
        {
            try
            {
                var saveFileDialog = new SaveFileDialog();
                saveFileDialog.FileName = $"Teacher's Salary Sheet - {DateTime.Now.Date.ToShortDateString()}.xps";
                saveFileDialog.Filter = "XPS File (*.xps)|*.xps|All files (*.*)|*.*";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    DataAccess.CreateSalarySheet(new SalarySheet
                {
                    DataContext = new SalarySheetViewModel
                    {
                        Heading = SalarySheetViewModel.Heading,
                        ItemSource = SalarySheetViewModel.ItemSource,
                        IsEditButtonVisible = false,
                        CanEditHeading = false
                    }
                }, saveFileDialog.FileName);
            }
            catch(Exception ex)
            {
                Logger.Log(ex, true);
            }
            
        }

        #endregion
    }
}

using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;

namespace School_Manager
{
    public class DashboardViewModel : BaseViewModel
    {
        #region Constructor

        public DashboardViewModel()
        {
            StudentsCount = DataAccess.GetDataTable("SELECT COUNT(Student_ID) FROM Students WHERE Is_Active = 1").Rows[0][0].ToString();
            ParentsCount = DataAccess.GetDataTable("SELECT COUNT(Parent_ID) FROM Parents WHERE Is_Active = 1").Rows[0][0].ToString();
            TeachersCount = DataAccess.GetDataTable("SELECT COUNT(Teacher_ID) FROM Teachers WHERE Is_Active = 1").Rows[0][0].ToString();

            XFormatter = value => value.ToString();
            YFormatter = value => value.ToString("C0", CultureInfo.CreateSpecificCulture("ur-PK"));

            DueLabel = value => value.Y.ToString("C0", CultureInfo.CreateSpecificCulture("ur-PK"));

            SelectedDateChanged = new RelayCommand(DateChanged);
            SelectedDate = DateTime.Now;
            DateChanged();
        }

        #endregion

        #region Properties

        public SeriesCollection FeeCollection { get; set; }

        public SeriesCollection CollectionStatus { get; set; }

        public SeriesCollection MonthlyExpenses { get; set; }

        public SeriesCollection TotalExpenses { get; set; }

        public SeriesCollection StudentAttendence { get; set; }

        public SeriesCollection TeacherAttendence { get; set; }

        public Func<double, string> XFormatter { get; set; }

        public Func<double, string> YFormatter { get; set; }

        public Func<ChartPoint, string> DueLabel { get; set; }

        public string DueFees { get; set; }

        public string CollectedFees { get; set; }

        public string StudentPresent { get; set; }

        public string StudentAbsent { get; set; }

        public string StudentLeave { get; set; }

        public string TeacherPresent { get; set; }

        public string TeacherAbsent { get; set; }

        public string TeacherLeave { get; set; }

        public DateTime SelectedDate { get; set; }

        public string StudentsCount { get; set; }

        public string ParentsCount { get; set; }

        public string TeachersCount { get; set; }

        #endregion

        #region Commands

        public ICommand SelectedDateChanged { get; set; }

        #endregion

        #region Methods

        private void SetStudentAttendene(int day, int month, int year)
        {
            DataTable table = DataAccess.GetDataTable($"SELECT SUM (CASE WHEN [{day}] = 'P' THEN 1 ELSE 0 END) AS Present, " +
                $"SUM (CASE WHEN [{day}] = 'A' THEN 1 ELSE 0 END) AS [Absent], " +
                $"SUM (CASE WHEN [{day}] = 'L' THEN 1 ELSE 0 END) AS [Leave] " +
                $"FROM Student_Attendence WHERE Month = {month} AND Year = {year}");

            if (table.Rows.Count == 0)
            {
                StudentAttendence = null;
                StudentPresent = "";
                StudentAbsent = "";
                StudentLeave = "";
                return;
            }

            StudentAttendence = new SeriesCollection
            {
                new PieSeries
                {
                    Title = "Present",
                    Values = new ChartValues<int>{ table.Rows[0]["Present"].ToString().IsNullOrEmpty() ? 0 : (int)table.Rows[0]["Present"]  },
                    Fill = App.Current.MainWindow.FindResource("DarkGreenColorBrush") as Brush,
                },
                new PieSeries
                {
                    Title = "Absent",
                    Values = new ChartValues<int>{ table.Rows[0]["Absent"].ToString().IsNullOrEmpty() ? 0 : (int)table.Rows[0]["Absent"]  },
                    Fill = App.Current.MainWindow.FindResource("RedColorBrush") as Brush,
                },
                new PieSeries
                {
                    Title = "Leave",
                    Values = new ChartValues<int>{ table.Rows[0]["Leave"].ToString().IsNullOrEmpty() ? 0 : (int)table.Rows[0]["Leave"] },
                    Fill = App.Current.MainWindow.FindResource("DarkYellowColorBrush") as Brush,
                }
            };

            double present = table.Rows[0]["Present"].ToString().IsNullOrEmpty() ? 0 : (int)table.Rows[0]["Present"];
            double absent = table.Rows[0]["Absent"].ToString().IsNullOrEmpty() ? 0 : (int)table.Rows[0]["Absent"];
            double leave = table.Rows[0]["Leave"].ToString().IsNullOrEmpty() ? 0 : (int)table.Rows[0]["Leave"];

            double total = present + absent + leave;

            StudentPresent = $"Present {Environment.NewLine}{present} ({present / total:P})";
            StudentAbsent = $"Absent {Environment.NewLine}{absent} ({absent / total:P})";
            StudentLeave = $"Leave {Environment.NewLine}{leave} ({leave / total:P})";
        }

        private void SetTeacherAttendene(int day,  int month, int year)
        {
            DataTable table = DataAccess.GetDataTable($"SELECT SUM (CASE WHEN [{day}] = 'P' THEN 1 ELSE 0 END) AS Present, " +
                $"SUM (CASE WHEN [{day}] = 'A' THEN 1 ELSE 0 END) AS [Absent], " +
                $"SUM (CASE WHEN [{day}] = 'L' THEN 1 ELSE 0 END) AS [Leave] " +
                $"FROM Teacher_Attendence WHERE Month = {month} AND Year = {year}");

            if (table.Rows.Count == 0)
            {
                TeacherAttendence = null;
                TeacherPresent = "";
                TeacherAbsent = "";
                TeacherLeave = "";
                return;
            }

            TeacherAttendence = new SeriesCollection
            {
                new PieSeries
                {
                    Title = "Present",
                    Values = new ChartValues<int>{ table.Rows[0]["Present"].ToString().IsNullOrEmpty() ? 0 : (int)table.Rows[0]["Present"] },
                    Fill = App.Current.MainWindow.FindResource("DarkGreenColorBrush") as Brush
                },
                new PieSeries
                {
                    Title = "Absent",
                    Values = new ChartValues<int>{ table.Rows[0]["Absent"].ToString().IsNullOrEmpty() ? 0 : (int)table.Rows[0]["Absent"]  },
                    Fill = App.Current.MainWindow.FindResource("RedColorBrush") as Brush
                },
                new PieSeries
                {
                    Title = "Leave",
                    Values = new ChartValues<int>{ table.Rows[0]["Leave"].ToString().IsNullOrEmpty() ? 0 : (int)table.Rows[0]["Leave"] },
                    Fill = App.Current.MainWindow.FindResource("DarkYellowColorBrush") as Brush
                }
            };
            double present = table.Rows[0]["Present"].ToString().IsNullOrEmpty() ? 0 : (int)table.Rows[0]["Present"];
            double absent = table.Rows[0]["Absent"].ToString().IsNullOrEmpty() ? 0 : (int)table.Rows[0]["Absent"];
            double leave = table.Rows[0]["Leave"].ToString().IsNullOrEmpty() ? 0 : (int)table.Rows[0]["Leave"];

            double total = present + absent + leave;

            TeacherPresent = $"Present {Environment.NewLine}{present} ({present / total:P})";
            TeacherAbsent = $"Absent {Environment.NewLine}{absent} ({absent  / total:P})";
            TeacherLeave = $"Leave {Environment.NewLine}{leave} ({leave / total:P})";
        }

        private void SetCollectionStatus(int month, int year)
        {
            var table = DataAccess.GetCollectedStatus(month , year);
            if (table.Rows.Count == 0)
            {
                CollectedFees = null;
                CollectedFees = "";
                DueFees = "";
                return;
            }

            CollectionStatus = new SeriesCollection
            {
                new PieSeries
                {
                    Title = "Collected Fees",
                    Values = new ChartValues<int>{  table.Rows[0]["Collection"].ToString().IsNullOrEmpty() ? 0 : (int)table.Rows[0]["Collection"] },
                    Fill = App.Current.MainWindow.FindResource("PrimaryVeryLightColorBrush") as Brush,
                },
                new PieSeries
                {
                    Title = "Due Fees",
                    Values = new ChartValues<int>{ table.Rows[0]["Dues"].ToString().IsNullOrEmpty() ? 0 : (int) table.Rows[0]["Dues"] },
                    Fill = App.Current.MainWindow.FindResource("RedColorBrush") as Brush,
                }
            };

            double collectedFee = table.Rows[0]["Collection"].ToString().IsNullOrEmpty() ? 0 : (int)table.Rows[0]["Collection"];
            double dueFee = table.Rows[0]["Dues"].ToString().IsNullOrEmpty() ? 0 : (int)table.Rows[0]["Dues"];
            double total = collectedFee + dueFee;

            CollectedFees = $"Collected Fees {Environment.NewLine}{collectedFee.ToString("C0", CultureInfo.CreateSpecificCulture("ur-PK"))} ({collectedFee / total:P})";
            DueFees = $"Due Fees {Environment.NewLine}{dueFee.ToString("C0", CultureInfo.CreateSpecificCulture("ur-PK"))} ({dueFee / total:P})";

        }

        private void SetFeeCollection(int month ,int year)
        {
            var table = DataAccess.GetFeeCollections(month , year);
            if (table.Rows.Count == 0)
            {
                FeeCollection = null;
                return;
            }
            var points = table.AsEnumerable().Select(x => new ObservablePoint(((DateTime)x["Received Date"]).Day, (int)x["Collection"])).ToList();
            var chartpoints = new ChartValues<ObservablePoint>(points);
            FeeCollection = new SeriesCollection
            {
                new StackedColumnSeries
                {
                    Title = "Amount",
                    Values = chartpoints,
                    LabelPoint = label => $"{label.Y.ToString("C0", CultureInfo.CreateSpecificCulture("ur-PK"))}",
                    LabelsPosition = BarLabelPosition.Parallel
                }
            };
        }

        private void SetMonthlyExpense(int month, int year)
        {
            var table = DataAccess.GetMonthlyExpenses(month.ToString(), year.ToString());
            if (table.Rows.Count == 0)
            {
                MonthlyExpenses = null;
                return;
            }

            var points = table.AsEnumerable().Select(x => new ObservablePoint(((DateTime)x["Date"]).Day, (int)x["Amount"])).ToList();
            var chartpoints = new ChartValues<ObservablePoint>(points);
            MonthlyExpenses = new SeriesCollection
            {
                new StackedColumnSeries
                {
                    Title = "Amount",
                    Values = chartpoints,
                    LabelPoint = label => $"{label.Y.ToString("C0", CultureInfo.CreateSpecificCulture("ur-PK"))}"
                }
            };
        }

        private void SetTotalEpenses(int month, int year)
        {
            var table = DataAccess.GetTotalExpense(month.ToString() , year.ToString());
            if (table.Rows.Count == 0)
            {
                TotalExpenses = null;
                return;
            }

            TotalExpenses = new SeriesCollection();

            foreach (DataRow item in table.Rows)
            {
                TotalExpenses.Add(new PieSeries
                {
                    Title = item["Expense"].ToString(),
                    Values = new ChartValues<ObservableValue> {
                        table.Rows[0]["Amount"].ToString().IsNullOrEmpty() ? new ObservableValue(0) : 
                        new ObservableValue((int)item["Amount"]) },
                    LabelPoint = label => $"{label.Y.ToString("C0", CultureInfo.CreateSpecificCulture("ur-PK"))}"
                });
            }

        }

        private void DateChanged()
        {
            var day = SelectedDate.Day;
            var month = SelectedDate.Month;
            var year = SelectedDate.Year;

            SetStudentAttendene(day,month,year);
            SetTeacherAttendene(day, month, year);
            SetCollectionStatus(month, year);
            SetFeeCollection(month, year);
            SetMonthlyExpense(month, year);
            SetTotalEpenses(month, year);
        }

        #endregion

    }
}

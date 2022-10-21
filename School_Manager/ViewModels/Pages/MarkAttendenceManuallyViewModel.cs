using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace School_Manager
{
    public class MarkAttendenceManuallyViewModel : BaseViewModel
    {
        #region Constructor

        public MarkAttendenceManuallyViewModel()
        {
            MarkAttendenceCommand = new RelayParameterizedCommand(parameter => MarkAttendence(parameter));
        }

        #endregion

        #region Private Members

        private DateTime _SelectedDate = DateTime.Now;

        string year = DateTime.Now.Year.ToString();
        string month = DateTime.Now.Month.ToString();
        string day = DateTime.Now.Day.ToString();

        #endregion

        #region Properties

        public bool IsAttendenceRunning { get; set; } = false;

        public bool IsAttendenceMarked { get; set; }

        public ListEntity AttendenceOptions { get; set; } = new ListEntity
        {
            FeildName = "Category",
            Value = "Students",
            Items = new List<string> { "Students", "Teachers" },
            ValidationType = ValidationType.NotEmpty
        };

        public DateTime SelectedDate
        {
            get => _SelectedDate;
            set
            {
                if (_SelectedDate != value)
                {
                    _SelectedDate = value;
                    year = _SelectedDate.Year.ToString();
                    month = _SelectedDate.Month.ToString();
                    day = _SelectedDate.Day.ToString();
                }
            }
        }

        public TextEntity UserID { get; set; } = new TextEntity { FeildName = "Enter ID", ValidationType = ValidationType.NumericWithNonEmpty };

        public string IDName { get; set; }

        public string ID { get; set; }

        public BitmapImage Photo { get; set; }

        public string Name { get; set; }

        public string Class { get; set; }

        public bool IsClassVisible { get; set; }

        public string DateAndTime { get; set; }

        public string AlertMessage { get; set; }

        public string CurrentUser { get; set; }

        public bool ShowUserInfo { get; set; } = false;

        public int AttendenceCount { get; set; } = 0;

        #endregion

        #region Commands

        public ICommand IDChangedCommand { get; set; }

        public ICommand MarkAttendenceCommand { get; set; }

        #endregion

        #region Command Methods

        private void StartAttendence()
        {
            try
            {
                if (AttendenceOptions.Value == "Students")
                {
                    //for student attendence
                    if (DataAccess.GetDataTable($"SELECT [{day}] FROM Student_Attendence WHERE Month = {month} AND Year = {year}").Rows.Count == 0)
                    {
                        //to insert monthly record
                        DataAccess.ExecuteQuery($"INSERT INTO Student_Attendence (Month,Year,Student_ID) " +
                            $"SELECT '{month}' , '{year}' , Students.Student_ID FROM Students  WHERE Students.Is_Active = 1");
                        //to insert abscent in current day
                        DataAccess.ExecuteQuery($"UPDATE Student_Attendence SET [{day}] = 'A' WHERE Year = '{year}' AND Month = '{month}'");
                    }
                    else
                    {
                        //to insert abscent in current day
                        if (DataAccess.GetDataTable($"SELECT [{day}] FROM Student_Attendence WHERE Year = '{year}' AND Month = '{month}' AND [{day}] is null").Rows.Count != 0)
                        {
                            //to insert abscent in current day
                            DataAccess.ExecuteQuery($"UPDATE Student_Attendence SET [{day}] = 'A' WHERE Year = '{year}' AND Month = '{month}'");
                        }
                    }
                }
                else if (AttendenceOptions.Value == "Teachers")
                {
                    //for teacher attendence
                    if (DataAccess.GetDataTable($"SELECT [{day}] FROM Teacher_Attendence WHERE Month = {month} AND Year = {year}").Rows.Count == 0)
                    {
                        //to insert monthly record
                        DataAccess.ExecuteQuery($"INSERT INTO Teacher_Attendence (Month,Year,Teacher_ID) " +
                            $"SELECT '{month}' , '{year}' , Teachers.Teacher_ID FROM Teachers  WHERE Teachers.Is_Active = 1");
                        //to insert abscent in current day
                        DataAccess.ExecuteQuery($"UPDATE Teacher_Attendence SET [{day}] = 'A' WHERE Year = '{year}' AND Month = '{month}'");
                    }
                    else
                    {
                        //to insert abscent in current day
                        if (DataAccess.GetDataTable($"SELECT [{day}] FROM Teacher_Attendence WHERE Year = '{year}' AND Month = '{month}' AND [{day}] is null").Rows.Count != 0)
                        {
                            //to insert abscent in current day
                            DataAccess.ExecuteQuery($"UPDATE Teacher_Attendence SET [{day}] = 'A' WHERE Year = '{year}' AND Month = '{month}'");
                        }
                    }
                }
                IsAttendenceRunning = true;
            }
            catch (Exception ex)
            {
                Logger.Log(ex, true);
            }
        }

        private void MarkAttendence(object sender)
        {
            string attendence = sender as string;
            if (attendence == "Mark Present")
                attendence = "P";
            else if (attendence == "Mark Abscent")
                attendence = "A";
            else if (attendence == "Mark Leave")
                attendence = "L";
            try
            {
                if (UserID.Value.IsNullOrEmpty())
                {
                    DialogManager.ShowValidationMessage();
                    return;
                }

                StartAttendence();

                DataTable table = new DataTable();

                if (AttendenceOptions.Value == "Students")
                {
                    table = DataAccess.GetDataTable($"SELECT [Student_ID] , [Name] , [Photo] , [Class] , [Section] FROM Students" +
                    $" LEFT JOIN Classes ON Classes.Class_ID = Students.Class_ID" +
                    $" LEFT JOIN Sections ON Sections.Section_ID = Students.Section_ID" +
                    $" WHERE Students.Student_ID = {UserID.Value} AND Students.Is_Active = 1");
                     IDName = DataAccess.GetStudentID();
                    if (table.Rows.Count == 0)
                    {
                        DialogManager.ShowMessageDialog("Warning", $"{IDName} not exits.Please enter the correct {IDName}.",DialogTitleColor.Red);
                        return;
                    }
                     DataAccess.ExecuteQuery($"UPDATE Student_Attendence SET [{day}] = '{attendence}' WHERE Year = '{year}' AND Month = '{month}' AND Student_ID = {UserID.Value}");
                     CurrentUser = "Student";
                }
                else if (AttendenceOptions.Value == "Teachers")
                {
                    IDName = DataAccess.GetTeacherID();
                    table = DataAccess.GetDataTable($"SELECT [Teacher_ID] , [Name] , [Photo] FROM Teachers" +
                    $" WHERE Teachers.Teacher_ID = {UserID.Value}");
                    if (table.Rows.Count == 0)
                    {
                        DialogManager.ShowMessageDialog("Warning", $"{IDName} not exits.Please enter the correct {IDName}.",DialogTitleColor.Red);
                        return;
                    }
                    DataAccess.ExecuteQuery($"UPDATE Teacher_Attendence SET [{day}] = '{attendence}' WHERE Year = '{year}' AND Month = '{month}' AND [Teacher_ID] = {UserID.Value}");
                    CurrentUser = "Teacher";
                }


                ID = UserID.Value.ToString();
                Name = table.Rows[0]["Name"].ToString();
                Photo = Helper.ByteArrayToImage(table.Rows[0]["Photo"] as byte[]);
                if (AttendenceOptions.Value != "Teachers")
                {
                    if (table.Rows[0]["Section"].ToString().IsNullOrEmpty())
                    {
                        Class = table.Rows[0]["Class"].ToString();
                    }
                    else
                    {
                        Class = $"{table.Rows[0]["Class"].ToString()} - {table.Rows[0]["Section"].ToString()}";
                    }
                    IsClassVisible = true;
                }
                else
                    IsClassVisible = false;

                IsAttendenceMarked = true;
                ShowUserInfo = true;
                UserID.Value = "";
                DateAndTime = DateTime.Now.ToString();
                AttendenceCount++;
            }
            catch (Exception ex)
            {
                Logger.Log(ex, true);
                IsAttendenceMarked = false;
                ShowUserInfo = true;
                UserID.Value = "";
            }
        }

        #endregion
    }
}

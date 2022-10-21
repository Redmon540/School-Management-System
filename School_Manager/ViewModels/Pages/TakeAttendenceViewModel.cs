using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace School_Manager
{
    public class TakeAttendenceViewModel : BaseViewModel
    {
        #region Constructor

        public TakeAttendenceViewModel()
        {
            StartAttendenceCommand = new RelayCommand(StartAttendence);
            IDChangedCommand = new RelayCommand(IDChanged);
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
            FeildName = "Attendence Options",
            Value = "All",
            Items = new List<string> { "All", "Students", "Teachers" },
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

        public string QRCode { get; set; }

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

        public ICommand StartAttendenceCommand { get; set; }

        #endregion

        #region Command Methods

        private void StartAttendence()
        {
            try
            {
                if (AttendenceOptions.Value == "All")
                {
                    //for student attendence
                    if (DataAccess.GetDataTable($"SELECT [{day}] FROM Student_Attendence WHERE Month = {month} AND Year = {year}").Rows.Count == 0)
                    {
                        //to insert monthly record
                        DataAccess.ExecuteQuery($"INSERT INTO Student_Attendence (Month,Year,Student_ID) " +
                            $"SELECT '{month}' , '{year}' , Students.Student_ID FROM Students WHERE Students.Is_Active = 1");
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
                else if (AttendenceOptions.Value == "Students")
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
                        DataAccess.ExecuteQuery($"INSERT INTO Teacher_Attendence ( Month , Year , Teacher_ID ) " +
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
            catch(Exception ex)
            {
                Logger.Log(ex, true);
            }
        }

        private void IDChanged()
        {
            try
            {
                if (QRCode.IsNullOrEmpty())
                {
                    return;
                }

                if (!QRCode.EndsWith("\r\n"))
                    return;

                string [] codes = QRCode.Split(new string[] { "\r\n" }, StringSplitOptions.None);

                for (int i = 0; i < codes.Length; i++)
                {
                    string[] qr = codes[i].Split('-');
                    if (qr.Length != 2)
                    {
                        return;
                    }

                    int id = 0;
                    if (!int.TryParse(qr[1], out id))
                    {
                        return;
                    }

                    if (id <= 0)
                    {
                        return;
                    }

                    DataTable table = new DataTable();

                    if (qr[0] == "STD")
                    {
                        if (AttendenceOptions.Value == "Students" || AttendenceOptions.Value == "All")
                        {
                            table = DataAccess.GetDataTable($"SELECT [Student_ID] , [Name] , [Photo] , [Class] , [Section] FROM Students" +
                            $" LEFT JOIN Classes ON Classes.Class_ID = Students.Class_ID" +
                            $" LEFT JOIN Sections ON Sections.Section_ID = Students.Section_ID" +
                            $" WHERE Students.Student_ID = {id} AND Students.Is_Active = 1");
                            if (table.Rows.Count != 0)
                            {
                                DataAccess.ExecuteQuery($"UPDATE Student_Attendence SET [{day}] = 'P' WHERE Year = '{year}' AND Month = '{month}' AND Student_ID = {id}");
                                IDName = DataAccess.GetStudentID();
                                CurrentUser = "Student";

                                ID = id.ToString();
                                Name = table.Rows[0]["Name"].ToString();
                                Photo = Helper.ByteArrayToImage(table.Rows[0]["Photo"] as byte[]);
                                if (table.Rows[0]["Section"].ToString().IsNullOrEmpty())
                                {
                                    Class = table.Rows[0]["Class"].ToString();
                                }
                                else
                                {
                                    Class = $"{table.Rows[0]["Class"].ToString()} - {table.Rows[0]["Section"].ToString()}";
                                }
                                IsClassVisible = true;

                                IsAttendenceMarked = true;
                                ShowUserInfo = true;
                                QRCode = "";
                                DateAndTime = DateTime.Now.ToString();
                                AttendenceCount++;
                            }
                            else
                            {
                                IsAttendenceMarked = false;
                                ShowUserInfo = true;
                                QRCode = "";
                                return;
                            }
                        }
                        else
                        {
                            IsAttendenceMarked = false;
                            ShowUserInfo = true;
                            QRCode = "";
                            return;
                        }
                    }
                    else if (qr[0] == "TCH")
                    {
                        if (AttendenceOptions.Value == "Teachers" || AttendenceOptions.Value == "All")
                        {
                            table = DataAccess.GetDataTable($"SELECT [Teacher_ID] , [Name] , [Photo]  FROM Teachers" +
                            $" WHERE Teachers.Teacher_ID = {id}");
                            if (table.Rows.Count != 0)
                            {
                                DataAccess.ExecuteQuery($"UPDATE Teacher_Attendence SET [{day}] = 'P' WHERE Year = '{year}' AND Month = '{month}' AND Teacher_ID = {id}");
                                IDName = DataAccess.GetTeacherID();
                                CurrentUser = "Teacher";

                                ID = id.ToString();
                                Name = table.Rows[0]["Name"].ToString();
                                Photo = Helper.ByteArrayToImage(table.Rows[0]["Photo"] as byte[]);
                                IsClassVisible = false;

                                IsAttendenceMarked = true;
                                ShowUserInfo = true;
                                QRCode = "";
                                DateAndTime = DateTime.Now.ToString();
                                AttendenceCount++;
                            }
                            else
                            {
                                IsAttendenceMarked = false;
                                ShowUserInfo = true;
                                QRCode = "";
                                return;
                            }
                        }
                        else
                        {
                            IsAttendenceMarked = false;
                            ShowUserInfo = true;
                            QRCode = "";
                            return;
                        }
                    }


                    
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex, true);
                IsAttendenceMarked = false;
                ShowUserInfo = true;
                QRCode = "";
            }
        }

        #endregion
    }
}

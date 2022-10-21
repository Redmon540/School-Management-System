using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;

namespace School_Manager
{
    /// <summary>
    /// Class that provides methods to interact with SQL Server Database
    /// </summary>
    public static class DataAccess
    {
        #region Public Properties
        public static string connectionString;
        static SqlConnection sqlConn = new SqlConnection();
        static SqlCommand sqlCmd = new SqlCommand();
        static SqlDataAdapter sqlDa = new SqlDataAdapter();
        #endregion

        /// <summary>
        /// TO EXECUTE ANY SQL QUERY
        /// </summary>
        /// <param name="query">SQL Query</param>
        public static void ExecuteQuery(string query)
        {
            
            using (SqlConnection sqlConn = new SqlConnection(connectionString))
            {
                sqlConn.Open();
                SqlCommand sqlCmd = new SqlCommand(query, sqlConn);
                sqlCmd.ExecuteNonQuery();
                sqlCmd.Dispose();
            }
        }

        /// <summary>
        /// Accepts SQL Command as paraemeter which can be used to insert or update data through parameters
        /// </summary>
        /// <param name="SqlCommand"></param>
        public static void ExecuteQuery(SqlCommand SqlCommand)
        {
            
            using (SqlConnection sqlConn = new SqlConnection(connectionString))
            {
                sqlConn.Open();
                SqlCommand.Connection = sqlConn;
                SqlCommand.ExecuteNonQuery();
            }
        }
        
        /// <summary>
        /// to get a DataTable against a sql query
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public static DataTable GetDataTable(string query)
        {
            
            DataTable dTable = new DataTable();
            using (SqlConnection sqlConn = new SqlConnection(connectionString))
            {
                sqlConn.Open();
                SqlCommand sqlCmd = new SqlCommand(query, sqlConn);
                SqlDataAdapter sqlDa = new SqlDataAdapter(sqlCmd);
                sqlDa.Fill(dTable);
            }
            return dTable;
        }

        /// <summary>
        /// Accepts sql command and returns a data table
        /// </summary>
        /// <param name="SqlCmd"></param>
        /// <returns></returns>
        public static DataTable GetDataTable(SqlCommand SqlCmd)
        {

            DataTable dTable = new DataTable();
            List<string> list = new List<string>();
            using (SqlConnection sqlConn = new SqlConnection(connectionString))
            {
                SqlCmd.Connection = sqlConn;
                sqlConn.Open();
                SqlDataAdapter sqlDa = new SqlDataAdapter(SqlCmd);
                sqlDa.Fill(dTable);
            }
            return dTable;
        }

        /// <summary>
        /// to get parent id of any student's parents
        /// </summary>
        /// <param name="StudentID"></param>
        /// <returns></returns>
        public static string GetParentID(string StudentID)
        {
            var dTable = GetDataTable($"SELECT Parent_ID FROM Students WHERE Student_ID = {StudentID}");
            return dTable.Rows[0][0].ToString();
        }
        
        /// <summary>
        /// To get all class names
        /// </summary>
        /// <returns>A List of strings containing class names</returns>
        public static List<string> GetClassNames()
        {
            DataTable dTable = GetDataTable("SELECT [Class] FROM Classes ORDER BY [Class_ID]");
            List<string> list = new List<string>();
            for (int i = 0; i < dTable.Rows.Count; i++)
            {
                list.Add(dTable.Rows[i][0].ToString());
            }
            return list;
        }

        public static List<string> GetSectionsNames(string ClassID)
        {
            
            DataTable dTable = new DataTable();
            List<string> list = new List<string>();
            using (SqlConnection sqlConn = new SqlConnection(connectionString))
            {
                sqlConn.Open();
                SqlCommand sqlCmd = new SqlCommand("SELECT [Section] FROM [Sections] WHERE  [Class_ID] = @classID", sqlConn);
                sqlCmd.Parameters.Add(new SqlParameter() { ParameterName = "@classID", Value = ClassID });

                SqlDataAdapter sqlDa = new SqlDataAdapter(sqlCmd);
                sqlDa.Fill(dTable);
            }
            return dTable.AsEnumerable().Select(x => x[0].ToString()).ToList();
        }
       
        /// <summary>
        /// Gets the next identity column value of the table
        /// </summary>
        /// <param name="TableName"></param>
        /// <returns></returns>
        public static string GetNextID(string TableName)
        {
            if(TableName == "Students")
            {
                if (GetDataTable($"SELECT * FROM {TableName}").Rows.Count == 0)
                {
                    return "1";
                }
                return GetDataTable("SELECT MAX(Student_ID) +1 FROM Students").Rows[0][0].ToString();
            }
            if(GetDataTable($"SELECT * FROM {TableName}").Rows.Count == 0)
            {
                return "1";
            }
            DataTable dTable = new DataTable();
            using (SqlConnection sqlConn = new SqlConnection(connectionString))
            {
                sqlConn.Open();
                SqlCommand sqlCmd = new SqlCommand("GetNextID", sqlConn);
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Parameters.Add("@table", SqlDbType.NVarChar).Value = TableName;
                SqlDataAdapter sqlDa = new SqlDataAdapter(sqlCmd);
                sqlDa.Fill(dTable);
            }
            return dTable.Rows[0][0].ToString();
        }

        public static DataTable GetFeeRecord(string ClassID, string month , string year, string SectionID)
        {
            
            DataTable dTable = new DataTable();
            using (SqlConnection sqlConn = new SqlConnection(connectionString))
            {
                sqlConn.Open();
                SqlCommand sqlCmd = new SqlCommand("GetFeeRecord", sqlConn);
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Parameters.Add("@class_id", SqlDbType.Int).Value = ClassID;
                sqlCmd.Parameters.Add("@month", SqlDbType.Int).Value = month;
                sqlCmd.Parameters.Add("@year", SqlDbType.Int).Value = year;
                if (SectionID.IsNullOrEmpty())
                    sqlCmd.Parameters.Add("@section_id", SqlDbType.Int).Value = DBNull.Value;
                else
                    sqlCmd.Parameters.Add("@section_id", SqlDbType.Int).Value = SectionID;
                SqlDataAdapter sqlDa = new SqlDataAdapter(sqlCmd);
                sqlDa.Fill(dTable);
            }
            dTable.Columns["Student_ID"].ColumnName = GetStudentID();
            return dTable;
        }

        public static DataTable GetCurrentSessionFeeRecord(string SessionID, string StudentID)
        {
            
            DataTable dTable = new DataTable();
            using (SqlConnection sqlConn = new SqlConnection(connectionString))
            {
                sqlConn.Open();
                SqlCommand sqlCmd = new SqlCommand("GetCurrentSessionRecord", sqlConn);
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Parameters.Add("@session_id", SqlDbType.NVarChar).Value = SessionID;
                sqlCmd.Parameters.Add("@student_id", SqlDbType.NVarChar).Value = StudentID;
                SqlDataAdapter sqlDa = new SqlDataAdapter(sqlCmd);
                sqlDa.Fill(dTable);
            }
            return dTable;
        }

        public static ObservableCollection<MonthlyFeeRecord> GetFeeDetails(string StudentID, string FeeOptions, string Month , string Year)
        {
            DataTable dTable = new DataTable();
            ObservableCollection<MonthlyFeeRecord> feeCollection = new ObservableCollection<MonthlyFeeRecord>();

            
            using (SqlConnection sqlConn = new SqlConnection(connectionString))
            {
                sqlConn.Open();
                SqlCommand sqlCmd = new SqlCommand("GetFeeDetails", sqlConn);
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Parameters.Add("@student_id", SqlDbType.NVarChar).Value = StudentID;
                sqlCmd.Parameters.Add("@month", SqlDbType.NVarChar).Value = Month == "All" ? (object) DBNull.Value : Helper.GetMonthNumber(Month);
                sqlCmd.Parameters.Add("@year", SqlDbType.NVarChar).Value = Year == "All" ? (object) DBNull.Value : Year;
                sqlCmd.Parameters.Add("@status", SqlDbType.NVarChar).Value = FeeOptions == "All" ? (object)DBNull.Value : FeeOptions;
                SqlDataAdapter sqlDa = new SqlDataAdapter(sqlCmd);
                sqlDa.Fill(dTable);
            }

            var dates = dTable.AsEnumerable().Select(x => new DateTime((int)x["Year"], (int)x["Month"], 1)).ToList();
            //to sort dates in descending order
            dates.Sort((a, b) => b.Date.CompareTo(a.Date));
            dates = dates.Distinct().ToList();

            foreach (var date in dates)
            {
                feeCollection.Add(new MonthlyFeeRecord()
                {
                    FeeEntities = new ObservableCollection<FeeEntity>(
                        dTable.AsEnumerable().
                    Where(e => (int)e["Month"] == date.Month && (int)e["Year"] == date.Year).
                    Select(e => new FeeEntity()
                    {
                        FeeRecordID = e["Fee_Record_ID"].ToString(),
                        Fee = e["Fee"].ToString(),
                        Amount = e["Amount"].ToString(),
                        LateFee = e["Late_Fee"].ToString(),
                        Discount = e["Discount"].ToString(),
                        PaidAmount = e["Paid_Amount"].ToString(),
                        FeeStatus = e["Fee_Status"].ToString(),
                        Date = (DateTime)e["Due_Date"]
                    }).ToList()
                    ),
                    Session = $"{Helper.GetMonthName(date.Month)}, {date.Year}"
                });

            }

            return new ObservableCollection<MonthlyFeeRecord>(feeCollection.Where(e => e.FeeEntities.Count > 0).Select(e => e).ToList());

        }

        public static ObservableCollection<MonthlyFeeRecord> GetPaymentFeeDetails(string StudentID)
        {
            DataTable dTable = new DataTable();
            ObservableCollection<MonthlyFeeRecord> feeCollection = new ObservableCollection<MonthlyFeeRecord>();

            
            using (SqlConnection sqlConn = new SqlConnection(connectionString))
            {
                sqlConn.Open();
                SqlCommand sqlCmd = new SqlCommand("[GetFeePaymentDetails]", sqlConn);
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Parameters.Add("@student_id", SqlDbType.NVarChar).Value = StudentID;
                SqlDataAdapter sqlDa = new SqlDataAdapter(sqlCmd);
                sqlDa.Fill(dTable);
            }

            var dates = dTable.AsEnumerable().Select(x => new DateTime((int)x["Year"], (int)x["Month"], 1)).ToList();
            //to sort dates in descending order
            dates.Sort((a, b) => b.Date.CompareTo(a.Date));
            dates = dates.Distinct().ToList();

            foreach (var date in dates)
            {
                feeCollection.Add(new MonthlyFeeRecord()
                {
                    FeeEntities = new ObservableCollection<FeeEntity>(
                        dTable.AsEnumerable().
                    Where(e => (int)e["Month"] == date.Month && (int)e["Year"] == date.Year).
                    Select(e => new FeeEntity()
                    {
                        FeeRecordID = e["Fee_Record_ID"].ToString(),
                        Fee = e["Fee"].ToString(),
                        Amount = e["Amount"].ToString(),
                        LateFee = e["Late_Fee"].ToString(),
                        Discount = e["Discount"].ToString(),
                        PaidAmount = e["Paid_Amount"].ToString(),
                        FeeStatus = "PAY",
                        Date = (DateTime)e["Due_Date"]
                    }).ToList()
                    ),
                    Session = $"{Helper.GetMonthName(date.Month)}, {date.Year}"
                });

            }
            return new ObservableCollection<MonthlyFeeRecord>(feeCollection.Where(e => e.FeeEntities.Count > 0).Select(e => e).ToList());
        }

        public static ObservableCollection<MonthlyFeeRecord> GetFeeVoucherDetails(string VoucherID)
        {
            ObservableCollection<MonthlyFeeRecord> feeCollection = new ObservableCollection<MonthlyFeeRecord>();
            //to get all the fees present on fee voucher (all the feeses of sessions that are listed on voucher even if not all feeses are on voucher)
            DataTable record = new DataTable();
            
            using (SqlConnection sqlConn = new SqlConnection(connectionString))
            {
                sqlConn.Open();
                SqlCommand sqlCmd = new SqlCommand("GetFeeVoucherDetails", sqlConn);
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Parameters.Add("@voucher_id", SqlDbType.NVarChar).Value = VoucherID;
                SqlDataAdapter sqlDa = new SqlDataAdapter(sqlCmd);
                sqlDa.Fill(record);
            }
            //to get distinct sessions to set the fee panel with fees according to every session
            var sessions = record.AsEnumerable().GroupBy(x => x["Session_ID"]).Select(x => x.First()).ToList();

            foreach (DataRow session in sessions)
            {
                feeCollection.Add(new MonthlyFeeRecord()
                {
                    FeeEntities = new ObservableCollection<FeeEntity>(record.AsEnumerable().
                            Where(e => e["Session_ID"].ToString() == session["Session_ID"].ToString()).
                            Select(e => new FeeEntity()
                            {
                                FeeRecordID = e["Fee_Record_ID"].ToString(),
                                Fee = e["Fee"].ToString(),
                                Amount = e["Amount"].ToString(),
                                LateFee = e["Late Fee"].ToString(),
                                Discount = e["Discount"].ToString(),
                                FeeStatus = (bool)e["Fee_Status"] == true ? "PAID" : "PAY",
                                DueDate = ((DateTime)e["Due_Date"]).ToString("dd-MMM-yyyy")
                            }).ToList()
                            ),
                    Session = $"{session["Month"]}, {session["Year"]}"
                });
            }
            return new ObservableCollection<MonthlyFeeRecord>(feeCollection.Where(e => e.FeeEntities.Count > 0).Select(e => e).ToList());
        }

        public static string GetClassID(string ClassName)
        {
            return GetDataTable($"SELECT [Class_ID] FROM [Classes] WHERE [Class] = '{ClassName}'").Rows[0][0].ToString();
        }

        public static string GetClassName(string ClassID)
        {
            return GetDataTable($"SELECT [Class] FROM [Classes] WHERE [Class_ID] = '{ClassID}'").Rows[0][0].ToString();
        }

        public static string GetSessionID(string Month, string Year)
        {
            return GetDataTable($"SELECT [Session_ID] FROM [Session] WHERE [Month] = '{Month}' AND [Year] = '{Year}'").Rows[0][0].ToString();
        }

        public static bool InsertAccount(string AccountType, string UserName, SecureString Password)
        {
            bool inserted = true;
            try
            {
                var accountType = GetDataTable($"SELECT [Account_Type_ID] FROM [AccountType] WHERE [Account Type] = '{AccountType}'").Rows[0][0].ToString();
                var salt = Helper.GenerateSalt();
                
                DataTable dTable = new DataTable();
                using (SqlConnection sqlConn = new SqlConnection(connectionString))
                {
                    sqlConn.Open();
                    SqlCommand sqlCmd = new SqlCommand("INSERT INTO Accounts([Account_Type_ID] , [UserName] , [PasswordHash] , [Salt]) " +
                        "VALUES (@acountType , @username , @passHash , @salt)", sqlConn);
                    sqlCmd.Parameters.Add("@acountType", SqlDbType.Int).Value = Convert.ToInt32(accountType);
                    sqlCmd.Parameters.Add("@username", SqlDbType.NVarChar).Value = UserName;
                    sqlCmd.Parameters.Add("@passHash", SqlDbType.Binary).Value = Helper.GeneratePasswordHash(Password, salt);
                    sqlCmd.Parameters.Add("@salt", SqlDbType.Binary).Value = salt;
                    sqlCmd.ExecuteNonQuery();
                }
            }
            catch
            {
                inserted = false;
            }
            return inserted;
        }

        public static bool DoesClassExist(string ClassName)
        {
            DataTable dTable = new DataTable();
            using (sqlConn = new SqlConnection(connectionString))
            {
                sqlConn.Open();
                sqlCmd = new SqlCommand("SELECT * FROM Classes WHERE Class = @class", sqlConn);
                sqlCmd.Parameters.Add("@class", SqlDbType.NVarChar).Value = ClassName;
                sqlDa = new SqlDataAdapter(sqlCmd);
                sqlDa.Fill(dTable);
            }
            return dTable.Rows.Count != 0;
        }

        public static string GetSectionID(string SectionName, string ClassID)
        {
            DataTable dTable = new DataTable();
            using (sqlConn = new SqlConnection(connectionString))
            {
                sqlConn.Open();
                sqlCmd = new SqlCommand($"SELECT Section_ID FROM Sections WHERE Class_ID = {ClassID} AND Section = '{SectionName}'", sqlConn);
                sqlDa = new SqlDataAdapter(sqlCmd);
                sqlDa.Fill(dTable);
            }
            return dTable.Rows[0][0].ToString();
        }

        public static string GetSection(string SectionID)
        {
            DataTable dTable = new DataTable();
            using (sqlConn = new SqlConnection(connectionString))
            {
                sqlConn.Open();
                sqlCmd = new SqlCommand($"SELECT Section FROM Sections WHERE Section_ID = '{SectionID}'", sqlConn);
                sqlDa = new SqlDataAdapter(sqlCmd);
                sqlDa.Fill(dTable);
            }
            return dTable.Rows.Count == 0 ? "" : dTable.Rows[0][0].ToString();
        }

        public static string GetStudentID()
        {
            return GetDataTable($"SELECT Custom_Columns FROM _ColumnInformation WHERE Table_Name = 'Student_ID_Name'").Rows[0][0].ToString();
        }

        public static string GetParentID()
        {
            return GetDataTable($"SELECT Custom_Columns FROM _ColumnInformation WHERE Table_Name = 'Parent_ID_Name'").Rows[0][0].ToString();
        }

        public static string GetTeacherID()
        {
            return GetDataTable($"SELECT Custom_Columns FROM _ColumnInformation WHERE Table_Name = 'Teacher_ID_Name'").Rows[0][0].ToString();
        }

        public static string GetTeacherClassIDName()
        {
            return GetDataTable($"SELECT Custom_Columns FROM _ColumnInformation WHERE Table_Name = 'Teacher_Class_ID_Name'").Rows[0][0].ToString();
        }

        public static void CreateIDCards(string FrontCard, string BackCard, IDCardViewModel IDCardViewModel, string Class_ID, string FileName)
        {
            if (IDCardViewModel.SchoolLogo != null)
            {
                IDCardViewModel.SchoolLogo.Freeze();
            }

            DataTable table = new DataTable();

            if (Class_ID.IsNullOrEmpty())
            {
                table = GetDataTable($"SELECT [Photo] , [Student_ID] , [Name] , [Father Name] , [Class] , [Section] FROM Students " +
                $"LEFT JOIN Parents ON Parents.Parent_ID = Students.Parent_ID " +
                $"LEFT JOIN Classes ON Classes.Class_ID = Students.Class_ID " +
                $"LEFT JOIN Sections ON Sections.Section_ID = Students.Section_ID");
            }
            else
            {
                table = GetDataTable($"SELECT [Photo] , [Student_ID] , [Name] , [Father Name] , [Class] , [Section] FROM Students " +
                $"LEFT JOIN Parents ON Parents.Parent_ID = Students.Parent_ID " +
                $"LEFT JOIN Classes ON Classes.Class_ID = Students.Class_ID " +
                $"LEFT JOIN Sections ON Sections.Section_ID = Students.Section_ID WHERE Class_ID = {Class_ID}");
            }

            var style = Application.Current.MainWindow.FindResource("IDCardSheetStyle") as Style;
            //run another UI thread because FeeVoucher is a ui element and can only be created by UI thread
            //which keeps the application responsive
            Thread thread = new Thread(() =>
            {
                FixedDocument fixDoc = new FixedDocument();
                int i, j;
                for (i = 0; i < table.Rows.Count - 10; i++)
                {
                    ItemsControl FrontSheet = new ItemsControl() { Style = style };
                    ItemsControl BackSheet = new ItemsControl() { Style = style };
                    List<UserControl> FrontCards = new List<UserControl>();
                    List<UserControl> BackCards = new List<UserControl>();

                    for (j = i; j < i + 10; j++)
                    {
                        var CardViewModel = new IDCardViewModel()
                        {
                            IDName = IDCardViewModel.IDName,
                            ID = table.Rows[j]["Student_ID"].ToString(),
                            Name = table.Rows[j]["Name"].ToString(),
                            FatherName = table.Rows[j]["Father Name"].ToString(),
                            Class = table.Rows[j]["Section"].ToString().IsNotNullOrEmpty() ? $"{table.Rows[j]["Class"]} - {table.Rows[j]["Section"]}" : table.Rows[j]["Class"].ToString(),
                            Photo = table.Rows[j]["Photo"] != null ? Helper.ByteArrayToImage(table.Rows[j]["Photo"] as byte[]) : null,
                            QRCode = Helper.GetQRCode($"STD-{table.Rows[j]["Student_ID"]}"),
                            IssueDate = IDCardViewModel.IssueDate,
                            ValidDate = IDCardViewModel.ValidDate,
                            SchoolLogo = IDCardViewModel.SchoolLogo,
                            SchoolName = IDCardViewModel.SchoolName,
                            Note = IDCardViewModel.Note,
                            TermsAndConditions = IDCardViewModel.TermsAndConditions
                        };
                        //add front card to the list
                        object cardFront = Activator.CreateInstance(Type.GetType(FrontCard));
                        (cardFront as UserControl).DataContext = CardViewModel;
                        FrontCards.Add(cardFront as UserControl);

                        //add back card to the list
                        object cardBack = Activator.CreateInstance(Type.GetType(BackCard));
                        (cardBack as UserControl).DataContext = CardViewModel;
                        BackCards.Add(cardBack as UserControl);
                    }
                    i = j;

                    //for front card sheet
                    FrontSheet.ItemsSource = FrontCards;
                    PageContent PgContent1 = new PageContent();
                    FixedPage fixPg1 = new FixedPage();
                    fixPg1.Height = 1122.528;
                    fixPg1.Width = 793.728;
                    fixPg1.Children.Add(FrontSheet);
                    ((IAddChild)PgContent1).AddChild(fixPg1);
                    fixDoc.Pages.Add(PgContent1);

                    //for front card sheet
                    BackSheet.ItemsSource = BackCards;
                    PageContent PgContent2 = new PageContent();
                    FixedPage fixPg2 = new FixedPage();
                    fixPg2.Height = 1122.528;
                    fixPg2.Width = 793.728;
                    fixPg2.Children.Add(BackSheet);
                    ((IAddChild)PgContent2).AddChild(fixPg2);
                    fixDoc.Pages.Add(PgContent2);
                }

                //Now Write the document
                XpsDocument xpsd = new XpsDocument(FileName, FileAccess.ReadWrite);
                XpsDocumentWriter xw = XpsDocument.CreateXpsDocumentWriter(xpsd);
                xw.Write(fixDoc);
                xpsd.Close();

                //PdfSharp.Xps.XpsConverter.Convert(@"F:\test.xps", @"F:\testPDF.pdf", 0);

                //var pdfXpsDoc = PdfSharp.Xps.XpsModel.XpsDocument.Open();
                //PdfSharp.Xps.XpsConverter.Convert(pdfXpsDoc, d.FileName, 0);
            });
            //make thread a UI thread
            thread.SetApartmentState(ApartmentState.STA);
            //start the thread
            thread.Start();
            //helps to wait for the task of this thread to finish before it returns to the main thread
            thread.Join();
        }

        public static void CreateIDCards(string FrontCard, string BackCard, IDCardViewModel CardViewModel, string FileName)
        {
            if (CardViewModel.Photo != null)
            {
                CardViewModel.Photo.Freeze();
            }

            if (CardViewModel.QRCode != null)
            {
                CardViewModel.QRCode.Freeze();
            }

            if (CardViewModel.SchoolLogo != null)
            {
                CardViewModel.SchoolLogo.Freeze();
            }

            var style = Application.Current.MainWindow.FindResource("IDCardSheetStyle") as Style;

            //run another UI thread because FeeVoucher is a ui element and can only be created by UI thread
            //which keeps the application responsive
            Thread thread = new Thread(() =>
            {
                FixedDocument fixDoc = new FixedDocument();

                ItemsControl Sheet = new ItemsControl() { Style = style };
                Sheet.Items.Add(Activator.CreateInstance(Type.GetType(FrontCard)));
                Sheet.Items.Add(Activator.CreateInstance(Type.GetType(BackCard)));
                Sheet.DataContext = CardViewModel;

                //for front card sheet
                PageContent PgContent1 = new PageContent();
                FixedPage fixPg1 = new FixedPage();
                fixPg1.Height = 1122.528;
                fixPg1.Width = 793.728;
                fixPg1.Children.Add(Sheet);
                ((IAddChild)PgContent1).AddChild(fixPg1);
                fixDoc.Pages.Add(PgContent1);


                //Now Write the document
                XpsDocument xpsd = new XpsDocument(FileName, FileAccess.ReadWrite);
                XpsDocumentWriter xw = XpsDocument.CreateXpsDocumentWriter(xpsd);
                xw.Write(fixDoc);
                xpsd.Close();

                //PdfSharp.Xps.XpsConverter.Convert(@"F:\test.xps", @"F:\testPDF.pdf", 0);

                //var pdfXpsDoc = PdfSharp.Xps.XpsModel.XpsDocument.Open();
                //PdfSharp.Xps.XpsConverter.Convert(pdfXpsDoc, d.FileName, 0);
            });
            //make thread a UI thread
            thread.SetApartmentState(ApartmentState.STA);
            //start the thread
            thread.Start();
            //helps to wait for the task of this thread to finish before it returns to the main thread
            thread.Join();
        }

        public static void CreateStudentIDCards(
            ObservableCollection<DesignerControls> FrontCardItems,
            ObservableCollection<DesignerControls> BackCardItems, BitmapImage FrontCardBackground,
            BitmapImage BackCardBackground, double CardWidth, double CardHeight,
            string ID, string Class_ID, string FileName, bool IsBackCardAdded)
        {
            //run another UI thread because FeeVoucher is a ui element and can only be created by UI thread
            //which keeps the application responsive
            //Thread thread = new Thread(() =>
            App.Current.MainWindow.Dispatcher.BeginInvoke(new ThreadStart(() =>
            {
                string idName = "";
                idName = GetStudentID();

                DataTable table = new DataTable();

                if (ID.IsNotNullOrEmpty())
                {
                    table = GetDataTable($"SELECT * FROM Students " +
                $"LEFT JOIN Parents ON Parents.Parent_ID = Students.Parent_ID " +
                $"LEFT JOIN Classes ON Classes.Class_ID = Students.Class_ID " +
                $"LEFT JOIN Sections ON Sections.Section_ID = Students.Section_ID WHERE Student_ID = {ID} AND Students.Is_Active = 1");

                }
                else if (Class_ID.IsNotNullOrEmpty())
                {
                    table = GetDataTable($"SELECT * FROM Students " +
                $"LEFT JOIN Parents ON Parents.Parent_ID = Students.Parent_ID " +
                $"LEFT JOIN Classes ON Classes.Class_ID = Students.Class_ID " +
                $"LEFT JOIN Sections ON Sections.Section_ID = Students.Section_ID WHERE Students.Class_ID = {Class_ID} AND Students.Is_Active = 1");
                }
                else
                {
                    table = GetDataTable($"SELECT * FROM Students " +
                       $"LEFT JOIN Parents ON Parents.Parent_ID = Students.Parent_ID " +
                       $"LEFT JOIN Classes ON Classes.Class_ID = Students.Class_ID " +
                       $"LEFT JOIN Sections ON Sections.Section_ID = Students.Section_ID AND Students.Is_Active = 1");
                }


                FixedDocument fixDoc = new FixedDocument();

                foreach (DataRow row in table.Rows)
                {
                    List<DesignerControls> frontCardItems = new List<DesignerControls>();
                    List<DesignerControls> backCardItems = new List<DesignerControls>();
                    foreach (var cardItem in FrontCardItems)
                    {
                        TextControl TextBlock = cardItem as TextControl;
                        if (TextBlock != null)
                        {
                            frontCardItems.Add(new TextControl
                            {
                                CanvasLeft = TextBlock.CanvasLeft,
                                CanvasTop = TextBlock.CanvasTop,
                                FontSize = TextBlock.FontSize,
                                FontStyle = TextBlock.FontStyle,
                                FontWeight = TextBlock.FontWeight,
                                Foreground = TextBlock.Foreground,
                                Height = TextBlock.Height,
                                Width = TextBlock.Width,
                                IsHitTestVisible = true,
                                IsSelected = false,
                                TextAlignment = TextBlock.TextAlignment,
                                TextDecoration = TextBlock.TextDecoration,
                                Value = TextBlock.Value != idName ? row[$"{cardItem.Value}"].ToString() : row[$"Student_ID"].ToString()
                            });

                        }
                        ImageControl Image = cardItem as ImageControl;
                        if (Image != null)
                        {
                            var image = new ImageControl
                            {
                                CanvasLeft = Image.CanvasLeft,
                                CanvasTop = Image.CanvasTop,
                                Height = Image.Height,
                                Width = Image.Width,
                                IsSelected = false,
                                IsHitTestVisible = true,
                            };
                            BitmapImage img = Helper.ByteArrayToImage(row[$"{Image.Value}"] as byte[]);
                            if (img != null)
                            {
                                img.Freeze();
                                image.Image = img;
                                frontCardItems.Add(image);
                            }
                        }
                        QRCodeControl QrCode = cardItem as QRCodeControl;
                        if (QrCode != null)
                        {
                            var image = new QRCodeControl
                            {
                                CanvasLeft = QrCode.CanvasLeft,
                                CanvasTop = QrCode.CanvasTop,
                                Height = QrCode.Height,
                                Width = QrCode.Width,
                                IsSelected = false,
                                IsHitTestVisible = true,
                            };
                            BitmapImage img = Helper.GetQRCode($"STD-{row["Student_ID"].ToString()}");
                            if (img != null)
                            {
                                img.Freeze();
                                image.Image = img;
                                frontCardItems.Add(image);
                            }
                        }
                    }

                    if (IsBackCardAdded)
                    {
                        foreach (var cardItem in BackCardItems)
                        {
                            TextControl TextBlock = cardItem as TextControl;
                            if (TextBlock != null)
                            {
                                backCardItems.Add(new TextControl
                                {
                                    CanvasLeft = TextBlock.CanvasLeft,
                                    CanvasTop = TextBlock.CanvasTop,
                                    FontSize = TextBlock.FontSize,
                                    FontStyle = TextBlock.FontStyle,
                                    FontWeight = TextBlock.FontWeight,
                                    Foreground = TextBlock.Foreground,
                                    Height = TextBlock.Height,
                                    Width = TextBlock.Width,
                                    IsHitTestVisible = true,
                                    IsSelected = false,
                                    TextAlignment = TextBlock.TextAlignment,
                                    TextDecoration = TextBlock.TextDecoration,
                                    Value = TextBlock.Value != idName ? row[$"{cardItem.Value}"].ToString() : row[$"Student_ID"].ToString()
                                });

                            }
                            ImageControl Image = cardItem as ImageControl;
                            if (Image != null)
                            {
                                var image = new ImageControl
                                {
                                    CanvasLeft = Image.CanvasLeft,
                                    CanvasTop = Image.CanvasTop,
                                    Height = Image.Height,
                                    Width = Image.Width,
                                    IsSelected = false,
                                    IsHitTestVisible = true,
                                };
                                BitmapImage img = Helper.ByteArrayToImage(row[$"{Image.Value}"] as byte[]);
                                if (img != null)
                                {
                                    img.Freeze();
                                    image.Image = img;
                                    backCardItems.Add(image);
                                }
                            }
                            QRCodeControl QrCode = cardItem as QRCodeControl;
                            if (QrCode != null)
                            {
                                var image = new QRCodeControl
                                {
                                    CanvasLeft = QrCode.CanvasLeft,
                                    CanvasTop = QrCode.CanvasTop,
                                    Height = QrCode.Height,
                                    Width = QrCode.Width,
                                    IsSelected = false,
                                    IsHitTestVisible = true,
                                };
                                BitmapImage img = Helper.GetQRCode($"STD-{row["Student_ID"].ToString()}");
                                if (img != null)
                                {
                                    img.Freeze();
                                    image.Image = img;
                                    backCardItems.Add(image);
                                }
                            }
                        }
                    }
                    var i = new CardDesigner();
                    StackPanel stackpanel = new StackPanel();
                    stackpanel.Children.Add(new CardDesigner
                    {
                        ItemSource = new ObservableCollection<DesignerControls>(frontCardItems),
                        BackgroundImage = FrontCardBackground,
                        Width = CardWidth * 96,
                        Height = CardHeight * 96
                    });
                    if (IsBackCardAdded)
                    {
                        stackpanel.Children.Add(new CardDesigner
                        {
                            ItemSource = new ObservableCollection<DesignerControls>(backCardItems),
                            BackgroundImage = BackCardBackground,
                            Width = CardWidth * 96,
                            Height = CardHeight * 96
                        });
                    }

                    PageContent PgContent1 = new PageContent();
                    FixedPage fixPg1 = new FixedPage();
                    if (IsBackCardAdded)
                        fixPg1.Height = CardHeight * 96 * 2;
                    else
                        fixPg1.Height = CardHeight * 96;

                    fixPg1.Width = CardWidth * 96;
                    fixPg1.Children.Add(stackpanel);
                    ((IAddChild)PgContent1).AddChild(fixPg1);
                    fixDoc.Pages.Add(PgContent1);
                }



                //Now Write the document
                XpsDocument xpsd = new XpsDocument(FileName, FileAccess.ReadWrite);
                XpsDocumentWriter xw = XpsDocument.CreateXpsDocumentWriter(xpsd);
                xw.Write(fixDoc);
                xpsd.Close();

                //PdfSharp.Xps.XpsConverter.Convert(FileName, FileName.Replace(".xps", ".pdf"),0,true);

                //var pdfXpsDoc = PdfSharp.Xps.XpsModel.XpsDocument.Open();
                //PdfSharp.Xps.XpsConverter.Convert(pdfXpsDoc, d.FileName, 0);
            }));
            ////make thread a UI thread
            //thread.SetApartmentState(ApartmentState.STA);
            ////start the thread
            //thread.Start();
            ////helps to wait for the task of this thread to finish before it returns to the main thread
            //thread.Join();
        }

        public static void CreateTeacherIDCards(
           ObservableCollection<DesignerControls> FrontCardItems,
           ObservableCollection<DesignerControls> BackCardItems, BitmapImage FrontCardBackground,
           BitmapImage BackCardBackground, double CardWidth, double CardHeight, string ID,
           string FileName, bool IsBackCardAdded)
        {
            //run another UI thread because FeeVoucher is a ui element and can only be created by UI thread
            //which keeps the application responsive
            //Thread thread = new Thread(() =>
            App.Current.MainWindow.Dispatcher.BeginInvoke(new ThreadStart(() =>
            {
                string idName = "";
                idName = GetTeacherID();

                DataTable table = new DataTable();

                if (ID.IsNotNullOrEmpty())
                {
                    table = GetDataTable($"SELECT * FROM Teachers WHERE Teacher_ID = {ID}");
                }
                else
                {
                    table = GetDataTable($"SELECT * FROM Teachers ");
                }

                FixedDocument fixDoc = new FixedDocument();

                foreach (DataRow row in table.Rows)
                {
                    List<DesignerControls> frontCardItems = new List<DesignerControls>();
                    List<DesignerControls> backCardItems = new List<DesignerControls>();
                    foreach (var cardItem in FrontCardItems)
                    {
                        TextControl TextBlock = cardItem as TextControl;
                        if (TextBlock != null)
                        {
                            frontCardItems.Add(new TextControl
                            {
                                CanvasLeft = TextBlock.CanvasLeft,
                                CanvasTop = TextBlock.CanvasTop,
                                FontSize = TextBlock.FontSize,
                                FontStyle = TextBlock.FontStyle,
                                FontWeight = TextBlock.FontWeight,
                                Foreground = TextBlock.Foreground,
                                Height = TextBlock.Height,
                                Width = TextBlock.Width,
                                IsHitTestVisible = true,
                                IsSelected = false,
                                TextAlignment = TextBlock.TextAlignment,
                                TextDecoration = TextBlock.TextDecoration,
                                Value = TextBlock.Value != idName ? row[$"{cardItem.Value}"].ToString() : row[$"Teacher_ID"].ToString()
                            });

                        }
                        ImageControl Image = cardItem as ImageControl;
                        if (Image != null)
                        {
                            var image = new ImageControl
                            {
                                CanvasLeft = Image.CanvasLeft,
                                CanvasTop = Image.CanvasTop,
                                Height = Image.Height,
                                Width = Image.Width,
                                IsSelected = false,
                                IsHitTestVisible = true,
                            };
                            BitmapImage img = Helper.ByteArrayToImage(row[$"{Image.Value}"] as byte[]);
                            if (img != null)
                            {
                                img.Freeze();
                                image.Image = img;
                                frontCardItems.Add(image);
                            }
                        }
                        QRCodeControl QrCode = cardItem as QRCodeControl;
                        if (QrCode != null)
                        {
                            var image = new QRCodeControl
                            {
                                CanvasLeft = QrCode.CanvasLeft,
                                CanvasTop = QrCode.CanvasTop,
                                Height = QrCode.Height,
                                Width = QrCode.Width,
                                IsSelected = false,
                                IsHitTestVisible = true,
                            };
                            BitmapImage img = Helper.GetQRCode($"TCH-{row["Teacher_ID"].ToString()}");
                            if (img != null)
                            {
                                img.Freeze();
                                image.Image = img;
                                frontCardItems.Add(image);
                            }
                        }
                    }

                    if (IsBackCardAdded)
                    {
                        foreach (var cardItem in BackCardItems)
                        {
                            TextControl TextBlock = cardItem as TextControl;
                            if (TextBlock != null)
                            {
                                backCardItems.Add(new TextControl
                                {
                                    CanvasLeft = TextBlock.CanvasLeft,
                                    CanvasTop = TextBlock.CanvasTop,
                                    FontSize = TextBlock.FontSize,
                                    FontStyle = TextBlock.FontStyle,
                                    FontWeight = TextBlock.FontWeight,
                                    Foreground = TextBlock.Foreground,
                                    Height = TextBlock.Height,
                                    Width = TextBlock.Width,
                                    IsHitTestVisible = true,
                                    IsSelected = false,
                                    TextAlignment = TextBlock.TextAlignment,
                                    TextDecoration = TextBlock.TextDecoration,
                                    Value = TextBlock.Value != idName ? row[$"{cardItem.Value}"].ToString() : row[$"Teacher_ID"].ToString()
                                });

                            }
                            ImageControl Image = cardItem as ImageControl;
                            if (Image != null)
                            {
                                var image = new ImageControl
                                {
                                    CanvasLeft = Image.CanvasLeft,
                                    CanvasTop = Image.CanvasTop,
                                    Height = Image.Height,
                                    Width = Image.Width,
                                    IsSelected = false,
                                    IsHitTestVisible = true,
                                };
                                BitmapImage img = Helper.ByteArrayToImage(row[$"{Image.Value}"] as byte[]);
                                if (img != null)
                                {
                                    img.Freeze();
                                    image.Image = img;
                                    backCardItems.Add(image);
                                }
                            }
                            QRCodeControl QrCode = cardItem as QRCodeControl;
                            if (QrCode != null)
                            {
                                var image = new QRCodeControl
                                {
                                    CanvasLeft = QrCode.CanvasLeft,
                                    CanvasTop = QrCode.CanvasTop,
                                    Height = QrCode.Height,
                                    Width = QrCode.Width,
                                    IsSelected = false,
                                    IsHitTestVisible = true,
                                };
                                BitmapImage img = Helper.GetQRCode($"TCH-{row["Teacher_ID"].ToString()}");
                                if (img != null)
                                {
                                    img.Freeze();
                                    image.Image = img;
                                    backCardItems.Add(image);
                                }
                            }
                        }
                    }
                    var i = new CardDesigner();
                    StackPanel stackpanel = new StackPanel();
                    stackpanel.Children.Add(new CardDesigner
                    {
                        ItemSource = new ObservableCollection<DesignerControls>(frontCardItems),
                        BackgroundImage = FrontCardBackground,
                        Width = CardWidth * 96,
                        Height = CardHeight * 96
                    });
                    if (IsBackCardAdded)
                    {
                        stackpanel.Children.Add(new CardDesigner
                        {
                            ItemSource = new ObservableCollection<DesignerControls>(backCardItems),
                            BackgroundImage = BackCardBackground,
                            Width = CardWidth * 96,
                            Height = CardHeight * 96
                        });
                    }

                    PageContent PgContent1 = new PageContent();
                    FixedPage fixPg1 = new FixedPage();
                    if (IsBackCardAdded)
                        fixPg1.Height = CardHeight * 96 * 2;
                    else
                        fixPg1.Height = CardHeight * 96;

                    fixPg1.Width = CardWidth * 96;
                    fixPg1.Children.Add(stackpanel);
                    ((IAddChild)PgContent1).AddChild(fixPg1);
                    fixDoc.Pages.Add(PgContent1);
                }



                //Now Write the document
                XpsDocument xpsd = new XpsDocument(FileName, FileAccess.ReadWrite);
                XpsDocumentWriter xw = XpsDocument.CreateXpsDocumentWriter(xpsd);
                xw.Write(fixDoc);
                xpsd.Close();

                //PdfSharp.Xps.XpsConverter.Convert(FileName, @"F:\testPDF.pdf", 0);

                //var pdfXpsDoc = PdfSharp.Xps.XpsModel.XpsDocument.Open();
                //PdfSharp.Xps.XpsConverter.Convert(pdfXpsDoc, d.FileName, 0);
            }));
            ////make thread a UI thread
            //thread.SetApartmentState(ApartmentState.STA);
            ////start the thread
            //thread.Start();
            ////helps to wait for the task of this thread to finish before it returns to the main thread
            //thread.Join();
        }

        public static string GetYearID(string Year)
        {
            return GetDataTable($"SELECT Year_ID FROM Years WHERE Year = {Year}").Rows[0][0].ToString();
        }

        public static void CreateSalarySheet(SalarySheet grid, string FileName)
        {
            var model = (grid.DataContext as SalarySheetViewModel);
            DataTable data = model.ItemSource;
            FixedDocument fixDoc = new FixedDocument();
            int i, j;
            for (i = 0; i < data.Rows.Count; i++)
            {
                DataTable table = new DataTable();
                foreach (DataColumn col in data.Columns)
                {
                    table.Columns.Add(col.ColumnName, col.DataType);
                }
                for (j = i; j < i + 16; j++)
                {
                    if (j < data.Rows.Count)
                        table.Rows.Add(data.Rows[j].ItemArray);
                }
                i = j-1;
                SalarySheet sheet = new SalarySheet
                {
                    DataContext = new SalarySheetViewModel
                    {
                        Heading = model.Heading,
                        CanEditHeading = false,
                        IsEditButtonVisible = false,
                        CanAddRows = false,
                        ItemSource = table
                    }
                };
                PageContent PgContent1 = new PageContent();
                FixedPage fixPg1 = new FixedPage();

                sheet.Margin = new Thickness(40);
                sheet.Width = 1050;
                sheet.Height = 720;
                sheet.HorizontalAlignment = HorizontalAlignment.Center;
                fixPg1.Width = 1122.528;
                fixPg1.Height = 793.728;
                fixPg1.Children.Add(sheet);
                ((IAddChild)PgContent1).AddChild(fixPg1);
                fixDoc.Pages.Add(PgContent1);
            }

            //Now Write the document
            XpsDocument xpsd = new XpsDocument(FileName, FileAccess.ReadWrite);
            XpsDocumentWriter xw = XpsDocument.CreateXpsDocumentWriter(xpsd);
            xw.Write(fixDoc);
            xpsd.Close();
        }

        public static void CreateFeeVoucher(ObservableCollection<DesignerControls> _FrontVoucherItems,
            ObservableCollection<DesignerControls> _BackVoucherItems, BitmapImage FrontCardBackground,
            BitmapImage BackCardBackground, double CardWidth, double CardHeight, string Class,
            string FileName, bool IsBackVoucherAdded, string Student_ID = null)
        {
            if(FrontCardBackground != null)
            FrontCardBackground.Freeze();
            if(BackCardBackground != null)
            BackCardBackground.Freeze();
            //run another UI thread because FeeVoucher is a ui element and can only be created by UI thread
            //which keeps the application responsive
            //Thread thread = new Thread(() =>
            //App.Current.MainWindow.Dispatcher.BeginInvoke(new ThreadStart(() =>
            //{
                var FrontVoucherItems = new ObservableCollection<DesignerControls>(_FrontVoucherItems);
                var BackVoucherItems = new ObservableCollection<DesignerControls>(_BackVoucherItems);
                var classID = GetClassID(Class);
                //to select all the sessions from the voucher
                var sessions = FrontVoucherItems.Where(x => x.GetType() == typeof(FeeControl)).Select(x => new Session { Month = (x as FeeControl).Month, Year = (x as FeeControl).Year }).Distinct().ToList();

                var s1 = "";
                foreach (var item in sessions)
                {
                    s1 += $"OR ([Month] = {item.Month} AND [Year] = {item.Year}) ";
                }
                s1 = s1.Remove(0, 2);
                var s2 = "";
                foreach (var item in sessions)
                {
                    s2 += $"AND ([Month] != {item.Month} OR [Year] != {item.Year}) ";
                }
                s2 = s2.Remove(0, 3);

                DataTable feeRecord = new DataTable();
                DataTable previousDues = new DataTable();
                DataTable studentInfo = new DataTable();
                var dateTime = DateTime.Now;


                if (Student_ID.IsNullOrEmpty())
                {
                    feeRecord = GetDataTable($"SELECT Fee_Record.Student_ID , Fee_Record_ID , Fee , Amount, Discount , Amount - Discount AS Total , " +
                        $"Late_Fee AS [Late Fee] , Paid_Amount, Due_Date AS [Due Date] , [Month] , [Year] FROM Fee_Record LEFT JOIN Voucher_Record " +
                        $"ON Voucher_Record.[Voucher ID] = Fee_Record.Paid_Voucher_ID  " +
                        $"LEFT JOIN Students ON Students.Student_ID = Fee_Record.Student_ID " +
                        $" WHERE Students.Class_ID = {classID} " +
                        $"AND( {s1} )ORDER BY Student_ID");

                    previousDues = GetDataTable($"SELECT Fee_Record.Student_ID ,SUM " +
                        $"(Amount + (CASE WHEN (([Received Date] IS NOT NULL AND  Due_Date < [Received Date]) " +
                        $"OR ([Received Date] IS NULL AND Due_Date < GETDATE())) THEN ISNULL(Late_Fee,0) ELSE 0 END ) - " +
                        $"Discount - Paid_Amount ) AS Dues FROM Fee_Record LEFT JOIN Voucher_Record ON " +
                        $"Voucher_Record.[Voucher ID] = Fee_Record.Paid_Voucher_ID " +
                        $"LEFT JOIN Students ON Students.Student_ID = Fee_Record.Student_ID " +
                        $" WHERE Students.Class_ID = {classID} AND" +
                        $"{s2} GROUP BY Fee_Record.Student_ID ORDER BY Fee_Record.Student_ID");

                ////insert fee_record_id in voucher_record and fee_per_voucher_record
                //ExecuteQuery($"INSERT INTO Voucher_Record(Student_ID,[DateTime]) " +
                //                $"SELECT Student_ID, '{dateTime}' FROM Students WHERE Students.Is_Active = 1 " +
                //                $"AND Students.Class_ID = {classID}" +
                //                $" " +
                //                $"SELECT * INTO #Vouchers1 FROM Voucher_Record  " +
                //                $"WHERE[DateTime] = '{dateTime}' " +
                //                $" " +
                //                $"INSERT INTO Fee_Per_Voucher_Record(Fee_Record_ID, Voucher_ID, Is_Previous) " +
                //                $"SELECT Fee_Record_ID, #Vouchers1.[Voucher ID] , 0 FROM Fee_Record " +
                //                $"LEFT JOIN #Vouchers1 ON #Vouchers1.Student_ID = Fee_Record.Student_ID  " +
                //                $"LEFT JOIN Voucher_Record ON Voucher_Record.[Voucher ID] = Fee_Record.Paid_Voucher_ID " +
                //                $"LEFT JOIN Students ON Students.Student_ID = Fee_Record.Student_ID " +
                //                $"WHERE Students.Class_ID = {classID} AND Amount + (CASE WHEN((Voucher_Record.[Received Date] " +
                //                $"IS NOT NULL AND  Due_Date < Voucher_Record.[Received Date]) OR(Voucher_Record.[Received Date] IS NULL AND " +
                //                $"Due_Date < GETDATE())) THEN ISNULL(Late_Fee,0) ELSE 0 END )-Discount - Paid_Amount != 0 ");

                studentInfo = GetDataTable($"SELECT * FROM Students " +
                    $"LEFT JOIN Parents ON Parents.Parent_ID = Students.Parent_ID " +
                    $"LEFT JOIN Classes ON Classes.Class_ID = Students.Class_ID " +
                    $"LEFT JOIN Sections ON Sections.Section_ID = Students.Section_ID " +
                    $"WHERE Students.Class_ID = {classID} AND Students.Is_Active = 1");
                }
                else
                {
                    feeRecord = GetDataTable($"SELECT Fee_Record.Student_ID , Fee_Record_ID , Fee , Amount, Discount , Amount - Discount AS Total ," +
                        $"Late_Fee AS [Late Fee], Paid_Amount, Due_Date AS [Due Date], [Month] , [Year] FROM Fee_Record LEFT JOIN Voucher_Record " +
                        $"ON Voucher_Record.[Voucher ID] = Fee_Record.Paid_Voucher_ID  " +
                        $"WHERE Fee_Record.Student_ID = {Student_ID} " +
                        $"AND( {s1} )ORDER BY Student_ID");

                    previousDues = GetDataTable($"SELECT Fee_Record.Student_ID ,SUM " +
                        $"(Amount + (CASE WHEN (([Received Date] IS NOT NULL AND  Due_Date < [Received Date]) " +
                        $"OR ([Received Date] IS NULL AND Due_Date < GETDATE())) THEN ISNULL(Late_Fee,0) ELSE 0 END ) - " +
                        $"Discount - Paid_Amount ) AS Dues FROM Fee_Record LEFT JOIN Voucher_Record ON " +
                        $"Voucher_Record.[Voucher ID] = Fee_Record.Paid_Voucher_ID WHERE " +
                        $"Fee_Record.Student_ID = {Student_ID} AND " +
                        $"{s2} GROUP BY Fee_Record.Student_ID ORDER BY Fee_Record.Student_ID");

                //insert fee_record_id in voucher_record and fee_per_voucher_record
                //ExecuteQuery($"INSERT INTO Voucher_Record(Student_ID,[DateTime]) " +
                //                $"SELECT Student_ID, '{dateTime}' FROM Students WHERE Students.Is_Active = 1 " +
                //                $"AND Students.Student_ID = {Student_ID}" +
                //                $" " +
                //                $"SELECT * INTO #Vouchers1 FROM Voucher_Record  " +
                //                $"WHERE[DateTime] = '{dateTime}' " +
                //                $" " +
                //                $"INSERT INTO Fee_Per_Voucher_Record(Fee_Record_ID, Voucher_ID, Is_Previous) " +
                //                $"SELECT Fee_Record_ID, #Vouchers1.[Voucher ID] , 0 FROM Fee_Record " +
                //                $"LEFT JOIN #Vouchers1 ON #Vouchers1.Student_ID = Fee_Record.Student_ID  " +
                //                $"LEFT JOIN Voucher_Record ON Voucher_Record.[Voucher ID] = Fee_Record.Paid_Voucher_ID " +
                //                $"WHERE Fee_Record.Student_ID = {Student_ID} AND Amount + (CASE WHEN((Voucher_Record.[Received Date] " +
                //                $"IS NOT NULL AND  Due_Date < Voucher_Record.[Received Date]) OR(Voucher_Record.[Received Date] IS NULL AND " +
                //                $"Due_Date < GETDATE())) THEN ISNULL(Late_Fee,0) ELSE 0 END )-Discount - Paid_Amount != 0 ");

                studentInfo = GetDataTable($"SELECT * FROM Students " +
                    $"LEFT JOIN Parents ON Parents.Parent_ID = Students.Parent_ID " +
                    $"LEFT JOIN Classes ON Classes.Class_ID = Students.Class_ID " +
                    $"LEFT JOIN Sections ON Sections.Section_ID = Students.Section_ID " +
                    $"WHERE Students.Student_ID = {Student_ID} AND Students.Is_Active = 1");
                }

                var idName = GetStudentID();

                FixedDocument fixDoc = new FixedDocument();

                foreach (DataRow row in studentInfo.Rows)
                {
                    List<DesignerControls> frontCardItems = new List<DesignerControls>();
                    List<DesignerControls> backCardItems = new List<DesignerControls>();

                    var studentFeeRecord = feeRecord.AsEnumerable().Where(x => x["Student_ID"].ToString() == row["Student_ID"].ToString()).Select(x => x).CopyToDataTable();

                #region FOR FRONT VOUCHER
                {
                        foreach (var cardItem in FrontVoucherItems)
                        {
                            TextControl TextBlock = cardItem as TextControl;
                            if (TextBlock != null)
                            {
                                frontCardItems.Add(new TextControl
                                {
                                    CanvasLeft = TextBlock.CanvasLeft,
                                    CanvasTop = TextBlock.CanvasTop,
                                    FontSize = TextBlock.FontSize,
                                    FontStyle = TextBlock.FontStyle,
                                    FontWeight = TextBlock.FontWeight,
                                    Foreground = TextBlock.Foreground,
                                    Height = TextBlock.Height,
                                    Width = TextBlock.Width,
                                    IsHitTestVisible = true,
                                    IsSelected = false,
                                    TextAlignment = TextBlock.TextAlignment,
                                    TextDecoration = TextBlock.TextDecoration,
                                    Value = TextBlock.Value == idName ? row[$"Student_ID"].ToString() : TextBlock.Value == "Voucher ID" ? row[$"Student_ID"].ToString() : row[$"{cardItem.Value}"].ToString()
                                });

                            }
                            ImageControl Image = cardItem as ImageControl;
                            if (Image != null)
                            {
                                var image = new ImageControl
                                {
                                    CanvasLeft = Image.CanvasLeft,
                                    CanvasTop = Image.CanvasTop,
                                    Height = Image.Height,
                                    Width = Image.Width,
                                    IsSelected = false,
                                    IsHitTestVisible = true,
                                };
                                BitmapImage img = Helper.ByteArrayToImage(row[$"{Image.Value}"] as byte[]);
                                if (img != null)
                                {
                                    img.Freeze();
                                    image.Image = img;
                                    frontCardItems.Add(image);
                                }
                            }
                            QRCodeControl QrCode = cardItem as QRCodeControl;
                            if (QrCode != null)
                            {
                                var image = new QRCodeControl
                                {
                                    CanvasLeft = QrCode.CanvasLeft,
                                    CanvasTop = QrCode.CanvasTop,
                                    Height = QrCode.Height,
                                    Width = QrCode.Width,
                                    IsSelected = false,
                                    IsHitTestVisible = true,
                                };
                                BitmapImage img = Helper.GetQRCode($"STD-{row["Student_ID"].ToString()}");
                                if (img != null)
                                {
                                    img.Freeze();
                                    image.Image = img;
                                    frontCardItems.Add(image);
                                }
                            }
                            FeeControl fee = cardItem as FeeControl;
                            if (fee != null)
                            {
                                FeeControl feeItem = new FeeControl
                                {
                                    CanvasLeft = fee.CanvasLeft,
                                    CanvasTop = fee.CanvasTop,
                                    FontSize = fee.FontSize,
                                    FontStyle = fee.FontStyle,
                                    FontWeight = fee.FontWeight,
                                    Foreground = fee.Foreground,
                                    Height = fee.Height,
                                    Width = fee.Width,
                                    IsHitTestVisible = true,
                                    IsSelected = false,
                                    TextAlignment = fee.TextAlignment,
                                    TextDecoration = fee.TextDecoration,
                                    FeeAttribite = fee.FeeAttribite,
                                };
                                if (cardItem.Value == "TOTAL FEE" || cardItem.Value == "TOTAL DISCOUNT"
                                    || cardItem.Value == "GRAND TOTAL" || cardItem.Value == "TOTAL LATE FEE"
                                    || cardItem.Value == "DUES" || cardItem.Value == "GRAND SUM")
                                {
                                feeItem.Value = cardItem.Value;
                            }
                                else if (cardItem.Value == "Due Date")
                                {
                                    var date = studentFeeRecord.AsEnumerable().
                                        Where(x => x["Fee"].ToString() == fee.Fee && x["Month"].ToString() == fee.Month && x["Year"].ToString() == fee.Year).
                                        Select(x => x[fee.Value].ToString()).FirstOrDefault();
                                    if (date.IsNotNullOrEmpty())
                                        feeItem.Value = DateTime.Parse(date).ToString("dd-MMM-yyyy");
                                }
                                else
                                {
                                    if (cardItem.Value == "Discount" || cardItem.Value == "Late Fee" || cardItem.Value == "Total")
                                        feeItem.Value = studentFeeRecord.AsEnumerable().
                                        Where(x => x["Fee"].ToString() == fee.Fee && x["Month"].ToString() == fee.Month && x["Year"].ToString() == fee.Year).
                                            Select(x => x[cardItem.Value].ToString()).FirstOrDefault();
                                    else
                                        feeItem.Value = studentFeeRecord.AsEnumerable().
                                        Where(x => x["Fee"].ToString() == fee.Fee && x["Month"].ToString() == fee.Month && x["Year"].ToString() == fee.Year).
                                            Select(x => x["Amount"].ToString()).FirstOrDefault();

                                }
                                frontCardItems.Add(feeItem);
                            }
                        }

                        //to find the sums of all the present fee , late fee, discount ...
                        int totalFee = studentFeeRecord.AsEnumerable().Select(x => (int)x["Amount"]).Sum();
                        int totalLateFee = studentFeeRecord.AsEnumerable().Select(x => (int)x["Late Fee"]).Sum();
                        int totalDiscount = studentFeeRecord.AsEnumerable().Select(x => (int)x["Discount"]).Sum();
                        int dues = previousDues.AsEnumerable().Where(x => x["Student_ID"].ToString() == row["Student_ID"].ToString()).Select(x => (int)x["Dues"]).FirstOrDefault();
                        int grandTotal = totalFee - totalDiscount;
                        int grandSum = totalFee + dues - totalDiscount;
                        foreach (var item in frontCardItems)
                        {
                            var fee = item as FeeControl;
                            if (fee != null)
                            {
                                if (fee.Value == "TOTAL FEE")
                                    fee.Value = totalFee.ToString();
                                else if (fee.Value == "TOTAL LATE FEE")
                                    fee.Value = totalLateFee.ToString();
                                else if (fee.Value == "TOTAL DISCOUNT")
                                    fee.Value = totalDiscount.ToString();
                                else if (fee.Value == "GRAND TOTAL")
                                    fee.Value = grandTotal.ToString();
                                else if (fee.Value == "DUES")
                                    fee.Value = dues.ToString();
                                else if (fee.Value == "GRAND SUM")
                                    fee.Value = grandSum.ToString();
                            }
                        }

                }
                    #endregion

                    #region FOR BACK VOCUHER

                    if (IsBackVoucherAdded)
                    {
                        foreach (var cardItem in BackVoucherItems)
                        {
                            TextControl TextBlock = cardItem as TextControl;
                            if (TextBlock != null)
                            {
                                backCardItems.Add(new TextControl
                                {
                                    CanvasLeft = TextBlock.CanvasLeft,
                                    CanvasTop = TextBlock.CanvasTop,
                                    FontSize = TextBlock.FontSize,
                                    FontStyle = TextBlock.FontStyle,
                                    FontWeight = TextBlock.FontWeight,
                                    Foreground = TextBlock.Foreground,
                                    Height = TextBlock.Height,
                                    Width = TextBlock.Width,
                                    IsHitTestVisible = true,
                                    IsSelected = false,
                                    TextAlignment = TextBlock.TextAlignment,
                                    TextDecoration = TextBlock.TextDecoration,
                                    Value = TextBlock.Value == idName ? row[$"Student_ID"].ToString() : TextBlock.Value == "Voucher ID" ? row[$"Student_ID"].ToString() : row[$"{cardItem.Value}"].ToString()
                                });

                            }
                            ImageControl Image = cardItem as ImageControl;
                            if (Image != null)
                            {
                                var image = new ImageControl
                                {
                                    CanvasLeft = Image.CanvasLeft,
                                    CanvasTop = Image.CanvasTop,
                                    Height = Image.Height,
                                    Width = Image.Width,
                                    IsSelected = false,
                                    IsHitTestVisible = true,
                                };
                                BitmapImage img = Helper.ByteArrayToImage(row[$"{Image.Value}"] as byte[]);
                                if (img != null)
                                {
                                    img.Freeze();
                                    image.Image = img;
                                    backCardItems.Add(image);
                                }
                            }
                            QRCodeControl QrCode = cardItem as QRCodeControl;
                            if (QrCode != null)
                            {
                                var image = new QRCodeControl
                                {
                                    CanvasLeft = QrCode.CanvasLeft,
                                    CanvasTop = QrCode.CanvasTop,
                                    Height = QrCode.Height,
                                    Width = QrCode.Width,
                                    IsSelected = false,
                                    IsHitTestVisible = true,
                                };
                                BitmapImage img = Helper.GetQRCode($"STD-{row["Student_ID"].ToString()}");
                                if (img != null)
                                {
                                    img.Freeze();
                                    image.Image = img;
                                    backCardItems.Add(image);
                                }
                            }
                            FeeControl fee = cardItem as FeeControl;
                            if (fee != null)
                            {
                                FeeControl feeItem = new FeeControl
                                {
                                    CanvasLeft = fee.CanvasLeft,
                                    CanvasTop = fee.CanvasTop,
                                    FontSize = fee.FontSize,
                                    FontStyle = fee.FontStyle,
                                    FontWeight = fee.FontWeight,
                                    Foreground = fee.Foreground,
                                    Height = fee.Height,
                                    Width = fee.Width,
                                    IsHitTestVisible = true,
                                    IsSelected = false,
                                    TextAlignment = fee.TextAlignment,
                                    TextDecoration = fee.TextDecoration,
                                    FeeAttribite = fee.FeeAttribite,
                                    Value = cardItem.Value
                                };
                                if (cardItem.Value == "TOTAL FEE" || cardItem.Value == "TOTAL DISCOUNT"
                                    || cardItem.Value == "GRAND TOTAL" || cardItem.Value == "TOTAL LATE FEE"
                                    || cardItem.Value == "DUES" || cardItem.Value == "GRAND SUM")
                                { }
                                else if (cardItem.Value == "Due Date")
                                {
                                    var date = feeRecord.AsEnumerable().
                                        Where(x => x["Fee"].ToString() == fee.Fee && x["Month"].ToString() == fee.Month && x["Year"].ToString() == fee.Year).
                                        Select(x => x["Due_Date"].ToString()).First();
                                    if (date.IsNotNullOrEmpty())
                                        feeItem.Value = DateTime.Parse(date).ToString("dd-MMM-yyyy");
                                }
                                else
                                {
                                    if (cardItem.Value == "Discount" || cardItem.Value == "Late Fee" || cardItem.Value == "Total")
                                        feeItem.Value = feeRecord.AsEnumerable().
                                        Where(x => x["Fee"].ToString() == fee.Fee && x["Month"].ToString() == fee.Month && x["Year"].ToString() == fee.Year).
                                            Select(x => x[cardItem.Value].ToString()).FirstOrDefault();
                                    else
                                        feeItem.Value = feeRecord.AsEnumerable().
                                        Where(x => x["Fee"].ToString() == fee.Fee && x["Month"].ToString() == fee.Month && x["Year"].ToString() == fee.Year).
                                            Select(x => x["Amount"].ToString()).FirstOrDefault();

                                }
                                backCardItems.Add(feeItem);
                            }
                        }

                        //to find the sums of all the present fee , late fee, discount ...
                        int totalFee = studentFeeRecord.AsEnumerable().Select(x => (int)x["Amount"]).Sum();
                        int totalLateFee = studentFeeRecord.AsEnumerable().Select(x => (int)x["Late Fee"]).Sum();
                        int totalDiscount = studentFeeRecord.AsEnumerable().Select(x => (int)x["Discount"]).Sum();
                        int dues = previousDues.AsEnumerable().Where(x => x["Student_ID"].ToString() == row["Student_ID"].ToString()).Select(x => (int)x["Dues"]).FirstOrDefault();
                        int grandTotal = totalFee - totalDiscount;
                        int grandSum = totalFee + dues - totalDiscount;
                        foreach (var item in backCardItems)
                        {
                            var fee = item as FeeControl;
                            if (fee != null)
                            {
                                if (fee.Value == "TOTAL FEE")
                                    fee.Value = totalFee.ToString();
                                else if (fee.Value == "TOTAL LATE FEE")
                                    fee.Value = totalLateFee.ToString();
                                else if (fee.Value == "TOTAL DISCOUNT")
                                    fee.Value = totalDiscount.ToString();
                                else if (fee.Value == "GRAND TOTAL")
                                    fee.Value = grandTotal.ToString();
                                else if (fee.Value == "DUES")
                                    fee.Value = dues.ToString();
                                else if (fee.Value == "GRAND SUM")
                                    fee.Value = grandSum.ToString();
                            }
                        }
                    }

                    #endregion

                    StackPanel stackpanel = new StackPanel();
                    stackpanel.Children.Add(new CardDesigner
                    {
                        ItemSource = new ObservableCollection<DesignerControls>(frontCardItems),
                        BackgroundImage = FrontCardBackground,
                        Width = CardWidth * 96,
                        Height = CardHeight * 96
                    });
                    if (IsBackVoucherAdded)
                    {
                        stackpanel.Children.Add(new CardDesigner
                        {
                            ItemSource = new ObservableCollection<DesignerControls>(backCardItems),
                            BackgroundImage = BackCardBackground,
                            Width = CardWidth * 96,
                            Height = CardHeight * 96
                        });
                    }

                    PageContent PgContent1 = new PageContent();
                    FixedPage fixPg1 = new FixedPage();
                    if (IsBackVoucherAdded)
                        fixPg1.Height = CardHeight * 96 * 2;
                    else
                        fixPg1.Height = CardHeight * 96;

                    fixPg1.Width = CardWidth * 96;
                    fixPg1.Children.Add(stackpanel);
                    ((IAddChild)PgContent1).AddChild(fixPg1);
                    fixDoc.Pages.Add(PgContent1);
                }

                //Now Write the document
                XpsDocument xpsd = new XpsDocument(FileName, FileAccess.ReadWrite);
                XpsDocumentWriter xw = XpsDocument.CreateXpsDocumentWriter(xpsd);
                xw.Write(fixDoc);
                xpsd.Close();
            //});
            //////make thread a UI thread
            //thread.SetApartmentState(ApartmentState.STA);
            ////start the thread
            //thread.Start();
            ////helps to wait for the task of this thread to finish before it returns to the main thread
            //thread.Join();
        }

        public static DataTable GetCollectedStatus(int month, int year)
        {
            DataTable table = new DataTable();
            
            using (SqlConnection sqlConn = new SqlConnection(connectionString))
            {
                sqlConn.Open();
                SqlCommand sqlCmd = new SqlCommand("GetCollectionStatus", sqlConn);
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Parameters.Add("@month", SqlDbType.Int).Value = month;
                sqlCmd.Parameters.Add("@year", SqlDbType.Int).Value = year;
                SqlDataAdapter sqlDa = new SqlDataAdapter(sqlCmd);
                sqlDa.Fill(table);
            }
            return table;
        }

        public static DataTable GetFeeCollections(int month, int year)
        {
            DataTable table = new DataTable();
            
            using (SqlConnection sqlConn = new SqlConnection(connectionString))
            {
                sqlConn.Open();
                SqlCommand sqlCmd = new SqlCommand("GetFeeCollections", sqlConn);
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Parameters.Add("@month", SqlDbType.Int).Value = month;
                sqlCmd.Parameters.Add("@year", SqlDbType.Int).Value = year;
                SqlDataAdapter sqlDa = new SqlDataAdapter(sqlCmd);
                sqlDa.Fill(table);
            }
            return table;
        }

        public static void SaveVoucherTemplate(VoucherTemplate Voucher, string VoucherName, string ClassID)
        {
            var desginerID = GetNextID("Designer");
            ExecuteQuery($"INSERT INTO Designer ([Designer_Name] , [Designer_Type] , [Class_ID]) VALUES ('{VoucherName}' , 1 , {ClassID})");

            var frontItemID = GetNextID("DesignerProperties");
            ExecuteQuery($"INSERT INTO DesignerProperties ([Designer_ID] , [Property_Name] ) VALUES ('{desginerID}' , '{nameof(Voucher.FrontItems)}')");
            var backItemID = GetNextID("DesignerProperties");
            ExecuteQuery($"INSERT INTO DesignerProperties ([Designer_ID] , [Property_Name] ) VALUES ('{desginerID}' , '{nameof(Voucher.BackItems)}')");

            
            using (SqlConnection sqlConn = new SqlConnection(connectionString))
            {
                sqlConn.Open();
                SqlCommand sqlCmd = new SqlCommand();
                sqlCmd.Connection = sqlConn;
                byte[] image1 = Helper.ImageToByteArray(Voucher.FrontImage);
                byte[] image2 = Helper.ImageToByteArray(Voucher.BackImage);
                if (image1 != null)
                    sqlCmd.Parameters.Add("@image1", SqlDbType.VarBinary).Value = image1;
                if (image2 != null)
                    sqlCmd.Parameters.Add("@image2", SqlDbType.VarBinary).Value = image2;
                sqlCmd.CommandText = $"INSERT INTO DesignerProperties ([Designer_ID] , [Property_Name] , [Image]) VALUES ('{desginerID}' , '{nameof(Voucher.FrontImage)}' , {(image1 == null ? "NULL" : "@image1")} )   " +
                    $" INSERT INTO DesignerProperties ([Designer_ID] , [Property_Name] , [Image]) VALUES ('{desginerID}' , '{nameof(Voucher.BackImage)}' , {(image2 == null ? "NULL" : "@image2")} )  ";
                sqlCmd.ExecuteNonQuery();
            }
            ExecuteQuery($"INSERT INTO DesignerProperties ([Designer_ID] , [Property_Name] , [Designer_Property_Value]) VALUES ('{desginerID}' , '{nameof(Voucher.IsBackAdded)}' , '{Voucher.IsBackAdded}')");
            ExecuteQuery($"INSERT INTO DesignerProperties ([Designer_ID] , [Property_Name] , [Designer_Property_Value]) VALUES ('{desginerID}' , '{nameof(Voucher.Height)}' , '{Voucher.Height}')");
            ExecuteQuery($"INSERT INTO DesignerProperties ([Designer_ID] , [Property_Name] , [Designer_Property_Value]) VALUES ('{desginerID}' , '{nameof(Voucher.Width)}' , '{Voucher.Width}')");

            foreach (var item in Voucher.FrontItems)
            {
                var itemID = GetNextID("DesignerItem");
                ExecuteQuery($"INSERT INTO DesignerItem ([Designer_Property_ID] , [Type] ) VALUES ('{frontItemID}' , '{item.GetType().Name}')");

                //insert designer control properties that are common in all items
                ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(item.CanvasLeft)}' , '{item.CanvasLeft}' , '{item.CanvasLeft.GetType()}')");
                ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(item.CanvasTop)}' , '{item.CanvasTop}' , '{item.CanvasTop.GetType()}')");
                ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(item.Width)}' , '{item.Width}' , '{item.Width.GetType()}')");
                ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(item.Height)}' , '{item.Height}' , '{item.Height.GetType()}')");
                ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(item.Value)}' , '{item.Value}' , '{item.Value.GetType()}')");

                switch (item.GetType().Name)
                {
                    case nameof(TextControl):
                        {
                            var text = item as TextControl;
                            ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(text.Foreground)}' , '{((SolidColorBrush)text.Foreground).Color.ToString()}' , '{text.Foreground.GetType()}')");
                            ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(text.Background)}' , '{((SolidColorBrush)text.Background).Color.ToString()}' , '{text.Background.GetType()}')");
                            ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(text.FontSize)}' , '{text.FontSize}' , '{text.FontSize.GetType()}')");
                            ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(text.FontStyle)}' , '{text.FontStyle}' , '{text.FontStyle.GetType()}')");
                            ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(text.FontWeight)}' , '{text.FontWeight}' , '{text.FontWeight.GetType()}')");
                            ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(text.TextAlignment)}' , '{text.TextAlignment}' , '{text.TextAlignment.GetType()}')");
                            break;
                        }
                    case nameof(ImageControl):
                        {
                            var image = item as ImageControl;
                            ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(image.Image)}' , 'image' , '{image.Image.GetType()}')");
                            break;
                        }
                    case nameof(QRCodeControl):
                        {
                            var qrCode = item as QRCodeControl;
                            ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(qrCode.Image)}' , 'image' , '{qrCode.Image.GetType()}')");
                            break;
                        }
                    case nameof(FeeControl):
                        {
                            var fee = item as FeeControl;
                            ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(fee.Foreground)}' , '{((SolidColorBrush)fee.Foreground).Color.ToString()}' , '{fee.Foreground.GetType()}')");
                            ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(fee.Background)}' , '{((SolidColorBrush)fee.Background).Color.ToString()}' , '{fee.Background.GetType()}')");
                            ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(fee.FontSize)}' , '{fee.FontSize}' , '{fee.FontSize.GetType()}')");
                            ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(fee.FontStyle)}' , '{fee.FontStyle}' , '{fee.FontStyle.GetType()}')");
                            ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(fee.FontWeight)}' , '{fee.FontWeight}' , '{fee.FontWeight.GetType()}')");
                            ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(fee.TextAlignment)}' , '{fee.TextAlignment}' , '{fee.TextAlignment.GetType()}')");
                            ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(fee.Month)}' , '{fee.Month ?? "NULL"}' , '{(fee.Month == null ? "NULL" : fee.Month.GetType().ToString())}')");
                            ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(fee.Year)}' , '{fee.Year ?? "NULL"}' , '{(fee.Year == null ? "NULL" : fee.Year.GetType().ToString())}')");
                            ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(fee.Fee)}' , '{fee.Fee ?? "NULL"}' , '{(fee.Fee == null ? "NULL" : fee.Fee.GetType().ToString())}')");
                            break;
                        }
                    default:
                        break;
                }
            }
            foreach (var item in Voucher.BackItems)
            {
                var itemID = GetNextID("DesignerItem");
                ExecuteQuery($"INSERT INTO DesignerItem ([Designer_Property_ID] , [Type] ) VALUES ('{frontItemID}' , '{item.GetType().Name}')");

                //insert designer control properties that are common in all items
                ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(item.CanvasLeft)}' , '{item.CanvasLeft}' , '{item.CanvasLeft.GetType()}')");
                ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(item.CanvasTop)}' , '{item.CanvasTop}' , '{item.CanvasTop.GetType()}')");
                ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(item.Width)}' , '{item.Width}' , '{item.Width.GetType()}')");
                ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(item.Height)}' , '{item.Height}' , '{item.Height.GetType()}')");
                ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(item.Value)}' , '{item.Value}' , '{item.Value.GetType()}')");

                switch (item.GetType().Name)
                {
                    case nameof(TextControl):
                        {
                            var text = item as TextControl;
                            ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(text.Foreground)}' , '{((SolidColorBrush)text.Foreground).Color.ToString()}' , '{text.Foreground.GetType()}')");
                            ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(text.Background)}' , '{((SolidColorBrush)text.Background).Color.ToString()}' , '{text.Background.GetType()}')");
                            ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(text.FontSize)}' , '{text.FontSize}' , '{text.FontSize.GetType()}')");
                            ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(text.FontStyle)}' , '{text.FontStyle}' , '{text.FontStyle.GetType()}')");
                            ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(text.FontWeight)}' , '{text.FontWeight}' , '{text.FontWeight.GetType()}')");
                            ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(text.TextAlignment)}' , '{text.TextAlignment}' , '{text.TextAlignment.GetType()}')");
                            ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(text.TextDecoration)}' , '{text.TextDecoration}' , '{text.TextDecoration.GetType()}')");
                            break;
                        }
                    case nameof(ImageControl):
                        {
                            var image = item as ImageControl;
                            ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(image.Image)}' , 'image' , '{image.Image.GetType()}')");
                            break;
                        }
                    case nameof(QRCodeControl):
                        {
                            var qrCode = item as QRCodeControl;
                            ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(qrCode.Image)}' , 'image' , '{qrCode.Image.GetType()}')");
                            break;
                        }
                    case nameof(FeeControl):
                        {
                            var fee = item as FeeControl;
                            ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(fee.Foreground)}' , '{((SolidColorBrush)fee.Foreground).Color.ToString()}' , '{fee.Foreground.GetType()}')");
                            ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(fee.Background)}' , '{((SolidColorBrush)fee.Background).Color.ToString()}' , '{fee.Background.GetType()}')");
                            ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(fee.FontSize)}' , '{fee.FontSize}' , '{fee.FontSize.GetType()}')");
                            ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(fee.FontStyle)}' , '{fee.FontStyle}' , '{fee.FontStyle.GetType()}')");
                            ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(fee.FontWeight)}' , '{fee.FontWeight}' , '{fee.FontWeight.GetType()}')");
                            ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(fee.TextAlignment)}' , '{fee.TextAlignment}' , '{fee.TextAlignment.GetType()}')");
                            ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(fee.Month)}' , '{fee.Month ?? "NULL"}' , '{(fee.Month == null ? "NULL" : fee.Month.GetType().ToString())}')");
                            ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(fee.Year)}' , '{fee.Year ?? "NULL"}' , '{(fee.Year == null ? "NULL" : fee.Year.GetType().ToString())}')");
                            ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(fee.Fee)}' , '{fee.Fee ?? "NULL"}' , '{(fee.Fee == null ? "NULL" : fee.Fee.GetType().ToString())}')");
                            break;
                        }
                    default:
                        break;
                }
            }
        }

        public static void SaveCardTemplate(VoucherTemplate Voucher, string VoucherName)
        {
            var desginerID = GetNextID("Designer");
            ExecuteQuery($"INSERT INTO Designer ([Designer_Name] , [Designer_Type] ) VALUES ('{VoucherName}' , 2)");

            var frontItemID = GetNextID("DesignerProperties");
            ExecuteQuery($"INSERT INTO DesignerProperties ([Designer_ID] , [Property_Name] ) VALUES ('{desginerID}' , '{nameof(Voucher.FrontItems)}')");
            var backItemID = GetNextID("DesignerProperties");
            ExecuteQuery($"INSERT INTO DesignerProperties ([Designer_ID] , [Property_Name] ) VALUES ('{desginerID}' , '{nameof(Voucher.BackItems)}')");

            
            using (SqlConnection sqlConn = new SqlConnection(connectionString))
            {
                sqlConn.Open();
                SqlCommand sqlCmd = new SqlCommand();
                sqlCmd.Connection = sqlConn;
                byte[] image1 = Helper.ImageToByteArray(Voucher.FrontImage);
                byte[] image2 = Helper.ImageToByteArray(Voucher.BackImage);
                if (image1 != null)
                    sqlCmd.Parameters.Add("@image1", SqlDbType.VarBinary).Value = image1;
                if (image2 != null)
                    sqlCmd.Parameters.Add("@image2", SqlDbType.VarBinary).Value = image2;
                sqlCmd.CommandText = $"INSERT INTO DesignerProperties ([Designer_ID] , [Property_Name] , [Image]) VALUES ('{desginerID}' , '{nameof(Voucher.FrontImage)}' , {(image1 == null ? "NULL" : "@image1")} )   " +
                    $" INSERT INTO DesignerProperties ([Designer_ID] , [Property_Name] , [Image]) VALUES ('{desginerID}' , '{nameof(Voucher.BackImage)}' , {(image2 == null ? "NULL" : "@image2")} )  ";
                sqlCmd.ExecuteNonQuery();
            }
            ExecuteQuery($"INSERT INTO DesignerProperties ([Designer_ID] , [Property_Name] , [Designer_Property_Value]) VALUES ('{desginerID}' , '{nameof(Voucher.IsBackAdded)}' , '{Voucher.IsBackAdded}')");
            ExecuteQuery($"INSERT INTO DesignerProperties ([Designer_ID] , [Property_Name] , [Designer_Property_Value]) VALUES ('{desginerID}' , '{nameof(Voucher.Height)}' , '{Voucher.Height}')");
            ExecuteQuery($"INSERT INTO DesignerProperties ([Designer_ID] , [Property_Name] , [Designer_Property_Value]) VALUES ('{desginerID}' , '{nameof(Voucher.Width)}' , '{Voucher.Width}')");

            foreach (var item in Voucher.FrontItems)
            {
                var itemID = GetNextID("DesignerItem");
                ExecuteQuery($"INSERT INTO DesignerItem ([Designer_Property_ID] , [Type] ) VALUES ('{frontItemID}' , '{item.GetType().Name}')");

                //insert designer control properties that are common in all items
                ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(item.CanvasLeft)}' , '{item.CanvasLeft}' , '{item.CanvasLeft.GetType()}')");
                ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(item.CanvasTop)}' , '{item.CanvasTop}' , '{item.CanvasTop.GetType()}')");
                ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(item.Width)}' , '{item.Width}' , '{item.Width.GetType()}')");
                ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(item.Height)}' , '{item.Height}' , '{item.Height.GetType()}')");
                ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(item.Value)}' , '{item.Value}' , '{item.Value.GetType()}')");

                switch (item.GetType().Name)
                {
                    case nameof(TextControl):
                        {
                            var text = item as TextControl;
                            ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(text.Foreground)}' , '{((SolidColorBrush)text.Foreground).Color.ToString()}' , '{text.Foreground.GetType()}')");
                            ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(text.Background)}' , '{((SolidColorBrush)text.Background).Color.ToString()}' , '{text.Background.GetType()}')");
                            ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(text.FontSize)}' , '{text.FontSize}' , '{text.FontSize.GetType()}')");
                            ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(text.FontStyle)}' , '{text.FontStyle}' , '{text.FontStyle.GetType()}')");
                            ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(text.FontWeight)}' , '{text.FontWeight}' , '{text.FontWeight.GetType()}')");
                            ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(text.TextAlignment)}' , '{text.TextAlignment}' , '{text.TextAlignment.GetType()}')");
                            break;
                        }
                    case nameof(ImageControl):
                        {
                            var image = item as ImageControl;
                            ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(image.Image)}' , 'image' , '{image.Image.GetType()}')");
                            break;
                        }
                    case nameof(QRCodeControl):
                        {
                            var qrCode = item as QRCodeControl;
                            ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(qrCode.Image)}' , 'image' , '{qrCode.Image.GetType()}')");
                            break;
                        }
                    case nameof(FeeControl):
                        {
                            var fee = item as FeeControl;
                            ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(fee.Foreground)}' , '{((SolidColorBrush)fee.Foreground).Color.ToString()}' , '{fee.Foreground.GetType()}')");
                            ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(fee.Background)}' , '{((SolidColorBrush)fee.Background).Color.ToString()}' , '{fee.Background.GetType()}')");
                            ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(fee.FontSize)}' , '{fee.FontSize}' , '{fee.FontSize.GetType()}')");
                            ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(fee.FontStyle)}' , '{fee.FontStyle}' , '{fee.FontStyle.GetType()}')");
                            ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(fee.FontWeight)}' , '{fee.FontWeight}' , '{fee.FontWeight.GetType()}')");
                            ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(fee.TextAlignment)}' , '{fee.TextAlignment}' , '{fee.TextAlignment.GetType()}')");
                            ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(fee.Month)}' , '{fee.Month ?? "NULL"}' , '{(fee.Month == null ? "NULL" : fee.Month.GetType().ToString())}')");
                            ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(fee.Year)}' , '{fee.Year ?? "NULL"}' , '{(fee.Year == null ? "NULL" : fee.Year.GetType().ToString())}')");
                            ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(fee.Fee)}' , '{fee.Fee ?? "NULL"}' , '{(fee.Fee == null ? "NULL" : fee.Fee.GetType().ToString())}')");
                            break;
                        }
                    default:
                        break;
                }
            }
            foreach (var item in Voucher.BackItems)
            {
                var itemID = GetNextID("DesignerItem");
                ExecuteQuery($"INSERT INTO DesignerItem ([Designer_Property_ID] , [Type] ) VALUES ('{frontItemID}' , '{item.GetType().Name}')");

                //insert designer control properties that are common in all items
                ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(item.CanvasLeft)}' , '{item.CanvasLeft}' , '{item.CanvasLeft.GetType()}')");
                ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(item.CanvasTop)}' , '{item.CanvasTop}' , '{item.CanvasTop.GetType()}')");
                ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(item.Width)}' , '{item.Width}' , '{item.Width.GetType()}')");
                ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(item.Height)}' , '{item.Height}' , '{item.Height.GetType()}')");
                ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(item.Value)}' , '{item.Value}' , '{item.Value.GetType()}')");

                switch (item.GetType().Name)
                {
                    case nameof(TextControl):
                        {
                            var text = item as TextControl;
                            ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(text.Foreground)}' , '{((SolidColorBrush)text.Foreground).Color.ToString()}' , '{text.Foreground.GetType()}')");
                            ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(text.Background)}' , '{((SolidColorBrush)text.Background).Color.ToString()}' , '{text.Background.GetType()}')");
                            ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(text.FontSize)}' , '{text.FontSize}' , '{text.FontSize.GetType()}')");
                            ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(text.FontStyle)}' , '{text.FontStyle}' , '{text.FontStyle.GetType()}')");
                            ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(text.FontWeight)}' , '{text.FontWeight}' , '{text.FontWeight.GetType()}')");
                            ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(text.TextAlignment)}' , '{text.TextAlignment}' , '{text.TextAlignment.GetType()}')");
                            ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(text.TextDecoration)}' , '{text.TextDecoration}' , '{text.TextDecoration.GetType()}')");
                            break;
                        }
                    case nameof(ImageControl):
                        {
                            var image = item as ImageControl;
                            ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(image.Image)}' , 'image' , '{image.Image.GetType()}')");
                            break;
                        }
                    case nameof(QRCodeControl):
                        {
                            var qrCode = item as QRCodeControl;
                            ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(qrCode.Image)}' , 'image' , '{qrCode.Image.GetType()}')");
                            break;
                        }
                    case nameof(FeeControl):
                        {
                            var fee = item as FeeControl;
                            ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(fee.Foreground)}' , '{((SolidColorBrush)fee.Foreground).Color.ToString()}' , '{fee.Foreground.GetType()}')");
                            ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(fee.Background)}' , '{((SolidColorBrush)fee.Background).Color.ToString()}' , '{fee.Background.GetType()}')");
                            ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(fee.FontSize)}' , '{fee.FontSize}' , '{fee.FontSize.GetType()}')");
                            ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(fee.FontStyle)}' , '{fee.FontStyle}' , '{fee.FontStyle.GetType()}')");
                            ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(fee.FontWeight)}' , '{fee.FontWeight}' , '{fee.FontWeight.GetType()}')");
                            ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(fee.TextAlignment)}' , '{fee.TextAlignment}' , '{fee.TextAlignment.GetType()}')");
                            ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(fee.Month)}' , '{fee.Month ?? "NULL"}' , '{(fee.Month == null ? "NULL" : fee.Month.GetType().ToString())}')");
                            ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(fee.Year)}' , '{fee.Year ?? "NULL"}' , '{(fee.Year == null ? "NULL" : fee.Year.GetType().ToString())}')");
                            ExecuteQuery($"INSERT INTO DesignerItemProperties ([Designer_Item_ID] , [Name] , [Value] , [DataType] ) VALUES ('{itemID}' , '{nameof(fee.Fee)}' , '{fee.Fee ?? "NULL"}' , '{(fee.Fee == null ? "NULL" : fee.Fee.GetType().ToString())}')");
                            break;
                        }
                    default:
                        break;
                }
            }
        }

        public static VoucherTemplate GetCardTemplate(string CardName)
        {
            VoucherTemplate voucher = new VoucherTemplate();
            var table = GetDataTable($"SELECT * FROM Designer LEFT JOIN " +
                $"DesignerProperties ON DesignerProperties.Designer_ID = Designer.Designer_ID " +
                $"LEFT JOIN DesignerItem ON DesignerItem.[Designer_Property_ID] = DesignerProperties.Designer_Property_ID " +
                $"LEFT JOIN DesignerItemProperties ON DesignerItemProperties.Designer_Item_ID = DesignerItem.[Designer_Item_ID] " +
                $"WHERE Designer.Designer_Name = '{CardName}' AND [Designer_Type] = 2 ");

            if (table.Rows.Count == 0)
                return new VoucherTemplate();

            var frontItems = table.AsEnumerable().Where(x => x["Property_Name"].ToString() == "FrontItems").Select(x => x).ToList();
            var backItems = table.AsEnumerable().Where(x => x["Property_Name"].ToString() == "BackItems").Select(x => x).ToList();
            var frontItemID = frontItems.Select(x => x["Designer_Item_ID"].ToString()).Distinct().ToList();
            var backItemID = backItems.Select(x => x["Designer_Item_ID"].ToString()).Distinct().ToList();
            voucher.FrontItems = GetDesignerItems(frontItems, frontItemID);
            voucher.BackItems = GetDesignerItems(backItems, backItemID);
            var frontImage = table.AsEnumerable().Where(x => x["Property_Name"].ToString() == "FrontImage").Select(x => x["Image"]).FirstOrDefault();
            if (frontImage != DBNull.Value)
                voucher.FrontImage = Helper.ByteArrayToImage((byte[])frontImage);
            var backImage = table.AsEnumerable().Where(x => x["Property_Name"].ToString() == "BackImage").Select(x => x["Image"]).FirstOrDefault();
            if (backImage != DBNull.Value)
                voucher.BackImage = Helper.ByteArrayToImage((byte[])backImage);
            voucher.Height = table.AsEnumerable().Where(x => x["Property_Name"].ToString() == "Height").Select(x => x["Designer_Property_Value"].ToString()).FirstOrDefault();
            voucher.Width = table.AsEnumerable().Where(x => x["Property_Name"].ToString() == "Width").Select(x => x["Designer_Property_Value"].ToString()).FirstOrDefault();
            if(table.AsEnumerable().Where(x => x["Property_Name"].ToString() == "IsBackAdded").Select(x => x["Designer_Property_Value"].ToString()).FirstOrDefault() != null)
                voucher.IsBackAdded = Boolean.Parse(table.AsEnumerable().Where(x => x["Property_Name"].ToString() == "IsBackAdded").Select(x => x["Designer_Property_Value"].ToString()).FirstOrDefault());

            return voucher;
        }

        public static VoucherTemplate GetsVoucherTemplate(string VoucherName)
        {
            VoucherTemplate voucher = new VoucherTemplate();
            var table = GetDataTable($"SELECT * FROM Designer LEFT JOIN " +
                $"DesignerProperties ON DesignerProperties.Designer_ID = Designer.Designer_ID " +
                $"LEFT JOIN DesignerItem ON DesignerItem.[Designer_Property_ID] = DesignerProperties.Designer_Property_ID " +
                $"LEFT JOIN DesignerItemProperties ON DesignerItemProperties.Designer_Item_ID = DesignerItem.[Designer_Item_ID] " +
                $"WHERE Designer.Designer_Name = '{VoucherName}'  AND [Designer_Type] = 1");

            if (table.Rows.Count == 0)
                return new VoucherTemplate();

            var frontItems = table.AsEnumerable().Where(x => x["Property_Name"].ToString() == "FrontItems").Select(x => x).ToList();
            var backItems = table.AsEnumerable().Where(x => x["Property_Name"].ToString() == "BackItems").Select(x => x).ToList();
            var frontItemID = frontItems.Select(x => x["Designer_Item_ID"].ToString()).Distinct().ToList();
            var backItemID = backItems.Select(x => x["Designer_Item_ID"].ToString()).Distinct().ToList();
            voucher.FrontItems = GetDesignerItems(frontItems, frontItemID);
            voucher.BackItems = GetDesignerItems(backItems, backItemID);
            var frontImage = table.AsEnumerable().Where(x => x["Property_Name"].ToString() == "FrontImage").Select(x => x["Image"]).FirstOrDefault();
            if (frontImage != DBNull.Value)
                voucher.FrontImage = Helper.ByteArrayToImage((byte[])frontImage);
            var backImage = table.AsEnumerable().Where(x => x["Property_Name"].ToString() == "BackImage").Select(x => x["Image"]).FirstOrDefault();
            if (backImage != DBNull.Value)
                voucher.BackImage = Helper.ByteArrayToImage((byte[])backImage);
            voucher.Height = table.AsEnumerable().Where(x => x["Property_Name"].ToString() == "Height").Select(x => x["Designer_Property_Value"].ToString()).FirstOrDefault();
            voucher.Width = table.AsEnumerable().Where(x => x["Property_Name"].ToString() == "Width").Select(x => x["Designer_Property_Value"].ToString()).FirstOrDefault();
            voucher.IsBackAdded = Boolean.Parse(table.AsEnumerable().Where(x => x["Property_Name"].ToString() == "IsBackAdded").Select(x => x["Designer_Property_Value"].ToString()).FirstOrDefault());

            return voucher;
        }

        private static ObservableCollection<DesignerControls> GetDesignerItems(List<DataRow> Items, List<string> ItemID)
        {
            var list = new ObservableCollection<DesignerControls>();
            foreach (var id in ItemID)
            {
                var itemProps = Items.Where(x => x["Designer_Item_ID"].ToString() == id).Select(x => x).ToList();
                var itemType = itemProps[0]["Type"].ToString();
                switch (itemType)
                {
                    case nameof(TextControl):
                        {
                            var textItem = new TextControl();
                            textItem.Background = Helper.GetBrush(itemProps.Where(x => x["Name"].ToString() == nameof(textItem.Background).ToString()).Select(x => x["Value"].ToString()).FirstOrDefault());
                            textItem.Foreground = Helper.GetBrush(itemProps.Where(x => x["Name"].ToString() == nameof(textItem.Foreground).ToString()).Select(x => x["Value"].ToString()).FirstOrDefault());
                            textItem.CanvasLeft = Convert.ToDouble(itemProps.Where(x => x["Name"].ToString() == nameof(textItem.CanvasLeft).ToString()).Select(x => x["Value"].ToString()).FirstOrDefault());
                            textItem.CanvasTop = Convert.ToDouble(itemProps.Where(x => x["Name"].ToString() == nameof(textItem.CanvasTop).ToString()).Select(x => x["Value"].ToString()).FirstOrDefault());
                            textItem.Width = Convert.ToDouble(itemProps.Where(x => x["Name"].ToString() == nameof(textItem.Width).ToString()).Select(x => x["Value"].ToString()).FirstOrDefault());
                            textItem.Height = Convert.ToDouble(itemProps.Where(x => x["Name"].ToString() == nameof(textItem.Height).ToString()).Select(x => x["Value"].ToString()).FirstOrDefault());
                            textItem.FontSize = Convert.ToDouble(itemProps.Where(x => x["Name"].ToString() == nameof(textItem.FontSize).ToString()).Select(x => x["Value"].ToString()).FirstOrDefault());
                            textItem.FontStyle = Helper.GetFontStyle(itemProps.Where(x => x["Name"].ToString() == nameof(textItem.FontStyle).ToString()).Select(x => x["Value"].ToString()).FirstOrDefault());
                            textItem.FontWeight = Helper.GetFontWeight(itemProps.Where(x => x["Name"].ToString() == nameof(textItem.FontWeight).ToString()).Select(x => x["Value"].ToString()).FirstOrDefault());
                            textItem.TextAlignment = Helper.GetTextAlignment(itemProps.Where(x => x["Name"].ToString() == nameof(textItem.TextAlignment).ToString()).Select(x => x["Value"].ToString()).FirstOrDefault());
                            textItem.Value = itemProps.Where(x => x["Name"].ToString() == nameof(textItem.Value).ToString()).Select(x => x["Value"].ToString()).FirstOrDefault();

                            list.Add(textItem);
                            break;
                        }
                    case nameof(ImageControl):
                        {
                            var imageItem = new ImageControl();
                            imageItem.CanvasLeft = Convert.ToDouble(itemProps.Where(x => x["Name"].ToString() == nameof(imageItem.CanvasLeft).ToString()).Select(x => x["Value"].ToString()).FirstOrDefault());
                            imageItem.CanvasTop = Convert.ToDouble(itemProps.Where(x => x["Name"].ToString() == nameof(imageItem.CanvasTop).ToString()).Select(x => x["Value"].ToString()).FirstOrDefault());
                            imageItem.Width = Convert.ToDouble(itemProps.Where(x => x["Name"].ToString() == nameof(imageItem.Width).ToString()).Select(x => x["Value"].ToString()).FirstOrDefault());
                            imageItem.Height = Convert.ToDouble(itemProps.Where(x => x["Name"].ToString() == nameof(imageItem.Height).ToString()).Select(x => x["Value"].ToString()).FirstOrDefault());
                            imageItem.Value = itemProps.Where(x => x["Name"].ToString() == nameof(imageItem.Value).ToString()).Select(x => x["Value"].ToString()).FirstOrDefault();

                            list.Add(imageItem);
                            break;
                        }
                    case nameof(QRCodeControl):
                        {
                            var qrItem = new QRCodeControl();
                            qrItem.CanvasLeft = Convert.ToDouble(itemProps.Where(x => x["Name"].ToString() == nameof(qrItem.CanvasLeft).ToString()).Select(x => x["Value"].ToString()).FirstOrDefault());
                            qrItem.CanvasTop = Convert.ToDouble(itemProps.Where(x => x["Name"].ToString() == nameof(qrItem.CanvasTop).ToString()).Select(x => x["Value"].ToString()).FirstOrDefault());
                            qrItem.Width = Convert.ToDouble(itemProps.Where(x => x["Name"].ToString() == nameof(qrItem.Width).ToString()).Select(x => x["Value"].ToString()).FirstOrDefault());
                            qrItem.Height = Convert.ToDouble(itemProps.Where(x => x["Name"].ToString() == nameof(qrItem.Height).ToString()).Select(x => x["Value"].ToString()).FirstOrDefault());
                            qrItem.Value = itemProps.Where(x => x["Name"].ToString() == nameof(qrItem.Value).ToString()).Select(x => x["Value"].ToString()).FirstOrDefault();

                            list.Add(qrItem);
                            break;
                        }
                    case nameof(FeeControl):
                        {
                            var feeItem = new FeeControl();
                            feeItem.Background = Helper.GetBrush(itemProps.Where(x => x["Name"].ToString() == nameof(feeItem.Background).ToString()).Select(x => x["Value"].ToString()).FirstOrDefault());
                            feeItem.Foreground = Helper.GetBrush(itemProps.Where(x => x["Name"].ToString() == nameof(feeItem.Foreground).ToString()).Select(x => x["Value"].ToString()).FirstOrDefault());
                            feeItem.CanvasLeft = Convert.ToDouble(itemProps.Where(x => x["Name"].ToString() == nameof(feeItem.CanvasLeft).ToString()).Select(x => x["Value"].ToString()).FirstOrDefault());
                            feeItem.CanvasTop = Convert.ToDouble(itemProps.Where(x => x["Name"].ToString() == nameof(feeItem.CanvasTop).ToString()).Select(x => x["Value"].ToString()).FirstOrDefault());
                            feeItem.Width = Convert.ToDouble(itemProps.Where(x => x["Name"].ToString() == nameof(feeItem.Width).ToString()).Select(x => x["Value"].ToString()).FirstOrDefault());
                            feeItem.Height = Convert.ToDouble(itemProps.Where(x => x["Name"].ToString() == nameof(feeItem.Height).ToString()).Select(x => x["Value"].ToString()).FirstOrDefault());
                            feeItem.FontSize = Convert.ToDouble(itemProps.Where(x => x["Name"].ToString() == nameof(feeItem.FontSize).ToString()).Select(x => x["Value"].ToString()).FirstOrDefault());
                            feeItem.FontStyle = Helper.GetFontStyle(itemProps.Where(x => x["Name"].ToString() == nameof(feeItem.FontStyle).ToString()).Select(x => x["Value"].ToString()).FirstOrDefault());
                            feeItem.FontWeight = Helper.GetFontWeight(itemProps.Where(x => x["Name"].ToString() == nameof(feeItem.FontWeight).ToString()).Select(x => x["Value"].ToString()).FirstOrDefault());
                            feeItem.TextAlignment = Helper.GetTextAlignment(itemProps.Where(x => x["Name"].ToString() == nameof(feeItem.TextAlignment).ToString()).Select(x => x["Value"].ToString()).FirstOrDefault());
                            feeItem.Value = itemProps.Where(x => x["Name"].ToString() == nameof(feeItem.Value).ToString()).Select(x => x["Value"].ToString()).FirstOrDefault();
                            feeItem.Month = itemProps.Where(x => x["Name"].ToString() == nameof(feeItem.Month).ToString()).Select(x => x["Value"].ToString()).FirstOrDefault();
                            feeItem.Year = itemProps.Where(x => x["Name"].ToString() == nameof(feeItem.Year).ToString()).Select(x => x["Value"].ToString()).FirstOrDefault();
                            feeItem.Fee = itemProps.Where(x => x["Name"].ToString() == nameof(feeItem.Fee).ToString()).Select(x => x["Value"].ToString()).FirstOrDefault();

                            list.Add(feeItem);
                            break;
                        }
                }
            }
            return list;
        }

        public static List<string> GetVouchers()
        {
            return GetDataTable($"SELECT [Designer_Name] FROM Designer WHERE  [Designer_Type] = 1 ").AsEnumerable().Select(x => x["Designer_Name"].ToString()).ToList();
        }

        public static List<string> GetCards()
        {
            return GetDataTable($"SELECT [Designer_Name] FROM Designer WHERE [Designer_Type] = 2 ").AsEnumerable().Select(x => x["Designer_Name"].ToString()).ToList();
        }

        public static void InsertIntoTable(string TableName,List<ColumnEntity> Columns)
        {
            if (Columns.Count == 0)
                return;
            SqlCommand sqlCmd = new SqlCommand();
            string columns = "";
            string values = "";
            int columnCount = 0;
            foreach (var column in Columns)
            {
                columns += $",[{column.ColumnName}]";
                values += $",@var{columnCount}";
                sqlCmd.Parameters.Add(new SqlParameter() { ParameterName = $"@var{columnCount++}", Value = column.Value.IsNullOrEmpty()? (object) DBNull.Value : column.Value});
            }
            columns = columns.Remove(0, 1);
            values = values.Remove(0, 1);
            sqlCmd.CommandText = $"INSERT INTO {TableName} ({columns}) VALUES ({values})";
            //commit insertion in database
            ExecuteQuery(sqlCmd);
        }

        public static string ImportStudentData(
            DataSet dataSet,
            List<MapperEntity> StudentColumnns,
            List<MapperEntity> ParentColumnns,
            string Sheet, string ClassID, string SectionID, bool RemoveDulpicateParent)
        {
            var result = "imported";
            var studentIdName = GetStudentID();
            var parentIdName = GetParentID();
            var isParentIdImported = false;
            var isStudentIdImported = false;
            foreach (DataRow row in dataSet.Tables[Sheet].Rows)
            {
                var studentEntities = new List<ColumnEntity>();
                var parentEntities = new List<ColumnEntity>();
                var studentID = GetNextID("Students");
                var parentID = GetNextID("Parents");
                foreach (var item in StudentColumnns)
                {
                    //if column is not mapped to anything then do nothing
                    if (item.Value.IsNullOrEmpty())
                        continue;

                    if(item.FeildName == studentIdName)
                    {
                        studentEntities.Add(new ColumnEntity { ColumnName = "Student_ID", 
                            Value = row[item.Value].ToString().IsNullOrEmpty()? studentID : row[item.Value].ToString() });
                        isStudentIdImported = true;
                    }
                    else
                    {
                        studentEntities.Add(new ColumnEntity { ColumnName = item.FeildName, Value = row[item.Value].ToString() });
                    }
                }
                foreach (var item in ParentColumnns)
                {
                    //if column is not mapped to anything then do nothing
                    if (item.Value.IsNullOrEmpty())
                        continue;
                    var columnEntity = new ColumnEntity();
                    if (item.FeildName == parentIdName)
                    {
                        columnEntity.ColumnName = "Parent_ID";
                        if(row[item.Value].ToString().IsNullOrEmpty())
                        {
                            columnEntity.Value = parentID;
                        }
                        else
                        {
                            columnEntity.Value = row[item.Value].ToString();
                            parentID = row[item.Value].ToString();
                        }
                        parentEntities.Add(new ColumnEntity
                        {
                            ColumnName = "Parent_ID",
                            Value = row[item.Value].ToString().IsNullOrEmpty() ? parentID : row[item.Value].ToString()
                        });
                        isParentIdImported = true;
                    }
                    else
                    {
                        parentEntities.Add(new ColumnEntity { ColumnName = item.FeildName, Value = row[item.Value].ToString() });
                    }
                }

                //TODO: for now cannot insert parent id because identity insert is ON, also implement the remove dulpicate parent
                //if (!isParentIdImported)
                //    parentEntities.Add(new ColumnEntity { ColumnName = "Parent_ID", Value = parentID });
                if (!isStudentIdImported)
                    studentEntities.Add(new ColumnEntity { ColumnName = "Student_ID", Value = studentID });

                studentEntities.Add(new ColumnEntity { ColumnName = "Parent_ID", Value = parentID });
                studentEntities.Add(new ColumnEntity { ColumnName = "Class_ID", Value = ClassID });
                studentEntities.Add(new ColumnEntity { ColumnName = "Section_ID", Value = SectionID });

                InsertIntoTable("Parents", parentEntities);
                InsertIntoTable("Students", studentEntities);
            }
            //catch(SqlException ex)
            //{
            //    if(ex.Number == 2627)
            //        result = ex.Message;
            //}
            return result;
        }

        public static void ImportTeacherData(
            DataSet dataSet,
            List<MapperEntity> TeacherColumnns,
            string Sheet)
        {
            
            using (SqlConnection sqlConn = new SqlConnection(connectionString))
            {
                sqlConn.Open();
                SqlCommand sqlCmd = new SqlCommand();
                sqlCmd.Connection = sqlConn;

                //for each record
                foreach (DataRow row in dataSet.Tables[Sheet].Rows)
                {
                    var teacherEntities = new List<ColumnEntity>();
                    var teacherIdName = GetTeacherID();
                    var teacherID = GetNextID("Teachers");
                    bool isteacerIdImported = false;
                    foreach (var item in TeacherColumnns)
                    {
                        //if column is not mapped to anything then do nothing
                        if (item.Value.IsNullOrEmpty())
                            continue;
                        var columnEntity = new ColumnEntity();
                        if (item.FeildName == teacherIdName)
                        {
                            columnEntity.ColumnName = "Teacher_ID";
                            if (row[item.Value].ToString().IsNullOrEmpty())
                            {
                                columnEntity.Value = teacherID;
                            }
                            else
                            {
                                columnEntity.Value = row[item.Value].ToString();
                                teacherID = row[item.Value].ToString();
                            }
                            teacherEntities.Add(new ColumnEntity
                            {
                                ColumnName = "Teacher_ID",
                                Value = row[item.Value].ToString().IsNullOrEmpty() ? teacherID : row[item.Value].ToString()
                            });
                            isteacerIdImported = true;
                        }
                        else
                        {
                            teacherEntities.Add(new ColumnEntity { ColumnName = item.FeildName, Value = row[item.Value].ToString() });
                        }
                    }
                    
                    InsertIntoTable("Teachers", teacherEntities);
                }
            }
        }

        public static bool Register(string Key, RegistrationType Type, DateTime StartDate, DateTime EndDate)
        {
            //split the key into hash and random number
            string keyhash = Key.Substring(0, 9).Replace("-", "");
            string key = Key.Substring(10, 9).Replace("-", "");

            //generate the hash of the product id, random number and key
            var generatedHash = Helper.GetStringFromHash(Helper.GetHash(ProductInformation.ProductID + key + Type));

            //compare the hashes
            if (!generatedHash.StartsWith(keyhash))
                return false;


            DataTable dTable = new DataTable();
            
            using (SqlConnection sqlConn = new SqlConnection(connectionString))
            {
                sqlConn.Open();
                SqlCommand sqlCmd = new SqlCommand("SELECT * FROM RegisteredKeys WHERE [KeyHash] = @keyhash", sqlConn);
                sqlCmd.Parameters.Add("@keyhash", SqlDbType.Binary).Value = Helper.GetHash(keyhash + key);
                sqlCmd.Connection = sqlConn;
                SqlDataAdapter sqlDa = new SqlDataAdapter(sqlCmd);
                sqlDa.Fill(dTable);
            }

            if (dTable.Rows.Count != 0)
                return false;
            //Now process the registration
            if (GetDataTable("SELECT * FROM Registration").Rows.Count == 0)
            {
                ExecuteQuery($"INSERT INTO Registration ( [Start_Date] , [End_Date]  , [Current_Date] " +
                    $" , [Days_Left] , [Run_Time] , [Registered_Date]  , Registration_Type )" +
                    $"VALUES ('{StartDate.ToString().Encrypt()}' , '{EndDate.ToString().Encrypt()}' , '{DateTime.Now.ToString().Encrypt()}' ,  " +
                    $"'{(EndDate - StartDate).TotalDays.ToString().Encrypt()}' , '{TimeSpan.Zero.ToString().Encrypt()}' , " +
                    $" '{DateTime.Now.ToString().Encrypt()}' , '{Type.ToString().Encrypt()}' )");
            }
            else
            {
                ExecuteQuery($"UPDATE Registration SET [Start_Date] = '{StartDate.ToString().Encrypt()}' , [End_Date] = '{EndDate.ToString().Encrypt()}' , " +
                    $"[Current_Date] = '{DateTime.Now.ToString().Encrypt()}' , [Old_Current_Date] = NULL" +
                    $" , [Days_Left] = '{(EndDate - StartDate).TotalDays.ToString().Encrypt()}' , " +
                    $"[Run_Time] = '{TimeSpan.Zero.ToString().Encrypt()}' , [Registered_Date] = '{DateTime.Now.ToString().Encrypt()}' " +
                    $", Registration_Type = '{Type.ToString().Encrypt()}' ");
            }
            //insert the key into registered keys
            using (SqlConnection sqlConn = new SqlConnection(connectionString))
            {
                sqlConn.Open();
                SqlCommand sqlCmd = new SqlCommand("INSERT INTO RegisteredKeys ( [KeyHash] ) VALUES ( @keyhash )", sqlConn);
                sqlCmd.Parameters.Add("@keyhash", SqlDbType.Binary).Value = Helper.GetHash(keyhash + key);
                sqlCmd.Connection = sqlConn;
                sqlCmd.ExecuteNonQuery();
            }
            return true;
        }

        public static RegistrationStatus CheckRegistration()
        {
            DataTable table = GetDataTable("SELECT * FROM Registration");

            if(table.Rows.Count == 0)
            {
                return new RegistrationStatus { DaysLeft = 0, Status = RegistrationWarning.Expired, Type = "Monthly" };
            }

            DateTime startDate = new DateTime();
            DateTime.TryParse(table.Rows[0]["Start_Date"].ToString().Decrypt(), out startDate);

            DateTime endDate = new DateTime();
            DateTime.TryParse(table.Rows[0]["End_Date"].ToString().Decrypt(), out endDate);

            DateTime currentDate = new DateTime();
            DateTime.TryParse(table.Rows[0]["Current_Date"].ToString().Decrypt(), out currentDate);

            DateTime oldCurrentDate = new DateTime();
            DateTime.TryParse(table.Rows[0]["Old_Current_Date"].ToString().Decrypt(), out oldCurrentDate);

            DateTime registeredDate = new DateTime();
            DateTime.TryParse(table.Rows[0]["Old_Current_Date"].ToString().Decrypt(), out registeredDate);


            TimeSpan runTime = new TimeSpan();
            TimeSpan.TryParse(table.Rows[0]["Run_Time"].ToString().Decrypt(), out runTime);

            int daysLeft = (int)Convert.ToDouble(table.Rows[0]["Days_Left"].ToString().Decrypt());
            var registrationType = table.Rows[0]["Registration_Type"].ToString().Decrypt();

            if (registrationType == "FullTime")
                return new RegistrationStatus { Status = RegistrationWarning.Registered, DaysLeft = daysLeft, Type = registrationType };

            if (daysLeft < 1)
                return new RegistrationStatus { Status = RegistrationWarning.Expired, DaysLeft = daysLeft, Type = registrationType };

            var date = DateTime.Now;

            //means its in registration period
            if (date.Date >= startDate.Date && date.Date <= endDate.Date)
            {
                //calculates the number of days left of registration
                int days = (int)(date.Date - currentDate.Date).TotalDays;
                days = Math.Abs(days);
                if (runTime.TotalHours > 24 && days < 1)
                {
                    runTime = runTime - new TimeSpan(24, 0, 0);
                    daysLeft--;
                }
                else
                {
                    daysLeft = daysLeft - days;
                }
                ExecuteQuery($"UPDATE Registration SET [Current_Date] = '{date.ToString().Encrypt()}'" +
                        $" , [Days_Left] = '{daysLeft.ToString().Encrypt()}' , [Run_Time] = '{runTime.ToString().Encrypt()}'");

                if (daysLeft < 1)
                    return new RegistrationStatus { Status = RegistrationWarning.Expired, DaysLeft = daysLeft, Type = registrationType };
                if (daysLeft < 6)
                    return new RegistrationStatus { Status = RegistrationWarning.Alert, DaysLeft = daysLeft, Type = registrationType };
                return new RegistrationStatus { Status = RegistrationWarning.Registered, DaysLeft = daysLeft, Type = registrationType };
            }
            return new RegistrationStatus { Status = RegistrationWarning.Expired, DaysLeft = daysLeft, Type = registrationType };
        }

        public static void SetProductInfo()
        {
            var table = GetDataTable($"SELECT * FROM Product_Information");
            ProductInformation.ProductID = table.Rows[0]["Product_ID"].ToString();
            ProductInformation.ProductVersion = table.Rows[0]["Product_Version"].ToString();
            ProductInformation.RegisteredTo = table.Rows[0]["Registered_To"].ToString();
            ProductInformation.RegistrationDate = table.Rows[0]["Registered_Date"].ToString();
        }

        public static void CreateExpense(string Title)
        {
            using (sqlConn = new SqlConnection(connectionString))
            {
                sqlConn.Open();
                sqlCmd = new SqlCommand($"INSERT INTO Expense_Type(Expense_Title) VALUES (@title)", sqlConn);
                sqlCmd.Parameters.Add("@title", SqlDbType.NVarChar).Value = Title;
                sqlCmd.ExecuteNonQuery();
            }
        }

        public static void AddExpense(string ExpenseType, string Amount, string Description, string Date)
        {
            using (sqlConn = new SqlConnection(connectionString))
            {
                sqlConn.Open();
                sqlCmd = new SqlCommand($"INSERT INTO Expenses([Expense_Type_ID] , [Amount], [Description] , [Date]) VALUES (@expense_id , @amount, @description , @date)", sqlConn);
                sqlCmd.Parameters.Add("@expense_id", SqlDbType.NVarChar).Value = GetDataTable($"SELECT [Expense_Type_ID] FROM Expense_Type WHERE Expense_Title = '{ExpenseType}'").AsEnumerable().Select(x => x[0].ToString()).FirstOrDefault();
                sqlCmd.Parameters.Add("@amount", SqlDbType.NVarChar).Value = Amount;
                sqlCmd.Parameters.Add("@description", SqlDbType.NVarChar).Value = Description ?? "";
                sqlCmd.Parameters.Add("@date", SqlDbType.NVarChar).Value = Date;
                sqlCmd.ExecuteNonQuery();
            }
        }

        public static void EditExpense(string ExpenseID, string ExpenseType, string Amount, string Description)
        {
            using (sqlConn = new SqlConnection(connectionString))
            {
                sqlConn.Open();
                sqlCmd = new SqlCommand($"UPDATE Expenses SET [Expense_Type_ID] = @expense_id , [Amount] = @amount , [Description] = @description WHERE Expense_ID = {ExpenseID}", sqlConn);
                sqlCmd.Parameters.Add("@expense_id", SqlDbType.NVarChar).Value = GetDataTable($"SELECT [Expense_Type_ID] FROM Expense_Type WHERE Expense_Title = '{ExpenseType}'").AsEnumerable().Select(x => x[0].ToString()).FirstOrDefault();
                sqlCmd.Parameters.Add("@amount", SqlDbType.NVarChar).Value = Amount;
                sqlCmd.Parameters.Add("@description", SqlDbType.NVarChar).Value = Description;
                sqlCmd.ExecuteNonQuery();
            }
        }

        public static ObservableCollection<Expense> LoadExpenses(string Day , string Month, string Year, bool IsGrouped)
        {
            var date = "";
            if (Day != "All")
                date += $"AND Day([Date]) = {Day} ";
            if(Month != "All")
                date += $"AND Month([Date]) = {Helper.GetMonthNumber(Month)} ";
            if(Year != "All")
                date += $"AND Year([Date]) = {Year} ";
            if (date.IsNotNullOrEmpty())
            {
                date = date.Remove(0, 3);
                date = $"WHERE {date} ";
            }
            if (IsGrouped)
            {
                return new ObservableCollection<Expense>
            (
                GetDataTable($"SELECT Expense_Title , SUM(Amount) AS Amount FROM Expenses " +
                $"LEFT JOIN Expense_Type ON Expense_Type.Expense_Type_ID = Expenses.Expense_Type_ID " +
                $"{date} " +
                $"GROUP BY Expense_Title ORDER BY Expense_Title").AsEnumerable()
                .Select(x => new Expense
                {
                    Amount = Convert.ToInt32(x["Amount"]),
                    ExpenseType = x["Expense_Title"].ToString(),
                    EditIcon = Application.Current.FindResource("EditIcon") as string
                }).ToList()
            );
            }

            return new ObservableCollection<Expense>
            (
                GetDataTable($"SELECT Expenses.* , [Expense_Title] FROM Expenses " +
                $"LEFT JOIN Expense_Type ON Expense_Type.Expense_Type_ID = Expenses.Expense_Type_ID " +
                $"{date} ORDER BY Expense_Title").AsEnumerable()
                .Select(x => new Expense
                {
                    ExpenseID = x["Expense_ID"].ToString(),
                    Amount = Convert.ToInt32(x["Amount"]),
                    Description = x["Description"].ToString(),
                    ExpenseType = x["Expense_Title"].ToString(),
                    EditIcon = Application.Current.FindResource("EditIcon") as string
                }).ToList()
            );
        }

        public static ObservableCollection<string> GetExpenses()
        {
            return new ObservableCollection<string>
                (
                GetDataTable($"SELECT [Expense_Title] FROM Expense_Type").AsEnumerable().Select(x => x[0].ToString())
                );
        }

        public static DataTable GetMonthlyExpenses(string Month, string Year)
        {
            return GetDataTable($"SELECT [Date], SUM(Amount) AS Amount FROM Expenses " +
                $"RIGHT JOIN Expense_Type ON Expense_Type.Expense_Type_ID = Expenses.Expense_Type_ID " +
                $"WHERE Month([Date]) = {Month} AND Year([Date]) = {Year} GROUP BY [Date]");
        }

        public static DataTable GetTotalExpense(string Month, string Year)
        {
            return GetDataTable($"SELECT Expense_Title AS Expense, SUM(Amount) AS Amount FROM Expenses " +
                $"RIGHT JOIN Expense_Type ON Expense_Type.Expense_Type_ID = Expenses.Expense_Type_ID " +
                $"WHERE Month([Date]) = {Month} AND Year([Date]) = {Year} GROUP BY Expense_Type.Expense_Title");
        }

        public static void CreateFeeRecord(FeeEntity fee, string classID, string month , string year)
        {
            //to create the fee record
            ExecuteQuery($"INSERT INTO Fee_Record " +
                $"( Fee_ID, Student_ID, [Month] , [Year] , Fee , Amount , Late_Fee , Discount , Due_Date ) " +
                $"SELECT {fee.FeeID} , Students.Student_ID , {month} , {year} , '{fee.Fee}' , {fee.Amount} , " +
                $"{fee.LateFee} , ISNULL(Discounts.Discount,0) , " +
                $"'{year}-{month}-{fee.DueDate}' FROM Students LEFT JOIN Discounts ON " +
                $"Discounts.Student_ID = Students.Student_ID AND " +
                $"Discounts.Fee_ID = {fee.FeeID} WHERE Students.Class_ID = {classID}  AND Students.Is_Active = 1");

            //to save the information of the fee record in already created fee records table
            ExecuteQuery($"INSERT INTO Created_Fee_Records (Class_ID , Fee_ID, [Month] , [Year]) VALUES ( {classID} , {fee.FeeID} , {month} , {year})");
        }

        public static void CreateFeeRecord(DataRow fee, string classID, string month , string year)
        {
            //to create the fee record
            ExecuteQuery($"INSERT INTO Fee_Record " +
            $"( Fee_ID, Student_ID, [Month] , [Year] , Fee , Amount , Late_Fee , Discount , Due_Date ) " +
            $"SELECT {fee["Fee_ID"]} , Students.Student_ID , {month} , {year} , '{fee["Fee"]}' , {fee["Amount"]} , " +
            $"{fee["Late_Fee"]} , ISNULL(Discounts.Discount,0) , " +
            $"'{year}-{month}-{fee["Due_Date"]}' FROM Students LEFT JOIN Discounts ON " +
            $"Discounts.Student_ID = Students.Student_ID AND " +
            $"Discounts.Fee_ID = {fee["Fee_ID"]} WHERE Students.Class_ID = {classID}  AND Students.Is_Active = 1");

            //to save the information of the fee record in already created fee records table
            ExecuteQuery($"INSERT INTO Created_Fee_Records (Class_ID , Fee_ID, [Month] , [Year]) VALUES ( {classID} , {fee["Fee_ID"]} , {month} , {year})");

        }

        public static bool SetupApplication()
        {
            var table = GetDataTable("SELECT name FROM master.sys.databases WHERE Name = N'ims_db'");
            if (table.Rows.Count != 0)
                return true;

            var window = new ApplicationSetupWindow();
            window.Show();

            return false;
        }
    }
}

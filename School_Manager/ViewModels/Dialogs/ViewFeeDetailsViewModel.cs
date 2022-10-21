using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace School_Manager
{
    /// <summary>
    /// view fee details ViewModel
    /// </summary>
    public class ViewFeeDetailsViewModel : BasePropertyChanged
    {
        #region Default Constructor

        public ViewFeeDetailsViewModel(string StudentID, string SelectedClass)
        {
            //sets the student name
            StudentName = DataAccess.GetDataTable($"SELECT [Name] FROM Students WHERE [Student_ID] = '{StudentID}'").Rows[0][0].ToString();

            this.SelectedClass = SelectedClass;
            this.StudentID = StudentID;

            //initialize the properties
            FeeNames = new ListEntity { FeildName = "Fee", IsEditable = true};
            FeeNames.Items = DataAccess.GetDataTable($"SELECT [Fee] FROM Fee_Structure WHERE Class_ID = {DataAccess.GetClassID(SelectedClass)}").AsEnumerable().Select(e => e["Fee"] as string).ToList();
            var list = Helper.GetYears();
            list.Insert(0,"All");
            Years = new ListEntity { FeildName = "Year", Items = list, ValidationType = ValidationType.NotEmpty, Value = DateTime.Now.Year.ToString() };
            list = Helper.GetMonths();
            list.Insert(0,"All");
            Months = new ListEntity { FeildName = "Month", Items = list, Value = Helper.GetMonthName(DateTime.Now.Month), ValidationType = ValidationType.NotEmpty };

            GetFeeDetails();
            
            //initialize the command
            GetFeeDeatilsCommand = new RelayCommand(GetFeeDetails);
            AddFeeCommand = new RelayCommand(AddFee);
            SetFeeStructureCommand = new RelayCommand(SetFeeStructure);
            DeleteFeeCommand = new RelayParameterizedCommand(parameter => DeleteFee(parameter));
            EditFeeCommand = new RelayParameterizedCommand(parameter => EditFee(parameter));
            SetDueDateCommand = new RelayCommand(SetDueDate);
            CancelEditCommand = new RelayParameterizedCommand(parameter => CancelEdit(parameter));
            UpdateDueDateCommand = new RelayCommand(UpdateDueDate);
        }

        #endregion

        #region Public Properties

        public string StudentID { get; set; }

        public string StudentName { get; set; }

        public string SelectedClass { get; set; }

        public ListEntity FeeOptions { get; set; } = new ListEntity { FeildName = "Status",
            Items = new List<string> { "All", "Paid", "Partially Paid", "Pending" , "Due" },
            ValidationType = ValidationType.NotEmpty, Value = "All" };

        public ListEntity Months { get; set; }

        public ListEntity Years { get; set; }

        public ListEntity FeeMonths { get; set; } = new ListEntity { FeildName = "Month", Value = Helper.GetMonthName(DateTime.Now.Month), Items = Helper.GetMonths(), ValidationType = ValidationType.NotEmpty };

        public ListEntity FeeYears { get; set; } = new ListEntity { FeildName = "Year", Value = DateTime.Now.Year.ToString(), Items = Helper.GetYears(), ValidationType = ValidationType.NotEmpty };

        public string SelectedDueDate { get; set; }

        public ListEntity FeeNames { get; set; }

        public TextEntity Amount { get; set; } = new TextEntity { FeildName = "Amount", ValidationType = ValidationType.NumericWithNonEmpty, Value = "0" };

        public TextEntity LateFee { get; set; } = new TextEntity { FeildName = "Late Fee", ValidationType = ValidationType.NumericWithNonEmpty, Value = "0" };

        public TextEntity Discount { get; set; } = new TextEntity { FeildName = "Discount", ValidationType = ValidationType.NumericWithNonEmpty , Value = "0" };

        public TextEntity DueDate { get; set; } = new TextEntity { FeildName = "Due Date", DueDate = DateTime.Now };

        public ObservableCollection<MonthlyFeeRecord> FeeRecordCollection { get; set; }

        public bool IsAddFeeOpen { get; set; } = false;

        #endregion

        #region Public Commands

        public ICommand GetFeeDeatilsCommand { get; set; }

        public ICommand AddFeeCommand { get; set; }

        public ICommand SetFeeStructureCommand { get; set; }

        public ICommand DeleteFeeCommand { get; set; }

        public ICommand EditFeeCommand { get; set; }

        public ICommand SetDueDateCommand { get; set; }

        public ICommand CancelEditCommand { get; set; }

        public ICommand UpdateDueDateCommand { get; set; }

        #endregion

        #region Command Methods

        private void GetFeeDetails()
        {
            FeeRecordCollection = new ObservableCollection<MonthlyFeeRecord>(DataAccess.GetFeeDetails(StudentID, FeeOptions.Value, Months.Value, Years.Value));
            foreach (var item in FeeRecordCollection)
            {
                foreach (var fee in item.FeeEntities)
                {
                    fee.TakeBackup();
                }
            }
        }

        private void AddFee()
        {
            if(FeeNames.Text.IsNullOrEmpty())
            {
                DialogManager.ShowMessageDialog("Warning", "Fee cannot be empty.",DialogTitleColor.Red);
                return;
            }
            if(!Amount.IsValid || !Discount.IsValid || !LateFee.IsValid || !FeeMonths.IsValid || !FeeYears.IsValid)
            {
                DialogManager.ShowValidationMessage();
                return;
            }

            DataAccess.ExecuteQuery($"INSERT INTO Fee_Record(Fee , Amount , Late_Fee , Discount , Due_Date , [Month] , [Year] , Student_ID) " +
                $"VALUES ('{FeeNames.Text}' , {Amount.Value} , {LateFee.Value} , {Discount.Value} , " +
                $"'{DueDate.DueDate.Date.ToShortDateString()}' , {Helper.GetMonthNumber(FeeMonths.Value)} , " +
                $"{FeeYears.Value} , {StudentID})");
            GetFeeDetails();
            DialogManager.ShowMessageDialog("Message", "Fee added successfully.",DialogTitleColor.Green);
        }

        private void SetFeeStructure()
        {
            if (FeeNames.Value.IsNullOrEmpty())
            {
                return;
            }
            var classID = DataAccess.GetClassID(SelectedClass);
            var discount = DataAccess.GetDataTable($"SELECT [Discount] FROM Fee_Structure " +
                    $"LEFT JOIN Discounts ON Discounts.Fee_ID = Fee_Structure.Fee_ID " +
                    $"WHERE Fee_Structure.Class_ID = {classID} AND  Fee =  '{FeeNames.Value}' AND Student_ID = {StudentID}");
            var table = DataAccess.GetDataTable($"SELECT * FROM Fee_Structure WHERE Class_ID = {classID} AND  Fee =  '{FeeNames.Value}'");
            if(table.Rows.Count != 0)
            {
                Amount.Value = table.Rows[0]["Amount"].ToString();
                LateFee.Value = table.Rows[0]["Late_Fee"].ToString();
                Discount.Value = discount.Rows.Count == 0 ? "0" : discount.Rows[0][0].ToString();
                DueDate.DueDate = new DateTime(int.Parse(FeeYears.Value), Helper.GetMonthNumber(FeeMonths.Value), (int)table.Rows[0]["Due_Date"]);
            }

        }

        private void UpdateDueDate()
        {
            DueDate.DueDate = new DateTime(int.Parse(FeeYears.Value), Helper.GetMonthNumber(FeeMonths.Value), DueDate.DueDate.Day);
        }

        private void SetDueDate()
        {
            //if(_DueDate.IsNullOrEmpty())
            //    DueDate = new DateTime(Convert.ToInt32(SelectedFeeYear), Convert.ToInt32(Helper.GetMonthNumber(SelectedFeeMonth)), 10);
            //else
            //    DueDate = new DateTime(Convert.ToInt32(SelectedFeeYear), Convert.ToInt32(Helper.GetMonthNumber(SelectedFeeMonth)), Convert.ToInt32(_DueDate));
        }

        private void DeleteFee(object parameter)
        {
            var fee = parameter as FeeEntity;
            if(fee.DeleteButtonIcon == Application.Current.FindResource("DeleteBinIcon") as string)
            {
                if(DialogManager.ShowMessageDialog("Warning", "Are you sure, you want to delete this fee?", true))
                {
                    DataAccess.ExecuteQuery($"DELETE FROM Fee_Record WHERE Fee_Record_ID = {fee.FeeRecordID}");
                    GetFeeDetails();
                }
            }
            else
            {
                CancelEdit(parameter);
            }
        }

        private void EditFee(object parameter)
        {
            using (new WaitCursor())
            {
                try
                {
                    FeeEntity record = parameter as FeeEntity;
                    if (!record.IsWholeValid)
                    {
                        DialogManager.ShowValidationMessage();
                        return;
                    }
                    record.IsEditEnable ^= true;
                    if (record.IsEditEnable)
                    {
                        record.EditButtonIcon = Application.Current.FindResource("CheckIcon") as string;
                        record.DeleteButtonIcon = Application.Current.FindResource("CrossIcon") as string;
                        record.TakeBackup();
                    }

                    else
                    {
                        //save
                        record.IsEditEnable = false;
                        record.EditButtonIcon = Application.Current.FindResource("EditIcon") as string;
                        record.DeleteButtonIcon = Application.Current.FindResource("DeleteBinIcon") as string;

                        //TODO: when updating due date it also changes the late fee
                        if (record.Date.Date < DateTime.Now.Date)
                        {
                            DataAccess.ExecuteQuery($"UPDATE Fee_Record SET Fee = '{record.Fee}' , " +
                               $"Amount = {record.Amount} , Late_Fee = {record.LateFee} , " +
                               $"Discount = {record.Discount} , Due_Date = '{record.Date.ToShortDateString()}' " +
                               $"WHERE Fee_Record_ID = {record.FeeRecordID}");
                        }
                        else
                        {
                            DataAccess.ExecuteQuery($"UPDATE Fee_Record SET Fee = '{record.Fee}' , " +
                                 $"Amount = {record.Amount} , Discount = {record.Discount} ,  Due_Date = '{record.Date.ToShortDateString()}' " +
                                 $"WHERE Fee_Record_ID = {record.FeeRecordID}");
                        }
                        GetFeeDetails();
                        record.EditButtonIcon = Application.Current.FindResource("EditIcon") as string;

                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(ex, true);
                }

            }
        }

        private void CancelEdit(object parameter)
        {
            var fee = parameter as FeeEntity;
            fee.RestoreBackup();
            fee.EditButtonIcon = Application.Current.FindResource("EditIcon") as string;
            fee.DeleteButtonIcon = Application.Current.FindResource("DeleteBinIcon") as string;
            fee.IsEditEnable = false;
        }

        #endregion
    }
}

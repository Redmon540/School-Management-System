using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace School_Manager
{
    public class FeeCollectionViewModel :  BaseViewModel
    {
        #region Constructor

        public FeeCollectionViewModel()
        {
            DeleteFeeCommand = new RelayParameterizedCommand(parameter => DeleteFee(parameter));
            EditFeeCommand = new RelayParameterizedCommand(parameter => EditFee(parameter));
            IDChangedCommand = new RelayCommand(IDChanged);
            PayFeeCommand = new RelayParameterizedCommand(parameter => PayFee(parameter));
            SearchIDCommand = new RelayCommand(SearchID);
            CancelEditCommand = new RelayParameterizedCommand(parameter => CancelEdit(parameter));
        }

        #endregion

        #region Primate member

        private int voucherID;

        #endregion

        #region Properties

        public ObservableCollection<MonthlyFeeRecord> FeeRecordCollection { get; set; }

        public bool AutoPay { get; set; } = false;

        public bool IsFeeCollected { get; set; }

        public bool IsFeeCollectionVisible { get; set; }

        public bool IsFeeAvailable { get; set; } = false;

        public bool IsFeePayButtonVisible { get; set; } = false;

        public string QRCode { get; set; }

        public string IDName { get; set; }

        public string ID { get; set; }

        public TextEntity StudentID { get; set; } = new TextEntity { FeildName = DataAccess.GetStudentID() };

        public BitmapImage Photo { get; set; }

        public string Name { get; set; }

        public string Class { get; set; }

        public int FeeAmount => FeeRecordCollection == null ? 0 : FeeRecordCollection.Select(x => x.FeeEntities.Select(y => int.Parse(y.DueAmount)).Sum()).Sum();

        public string DateAndTime { get; set; }

        public string AlertMessage { get; set; }

        public bool ShowUserInfo { get; set; } = false;

        public int FeeCount { get; set; } = 0;

        public string FeePayStatus { get; set; } = "Pay All Fees";

        #endregion

        #region Commands

        public ICommand DeleteFeeCommand { get; set; }

        public ICommand EditFeeCommand { get; set; }

        public ICommand CancelEditCommand { get; set; }

        public ICommand IDChangedCommand { get; set; }

        public ICommand PayFeeCommand { get; set; }

        public ICommand SearchIDCommand { get; set; }

        #endregion

        #region Command Methods

        private void LoadFeeDetails()
        {
            FeeRecordCollection = DataAccess.GetPaymentFeeDetails(StudentID.Value);
            
        }

        private void SearchID()
        {
            try
            {
                if (StudentID.Value.IsNotNullOrEmpty())
                {
                    voucherID = 0;
                    LoadFeeDetails();
                    if (FeeRecordCollection.Count == 0)
                    {
                        IsFeeAvailable = true;
                        IsFeePayButtonVisible = false;
                    }
                    else
                    {
                        IsFeeAvailable = false;
                        IsFeePayButtonVisible = true;
                    }
                    DataTable table = DataAccess.GetDataTable($"SELECT [Student_ID] , [Name] , [Photo] , [Class] , [Section] FROM Students" +
                        $" LEFT JOIN Classes ON Classes.Class_ID = Students.Class_ID" +
                        $" LEFT JOIN Sections ON Sections.Section_ID = Students.Section_ID" +
                        $" WHERE Students.Student_ID = {StudentID.Value} AND Students.Is_Active = 1");
                    if (table.Rows.Count != 0)
                    {

                        IDName = DataAccess.GetStudentID();

                        ID = StudentID.Value;
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

                        ShowUserInfo = true;
                        QRCode = "";
                        DateAndTime = DateTime.Now.ToString();
                        IsFeeCollectionVisible = false;
                    }
                    else
                    {
                        ID = "";
                        Name = "";
                        Photo = null;
                    }
                }
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
                using (new WaitCursor())
                {
                    if (QRCode.IsNullOrEmpty())
                    {
                        return;
                    }

                    if (!QRCode.EndsWith("\r\n"))
                        return;

                    string[] codes = QRCode.Split(new string[] { "\r\n" }, StringSplitOptions.None);

                    for (int i = 0; i < codes.Length; i++)
                    {
                        string[] qr = codes[i].Split('-');
                        if (qr.Length != 2)
                        {
                            return;
                        }

                        if (!int.TryParse(qr[1], out voucherID))
                        {
                            return;
                        }

                        if (voucherID <= 0)
                        {
                            return;
                        }

                        DataTable table = new DataTable();
                        if (qr[0] == "STD")
                        {
                            if (AutoPay)
                            {
                                StudentID.Value = qr[1];
                                SearchID();
                                foreach (var item in FeeRecordCollection)
                                {
                                    foreach (var feeItem in item.FeeEntities)
                                    {
                                        if (feeItem.PaidAmount == "0")
                                        {
                                            var date = DateTime.Now;
                                            DataAccess.ExecuteQuery($"INSERT INTO Voucher_Record (Student_ID , [DateTime]) VALUES ({StudentID.Value} , '{date}')");
                                            voucherID = (int)DataAccess.GetDataTable($"SELECT [Voucher ID] FROM Voucher_Record WHERE Student_ID = {StudentID.Value} AND [DateTime] = '{date}'").Rows[0][0];

                                            feeItem.PaidAmount = (Convert.ToInt32(feeItem.PaidAmount) + Convert.ToInt32(feeItem.DueAmount)).ToString();
                                            if (feeItem.DueAmount == "0")
                                                feeItem.FeeStatus = "PAID";
                                            DataAccess.ExecuteQuery($"UPDATE Fee_Record SET Paid_Amount = {feeItem.PaidAmount} , Paid_Voucher_ID = {voucherID} WHERE Fee_Record_ID = {feeItem.FeeRecordID}");
                                            DataAccess.ExecuteQuery($"UPDATE [Voucher_Record] SET [Status] = 1 , " +
                                                $"[Received Time] = '{DateTime.Now.TimeOfDay}' , [Received Date] = '{DateTime.Now.Date}' , " +
                                                $"[Received By] = '{Account.User}' , [Payment Method] = 'CASH' WHERE [Voucher ID] = {voucherID}");
                                        }
                                        else
                                        {
                                            feeItem.PaidAmount = (Convert.ToInt32(feeItem.PaidAmount) + Convert.ToInt32(feeItem.DueAmount)).ToString();
                                            if (feeItem.DueAmount == "0")
                                                feeItem.FeeStatus = "PAID";
                                            DataAccess.ExecuteQuery($"UPDATE Fee_Record SET Paid_Amount = {feeItem.PaidAmount} WHERE Fee_Record_ID = {feeItem.FeeRecordID}");
                                        }
                                    }
                                }
                                FeeCount++;
                            }
                            else
                            {
                                StudentID.Value = qr[1];
                                SearchID();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex, true);
                ShowUserInfo = true;
                QRCode = "";
            }
        }

        private void PayFee(object sender)
        {
            //try
            {
                FeeEntity fee = sender as FeeEntity;
                if (fee == null)
                {
                    foreach (var item in FeeRecordCollection)
                    {
                        foreach (var feeItem in item.FeeEntities)
                        {
                            if (!feeItem.IsAmountToPayValid || !feeItem.IsWholeValid)
                            {
                                DialogManager.ShowValidationMessage();
                                return;
                            }
                            if (feeItem.IsEditEnable)
                            {
                                EditFee(feeItem);
                                if (feeItem.IsEditEnable)
                                    return;
                            }
                            if (feeItem.PaidAmount == "0")
                            {
                                using(new WaitCursor())
                                {
                                    if (voucherID == 0)
                                    {
                                        var date = DateTime.Now;
                                        DataAccess.ExecuteQuery($"INSERT INTO Voucher_Record (Student_ID , [DateTime]) VALUES ({StudentID.Value} , '{date}')");
                                        voucherID = (int)DataAccess.GetDataTable($"SELECT [Voucher ID] FROM Voucher_Record WHERE Student_ID = {StudentID.Value} AND [DateTime] = '{date}'").Rows[0][0];
                                    }
                                    feeItem.PaidAmount = (Convert.ToInt32(feeItem.PaidAmount) + Convert.ToInt32(feeItem.AmountToPay)).ToString();
                                    if (feeItem.DueAmount == "0")
                                        feeItem .FeeStatus = "PAID";
                                    DataAccess.ExecuteQuery($"UPDATE Fee_Record SET Paid_Amount = {feeItem.PaidAmount} , Paid_Voucher_ID = {voucherID} WHERE Fee_Record_ID = {feeItem.FeeRecordID}");
                                    DataAccess.ExecuteQuery($"UPDATE [Voucher_Record] SET [Status] = 1 , " +
                                        $"[Received Time] = '{DateTime.Now.TimeOfDay}' , [Received Date] = '{DateTime.Now.Date}' , " +
                                        $"[Received By] = '{Account.User}' , [Payment Method] = 'CASH' WHERE [Voucher ID] = {voucherID}");
                                }
                            }
                            else
                            {
                                using(new WaitCursor())
                                {
                                    feeItem.PaidAmount = (Convert.ToInt32(feeItem.PaidAmount) + Convert.ToInt32(feeItem.AmountToPay)).ToString();
                                    if (feeItem.DueAmount == "0")
                                        feeItem.FeeStatus = "PAID";
                                    DataAccess.ExecuteQuery($"UPDATE Fee_Record SET Paid_Amount = {feeItem.PaidAmount} WHERE Fee_Record_ID = {feeItem.FeeRecordID}");
                                }
                            }

                        }
                    }
                    FeeCount++;
                }
                else
                if (fee.FeeStatus == "PAY" && voucherID.ToString().IsNotNullOrEmpty())
                {
                    if(!fee.IsAmountToPayValid || !fee.IsWholeValid)
                    {
                        DialogManager.ShowValidationMessage();
                        return;
                    }
                    if (fee.IsEditEnable)
                    {
                        EditFee(fee);
                        if (fee.IsEditEnable)
                            return;
                    }
                    if(fee.PaidAmount == "0")
                    {
                        if (voucherID == 0)
                        {
                            var date = DateTime.Now;
                            DataAccess.ExecuteQuery($"INSERT INTO Voucher_Record (Student_ID , [DateTime]) VALUES ({StudentID.Value} , '{date}')");
                            voucherID = (int)DataAccess.GetDataTable($"SELECT [Voucher ID] FROM Voucher_Record WHERE Student_ID = {StudentID.Value} AND [DateTime] = '{date}'").Rows[0][0];
                        }
                        fee.PaidAmount = (Convert.ToInt32(fee.PaidAmount) + Convert.ToInt32(fee.AmountToPay)).ToString();
                        if (fee.DueAmount == "0")
                            fee.FeeStatus = "PAID";

                        DataAccess.ExecuteQuery($"UPDATE Fee_Record SET Paid_Amount = {fee.PaidAmount} , Paid_Voucher_ID = {voucherID} WHERE Fee_Record_ID = {fee.FeeRecordID}");
                        DataAccess.ExecuteQuery($"UPDATE [Voucher_Record] SET [Status] = 1 , " +
                            $"[Received Time] = '{DateTime.Now.TimeOfDay}' , [Received Date] = '{DateTime.Now.Date}' , " +
                            $"[Received By] = '{Account.User}' , [Payment Method] = 'CASH' WHERE [Voucher ID] = {voucherID}");
                    }
                    else
                    {
                        fee.PaidAmount = (Convert.ToInt32(fee.PaidAmount) + Convert.ToInt32(fee.AmountToPay)).ToString();
                        if (fee.DueAmount == "0")
                            fee.FeeStatus = "PAID";
                        DataAccess.ExecuteQuery($"UPDATE Fee_Record SET Paid_Amount = {fee.PaidAmount} WHERE Fee_Record_ID = {fee.FeeRecordID}");
                    }
                }
                else
                {
                    //DataAccess.ExecuteQuery($"UPDATE Fee_Record SET Fee_Status = 0 , Paid_Voucher_ID = NULL WHERE Fee_Record_ID = {fee.FeeRecordID}");
                    //fee.FeeStatus = "PAY";
                }
            }
            //catch(Exception ex)
            //{
            //    Logger.Log(ex, true);
            //}
        }

        private void DeleteFee(object parameter)
        {
            var fee = parameter as FeeEntity;
            if (fee.DeleteButtonIcon == Application.Current.FindResource("DeleteBinIcon") as string)
            {
                if (DialogManager.ShowMessageDialog("Warning", "Are you sure, you want to delete this fee?", true))
                {
                    DataAccess.ExecuteQuery($"DELETE FROM Fee_Record WHERE Fee_Record_ID = {fee.FeeRecordID}");
                    SearchID();
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
                        SearchID();
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

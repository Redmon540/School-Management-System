using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;

namespace School_Manager
{

    public class MonthlyFeeRecord : BasePropertyChanged
    {
        public string Session { get; set; }

        public ObservableCollection<FeeEntity> FeeEntities { get; set; }

        public int TotalAmount
        {
            get => FeeEntities.Select(x => x.Amount.IsNullOrEmpty() ? 0 : int.Parse(x.Amount)).Sum();
        }
        public int TotalDiscount
        {
            get => FeeEntities.Select(x => x.Discount.IsNullOrEmpty() ? 0 : int.Parse(x.Discount)).Sum();
        }
        public int TotalLateFee
        {
            get => FeeEntities.Select(x => x.LateFee.IsNullOrEmpty() ? 0 : int.Parse(x.LateFee)).Sum();
        }
        public int TotalPaidAmount
        {
            get => FeeEntities.Select(x => x.PaidAmount.IsNullOrEmpty() ? 0 : int.Parse(x.PaidAmount)).Sum();
        }
        public int TotalDue
        {
            get => FeeEntities.Select(x => x.DueAmount.IsNullOrEmpty() ? 0 : int.Parse(x.DueAmount)).Sum();
        }
        public int GrandTotal
        {
            get => FeeEntities.Select(x => x.Total.IsNullOrEmpty() ? 0 : int.Parse(x.Total)).Sum();
        }
    }

    public class FeeEntity : BasePropertyChanged
    {
        public FeeEntity Backup { get; set; }

        public string FeeRecordID { get; set; }

        public string FeeID { get; set; }

        public string Fee { get;set; }

        public string Amount { get; set; }

        public string LateFee { get; set; }

        public string DiscountID { get; set; }

        public string Discount { get; set; } = "0";

        public string FeeStatus { get; set; }

        public string DueDate { get; set; }

        public DateTime Date { get; set; }

        public string PaidAmount { get; set; }

        public string DueAmount
        {
            get
            {
                int amount, latefee, discount, paidamount;
                int.TryParse(Amount, out amount);
                int.TryParse(LateFee, out latefee);
                int.TryParse(Discount, out discount);
                int.TryParse(PaidAmount, out paidamount);
                AmountToPay = ((amount + latefee - discount) - paidamount).ToString();
                return ((amount + latefee - discount) - paidamount).ToString();
            }
        }

        public string AmountToPay { get; set; }

        public bool IsAmountToPayValid 
        {
            get
            {
                if (AmountToPay.IsNullOrEmpty())
                    return false;
                int a = 0;
                if (Regex.Match(AmountToPay, @"^[0-9]*$").Success)
                {
                    a = Convert.ToInt32(AmountToPay);
                    if (IsWholeValid)
                    {
                        if (a > Convert.ToInt32(Amount) + Convert.ToInt32(LateFee) - Convert.ToInt32(Discount))
                            return false;
                        else
                            return true;
                    }
                    else
                        return true;
                }
                else
                    return false;
            }
        }

        public string PaidAmountAlert { get; set; } = "Invalid amount";

        public bool IsEditButtonEnable
        {
            get
            {
                if (FeeStatus == "PAID")
                    return false;
                else
                    return true;
            }
            set => IsEditButtonEnable = value;
        }

        public string Total
        {
            get
            {
                int amount, latefee, discount;
                int.TryParse(Amount, out amount);
                int.TryParse(LateFee, out latefee);
                int.TryParse(Discount, out discount);
                return (amount + latefee - discount).ToString();
            }
        }

        public bool IsEditEnable { get; set; } = false;

        public bool IsChanged { get; set; } = false;

        public bool IsChecked { get; set; }

        public bool IsValid
        {
            get
            {
                if (IsFeeValid && IsAmountValid && IsLateFeeValid && DueDate.IsNotNullOrEmpty())
                    return true;
                return false;
            }
        }

        public bool IsWholeValid
        {
            get
            {
                if (IsDiscountValid && IsFeeValid && IsAmountValid && IsLateFeeValid)
                    return true;
                return false;
            }
        }

        public bool IsFeeValid
        {
            get
            {
                if (Fee.IsNullOrEmpty())
                    return false;
                return true;
            }
        }

        public bool IsAmountValid
        {
            get
            {
                if (Amount.IsNullOrEmpty())
                    return false;
                if(Regex.Match(Amount, @"^[0-9]*$").Success)
                {
                    int amount, discount, lateFee, paid;
                    int.TryParse(Amount, out amount);
                    int.TryParse(Discount, out discount);
                    int.TryParse(LateFee, out lateFee);
                    int.TryParse(PaidAmount, out paid);
                    if (amount + lateFee - discount < paid)
                        return false;
                    return true;
                }
                return false;
            }
        }

        public bool IsLateFeeValid
        {
            get
            {
                if (LateFee.IsNullOrEmpty())
                    return false;
                if(Regex.Match(LateFee, @"^[0-9]*$").Success)
                {
                    int amount, discount, lateFee, paid;
                    int.TryParse(Amount, out amount);
                    int.TryParse(Discount, out discount);
                    int.TryParse(LateFee, out lateFee);
                    int.TryParse(PaidAmount, out paid);
                    if (amount + lateFee - discount < paid)
                        return false;
                    return true;
                }
                return false;
            }
        }

        public bool IsDiscountValid
        {
            get
            {
                if (Discount.IsNullOrEmpty())
                    return false;
                if(Regex.Match(Discount, @"^[0-9]*$").Success)
                {
                    int amount, discount, lateFee;
                    int.TryParse(Amount,out amount);
                    int.TryParse(Discount,out discount);
                    int.TryParse(LateFee,out lateFee);
                    if (discount > amount + lateFee)
                        return false;
                    return true;
                }
                return false;
            }
        }

        public bool IsOld { get; set; } = false;

        public string DiscountAlert { get; set; } = "Only numeric data is allowed.";

        public string EditButtonIcon { get; set; } = Application.Current.FindResource("EditIcon") as string;

        public string DeleteButtonIcon { get; set; } = Application.Current.FindResource("DeleteBinIcon") as string;

        public Brush Background { get; set; }

        public string Session { get; set; }

        public string Month { get; set; }

        public string Year { get; set; }

        public void TakeBackup()
        {
            Backup = new FeeEntity();
            Backup.Amount = Amount;
            Backup.Background = Background;
            Backup.Discount = Discount;
            Backup.DiscountAlert = DiscountAlert;
            Backup.DiscountID = DiscountID;
            Backup.DueDate = DueDate;
            Backup.EditButtonIcon = EditButtonIcon;
            Backup.Fee = Fee;
            Backup.FeeID = FeeID;
            Backup.FeeRecordID = FeeRecordID;
            Backup.FeeStatus = FeeStatus;
            Backup.IsChecked = IsChecked;
            Backup.IsEditButtonEnable = IsEditButtonEnable;
            Backup.IsOld = IsOld;
            Backup.LateFee = LateFee;
            Backup.Month = Month;
            Backup.Session = Session;
            Backup.Year = Year;
            Backup.Date = Date;
        }

        public void RestoreBackup()
        {
            Amount = Backup.Amount;
            Background = Backup.Background;
            Discount = Backup.Discount;
            DiscountAlert = Backup.DiscountAlert;
            DiscountID = Backup.DiscountID;
            DueDate = Backup.DueDate;
            EditButtonIcon = Backup.EditButtonIcon;
            Fee = Backup.Fee;
            FeeID = Backup.FeeID;
            FeeRecordID = Backup.FeeRecordID;
            FeeStatus = Backup.FeeStatus;
            IsChecked = Backup.IsChecked;
            IsEditButtonEnable = Backup.IsEditButtonEnable;
            IsOld = Backup.IsOld;
            LateFee = Backup.LateFee;
            Month = Backup.Month;
            Session = Backup.Session;
            Year = Backup.Year;
            Date = Backup.Date;
        }
    }

    public class Session
    {
        public string Month { get; set; }
        public string Year { get; set; }
    }    
}

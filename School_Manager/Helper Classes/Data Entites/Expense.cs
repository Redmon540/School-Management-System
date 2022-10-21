using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;

namespace School_Manager
{
    public class Expense : BasePropertyChanged
    {
        public string ExpenseID { get; set; }
        public string ExpenseType { get; set; }
        public int Amount { get; set; }
        public string Description { get; set; }
        public bool IsEditing { get; set; }
        public string EditIcon { get; set; }
        public bool IsAmountValid
        {
            get
            {
                if (Amount.ToString().IsNullOrEmpty())
                    return false;
                return Regex.Match(Amount.ToString(), @"^[0-9]*$").Success;
            }
        }
        //public Brush Foreground { get; set; } = Helper.GetRandomColor();
    }

    public class ExpenseCollection : BasePropertyChanged
    {
        public ObservableCollection<string> ExpenseTypes { get; set; }
        public string SelectedExpense { get; set; }
        public bool IsEditing { get; set; }
        public string EditIcon { get; set; } = Application.Current.FindResource("EditIcon") as string;
    }

}

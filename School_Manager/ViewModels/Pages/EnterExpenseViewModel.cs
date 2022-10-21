using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace School_Manager
{
    public class EnterExpenseViewModel : BaseViewModel
    {
        #region Constructor

        public EnterExpenseViewModel()
        {

            //set the commands
            CreateExpenseCommand = new RelayCommand(CreateExpense);
            AddExpenseCommand = new RelayCommand(AddExpense);
            EditExpenseCommand = new RelayParameterizedCommand(parameter => EditEpense(parameter));
            DeleteExpenseCommand = new RelayParameterizedCommand(parameter => DeleteExpense(parameter));
            EditExpenseTypeCommand = new RelayCommand(EditEpenseType);
            DeleteExpenseTypeCommand = new RelayCommand(DeleteExpenseType);
            DateChangedCommand = new RelayCommand(LoadExpenses);

            //set properties
            var list = Helper.GetDays();
            list.Insert(0, "All");
            Days = new ListEntity { FeildName = "Day", Items = list, ValidationType = ValidationType.NotEmpty, Value = DateTime.Now.Day.ToString() };
            list = Helper.GetMonths();
            list.Insert(0, "All");
            Months = new ListEntity { FeildName = "Months", Items = list, ValidationType = ValidationType.NotEmpty, Value = Helper.GetMonthName(DateTime.Now.Month) };
            list = Helper.GetYears();
            list.Insert(0, "All");
            Years = new ListEntity { FeildName = "Years", Items = list, ValidationType = ValidationType.NotEmpty, Value = DateTime.Now.Year.ToString() };


            LoadExpenses();
        }

        #endregion

        #region Properties

        private string _SelectedExpense { get; set; }

        public DateTime SelectedDate { get; set; } = DateTime.Now.Date;

        public ListEntity Days { get; set; }

        public ListEntity Months { get; set; }

        public ListEntity Years { get; set; }

        private bool _IsGrouped;

        public bool IsGrouped
        {
            get
            {
                return _IsGrouped;
            }
            set
            {
                _IsGrouped = value;
                LoadExpenses();
            }
        }

        public string ExpenseTitle { get; set; }

        public string Amount { get; set; }

        public string Description { get; set; }

        public ObservableCollection<Expense> Expenses { get; set; }

        public ExpenseCollection ExpenseCollection { get; set; } = new ExpenseCollection { ExpenseTypes = DataAccess.GetExpenses() };

        public int TotalAmount
        {
            get
            {
                if (Expenses == null || Expenses.Count == 0)
                    return 0;
                return Expenses.Sum(x => x.Amount);
            }
        }

        #endregion

        #region Commands

        public ICommand CreateExpenseCommand { get; set; }

        public ICommand AddExpenseCommand { get; set; }

        public ICommand EditExpenseCommand { get; set; }

        public ICommand DeleteExpenseCommand { get; set; }

        public ICommand EditExpenseTypeCommand { get; set; }

        public ICommand DeleteExpenseTypeCommand { get; set; }

        public ICommand DateChangedCommand { get; set; }

        #endregion

        #region Command Methods

        private void LoadExpenses()
        {
            Expenses = DataAccess.LoadExpenses(Days.Value , Months.Value , Years.Value, IsGrouped);
        }

        private void CreateExpense()
        {
            if (ExpenseTitle.IsNullOrEmpty())
            {
                DialogManager.ShowMessageDialog("Warning", "Expense Title cannot be empty.",DialogTitleColor.Red);
                return;
            }

            if(DataAccess.GetDataTable($"SELECT * FROM Expense_Type WHERE Expense_Title = '{ExpenseTitle}'").Rows.Count == 0)
            {
                DataAccess.CreateExpense(ExpenseTitle);
                ExpenseTitle = "";
                ExpenseCollection.ExpenseTypes = DataAccess.GetExpenses();
            }
            else
            {
                DialogManager.ShowMessageDialog("Warning", "Please select a different name, this expense already exists.",DialogTitleColor.Red);
            }
        }

        private void AddExpense()
        {
            if (ExpenseCollection.SelectedExpense.IsNullOrEmpty() || Amount.IsNullOrEmpty() || !Regex.Match(Amount, @"^[0-9]*$").Success)
            {
                DialogManager.ShowValidationMessage();
                return;
            }

            DataAccess.AddExpense(ExpenseCollection.SelectedExpense, Amount, Description, SelectedDate.Date.ToShortDateString());
            LoadExpenses();
            Amount = "";
            Description = "";
        }

        private void EditEpense(object sender)
        {
            var expense = sender as Expense;
            //to commit edit
            if(expense.IsEditing)
            {
                if (!expense.IsAmountValid)
                {
                    DialogManager.ShowValidationMessage();
                    return;
                }
                DataAccess.EditExpense(expense.ExpenseID, expense.ExpenseType, expense.Amount.ToString(), expense.Description);
                expense.EditIcon = Application.Current.FindResource("EditIcon") as string;
                expense.IsEditing = false;
            }
            //to start editing
            else
            {
                expense.EditIcon = Application.Current.FindResource("CheckIcon") as string;
                expense.IsEditing = true;
            }
        }

        private void EditEpenseType()
        {
            //to commit edit
            if(ExpenseCollection.IsEditing)
            {
                if(ExpenseCollection.SelectedExpense.IsNotNullOrEmpty())
                {
                    //if(DataAccess.GetDataTable($"SELECT * FROM Expense_Type WHERE Expense_Title = '{ExpenseCollection.SelectedExpense}' ").Rows.Count == 0)
                    {
                        DataAccess.ExecuteQuery($"UPDATE Expense_Type SET Expense_Title = '{ExpenseCollection.SelectedExpense}' WHERE Expense_Title = '{_SelectedExpense}'");
                        ExpenseCollection.EditIcon = Application.Current.FindResource("EditIcon") as string;
                        ExpenseCollection.IsEditing = false;
                        ExpenseCollection.ExpenseTypes = DataAccess.GetExpenses();
                    }
                    //else
                    //{
                    //    DialogManager.ShowMessageDialog("Warning", "This expense already exists, please use a different name.");
                    //}
                }
                else
                {
                    DialogManager.ShowMessageDialog("Warning", "Invalid Expense Title.",DialogTitleColor.Red);
                    ExpenseCollection.SelectedExpense = _SelectedExpense;
                }
                
            }
            //to start editing
            else
            {
                if(ExpenseCollection.SelectedExpense.IsNullOrEmpty())
                {
                    DialogManager.ShowMessageDialog("Warning", "Please select an expense to edit.",DialogTitleColor.Red);
                    return;
                }
                _SelectedExpense = ExpenseCollection.SelectedExpense;
                ExpenseCollection.EditIcon = Application.Current.FindResource("CheckIcon") as string;
                ExpenseCollection.IsEditing = true;
            }
        }

        private void DeleteExpenseType()
        {
            if (DialogManager.ShowMessageDialog("Warning", "Are you sure, you want to delete this expense?", true))
            {
                if (DataAccess.GetDataTable($"SELECT * FROM Expenses LEFT JOIN Expense_Type ON " +
                    $"Expense_Type.Expense_Type_ID = Expenses.Expense_Type_ID WHERE Expense_Title = '{ExpenseCollection.SelectedExpense}'").Rows.Count == 0)
                {
                    DataAccess.ExecuteQuery($"DELETE FROM Expense_Type WHERE Expense_Title = '{ExpenseCollection.SelectedExpense}' ");
                    ExpenseCollection.ExpenseTypes = DataAccess.GetExpenses();
                }
                else
                {
                    DialogManager.ShowMessageDialog("Warning", "Cannot delete this expense, because its used in previous records.",DialogTitleColor.Red);
                }
            }
        }

        private void DeleteExpense(object sender)
        {
            if(DialogManager.ShowMessageDialog("Warning","Are you sure, you want to delete this expense?",true))
            {
                DataAccess.ExecuteQuery($"DELETE FROM Expenses WHERE Expense_ID = {(sender as Expense).ExpenseID}");
                LoadExpenses();
            }
        }

        #endregion
    }
}

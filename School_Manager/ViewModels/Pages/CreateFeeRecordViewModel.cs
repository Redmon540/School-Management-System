using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows.Input;

namespace School_Manager
{
    public class CreateFeeRecordViewModel : BaseViewModel
    {
        #region Default Constructor

        public CreateFeeRecordViewModel()
        {
            //initialize the properties
            CustomFee = new ObservableCollection<FeeEntity>();
            FeeEntities = new ObservableCollection<FeeEntity>();
            DueDates = new ObservableCollection<string>()
            {
                "1","2","3","4","5","6","7","8","9","10",
                "11","12","13","14","15","16","17","18","19","20",
                "21","22","23","24","25","26","27","28","29","30","31"
            };
            var list = DataAccess.GetClassNames();
            list.Insert(0, "All");
            Classes = new ListEntity { FeildName = "Class", Items = list, ValidationType = ValidationType.NotEmpty };

            //initialize commands
            AddCustomFee = new RelayCommand(AddCustomColumns);
            RemoveFeeCommand = new RelayParameterizedCommand(parameter => RemoveFeeEntity(parameter));
            ClassChangedCommand = new RelayCommand(ClassChanged);
            CreateRecordCommand = new RelayCommand(CreateRecord);
            MakeDeafultCommand = new RelayCommand(MakeDefault);
            AddFeeCommand = new RelayCommand(AddFees);
        }

        #endregion

        #region Public Properties

        public ListEntity Classes { get; set; }

        public ListEntity Months { get; set; } = new ListEntity { FeildName = "Month", Items = Helper.GetMonths(), Value = Helper.GetMonthName(DateTime.Now.Month) , ValidationType = ValidationType.NotEmpty };

        public ListEntity Years { get; set; } = new ListEntity { FeildName = "Year", Items = Helper.GetYears(), Value = DateTime.Now.Year.ToString(),  ValidationType = ValidationType.NotEmpty };

        public ObservableCollection<string> DueDates { get; set; }

        public ObservableCollection<FeeEntity> FeeEntities { get; set; }

        public ObservableCollection<FeeEntity> CustomFee { get; set; }

        public bool IsFeeStructureVisible { get; set; } = true;

        #endregion

        #region Public Commands

        public ICommand ClassChangedCommand { get; set; }
       
        public ICommand CreateRecordCommand { get; set; }
       
        public ICommand AddCustomFee { get; set; }

        public ICommand AddFeeCommand { get; set; }

        public ICommand RemoveFeeCommand { get; set; }

        public ICommand MakeDeafultCommand { get; set; }

        public ICommand SessionChangedCommand { get; set; }

        #endregion

        #region Command Methods

        private void AddFees()
        {
            try
            {
                bool isValid = true;
                foreach (FeeEntity item in CustomFee)
                {
                    if (!item.IsValid)
                    {
                        isValid = false;
                        break;
                    }
                }
                if (!isValid || CustomFee.Count == 0 || !Classes.IsValid)
                {
                    DialogManager.ShowValidationMessage();
                    return;
                }

                foreach (var item in CustomFee)
                {
                    if (DataAccess.GetDataTable($"SELECT * FROM Fee_Structure WHERE Class_ID = {DataAccess.GetClassID(Classes.Value)} AND Fee = '{item.Fee}'").Rows.Count == 0)
                    {
                        DataAccess.ExecuteQuery($"INSERT INTO Fee_Structure (Class_ID , Fee , Amount , Late_Fee , Due_Date )" +
                            $"VALUES ({DataAccess.GetClassID(Classes.Value)} , '{item.Fee}' , {item.Amount} , {item.LateFee} , {item.DueDate} )");
                    }
                    else
                    {
                        DialogManager.ShowMessageDialog("Warning", $"{item.Fee} already exists. Please select a different fee name.",DialogTitleColor.Red);
                    }
                }
                CustomFee.Clear();
                ClassChanged();
                DialogManager.ShowMessageDialog("Warning", "Fee Added Successfully.",DialogTitleColor.Green);
            }
            catch(Exception ex)
            {
                Logger.Log(ex, true);
            }            
        }

        private void AddCustomColumns()
        {
            try
            {
                CustomFee.Add(new FeeEntity());
            }
            catch(Exception ex)
            {
                Logger.Log(ex, true);
            }
        }

        private void RemoveFeeEntity(object parameter)
        {
            try
            {
                CustomFee.Remove((FeeEntity)parameter);
            }
            catch(Exception ex)
            {
                Logger.Log(ex, true);
            }
        }

        private void ClassChanged()
        {
            try
            {
                if (Classes.Value == "All")
                {
                    IsFeeStructureVisible = false;
                }
                else
                {
                    IsFeeStructureVisible = true;
                    FeeEntities.Clear();
                    var classID = DataAccess.GetClassID(Classes.Value);
                    DataTable fee_Strucutre = DataAccess.GetDataTable($"SELECT [Fee_ID] , [Fee] , [Amount] , [Due_Date], [Late_Fee] FROM Fee_Structure WHERE Class_ID = {classID}");
                    DataTable table = DataAccess.GetDataTable($"SELECT [Default_Fee_Columns] FROM ColumnInformation WHERE [Class_ID] = {classID} AND [Default_Fee_Columns] IS NOT NULL");
                    List<string> defaultFeeColumns = new List<string>();
                    if (table.Rows.Count != 0)
                    {
                        defaultFeeColumns = table.Rows[0][0].ToString().Split(',').ToList();
                        foreach (DataRow row in fee_Strucutre.Rows)
                        {
                            FeeEntities.Add(new FeeEntity()
                            {
                                Fee = row["Fee"].ToString(),
                                Amount = row["Amount"].ToString(),
                                FeeID = row["Fee_ID"].ToString(),
                                DueDate = row["Due_Date"].ToString(),
                                LateFee = row["Late_Fee"].ToString(),
                                IsChecked = defaultFeeColumns.Contains(row["Fee"].ToString())
                            });
                        }
                    }
                    else
                    {
                        foreach (DataRow row in fee_Strucutre.Rows)
                        {
                            FeeEntities.Add(new FeeEntity()
                            {
                                Fee = row["Fee"].ToString(),
                                Amount = row["Amount"].ToString(),
                                FeeID = row["Fee_ID"].ToString(),
                                DueDate = row["Due_Date"].ToString(),
                                LateFee = row["Late_Fee"].ToString(),
                                IsChecked = true
                            });
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Logger.Log(ex, true);
            }
        }

        private void CreateRecord()
        {
            try
            {
                if (!Classes.IsValid || !Months.IsValid || !Years.IsValid)
                {
                    DialogManager.ShowMessageDialog("Warning!", "Please select all the required information.",DialogTitleColor.Red);
                    return;
                }
                //if all classes are selected
                if(Classes.Value == "All")
                {
                    var createFee = "";
                    var month = Helper.GetMonthNumber(Months.Value);
                    var year = Years.Value;
                    var classIDs = DataAccess.GetDataTable($"SELECT [Class_ID] FROM Classes").AsEnumerable().Select(x=> x["Class_ID"].ToString()).Distinct().ToList();
                    foreach (var classID in classIDs)
                    {
                        var feeStructure = DataAccess.GetDataTable($"SELECT * FROM Fee_Structure WHERE Class_ID = {classID}");
                        var defaultFeeColumns = DataAccess.GetDataTable($"SELECT [Default_Fee_Columns] FROM ColumnInformation " +
                            $"WHERE [Class_ID] = {classID} ")
                            .Rows[0][0].ToString().Split(new char[] {','},StringSplitOptions.RemoveEmptyEntries).ToList();
                        if(defaultFeeColumns.Count == 0)
                        {
                            foreach (DataRow fee in feeStructure.Rows)
                            {
                                if(DataAccess.GetDataTable($"SELECT * FROM Created_Fee_Records " +
                                $"WHERE Class_ID = {classID} AND Fee_ID = {fee["Fee_ID"]} AND Month = {Helper.GetMonthNumber(Months.Value)} " +
                                $"AND Year = {Years.Value}").Rows.Count == 0)
                                {
                                    DataAccess.CreateFeeRecord(fee, classID, month.ToString(), year);
                                }
                            }
                        }
                        else
                        {
                            foreach (DataRow fee in feeStructure.Rows)
                            {
                                if (defaultFeeColumns.Contains(fee["Fee"].ToString()))
                                {
                                    if (DataAccess.GetDataTable($"SELECT * FROM Created_Fee_Records " +
                                $"WHERE Class_ID = {classID} AND Fee_ID = {fee["Fee_ID"]} AND Month = {Helper.GetMonthNumber(Months.Value)} " +
                                $"AND Year = {Years.Value}").Rows.Count == 0)
                                    {
                                        DataAccess.CreateFeeRecord(fee, classID, month.ToString(), year);
                                    }
                                    else
                                    {
                                        createFee += $", {fee["Fee"]}";
                                    }
                                }
                            }
                        }
                    }
                    DialogManager.ShowMessageDialog("Message", "Fee record for all classes created successfully.",DialogTitleColor.Green);
                }
                //if a specific class is selected
                else
                {
                    var classID = DataAccess.GetClassID(Classes.Value);
                    var fee = FeeEntities.Select(x => x).Where(x => x.IsChecked).ToList();
                    if (fee.Count == 0)
                    {
                        DialogManager.ShowMessageDialog("Warning", "Please select atleast one fee to create record.",DialogTitleColor.Red);
                        return;
                    }
                    var createdFees = fee.Where(x => DataAccess.GetDataTable($"SELECT * FROM Created_Fee_Records " +
                        $"WHERE Class_ID = {classID} AND Fee_ID = {x.FeeID} AND Month = {Helper.GetMonthNumber(Months.Value)} " +
                        $"AND Year = {Years.Value}").Rows.Count != 0)
                        .Select(x => x.Fee).ToList();
                    if (createdFees.Count == 0)
                    {
                        var month = Helper.GetMonthNumber(Months.Value);
                        var year = Years.Value;
                        foreach (FeeEntity item in FeeEntities)
                        {
                            if (item.IsChecked)
                            {
                                DataAccess.CreateFeeRecord(item, classID, month.ToString(), year);
                            }
                        }
                        DialogManager.ShowMessageDialog("Message", "Fee Record Created Successfully!",DialogTitleColor.Green);
                        FeeEntities.Clear();
                        CustomFee.Clear();
                        ClassChanged();
                    }
                    else
                    {
                        var createdfee = "";
                        foreach (var item in createdFees)
                        {
                            createdfee += $", {item}";
                        }
                        if (createdfee.IsNotNullOrEmpty())
                            createdfee = createdfee.Remove(0, 1);
                        DialogManager.ShowMessageDialog("Warning", $"{createdfee} has already been created. Please select different fee or month.",DialogTitleColor.Red);
                    }
                }
            }
            catch(Exception ex)
            {
                Logger.Log(ex, true);
            }
        }

        private void MakeDefault()
        {
            try
            {
                string feeColumns = "";
                foreach (FeeEntity checkBox in FeeEntities)
                {
                    if (checkBox.IsChecked == true)
                        feeColumns += $",{checkBox.Fee}";
                }
                if (!string.IsNullOrEmpty(feeColumns))
                {
                    feeColumns = feeColumns.Remove(0, 1);
                    DataAccess.ExecuteQuery($"UPDATE ColumnInformation SET [Default_Fee_Columns] = '{feeColumns}' " +
                        $"WHERE [Class_ID] = {DataAccess.GetClassID(Classes.Value)}");
                }
            }
            catch(Exception ex)
            {
                Logger.Log(ex, true);
            }
        }

        #endregion
    }
}
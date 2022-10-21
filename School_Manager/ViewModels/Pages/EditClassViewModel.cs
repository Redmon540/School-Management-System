using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace School_Manager
{
    public class EditClassViewModel : BaseViewModel
    {
        #region Default Constructor

        public EditClassViewModel()
        {
            DueDates = new ObservableCollection<string>()
            {
                "1","2","3","4","5","6","7","8","9","10",
                "11","12","13","14","15","16","17","18","19","20",
                "21","22","23","24","25","26","27","28","29","30","31"
            };

            Sections = new ObservableCollection<TextEntity>();
            ClassName = new TextEntity { FeildName = "Rename Class", ValidationType = ValidationType.ClassName};
            FeeInformation = new ObservableCollection<FeeEntity>();
            Classes = new ListEntity
            {
                FeildName = "Class",
                Items = DataAccess.GetClassNames(),
            };

            //set the commands
            AddFeeCommand = new RelayCommand(AddFee);
            RemoveFeeCommand = new RelayParameterizedCommand(parameter => RemoveFee(parameter));
            AddSectionCommand = new RelayCommand(AddSection);
            RemoveSectionCommand = new RelayParameterizedCommand(parameter => RemoveSection(parameter));
            UpdateClassCommand = new RelayCommand(UpdateClass);
            ClassChangedCommand = new RelayCommand(ClassChanged);
            EditClassCommand = new RelayCommand(EditClass);
            CancleEditingCommand = new RelayCommand(CancleEditingClass);
            DeleteClassCommand = new RelayCommand(DeleteClass);
        }

        #endregion

        #region Public Properties

        public TextEntity ClassName { get; set; }

        public ListEntity Classes { get; set; }

        public bool IsEditingClass { get; set; }

        public string EditIcon { get; set; } = Application.Current.FindResource("EditIcon") as string;

        public ObservableCollection<FeeEntity> FeeInformation { get; set; }

        public ObservableCollection<string> DueDates { get; set; }

        public ObservableCollection<TextEntity> Sections { get; set; }

        #endregion

        #region Commands

        public ICommand UpdateClassCommand { get; set; }

        public ICommand AddFeeCommand { get; set; }

        public ICommand RemoveFeeCommand { get; set; }

        public ICommand MessageBarActionCommand { get; set; }

        public ICommand AddSectionCommand { get; set; }

        public ICommand RemoveSectionCommand { get; set; }

        public ICommand ClassChangedCommand { get; set; }

        public ICommand EditClassCommand { get; set; }

        public ICommand CancleEditingCommand { get; set; }

        public ICommand DeleteClassCommand { get; set; }
        
        #endregion

        #region Command Methods

        private void AddFee()
        {
            try
            {
                FeeInformation.Add(new FeeEntity());
            }
            catch(Exception ex)
            {
                Logger.Log(ex, true);
            }
        }

        private void RemoveFee(object sender)
        {
            try
            {
                var fee = sender as FeeEntity;
                bool result = true;
                if (fee.IsOld)
                {
                    result = DialogManager.ShowMessageDialog("Warning", "Are you sure? You want to delete this fee permenantly.", true);
                   if (result)
                   {
                        DataAccess.ExecuteQuery($"DELETE FROM Fee_Structure WHERE Fee_ID = {fee.FeeID}");
                   }
                }
                if(result)
                    FeeInformation.Remove(sender as FeeEntity);
            }
            catch(Exception ex)
            {
                Logger.Log(ex, true);
            }
        }

        private void AddSection()
        {
            try
            {
                Sections.Add(new TextEntity() { ValidationType = ValidationType.NotEmpty });
            }
            catch(Exception ex)
            {
                Logger.Log(ex, true);
            }
        }

        private void RemoveSection(object sender)
        {
            try
            {
                var section = sender as TextEntity;
                bool result = true;
                if (section.IsOld)
                {
                    result = DialogManager.ShowMessageDialog("Warning", "Are you sure? You want to delete this section permenantly.", true);
                    if (result)
                    {
                        DataAccess.ExecuteQuery($"DELETE FROM Sections WHERE Section_ID = {section.SectionID}");
                    }
                }
                if (result)
                    Sections.Remove(sender as TextEntity);
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
                if (Classes.Value.IsNullOrEmpty())
                    return;
                ClassName.Value = Classes.Value;
                //get the sections
                Sections = new ObservableCollection<TextEntity>( DataAccess.GetDataTable($"SELECT [Section_ID] , [Section] FROM Classes JOIN Sections ON Sections.Class_ID = Classes.Class_ID WHERE CLasses.Class_ID = {DataAccess.GetClassID(Classes.Value)}").
                    AsEnumerable().Select(x => new TextEntity { FeildName = x["Section"].ToString(), Value = x["Section"].ToString(),
                        ValidationType = ValidationType.NotEmpty , SectionID = x["Section_ID"].ToString() , IsOld = true}).ToList());
                //get the fee structure
                FeeInformation = new ObservableCollection<FeeEntity>(DataAccess.GetDataTable($"SELECT * FROM Fee_Structure WHERE Class_ID = {DataAccess.GetClassID(Classes.Value)}")
                    .AsEnumerable().Select(x => new FeeEntity
                    {
                        Fee = x["Fee"].ToString(),
                        Amount = x["Amount"].ToString(),
                        LateFee = x["Late_Fee"].ToString(),
                        DueDate = x["Due_Date"].ToString(),
                        FeeID = x["Fee_ID"].ToString(),
                        IsOld = true
                    }).ToList());
            }
            catch(Exception ex)
            {
                Logger.Log(ex, true);
            }
        }

        private void EditClass()
        {
            try
            {
                if(EditIcon == Application.Current.FindResource("EditIcon") as string)
                {
                    EditIcon = Application.Current.FindResource("CheckIcon") as string;
                    IsEditingClass = true;
                }
                else
                {
                    if (ClassName.Value != Classes.Value)
                    {
                        //update class name
                        SqlCommand sqlCommand = new SqlCommand();
                        sqlCommand.CommandText = $"UPDATE Classes SET [Class] = @className WHERE Class_ID = {DataAccess.GetClassID(Classes.Value)}";
                        sqlCommand.Parameters.Add("@className", SqlDbType.NVarChar).Value = ClassName.Value;
                        DataAccess.ExecuteQuery(sqlCommand);
                        Classes.Items = DataAccess.GetClassNames();
                        Classes.Value = ClassName.Value;
                    }
                    EditIcon = Application.Current.FindResource("EditIcon") as string;
                    IsEditingClass = false;
                }
            }
            catch(Exception ex)
            {
                Logger.Log(ex, true);
            }
        }

        private void CancleEditingClass()
        {
            try
            {
                EditIcon = Application.Current.FindResource("EditIcon") as string;
                IsEditingClass = false;
            }
            catch (Exception ex)
            {
                Logger.Log(ex, true);
            }
        }

        private void DeleteClass()
        {
            try
            {
                if(Classes.IsValid && Classes.Value.IsNotNullOrEmpty())
                {
                    if(DialogManager.ShowMessageDialog("Warning","Are you sure, you want to delete this class permenantly?",true))
                    {
                        var classID = DataAccess.GetClassID(Classes.Value);
                        DataAccess.ExecuteQuery($"DELETE FROM Fee_Structure WHERE Class_ID = {classID}");
                        DataAccess.ExecuteQuery($"DELETE FROM Sections WHERE Class_ID = {classID}");
                        DataAccess.ExecuteQuery($"DELETE FROM Classes WHERE Class = '{Classes.Value}'");
                        DialogManager.ShowMessageDialog("Message", "Class deleted successfully.",DialogTitleColor.Red);
                        Classes.Items = DataAccess.GetClassNames();
                        FeeInformation = new ObservableCollection<FeeEntity>();
                        Sections = new ObservableCollection<TextEntity>();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex, true);
            }
        }

        private void UpdateClass()
        {
            try
            {
                if(Classes.Value.IsNullOrEmpty())
                {
                    DialogManager.ShowMessageDialog("Warning", "Please select a class before proceeding.",DialogTitleColor.Red);
                    return;
                }
                //if forms contain an error then show the invalid input message
                if (FeeInformation.Select(s => s).Where(s => s.IsValid == false).ToList().Count != 0 ||
                Sections.Select(s => s).Where(s => s.IsValid == false).ToList().Count != 0 || !ClassName.IsValid)
                {
                    DialogManager.ShowValidationMessage();
                }
                else
                {
                    using (new WaitCursor())
                    {
                        if (ClassName.Value != Classes.Value)
                        {
                            //update class name
                            SqlCommand sqlCommand = new SqlCommand();
                            sqlCommand.CommandText = $"UPDATE Classes SET [Class] = @className WHERE Class_ID = {DataAccess.GetClassID(Classes.Value)}";
                            sqlCommand.Parameters.Add("@className", SqlDbType.NVarChar).Value = ClassName.Value;
                            DataAccess.ExecuteQuery(sqlCommand);
                            Classes.Items = DataAccess.GetClassNames();
                        }
                        //update sections
                        foreach (var section in Sections)
                        {
                            if (section.IsOld)
                            {
                                SqlCommand sqlCommand = new SqlCommand();
                                sqlCommand.CommandText = $"UPDATE Sections SET [Section] = @section WHERE Section_ID = {section.SectionID}";
                                sqlCommand.Parameters.Add("@section", SqlDbType.NVarChar).Value = section.Value;
                                DataAccess.ExecuteQuery(sqlCommand);
                            }
                            else
                            {
                                SqlCommand sqlCommand = new SqlCommand();
                                sqlCommand.CommandText = "INSERT INTO Sections([Class_ID] , [Section]) VALUES ( @classID , @section)";
                                sqlCommand.Parameters.Add("@classID", SqlDbType.NVarChar).Value = DataAccess.GetClassID(ClassName.Value);
                                sqlCommand.Parameters.Add("@section", SqlDbType.NVarChar).Value = section.Value;
                                DataAccess.ExecuteQuery(sqlCommand);
                            }
                        }

                        //update the feeses
                        foreach (var fee in FeeInformation)
                        {
                            if (fee.IsOld)
                            {
                                SqlCommand sqlCommand = new SqlCommand();
                                sqlCommand.CommandText = "UPDATE Fee_Structure SET Fee = @fee , Amount = @amount , [Late_Fee] = @lateFee , [Due_Date] = @dueDate WHERE Fee_ID = @feeID";
                                sqlCommand.Parameters.Add("@fee", SqlDbType.NVarChar).Value = fee.Fee;
                                sqlCommand.Parameters.Add("@amount", SqlDbType.NVarChar).Value = fee.Amount;
                                sqlCommand.Parameters.Add("@lateFee", SqlDbType.NVarChar).Value = fee.LateFee;
                                sqlCommand.Parameters.Add("@dueDate", SqlDbType.NVarChar).Value = fee.DueDate;
                                sqlCommand.Parameters.Add("@feeID", SqlDbType.NVarChar).Value = fee.FeeID;
                                DataAccess.ExecuteQuery(sqlCommand);
                            }
                            else
                            {
                                SqlCommand sqlCommand = new SqlCommand();
                                sqlCommand.CommandText = "INSERT INTO Fee_Structure (Class_ID, Fee , Amount ,[Late_Fee] , [Due_Date]) " +
                                    $" VALUES ( @classID , @fee , @amount , @lateFee , @dueDate )";
                                sqlCommand.Parameters.Add("@classID", SqlDbType.NVarChar).Value = DataAccess.GetClassID(ClassName.Value);
                                sqlCommand.Parameters.Add("@fee", SqlDbType.NVarChar).Value = fee.Fee;
                                sqlCommand.Parameters.Add("@amount", SqlDbType.NVarChar).Value = fee.Amount;
                                sqlCommand.Parameters.Add("@lateFee", SqlDbType.NVarChar).Value = fee.LateFee;
                                sqlCommand.Parameters.Add("@dueDate", SqlDbType.NVarChar).Value = fee.DueDate;
                                DataAccess.ExecuteQuery(sqlCommand);
                            }
                        }
                    }
                    DialogManager.ShowMessageDialog("Message", "Class Updated Successfully.",DialogTitleColor.Green);
                    FeeInformation = new ObservableCollection<FeeEntity>();
                    Sections = new ObservableCollection<TextEntity>();
                    ClassName.Value = "";
                    Classes.Value = null;
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex, true);
            }
        }

        #endregion
    }

}

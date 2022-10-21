using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Input;

namespace School_Manager
{
    public class CreateClassViewModel : BaseViewModel
    {
        #region Default Constructor

        public CreateClassViewModel()
        {
            //set the properties
            DueDates = new ObservableCollection<string>()
            {
                "1","2","3","4","5","6","7","8","9","10",
                "11","12","13","14","15","16","17","18","19","20",
                "21","22","23","24","25","26","27","28","29","30","31"
            };

            Sections = new ObservableCollection<TextEntity>();
            FeeInformation = new ObservableCollection<FeeEntity>();
            ClassName = new TextEntity { FeildName = "Enter Class Name" , ValidationType = ValidationType.ClassName};

            //set the commands
            AddFeeCommand = new RelayCommand(AddFee);
            RemoveFeeCommand = new RelayParameterizedCommand(parameter => RemoveFee(parameter));
            AddSectionCommand = new RelayCommand(AddSection);
            RemoveSectionCommand = new RelayParameterizedCommand(parameter => RemoveSection(parameter));
            CreateClassCommand = new RelayCommand(CreateClassCommandMethod);
        }

        #endregion

        #region Public Properties

        public TextEntity ClassName { get; set; }

        public ObservableCollection<FeeEntity> FeeInformation { get; set; }

        public ObservableCollection<string> DueDates { get; set; }

        public ObservableCollection<TextEntity> Sections { get; set; }

        #endregion

        #region Commands

        public ICommand CreateClassCommand { get; set; }

        public ICommand AddFeeCommand { get; set; }

        public ICommand RemoveFeeCommand { get; set; }

        public ICommand MessageBarActionCommand { get; set; }

        public ICommand AddSectionCommand { get; set; }

        public ICommand RemoveSectionCommand { get; set; }

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
                Sections.Add(new TextEntity() { ValidationType = ValidationType.NotEmpty});
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
                Sections.Remove(sender as TextEntity);
            }
            catch(Exception ex)
            {
                Logger.Log(ex, true);
            }
        }

        private void CreateClassCommandMethod()
        {
            //if forms contain an error then show the invalid input message
            if (FeeInformation.Select(s => s).Where(s => s.IsValid == false).ToList().Count != 0 ||
            Sections.Select(s => s).Where(s => s.IsValid == false).ToList().Count != 0 || !ClassName.IsValid )
            {
                DialogManager.ShowValidationMessage();
            }
            else
            if (DataAccess.DoesClassExist(ClassName.Value))
            {
                DialogManager.ShowMessageDialog("Warning!", "The class already exist, please select a different class name.",DialogTitleColor.Red);
            }
            else
            {
                try
                {
                    using (new WaitCursor())
                    {
                        //Creates the class
                        SqlCommand sqlCommand = new SqlCommand();
                        sqlCommand.CommandText = $"INSERT INTO Classes([Class]) VALUES (@className)";
                        sqlCommand.Parameters.Add("@className", SqlDbType.NVarChar).Value = ClassName.Value;
                        DataAccess.ExecuteQuery(sqlCommand);

                        //Creates the sections
                        foreach (var section in Sections)
                        {
                            sqlCommand = new SqlCommand();
                            sqlCommand.CommandText = "INSERT INTO Sections([Class_ID] , [Section]) VALUES ( @classID , @section)";
                            sqlCommand.Parameters.Add("@classID", SqlDbType.NVarChar).Value = DataAccess.GetClassID(ClassName.Value);
                            sqlCommand.Parameters.Add("@section", SqlDbType.NVarChar).Value = section.Value;
                            DataAccess.ExecuteQuery(sqlCommand);
                        }

                        //Create the feeses
                        foreach (var fee in FeeInformation)
                        {
                            sqlCommand = new SqlCommand();
                            sqlCommand.CommandText = "INSERT INTO Fee_Structure (Class_ID , Fee , Amount ,[Late_Fee] , [Due_Date]) " +
                                $" VALUES ( @classID, @fee , @amount , @lateFee , @dueDate )";
                            sqlCommand.Parameters.Add("@classID", SqlDbType.NVarChar).Value = DataAccess.GetClassID(ClassName.Value);
                            sqlCommand.Parameters.Add("@fee", SqlDbType.NVarChar).Value = fee.Fee;
                            sqlCommand.Parameters.Add("@amount", SqlDbType.NVarChar).Value = fee.Amount;
                            sqlCommand.Parameters.Add("@lateFee", SqlDbType.NVarChar).Value = fee.LateFee;
                            sqlCommand.Parameters.Add("@dueDate", SqlDbType.NVarChar).Value = fee.DueDate;
                            DataAccess.ExecuteQuery(sqlCommand);
                        }
                        sqlCommand = new SqlCommand();
                        sqlCommand.CommandText = "INSERT INTO [ColumnInformation] (Class_ID) VALUES (@class)";
                        sqlCommand.Parameters.Add(new SqlParameter() { ParameterName = "@class", Value = DataAccess.GetClassID(ClassName.Value) });
                        DataAccess.ExecuteQuery(sqlCommand);

                    }
                        DialogManager.ShowMessageDialog("Message", "Class Created Successfully.",DialogTitleColor.Green);
                        FeeInformation = new ObservableCollection<FeeEntity>();
                        Sections = new ObservableCollection<TextEntity>();
                        ClassName.Value = "";
                }
                catch (Exception ex)
                {
                    Logger.Log(ex, true);
                }
            }

        }

        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace School_Manager
{
    public class AdmitTeacherViewModel : BaseViewModel
    {
        #region Dafault Constructor

        public AdmitTeacherViewModel()
        {
            //initialize properties
            TeacherEntites = new ObservableCollection<TextEntity>();
            SetTeacherEntitiesPanel();

            //initialize commands
            TeacherPhotoCommand = new RelayCommand(TeacherPhotoUpdate);
            AdmitTeacherCommand = new RelayCommand(AdmitTeacherMethod);
            ResetTeacherPanelCommand = new RelayCommand(ResetTeacherPanel);
        }

        #endregion

        #region Public Properties

        public BitmapImage TeacherPhoto { get; set; }

        public ObservableCollection<TextEntity> TeacherEntites { get; set; }

        public bool IsPhotoPanelVisible { get; set; }

        #endregion

        #region Private Members

        /// <summary>
        /// Teacher photo path on the hard drive
        /// </summary>
        private string TeacherPhotoPath;

        #endregion

        #region Commands

        public ICommand TeacherPhotoCommand { get; set; }

        public ICommand AdmitTeacherCommand { get; set; }

        public ICommand ResetTeacherPanelCommand { get; set; }

        #endregion

        #region Command Methods

        /// <summary>
        /// Opens a file dialog to select photo from drive
        /// </summary>
        private void TeacherPhotoUpdate()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files(*.BMP;*.JPG)|*.BMP;*.JPG|All files (*.*)|*.*";
            openFileDialog.ShowDialog();
            if (!string.IsNullOrEmpty(openFileDialog.FileName))
            {
                TeacherPhotoPath = openFileDialog.FileName;
                TeacherPhoto = new BitmapImage(new Uri(TeacherPhotoPath));
            }
        }

        /// <summary>
        /// resets the teacher panel
        /// </summary>
        private void ResetTeacherPanel()
        {
            foreach (TextEntity item in TeacherEntites)
            {
                item.Value = "";
            }
            TeacherPhoto = null;
            TeacherPhotoPath = string.Empty;
            CollectionViewSource.GetDefaultView(TeacherEntites).Refresh();
        }

        private void AdmitTeacherMethod()
        {
            try
            {
                bool isValid = true;
                foreach (var item in TeacherEntites)
                {
                    if (!item.IsValid)
                    {
                        isValid = false;
                        break;
                    }
                }
                if (!isValid)
                {
                    DialogManager.ShowValidationMessage();
                    return;
                }

                string columns = "";
                string values = "";
                string _TeacherIdName = DataAccess.GetTeacherID();
                SqlCommand sqlCmd = new SqlCommand();
                int var = 1;

                foreach (TextEntity item in TeacherEntites)
                {
                    if (item.FeildName != _TeacherIdName && item.Value.IsNotNullOrEmpty())
                    {
                        if (item.FeildName == DataAccess.GetTeacherClassIDName())
                        {
                            columns += $",[Class_ID]";
                            values += $",@var{var}";
                            sqlCmd.Parameters.Add(new SqlParameter() { ParameterName = $"@var{var++}", Value = item.Value.IsNullOrEmpty() ? "" : item.Value == "None" ? "" : DataAccess.GetClassID(item.Value) });
                        }
                        else
                        {
                            columns += $",[{item.FeildName}]";
                            values += $",@var{var}";
                            sqlCmd.Parameters.Add(new SqlParameter() { ParameterName = $"@var{var++}", Value = item.Value });
                        }
                    }
                }


                //for photo
                if (!string.IsNullOrEmpty(TeacherPhotoPath))
                {
                    columns += $", [Photo]";
                    values += $", (SELECT BulkColumn FROM OPENROWSET(BULK '{TeacherPhotoPath}', SINGLE_BLOB) as IMG_DATA)";
                }

                if (columns.IsNullOrEmpty() || values.IsNullOrEmpty())
                {
                    DialogManager.ShowMessageDialog("Warning!", "Teacher information cannot be empty.",DialogTitleColor.Red);
                    return;
                }
                columns = columns.Remove(0, 1);
                values = values.Remove(0, 1);

                sqlCmd.CommandText = $"INSERT INTO Teachers ({columns}) VALUES ({values})";
                //commit insertion in database
                DataAccess.ExecuteQuery(sqlCmd);

                //to insert attendence record
                if(DataAccess.GetDataTable($"SELECT * FROM Teacher_Attendence WHERE Month = {DateTime.Now.Month} AND  Year = {DateTime.Now.Year} ").Rows.Count != 0)
                {
                    DataAccess.ExecuteQuery($"INSERT INTO Teacher_Attendence ([{DateTime.Now.Day}] , Month, Year , Teacher_ID ) " +
                            $"VALUES ( 'A' , {DateTime.Now.Month} , {DateTime.Now.Year} , {Convert.ToInt32(DataAccess.GetNextID("Teachers")) -1} ) ");
                }
                DialogManager.ShowMessageDialog("Message", "Teacher Admitted Successfull!",DialogTitleColor.Green);
                ResetTeacherPanel();
            }
            catch(Exception ex)
            {
                Logger.Log(ex, true);
            }
        }

        #endregion

        #region Private Methods

        private void SetTeacherEntitiesPanel()
        {
            try
            {
                TeacherEntites.Clear();
                // To set Teacher Panel
                List<string> TeacherFields = DataAccess.GetDataTable("SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'Teachers'").AsEnumerable().Select(s => s[0].ToString()).ToList();
                foreach (string item in TeacherFields)
                {
                    if (item == "Teacher_ID")
                        TeacherEntites.Add(new TextEntity()
                        {
                            FeildName = DataAccess.GetTeacherID(),
                            Value = DataAccess.GetNextID("Teachers"),
                            IsEnabled = false
                        });
                    else if (item == "Name")
                        TeacherEntites.Add(new TextEntity()
                        {
                            FeildName = item,
                            ValidationType = ValidationType.NotEmpty
                        });
                    else if (item.ToLower().Contains("date"))
                        TeacherEntites.Add(new DateEntity()
                        {
                            FeildName = item,
                        });
                    else if (item == "Gender")
                        TeacherEntites.Add(new ListEntity()
                        {
                            FeildName = item,
                            Items = new List<string>() { "Male", "Female", "Other" },
                            ValidationType = ValidationType.NotEmpty
                        });
                    else if (item.ToLower().Contains("email") || item.ToLower().Contains("e-mail"))
                        TeacherEntites.Add(new TextEntity()
                        {
                            FeildName = item,
                            ValidationType = ValidationType.Email
                        });
                    else if (item == "Salary")
                        TeacherEntites.Add(new TextEntity()
                        {
                            FeildName = item,
                            ValidationType = ValidationType.Numeric
                        });
                    else if (item == "Class_ID")
                    {
                        var list = new ListEntity()
                        {
                            FeildName = DataAccess.GetTeacherClassIDName(),
                            Items = DataAccess.GetClassNames()
                        };
                        list.Items.Insert(0, "None");
                        TeacherEntites.Add(list);
                    }
                    else if (item.ToLower().Contains("photo"))
                        IsPhotoPanelVisible = true;
                    else if (!item.Contains('_'))
                        TeacherEntites.Add(new TextEntity()
                        {
                            FeildName = item
                        });
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

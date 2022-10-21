using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace School_Manager
{
    public class EditTeacherViewModel : BaseViewModel
    {
        #region Default Constructor

        public EditTeacherViewModel(string TeacherID)
        {
            //initilialze the properties
            _TeacherID = TeacherID;
            TeacherEntities = new ObservableCollection<TextEntity>();
            _teacherIDName = DataAccess.GetTeacherID();

            //initialize the commands
            TeacherPhotoCommand = new RelayCommand(TeacherPhotoUpdate);
            UpdateRecordCommand = new RelayCommand(UpdateRecord);

            //set the editing panel
            SetEditingPanel();
        }

        #endregion

        #region Private Members

        private string _teacherIDName;

        private string _TeacherID { get; set; }

        private string _TeacherPhotoPath;

        #endregion

        #region Public Properties

        public ObservableCollection<TextEntity> TeacherEntities { get; set; }

        public BitmapImage TeacherPhoto { get; set; }

        #endregion

        #region Commands

        public ICommand TeacherPhotoCommand { get; set; }

        public ICommand UpdateRecordCommand { get; set; }

        #endregion

        #region Command Methods

        private void TeacherPhotoUpdate()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files(*.BMP;*.JPG)|*.BMP;*.JPG|All files (*.*)|*.*";
            openFileDialog.ShowDialog();
            if (!string.IsNullOrEmpty(openFileDialog.FileName))
            {
                _TeacherPhotoPath = openFileDialog.FileName;
                TeacherPhoto = new BitmapImage(new Uri(_TeacherPhotoPath));
            }
        }

        #endregion

        #region Methods

        private void SetEditingPanel()
        {
            string teacherClassID = "";
            // To set StudentsPanel
            DataTable Teacher = DataAccess.GetDataTable($"SELECT * FROM [Teachers] WHERE [Teacher_ID] = {_TeacherID}");
            for (int i = 0; i < Teacher.Columns.Count; i++)
            {
                if (Teacher.Columns[i].ColumnName == "Teacher_ID")
                {
                    var item = new TextEntity
                    {
                        FeildName = _teacherIDName,
                        Value = Teacher.Rows[0][i].ToString(),
                        IsEnabled = false
                    };
                    TeacherEntities.Add(item);
                }
                else if (Teacher.Columns[i].ColumnName.ToLower().Contains("name"))
                {
                    var item = new TextEntity
                    {
                        FeildName = Teacher.Columns[i].ColumnName,
                        Value = Teacher.Rows[0][i].ToString(),
                        ValidationType = ValidationType.NotEmpty
                    };
                    TeacherEntities.Add(item);
                }
                else if (Teacher.Columns[i].ColumnName.ToLower().Contains("photo"))
                {
                    if (Teacher.Rows[0][i] != DBNull.Value && Teacher.Rows[0][i] != null)
                    {
                        TeacherPhoto = Helper.ByteArrayToImage((byte[])Teacher.Rows[0][i]);
                    }
                }
                else if (Teacher.Columns[i].ColumnName.ToLower().Contains("date"))
                {
                    var item = new DateEntity
                    {
                        FeildName = Teacher.Columns[i].ColumnName,
                        Value = Teacher.Rows[0][i].ToString()
                    };
                    TeacherEntities.Add(item);
                }
                else if (Teacher.Columns[i].ColumnName.ToLower().Contains("contact"))
                {
                    var item = new TextEntity
                    {
                        FeildName = Teacher.Columns[i].ColumnName,
                        Value = Teacher.Rows[0][i].ToString(),
                        ValidationType = ValidationType.PhoneNumber
                    };
                    TeacherEntities.Add(item);
                }
                else if (Teacher.Columns[i].ColumnName.ToLower().Contains("email") || Teacher.Columns[i].ColumnName.ToLower().Contains("e-mail"))
                {
                    var item = new TextEntity
                    {
                        FeildName = Teacher.Columns[i].ColumnName,
                        Value = Teacher.Rows[0][i].ToString(),
                        ValidationType = ValidationType.Email
                    };
                    TeacherEntities.Add(item);
                }
                else if (Teacher.Columns[i].ColumnName == "Class_ID")
                {
                    teacherClassID = Teacher.Rows[0][i].ToString();
                }
                else if (Teacher.Columns[i].ColumnName.ToLower().Contains("gender"))
                {
                    var item = new ListEntity
                    {
                        FeildName = Teacher.Columns[i].ColumnName,
                        Value = Teacher.Rows[0][i].ToString(),
                        Items = new List<string>() { "Male", "Female", "Other" }
                    };
                    TeacherEntities.Add(item);
                }
                else if(!Teacher.Columns[i].ColumnName.ToLower().Contains("_"))
                {
                    var item = new TextEntity
                    {
                        FeildName = Teacher.Columns[i].ColumnName,
                        Value = Teacher.Rows[0][i].ToString()
                    };
                    TeacherEntities.Add(item);
                }
            }
            //to insert class under supervision column
            List<string> classes = DataAccess.GetClassNames();
            classes.Insert(0, "None");
            TeacherEntities.Add( new ListEntity
            {
                FeildName = DataAccess.GetTeacherClassIDName(),
                Items = classes,
                Value = teacherClassID.IsNullOrEmpty() || teacherClassID == "0"? "" :  DataAccess.GetClassName(teacherClassID),
            });
        }

        private void UpdateRecord()
        {
            try
            {

                #region Validation Check

                bool IsValid = true;

                foreach (var item in TeacherEntities)
                {
                    if (!item.IsValid)
                    {
                        IsValid = false;
                        break;
                    }
                }

                #endregion

                if (!IsValid)
                {
                    DialogManager.ShowValidationMessage();
                    return;
                }

                SqlCommand sqlCmd = new SqlCommand();
                string UpdateQuery = "";
                int var = 1;

                //To generate update query for StudentsTable
                foreach (TextEntity item in TeacherEntities)
                {
                    if (item.IsEnabled)
                    {
                        if(item.FeildName == DataAccess.GetTeacherClassIDName())
                        {
                            if(item.Value.IsNotNullOrEmpty())
                            {
                                UpdateQuery += $",[Class_ID] = @var{var}";
                                sqlCmd.Parameters.Add(new SqlParameter() { ParameterName = $"@var{var++}", Value = item.Value.IsNullOrEmpty() || item.Value == "None" ? "" : DataAccess.GetClassID(item.Value)});
                            }
                        }
                        else
                        {
                            UpdateQuery += $",[{item.FeildName}] = @var{var}";
                            sqlCmd.Parameters.Add(new SqlParameter() { ParameterName = $"@var{var++}", Value = item.Value });
                        }
                    }
                }
                
                if (!string.IsNullOrEmpty(_TeacherPhotoPath))
                {
                    UpdateQuery += $", [Photo] = (SELECT BulkColumn FROM OPENROWSET(BULK '{_TeacherPhotoPath}', SINGLE_BLOB) as IMG_DATA)";
                }

                UpdateQuery = UpdateQuery.Remove(0, 1);
                sqlCmd.CommandText = $"UPDATE Teachers SET {UpdateQuery} WHERE Teacher_ID = {_TeacherID}";

                //update datebase
                DataAccess.ExecuteQuery(sqlCmd);

                DialogManager.ShowMessageDialog("Update Successfull!", "Record Updated Successfully",DialogTitleColor.Green);

            }
            catch (Exception ex)
            {
                Logger.Log(ex, true);
            }
        }

        #endregion

    }
}

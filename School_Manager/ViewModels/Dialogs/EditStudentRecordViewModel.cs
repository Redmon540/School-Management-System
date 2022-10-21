using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace School_Manager
{
    /// <summary>
    /// View  model for edit student record dailog
    /// </summary>
    public class EditStudentRecordViewModel : BaseViewModel
    {
        #region Default Constructor

        public EditStudentRecordViewModel(string StudentID, string ParentID, string ClassName)
        {
            //Set the private variables
            mStudentID = StudentID;
            mParentID = ParentID;
            _parentIdName = DataAccess.GetParentID();
            _studentIdName = DataAccess.GetStudentID();

            //Initialize the lists
            StudentsEntities = new List<TextEntity>();
            ParentsEntities = new List<TextEntity>();
            FeeEntities = new ObservableCollection<FeeEntity>();
            Classes.Items = DataAccess.GetClassNames();
            Classes.Value = ClassName;

            ClassChanged();

            //Initialize commands
            StudentPhotoCommand = new RelayCommand(StudentPhotoUpdate);
            FatherPhotoCommand = new RelayCommand(FatherPhotoUpdate);
            MotherPhotoCommand = new RelayCommand(MotherPhotoUpdate);
            ClassSelectionChanged = new RelayCommand(ClassChanged);
            UpdateRecordCommand = new RelayCommand(UpdateRecord);

            //Set the editingpanels
            SetEditingPanels();
        }

        #endregion

        #region Private Members

        private string _studentIdName;

        private string _parentIdName;

        private string mStudentID;

        private string mParentID;

        private string StudentPhotoPath;

        private string FatherPhotoPath;

        private string MotherPhotoPath;

        #endregion

        #region Public Properties


        /// <summary>
        /// Students Table Entiites to make appropriate Controls to edit the data
        /// </summary>
        public List<TextEntity> StudentsEntities { get; set; }

        /// <summary>
        /// Parents Table Entiites to make appropriate Controls to edit the data
        /// </summary>
        public List<TextEntity> ParentsEntities { get; set; }

        /// <summary>
        /// Fee Table Entiites to make appropriate Controls to edit the data
        /// </summary>
        public ObservableCollection<FeeEntity> FeeEntities { get; set; }

        /// <summary>
        /// Student Photo
        /// </summary>
        public BitmapImage StudentPhoto { get; set; }

        /// <summary>
        /// Father Photo
        /// </summary>
        public BitmapImage FatherPhoto { get; set; }

        /// <summary>
        /// Mother Photo
        /// </summary>
        public BitmapImage MotherPhoto { get; set; }

        public ListEntity Classes { get; set; } = new ListEntity { FeildName = "Class", ValidationType = ValidationType.NotEmpty };

        public ListEntity Sections { get; set; } = new ListEntity { FeildName = "Section", ValidationType = ValidationType.NotEmpty };

        #endregion

        #region Public Commands

        /// <summary>
        /// Command that opens a File Dialog to select Student photo from drive
        /// </summary>
        public ICommand StudentPhotoCommand { get; set; }

        /// <summary>
        /// Command that opens a File Dialog to select Father photo from drive
        /// </summary>
        public ICommand FatherPhotoCommand { get; set; }

        /// <summary>
        /// Command that opens a File Dialog to select Mother photo from drive
        /// </summary>
        public ICommand MotherPhotoCommand { get; set; }

        public ICommand ClassSelectionChanged { get; set; }


        /// <summary>
        /// Updates Data in the database
        /// </summary>
        public ICommand UpdateRecordCommand { get; set; }
        #endregion

        #region Command Methods

        private void ClassChanged()
        {
            string classID = DataAccess.GetClassID(Classes.Value);
            List<string> list = DataAccess.GetSectionsNames(classID);
            if (list.Count > 0)
            {
                Sections.Items = list;
                Sections.IsEnabled = true;
            }
            else
            {
                Sections.Items = null;
                Sections.IsEnabled = false;
            }
            SetFeePanels();
        }

        /// <summary>
        /// Opens a file dialog to select student photo from drive
        /// </summary>
        private void StudentPhotoUpdate()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files(*.BMP;*.JPG)|*.BMP;*.JPG|All files (*.*)|*.*";
            openFileDialog.ShowDialog();
            if (!string.IsNullOrEmpty(openFileDialog.FileName))
            {
                StudentPhotoPath = openFileDialog.FileName;
                StudentPhoto = new BitmapImage(new Uri(StudentPhotoPath));
            }
        }

        /// <summary>
        /// Opens a file dialog to select father photo from drive
        /// </summary>
        private void FatherPhotoUpdate()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files(*.BMP;*.JPG)|*.BMP;*.JPG|All files (*.*)|*.*";
            openFileDialog.ShowDialog();
            if (!string.IsNullOrEmpty(openFileDialog.FileName))
            {
                FatherPhotoPath = openFileDialog.FileName;
                FatherPhoto = new BitmapImage(new Uri(FatherPhotoPath));
            }
        }

        /// <summary>
        /// Opens a file dialog to select mother photo from drive
        /// </summary>
        private void MotherPhotoUpdate()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files(*.BMP;*.JPG)|*.BMP;*.JPG|All files (*.*)|*.*";
            openFileDialog.ShowDialog();
            if (!string.IsNullOrEmpty(openFileDialog.FileName))
            {
                MotherPhotoPath = openFileDialog.FileName;
                MotherPhoto = new BitmapImage(new Uri(MotherPhotoPath));
            }
        }

        /// <summary>
        /// To commit the changes in database
        /// </summary>
        private void UpdateRecord()
        {
            try
            {

                #region Validation Check

                bool IsValid = true;
                foreach (var item in StudentsEntities)
                {
                    if (!item.IsValid)
                    {
                        IsValid = false;
                        break;
                    }
                }
                foreach (var item in ParentsEntities)
                {
                    if (!item.IsValid)
                    {
                        IsValid = false;
                        break;
                    }
                }
                foreach (var item in FeeEntities)
                {
                    if (!item.IsDiscountValid)
                    {
                        IsValid = false;
                        break;
                    }
                }
                if (!Classes.IsValid)
                {
                    IsValid = false;
                }

                if (!Sections.IsValid)
                {
                    IsValid = false;
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
                #region To Update Students Table

                //To generate update query for StudentsTable
                foreach (TextEntity item in StudentsEntities)
                {
                    if (item.IsEnabled)
                    {
                        UpdateQuery += $",[{item.FeildName}] = @var{var}";
                        sqlCmd.Parameters.Add(new SqlParameter() { ParameterName = $"@var{var++}", Value = item.Value });
                    }
                }
                //for photo
                if (StudentPhotoPath.IsNotNullOrEmpty())
                {
                    UpdateQuery += $",[Photo] = (SELECT BulkColumn FROM OPENROWSET(BULK '{StudentPhotoPath}', SINGLE_BLOB) as IMG_DATA)";
                }

                //to update class
                UpdateQuery += $",[Class_ID] = @var{var}";
                sqlCmd.Parameters.Add(new SqlParameter() { ParameterName = $"@var{var++}", Value = DataAccess.GetClassID(Classes.Value) });

                if (Sections.IsEnabled)
                {
                    UpdateQuery += $",[Section_ID] = @var{var}";
                    sqlCmd.Parameters.Add(new SqlParameter() { ParameterName = $"@var{var++}", Value = DataAccess.GetSectionID(Sections.Value, DataAccess.GetClassID(Classes.Value)) });
                }else
                {
                    UpdateQuery += $",[Section_ID] = NULL";
                    //sqlCmd.Parameters.Add(new SqlParameter() { ParameterName = $"@var{var++}", Value = "NULL" });
                }

                UpdateQuery = UpdateQuery.Remove(0, 1);
                sqlCmd.CommandText = $"UPDATE Students SET {UpdateQuery} WHERE [Student_ID] = {mStudentID}";

                //update datebase
                DataAccess.ExecuteQuery(sqlCmd);

                #endregion

                sqlCmd = new SqlCommand();
                UpdateQuery = "";
                var = 1;
                #region To Update Parents Table

                //To generate update query for StudentsTable
                foreach (TextEntity item in ParentsEntities)
                {
                    if (item.IsEnabled)
                    {
                        UpdateQuery += $",[{item.FeildName}] = @var{var}";
                        sqlCmd.Parameters.Add(new SqlParameter() { ParameterName = $"@var{var++}", Value = item.Value });
                    }
                }
                //for photo
                if (!string.IsNullOrEmpty(FatherPhotoPath))
                {
                    UpdateQuery += $", [Father Photo] = (SELECT BulkColumn FROM OPENROWSET(BULK '{FatherPhotoPath}', SINGLE_BLOB) as IMG_DATA)";
                }

                if (!string.IsNullOrEmpty(MotherPhotoPath))
                {
                    UpdateQuery += $", [Mother Photo] = (SELECT BulkColumn FROM OPENROWSET(BULK '{MotherPhotoPath}', SINGLE_BLOB) as IMG_DATA)";
                }

                UpdateQuery = UpdateQuery.Remove(0, 1);
                sqlCmd.CommandText = $"UPDATE Parents SET {UpdateQuery} WHERE Parent_ID = {mParentID}";

                //update datebase
                DataAccess.ExecuteQuery(sqlCmd);

                #endregion


                sqlCmd = new SqlCommand();
                UpdateQuery = "";
                var = 1;

                #region Update Discounts

                foreach (var item in FeeEntities)
                {
                    sqlCmd = new SqlCommand();
                    if (item.DiscountID.IsNotNullOrEmpty())
                    {
                        if (item.Discount.IsNullOrEmpty() || item.Discount == "0")
                        {
                            //cannot delete because Discount-ID is a foreign key in students table 
                            DataAccess.ExecuteQuery($"DELETE FROM Discounts WHERE Discount_ID = {item.DiscountID}");
                        }
                        else
                        {
                            sqlCmd.CommandText = $"UPDATE Discounts SET [Discount] = @var1 WHERE Discount_ID = {item.DiscountID}";
                            sqlCmd.Parameters.Add(new SqlParameter() { ParameterName = "@var1", Value = item.Discount });
                            DataAccess.ExecuteQuery(sqlCmd);
                        }
                    }
                    else
                    {
                        if (item.Discount.IsNotNullOrEmpty() && item.Discount != "0")
                        {
                            sqlCmd.CommandText = $"INSERT INTO Discounts ( Class_ID , Student_ID , Fee_ID , Discount ) VALUES ( {DataAccess.GetClassID(Classes.Value)} , {mStudentID} , {item.FeeID} , @var1 )";
                            sqlCmd.Parameters.Add(new SqlParameter() { ParameterName = "@var1", Value = item.Discount });
                            DataAccess.ExecuteQuery(sqlCmd);
                        }
                    }

                }

                #endregion


                DialogManager.ShowMessageDialog("Update Successfull!", "Record Updated Successfully",DialogTitleColor.Green);

            }
            catch(Exception ex)
            {
                Logger.Log(ex, true);
            }
        }

        #endregion

        #region Methods

        private void SetEditingPanels()
        {
            try
            {
                // To set StudentsPanel
                DataTable StudentsTable = DataAccess.GetDataTable($"SELECT * FROM Students WHERE [Student_ID] = {mStudentID}");
                for (int i = 0; i < StudentsTable.Columns.Count; i++)
                {
                    if (StudentsTable.Columns[i].ColumnName.ToLower().Contains("parent_id")) { }
                    else if (StudentsTable.Columns[i].ColumnName.ToLower().Contains("father name"))
                    {
                        var item = new TextEntity
                        {
                            FeildName = StudentsTable.Columns[i].ColumnName,
                            Value = StudentsTable.Rows[0][i].ToString(),
                            ValidationType = ValidationType.NotEmpty
                        };
                        StudentsEntities.Add(item);
                    }
                    else if (StudentsTable.Columns[i].ColumnName == "Student_ID")
                    {
                        var item = new TextEntity
                        {
                            FeildName = _studentIdName,
                            Value = StudentsTable.Rows[0][i].ToString(),
                            IsEnabled = false
                        };
                        StudentsEntities.Add(item);
                    }
                    else if (StudentsTable.Columns[i].ColumnName.ToLower().Contains("photo"))
                    {
                        if (StudentsTable.Rows[0][i] != DBNull.Value && StudentsTable.Rows[0][i] != null)
                        {
                            StudentPhoto = Helper.ByteArrayToImage((byte[])StudentsTable.Rows[0][i]);
                        }
                    }
                    else if (StudentsTable.Columns[i].ColumnName.ToLower().Contains("date"))
                    {
                        var item = new DateEntity
                        {
                            FeildName = StudentsTable.Columns[i].ColumnName,
                            Value = StudentsTable.Rows[0][i].ToString()
                        };
                        StudentsEntities.Add(item);
                    }
                    else if (StudentsTable.Columns[i].ColumnName.ToLower().Contains("gender"))
                    {
                        var item = new ListEntity
                        {
                            FeildName = StudentsTable.Columns[i].ColumnName,
                            Value = StudentsTable.Rows[0][i].ToString(),
                            Items = new List<string>() { "Male", "Female", "Other" }
                        };
                        StudentsEntities.Add(item);
                    }
                    else if (StudentsTable.Columns[i].ColumnName.ToLower().Contains("section"))
                    {
                        if(StudentsTable.Rows[0][i].ToString() != "0")
                            Sections.Value = DataAccess.GetSection(StudentsTable.Rows[0][i].ToString());
                    }
                    else if (StudentsTable.Columns[i].ColumnName.ToLower().Contains("contact"))
                    {
                        var item = new TextEntity
                        {
                            FeildName = StudentsTable.Columns[i].ColumnName,
                            Value = StudentsTable.Rows[0][i].ToString(),
                            //TODO: change the phone validation
                            //ValidationType = ValidationType.PhoneNumber
                        };
                        StudentsEntities.Add(item);
                    }
                    else if (StudentsTable.Columns[i].ColumnName.ToLower().Contains("email") || StudentsTable.Columns[i].ColumnName.ToLower().Contains("e-mail"))
                    {
                        var item = new TextEntity
                        {
                            FeildName = StudentsTable.Columns[i].ColumnName,
                            Value = StudentsTable.Rows[0][i].ToString(),
                            ValidationType = ValidationType.Email
                        };
                        StudentsEntities.Add(item);
                    }
                    else if (!StudentsTable.Columns[i].ColumnName.Contains("_"))
                    {
                        var item = new TextEntity
                        {
                            FeildName = StudentsTable.Columns[i].ColumnName,
                            Value = StudentsTable.Rows[0][i].ToString()
                        };
                        StudentsEntities.Add(item);
                    }
                }


                if (mParentID.IsNotNullOrEmpty())
                {
                    // To set Parents Panel
                    DataTable ParentsTable = DataAccess.GetDataTable($"SELECT * FROM Parents WHERE Parent_ID = {mParentID}");
                    for (int i = 0; i < ParentsTable.Columns.Count; i++)
                    {
                        if (ParentsTable.Columns[i].ColumnName == "Parent_ID")
                        {
                            var item = new TextEntity
                            {
                                FeildName = _parentIdName,
                                Value = ParentsTable.Rows[0][i].ToString(),
                                IsEnabled = false
                            };
                            ParentsEntities.Add(item);
                        }
                        else if (ParentsTable.Columns[i].ColumnName.ToLower().Contains("name"))
                        {
                            var item = new TextEntity
                            {
                                FeildName = ParentsTable.Columns[i].ColumnName,
                                Value = ParentsTable.Rows[0][i].ToString(),
                                ValidationType = ValidationType.NotEmpty
                            };
                            ParentsEntities.Add(item);
                        }
                        else if (ParentsTable.Columns[i].ColumnName.ToLower().Contains("father photo"))
                        {
                            if (ParentsTable.Rows[0][i] != DBNull.Value && ParentsTable.Rows[0][i] != null)
                            {
                                FatherPhoto = Helper.ByteArrayToImage((byte[])ParentsTable.Rows[0][i]);
                            }
                        }
                        else if (ParentsTable.Columns[i].ColumnName.ToLower().Contains("mother photo"))
                        {
                            if (ParentsTable.Rows[0][i] != DBNull.Value && ParentsTable.Rows[0][i] != null)
                            {
                                MotherPhoto = Helper.ByteArrayToImage((byte[])ParentsTable.Rows[0][i]);
                            }
                        }
                        else if (ParentsTable.Columns[i].ColumnName.ToLower().Contains("date"))
                        {
                            var item = new DateEntity
                            {
                                FeildName = ParentsTable.Columns[i].ColumnName,
                                Value = ParentsTable.Rows[0][i].ToString()
                            };
                            ParentsEntities.Add(item);
                        }
                        else if (ParentsTable.Columns[i].ColumnName.ToLower().Contains("gender"))
                        {
                            var item = new ListEntity
                            {
                                FeildName = ParentsTable.Columns[i].ColumnName,
                                Value = ParentsTable.Rows[0][i].ToString(),
                                Items = new List<string>() { "Male", "Female", "Other" }
                            };
                            ParentsEntities.Add(item);
                        }
                        else if (ParentsTable.Columns[i].ColumnName.ToLower().Contains("contact"))
                        {
                            var item = new TextEntity
                            {
                                FeildName = ParentsTable.Columns[i].ColumnName,
                                Value = ParentsTable.Rows[0][i].ToString(),
                                ValidationType = ValidationType.PhoneNumber
                            };
                            ParentsEntities.Add(item);
                        }
                        else if (ParentsTable.Columns[i].ColumnName.ToLower().Contains("email") || ParentsTable.Columns[i].ColumnName.ToLower().Contains("e-mail"))
                        {
                            var item = new TextEntity
                            {
                                FeildName = ParentsTable.Columns[i].ColumnName,
                                Value = ParentsTable.Rows[0][i].ToString(),
                                ValidationType = ValidationType.Email
                            };
                            ParentsEntities.Add(item);
                        }
                        else if (!ParentsTable.Columns[i].ColumnName.Contains("_"))
                        {
                            var item = new TextEntity
                            {
                                FeildName = ParentsTable.Columns[i].ColumnName,
                                Value = ParentsTable.Rows[0][i].ToString(),
                            };
                            ParentsEntities.Add(item);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex, true);
            }
        }

        private void SetFeePanels()
        {
            try
            {
                FeeEntities.Clear();

                // To set FeePanel
                string classID = DataAccess.GetClassID(Classes.Value);
                var feeStructure = DataAccess.GetDataTable($"SELECT * FROM Fee_Structure WHERE Class_ID = {classID}");
                var discount = DataAccess.GetDataTable($"SELECT * FROM Discounts WHERE Student_ID = {mStudentID} AND Class_ID = {classID}");
                foreach (DataRow row in feeStructure.Rows)
                {
                    var item = new FeeEntity()
                    {
                        FeeID = row["Fee_ID"].ToString(),
                        Fee = row["Fee"].ToString(),
                        Amount = row["Amount"].ToString(),
                    };
                    if (discount.Rows.Count != 0)
                    {
                        var rowsCollectoin = discount.AsEnumerable().Where(x => x["Fee_ID"].ToString() == row["Fee_ID"].ToString()).Select(i => i);
                        if (rowsCollectoin.Count() != 0)
                        {
                            var Discount = rowsCollectoin.CopyToDataTable();
                            if (Discount.Rows.Count != 0)
                            {
                                item.DiscountID = Discount.Rows[0]["Discount_ID"].ToString();
                                item.Discount = Discount.Rows[0]["Discount"].ToString();
                            }
                            else
                            {
                                item.Discount = "0";
                            }
                        }
                        else
                        {
                            item.Discount = "0";
                        }
                    }
                    else
                    {
                        item.Discount = "0";
                    }
                    FeeEntities.Add(item);
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

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace School_Manager
{
    class EditParentsViewModel : BaseViewModel
    {
        #region Default Constructor

        public EditParentsViewModel(string ParentID)
        {
            //Set the private variables
            mParentID = ParentID;
            _parentIDName = DataAccess.GetParentID();

            //Initialize the lists
            ParentsEntities = new List<TextEntity>();


            //Initialize commands
            FatherPhotoCommand = new RelayCommand(FatherPhotoUpdate);
            MotherPhotoCommand = new RelayCommand(MotherPhotoUpdate);
            UpdateRecordCommand = new RelayCommand(UpdateRecord);

            //Set the editingpanels
            SetEditingPanels();
        }

        #endregion

        #region Private Members

        private string mParentID;

        private string _parentIDName;

        private string FatherPhotoPath;

        private string MotherPhotoPath;

        #endregion

        #region Public Properties

        /// <summary>
        /// Parents Table Entiites to make appropriate Controls to edit the data
        /// </summary>
        public List<TextEntity> ParentsEntities { get; set; }
        
        /// <summary>
        /// Father Photo
        /// </summary>
        public BitmapImage FatherPhoto { get; set; }

        /// <summary>
        /// Mother Photo
        /// </summary>
        public BitmapImage MotherPhoto { get; set; }

        public bool IsSectionVisible { get; set; }

        #endregion

        #region Public Commands

        /// <summary>
        /// Command that opens a File Dialog to select Father photo from drive
        /// </summary>
        public ICommand FatherPhotoCommand { get; set; }

        /// <summary>
        /// Command that opens a File Dialog to select Mother photo from drive
        /// </summary>
        public ICommand MotherPhotoCommand { get; set; }

        /// <summary>
        /// Updates Data in the database
        /// </summary>
        public ICommand UpdateRecordCommand { get; set; }

        #endregion

        #region Command Methods

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
                
                foreach (var item in ParentsEntities)
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

                DialogManager.ShowMessageDialog("Update Successfull!", "Record Updated Successfully",DialogTitleColor.Green);

            }
            catch (Exception ex)
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
                                FeildName = _parentIDName,
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

        #endregion
    }
}

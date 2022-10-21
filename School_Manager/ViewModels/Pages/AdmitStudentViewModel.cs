using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace School_Manager
{
    public class AdmitStudentViewModel : BaseViewModel
    {
        #region Default Constructor

        public AdmitStudentViewModel()
        {
            //Initialize the lists
            StudentsEntites = new ObservableCollection<TextEntity>();
            ParentsEntites = new ObservableCollection<TextEntity>();
            FeeEntities = new ObservableCollection<FeeEntity>();

            //Initialize commands
            SetAllPanelsCommand = new RelayCommand(SetAllPanels);
            StudentPhotoCommand = new RelayCommand(StudentPhotoUpdate);
            FatherPhotoCommand = new RelayCommand(FatherPhotoUpdate);
            MotherPhotoCommand = new RelayCommand(MotherPhotoUpdate);
            ResetStudentsPanelCommand = new RelayCommand(ResetStudentsPanel);
            ResetParentsPanelCommand = new RelayCommand(ResetParentsPanel);
            SearchParentCommand = new RelayParameterizedCommand((parameter) => SearchParentMethod(parameter));
            AdmitStudentCommand = new RelayCommand(AdmitStudentMethod);
            ClassSelectionChanged = new RelayCommand(ClassChanged);

            //set the properties
            Classes.Items = DataAccess.GetClassNames();
            StudentIDName = DataAccess.GetStudentID();
            ParentIDName = DataAccess.GetParentID();

            SetAllPanels();
        }


        #endregion

        #region Private Members

        /// <summary>
        /// Student photo path on the hard drive
        /// </summary>
        private string StudentPhotoPath;

        /// <summary>
        /// father photo path on the hard drive
        /// </summary>
        private string FatherPhotoPath;

        /// <summary>
        /// mother photo path on the hard drive
        /// </summary>
        private string MotherPhotoPath;

        private bool IsParentAlreadyExist { get; set; }

        private string StudentIDName;

        private string ParentIDName;

        #endregion

        #region Public Properties

        /// <summary>
        /// Students Table Entiites to make appropriate Controls to edit the data
        /// </summary>
        public ObservableCollection<TextEntity> StudentsEntites { get; set; }

        /// <summary>
        /// Parents Table Entiites to make appropriate Controls to edit the data
        /// </summary>
        public ObservableCollection<TextEntity> ParentsEntites { get; set; }

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

        /// <summary>
        /// The class to admit student in
        /// </summary>
        public ListEntity Classes { get; set; } = new ListEntity { FeildName = "Class", ValidationType = ValidationType.NotEmpty };

        /// <summary>
        /// The class to admit student in
        /// </summary>
        public ListEntity Sections { get; set; } = new ListEntity { FeildName = "Section", ValidationType = ValidationType.NotEmpty };

        /// <summary>
        ///controls "Parent ID not found" alert
        /// </summary>
        public bool IsAlertVisible { get; set; } = false;

        /// <summary>
        /// control the visibility of all panels
        /// </summary>
        public Visibility PanelsVisibility { get; set; } = Visibility.Collapsed;

        public bool StudentPhotoVisibility { get; set; }

        public bool FatherPhotoVisibility { get; set; }

        public bool MotherPhotoVisibility { get; set; }

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

        /// <summary>
        /// Updates Data in the database
        /// </summary>
        public ICommand AdmitStudentCommand { get; set; }

        /// <summary>
        /// To search parents information by parent id
        /// </summary>
        public ICommand SearchParentCommand { get; set; }

        /// <summary>
        /// resets the students panel
        /// </summary>
        public ICommand ResetStudentsPanelCommand { get; set; }

        /// <summary>
        /// resets the parents panel
        /// </summary>
        public ICommand ResetParentsPanelCommand { get; set; }

        /// <summary>
        /// to set all the panels to admit student in a class
        /// </summary>
        public ICommand SetAllPanelsCommand { get; set; }

        public ICommand ClassSelectionChanged { get; set; }

        #endregion

        #region Command Methods

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
        /// to search parents information on data base
        /// </summary>
        private void SearchParentMethod(object sender)
        {
            ParentIDEntity parentID = sender as ParentIDEntity;
            if (parentID.IsEnabled)
            {
                SqlCommand sqlCmd = new SqlCommand();
                sqlCmd.CommandText = "SELECT * FROM Parents WHERE Parent_ID = @ParentID";
                sqlCmd.Parameters.Add(new SqlParameter() { ParameterName = "ParentID", Value = parentID.Value });
                DataTable table = DataAccess.GetDataTable(sqlCmd);
                //to get parents information
                if (table.Rows.Count != 0)
                {
                    //to iterate through all columns
                    for (int i = 0; i < table.Columns.Count; i++)
                    {
                        //to set father photo is exist
                        if (table.Columns[i].ColumnName.ToLower().Contains("father photo"))
                        {
                            if (table.Rows[0][i] != DBNull.Value)
                            {
                                FatherPhoto = Helper.ByteArrayToImage(table.Rows[0][i] as byte[]);
                            }
                        }
                        //to set mother photo if exist
                        else if (table.Columns[i].ColumnName.ToLower().Contains("mother photo"))
                        {
                            if (table.Rows[0][i] != DBNull.Value)
                            {
                                MotherPhoto = Helper.ByteArrayToImage(table.Rows[0][i] as byte[]);
                            }
                        }
                        else
                        {
                            //to iterate through all entities in ParentsEntities to set the values of the matching columns
                            foreach (TextEntity item in ParentsEntites)
                            {
                                if (table.Columns[i].ColumnName == item.FeildName)
                                {
                                    item.Value = table.Rows[0][i].ToString();
                                    item.IsEnabled = false;
                                }
                            }
                        }
                    }
                    parentID.IsEnabled = false;
                    parentID.IsValid = true;
                    IsParentAlreadyExist = true;
                    CollectionViewSource.GetDefaultView(ParentsEntites).Refresh();
                }
                else
                {
                    parentID.IsValid = false;
                    IsParentAlreadyExist = false;
                }
            }
            else
            {
                ResetParentsPanel();
                parentID.IsEnabled = true;
                CollectionViewSource.GetDefaultView(ParentsEntites).Refresh();
            }


        }

        /// <summary>
        /// resets the students panel
        /// </summary>
        private void ResetStudentsPanel()
        {
            foreach (TextEntity item in StudentsEntites)
            {
                if (item.FeildName != StudentIDName)
                {
                    item.Value = "";
                }
                else
                {
                    item.Value = DataAccess.GetNextID("Students");
                }
            }
            StudentPhoto = null;
            StudentPhotoPath = string.Empty;
            CollectionViewSource.GetDefaultView(StudentsEntites).Refresh();
        }

        /// <summary>
        /// resets the students panel
        /// </summary>
        private void ResetParentsPanel()
        {
            foreach (TextEntity item in ParentsEntites)
            {
                if (item.FeildName != ParentIDName)
                {
                    item.Value = "";
                    item.IsEnabled = true;
                }
                else
                {
                    item.Value = DataAccess.GetNextID("Parents");
                    item.IsEnabled = false;
                    (item as ParentIDEntity).IsValid = true;
                }
            }
            MotherPhotoPath = string.Empty;
            FatherPhotoPath = string.Empty;
            MotherPhoto = null;
            FatherPhoto = null;
            IsParentAlreadyExist = false;
            CollectionViewSource.GetDefaultView(ParentsEntites).Refresh();
        }

        /// <summary>
        /// Set all the panels for student admission
        /// </summary>
        private void SetAllPanels()
        {
            try
            {
                PanelsVisibility = Visibility.Visible;
                SetStudentsPanel();
                SetParentsPanel();
            }
            catch (Exception ex)
            {
                Logger.Log(ex, true);
            }
        }

        /// <summary>
        /// Sets the sections combo box if class has sections otherwise hides it
        /// </summary>
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
            SetFeePanel();
        }

        /// <summary>
        /// to insert record in database
        /// </summary>
        private void AdmitStudentMethod()
        {
            try
            {
                #region Validation Check

                bool isValid = true;
                foreach (var item in StudentsEntites)
                {
                    if (!item.IsValid)
                    {
                        isValid = false;
                        break;
                    }
                }
                foreach (var item in ParentsEntites)
                {
                    if (item.FeildName == ParentIDName)
                    {
                        if (!(item as ParentIDEntity).IsValid)
                        {
                            isValid = false;
                            break;
                        }
                    }
                    if (!item.IsValid)
                    {
                        isValid = false;
                        break;
                    }
                }
                if (!Classes.IsValid)
                {
                    isValid = false;
                }

                if (!Sections.IsValid)
                {
                    isValid = false;
                }
                foreach (var item in FeeEntities)
                {
                    if (!item.IsDiscountValid)
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

                #endregion

                string columns = "";
                string values = "";
                SqlCommand sqlCmd = new SqlCommand();
                string parentID = "";
                string StudentID = DataAccess.GetNextID("Students");
                int var = 1;


                #region Insert In Parents Table

                if (!IsParentAlreadyExist)
                {

                    parentID = DataAccess.GetNextID("Parents");

                    //for parents table
                    foreach (TextEntity item in ParentsEntites)
                    {
                        if (item.FeildName != ParentIDName && item.Value.IsNotNullOrEmpty())
                        {
                            columns += $",[{item.FeildName}]";
                            values += $",@var{var}";
                            sqlCmd.Parameters.Add(new SqlParameter() { ParameterName = $"@var{var++}", Value = item.Value });
                        }
                    }

                    //for photo
                    if (!string.IsNullOrEmpty(FatherPhotoPath))
                    {
                        columns += $",[Father Photo]";
                        values += $",(SELECT BulkColumn FROM OPENROWSET(BULK '{FatherPhotoPath}', SINGLE_BLOB) as IMG_DATA)";
                    }
                    //for photo
                    if (!string.IsNullOrEmpty(MotherPhotoPath))
                    {
                        columns += $",[Mother Photo]";
                        values += $",(SELECT BulkColumn FROM OPENROWSET(BULK '{MotherPhotoPath}', SINGLE_BLOB) as IMG_DATA)";
                    }

                    if (columns.IsNullOrEmpty() || values.IsNullOrEmpty())
                    {
                        DialogManager.ShowMessageDialog("Warning!", "Parents information cannot be empty.", DialogTitleColor.Red);
                        return;
                    }
                    columns = columns.Remove(0, 1);
                    values = values.Remove(0, 1);

                    sqlCmd.CommandText = $"INSERT INTO Parents ({columns}) VALUES ({values})";
                    //commit insertion in database
                    DataAccess.ExecuteQuery(sqlCmd);

                }
                else
                {
                    foreach (var item in ParentsEntites)
                    {
                        if (item.FeildName == ParentIDName)
                        {
                            parentID = item.Value;
                            break;
                        }
                    }
                }

                #endregion

                #region Insert In Students Table


                columns = "";
                values = "";

                var = 1;
                sqlCmd = new SqlCommand();

                //for students table
                foreach (TextEntity item in StudentsEntites)
                {
                    if (item.FeildName != StudentIDName && item.Value.IsNotNullOrEmpty())
                    {
                        columns += $",[{item.FeildName}]";
                        values += $",@var{var}";
                        sqlCmd.Parameters.Add(new SqlParameter() { ParameterName = $"@var{var++}", Value = item.Value });
                    }
                }

                //for photo
                if (!string.IsNullOrEmpty(StudentPhotoPath))
                {
                    columns += $",[Photo]";
                    values += $",(SELECT BulkColumn FROM OPENROWSET(BULK '{StudentPhotoPath}', SINGLE_BLOB) as IMG_DATA)";
                }

                //to insert FK parent_id
                columns += ",[Parent_ID]";
                values += $",@parentID";
                sqlCmd.Parameters.Add(new SqlParameter() { ParameterName = $"@parentID", Value = parentID });

                columns += ",[Class_ID]";
                values += $",@classID";
                sqlCmd.Parameters.Add(new SqlParameter() { ParameterName = $"@classID", Value = DataAccess.GetClassID(Classes.Value) });

                if (Sections.IsEnabled)
                {
                    columns += ",[Section_ID]";
                    values += $",@sectionID";
                    sqlCmd.Parameters.Add(new SqlParameter() { ParameterName = $"@sectionID", Value = DataAccess.GetSectionID(Sections.Value, DataAccess.GetClassID(Classes.Value)) });
                }

                columns += ",[Student_ID]";
                values += $",{DataAccess.GetNextID("Students")}";

                columns = columns.Remove(0, 1);
                values = values.Remove(0, 1);


                sqlCmd.CommandText = $"INSERT INTO Students ({columns}) VALUES ({values})";
                //commit insertion in database
                DataAccess.ExecuteQuery(sqlCmd);

                #endregion

                #region Insert In Discounts

                //For discount
                foreach (var item in FeeEntities)
                {
                    if (!item.Discount.IsNullOrEmpty())
                    {
                        DataAccess.ExecuteQuery($"INSERT INTO Discounts (Class_ID , Student_ID , Fee_ID , Discount ) VALUES ({DataAccess.GetClassID(Classes.Value)} , {StudentID} , {item.FeeID} , {item.Discount})");
                    }
                }

                #endregion

                #region Insert in record attendence

                //to insert attendence record
                if (DataAccess.GetDataTable($"SELECT * FROM Student_Attendence WHERE Month = {DateTime.Now.Month} AND  Year = {DateTime.Now.Year} ").Rows.Count != 0)
                {
                    DataAccess.ExecuteQuery($"INSERT INTO Student_Attendence ([{DateTime.Now.Day}] , Month , Year , Student_ID ) " +
                            $"VALUES ( 'A' , {DateTime.Now.Month} , {DateTime.Now.Year} , {Convert.ToInt32(DataAccess.GetNextID("Students")) - 1} ) ");
                }

                #endregion

                //show message
                DialogManager.ShowMessageDialog("Message", "Student Admitted Successfully !", DialogTitleColor.Green);

                ResetStudentsPanel();
                ResetParentsPanel();
                SetFeePanel();
                FeeEntities = new ObservableCollection<FeeEntity>();
            }
            catch (Exception ex)
            {
                Logger.Log(ex, true);
            }

        }
        #endregion

        #region Methods

        /// <summary>
        /// sets the students panel
        /// </summary>
        private void SetStudentsPanel()
        {
            StudentsEntites.Clear();

            // To set StudentsPanel
            List<string> StudentsFields = DataAccess.GetDataTable("SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Students'").AsEnumerable().Select(x => x[0].ToString()).ToList();
            for (int i = 0; i < StudentsFields.Count; i++)
            {

                if (StudentsFields[i].ToLower().Contains("photo"))
                {
                    StudentPhotoVisibility = true;
                }
                else if (StudentsFields[i] == "Student_ID")
                {
                    var item = new TextEntity
                    {
                        FeildName = StudentIDName,
                        Value = DataAccess.GetNextID("Students"),
                        ValidationType = ValidationType.Numeric,
                        IsEnabled = false

                    };
                    StudentsEntites.Add(item);
                }
                else if (StudentsFields[i] == "Name")
                {
                    var item = new TextEntity
                    {
                        FeildName = StudentsFields[i],
                        ValidationType = ValidationType.NotEmpty,

                    };
                    StudentsEntites.Add(item);
                }
                else if (StudentsFields[i].ToLower().Contains("date"))
                {
                    var item = new DateEntity
                    {
                        FeildName = StudentsFields[i],
                    };
                    StudentsEntites.Add(item);
                }
                else if (StudentsFields[i].ToLower().Contains("gender"))
                {
                    var item = new ListEntity
                    {
                        FeildName = StudentsFields[i],
                        Items = new List<string>() { "Male", "Female", "Other" }
                    };
                    StudentsEntites.Add(item);
                }
                else if (StudentsFields[i].ToLower().Contains("contact"))
                {
                    var item = new TextEntity
                    {
                        FeildName = StudentsFields[i],
                        ValidationType = ValidationType.PhoneNumber
                    };
                    StudentsEntites.Add(item);
                }
                else if (StudentsFields[i].ToLower().Contains("email") || StudentsFields[i].ToLower().Contains("e-mail"))
                {
                    var item = new TextEntity
                    {
                        FeildName = StudentsFields[i],
                        ValidationType = ValidationType.Email
                    };
                    StudentsEntites.Add(item);
                }
                else if (!StudentsFields[i].Contains("_"))
                {
                    var item = new TextEntity
                    {
                        FeildName = StudentsFields[i]
                    };
                    StudentsEntites.Add(item);
                }
            }

        }

        /// <summary>
        /// Sets parents panel
        /// </summary>
        private void SetParentsPanel()
        {
            ParentsEntites.Clear();
            // To set Parents Panel
            List<string> ParentsFeilds = DataAccess.GetDataTable("SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Parents'").AsEnumerable().Select(x => x[0].ToString()).Where(i => !i.Contains('_')).ToList();
            ParentsEntites.Add(new ParentIDEntity() { FeildName = ParentIDName, Value = DataAccess.GetNextID("Parents"), IsEnabled = false });
            for (int i = 0; i < ParentsFeilds.Count; i++)
            {

                if (ParentsFeilds[i].ToLower().Contains("father photo"))
                {
                    FatherPhotoVisibility = true;
                }
                else if (ParentsFeilds[i].ToLower().Contains("mother photo"))
                {
                    MotherPhotoVisibility = true;
                }
                else if (ParentsFeilds[i].ToLower().Contains("father name"))
                {
                    var item = new TextEntity
                    {
                        FeildName = ParentsFeilds[i],
                        ValidationType = ValidationType.NotEmpty
                    };
                    ParentsEntites.Add(item);
                }
                else if (ParentsFeilds[i].ToLower().Contains("date"))
                {
                    var item = new DateEntity
                    {
                        FeildName = ParentsFeilds[i],
                    };
                    ParentsEntites.Add(item);
                }
                else if (ParentsFeilds[i].ToLower().Contains("gender"))
                {
                    var item = new ListEntity
                    {
                        FeildName = ParentsFeilds[i],
                        Items = new List<string>() { "Male", "Female", "Other" }
                    };
                    ParentsEntites.Add(item);
                }
                else if (ParentsFeilds[i].ToLower().Contains("contact"))
                {
                    var item = new TextEntity
                    {
                        FeildName = ParentsFeilds[i],
                        ValidationType = ValidationType.PhoneNumber
                    };
                    ParentsEntites.Add(item);
                }
                else if (ParentsFeilds[i].ToLower().Contains("email") || ParentsFeilds[i].ToLower().Contains("e-mail"))
                {
                    var item = new TextEntity
                    {
                        FeildName = ParentsFeilds[i],
                        ValidationType = ValidationType.Email
                    };
                    ParentsEntites.Add(item);
                }
                else
                {
                    var item = new TextEntity
                    {
                        FeildName = ParentsFeilds[i],
                    };
                    ParentsEntites.Add(item);
                }
            }

        }

        private void SetFeePanel()
        {
            try
            {
                string classID = DataAccess.GetClassID(Classes.Value);
                FeeEntities = new ObservableCollection<FeeEntity>(DataAccess.GetDataTable($"SELECT * FROM [Fee_Structure] WHERE Class_ID = {DataAccess.GetClassID(Classes.Value)} ").AsEnumerable().Select(i =>
                new FeeEntity()
                {
                    Fee = i["Fee"].ToString(),
                    Amount = i["Amount"].ToString(),
                    FeeID = i["Fee_ID"].ToString(),
                    IsEditEnable = true
                }
                ).ToList());
            }
            catch (Exception ex)
            {
                Logger.Log(ex, true);
            }
        }

        #endregion
    }
}

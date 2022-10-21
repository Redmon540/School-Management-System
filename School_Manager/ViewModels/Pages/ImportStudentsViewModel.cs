using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;

namespace School_Manager
{
    public class ImportStudentsViewModel : BaseViewModel
    {
        #region Constructor

        public ImportStudentsViewModel()
        {
            SetStudents();
            OpenFileCommand = new RelayCommand(OpenFile);
            SheetChangedCommand = new RelayCommand(SheetChanged);
            ClassChangedCommand = new RelayCommand(ClassChanged);
            ImportCommand = new RelayCommand(Import);
        }

        #endregion

        #region Private Members

        private DataSet dataSet;

        private List<string> ColumnList = new List<string>();

        #endregion

        #region Properties

        public List<MapperEntity> StudentsColumns { get; set; }

        public List<MapperEntity> ParentsColumns { get; set; }

        public ListEntity Sheets { get; set; } = new ListEntity { FeildName = "Sheet", ValidationType = ValidationType.NotEmpty };

        public ListEntity Class { get; set; } = new ListEntity { FeildName = "Class", Items = DataAccess.GetClassNames(), ValidationType = ValidationType.NotEmpty };

        public ListEntity Section { get; set; }= new ListEntity { FeildName = "Section", ValidationType = ValidationType.NotEmpty };

        public bool IsMatchVisible { get; set; }

        public bool IsDialogOpen { get; set; }

        #endregion

        #region Commands

        public ICommand OpenFileCommand { get; set; }

        public ICommand SheetChangedCommand { get; set; }

        public ICommand ClassChangedCommand { get; set; }

        public ICommand ImportCommand { get; set; }

        #endregion

        #region Command Methods

        private void OpenFile()
        {
            try
            {
                var openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Excel Workbook (*.xls;*.xlsx)|*.xls;*.xlsx";
                openFileDialog.ShowDialog();
                if (!string.IsNullOrEmpty(openFileDialog.FileName))
                {
                    using (var stream = File.Open(openFileDialog.FileName, FileMode.Open, FileAccess.Read))
                    {
                        using (var reader = ExcelReaderFactory.CreateReader(stream))
                        {
                            dataSet = reader.AsDataSet(new ExcelDataSetConfiguration()
                            {
                                //to use first row as header
                                ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                                {
                                    UseHeaderRow = true
                                }
                            });
                        }
                    }
                    // populate the sheets combo box
                    var list = new List<string>();
                    foreach (DataTable table in dataSet.Tables)
                    {
                        list.Add(table.TableName);
                    }
                    Sheets.Items = list;
                    Sheets.Value = list[0];
                }
            }
            catch(Exception ex)
            {
                Logger.Log(ex, true);
            }
        }

        private void SetStudents()
        {
            try
            {
                StudentsColumns = DataAccess.GetDataTable("SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Students'")
                .AsEnumerable().Where(x => !x[0].ToString().Contains("_") && !x[0].ToString().ToLower().Contains("photo")).
                Select(x => new MapperEntity { FeildName = x[0].ToString(), Items = ColumnList }).ToList();
                StudentsColumns.Insert(0, new MapperEntity { FeildName = DataAccess.GetStudentID(), Items = ColumnList });
                ParentsColumns = DataAccess.GetDataTable("SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Parents'")
                    .AsEnumerable().Where(x => !x[0].ToString().Contains("_") && !x[0].ToString().ToLower().Contains("photo")).
                    Select(x => new MapperEntity { FeildName = x[0].ToString(), Items = ColumnList }).ToList();
            }
            catch(Exception ex)
            {
                Logger.Log(ex, true);
            }
            
        }

        private void SheetChanged()
        {
            try
            {
                if (Sheets.Value.IsNotNullOrEmpty())
                {
                    ColumnList.Clear();
                    foreach (DataColumn item in dataSet.Tables[Sheets.Value].Columns)
                    {
                        ColumnList.Add(item.ColumnName);
                    }
                    CollectionViewSource.GetDefaultView(ColumnList).Refresh();
                }
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
                if (Class.Value.IsNotNullOrEmpty())
                    Section.Items = DataAccess.GetSectionsNames(DataAccess.GetClassID(Class.Value));
                if (Section.Items.Count == 0)
                    Section.IsEnabled = false;
            }
            catch(Exception ex)
            {
                Logger.Log(ex, true);
            }
        }

        private async void Import()
        {
            try
            {
                if (Sheets.IsValid && Class.IsValid && (Section.IsValid || !Section.IsEnabled))
                {
                    string result = "";
                    IsDialogOpen = true;
                    await Task.Run(() => 
                    {
                        var classID = DataAccess.GetClassID(Class.Value);
                        result = DataAccess.ImportStudentData(dataSet, StudentsColumns, ParentsColumns,
                            Sheets.Value, classID,
                            Section.Value.IsNullOrEmpty() ? null : DataAccess.GetSectionID(Section.Value, classID)
                            , IsMatchVisible);
                    });
                    IsDialogOpen = false;
                    await Task.Delay(500);
                    if(result == "imported")
                        DialogManager.ShowMessageDialog("Message", "Data Imported Successfully.",DialogTitleColor.Green);
                    else
                    {
                        var studentID = DataAccess.GetStudentID();
                        DialogManager.ShowMessageDialog("Error", $"Faild to insert dulplicate {studentID} please make sure that {studentID} is unique {System.Environment.NewLine} {result}", DialogTitleColor.Red);
                    }
                }
                else
                {
                    DialogManager.ShowValidationMessage();
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

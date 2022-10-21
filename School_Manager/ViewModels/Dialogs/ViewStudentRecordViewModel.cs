using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Media.Imaging;

namespace School_Manager
{
    /// <summary>
    /// View  model for edit student record dailog
    /// </summary>
    public class ViewStudentRecordViewModel : BaseViewModel
    {
        #region Default Constructor

        public ViewStudentRecordViewModel(string StudentID, string ParentID, string ClassName)
        {
            //Set the private variables
            mStudentID = StudentID;
            mParentID = ParentID;
            mClassName = ClassName;
            _studentIDName = DataAccess.GetStudentID();
            _parentIDName = DataAccess.GetParentID();

            //Initialize the lists
            StudentsEntites = new List<TextEntity>();
            ParentsEntites = new List<TextEntity>();
            FeeEntities = new List<FeeEntity>();
            
            //Set the panels
            SetPanels();
        }

        #endregion

        #region Private Members

        private string mStudentID;

        private string mParentID;

        private string mClassName;

        private string _studentIDName;

        private string _parentIDName;

        #endregion

        #region Public Properties

        /// <summary>
        /// Students Table Entiites to make appropriate Controls to edit the data
        /// </summary>
        public List<TextEntity> StudentsEntites { get; set; }

        /// <summary>
        /// Parents Table Entiites to make appropriate Controls to edit the data
        /// </summary>
        public List<TextEntity> ParentsEntites { get; set; }

        /// <summary>
        /// Fee Table Entiites to make appropriate Controls to edit the data
        /// </summary>
        public List<FeeEntity> FeeEntities { get; set; }

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

        
        #endregion

        #region Methods

        private void SetPanels()
        {
            try
            {
                // To set StudentsPanel
                DataTable StudentsTable = DataAccess.GetDataTable($"SELECT * FROM Students WHERE [Student_ID] = {mStudentID}");
                for (int i = 0; i < StudentsTable.Columns.Count; i++)
                {
                    if (StudentsTable.Columns[i].ColumnName.ToLower().Contains("photo"))
                    {
                        if (StudentsTable.Rows[0][i] != DBNull.Value && StudentsTable.Rows[0][i] != null)
                            StudentPhoto = Helper.ByteArrayToImage((byte[])StudentsTable.Rows[0][i]);
                    }
                    else if (StudentsTable.Columns[i].ColumnName == "Student_ID")
                    {
                        StudentsEntites.Add(new DisabledTextEntity
                        {
                            FeildName = _studentIDName,
                            Value = StudentsTable.Rows[0][i].ToString()
                        });
                    }
                    else if (StudentsTable.Columns[i].ColumnName == "Class_ID")
                    {
                        StudentsEntites.Add(new DisabledTextEntity
                        {
                            FeildName = "Class",
                            Value = DataAccess.GetClassName(StudentsTable.Rows[0][i].ToString())
                        });
                    }
                    else if (StudentsTable.Columns[i].ColumnName == "Section_ID" && DataAccess.GetDataTable($"SELECT * FROM Sections WHERE Class_ID = {DataAccess.GetClassID(mClassName)}").Rows.Count != 0)
                    {
                        StudentsEntites.Add(new DisabledTextEntity
                        {
                            FeildName = "Section",
                            Value = DataAccess.GetSection(StudentsTable.Rows[0][i].ToString())
                        });
                    }
                    else if (!StudentsTable.Columns[i].ColumnName.Contains('_'))
                    {
                        //to get only date without time
                        if (StudentsTable.Columns[i].ColumnName.ToLower().Contains("date"))
                        {
                            var item = new DisabledTextEntity();
                            item.FeildName = StudentsTable.Columns[i].ColumnName;
                            string date = StudentsTable.Rows[0][i].ToString();
                            if (date.IsNotNullOrEmpty())
                                date = date.Remove(date.Length - 12, 12);
                            item.Value = date;
                            StudentsEntites.Add(item);
                        }
                        else
                        StudentsEntites.Add(new DisabledTextEntity
                        {
                            FeildName = StudentsTable.Columns[i].ColumnName,
                            Value = StudentsTable.Rows[0][i].ToString()
                        });
                    }
                }

                // To set Parents Panel
                if (mParentID.IsNotNullOrEmpty())
                {
                    DataTable ParentsTable = DataAccess.GetDataTable($"SELECT * FROM Parents WHERE Parent_ID = {mParentID}");
                    for (int i = 0; i < ParentsTable.Columns.Count; i++)
                    {
                        if (ParentsTable.Columns[i].ColumnName.ToLower().Contains("father photo"))
                        {
                            if (ParentsTable.Rows[0][i] != DBNull.Value && ParentsTable.Rows[0][i] != null)
                                FatherPhoto = Helper.ByteArrayToImage((byte[])ParentsTable.Rows[0][i]);
                        }
                        else if (ParentsTable.Columns[i].ColumnName.ToLower().Contains("mother photo"))
                        {
                            if (ParentsTable.Rows[0][i] != DBNull.Value && ParentsTable.Rows[0][i] != null)
                                MotherPhoto = Helper.ByteArrayToImage((byte[])ParentsTable.Rows[0][i]);
                        }
                        else if (ParentsTable.Columns[i].ColumnName == "Parent_ID")
                        {
                            var item = new DisabledTextEntity
                            {
                                FeildName = _parentIDName,
                                Value = ParentsTable.Rows[0][i].ToString(),
                            };
                            ParentsEntites.Add(item);
                        }
                        else if(!ParentsTable.Columns[i].ColumnName.Contains('_'))
                        {
                            //to get only date without time
                            if (ParentsTable.Columns[i].ColumnName.ToLower().Contains("date"))
                            {
                                var item = new DisabledTextEntity();
                                item.FeildName = ParentsTable.Columns[i].ColumnName;
                                string date = ParentsTable.Rows[0][i].ToString();
                                date = date.Remove(date.Length - 12, 12);
                                item.Value = date;
                                ParentsEntites.Add(item);
                            }
                            else
                                ParentsEntites.Add(new DisabledTextEntity
                                {
                                    FeildName = ParentsTable.Columns[i].ColumnName,
                                    Value = ParentsTable.Rows[0][i].ToString(),
                                });
                        }

                    }
                }
                string classID = DataAccess.GetClassID(mClassName);

                // To set FeePanel
                var feeStructure = DataAccess.GetDataTable($"SELECT * FROM Fee_Structure WHERE Class_ID = {classID}");
                var discount = DataAccess.GetDataTable($"SELECT * FROM Discounts WHERE Student_ID = {mStudentID} AND Class_ID = {classID}");
                foreach (DataRow row in feeStructure.Rows)
                {
                    FeeEntities.Add(new FeeEntity()
                    {
                        FeeID = row["Fee_ID"].ToString(),
                        Fee = row["Fee"].ToString(),
                        Amount = row["Amount"].ToString(),
                        Discount = discount.Rows.Count == 0 ? "0" : discount.AsEnumerable().Where(x => x["Fee_ID"].ToString() == row["Fee_ID"].ToString()).Select(i => i["Discount"].ToString()).Count() == 0 ? "0" :
                        discount.AsEnumerable().Where(x => x["Fee_ID"].ToString() == row["Fee_ID"].ToString()).Select(i => i["Discount"].ToString()).First()
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

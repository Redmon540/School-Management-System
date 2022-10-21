using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Media.Imaging;

namespace School_Manager
{
    public class ViewParentRecordViewModel : BaseViewModel
    {
        #region Default Constructor

        public ViewParentRecordViewModel(string ParentID)
        {
            //Set the private variables
            mParentID = ParentID;
            _parentIDName = DataAccess.GetParentID();

            //Initialize the lists
            ParentsEntites = new List<TextEntity>();

            //Set the panels
            SetPanels();
            SetChildrenPanel();
        }

        #endregion

        #region Private Members

        private string mParentID;

        private string _parentIDName;

        #endregion

        #region Public Properties

        public DataTable GridData { get; set; }

        public bool IsDataLoading { get; set; }

        /// <summary>
        /// Parents Table Entiites to make appropriate Controls to edit the data
        /// </summary>
        public List<TextEntity> ParentsEntites { get; set; }

        /// <summary>
        /// Father Photo
        /// </summary>
        public BitmapImage FatherPhoto { get; set; }

        public BitmapImage MotherPhoto { get; set; }

        #endregion

        #region Methods

        private void SetPanels()
        {
            try
            {
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
                        else
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
                            else if(!ParentsTable.Columns[i].ColumnName.ToLower().Contains("_"))
                                ParentsEntites.Add(new DisabledTextEntity
                                {
                                    FeildName = ParentsTable.Columns[i].ColumnName,
                                    Value = ParentsTable.Rows[0][i].ToString(),
                                });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex, true);
            }

        }

        private void SetChildrenPanel()
        {
            IsDataLoading = true;
            List<string> studentColumns = DataAccess.GetDataTable("SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Students'").AsEnumerable().Select(s => s[0].ToString()).Where(x => !x.Contains("_")).ToList();
            string query = $"Student_ID AS [{DataAccess.GetStudentID()}]";
            foreach (var item in studentColumns)
            {
                if(!item.ToLower().Contains("date"))
                {
                    query += $",[{item}]";
                }
                else if(item != "Student_ID")
                {
                    query += $",CONVERT(varchar(10) , [{item}] , 103) as [{item}]";
                }
            }
            if(query.IsNotNullOrEmpty())
            {
                GridData = DataAccess.GetDataTable($"SELECT {query} , Class , Section FROM Students LEFT JOIN Classes ON Classes.Class_ID = Students.Class_ID " +
                    $"LEFT JOIN Sections ON Sections.Section_ID = Students.[Section_ID] WHERE Parent_ID = {mParentID}");
            }
            IsDataLoading = false;
        }

        #endregion
    }
}

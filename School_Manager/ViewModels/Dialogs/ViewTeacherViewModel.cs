using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows.Media.Imaging;

namespace School_Manager
{
    public class ViewTeacherViewModel : BaseViewModel
    {
        #region Default Constructor

        public ViewTeacherViewModel(string TeacherID)
        {
            //initialize the properties
            _TeacherID = TeacherID;
            TeacherEntities = new ObservableCollection<TextEntity>();

            //set the view
            SetView();
        }
        #endregion

        #region Private Member

        private string _TeacherID { get; set; }

        #endregion

        #region Public Properties

        public ObservableCollection<TextEntity> TeacherEntities { get; set; }

        public BitmapImage TeacherPhoto { get; set; }

        #endregion

        #region Private Methods

        private void SetView()
        {
            string teacherClassId = "";
            DataTable Teacher = DataAccess.GetDataTable($"SELECT * FROM [Teachers] WHERE [Teacher_ID] = {_TeacherID}");
            for (int i = 0; i < Teacher.Columns.Count; i++)
            {
                if (Teacher.Columns[i].ColumnName.ToLower().Contains("photo"))
                {
                    if (Teacher.Rows[0][i] != DBNull.Value && Teacher.Rows[0][i] != null)
                    {
                        TeacherPhoto = Helper.ByteArrayToImage((byte[])Teacher.Rows[0][i]);
                    }
                }
                else if(Teacher.Columns[i].ColumnName == "Teacher_ID")
                {
                    TeacherEntities.Add(new DisabledTextEntity() { FeildName = DataAccess.GetTeacherID(), Value = Teacher.Rows[0][i].ToString() });
                }
                else if (Teacher.Columns[i].ColumnName == "Class_ID")
                {
                    teacherClassId = Teacher.Rows[0][i].ToString();
                }
                else if(!Teacher.Columns[i].ColumnName.Contains("_"))
                {
                    TeacherEntities.Add(new DisabledTextEntity() { FeildName = Teacher.Columns[i].ColumnName, Value = Teacher.Rows[0][i].ToString() });
                }
            }
            TeacherEntities.Add(new DisabledTextEntity
            {
                FeildName = DataAccess.GetTeacherClassIDName(),
                Value = teacherClassId.IsNullOrEmpty() || teacherClassId == "0" ? "None" : DataAccess.GetClassName(teacherClassId)
            });
        }

        #endregion
    }
}

using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;

namespace School_Manager
{
    public class PromoteStudentsViewModel : BaseViewModel
    {
        #region Constructor

        public PromoteStudentsViewModel()
        {
            //set the commands
            PromoteAllCommand = new RelayCommand(PromoteAll);
            FailAllCommand = new RelayCommand(FailAll);
            ChangePromotionCommand = new RelayParameterizedCommand(parameter =>  ChangePromotion(parameter));
            PromoteCommand = new RelayCommand(Promote);
            PreviousClassChangedCommand = new RelayCommand(PreviousClassChanged);
            PreviousSectionChangedCommand = new RelayCommand(PreviousSectionChanged);
            NextClassChangedCommand = new RelayCommand(NextClassChanged);
        }

        #endregion

        #region Properties

        public string IDName { get; set; } = DataAccess.GetStudentID();

        public ObservableCollection<StudentEntity> Students { get; set; }

        public ListEntity PreviousClasses { get; set; } = new ListEntity { FeildName = "Class", ValidationType = ValidationType.NotEmpty  ,Items = DataAccess.GetClassNames()};

        public ListEntity NextClasses { get; set; } = new ListEntity { FeildName = "Promote To Class", ValidationType = ValidationType.NotEmpty, Items = DataAccess.GetClassNames() };

        public ListEntity PreviousSections { get; set; } = new ListEntity { FeildName = "Sections", ValidationType = ValidationType.NotEmpty ,IsEnabled = false};

        public ListEntity NextSections { get; set; } = new ListEntity { FeildName = "Sections", ValidationType = ValidationType.NotEmpty ,IsEnabled = false};

        #endregion

        #region Commands

        public ICommand PromoteAllCommand { get; set; }

        public ICommand FailAllCommand { get; set; }

        public ICommand ChangePromotionCommand { get; set; }

        public ICommand PromoteCommand { get; set; }

        public ICommand PreviousClassChangedCommand { get; set; }

        public ICommand PreviousSectionChangedCommand { get; set; }

        public ICommand NextClassChangedCommand { get; set; }

        #endregion

        #region Command Methods

        private void PreviousClassChanged()
        {
            PreviousSections.Items = null;
            
            PreviousSections.Items = DataAccess.GetSectionsNames(DataAccess.GetClassID(PreviousClasses.Value));
            if (PreviousSections.Items.Count == 0)
            {
                PreviousSections.IsEnabled = false;
                Students = new ObservableCollection<StudentEntity>(DataAccess.GetDataTable($"SELECT [Student_ID] , [Name] FROM Students " +
                $"WHERE Class_ID = {DataAccess.GetClassID(PreviousClasses.Value)} AND Students.Is_Active = 1")
                .AsEnumerable().Select(x => new StudentEntity { StudentID = x["Student_ID"].ToString(), Name = x["Name"].ToString(), IsPromoting = true }).ToList());
            }
            else
            {
                PreviousSections.Items.Insert(0, "All");
                PreviousSections.Value = "All";
                PreviousSections.IsEnabled = true;
                CollectionViewSource.GetDefaultView(PreviousSections.Items).Refresh();
            }
        }

        private void PreviousSectionChanged()
        {
            if (PreviousSections.Value != null)
            {
                Students = new ObservableCollection<StudentEntity>(DataAccess.GetDataTable($"SELECT [Student_ID] , [Name] FROM Students " +
                    $"WHERE Class_ID = {DataAccess.GetClassID(PreviousClasses.Value)} AND Students.Is_Active = 1" +
                    $"{(PreviousSections.Value != "All" ? $" AND Section_ID = {DataAccess.GetSectionID(PreviousSections.Value, DataAccess.GetClassID(PreviousClasses.Value))} " : "")}")
                    .AsEnumerable().Select(x => new StudentEntity { StudentID = x["Student_ID"].ToString(), Name = x["Name"].ToString(), IsPromoting = true }).ToList());
            }
        }

        private void NextClassChanged()
        {
            NextSections.Items = DataAccess.GetSectionsNames(DataAccess.GetClassID(NextClasses.Value));
            NextSections.IsEnabled = NextSections.Items.Count == 0 ? false : true;
        }

        private void PromoteAll()
        {
            foreach (var student in Students)
            {
                student.IsPromoting = true;
            }
        }

        private void FailAll()
        {
            foreach (var student in Students)
            {
                student.IsPromoting = false;
            }
        }

        private void ChangePromotion(object sender)
        {
            (sender as StudentEntity).IsPromoting ^= true;
        }

        private void Promote()
        {
            try
            {
                bool isValid = true;
                if (!PreviousClasses.IsValid)
                    isValid = false;
                if (PreviousSections.IsEnabled == true && !PreviousSections.IsValid)
                    isValid = false;
                if (!NextClasses.IsValid)
                    isValid = false;
                if (NextSections.IsEnabled == true && !NextSections.IsValid)
                    isValid = false;

                if (!isValid)
                {
                    DialogManager.ShowValidationMessage();
                    return;
                }

                string query;

                if (NextSections.IsEnabled)
                    query = $"UPDATE Students SET Class_ID = {DataAccess.GetClassID(NextClasses.Value)} , Section_ID = {DataAccess.GetSectionID(NextSections.Value, DataAccess.GetClassID(NextClasses.Value))} WHERE Student_ID = ";
                else
                    query = $"UPDATE Students SET Class_ID = {DataAccess.GetClassID(NextClasses.Value)} , Section_ID = NULL WHERE Student_ID = ";


                foreach (var item in Students)
                {
                    if (item.IsPromoting)
                        DataAccess.ExecuteQuery(query + item.StudentID);
                }
                DialogManager.ShowMessageDialog("Message", "Students Promoted Successfully",DialogTitleColor.Green);
            }
            catch(Exception ex)
            {
                Logger.Log(ex, true);
            }
        }

        #endregion
    }

    #region Student Entity Class

    public class StudentEntity : BasePropertyChanged
    {
        public string StudentID { get; set; }
        public string Name { get; set; }
        public bool IsPromoting { get; set; }
    }

    #endregion
}

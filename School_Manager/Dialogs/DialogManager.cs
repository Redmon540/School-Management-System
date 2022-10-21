using System.Linq;
using System.Windows;

namespace School_Manager
{
    /// <summary>
    /// Manages the Custom Dialog Boxes
    /// </summary>
    public static class DialogManager
    {
        /// <summary>
        /// To get the current active/focused window of the application
        /// </summary>
        /// <returns></returns>
        public static BaseViewModel GetActiveWindow()
        {
            return Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive).DataContext as BaseViewModel;
        }

        #region Students Dialogs

        /// <summary>
        /// To show the ViewStudentRecord Dialog
        /// </summary>
        /// <param name="StudentID"></param>
        /// <param name="ParentID"></param>
        /// <param name="ClassName"></param>
        public static void ViewStudentRecord(string StudentID, string ParentID, string ClassName)
        {
            var mDialogPage = new DialogPage();
            var mViewStudentRecord = new ViewStudentRecord(StudentID, ParentID, ClassName);
            var mDialogWindowViewModel = new DialogWindowViewModel(mDialogPage)
            {
                Content = mViewStudentRecord,
            };
            mDialogPage.ViewModel = mDialogWindowViewModel;
            mDialogWindowViewModel.ShowDialog(mDialogWindowViewModel);
        }

        /// <summary>
        /// To show the EditStudentRecord dialog to edit the student record
        /// </summary>
        /// <param name="StudentID"></param>
        /// <param name="ParentID"></param>
        /// <param name="ClassName"></param>
        public static void EditStudentRecord(string StudentID, string ParentID, string ClassName)
        {
            var mDialogPage = new DialogPage();
            var mEditStudentRecord = new EditStudentRecord(StudentID, ParentID, ClassName);
            var mDialogWindowViewModel = new DialogWindowViewModel(mDialogPage)
            {
                Content = mEditStudentRecord,
            };
            mDialogPage.ViewModel = mDialogWindowViewModel;
            mDialogWindowViewModel.ShowDialog(mDialogWindowViewModel);
        }

        /// <summary>
        /// To show a confirmation message that returns a bool
        /// </summary>
        /// <param name="Title"></param>
        /// <param name="Message"></param>
        /// <param name="IsYesNoButtonVisible"></param>
        /// <returns></returns>
        public static (bool IsYesPressed, bool IsChecked) ShowDeleteStudentDialog(string Title, string Message, string CheckBoxContent, bool IsYesNoButtonVisible)
        {
            var mDialogWindow = new DialogWindow();
            var mDeleteStudentDialog = new DeleteStudentDialog(Message, "Yes", "No", CheckBoxContent, IsYesNoButtonVisible);
            var mDialogWindowViewModel = new DialogWindowViewModel(mDialogWindow)
            {
                Title = Title,
                Content = mDeleteStudentDialog
            };
            mDialogWindow.ViewModel = mDialogWindowViewModel;
            mDialogWindowViewModel.ShowDialog(mDialogWindowViewModel);
            return (mDeleteStudentDialog.IsYesPressed, mDeleteStudentDialog.IsChecked);
        }

        #endregion

        /// <summary>
        /// To show a Message box with a message and title
        /// </summary>
        /// <param name="Title"></param>
        /// <param name="Message"></param>
        public static void ShowMessageDialog(string Title,string Message, DialogTitleColor DialogTitleColor)
        {
            var mDialogWindow = new DialogWindow();
            var mMessageDialog = new MessageDialog(Message, "Ok", "Cancel", false);
            var mDialogWindowViewModel = new DialogWindowViewModel(mDialogWindow)
            {
                Title = Title,
                Content = mMessageDialog,
                DialogTitleColor = DialogTitleColor
            };
            mDialogWindow.ViewModel = mDialogWindowViewModel;
            mDialogWindowViewModel.ShowDialog(mDialogWindowViewModel);
        }
        
        /// <summary>
        /// To show a confirmation message that returns a bool
        /// </summary>
        /// <param name="Title"></param>
        /// <param name="Message"></param>
        /// <param name="IsYesNoButtonVisible"></param>
        /// <returns></returns>
        public static bool ShowMessageDialog(string Title, string Message, bool IsYesNoButtonVisible)
        {
            var mDialogWindow = new DialogWindow();
            var mMessageDialog = new MessageDialog(Message, "Yes", "No", IsYesNoButtonVisible);
            var mDialogWindowViewModel = new DialogWindowViewModel(mDialogWindow)
            {
                Title = Title,
                Content = mMessageDialog,
                DialogTitleColor = DialogTitleColor.Yellow
            };
            mDialogWindow.ViewModel = mDialogWindowViewModel;
            mDialogWindowViewModel.ShowDialog(mDialogWindowViewModel);
            return mMessageDialog.IsYesPressed;
        }

        public static void ShowErrorMessage()
        {
            DialogManager.ShowMessageDialog("Error", "An error occured. Bug report has been captured.",DialogTitleColor.Red);
        }

        /// <summary>
        /// To show the ViewStudentRecord Dialog
        /// </summary>
        /// <param name="StudentID"></param>
        /// <param name="ParentID"></param>
        /// <param name="ClassName"></param>
        public static void ViewFeeDetails(string StudentID, string SelectedClass)
        {
            var mDialogPage = new DialogPage();
            var mDialog = new ViewFeeDetails(StudentID, SelectedClass);
            var mDialogWindowViewModel = new DialogWindowViewModel(mDialogPage)
            {
                Content = mDialog,
            };
            mDialogPage.ViewModel = mDialogWindowViewModel;
            mDialogWindowViewModel.ShowDialog(mDialogWindowViewModel);
        }

        public static void ViewTeacherRecord(string TeacherID)
        {
            var mDialogPage = new DialogPage();
            var mViewTeacherRecord = new ViewTeacher(TeacherID);
            var mDialogWindowViewModel = new DialogWindowViewModel(mDialogPage)
            {
                Content = mViewTeacherRecord,
            };
            mDialogPage.ViewModel = mDialogWindowViewModel;
            mDialogWindowViewModel.ShowDialog(mDialogWindowViewModel);
        }

        public static void EditTeacherRecord(string TeacherID)
        {
            var mDialogPage = new DialogPage();
            var mEditTeacherRecord = new EditTeacher(TeacherID);
            var mDialogWindowViewModel = new DialogWindowViewModel(mDialogPage)
            {
                Content = mEditTeacherRecord,
            };
            mDialogPage.ViewModel = mDialogWindowViewModel;
            mDialogWindowViewModel.ShowDialog(mDialogWindowViewModel);
        }

        public static void ShowValidationMessage()
        {
            ShowMessageDialog("Error", "Please correct the inputs before preceding.",DialogTitleColor.Red);
        }

        #region Parents Dialogs

        public static void ViewParentRecord(string ParentID)
        {
            var mDialogPage = new DialogPage();
            var mViewStudentRecord = new ViewParentRecord(ParentID);
            var mDialogWindowViewModel = new DialogWindowViewModel(mDialogPage)
            {
                Content = mViewStudentRecord,
            };
            mDialogPage.ViewModel = mDialogWindowViewModel;
            mDialogWindowViewModel.ShowDialog(mDialogWindowViewModel);
        }

        /// <summary>
        /// To show the EditStudentRecord dialog to edit the student record
        /// </summary>
        /// <param name="StudentID"></param>
        /// <param name="ParentID"></param>
        /// <param name="ClassName"></param>
        public static void EditParents(string ParentID)
        {
            var mDialogPage = new DialogPage();
            var mEditStudentRecord = new EditParentsRecord(ParentID);
            var mDialogWindowViewModel = new DialogWindowViewModel(mDialogPage)
            {
                Content = mEditStudentRecord,
            };
            mDialogPage.ViewModel = mDialogWindowViewModel;
            mDialogWindowViewModel.ShowDialog(mDialogWindowViewModel);
        }

        #endregion
    }
}

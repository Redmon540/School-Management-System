using System;
using System.Diagnostics;
using System.Globalization;

namespace School_Manager
{
    /// <summary>
    /// Converts the <see cref="ApplicationPage"/> to an actual view/page
    /// </summary>
    public class ApplicationPageValueConverter : BaseValueConverter<ApplicationPageValueConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //Find the appropriate page
            switch((ApplicationPage)value)
            {
                case ApplicationPage.Login:
                    return new LoginPage();
                case ApplicationPage.Students:
                    return new Students();
                case ApplicationPage.CreateClass:
                    return new CreateClass();
                case ApplicationPage.AdmitStudent:
                    return new AdmitStudent();
                case ApplicationPage.CreateFeeRecord:
                    return new CreateFeeRecord();
                case ApplicationPage.FeeCollection:
                    return new FeeCollection();
                case ApplicationPage.FeeRecord:
                    return new FeeRecord();
                case ApplicationPage.CreateFeeVouchers:
                    return new CreateFeeVoucher();
                case ApplicationPage.AdmitTeacher:
                    return new AdmitTeacher();
                case ApplicationPage.Teachers:
                    return new Teachers();
                case ApplicationPage.SignUp:
                    return new CreateAccount();
                case ApplicationPage.ViewParents:
                    return new Parents();
                case ApplicationPage.CreateStudentIDCard:
                    return new CreateStudentIDCard();
                case ApplicationPage.CreateCustomIDCards:
                    return new CreateCustomIDCards();
                case ApplicationPage.TakeAttendence:
                    return new TakeAttendence();
                case ApplicationPage.ViewStudentAttendence:
                    return new StudentAttendence();
                case ApplicationPage.ViewTeacherAttendence:
                    return new TeacherAttendence();
                case ApplicationPage.MarkAttendenceManually:
                    return new MarkAttendenceManually();
                case ApplicationPage.TeacherSalary:
                    return new TeacherSalary();
                case ApplicationPage.EditClass:
                    return new EditClass();
                case ApplicationPage.PromoteStudents:
                    return new PromoteStudents();
                case ApplicationPage.Dashboard:
                    return new Dashboard();
                case ApplicationPage.ImportStudents:
                    return new ImportStudents();
                case ApplicationPage.ImportTeachers:
                    return new ImportTeacherData();
                case ApplicationPage.EditStudentInfoStructure:
                    return new EditStudentInfo();
                case ApplicationPage.EditTeacherInfoStructure:
                    return new EditTeacherInfo();
                case ApplicationPage.Registration:
                    return new Registeration();
                case ApplicationPage.Expense:
                    return new EnterExpense();
                case ApplicationPage.About:
                    return new About();
                case ApplicationPage.SendMessage:
                    return new SendMessage();
                case ApplicationPage.SetProductInfo:
                    return new SetProductInformation();
                default:
                    Debugger.Break();
                    return null;
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

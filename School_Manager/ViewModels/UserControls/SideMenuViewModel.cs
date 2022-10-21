using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace School_Manager
{
    public class SideMenuViewModel : BasePropertyChanged
    {
        #region Default Constructor

        public SideMenuViewModel()
        {
            string Icon = Application.Current.FindResource("DashboardIcon") as string;

            //initialize the properties
            SideMenuItems = new ObservableCollection<SideMenuItem>()
            {
                new SideMenuItem()
                {
                    Content = "Dashboard",
                    Icon = Application.Current.FindResource("DashboardIcon") as string,
                    Items = new ObservableCollection<SideMenuSubItems>()
                    {
                        new SideMenuSubItems() { Content = "View Dashboard"  , Page = ApplicationPage.Dashboard , IsSelected = true},
                    },
                },
                new SideMenuItem()
                {
                    Content = "Students",
                    Icon = Application.Current.FindResource("StudentIcon") as string,
                    Items = new ObservableCollection<SideMenuSubItems>()
                    {
                        new SideMenuSubItems() { Content = "View Students"  , Page = ApplicationPage.Students},
                        new SideMenuSubItems() { Content = "Admit Student" , Page = ApplicationPage.AdmitStudent},
                        new SideMenuSubItems() { Content = "Promote Student" , Page = ApplicationPage.PromoteStudents }
                    }
                },
                new SideMenuItem()
                {
                    Content = "Class",
                    Icon = Application.Current.FindResource("ClassIcon") as string,
                    Items = new ObservableCollection<SideMenuSubItems>()
                    {
                        new SideMenuSubItems() { Content = "Create Class" , Page = ApplicationPage.CreateClass},
                        new SideMenuSubItems() { Content = "Edit Class" , Page = ApplicationPage.EditClass }
                    }
                },
                new SideMenuItem()
                {
                    Content = "Parents",
                    Icon = Application.Current.FindResource("ParentIcon") as string,
                    Items = new ObservableCollection<SideMenuSubItems>()
                    {
                        new SideMenuSubItems() { Content = "View Parents" , Page = ApplicationPage.ViewParents },
                    }
                },
                new SideMenuItem()
                {
                    Content = "Teachers",
                    Icon = Application.Current.FindResource("TeacherIcon") as string,
                    Items = new ObservableCollection<SideMenuSubItems>()
                    {
                        new SideMenuSubItems() { Content = "View Teachers" , Page = ApplicationPage.Teachers },
                        new SideMenuSubItems() { Content = "Admit Teacher" , Page = ApplicationPage.AdmitTeacher},
                        new SideMenuSubItems() { Content = "Salary Sheet" , Page = ApplicationPage.TeacherSalary}
                    }
                },
                new SideMenuItem()
                {
                    Content = "Fee",
                    Icon = Application.Current.FindResource("FeeIcon") as string,
                    Items = new ObservableCollection<SideMenuSubItems>()
                    {
                        new SideMenuSubItems() { Content = "View Fee Details" , Page = ApplicationPage.FeeRecord},
                        new SideMenuSubItems() { Content = "Fee Collection" , Page = ApplicationPage.FeeCollection},
                        new SideMenuSubItems() { Content = "Create Fee Record" , Page = ApplicationPage.CreateFeeRecord},
                        new SideMenuSubItems() { Content = "Create Fee Voucher" , Page = ApplicationPage.CreateFeeVouchers}
                    }
                },
                new SideMenuItem()
                {
                    Content = "Attendence",
                    Icon = Application.Current.FindResource("AttendenceIcon") as string,
                    Items = new ObservableCollection<SideMenuSubItems>()
                    {
                        new SideMenuSubItems() { Content = "Student Attendence" , Page = ApplicationPage.ViewStudentAttendence},
                        new SideMenuSubItems() { Content = "Teacher Attendence" , Page = ApplicationPage.ViewTeacherAttendence},
                        new SideMenuSubItems() { Content = "Auto Attendence" , Page = ApplicationPage.TakeAttendence},
                        new SideMenuSubItems() { Content = "Manual Attendence" , Page = ApplicationPage.MarkAttendenceManually},
                    }
                },
                new SideMenuItem()
                {
                    Content = "ID Cards",
                    Icon = Application.Current.FindResource("IDCardIcon") as string,
                    Items = new ObservableCollection<SideMenuSubItems>()
                    {
                        new SideMenuSubItems() { Content = "Create ID Cards" , Page = ApplicationPage.CreateCustomIDCards},
                    }
                },
                new SideMenuItem()
                {
                    Content = "Expanse",
                    Icon = Application.Current.FindResource("ExpenseIcon") as string,
                    Items = new ObservableCollection<SideMenuSubItems>()
                    {
                        new SideMenuSubItems() { Content = "Manage Expenses" , Page = ApplicationPage.Expense},
                    }
                },
                new SideMenuItem()
                {
                    Content = "Message",
                    Icon = Application.Current.FindResource("MessageIcon") as string,
                    Items = new ObservableCollection<SideMenuSubItems>()
                    {
                        new SideMenuSubItems() { Content = "Send Message" , Page = ApplicationPage.SendMessage},
                        //new SideMenuSubItems() { Content = "Outbox" , Page = ApplicationPage.SendMessage},
                    }
                },
                new SideMenuItem()
                {
                    Content = "Extras",
                    Icon = Application.Current.FindResource("ExtraIcon") as string,
                    Items = new ObservableCollection<SideMenuSubItems>()
                    {
                        new SideMenuSubItems() { Content = "Import Student Data" , Page = ApplicationPage.ImportStudents},
                        new SideMenuSubItems() { Content = "Import Teacher Data" , Page = ApplicationPage.ImportTeachers},
                        new SideMenuSubItems() { Content = "Edit Student Structure" , Page = ApplicationPage.EditStudentInfoStructure},
                        new SideMenuSubItems() { Content = "Edit Teacher Structure" , Page = ApplicationPage.EditTeacherInfoStructure},
                    }
                },
                new SideMenuItem()
                {
                    Content = "About",
                    Icon = Application.Current.FindResource("AboutIcon") as string,
                    Items = new ObservableCollection<SideMenuSubItems>()
                    {
                        new SideMenuSubItems() { Content = "Product Registeration" , Page = ApplicationPage.Registration},
                        new SideMenuSubItems() { Content = "About Product" , Page = ApplicationPage.About},
                    }
                }
            };

            //initialize the commands
            MainItemCommand = new RelayParameterizedCommand(parameter =>  MainItemClick(parameter));
            SubItemCommand = new RelayParameterizedCommand(parameter =>  SubItemClick(parameter));

            //initially set the subitems
        }

        #endregion

        #region Public Properties

        public ObservableCollection<SideMenuItem> SideMenuItems { get; set; }

        /// <summary>
        /// Control the animation behaviour of subitems
        /// </summary>
        public bool IsAnimated { get; set; } = true;

        #endregion

        #region Commands

        public ICommand MainItemCommand { get; set; }

        public ICommand SubItemCommand { get; set; }

        #endregion

        #region Command Methods

        private async void MainItemClick(object parameter)
        {
            var values = (object[])parameter;
            Button btn = (Button)values[0];
            ItemsControl subItems = (ItemsControl)values[1];

            SideMenuItem item = btn.DataContext as SideMenuItem;
            if (item.CanOpen == false)
                return;
            if (item.IsOpen)
            {
                item.IsOpen = false;
                if(IsAnimated)
                {
                    DoubleAnimation animation = new DoubleAnimation
                    {
                        Duration = TimeSpan.FromSeconds(0.2),
                        EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseOut },
                        From = item.Items.Count * 40,
                        To = 0
                    };
                    subItems.BeginAnimation(ItemsControl.HeightProperty, animation);
                    await Task.Delay(500);
                }
                subItems.Visibility = Visibility.Collapsed;
            }
            else
            {
                item.IsOpen = true;
                subItems.Visibility = Visibility.Visible;
                if(IsAnimated)
                {
                    DoubleAnimation animation = new DoubleAnimation
                    {
                        Duration = TimeSpan.FromSeconds(0.2),
                        EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseIn },
                        From = 0,
                        To = item.Items.Count * 40
                    };
                    subItems.BeginAnimation(ItemsControl.HeightProperty, animation);
                    await Task.Delay(500);
                }
            }
        }

        private void SubItemClick(object parameter)
        {
            Button btn = (Button)parameter;
            SideMenuSubItems subItem = (SideMenuSubItems)btn.DataContext;
            (Application.Current.MainWindow.DataContext as WindowViewModel).CurrentPage = subItem.Page;
            subItem.IsSelected = true;
            foreach(SideMenuItem item in SideMenuItems)
            {
                foreach(SideMenuSubItems subitem in item.Items)
                {
                    if (subitem != subItem)
                        subitem.IsSelected = false;
                }
            }
        }

        #endregion
    }
}

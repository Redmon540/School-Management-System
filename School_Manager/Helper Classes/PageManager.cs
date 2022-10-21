using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace School_Manager
{
    public class PageManager
    {
        public static void GotoPage(ApplicationPage Page)
        {
            if(Page == ApplicationPage.Login || Page == ApplicationPage.SignUp)
                (Application.Current.MainWindow.DataContext as LoginWindowViewModel).CurrentPage = Page;
            else
                (Application.Current.MainWindow.DataContext as WindowViewModel).CurrentPage = Page;
        }
    }
}

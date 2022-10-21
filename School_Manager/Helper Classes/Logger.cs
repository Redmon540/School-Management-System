using System;

namespace School_Manager
{
    public static class Logger
    {
        public static void Log(Exception Error)
        {
            System.IO.File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "Log.txt",
                $@"{DateTime.Now.ToString()} {Environment.NewLine} {Error} {Environment.NewLine}{Environment.NewLine}");
        }

        public static void Log(Exception Error,bool ShowErrorMessage)
        {
            System.IO.File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "Log.txt",
                $@"{DateTime.Now.ToString()} {Environment.NewLine} {Error} {Environment.NewLine}{Environment.NewLine}");
            if(ShowErrorMessage)
                DialogManager.ShowMessageDialog("Error", "An error occured. Bug report has been captured.",DialogTitleColor.Red);
        }

        public static void ShowError()
        {
            DialogManager.ShowMessageDialog("Error", "An error occured. Bug report has been captured.",DialogTitleColor.Red);
        }
    }
}

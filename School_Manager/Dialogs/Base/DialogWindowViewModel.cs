using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace School_Manager
{

    public class DialogWindowViewModel : WindowViewModel
    {
        #region Public Properties

        /// <summary>
        /// The title of this dialog window
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The content to host inside the dialog
        /// </summary>
        public Control Content { get; set; }

        /// <summary>
        /// Title brush
        /// </summary>
        public SolidColorBrush TitleBarColor 
        {
            get
            {
                if (DialogTitleColor == DialogTitleColor.Green)
                    return (SolidColorBrush)Application.Current.FindResource("DarkGreenColorBrush");
                if (DialogTitleColor == DialogTitleColor.Red)
                    return (SolidColorBrush)Application.Current.FindResource("RedColorBrush");

                return (SolidColorBrush)Application.Current.FindResource("DarkYellowColorBrush");
            }
        }

        public DialogTitleColor DialogTitleColor { get; set; } = DialogTitleColor.Yellow;

        /// <summary>
        /// Parent Control View Model
        /// </summary>
        public BaseViewModel mParentWindowViewModel { get; set; }

        /// <summary>
        /// Dialog Window of this ViewModel
        /// </summary>
        public Window mDialogWindow { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="window">Dialog Window</param>
        public DialogWindowViewModel(Window DialogWindow) : base(DialogWindow)
        {
            mDialogWindow = DialogWindow;
            mParentWindowViewModel = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive).DataContext as BaseViewModel;
            // Make minimum size smaller
            MinWindowWidth = 350;
            MinWindowHeight = 100;


            ResizeBorder = 0;
            
            // Make title bar smaller
            TitleHeight = 30;

            //Alter the Close Command to Remove the bluroverlay on dialog close
            CloseCommand = new RelayCommand(() =>
            {
                mDialogWindow.Close();
            });
        }

        /// <summary>
        /// Displays a single message box to the user
        /// </summary>
        /// <param name="viewModel">The view model</param>
        /// <typeparam name="T">The view model type for this control</typeparam>
        /// <returns></returns>
        public Task ShowDialog<T>(T viewModel)
            where T : DialogWindowViewModel
        {
            //To show blur overlay on show dialog
            mParentWindowViewModel.IsBlurOverlayVisible = true;

            // Create a task to await the dialog closing
            var tcs = new TaskCompletionSource<bool>();

            // Run on UI thread
            Application.Current.Dispatcher.Invoke(() =>
            {
                try
                {
                    // Show in the center of the parent
                    mDialogWindow.Owner = Application.Current.MainWindow;
                    mDialogWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;

                    // Show dialog
                    mDialogWindow.ShowDialog();
                }
                finally
                {
                    // Let caller know we finished
                    tcs.TrySetResult(true);
                }
            });

            //To Remove the bluroverlay on dialog close
            mParentWindowViewModel.IsBlurOverlayVisible = false;

            return tcs.Task;
        }
        #endregion
    }
}
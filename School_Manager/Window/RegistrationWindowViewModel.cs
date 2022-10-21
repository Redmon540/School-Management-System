using System.Windows;
using System.Windows.Input;

namespace School_Manager
{
    public class RegistrationWindowViewModel : BaseViewModel
    {
        #region Private Members

        protected Window mWindow;
        private int mWindowRadius = 20;

        #endregion

        #region Public Properties

        public double MinWindowWidth { get; set; } = 700;

        public double MinWindowHeight { get; set; } = 450;

        public int ResizeBorder { get; set; } = 5;

        public Thickness ResizeBorderThickness { get { return new Thickness(ResizeBorder); } }

        public Thickness InnerContentPadding { get { return new Thickness(0); } }

        public int WindowRadius
        {
            get => mWindow.WindowState == WindowState.Maximized ? 0 : mWindowRadius;
            set => mWindowRadius = value;
        }

        public CornerRadius WindowCornerRadius { get { return new CornerRadius(WindowRadius); } }

        public double TitleHeight { get; set; } = 42;

        public GridLength TitleHeightGridLength { get { return new GridLength(TitleHeight); } }

        public RegistrationControlViewModel RegistrationViewModel { get; set; } = new RegistrationControlViewModel();

        #endregion

        #region Public Commands

        public ICommand MaximizedCommand { get; set; }

        public ICommand MinimizedCommand { get; set; }

        public ICommand CloseCommand { get; set; }


        #endregion

        #region Constructor
        public RegistrationWindowViewModel(Window Window)
        {
            mWindow = Window;

            //Listen out for the window resizing
            mWindow.StateChanged += (sender, e) =>
            {
                //fire off events for all properties that are effected by a resize
                OnPropertyChanged(nameof(ResizeBorderThickness));
                OnPropertyChanged(nameof(WindowRadius));
                OnPropertyChanged(nameof(WindowCornerRadius));
            };


            //Create Commands
            MinimizedCommand = new RelayCommand(() => mWindow.WindowState = WindowState.Minimized);
            MaximizedCommand = new RelayCommand(() => mWindow.WindowState ^= WindowState.Maximized);
            CloseCommand = new RelayCommand(() => mWindow.Close());


            //Fixes Window Resize Issue
            var resizer = new WindowResizer(mWindow);
        }
        #endregion
    }
}

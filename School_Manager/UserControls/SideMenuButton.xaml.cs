using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace School_Manager
{
    /// <summary>
    /// Interaction logic for SideMenuButton.xaml
    /// </summary>
    public partial class SideMenuButton : UserControl
    {
        #region Default Constructor
        public SideMenuButton()
        {
            InitializeComponent();

            //To set the IsOpen Property of the SideMenu
            IsOpen = false;
            
            // Sets the data context of the Side Menu User Control to itself
            this.DataContext = this;
        }
        #endregion

        #region Public Properties
        public string MainContent { get; set; }

        /// <summary>
        /// Gets or sets additional content for the UserControl
        /// </summary>
        public Visibility PanelVisibility
        {
            get { return (Visibility) GetValue(PanelVisibilityProperty); }
            set { SetValue(PanelVisibilityProperty, value); }
        }
        public static readonly DependencyProperty PanelVisibilityProperty =
            DependencyProperty.Register("PanelVisibility", typeof(Visibility), typeof(SideMenuButton),
              new PropertyMetadata(null));

        /// <summary>
        /// Gets or Sets the visibility of side menu
        /// </summary>
        public object AdditionalContent
        {
            get { return GetValue(AdditionalContentProperty); }
            set { SetValue(AdditionalContentProperty, value); }
        }

        public static readonly DependencyProperty AdditionalContentProperty =
            DependencyProperty.Register("AdditionalContent", typeof(object), typeof(SideMenuButton),
              new PropertyMetadata(null));

        /// <summary>
        /// Tells if the SubMenu Panel is open or close
        /// </summary>
        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(SideMenuButton),
              new PropertyMetadata(null));

        #endregion

        #region Private Members

        private double PanelHeight;

        #endregion

        #region Public Commands

        public ICommand command { get; set; }

        #endregion

        /// <summary>
        /// To Control the expand and collaps animation of the sub menu content
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            
            //To Close
            if (IsOpen)
            {
                IsOpen = false;
                Storyboard sb = new Storyboard();
                DoubleAnimation animation = new DoubleAnimation
                {
                    Duration = TimeSpan.FromSeconds(0.2),
                    EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseOut },
                    From = PanelHeight,
                    To = 0
                };
                SubContentPanel.BeginAnimation(HeightProperty, animation);
                await Task.Delay(500);
                SubContentPanel.Visibility = Visibility.Collapsed;

            }
            //To Open
            else
            {
                IsOpen = true;
                SubContentPanel.Visibility = Visibility.Visible;
                Storyboard sb = new Storyboard();
                DoubleAnimation animation = new DoubleAnimation
                {
                    Duration = TimeSpan.FromSeconds(0.2),
                    EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseIn },
                    From = 0,
                    To = PanelHeight
                };
                SubContentPanel.BeginAnimation(StackPanel.HeightProperty, animation);
                await Task.Delay(500);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //To Get the actual height of the sub menu panel for animation
            PanelHeight = SubContentPanel.ActualHeight;
            SubContentPanel.Visibility = Visibility.Collapsed;
        }
    }
}

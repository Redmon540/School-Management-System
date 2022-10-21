using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace School_Manager
{

    /// <summary>
    /// Interaction logic for SearchBox.xaml
    /// </summary>
    public partial class SearchBox : UserControl
    {
        #region Private Members

        private float animationTime;

        #endregion

        #region Default Constructor

        public SearchBox()
        {
            InitializeComponent();
            animationTime = 0.4f;
            //To initialize commands
            CloseSearchBoxCommand = new RelayCommand(async () =>
            {
                if (IsSearchOpen)
                {
                    _ = SearchGrid.DecreaseWidth(animationTime, 0);
                    //To Deal with the frozen element error
                    if (border.Background.IsFrozen)
                    {
                        border.Background = border.Background.CloneCurrentValue();
                    }

                    var animation = new ColorAnimation
                    {
                        Duration = new Duration(TimeSpan.FromSeconds(animationTime)),
                        From = (Color)FindResource("VeryLightGray"),
                        To = (Color)FindResource("LightColor")
                    };
                    border.Background.BeginAnimation(SolidColorBrush.ColorProperty, animation);
                    await PopupGrid.DecreaseWidth(animationTime, 0);

                    textBox.Text = string.Empty;
                    IsSearchOpen = false;
                    popupBox.IsEnabled = false;
                    BeginSearch = true;
                    BeginSearch = false;
                }
            });
            BeginSearchCommand = new RelayCommand(() =>
            {
                //To begin the search (this will trigger the BeginSearch in Searchable datagrid)
                BeginSearch = true;

                //To set set the begin search to false after the search is performed
                BeginSearch = false;
            });


        }
        #endregion

        #region Public Properties

        /// <summary>
        /// Search text property to get the text that is being searched by user
        /// </summary>
        public string SearchText
        {
            get => (string)GetValue(SearchTextProperty);
            set => SetValue(SearchTextProperty, value);
        }
        public static readonly DependencyProperty SearchTextProperty =
          DependencyProperty.Register("SearchText", typeof(string), typeof(SearchBox), new PropertyMetadata(new PropertyChangedCallback(OnSearchTextChanged)));
        private static void OnSearchTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var searchBox = (SearchBox)d;
            searchBox.SearchText = e.NewValue as string;
        }

        /// <summary>
        /// Indicate if the search should begin
        /// </summary>
        public bool BeginSearch
        {
            get => (bool)GetValue(BeginSearchProperty);
            set => SetValue(BeginSearchProperty, value);
        }
        public static readonly DependencyProperty BeginSearchProperty =
          DependencyProperty.Register("BeginSearch", typeof(bool), typeof(SearchBox), new PropertyMetadata(new PropertyChangedCallback(OnBeginSearchChanged)));
        private static void OnBeginSearchChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var searchBox = (SearchBox)d;
            searchBox.BeginSearch = (bool)e.NewValue;
        }

        /// <summary>
        /// SearchColumns Dependency property contains list of Checkbox Elements
        /// to search just in user selected fields
        /// </summary>
        public ObservableCollection<Check> SearchColumns
        {
            get => (ObservableCollection<Check>)GetValue(SearchColumnsProperty);
            set => SetValue(SearchColumnsProperty, value);
        }
        public static readonly DependencyProperty SearchColumnsProperty =
            DependencyProperty.Register("SearchColumns", typeof(ObservableCollection<Check>), typeof(SearchBox), new PropertyMetadata(new PropertyChangedCallback(OnSearchColumnsChanged)));
        private static void OnSearchColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var searchBox = (SearchBox)d;
            searchBox.SearchColumns = e.NewValue as ObservableCollection<Check>;
        }

        /// <summary>
        /// Name of the selected class
        /// </summary>
        public string SelectedClass
        {
            get => (string)GetValue(SelectedClassProperty);
            set => SetValue(SelectedClassProperty, value);
        }
        public static readonly DependencyProperty SelectedClassProperty =
          DependencyProperty.Register("SelectedClass", typeof(string), typeof(SearchBox), new PropertyMetadata(new PropertyChangedCallback(OnSelectedClassChanged)));
        private static void OnSelectedClassChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var searchBox = (SearchBox)d;
            searchBox.SelectedClass = e.NewValue as string;
        }

        /// <summary>
        /// dategrid type (like parents, students , fee..)
        /// </summary>
        public DataGridType Type
        {
            get => (DataGridType)GetValue(TypeProperty);
            set => SetValue(TypeProperty, value);
        }
        public static readonly DependencyProperty TypeProperty =
        DependencyProperty.Register("Type", typeof(DataGridType), typeof(SearchBox), new PropertyMetadata(new PropertyChangedCallback(OnTypeChanged)));
        private static void OnTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SearchBox searchableDataGrid = (SearchBox)d;
            searchableDataGrid.Type = (DataGridType)e.NewValue;
        }


        /// <summary>
        /// Tells if search box is open or close
        /// </summary>
        public bool IsSearchOpen { get; set; }

        #endregion

        #region Commands

        public ICommand CloseSearchBoxCommand { get; set; }

        public ICommand BeginSearchCommand { get; set; }


        #endregion

        #region Events

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (!IsSearchOpen)
            {
                _ = SearchGrid.IncreaseWidth(animationTime, 250);
                //To Deal with the frozen element error
                if (border.Background.IsFrozen)
                {
                    border.Background = border.Background.CloneCurrentValue();
                }

                var animation = new ColorAnimation
                {
                    Duration = new Duration(TimeSpan.FromSeconds(animationTime)),
                    From = (Color)FindResource("LightColor"),
                    To = (Color)FindResource("VeryLightGray")
                };
                border.Background.BeginAnimation(SolidColorBrush.ColorProperty, animation);
                await PopupGrid.IncreaseWidth(animationTime, 50);

                textBox.Focus();
                IsSearchOpen = true;
                popupBox.IsEnabled = true;
            }
            else
            {
                textBox.Focus();
                BeginSearch = true;
                BeginSearch = false;
            }
        }

        private async void CancleButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsSearchOpen)
            {
                _ = SearchGrid.DecreaseWidth(animationTime, 0);
                //To Deal with the frozen element error
                if (border.Background.IsFrozen)
                {
                    border.Background = border.Background.CloneCurrentValue();
                }

                var animation = new ColorAnimation
                {
                    Duration = new Duration(TimeSpan.FromSeconds(animationTime)),
                    From = (Color)FindResource("VeryLightGray"),
                    To = (Color)FindResource("LightColor")
                };
                border.Background.BeginAnimation(SolidColorBrush.ColorProperty, animation);
                await PopupGrid.DecreaseWidth(animationTime, 0);

                textBox.Text = string.Empty;
                IsSearchOpen = false;
                popupBox.IsEnabled = false;
                BeginSearch = true;
                BeginSearch = false;
            }
        }

        private void PopupBox_Closed(object sender, RoutedEventArgs e)
        {
            try
            {
                if (PopupItems.ItemsSource != null)
                {
                    ObservableCollection<Check> SearchcheckBoxes = new ObservableCollection<Check>(PopupItems.ItemsSource as ObservableCollection<Check>);
                    string q = "";
                    foreach (Check checkBox in SearchcheckBoxes)
                    {
                        if (checkBox.IsChecked == true)
                        {
                            q += $",{checkBox.Content}";
                        }
                    }
                    if (q != "")
                    {
                        q = q.Remove(0, 1);
                        switch (Type)
                        {
                            case DataGridType.Students:
                                {
                                    DataAccess.ExecuteQuery($"UPDATE ColumnInformation SET Search_Columns = '{q}' WHERE Class_ID = {DataAccess.GetClassID(SelectedClass)}");
                                    break;
                                }
                            case DataGridType.Parents:
                                {
                                    DataAccess.ExecuteQuery($"UPDATE _ColumnInformation SET Search_Columns = '{q}' WHERE Table_Name = 'Parents'");
                                    break;
                                }
                            case DataGridType.Teachers:
                                {
                                    DataAccess.ExecuteQuery($"UPDATE _ColumnInformation SET Search_Columns = '{q}' WHERE Table_Name = 'Teachers'");
                                    break;
                                }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Logger.Log(ex, true);
            }
        }

        #endregion

    }
}

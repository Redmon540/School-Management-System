using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;

namespace School_Manager
{
    /// <summary>
    /// Interaction logic for SearchableDataGrid.xaml
    /// </summary>
    public partial class SearchableDataGrid : UserControl
    {
        #region Default Constructor
        public SearchableDataGrid()
        {
            InitializeComponent();
        }
        #endregion

        #region Public Dependency Properties

        /// <summary>
        /// SearchText Dependency Property to search in DataGrid
        /// </summary>
        public string SearchText
        {
            get
            {
                return GetValue(SearchTextProperty) as string;
            }
            set
            {
                SetValue(SearchTextProperty, value);
            }
        }
        public static readonly DependencyProperty SearchTextProperty =
        DependencyProperty.Register("SearchText", typeof(string), typeof(SearchableDataGrid), new PropertyMetadata(new PropertyChangedCallback(OnSearchTextChanged)));
        private static void OnSearchTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SearchableDataGrid searchableDataGrid = (SearchableDataGrid)d;
            searchableDataGrid.SearchText = e.NewValue as string;
        }

        /// <summary>
        /// ItemSource Dependency Property to set the itemsource of the DataGrid
        /// </summary>
        public DataTable ItemsSource
        {
            get { return (DataTable)GetValue(ItemsSourceProperty); }
            set
            {
                SetValue(ItemsSourceProperty, value);
                if (AddActionColumn && (Type == DataGridType.Students || Type == DataGridType.Teachers || Type == DataGridType.Parents))
                {
                    dataGrid.Columns.Clear();
                    dataGrid.Columns.Insert(0, new DataGridTemplateColumn() { CellTemplate = FindResource("ActionColumnTemplate") as DataTemplate, Header = "Actions" });
                }
                else if (AddActionColumn && Type == DataGridType.FeeRecord)
                {
                    dataGrid.Columns.Clear();
                    dataGrid.Columns.Insert(0, new DataGridTemplateColumn() { CellTemplate = FindResource("ViewFeeRecordTemplate") as DataTemplate, Header = "Actions" });
                }
            }
        }
        public static readonly DependencyProperty ItemsSourceProperty =
          DependencyProperty.Register("ItemsSource", typeof(DataTable), typeof(SearchableDataGrid), new PropertyMetadata(new PropertyChangedCallback(OnItemSourceChanged)));
        private static void OnItemSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var searchableDataGrid = (SearchableDataGrid)d;
            searchableDataGrid.ItemsSource = e.NewValue as DataTable;
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
            DependencyProperty.Register("SearchColumns", typeof(ObservableCollection<Check>), typeof(SearchableDataGrid), new PropertyMetadata(new PropertyChangedCallback(OnSearchColumnsChanged)));
        private static void OnSearchColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var searchableDataGrid = (SearchableDataGrid)d;
            searchableDataGrid.SearchColumns = e.NewValue as ObservableCollection<Check>;
        }

        /// <summary>
        /// Indicate if the search should begin
        /// </summary>
        public bool BeginSearch
        {
            get => (bool)GetValue(BeginSearchProperty);
            set
            {
                using (new WaitCursor())
                {
                    SetValue(BeginSearchProperty, value);
                    if (value)
                    {
                        if (SearchText == "")
                        {
                            if (dataGrid.ItemsSource != null)
                            {
                                (dataGrid.ItemsSource as DataView).RowFilter = "";
                            }

                            return;
                        }
                        FilterGrid();
                        HighText();
                    }

                }
            }
        }
        public static readonly DependencyProperty BeginSearchProperty =
          DependencyProperty.Register("BeginSearch", typeof(bool), typeof(SearchableDataGrid), new PropertyMetadata(new PropertyChangedCallback(OnBeginSearchChanged)));
        private static void OnBeginSearchChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var searchBox = (SearchableDataGrid)d;
            searchBox.BeginSearch = (bool)e.NewValue;
        }

        /// <summary>
        /// Indicate if the data is loading
        /// </summary>
        public bool IsDataLoading
        {
            get => (bool)GetValue(IsDataLoadingProperty);
            set => SetValue(IsDataLoadingProperty, value);
        }
        public static readonly DependencyProperty IsDataLoadingProperty =
          DependencyProperty.Register("IsDataLoading", typeof(bool), typeof(SearchableDataGrid), new PropertyMetadata(new PropertyChangedCallback(OnIsDataLoadingChanged)));
        private static void OnIsDataLoadingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var searchBox = (SearchableDataGrid)d;
            searchBox.IsDataLoading = (bool)e.NewValue;
        }


        /// <summary>
        /// Selected index of datagrid
        /// </summary>
        public object SelectedItem
        {
            get => (int)GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }
        public static readonly DependencyProperty SelectedItemProperty =
        DependencyProperty.Register("SelectedItem", typeof(object), typeof(SearchableDataGrid), new PropertyMetadata(new PropertyChangedCallback(OnSelectedItemChanged)));
        private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SearchableDataGrid searchableDataGrid = (SearchableDataGrid)d;
            searchableDataGrid.SelectedItem = e.NewValue;
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
        DependencyProperty.Register("Type", typeof(DataGridType), typeof(SearchableDataGrid), new PropertyMetadata(new PropertyChangedCallback(OnTypeChanged)));
        private static void OnTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SearchableDataGrid searchableDataGrid = (SearchableDataGrid)d;
            searchableDataGrid.Type = (DataGridType)e.NewValue;
        }

        /// <summary>
        /// To add view edit and delete button in datagrid
        /// </summary>
        public bool AddActionColumn { get; set; } = true;

        #endregion

        #region Filter Methods

        /// <summary>
        /// To Filter rows of datagrid according to search result
        /// </summary>
        private void FilterGrid()
        {
            if (dataGrid.ItemsSource != null)
            {
                string query = "";
                foreach (Check checkbox in SearchColumns)
                {
                    if (checkbox.IsChecked == true)
                    {
                        if (checkbox.Content == DataAccess.GetStudentID() || checkbox.Content == DataAccess.GetParentID() || checkbox.Content == DataAccess.GetTeacherID())
                        {
                            // to check if its the first columns and form the appropriate filter query ;)
                            if (query == "")
                            {
                                query = $"Convert([{checkbox.Content}], System.String) LIKE '{SearchText}'";
                            }
                            else
                            {
                                query += $" or Convert([{checkbox.Content}], System.String) LIKE '{SearchText}'";
                            }
                        }
                        else
                        {
                            // to check if its the first columns and form the appropriate filter query ;)
                            if (query == "")
                            {
                                query = $"Convert([{checkbox.Content}], System.String) LIKE '%{SearchText}%'";
                            }
                            else
                            {
                                query += $" or Convert([{checkbox.Content}], System.String) LIKE '%{SearchText}%'";
                            }
                        }
                    }
                }
                (dataGrid.ItemsSource as DataView).RowFilter = query;
            }

        }

        #endregion

        #region Search Methods

        //problem not highliting search
        public void HighlightSearch()
        {
            bool success = true;
            try
            {
                if (dataGrid.ItemsSource != null)
                {
                    for (int i = 0; i < dataGrid.Columns.Count - 1; i++)
                    {
                        var checkbox = SearchColumns[i];
                        if (checkbox.IsChecked == true)
                        {
                            if (checkbox.Content == "Student ID")
                            {
                                for (int j = 0; j < dataGrid.Items.Count; j++)
                                {
                                    DataGridCell cell = dataGrid.GetCell(j, i);
                                    if (cell.Content is TextBlock textBlock)
                                    {
                                        string Text = textBlock.Text;
                                        if (string.IsNullOrEmpty(SearchText))
                                        {
                                            textBlock.Inlines.Clear();
                                            textBlock.Text = Text;
                                        }
                                        else if (textBlock != null)
                                        {
                                            textBlock.Inlines.Clear();
                                            if (SearchText == textBlock.Text)
                                            {
                                                for (int k = 0; k < Text.Length; k++)
                                                {
                                                    Run runx = new Run(Text[k].ToString());
                                                    runx.Background = (Brush)FindResource("SecondaryColorBrush");
                                                    textBlock.Inlines.Add(runx);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                for (int j = 0; j < dataGrid.Items.Count; j++)
                                {
                                    DataGridCell cell = dataGrid.GetCell(j, i);
                                    if (cell.Content is TextBlock textBlock)
                                    {
                                        string Text = textBlock.Text;
                                        if (string.IsNullOrEmpty(SearchText))
                                        {
                                            textBlock.Inlines.Clear();
                                            textBlock.Text = Text;
                                        }
                                        else if (textBlock != null)
                                        {
                                            textBlock.Inlines.Clear();
                                            Match match = Regex.Match(Text, $"({SearchText})", RegexOptions.IgnoreCase);
                                            for (int k = 0; k < Text.Length; k++)
                                            {
                                                if (k >= match.Index && k < (SearchText.Length + match.Index) && match.Success)
                                                {
                                                    Run runx = new Run(Text[k].ToString());
                                                    runx.Background = (Brush)FindResource("SecondaryColorBrush");
                                                    textBlock.Inlines.Add(runx);
                                                }
                                                else
                                                {
                                                    textBlock.Inlines.Add(Text[k].ToString());
                                                }
                                            }

                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                success = false;
                Logger.Log(ex);
            }
            finally
            {
                if (!success)
                {
                    DialogManager.ShowErrorMessage();
                }
            }
        }

        /// <summary>
        /// Method to highlight searched text in datagrid cells
        /// </summary>
        /// <param name="datagrid">DataGrid to search in</param>
        /// <param name="SearchText">Text to search</param>
        public void HighText()
        {
            if (SearchText == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(SearchText))
            {
                if ((dataGrid.ItemsSource as DataView).RowFilter == "")
                {
                    return;
                }
            }
            string searchtext = SearchText;
            Regex SearchRegex = new Regex(searchtext, RegexOptions.IgnoreCase);
            if (dataGrid.ItemsSource != null)
            {
                var highlightBrush = FindResource("SecondaryColorBrush") as Brush;
                int i = 0;
                foreach (var checkbox in SearchColumns)
                {
                    if (checkbox.IsChecked == true)
                    {
                        i = dataGrid.Columns.IndexOf(dataGrid.Columns.FirstOrDefault(c => c.Header.ToString() == checkbox.Content));
                        if (i == -1)
                            continue;
                        if (checkbox.Content == DataAccess.GetStudentID() || checkbox.Content == DataAccess.GetParentID() || checkbox.Content == DataAccess.GetTeacherID())
                        {
                            for (int j = 0; j < dataGrid.Items.Count; j++)
                            {
                                DataGridCell cell = dataGrid.GetCell(j, i);
                                if (cell != null)
                                    if (cell.Content is TextBlock itx)
                                {
                                    Regex regex = new Regex("(" + SearchText + ")", RegexOptions.IgnoreCase);
                                    TextBlock tb = itx as TextBlock;
                                    if (SearchText.Length == 0)
                                    {
                                        string str = tb.Text;
                                        tb.Inlines.Clear();
                                        tb.Inlines.Add(str);
                                    }
                                    else
                                    {
                                        if (SearchText == tb.Text)
                                        {
                                            string str = tb.Text;
                                            tb.Inlines.Clear();
                                            for (int k = 0; k < str.Length; k++)
                                            {
                                                Run runx = new Run(str[k].ToString());
                                                runx.Background = (Brush)FindResource("SecondaryColorBrush");
                                                tb.Inlines.Add(runx);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            for (int j = 0; j < dataGrid.Items.Count; j++)
                            {
                                DataGridCell cell = dataGrid.GetCell(j, i);
                                string[] substrings = null;
                                if(cell != null)
                                if (cell.Content is TextBlock itx)
                                {
                                    Regex regex = new Regex("(" + SearchText + ")", RegexOptions.IgnoreCase);
                                    TextBlock tb = itx as TextBlock;
                                    if (SearchText.Length == 0)
                                    {
                                        string str = tb.Text;
                                        tb.Inlines.Clear();
                                        tb.Inlines.Add(str);
                                    }
                                    else
                                    {
                                        substrings = regex.Split(tb.Text);
                                        tb.Inlines.Clear();
                                        foreach (var item in substrings)
                                        {
                                            if (regex.Match(item).Success)
                                            {
                                                Run runx = new Run(item);
                                                runx.Background = highlightBrush;
                                                tb.Inlines.Add(runx);
                                            }
                                            else
                                            {
                                                tb.Inlines.Add(item);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }


        #endregion

        private void AutoGeneratingColumns(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            switch (Type)
            {
                case DataGridType.Students:
                    {
                        if (e.PropertyType == typeof(byte[]))
                        {
                            FrameworkElementFactory photo = new FrameworkElementFactory(typeof(PhotoTemplate));
                            photo.SetValue(PhotoTemplate.BindingPathProperty, e.Column.Header);
                            DataTemplate template = new DataTemplate();
                            template.VisualTree = photo;
                            e.Column = new DataGridTemplateColumn() { CellTemplate = template, Header = e.Column.Header };
                            return;
                        }
                        break;
                    }
                case DataGridType.StudentWithoutAction:
                    {
                        if (e.PropertyType == typeof(byte[]))
                        {
                            FrameworkElementFactory photo = new FrameworkElementFactory(typeof(PhotoTemplate));
                            photo.SetValue(PhotoTemplate.BindingPathProperty, e.Column.Header);
                            DataTemplate template = new DataTemplate();
                            template.VisualTree = photo;
                            e.Column = new DataGridTemplateColumn() { CellTemplate = template, Header = e.Column.Header };
                            return;
                        }
                        break;
                    }
                case DataGridType.Parents:
                    {
                        if (e.PropertyType == typeof(byte[]))
                        {
                            FrameworkElementFactory photo = new FrameworkElementFactory(typeof(PhotoTemplate));
                            photo.SetValue(PhotoTemplate.BindingPathProperty, e.Column.Header);
                            DataTemplate template = new DataTemplate();
                            template.VisualTree = photo;
                            e.Column = new DataGridTemplateColumn() { CellTemplate = template, Header = e.Column.Header };
                            return;
                        }
                        break;
                    }
                case DataGridType.FeeCollection:
                    {
                        break;
                    }
                case DataGridType.FeeRecord:
                    {
                        if (e.PropertyType == typeof(bool))
                        {
                            e.Cancel = true;
                            DataTemplate dataTemplate = new DataTemplate();
                            FrameworkElementFactory button = new FrameworkElementFactory(typeof(Button));
                            var binding = new Binding()
                            {
                                Path = new PropertyPath(e.PropertyName),
                                Mode = BindingMode.TwoWay,
                            };
                            button.SetValue(StyleProperty, FindResource("FeeButton") as Style);
                            button.SetBinding(ContentProperty, binding);
                            dataTemplate.VisualTree = button;
                            dataGrid.Columns.Add(new DataGridTemplateColumn()
                            {
                                CellTemplate = dataTemplate,
                                Header = e.Column.Header
                            });
                            return;

                        }
                        else if (e.PropertyName == "Total")
                        {
                            e.Column = new DataGridTemplateColumn() { CellTemplate = (DataTemplate)FindResource("TotalFillTemplate"), Header = "Total" };
                            return;
                        }
                        else if (e.PropertyName == "FeeStatus")
                        {
                            e.Cancel = true;
                            return;
                        }
                        break;
                    }

                case DataGridType.Teachers:
                    {
                        if (e.PropertyType == typeof(byte[]))
                        {
                            FrameworkElementFactory photo = new FrameworkElementFactory(typeof(PhotoTemplate));
                            photo.SetValue(PhotoTemplate.BindingPathProperty, e.Column.Header);
                            DataTemplate template = new DataTemplate();
                            template.VisualTree = photo;
                            e.Column = new DataGridTemplateColumn() { CellTemplate = template, Header = e.Column.Header };
                            return;
                        }
                        break;
                    }
                case DataGridType.Attendence:
                    {
                        if (e.PropertyName != "Name" && e.PropertyName != DataAccess.GetStudentID() && e.PropertyName != DataAccess.GetTeacherID())
                        {
                            DataTemplate dataTemplate = new DataTemplate();
                            FrameworkElementFactory button = new FrameworkElementFactory(typeof(Button));
                            var binding = new Binding()
                            {
                                Path = new PropertyPath(e.PropertyName),
                                Mode = BindingMode.TwoWay,
                            };
                            button.SetValue(StyleProperty, FindResource("AttendenceButton") as Style);
                            button.SetBinding(DataContextProperty, binding);
                            dataTemplate.VisualTree = button;
                            var column = new DataGridTemplateColumn()
                            {
                                CellTemplate = dataTemplate,
                                Header = e.Column.Header
                            };
                            e.Column = column;
                            return;

                        }
                        else if (e.PropertyType == typeof(byte[]))
                        {
                            FrameworkElementFactory photo = new FrameworkElementFactory(typeof(PhotoTemplate));
                            photo.SetValue(PhotoTemplate.BindingPathProperty, e.Column.Header);
                            DataTemplate template = new DataTemplate();
                            template.VisualTree = photo;
                            e.Column = new DataGridTemplateColumn() { CellTemplate = template, Header = e.Column.Header };
                            return;
                        }
                        break;
                    }
            }

            #region Handle Special Character Column Names

            var columnName = (string)e.Column.Header;

            //COPY PASTE CODE :)
            // We'll build a string with escaped characters.
            // The capacity is the length times 2 (for the carets),
            // plus 2 for the square brackets.
            // This is not optimized for multi-character glyphs, like emojis

            var bindingBuilder = new StringBuilder(columnName.Length * 2 + 2);

            bindingBuilder.Append('[');
            foreach (var c in columnName)
            {
                bindingBuilder.Append('^');
                bindingBuilder.Append(c);
            }
            bindingBuilder.Append(']');

            e.Column = new DataGridTextColumn
            {
                Binding = new Binding(bindingBuilder.ToString()),
                Header = e.Column.Header,
            }; 

            #endregion

        }
    }
}
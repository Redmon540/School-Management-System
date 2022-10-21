using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace School_Manager
{
    public class CreateCustomIDCardsViewModel : BaseViewModel
    {
        #region Constructor

        public CreateCustomIDCardsViewModel()
        {
            //set properties
            FrontCardItems = new ObservableCollection<DesignerControls>();
            BackCardItems = new ObservableCollection<DesignerControls>();
            ColumnsList = new ObservableCollection<string>();
            var list = DataAccess.GetClassNames();
            list.Insert(0, "All");
            Class = new ListEntity { FeildName = "Class", Items = list};

            //set commands
            SelectFrontCardBackground = new RelayCommand(SelectFrontBackground);
            SelectBackCardBackground = new RelayCommand(SelectBackBackground);
            SelectionChangeCommand = new RelayParameterizedCommand(parameter => SelectionChanged(parameter));
            TextStyleCommand = new RelayParameterizedCommand(parameter => TextStyle(parameter));
            DeleteCommand = new RelayParameterizedCommand(parameter => DeleteItem(parameter));
            CreateCardCommand = new RelayCommand(CreateCards);
            IDTypeChangedCommand = new RelayCommand(IDTypeChanged);
            OptionsChangedCommand = new RelayCommand(OptionsChanged);
            DeleteCardCommand = new RelayCommand(DeleteCard);
            SaveCardCommand = new RelayCommand(SaveCard);
            CardChangedCommand = new RelayCommand(CardChanged);
        }

        #endregion

        #region Private


        private int? _SelectedAlignment;

        private double? _SelectedFontSize;

        private Color? _SelectedForeground;

        #endregion

        #region Properties

        public string InfoText { get; set; }

        public bool IsDialogOpen { get; set; }

        public TextEntity ID { get; set; }

        public bool IsIDVisible { get; set; } = false;

        public ObservableCollection<string> ColumnsList { get; set; }

        public ListEntity IDType { get; set; } = new ListEntity { FeildName = "ID Type", Items = new List<string> { "Student", "Teacher" }, Value = "Student", ValidationType = ValidationType.NotEmpty};

        public ListEntity Options { get; set; } = new ListEntity { FeildName = "Options", Value = "All Students" , ValidationType = ValidationType.NotEmpty};

        public ObservableCollection<DesignerControls> FrontCardItems { get; set; }

        public ObservableCollection<DesignerControls> BackCardItems { get; set; }

        public BitmapImage CardFrontBackground { get; set; }

        public BitmapImage CardBackBackground { get; set; }

        public TextEntity CardWidth { get; set; } = new TextEntity { FeildName = "Width", Value = "3.5", ValidationType = ValidationType.Decimal };

        public TextEntity CardHeight { get; set; } = new TextEntity { FeildName = "Height", Value = "2", ValidationType = ValidationType.Decimal };

        public object SelectedTextAlignment { get; set; }

        public bool IsBackCardVisible { get; set; }

        public List<double> FontSizes { get; set; } = new List<double> { 6, 8, 10, 12, 14, 16, 18, 20, 22, 24, 26, 28, 30 };

        public double? SelectedFontSize
        {
            get => _SelectedFontSize;
            set
            {
                if (_SelectedFontSize != value)
                {
                    _SelectedFontSize = value;
                    foreach (DesignerControls control in FrontCardItems)
                    {
                        if (control.IsSelected)
                        {
                            TextControl item = control as TextControl;
                            if (item != null)
                            {
                                item.FontSize = _SelectedFontSize ?? 0;
                                break;
                            }
                        }
                    }
                    foreach (DesignerControls control in BackCardItems)
                    {
                        if (control.IsSelected)
                        {
                            TextControl item = control as TextControl;
                            if (item != null)
                            {
                                item.FontSize = _SelectedFontSize ?? 0;
                                break;
                            }
                        }
                    }
                }
            }
        }

        public bool IsBoldSelected { get; set; }

        public bool IsItalicSelected { get; set; }

        public bool IsUnderlineSelected { get; set; }

        public int? SelectedAligment
        {
            get => _SelectedAlignment;
            set
            {
                if (_SelectedAlignment != value)
                {
                    _SelectedAlignment = value;
                    if (_SelectedAlignment.ToString().IsNotNullOrEmpty())
                    {
                        var sender = int.Parse(_SelectedAlignment.ToString());
                        if (sender == 0)
                        {
                            foreach (DesignerControls control in FrontCardItems)
                            {
                                if (control.IsSelected)
                                {
                                    TextControl item = control as TextControl;
                                    if (item != null)
                                    {
                                        item.TextAlignment = TextAlignment.Left;
                                        break;
                                    }
                                }
                            }
                            foreach (DesignerControls control in BackCardItems)
                            {
                                if (control.IsSelected)
                                {
                                    TextControl item = control as TextControl;
                                    if (item != null)
                                    {
                                        item.TextAlignment = TextAlignment.Left;
                                        break;
                                    }
                                }
                            }
                        }
                        else if (sender == 1)
                        {
                            foreach (DesignerControls control in FrontCardItems)
                            {
                                if (control.IsSelected)
                                {
                                    TextControl item = control as TextControl;
                                    if (item != null)
                                    {
                                        item.TextAlignment = TextAlignment.Right;
                                        break;
                                    }
                                }
                            }

                            foreach (DesignerControls control in BackCardItems)
                            {
                                if (control.IsSelected)
                                {
                                    TextControl item = control as TextControl;
                                    if (item != null)
                                    {
                                        item.TextAlignment = TextAlignment.Right;
                                        break;
                                    }
                                }
                            }
                        }
                        else if (sender == 2)
                        {
                            foreach (DesignerControls control in FrontCardItems)
                            {
                                if (control.IsSelected)
                                {
                                    TextControl item = control as TextControl;
                                    if (item != null)
                                    {
                                        item.TextAlignment = TextAlignment.Center;
                                        break;
                                    }
                                }
                            }

                            foreach (DesignerControls control in BackCardItems)
                            {
                                if (control.IsSelected)
                                {
                                    TextControl item = control as TextControl;
                                    if (item != null)
                                    {
                                        item.TextAlignment = TextAlignment.Center;
                                        break;
                                    }
                                }
                            }
                        }
                        else if (sender == 3)
                        {
                            foreach (DesignerControls control in FrontCardItems)
                            {
                                if (control.IsSelected)
                                {
                                    TextControl item = control as TextControl;
                                    if (item != null)
                                    {
                                        item.TextAlignment = TextAlignment.Justify;
                                        break;
                                    }
                                }
                            }

                            foreach (DesignerControls control in BackCardItems)
                            {
                                if (control.IsSelected)
                                {
                                    TextControl item = control as TextControl;
                                    if (item != null)
                                    {
                                        item.TextAlignment = TextAlignment.Justify;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public Color? SelectedForeground
        {
            get => _SelectedForeground;
            set
            {
                if (_SelectedForeground != value)
                {
                    _SelectedForeground = value;
                    if (_SelectedForeground != null)
                    {
                        foreach (DesignerControls control in FrontCardItems)
                        {
                            if (control.IsSelected)
                            {
                                TextControl item = control as TextControl;
                                if (item != null)
                                {
                                    item.Foreground = new SolidColorBrush((Color)_SelectedForeground);
                                    break;
                                }
                            }
                        }
                        foreach (DesignerControls control in BackCardItems)
                        {
                            if (control.IsSelected)
                            {
                                TextControl item = control as TextControl;
                                if (item != null)
                                {
                                    item.Foreground = new SolidColorBrush((Color)_SelectedForeground);
                                    break;
                                }
                            }
                        }

                    }
                }

            }
        }

        public ListEntity Cards { get; set; } = new ListEntity { FeildName = "Cards", ValidationType = ValidationType.None , Items = DataAccess.GetCards() };

        public TextEntity CardName { get; set; } = new TextEntity { FeildName = "Enter Card Name" };

        public ListEntity Class { get; set; }

        public bool ClassVisibility { get; set; }

        #endregion

        #region Commands

        public ICommand SelectFrontCardBackground { get; set; }

        public ICommand SelectBackCardBackground { get; set; }

        public ICommand SelectionChangeCommand { get; set; }

        public ICommand TextStyleCommand { get; set; }

        public ICommand DeleteCommand { get; set; }

        public ICommand CreateCardCommand { get; set; }

        public ICommand IDTypeChangedCommand { get; set; }

        public ICommand OptionsChangedCommand { get; set; }

        public ICommand DeleteCardCommand { get; set; }

        public ICommand SaveCardCommand { get; set; }

        public ICommand CardChangedCommand { get; set; }

        #endregion

        #region Command Methods

        private void IDTypeChanged()
        {
            try
            {
                if (IDType.Value == "Student")
                {
                    InfoText = "Student Information";
                    List<string> list = new List<string>();
                    list.Add("QR Code");
                    list.Add(DataAccess.GetStudentID());
                    list.AddRange(DataAccess.GetDataTable("SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Students'").AsEnumerable().Select(x => x[0].ToString()).Where(x => !x.Contains("_")).ToList());
                    list.AddRange(DataAccess.GetDataTable("SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Parents'").AsEnumerable().Select(x => x[0].ToString()).Where(x => !x.Contains("_")).ToList());
                    list.Add("Class");
                    list.Add("Section");
                    ColumnsList = new ObservableCollection<string>(list);
                    Options.Items = new List<string> { "All Students", "Single Student" };
                    ID = new TextEntity { FeildName = DataAccess.GetStudentID(), ValidationType = ValidationType.NumericWithNonEmpty };
                    FrontCardItems = new ObservableCollection<DesignerControls>();
                    BackCardItems = new ObservableCollection<DesignerControls>();
                    Class.ValidationType = ValidationType.NotEmpty;
                    ClassVisibility = true;
                }
                else
                {
                    InfoText = "Teacher Information";
                    List<string> list = new List<string>();
                    list.Add("QR Code");
                    list.Add(DataAccess.GetTeacherID());
                    list.AddRange(DataAccess.GetDataTable("SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Teachers'").AsEnumerable().Select(x => x[0].ToString()).Where(x => !x.Contains("_")).ToList());
                    ColumnsList = new ObservableCollection<string>(list);
                    Options.Items = new List<string> { "All Teachers", "Single Teacher" };
                    ID = new TextEntity { FeildName = DataAccess.GetTeacherID(), ValidationType = ValidationType.NumericWithNonEmpty };
                    FrontCardItems = new ObservableCollection<DesignerControls>();
                    BackCardItems = new ObservableCollection<DesignerControls>();
                    Class.ValidationType = ValidationType.None;
                    ClassVisibility = false;
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex, true);
            }
        }

        private void OptionsChanged()
        {
            try
            {
                if (Options.Value == "Single Student" || Options.Value == "Single Teacher")
                    IsIDVisible = true;
                else
                    IsIDVisible = false;
            }
            catch (Exception ex)
            {
                Logger.Log(ex, true);
            }
        }

        private void TextStyle(object sender)
        {
            try
            {
                if (sender == App.Current.MainWindow.FindResource("BoldTextIcon"))
                {
                    foreach (DesignerControls control in FrontCardItems)
                    {
                        if (control.IsSelected)
                        {
                            var item = control as TextControl;
                            if (item != null)
                            {
                                if (IsBoldSelected)
                                {
                                    item.FontWeight = FontWeights.SemiBold;
                                    IsBoldSelected = !IsBoldSelected;
                                    break;
                                }
                                else
                                {
                                    item.FontWeight = FontWeights.Bold;
                                    IsBoldSelected = !IsBoldSelected;
                                    break;
                                }
                            }
                        }
                    }
                    foreach (DesignerControls control in BackCardItems)
                    {
                        if (control.IsSelected)
                        {
                            var item = control as TextControl;
                            if (item != null)
                            {
                                if (IsBoldSelected)
                                {
                                    item.FontWeight = FontWeights.Normal;
                                    IsBoldSelected = !IsBoldSelected;
                                    break;
                                }
                                else
                                {
                                    item.FontWeight = FontWeights.Bold;
                                    IsBoldSelected = !IsBoldSelected;
                                    break;
                                }
                            }
                        }
                    }
                }
                else if (sender == App.Current.MainWindow.FindResource("ItalicTextIcon"))
                {
                    foreach (DesignerControls control in FrontCardItems)
                    {
                        if (control.IsSelected)
                        {
                            var item = control as TextControl;
                            if (item != null)
                            {
                                if (IsItalicSelected)
                                {
                                    item.FontStyle = FontStyles.Normal;
                                    IsItalicSelected = !IsItalicSelected;
                                    break;
                                }
                                else
                                {
                                    item.FontStyle = FontStyles.Italic;
                                    IsItalicSelected = !IsItalicSelected;
                                    break;
                                }
                            }
                        }
                    }
                    foreach (DesignerControls control in BackCardItems)
                    {
                        if (control.IsSelected)
                        {
                            var item = control as TextControl;
                            if (item != null)
                            {
                                if (IsItalicSelected)
                                {
                                    item.FontStyle = FontStyles.Normal;
                                    IsItalicSelected = !IsItalicSelected;
                                    break;
                                }
                                else
                                {
                                    item.FontStyle = FontStyles.Italic;
                                    IsItalicSelected = !IsItalicSelected;
                                    break;
                                }
                            }
                        }
                    }
                }
                else if (sender == App.Current.MainWindow.FindResource("UnderlineTextIcon"))
                {
                    foreach (DesignerControls control in FrontCardItems)
                    {
                        if (control.IsSelected)
                        {
                            var item = control as TextControl;
                            if (item != null)
                            {
                                if (IsUnderlineSelected)
                                {
                                    item.TextDecoration = null;
                                    IsUnderlineSelected = !IsUnderlineSelected;
                                    break;
                                }
                                else
                                {
                                    item.TextDecoration = TextDecorations.Underline;
                                    IsUnderlineSelected = !IsUnderlineSelected;
                                    break;
                                }
                            }
                        }
                    }
                    foreach (DesignerControls control in BackCardItems)
                    {
                        if (control.IsSelected)
                        {
                            var item = control as TextControl;
                            if (item != null)
                            {
                                if (IsUnderlineSelected)
                                {
                                    item.TextDecoration = null;
                                    IsUnderlineSelected = !IsUnderlineSelected;
                                    break;
                                }
                                else
                                {
                                    item.TextDecoration = TextDecorations.Underline;
                                    IsUnderlineSelected = !IsUnderlineSelected;
                                    break;
                                }
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

        private void SelectFrontBackground()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.ShowDialog();
            if (!string.IsNullOrEmpty(openFileDialog.FileName))
            {
                CardFrontBackground = new BitmapImage(new Uri(openFileDialog.FileName));
            }
        }

        private void SelectBackBackground()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.ShowDialog();
            if (!string.IsNullOrEmpty(openFileDialog.FileName))
            {
                CardBackBackground = new BitmapImage(new Uri(openFileDialog.FileName));
            }
            IsBackCardVisible = true;
        }

        private void SelectionChanged(object sender)
        {
            try
            {
                var item = sender as TextControl;
                if (item != null)
                {

                    item.IsHitTestVisible = false;
                    item.IsSelected = true;
                    foreach (DesignerControls control in FrontCardItems)
                    {
                        if (control != item)
                        {
                            control.IsSelected = false;
                            control.IsHitTestVisible = true;
                        }
                    }
                    foreach (DesignerControls control in BackCardItems)
                    {
                        if (control != item)
                        {
                            control.IsSelected = false;
                            control.IsHitTestVisible = true;
                        }
                    }
                    SelectedFontSize = item.FontSize;
                    SelectedAligment = (int)item.TextAlignment;
                    SelectedForeground = (item.Foreground as SolidColorBrush).Color;
                    IsBoldSelected = item.FontWeight == FontWeights.Bold ? true : false;
                    IsUnderlineSelected = item.TextDecoration == null ? false : true;
                    IsItalicSelected = item.FontStyle == FontStyles.Italic ? true : false;
                }
                else
                {
                    var item2 = sender as ImageControl;
                    if (item2 != null)
                    {
                        item2.IsHitTestVisible = false;
                        item2.IsSelected = true;
                        foreach (DesignerControls control in FrontCardItems)
                        {
                            if (control != item2)
                            {
                                control.IsSelected = false;
                                control.IsHitTestVisible = true;
                            }
                        }
                        foreach (DesignerControls control in BackCardItems)
                        {
                            if (control != item2)
                            {
                                control.IsSelected = false;
                                control.IsHitTestVisible = true;
                            }
                        }
                        SelectedAligment = null;
                        SelectedFontSize = null;
                        SelectedForeground = null;
                        IsBoldSelected = false;
                        IsUnderlineSelected = false;
                        IsItalicSelected = false;
                    }
                    else
                    {
                        var item3 = sender as QRCodeControl;
                        if (item3 != null)
                        {
                            item3.IsHitTestVisible = false;
                            item3.IsSelected = true;
                            foreach (DesignerControls control in FrontCardItems)
                            {
                                if (control != item3)
                                {
                                    control.IsSelected = false;
                                    control.IsHitTestVisible = true;
                                }
                            }
                            foreach (DesignerControls control in BackCardItems)
                            {
                                if (control != item3)
                                {
                                    control.IsSelected = false;
                                    control.IsHitTestVisible = true;
                                }
                            }
                            SelectedAligment = null;
                            SelectedFontSize = null;
                            SelectedForeground = null;
                            IsBoldSelected = false;
                            IsUnderlineSelected = false;
                            IsItalicSelected = false;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Logger.Log(ex, true);
            }
        }

        private void DeleteItem(object sender)
        {
            try
            {
                foreach (var item in FrontCardItems)
                {
                    if (item.IsSelected)
                    {
                        FrontCardItems.Remove(item);
                        SelectedAligment = null;
                        SelectedFontSize = null;
                        SelectedForeground = null;
                        IsBoldSelected = false;
                        IsUnderlineSelected = false;
                        IsItalicSelected = false;
                        return;
                    }
                }
                foreach (var item in BackCardItems)
                {
                    if (item.IsSelected)
                    {
                        BackCardItems.Remove(item);
                        SelectedAligment = null;
                        SelectedFontSize = null;
                        SelectedForeground = null;
                        IsBoldSelected = false;
                        IsUnderlineSelected = false;
                        IsItalicSelected = false;
                        return;
                    }
                }
            }
            catch(Exception ex)
            {
                Logger.Log(ex, true);
            }
        }

        private void CardChanged()
        {
            try
            {
                using (new WaitCursor())
                {
                    if (Cards.Value.IsNotNullOrEmpty())
                    {
                        var card = DataAccess.GetCardTemplate(Cards.Value);
                        FrontCardItems = card.FrontItems;
                        BackCardItems = card.BackItems;
                        CardFrontBackground = card.FrontImage;
                        CardBackBackground = card.BackImage;
                        CardWidth.Value = card.Width;
                        CardHeight.Value = card.Height;
                        IsBackCardVisible = card.IsBackAdded;
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.Log(ex, true);
            }
        }

        private void SaveCard()
        {
            try
            {
                using (new WaitCursor())
                {
                    CardName.ValidationType = ValidationType.NotEmpty;
                    if (CardName.IsValid)
                    {
                        DataAccess.SaveCardTemplate(
                            new VoucherTemplate
                            {
                                FrontImage = CardFrontBackground,
                                BackImage = CardBackBackground,
                                FrontItems = FrontCardItems,
                                BackItems = BackCardItems,
                                Width = CardWidth.Value,
                                Height = CardHeight.Value,
                                IsBackAdded = IsBackCardVisible
                            },
                        CardName.Value);
                        Cards.Items = DataAccess.GetCards();
                        Cards.Value = CardName.Value;
                        CardName.Value = "";
                        CardName.ValidationType = ValidationType.None;
                    }
                    else
                    {
                        DialogManager.ShowValidationMessage();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex, true);
            }
        }

        private void DeleteCard()
        {
            try
            {
                if (Cards.Value.IsNotNullOrEmpty())
                {
                    if (DialogManager.ShowMessageDialog("Warning", "Are you sure, you want to delete the selected voucher template?", true))
                    {
                        var h = CardHeight.Value;
                        var w = CardWidth.Value;
                        DataAccess.ExecuteQuery($"DELETE FROM Designer WHERE Designer_Name = '{Cards.Value}' AND [Designer_Type] = 2");
                        FrontCardItems = new ObservableCollection<DesignerControls>();
                        BackCardItems = new ObservableCollection<DesignerControls>();
                        CardFrontBackground = null;
                        CardBackBackground = null;
                        Cards.Items = DataAccess.GetCards();
                        CardHeight.Value = h;
                        CardWidth.Value = w;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex, true);
            }
        }

        private async void CreateCards()
        {
            try
            {
                bool isValid = true;
                if (!IDType.IsValid)
                    isValid = false;
                if (!Options.IsValid)
                    isValid = false;
                if (!Class.IsValid)
                    isValid = false;
                if (IsIDVisible && !ID.IsValid)
                    isValid = false;
                if (!isValid)
                {
                    DialogManager.ShowValidationMessage();
                    return;
                }

                using (new WaitCursor())
                {

                    var saveFileDialog = new SaveFileDialog();
                    saveFileDialog.FileName = $"{Class.Value} - ID Cards.xps";
                    saveFileDialog.Filter = "XPS File (*.xps)|*.xps|All files (*.*)|*.*";
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        IsDialogOpen = true;
                        await Task.Delay(500);
                        if (IDType.Value == "Student")
                        {
                            if (Options.Value == "All Students")
                                DataAccess.CreateStudentIDCards(FrontCardItems, BackCardItems,
                                    CardFrontBackground, CardBackBackground, double.Parse(CardWidth.Value), 
                                    double.Parse(CardHeight.Value), null, Class.Value == "All" ? null : DataAccess.GetClassID(Class.Value),
                                    saveFileDialog.FileName, IsBackCardVisible);
                            else if (Options.Value == "Single Student")
                            {
                                if (DataAccess.GetDataTable($"SELECT * FROM Students WHERE Student_ID = {ID.Value} ").Rows.Count == 0)
                                    DialogManager.ShowMessageDialog("Warning", $"{DataAccess.GetStudentID()} not found.",DialogTitleColor.Red);
                                else
                                    DataAccess.CreateStudentIDCards(FrontCardItems, BackCardItems,
                                    CardFrontBackground, CardBackBackground, double.Parse(CardWidth.Value), double.Parse(CardHeight.Value), ID.Value, null,
                                    saveFileDialog.FileName, IsBackCardVisible);
                            }
                        }
                        else
                        {
                            if (Options.Value == "All Teachers")
                                DataAccess.CreateTeacherIDCards(FrontCardItems, BackCardItems,
                                    CardFrontBackground, CardBackBackground, double.Parse(CardWidth.Value), double.Parse(CardHeight.Value), null,
                                    saveFileDialog.FileName, IsBackCardVisible);
                            else if (Options.Value == "Single Teacher")
                            {
                                if (DataAccess.GetDataTable($"SELECT * FROM Teachers WHERE Teacher_ID = {ID.Value} ").Rows.Count == 0)
                                    DialogManager.ShowMessageDialog("Warning", $"{DataAccess.GetStudentID()} not found.",DialogTitleColor.Red);
                                else
                                    DataAccess.CreateTeacherIDCards(FrontCardItems, BackCardItems,
                                    CardFrontBackground, CardBackBackground, double.Parse(CardWidth.Value), double.Parse(CardHeight.Value), ID.Value,
                                    saveFileDialog.FileName, IsBackCardVisible);
                            }
                        }
                        IsDialogOpen = false;
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
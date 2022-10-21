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
    public class CreateFeeVoucherViewModel : BaseViewModel
    {
        #region Constructor

        public CreateFeeVoucherViewModel()
        {
            //set properties
            FrontCardItems = new ObservableCollection<DesignerControls>();
            BackCardItems = new ObservableCollection<DesignerControls>();

            List<string> list = new List<string>();
            list.Add("QR Code");
            list.Add(DataAccess.GetStudentID());
            list.AddRange(DataAccess.GetDataTable("SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Students'").AsEnumerable().Select(x => x[0].ToString()).Where(x => !x.Contains("_")).ToList());
            list.AddRange(DataAccess.GetDataTable("SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Parents'").AsEnumerable().Select(x => x[0].ToString()).Where(x => !x.Contains("_")).ToList());
            list.Add("Class");
            list.Add("Section");
            ColumnsList = new ObservableCollection<string>(list);

            //set commands
            SelectFrontCardBackground = new RelayCommand(SelectFrontBackground);
            SelectBackCardBackground = new RelayCommand(SelectBackBackground);
            SelectionChangeCommand = new RelayParameterizedCommand(parameter => SelectionChanged(parameter));
            TextStyleCommand = new RelayParameterizedCommand(parameter => TextStyle(parameter));
            DeleteCommand = new RelayCommand(DeleteItem);
            CreateCardCommand = new RelayCommand(CreateVouchers);
            GetClassFeeStructureCommand = new RelayCommand(GetClassFeeStructure);
            OptionsChangedCommand = new RelayCommand(OptionsChanged);
            SaveVoucherCommand = new RelayCommand(SaveVoucher);
            DeleteVoucherCommand = new RelayCommand(DeleteVoucher);
            VoucherChangedCommand = new RelayCommand(VoucherChanged);
        }

        #endregion

        #region Private


        private int? _SelectedAlignment;

        private double? _SelectedFontSize;

        private Color? _SelectedForeground;

        #endregion

        #region Properties

        public TextEntity ID { get; set; } = new TextEntity { FeildName = DataAccess.GetStudentID() ,ValidationType = ValidationType.NotEmpty };

        public bool IsIDVisible { get; set; } = false;

        public ObservableCollection<string> ColumnsList { get; set; }

        public ListEntity Class { get; set; } = new ListEntity { FeildName = "Class", Items = DataAccess.GetClassNames() , ValidationType = ValidationType.NotEmpty };

        public ListEntity Months { get; set; } = new ListEntity { FeildName = "Month", Items = Helper.GetMonths() , Value = Helper.GetMonthName(DateTime.Now.Month) , ValidationType = ValidationType.NotEmpty };

        public ListEntity Years { get; set; } = new ListEntity { FeildName = "Year", Items = Helper.GetYears() , Value = DateTime.Now.Year.ToString() , ValidationType = ValidationType.NotEmpty };

        public ListEntity Options { get; set; } = new ListEntity { FeildName = "Options", Items = new List<string> { "Single Student" , "Current Class"} , Value = "Current Class" , ValidationType = ValidationType.NotEmpty };

        public ListEntity Vouchers { get; set; } = new ListEntity { FeildName = "Vouchers", ValidationType = ValidationType.None };

        public ObservableCollection<DesignerControls> FrontCardItems { get; set; }

        public ObservableCollection<DesignerControls> BackCardItems { get; set; }

        public ObservableCollection<FeeEntity> FeeStructure { get; set; }

        public BitmapImage CardFrontBackground { get; set; }

        public BitmapImage CardBackBackground { get; set; }

        public TextEntity CardWidth { get; set; } = new TextEntity { FeildName = "Width", Value = "8.5", ValidationType = ValidationType.Decimal };

        public TextEntity CardHeight { get; set; } = new TextEntity { FeildName = "Height", Value = "11", ValidationType = ValidationType.Decimal };

        public TextEntity VoucherName { get; set; } = new TextEntity { FeildName = "Enter Voucher Name" };

        public object SelectedTextAlignment { get; set; }

        public bool IsBackCardVisible { get; set; }

        public List<double> FontSizes { get; set; } = Helper.GetFontSizes();

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
                            var item = control as TextControl;
                            if (item != null)
                            {
                                item.FontSize = _SelectedFontSize ?? 0;
                                break;
                            }
                            else
                            {
                                var fee = control as FeeControl;
                                if (fee != null)
                                {
                                    fee.FontSize = _SelectedFontSize ?? 0;
                                    break;
                                }
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
                        else
                        {
                            var fee = control as FeeControl;
                            if (fee != null)
                            {
                                fee.FontSize = _SelectedFontSize ?? 0;
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
                                    var item = control as TextControl;
                                    if (item != null)
                                    {
                                        item.TextAlignment = TextAlignment.Left;
                                        break;
                                    }
                                    else
                                    {
                                        var fee = control as FeeControl;
                                        if (fee != null)
                                        {
                                            fee.TextAlignment = TextAlignment.Left;
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
                                        item.TextAlignment = TextAlignment.Left;
                                        break;
                                    }
                                    else
                                    {
                                        var fee = control as FeeControl;
                                        if (fee != null)
                                        {
                                            fee.TextAlignment = TextAlignment.Left;
                                            break;
                                        }
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
                                    var item = control as TextControl;
                                    if (item != null)
                                    {
                                        item.TextAlignment = TextAlignment.Right;
                                        break;
                                    }
                                    else
                                    {
                                        var fee = control as FeeControl;
                                        if (fee != null)
                                        {
                                            fee.TextAlignment = TextAlignment.Right;
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
                                        item.TextAlignment = TextAlignment.Right;
                                        break;
                                    }
                                }
                                else
                                {
                                    var fee = control as FeeControl;
                                    if (fee != null)
                                    {
                                        fee.TextAlignment = TextAlignment.Right;
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
                                    var item = control as TextControl;
                                    if (item != null)
                                    {
                                        item.TextAlignment = TextAlignment.Center;
                                        break;
                                    }
                                    else
                                    {
                                        var fee = control as FeeControl;
                                        if (fee != null)
                                        {
                                            fee.TextAlignment = TextAlignment.Center;
                                            break;
                                        }
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
                                else
                                {
                                    var fee = control as FeeControl;
                                    if (fee != null)
                                    {
                                        fee.TextAlignment = TextAlignment.Center;
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
                                    var item = control as TextControl;
                                    if (item != null)
                                    {
                                        item.TextAlignment = TextAlignment.Justify;
                                        break;
                                    }
                                }
                                else
                                {
                                    var fee = control as FeeControl;
                                    if (fee != null)
                                    {
                                        fee.TextAlignment = TextAlignment.Justify;
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
                                else
                                {
                                    var fee = control as FeeControl;
                                    if (fee != null)
                                    {
                                        fee.TextAlignment = TextAlignment.Justify;
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
                                var item = control as TextControl;
                                if (item != null)
                                {
                                    item.Foreground = new SolidColorBrush((Color)_SelectedForeground);
                                    break;
                                }
                                else
                                {
                                    var fee = control as FeeControl;
                                    if (fee != null)
                                    {
                                        fee.Foreground = new SolidColorBrush((Color)_SelectedForeground);
                                        break;
                                    }
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
                            else
                            {
                                var fee = control as FeeControl;
                                if (fee != null)
                                {
                                    fee.Foreground = new SolidColorBrush((Color)_SelectedForeground);
                                    break;
                                }
                            }
                        }

                    }
                }

            }
        }

        public bool IsStudentInfoExpanded { get; set; }

        public bool IsFeeExpanded { get; set; }

        public bool IsEditingExpanded { get; set; }

        public FeeEntity FeeEntity { get; set; } = new FeeEntity { Background = new SolidColorBrush(Colors.Red) };

        public bool IsDialogOpen { get; set; } = false;

        #endregion

        #region Commands

        public ICommand SelectFrontCardBackground { get; set; }

        public ICommand SelectBackCardBackground { get; set; }

        public ICommand SelectionChangeCommand { get; set; }

        public ICommand TextStyleCommand { get; set; }

        public ICommand DeleteCommand { get; set; }

        public ICommand CreateCardCommand { get; set; }

        public ICommand GetClassFeeStructureCommand { get; set; }

        public ICommand OptionsChangedCommand { get; set; }

        public ICommand SaveVoucherCommand { get; set; }

        public ICommand DeleteVoucherCommand { get; set; }

        public ICommand VoucherChangedCommand { get; set; }

        #endregion

        #region Command Methods

        private void GetClassFeeStructure()
        {
            try
            {
                if (Class.Value.IsNotNullOrEmpty() && Months.Value.IsNotNullOrEmpty() && Years.Value.IsNotNullOrEmpty())
                {
                    Vouchers.Items = DataAccess.GetVouchers();
                    IsFeeExpanded = true;
                    IsStudentInfoExpanded = true;
                    //to populate fee structure
                    FeeStructure = new ObservableCollection<FeeEntity>(
                        DataAccess.GetDataTable($"SELECT DISTINCT Fee_Record.Fee FROM Fee_Record " +
                        $"LEFT JOIN Students ON Students.Student_ID = Fee_Record.Student_ID " +
                        $"WHERE Class_ID = {DataAccess.GetClassID(Class.Value)} " +
                        $"AND [Month] = {Helper.GetMonthNumber(Months.Value)} AND [Year] = {Years.Value}")
                        .AsEnumerable().Select(x => 
                        new FeeEntity
                        {
                            Fee = x["Fee"].ToString(),
                            Background = Helper.GetRandomColor(),
                            Month = Helper.GetMonthNumber(Months.Value).ToString(),
                            Year = Years.Value
                        }).ToList()
                        );

                    FeeEntity = new FeeEntity
                    {
                        Background = new SolidColorBrush(Colors.Red),
                        Month = Helper.GetMonthNumber(Months.Value).ToString(),
                        Year = Years.Value
                    };
                    //to recolor the fee items in voucher
                    foreach (var feeStructure in FeeStructure)
                    {
                        foreach (var item in FrontCardItems)
                        {
                            var fee = item as FeeControl;
                            if (fee != null)
                            {
                                if(fee.Fee == feeStructure.Fee)
                                {
                                    fee.Month = feeStructure.Month;
                                    fee.Year = feeStructure.Year;
                                    fee.Background = feeStructure.Background;
                                }
                                else
                                {
                                    fee.Month = feeStructure.Month;
                                    fee.Year = feeStructure.Year;
                                }
                            }
                        }
                        foreach (var item in BackCardItems)
                        {
                            var fee = item as FeeControl;
                            if (fee != null)
                            {
                                if (fee.Fee == feeStructure.Fee)
                                {
                                    fee.Month = feeStructure.Month;
                                    fee.Year = feeStructure.Year;
                                    fee.Background = feeStructure.Background;
                                }
                                else
                                {
                                    fee.Month = feeStructure.Month;
                                    fee.Year = feeStructure.Year;
                                }
                            }
                        }
                    }
                    
                    ID = new TextEntity { FeildName = DataAccess.GetStudentID(), ValidationType = ValidationType.NumericWithNonEmpty };
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
                if (Options.Value == "Single Student")
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
                            else
                            {
                                var fee = control as FeeControl;
                                if (fee != null)
                                {
                                    if (IsBoldSelected)
                                    {
                                        fee.FontWeight = FontWeights.SemiBold;
                                        IsBoldSelected = !IsBoldSelected;
                                        break;
                                    }
                                    else
                                    {
                                        fee.FontWeight = FontWeights.Bold;
                                        IsBoldSelected = !IsBoldSelected;
                                        break;
                                    }
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
                            else
                            {
                                var fee = control as FeeControl;
                                if (fee != null)
                                {
                                    if (IsBoldSelected)
                                    {
                                        fee.FontWeight = FontWeights.SemiBold;
                                        IsBoldSelected = !IsBoldSelected;
                                        break;
                                    }
                                    else
                                    {
                                        fee.FontWeight = FontWeights.Bold;
                                        IsBoldSelected = !IsBoldSelected;
                                        break;
                                    }
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
                            else
                            {
                                var fee = control as FeeControl;
                                if (fee != null)
                                {
                                    if (IsItalicSelected)
                                    {
                                        fee.FontStyle = FontStyles.Normal;
                                        IsItalicSelected = !IsItalicSelected;
                                        break;
                                    }
                                    else
                                    {
                                        fee.FontStyle = FontStyles.Italic;
                                        IsItalicSelected = !IsItalicSelected;
                                        break;
                                    }
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
                            else
                            {
                                var fee = control as FeeControl;
                                if (fee != null)
                                {
                                    if (IsItalicSelected)
                                    {
                                        fee.FontStyle = FontStyles.Normal;
                                        IsItalicSelected = !IsItalicSelected;
                                        break;
                                    }
                                    else
                                    {
                                        fee.FontStyle = FontStyles.Italic;
                                        IsItalicSelected = !IsItalicSelected;
                                        break;
                                    }
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
                            else
                            {
                                var fee = control as FeeControl;
                                if (fee != null)
                                {
                                    if (IsUnderlineSelected)
                                    {
                                        fee.TextDecoration = null;
                                        IsUnderlineSelected = !IsUnderlineSelected;
                                        break;
                                    }
                                    else
                                    {
                                        fee.TextDecoration = TextDecorations.Underline;
                                        IsUnderlineSelected = !IsUnderlineSelected;
                                        break;
                                    }
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
                            else
                            {
                                var fee = control as FeeControl;
                                if (fee != null)
                                {
                                    if (IsUnderlineSelected)
                                    {
                                        fee.TextDecoration = null;
                                        IsUnderlineSelected = !IsUnderlineSelected;
                                        break;
                                    }
                                    else
                                    {
                                        fee.TextDecoration = TextDecorations.Underline;
                                        IsUnderlineSelected = !IsUnderlineSelected;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
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
                    var item4 = sender as FeeControl;
                    if(item4 != null)
                    {
                        item4.IsHitTestVisible = false;
                        item4.IsSelected = true;
                        foreach (DesignerControls control in FrontCardItems)
                        {
                            if (control != item4)
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
                        SelectedFontSize = item4.FontSize;
                        SelectedAligment = (int)item4.TextAlignment;
                        SelectedForeground = (item4.Foreground as SolidColorBrush).Color;
                        IsBoldSelected = item4.FontWeight == FontWeights.Bold ? true : false;
                        IsUnderlineSelected = item4.TextDecoration == null ? false : true;
                        IsItalicSelected = item4.FontStyle == FontStyles.Italic ? true : false;
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
            }
            catch (Exception ex)
            {
                Logger.Log(ex, true);
            }
        }

        private void DeleteItem()
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
            catch (Exception ex)
            {
                Logger.Log(ex, true);
            }
        }

        private void VoucherChanged()
        {
            try
            {
                using (new WaitCursor())
                {
                    if (Vouchers.Value.IsNotNullOrEmpty())
                    {
                        var voucher = DataAccess.GetsVoucherTemplate(Vouchers.Value);
                        FrontCardItems = voucher.FrontItems;
                        BackCardItems = voucher.BackItems;
                        CardFrontBackground = voucher.FrontImage;
                        CardBackBackground = voucher.BackImage;
                        CardWidth.Value = voucher.Width;
                        CardHeight.Value = voucher.Height;
                        IsBackCardVisible = voucher.IsBackAdded;

                        foreach (var item in FrontCardItems)
                        {
                            var fee = item as FeeControl;
                            if (fee != null)
                            {
                                fee.Month = Helper.GetMonthNumber(Months.Value).ToString();
                                fee.Year = Years.Value;
                            }
                        }
                        foreach (var item in BackCardItems)
                        {
                            var fee = item as FeeControl;
                            if (fee != null)
                            {
                                fee.Month = Helper.GetMonthNumber(Months.Value).ToString();
                                fee.Year = Years.Value;
                            }
                        }

                        //to recolor the fee items in voucher
                        foreach (var feeStructure in FeeStructure)
                        {
                            foreach (var item in FrontCardItems)
                            {
                                var fee = item as FeeControl;
                                if (fee != null)
                                {
                                    if (fee.Fee == feeStructure.Fee && fee.Month == feeStructure.Month && fee.Year == feeStructure.Year)
                                    {
                                        fee.Background = feeStructure.Background;
                                    }
                                }
                            }
                            foreach (var item in BackCardItems)
                            {
                                var fee = item as FeeControl;
                                if (fee != null)
                                {
                                    if (fee.Fee == feeStructure.Fee && fee.Month == feeStructure.Month && fee.Year == feeStructure.Year)
                                    {
                                        fee.Background = feeStructure.Background;
                                    }
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.Log(ex, true);
            }
        }

        private void SaveVoucher()
        {
            try
            {
                using (new WaitCursor())
                {
                    VoucherName.ValidationType = ValidationType.NotEmpty;
                    if (VoucherName.IsValid)
                    {
                        DataAccess.SaveVoucherTemplate(
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
                        VoucherName.Value, DataAccess.GetClassID(Class.Value));
                        Vouchers.Items = DataAccess.GetVouchers();
                        Vouchers.Value = VoucherName.Value;
                        VoucherName.Value = "";
                        VoucherName.ValidationType = ValidationType.None;
                    }
                    else
                    {
                        DialogManager.ShowValidationMessage();
                    }
                }
            }
            catch(Exception ex)
            {
                Logger.Log(ex, true);
            }
        }

        private void DeleteVoucher()
        {
            try
            {
                if (Vouchers.Value.IsNotNullOrEmpty() && Class.Value.IsNotNullOrEmpty())
                {
                    if (DialogManager.ShowMessageDialog("Warning", "Are you sure, you want to delete the selected voucher template?", true))
                    {
                        var h = CardHeight.Value;
                        var w = CardWidth.Value;
                        DataAccess.ExecuteQuery($"DELETE FROM Designer WHERE Designer_Name = '{Vouchers.Value}' AND Class_ID = {DataAccess.GetClassID(Class.Value)}");
                        FrontCardItems = new ObservableCollection<DesignerControls>();
                        BackCardItems = new ObservableCollection<DesignerControls>();
                        CardFrontBackground = null;
                        CardBackBackground = null;
                        Vouchers.Items = DataAccess.GetVouchers();
                        CardHeight.Value = h;
                        CardWidth.Value = w;
                    }
                }
            }
            catch(Exception ex)
            {
                Logger.Log(ex, true);
            }
        }

        private async void CreateVouchers()
        {
            try
            {
                bool isValid = true;
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
                    if(Options.Value == "Current Class")
                    {
                        var saveFileDialog = new SaveFileDialog();
                        saveFileDialog.FileName = $"{Class.Value} Vouchers - {DateTime.Now.Date.ToShortDateString()}.xps";
                        saveFileDialog.Filter = "XPS File (*.xps)|*.xps|All files (*.*)|*.*";
                        if (saveFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            IsDialogOpen = true;
                            await Task.Delay(1000);
                            DataAccess.CreateFeeVoucher(FrontCardItems, BackCardItems,
                                        CardFrontBackground, CardBackBackground, double.Parse(CardWidth.Value), double.Parse(CardHeight.Value), Class.Value,
                                        saveFileDialog.FileName, IsBackCardVisible);
                            IsDialogOpen = false;
                        }
                    }
                    else
                    {
                        if(DataAccess.GetDataTable($"SELECT * FROM Students WHERE Student_ID = {ID.Value} AND Class_ID = {DataAccess.GetClassID(Class.Value)}").Rows.Count == 0)
                        {
                            DialogManager.ShowMessageDialog("Warning", $"Student not found, please enter the correct {DataAccess.GetStudentID()}.",DialogTitleColor.Red);
                            return;
                        }
                        var saveFileDialog = new SaveFileDialog();
                        saveFileDialog.FileName = $"{Class.Value} Vouchers - {DateTime.Now.Date.ToShortDateString()}.xps";
                        saveFileDialog.Filter = "XPS File (*.xps)|*.xps|All files (*.*)|*.*";
                        if (saveFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            IsDialogOpen = true;
                            await Task.Delay(1000);
                            DataAccess.CreateFeeVoucher(FrontCardItems, BackCardItems,
                                        CardFrontBackground, CardBackBackground, double.Parse(CardWidth.Value), double.Parse(CardHeight.Value), Class.Value,
                                        saveFileDialog.FileName, IsBackCardVisible,ID.Value);
                            IsDialogOpen = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex, true);
            }
        }

        #endregion
    }
}

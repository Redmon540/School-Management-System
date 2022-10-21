using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace School_Manager
{
    public class DesignerControls : BasePropertyChanged
    {
        public string Value { get; set; }

        public virtual double Width { get; set; } = 80;

        public virtual double Height { get; set; } = 15;

        public double CanvasTop { get; set; } = 0;

        public double CanvasLeft { get; set; } = 0;

        public bool IsSelected { get; set; } = false;

        public bool IsHitTestVisible { get; set; } = true;
    }
    public class TextControl : DesignerControls
    {
        public TextAlignment TextAlignment { get; set; } = TextAlignment.Left;

        public System.Windows.FontStyle FontStyle { get; set; } = FontStyles.Normal;

        public double FontSize { get; set; } = 12;

        public FontWeight FontWeight { get; set; } = FontWeights.SemiBold;

        public TextDecorationCollection TextDecoration { get; set; } = null;

        public Brush Foreground { get; set; } = new SolidColorBrush(Colors.Black);

        public Brush Background { get; set; } = new SolidColorBrush(Colors.Transparent);

    }
    public class ImageControl : DesignerControls
    {
        public BitmapImage Image { get; set; } = new  BitmapImage(new Uri("pack://application:,,,/Resources/Photo-placeholder.png"));

        public override double Width { get; set; } = 60;

        public override double Height { get; set; } = 70;
    }
    public class QRCodeControl : DesignerControls
    {
        public BitmapImage Image { get; set; } = new BitmapImage(new Uri("pack://application:,,,/Resources/QRCode.png"));

        public override double Width { get; set; } = 70;

        public override double Height { get; set; } = 70;
    }
    public class FeeControl : DesignerControls
    {
        public TextAlignment TextAlignment { get; set; } = TextAlignment.Left;

        public System.Windows.FontStyle FontStyle { get; set; } = FontStyles.Normal;

        public double FontSize { get; set; } = 12;

        public FontWeight FontWeight { get; set; } = FontWeights.SemiBold;

        public TextDecorationCollection TextDecoration { get; set; } = null;

        public Brush Foreground { get; set; } = new SolidColorBrush(Colors.Black);

        public Brush Background { get; set; } = new SolidColorBrush(Colors.Transparent);

        public string Fee { get; set; }

        public string Month { get; set; }

        public string Year { get; set; }

        public FeeAttribite FeeAttribite { get; set; } = FeeAttribite.None;
    }

    public enum FeeAttribite
    {
        None,
        Fee,
        LateFee,
        Discount,
        DueDate,
        Total
    }
}
using System.Windows.Controls;

namespace School_Manager
{
    public class IDCardThumbnailViewModel : BasePropertyChanged
    {
        public UserControl CardFront { get; set; }

        public UserControl CardBack { get; set; }
    }
}

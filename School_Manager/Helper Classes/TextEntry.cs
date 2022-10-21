using System.Windows;

namespace School_Manager
{
    public class TextEntry : BasePropertyChanged
    {
        public string Text { get; set; }

        public string BackupText { get; set; }

        public bool IsEnabled { get; set; }

        public bool IsEditing { get; set; }

        public string EditIcon { get; set; } = Application.Current.FindResource("EditIcon") as string;

        public string DeleteIcon { get; set; } = Application.Current.FindResource("DeleteBinIcon") as string;
    }
}

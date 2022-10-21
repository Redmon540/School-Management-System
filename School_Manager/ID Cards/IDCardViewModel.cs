using System;
using System.Windows.Media.Imaging;

namespace School_Manager
{
    public class IDCardViewModel : BasePropertyChanged
    {
        public string SchoolName { get; set; }

        public BitmapImage SchoolLogo { get; set; } 

        public string IDName { get; set; }

        public string ID { get; set; }

        public string Name { get; set; }

        public string FatherName { get; set; }

        public string Class { get; set; }

        public BitmapImage Photo { get; set; }

        public string Note { get; set; }

        public string TermsAndConditions { get; set; }

        public DateTime SelectedIssueDate { get; set; } = DateTime.Now;

        public string IssueDate
        {
            get => SelectedIssueDate.ToString("dd-MMMM-yyyy");
            set
            {
                if (IssueDate != value)
                    IssueDate = value;
            }
        }

        public DateTime SelectedValidDate { get; set; } = DateTime.Now;

        public string ValidDate
        {
            get => SelectedValidDate.ToString("dd-MMMM-yyyy");
            set
            {
                if (ValidDate != value)
                    ValidDate = value;
            }
        }

        public BitmapImage QRCode { get; set; }
    }
}

using System.Windows.Media.Imaging;

namespace School_Manager
{
    class IDCardDummyViewModel: BaseViewModel
    {
        public string IDName { get; set; } = "Student ID";

        public string ID { get; set; } = "999";

        public string Name { get; set; } = "Student Name Here";

        public string FatherName { get; set; } = "Father Name Here";

        public string Class { get; set; } = "Class-Section";

        public BitmapImage Photo { get; set; }

        public string IssueDate { get; set; } = "Day-Month-Year";

        public string ValidDate { get; set; } = "Day-Month-Year";

        public BitmapImage QRCode { get; set; }
    }
}

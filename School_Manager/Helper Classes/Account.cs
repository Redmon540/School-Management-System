namespace School_Manager
{
    public static class Account
    {
        public static string User { get; set; } = "1";
        public static AccountType AccountType { get; set; }
        public static bool CanViewDashboard { get; set; } = true;
        public static bool CanViewStudentRecord { get; set; } = true;
        public static bool CanAddStudent { get; set; } = true;
        public static bool CanEditStudentRecord { get; set; } = true;
        public static bool CanDeleteStudentRecord { get; set; } = true;
        public static bool CanViewFeeRecord { get; set; } = true;
        public static bool CanEditFeeRecord { get; set; } = true;
        public static bool CanCreateFeeRecord { get; set; } = true;
        public static bool CanDeleteFeeRecord { get; set; } = true;
        public static bool CanAddTeacher { get; set; } = true;
        public static bool CanViewTeacher { get; set; } = true;
        public static bool CanEditTeacher { get; set; } = true;
        public static bool CanDeleteTeacher { get; set; } = true;
        public static bool CanViewParent { get; set; } = true;
        public static bool CanEditParent { get; set; } = true;
        public static bool CanDeleteParent { get; set; } = true;
    }

    public enum AccountType
    {
        Admin,
        Accountant
    }

}

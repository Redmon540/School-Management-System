namespace School_Manager
{
    public class RegistrationStatus
    {
        public RegistrationWarning Status { get; set; }
        public string Type { get; set; }
        public int DaysLeft { get; set; }
    }

    public enum RegistrationWarning
    {
        Registered,
        Alert,
        Expired
    }
}

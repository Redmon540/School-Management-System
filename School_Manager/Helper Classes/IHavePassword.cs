using System.Security;

namespace School_Manager
{
    /// <summary>
    /// An interface for a  class that can provide  a secure password
    /// </summary>
    public interface IHavePassword
    {
        SecureString SecurePassword { get;   }
        SecureString SecureConfirmPassword { get; }
        SecureString SecureAdminPassword { get; }
    }
}

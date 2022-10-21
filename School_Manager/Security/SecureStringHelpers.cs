using System;
using System.Runtime.InteropServices;
using System.Security;

namespace School_Manager
{
    /// <summary>
    /// Helpers for the <see cref="SecureString"/>class
    /// </summary>
    /// <param name="securePassword">The Secure String</param>
    /// <returns></returns>
    public static class SecureStringHelpers
    {
        public static string Unsecure(this SecureString secureString)
        {
            //Make sure we a secure string
            if (secureString == null)
                return string.Empty;

            // Get a pointer for an unsecure string in memory
            var unmanagedString = IntPtr.Zero;
            try
            {
                //Unsecures the secureString
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(secureString);
                return Marshal.PtrToStringUni(unmanagedString);

            }
            finally
            {
                // Clean up any memory alloation
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }
    }
}

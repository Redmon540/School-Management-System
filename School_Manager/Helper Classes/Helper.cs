using QRCoder;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace School_Manager
{
    public static class Helper
    {
        /// <summary>
        /// To convert a bit[] to a bitmap image
        /// </summary>
        /// <param name="imageData"></param>
        /// <returns></returns>
        public static BitmapImage ByteArrayToImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0)
                return null;
            var image = new BitmapImage();
            using (var mem = new MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            return image;
        }

        public static byte[] ImageToByteArray(BitmapImage image)
        {
            if (image == null)
                return null;
            byte[] data;
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image));
            using (MemoryStream ms = new MemoryStream())
            {
                encoder.Save(ms);
                data = ms.ToArray();
            }
            return data;
        }

        public static System.Windows.Media.Brush GetBrush(string HexValue)
        {
            var converter = new BrushConverter();
            return(System.Windows.Media.Brush)converter.ConvertFromString(HexValue);
        }

        public static BitmapImage CreateThumbnail(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0)
                return null;
            var image = new BitmapImage();
            using (var mem = new MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.DecodePixelHeight = 50;
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            return image;
        }

        public static BitmapImage GetQRCode(string Content)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(Content, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qr = qrCode.GetGraphic(20);
            BitmapImage bitmapImage = new BitmapImage();
            using (MemoryStream memory = new MemoryStream())
            {
                qr.Save(memory, ImageFormat.Png);
                memory.Position = 0;
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
            }
            return bitmapImage;
        }

        public static List<string> GetDays()
        {
            return new List<string> { "1" , "2","3","4","5","6","7","8","9","10","11","12"
            ,"13","14","15","16","17","18","19","20","21","22","23","24","25","26","27","28","29","30","31"};
        }
             
        /// <summary>
        /// Returns list of string names of months
        /// </summary>
        /// <returns></returns>
        public static List<string> GetMonths()
        {
            return new List<string>() { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
        }

        /// <summary>
        /// Returns string list of years from 2010 to 2030
        /// </summary>
        /// <returns></returns>
        public static List<string> GetYears()
        {
            return new List<string>() { "2019", "2020", "2021", "2022", "2023", "2024", "2025" , "2026" , "2027" , "2028" , "2029" ,
            "2030" , "2031" , "2032" , "2033" , "2034" , "2035" , "2036" , "2037" , "2038" , "2039" , "2040" ,
            "2041" , "2042" , "2043" , "2044" , "2045" , "2046" , "2047" , "2048" , "2049" , "2050" ,
            "2051" , "2052" , "2053" , "2054" , "2055" , "2056" , "2057" , "2058" , "2059" , "2060" ,
            "2061" , "2062" , "2063" , "2064" , "2065" , "2066" , "2067" , "2068" , "2069" , "2070" ,
            "2071" , "207" , "2073" , "2074" , "2075" , "2076" , "2077" , "2078" , "2079" , "2080" ,
            "2081" , "2082" , "2083" , "2084" , "2085" , "2086" , "2087" , "2088" , "2089" , "2090" ,
            "2091" , "2092" , "2093" , "2094" , "2095" , "2096" , "2097" , "2098" , "2099"};
        }

        /// <summary>
        /// Gets the month number against its name
        /// </summary>
        /// <param name="Month"></param>
        /// <returns></returns>
        public static int GetMonthNumber(string Month)
        {
            switch (Month)
            {
                case "January":
                    return 1;
                case "February":
                    return 2;
                case "March":
                    return 3;
                case "April":
                    return 4;
                case "May":
                    return 5;
                case "June":
                    return 6;
                case "July":
                    return 7;
                case "August":
                    return 8;
                case "September":
                    return 9;
                case "October":
                    return 10;
                case "November":
                    return 11;
                case "December":
                    return 12;
            }
            return 0;
                
        }

        /// <summary>
        /// Get the name of month against its number
        /// </summary>
        /// <param name="Month"></param>
        /// <returns></returns>
        public static string GetMonthName(int Month)
        {
            switch (Month)
            {
                case 1:
                    return "January";
                case 2:
                    return "February";
                case 3:
                    return "March";
                case 4:
                    return "April";
                case 5:
                    return "May";
                case 6:
                    return "June";
                case 7:
                    return "July";
                case 8:
                    return "August";
                case 9:
                    return "September";
                case 10:
                    return "October";
                case 11:
                    return "November";
                case 12:
                    return "December";
            }
            return "";
                
        }

        /// <summary>
        /// To get fonts sizes
        /// </summary>
        /// <returns></returns>
        public static List<double> GetFontSizes() => new List<double> { 6, 8, 10, 12, 14, 16, 18, 20, 22, 24, 26, 28, 30, 32 , 34 , 36 , 38 ,40 , 42, 44 , 46 , 48 , 50 };

        private static Random r = new Random();

        /// <summary>
        /// Generates random light color between 150-255
        /// </summary>
        /// <returns></returns>
        public static System.Windows.Media.Brush GetRandomColor()
        {
            return new SolidColorBrush(System.Windows.Media.Color.FromRgb((byte)r.Next(1, 255),
              (byte)r.Next(1, 255), (byte)r.Next(1, 255)));
        }

        /// <summary>
        /// To clone a class without referencing eachother
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T Clone<T>(T source)
        {
            if (!typeof(T).IsSerializable)
            {
                throw new ArgumentException("The type must be serializable.", "source");
            }

            if (Object.ReferenceEquals(source, null))
            {
                return default(T);
            }

            System.Runtime.Serialization.IFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            Stream stream = new MemoryStream();
            using (stream)
            {
                formatter.Serialize(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(stream);
            }
        }

        /// <summary>
        /// Returns a boolean value indicating if row is null or empty
        /// </summary>
        /// <param name="Row"></param>
        /// <returns></returns>
        public static bool IsRowEmpty(this DataRow Row)
        {
            if (Row == null)
            {
                return true;
            }
            else
            {
                foreach (var value in Row.ItemArray)
                {
                    if (value.ToString().IsNullOrEmpty())
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        #region Text Helpers

        public static System.Windows.FontStyle GetFontStyle(string FontStyle)
        {
            if (FontStyle.IsNullOrEmpty())
                return FontStyles.Normal;
            var propertyInfo = typeof(FontStyles).GetProperty(FontStyle,
                                                  BindingFlags.Static |
                                                  BindingFlags.Public |
                                                  BindingFlags.IgnoreCase);
            return (System.Windows.FontStyle)propertyInfo.GetValue(null, null);
        }

        public static System.Windows.FontWeight GetFontWeight(string FontWeight)
        {
            if (FontWeight.IsNullOrEmpty())
                return FontWeights.Normal;
            var propertyInfo = typeof(FontWeights).GetProperty(FontWeight,
                                                  BindingFlags.Static |
                                                  BindingFlags.Public |
                                                  BindingFlags.IgnoreCase);
            return (System.Windows.FontWeight)propertyInfo.GetValue(null, null);
        }

        public static System.Windows.TextAlignment GetTextAlignment(string TextAligment)
        {
            if (TextAligment.IsNullOrEmpty())
                return TextAlignment.Left;
            return (TextAlignment)TextAlignment.Parse(typeof(TextAlignment), TextAligment);
            
        }

        #endregion

        #region Password Helpers

        /// <summary>
        /// Generate the SHA256 hash of the password with a salt
        /// </summary>
        /// <param name="password"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        public static byte[] GeneratePasswordHash(SecureString Password, byte[] salt)
        {
            byte[] password = Encoding.UTF8.GetBytes(Password.Unsecure());
            byte[] plainTextWithSaltBytes =
              new byte[password.Length + salt.Length];

            for (int i = 0; i < password.Length; i++)
            {
                plainTextWithSaltBytes[i] = password[i];
            }
            for (int i = 0; i < salt.Length; i++)
            {
                plainTextWithSaltBytes[password.Length + i] = salt[i];
            }
            using (HashAlgorithm algorithm = new SHA256Managed())
            {
                return algorithm.ComputeHash(plainTextWithSaltBytes);
            }
        }

        /// <summary>
        /// Generates a random salt of size 32
        /// </summary>
        /// <returns></returns>
        public static byte[] GenerateSalt()
        {
            var rng = new RNGCryptoServiceProvider();
            var array = new byte[32];
            rng.GetBytes(array);
            return array;
        }

        /// <summary>
        /// Compare the two byte hashes and return a boolean
        /// </summary>
        /// <param name="hash1"></param>
        /// <param name="hash2"></param>
        /// <returns></returns>
        public static bool CompareHashes(byte[] hash1, byte[] hash2)
        {
            bool match = true;
            for (int i = 0; i < hash1.Length; i++)
            {
                if (hash1[i] != hash2[i])
                {
                    match = false;
                    break;
                }
            }
            return match;
        }

        /// <summary>
        /// Compare the two hashes and return a boolean
        /// </summary>
        /// <param name="Password"></param>
        /// <param name="PasswordHash"></param>
        /// <param name="Salt"></param>
        /// <returns></returns>
        public static bool CompareHashes(SecureString Password, byte[] PasswordHash, byte[] Salt)
        {
            return CompareHashes(PasswordHash, GeneratePasswordHash(Password, Salt));
        }

        #endregion

        #region Registration Helpers

        public static byte[] GetHash(string Input)
        {
            using (HashAlgorithm algorithm = new SHA256Managed())
            {
                return algorithm.ComputeHash(Encoding.UTF8.GetBytes(Input));
            }
        }

        /// <summary>
        /// Returns the string representation in Hexadecimal of the hash
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        public static string GetStringFromHash(byte[] hash)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                //use X2 to format string as hexadecimal values(so it will only contain numbers and digits)
                result.Append(hash[i].ToString("X2"));
            }
            return result.ToString();
        }

        /// <summary>
        /// Returns the byte array of the hexadecimal string
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        // This constant is used to determine the keysize of the encryption algorithm in bits.
        // We divide this by 8 within the code below to get the equivalent number of bytes.
        private const int Keysize = 256;

        // This constant determines the number of iterations for the password bytes generation function.
        private const int DerivationIterations = 1000;

        public static string Encrypt(this string plainText)
        {
            string passPhrase = ProductInformation.ProductID;
            // Salt and IV is randomly generated each time, but is preprended to encrypted cipher text
            // so that the same Salt and IV values can be used when decrypting.  
            var saltStringBytes = Generate256BitsOfRandomEntropy();
            var ivStringBytes = Generate256BitsOfRandomEntropy();
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
            {
                var keyBytes = password.GetBytes(Keysize / 8);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 256;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                            {
                                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                                cryptoStream.FlushFinalBlock();
                                // Create the final bytes as a concatenation of the random salt bytes, the random iv bytes and the cipher bytes.
                                var cipherTextBytes = saltStringBytes;
                                cipherTextBytes = cipherTextBytes.Concat(ivStringBytes).ToArray();
                                cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Convert.ToBase64String(cipherTextBytes);
                            }
                        }
                    }
                }
            }
        }

        public static string Decrypt(this string cipherText)
        {
            if (cipherText.IsNullOrEmpty())
                return string.Empty;
            string passPhrase = ProductInformation.ProductID;

            // Get the complete stream of bytes that represent:
            // [32 bytes of Salt] + [32 bytes of IV] + [n bytes of CipherText]
            var cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText);
            // Get the saltbytes by extracting the first 32 bytes from the supplied cipherText bytes.
            var saltStringBytes = cipherTextBytesWithSaltAndIv.Take(Keysize / 8).ToArray();
            // Get the IV bytes by extracting the next 32 bytes from the supplied cipherText bytes.
            var ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(Keysize / 8).Take(Keysize / 8).ToArray();
            // Get the actual cipher text bytes by removing the first 64 bytes from the cipherText string.
            var cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip((Keysize / 8) * 2).Take(cipherTextBytesWithSaltAndIv.Length - ((Keysize / 8) * 2)).ToArray();

            using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
            {
                var keyBytes = password.GetBytes(Keysize / 8);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 256;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream(cipherTextBytes))
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                            {
                                var plainTextBytes = new byte[cipherTextBytes.Length];
                                var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                            }
                        }
                    }
                }
            }
        }

        private static byte[] Generate256BitsOfRandomEntropy()
        {
            var randomBytes = new byte[32]; // 32 Bytes will give us 256 bits.
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                // Fill the array with cryptographically secure random bytes.
                rngCsp.GetBytes(randomBytes);
            }
            return randomBytes;
        }

        #endregion

        #region String Helpers
        /// <summary>
        /// Extension method to check null or empty string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        public static bool IsNotNullOrEmpty(this string value)
        {
            return !string.IsNullOrEmpty(value);
        }

        #endregion

        #region DataGrid Helpers
        private static T FindAnchestor<T>(DependencyObject current)
         where T : DependencyObject
        {
            do
            {
                if (current is T)
                {
                    return (T)current;
                }
                current = VisualTreeHelper.GetParent(current);
            }
            while (current != null);
            return null;
        }
        public static T GetVisualChild<T>(Visual parent) where T : Visual
        {
            T child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(v);
                }
                if (child != null)
                {
                    break;
                }
            }
            return child;
        }
        public static DataGridRow GetRow(this DataGrid grid, int index)
        {
            DataGridRow row = (DataGridRow)grid.ItemContainerGenerator.ContainerFromIndex(index);
            if (row == null)
            {
                // May be virtualized, bring into view and try again.
                grid.UpdateLayout();
                grid.ScrollIntoView(grid.Items[index]);
                row = (DataGridRow)grid.ItemContainerGenerator.ContainerFromIndex(index);
            }
            return row;
        }
        public static DataGridCell GetCell(this DataGrid grid, DataGridRow row, int column)
        {
            if (row != null)
            {
                DataGridCellsPresenter presenter = GetVisualChild<DataGridCellsPresenter>(row);

                if (presenter == null)
                {
                    grid.ScrollIntoView(row, grid.Columns[column]);
                    presenter = GetVisualChild<DataGridCellsPresenter>(row);
                }

                DataGridCell cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
                return cell;
            }
            return null;
        }
        public static DataGridCell GetCell(this DataGrid grid, int row, int column)
        {
            DataGridRow rowContainer = grid.GetRow(row);
            return grid.GetCell(rowContainer, column);
        }

        #endregion

        #region Watch

        public static Stopwatch Timer = new Stopwatch();

        public static void StartTimer()
        {
            Timer.Start();
        }

        public static void StopTimer()
        {
            Timer.Stop();
        }

        #endregion
    }
}
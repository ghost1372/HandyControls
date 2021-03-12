using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace HandyControl.Tools
{
    public class CryptographyHelper
    {
        public static bool VerifyMD5(string hash, string input)
        {
            if (string.IsNullOrEmpty(hash))
                throw new ArgumentNullException(nameof(hash));

            if (string.IsNullOrEmpty(input))
                throw new ArgumentNullException(nameof(input));

            input = GenerateMD5(input);
            return hash.Equals(input);
        }

        public static bool VerifySHA256(string hash, string input)
        {
            if (string.IsNullOrEmpty(hash))
                throw new ArgumentNullException(nameof(hash));

            if (string.IsNullOrEmpty(input))
                throw new ArgumentNullException(nameof(input));

            input = GenerateSHA256(input);
            return hash.Equals(input);
        }
        public static string GenerateMD5(string input)
        {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentNullException(nameof(input));

            using MD5 md5 = MD5.Create();
            byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();
            foreach (var t in hashBytes)
            {
                sb.Append(t.ToString("X2"));
            }
            return sb.ToString();
        }

        /// <summary>
        /// Generate SHA256 for String
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GenerateSHA256(string input)
        {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentNullException(nameof(input));

            var crypt = new SHA256Managed();
            var hash = new StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(input));
            foreach (byte theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }
            return hash.ToString();
        }

        /// <summary>
        /// Generate SHA256 for File
        /// </summary>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        public static string GenerateSHA256ForFile(string FilePath)
        {
            if (string.IsNullOrEmpty(FilePath))
                throw new ArgumentNullException(nameof(FilePath));

            return BytesToString(GetHashSha256(FilePath));
        }

        private static readonly SHA256 Sha256 = SHA256.Create();
        private static byte[] GetHashSha256(string filename)
        {
            using FileStream stream = File.OpenRead(filename);
            return Sha256.ComputeHash(stream);
        }
        private static string BytesToString(byte[] bytes)
        {
            string result = "";
            foreach (byte b in bytes)
            {
                result += b.ToString("x2");
            }

            return result;
        }

        // Rfc2898DeriveBytes constants:
        internal static readonly byte[] salt = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }; // Must be at least eight bytes.  MAKE THIS SALTIER!
        internal static readonly int iterations = 1042; // Recommendation is >= 1000.

        /// <summary>Decrypt a file.</summary>
        /// <remarks>NB: "Padding is invalid and cannot be removed." is the Universal CryptoServices error.  Make sure the password, salt and iterations are correct before getting nervous.</remarks>
        /// <param name="sourceFilename">The full path and name of the file to be decrypted.</param>
        /// <param name="destinationFilename">The full path and name of the file to be output.</param>
        /// <param name="password">The password for the decryption.</param>
        public static void DecryptFileAES(string sourceFilename, string destinationFilename, string password)
        {
            if (string.IsNullOrEmpty(sourceFilename))
                throw new ArgumentNullException(nameof(sourceFilename));

            if (string.IsNullOrEmpty(destinationFilename))
                throw new ArgumentNullException(nameof(destinationFilename));

            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException(nameof(password));

            AesManaged aes = new AesManaged();
            aes.BlockSize = aes.LegalBlockSizes[0].MaxSize;
            aes.KeySize = aes.LegalKeySizes[0].MaxSize;
            // NB: Rfc2898DeriveBytes initialization and subsequent calls to   GetBytes   must be eactly the same, including order, on both the encryption and decryption sides.
            Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(password, salt, iterations);
            aes.Key = key.GetBytes(aes.KeySize / 8);
            aes.IV = key.GetBytes(aes.BlockSize / 8);
            aes.Mode = CipherMode.CBC;
            ICryptoTransform transform = aes.CreateDecryptor(aes.Key, aes.IV);

            using FileStream destination = new FileStream(destinationFilename, FileMode.CreateNew, FileAccess.Write, FileShare.None);
            using CryptoStream cryptoStream = new CryptoStream(destination, transform, CryptoStreamMode.Write);
            try
            {
                using FileStream source = new FileStream(sourceFilename, FileMode.Open, FileAccess.Read, FileShare.Read);
                source.CopyTo(cryptoStream);
            }
            catch (CryptographicException exception)
            {
                if (exception.Message == "Padding is invalid and cannot be removed.")
                    throw new ApplicationException("Universal Microsoft Cryptographic Exception (Not to be believed!)", exception);
                else
                    throw;
            }
        }

        /// <summary>Encrypt a file.</summary>
        /// <param name="sourceFilename">The full path and name of the file to be encrypted.</param>
        /// <param name="destinationFilename">The full path and name of the file to be output.</param>
        /// <param name="password">The password for the encryption.</param>
        public static void EncryptFileAES(string sourceFilename, string destinationFilename, string password)
        {
            if (string.IsNullOrEmpty(sourceFilename))
                throw new ArgumentNullException(nameof(sourceFilename));

            if (string.IsNullOrEmpty(destinationFilename))
                throw new ArgumentNullException(nameof(destinationFilename));

            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException(nameof(password));

            AesManaged aes = new AesManaged();
            aes.BlockSize = aes.LegalBlockSizes[0].MaxSize;
            aes.KeySize = aes.LegalKeySizes[0].MaxSize;
            // NB: Rfc2898DeriveBytes initialization and subsequent calls to   GetBytes   must be eactly the same, including order, on both the encryption and decryption sides.
            Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(password, salt, iterations);
            aes.Key = key.GetBytes(aes.KeySize / 8);
            aes.IV = key.GetBytes(aes.BlockSize / 8);
            aes.Mode = CipherMode.CBC;
            ICryptoTransform transform = aes.CreateEncryptor(aes.Key, aes.IV);

            using FileStream destination = new FileStream(destinationFilename, FileMode.CreateNew, FileAccess.Write, FileShare.None);
            using CryptoStream cryptoStream = new CryptoStream(destination, transform, CryptoStreamMode.Write);
            using FileStream source = new FileStream(sourceFilename, FileMode.Open, FileAccess.Read, FileShare.Read);
            source.CopyTo(cryptoStream);
        }

        public static string EncryptTextAES(string input, string password)
        {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentNullException(nameof(input));

            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException(nameof(password));

            RijndaelManaged objrij = new RijndaelManaged
            {
                Mode = CipherMode.CBC, Padding = PaddingMode.PKCS7, KeySize = 0x80, BlockSize = 0x80
            };
           
            //set the symmetric key that is used for encryption & decryption.
            byte[] passBytes = Encoding.UTF8.GetBytes(password);
            //set the initialization vector (IV) for the symmetric algorithm
            byte[] EncryptionkeyBytes = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            int len = passBytes.Length;
            if (len > EncryptionkeyBytes.Length)
            {
                len = EncryptionkeyBytes.Length;
            }
            Array.Copy(passBytes, EncryptionkeyBytes, len);
            objrij.Key = EncryptionkeyBytes;
            objrij.IV = EncryptionkeyBytes;
            //Creates symmetric AES object with the current key and initialization vector IV.
            ICryptoTransform objtransform = objrij.CreateEncryptor();
            byte[] textDataByte = Encoding.UTF8.GetBytes(input);
            //Final transform the test string.
            return Convert.ToBase64String(objtransform.TransformFinalBlock(textDataByte, 0, textDataByte.Length));
        }

        public static string DecryptTextAES(string EncryptedText, string password)
        {
            if (string.IsNullOrEmpty(EncryptedText))
                throw new ArgumentNullException(nameof(EncryptedText));

            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException(nameof(password));

            RijndaelManaged objrij = new RijndaelManaged
            {
                Mode = CipherMode.CBC, Padding = PaddingMode.PKCS7, KeySize = 0x80, BlockSize = 0x80
            };
            byte[] encryptedTextByte = Convert.FromBase64String(EncryptedText);
            byte[] passBytes = Encoding.UTF8.GetBytes(password);
            byte[] EncryptionkeyBytes = new byte[0x10];
            int len = passBytes.Length;
            if (len > EncryptionkeyBytes.Length)
            {
                len = EncryptionkeyBytes.Length;
            }
            Array.Copy(passBytes, EncryptionkeyBytes, len);
            objrij.Key = EncryptionkeyBytes;
            objrij.IV = EncryptionkeyBytes;
            byte[] TextByte = objrij.CreateDecryptor().TransformFinalBlock(encryptedTextByte, 0, encryptedTextByte.Length);
            return Encoding.UTF8.GetString(TextByte);  //it will return readable string
        }
    }
}

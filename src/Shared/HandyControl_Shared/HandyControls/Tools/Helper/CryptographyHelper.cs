using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace HandyControl.Tools;

public static class CryptographyHelper
{
    private static readonly SHA256 Sha256 = SHA256.Create();
    // Rfc2898DeriveBytes constants:
    internal static readonly byte[] salt = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }; // Must be at least eight bytes.  MAKE THIS SALTIER!
    internal static readonly int iterations = 1042; // Recommendation is >= 1000.

    #region Hash
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
    public static string GenerateSHA256FromFile(string FilePath)
    {
        if (string.IsNullOrEmpty(FilePath))
            throw new ArgumentNullException(nameof(FilePath));

        return BytesToString(GetHashSha256(FilePath));
    }

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
    #endregion

    #region String Encryption
    /// <summary>
    /// Encrypt string with AES
    /// </summary>
    /// <param name="input"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public static string EncryptStringAES(string input, string password)
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

    /// <summary>
    /// Decrypt string with AES
    /// </summary>
    /// <param name="encryptedString"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public static string DecryptStringAES(string encryptedString, string password)
    {
        if (string.IsNullOrEmpty(encryptedString))
            throw new ArgumentNullException(nameof(encryptedString));

        if (string.IsNullOrEmpty(password))
            throw new ArgumentNullException(nameof(password));

        RijndaelManaged objrij = new RijndaelManaged
        {
            Mode = CipherMode.CBC, Padding = PaddingMode.PKCS7, KeySize = 0x80, BlockSize = 0x80
        };
        byte[] encryptedTextByte = Convert.FromBase64String(encryptedString);
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

    /// <summary>
    /// Encrypt string with Base64
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string EncryptStringBase64(string input)
    {
        var btArray = Encoding.UTF8.GetBytes(input);
        return Convert.ToBase64String(btArray, 0, btArray.Length);
    }

    /// <summary>
    /// Decrypt string with Base64
    /// </summary>
    /// <param name="encryptedString"></param>
    /// <returns></returns>
    public static string DecryptStringBase64(string encryptedString)
    {
        var btArray = Convert.FromBase64String(encryptedString);
        return Encoding.UTF8.GetString(btArray);
    }

    /// <summary>
    /// Encrypt string with RSA
    /// </summary>
    /// <param name="input"></param>
    /// <param name="publicKey"></param>
    /// <returns></returns>
    public static string EncryptStringRSA(string input, string publicKey)
    {
        return Convert.ToBase64String(EncryptDataRSA(Encoding.UTF8.GetBytes(input), publicKey));
    }

    /// <summary>
    /// Decrypt string with RSA
    /// </summary>
    /// <param name="encryptedString"></param>
    /// <param name="privateKey"></param>
    /// <returns></returns>
    public static string DecryptStringRSA(string encryptedString, string privateKey)
    {
        return Encoding.UTF8.GetString(DecryptDataRSA(Convert.FromBase64String(encryptedString), privateKey));
    }
    #endregion

    #region File Encryption

    #region AES
    internal static CryptoStream DecryptAES(string sourceFilename, string destinationFilename, string password)
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
        return cryptoStream;
    }

#if !NET40
    /// <summary>Decrypt a file async.</summary>
    /// <param name="sourceFilename">The full path and name of the file to be decrypted.</param>
    /// <param name="destinationFilename">The full path and name of the file to be output.</param>
    /// <param name="password">The password for the decryption.</param>
    public static async void DecryptFileAESAsync(string sourceFilename, string destinationFilename, string password)
    {
        var cryptoStream = DecryptAES(sourceFilename, destinationFilename, password);
        try
        {
            using FileStream source = new FileStream(sourceFilename, FileMode.Open, FileAccess.Read, FileShare.Read);
            await source.CopyToAsync(cryptoStream);
        }
        catch (CryptographicException exception)
        {
            if (exception.Message == "Padding is invalid and cannot be removed.")
                throw new ApplicationException("Universal Microsoft Cryptographic Exception (Not to be believed!)", exception);
            else
                throw;
        }
    }
#endif
    /// <summary>Decrypt a file.</summary>
    /// <param name="sourceFilename">The full path and name of the file to be decrypted.</param>
    /// <param name="destinationFilename">The full path and name of the file to be output.</param>
    /// <param name="password">The password for the decryption.</param>
    public static void DecryptFileAES(string sourceFilename, string destinationFilename, string password)
    {
        var cryptoStream = DecryptAES(sourceFilename, destinationFilename, password);
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

    internal static CryptoStream EncryptAES(string sourceFilename, string destinationFilename, string password)
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
        return cryptoStream;
    }

#if !NET40
    /// <summary>Encrypt a file async.</summary>
    /// <param name="sourceFilename">The full path and name of the file to be encrypted.</param>
    /// <param name="destinationFilename">The full path and name of the file to be output.</param>
    /// <param name="password">The password for the encryption.</param>
    public static async void EncryptFileAESAsync(string sourceFilename, string destinationFilename, string password)
    {
        var cryptoStream = EncryptAES(sourceFilename, destinationFilename, password);
        using FileStream source = new FileStream(sourceFilename, FileMode.Open, FileAccess.Read, FileShare.Read);
        await source.CopyToAsync(cryptoStream);
    }
#endif

    /// <summary>Encrypt a file.</summary>
    /// <param name="sourceFilename">The full path and name of the file to be encrypted.</param>
    /// <param name="destinationFilename">The full path and name of the file to be output.</param>
    /// <param name="password">The password for the encryption.</param>
    public static void EncryptFileAES(string sourceFilename, string destinationFilename, string password)
    {
        var cryptoStream = EncryptAES(sourceFilename, destinationFilename, password);
        using FileStream source = new FileStream(sourceFilename, FileMode.Open, FileAccess.Read, FileShare.Read);
        source.CopyTo(cryptoStream);
    }
    #endregion

    #region RSA
    /// <summary>
    /// Encrypt a file Asymmetric
    /// </summary>
    /// <param name="data"></param>
    /// <param name="publicKey"></param>
    /// <returns></returns>
    public static byte[] EncryptDataRSA(byte[] data, string publicKey)
    {
        using (var asymmetricProvider = new RSACryptoServiceProvider())
        {
            asymmetricProvider.FromXmlString(publicKey);
            return asymmetricProvider.Encrypt(data, true);
        }
    }

    /// <summary>
    /// Decrypt a file Asymmetric
    /// </summary>
    /// <param name="data"></param>
    /// <param name="privateKey"></param>
    /// <returns></returns>
    public static byte[] DecryptDataRSA(byte[] data, string privateKey)
    {
        using (var asymmetricProvider = new RSACryptoServiceProvider())
        {
            asymmetricProvider.FromXmlString(privateKey);
            if (asymmetricProvider.PublicOnly)
                throw new Exception("The key provided is a public key and does not contain the private key elements required for decryption");
            return asymmetricProvider.Decrypt(data, true);
        }
    }
#if !NET40
    /// <summary>
    /// Encrypt a File async.
    /// </summary>
    /// <param name="inputFilePath"></param>
    /// <param name="outputFilePath"></param>
    /// <param name="publicKey"></param>
    public static async void EncryptFileRSAAsync(string inputFilePath, string outputFilePath, string publicKey)
    {
        using (var symmetricCypher = new AesManaged())
        {
            // Generate random key and IV for symmetric encryption
            var key = new byte[symmetricCypher.KeySize / 8];
            var iv = new byte[symmetricCypher.BlockSize / 8];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(key);
                rng.GetBytes(iv);
            }

            // Encrypt the symmetric key and IV
            var buf = new byte[key.Length + iv.Length];
            Array.Copy(key, buf, key.Length);
            Array.Copy(iv, 0, buf, key.Length, iv.Length);
            buf = EncryptDataRSA(buf, publicKey);

            var bufLen = BitConverter.GetBytes(buf.Length);

            // Symmetrically encrypt the data and write it to the file, along with the encrypted key and iv
            using (var cypherKey = symmetricCypher.CreateEncryptor(key, iv))
            using (var fsIn = new FileStream(inputFilePath, FileMode.Open))
            using (var fsOut = new FileStream(outputFilePath, FileMode.Create))
            using (var cs = new CryptoStream(fsOut, cypherKey, CryptoStreamMode.Write))
            {
                await fsOut.WriteAsync(bufLen, 0, bufLen.Length);
                await fsOut.WriteAsync(buf, 0, buf.Length);
                await fsIn.CopyToAsync(cs);
            }
        }
    }
#endif

    /// <summary>
    /// Encrypt a File.
    /// </summary>
    /// <param name="inputFilePath"></param>
    /// <param name="outputFilePath"></param>
    /// <param name="publicKey"></param>
    public static void EncryptFileRSA(string inputFilePath, string outputFilePath, string publicKey)
    {
        using (var symmetricCypher = new AesManaged())
        {
            // Generate random key and IV for symmetric encryption
            var key = new byte[symmetricCypher.KeySize / 8];
            var iv = new byte[symmetricCypher.BlockSize / 8];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(key);
                rng.GetBytes(iv);
            }

            // Encrypt the symmetric key and IV
            var buf = new byte[key.Length + iv.Length];
            Array.Copy(key, buf, key.Length);
            Array.Copy(iv, 0, buf, key.Length, iv.Length);
            buf = EncryptDataRSA(buf, publicKey);

            var bufLen = BitConverter.GetBytes(buf.Length);

            // Symmetrically encrypt the data and write it to the file, along with the encrypted key and iv
            using (var cypherKey = symmetricCypher.CreateEncryptor(key, iv))
            using (var fsIn = new FileStream(inputFilePath, FileMode.Open))
            using (var fsOut = new FileStream(outputFilePath, FileMode.Create))
            using (var cs = new CryptoStream(fsOut, cypherKey, CryptoStreamMode.Write))
            {
                fsOut.Write(bufLen, 0, bufLen.Length);
                fsOut.Write(buf, 0, buf.Length);
                fsIn.CopyTo(cs);
            }
        }
    }

#if !NET40
    /// <summary>
    /// Decrypt a File async.
    /// </summary>
    /// <param name="inputFilePath"></param>
    /// <param name="outputFilePath"></param>
    /// <param name="privateKey"></param>
    public static async void DecryptFileRSAAsync(string inputFilePath, string outputFilePath, string privateKey)
    {
        using (var symmetricCypher = new AesManaged())
        using (var fsIn = new FileStream(inputFilePath, FileMode.Open))
        {
            // Determine the length of the encrypted key and IV
            var buf = new byte[sizeof(int)];
            await fsIn.ReadAsync(buf, 0, buf.Length);
            var bufLen = BitConverter.ToInt32(buf, 0);

            // Read the encrypted key and IV data from the file and decrypt using the asymmetric algorithm
            buf = new byte[bufLen];
            await fsIn.ReadAsync(buf, 0, buf.Length);
            buf = DecryptDataRSA(buf, privateKey);

            var key = new byte[symmetricCypher.KeySize / 8];
            var iv = new byte[symmetricCypher.BlockSize / 8];
            Array.Copy(buf, key, key.Length);
            Array.Copy(buf, key.Length, iv, 0, iv.Length);

            // Decript the file data using the symmetric algorithm
            using (var cypherKey = symmetricCypher.CreateDecryptor(key, iv))
            using (var fsOut = new FileStream(outputFilePath, FileMode.Create))
            using (var cs = new CryptoStream(fsOut, cypherKey, CryptoStreamMode.Write))
            {
                await fsIn.CopyToAsync(cs);
            }
        }
    }
#endif

    /// <summary>
    /// Decrypt a File.
    /// </summary>
    /// <param name="inputFilePath"></param>
    /// <param name="outputFilePath"></param>
    /// <param name="privateKey"></param>
    public static void DecryptFileRSA(string inputFilePath, string outputFilePath, string privateKey)
    {
        using (var symmetricCypher = new AesManaged())
        using (var fsIn = new FileStream(inputFilePath, FileMode.Open))
        {
            // Determine the length of the encrypted key and IV
            var buf = new byte[sizeof(int)];
            fsIn.Read(buf, 0, buf.Length);
            var bufLen = BitConverter.ToInt32(buf, 0);

            // Read the encrypted key and IV data from the file and decrypt using the asymmetric algorithm
            buf = new byte[bufLen];
            fsIn.Read(buf, 0, buf.Length);
            buf = DecryptDataRSA(buf, privateKey);

            var key = new byte[symmetricCypher.KeySize / 8];
            var iv = new byte[symmetricCypher.BlockSize / 8];
            Array.Copy(buf, key, key.Length);
            Array.Copy(buf, key.Length, iv, 0, iv.Length);

            // Decript the file data using the symmetric algorithm
            using (var cypherKey = symmetricCypher.CreateDecryptor(key, iv))
            using (var fsOut = new FileStream(outputFilePath, FileMode.Create))
            using (var cs = new CryptoStream(fsOut, cypherKey, CryptoStreamMode.Write))
            {
                fsIn.CopyTo(cs);
            }
        }
    }
    #endregion

    #endregion

    #region RSA Key

    public class RSAKey
    {
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
    }

    /// <summary>
    /// This method generates RSA public and private keys
    /// KeySize is measured in bits. 1024 is the default, 2048 is better, 4096 is more robust but takes a fair bit longer to generate.
    /// </summary>
    /// <returns></returns>
    public static RSAKey GenerateRSAKey(int keySize = 1024)
    {
        using (var rsa = new RSACryptoServiceProvider(keySize))
        {
            return new RSAKey { PublicKey = rsa.ToXmlString(false), PrivateKey = rsa.ToXmlString(true) };
        }
    }

    /// <summary>
    /// Export PublicKey To File
    /// </summary>
    /// <param name="path"></param>
    /// <param name="publicKey"></param>
    public static void ExportPublicKeyToFile(string path, string publicKey)
    {
        File.WriteAllText(path, publicKey);
    }

    /// <summary>
    /// Read PublicKey from File
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string ReadPublicKey(string path)
    {
        return File.ReadAllText(path);
    }

    /// <summary>
    /// Export PrivateKey To File
    /// </summary>
    /// <param name="path"></param>
    /// <param name="privateKey"></param>
    /// <param name="password"></param>
    /// <param name="symmetricSalt"></param>
    public static void ExportPrivateKeyToFile(string path, string privateKey, string password, string symmetricSalt = null)
    {
        if (string.IsNullOrEmpty(symmetricSalt))
        {
            symmetricSalt = "HandyControls";
        }

        var salt = Encoding.UTF8.GetBytes(symmetricSalt);
        using (var cypher = new AesManaged())
        {
            var pdb = new Rfc2898DeriveBytes(password, salt);
            var key = pdb.GetBytes(cypher.KeySize / 8);
            var iv = pdb.GetBytes(cypher.BlockSize / 8);

            using (var encryptor = cypher.CreateEncryptor(key, iv))
            using (var fsEncrypt = new FileStream(path, FileMode.Create))
            using (var csEncrypt = new CryptoStream(fsEncrypt, encryptor, CryptoStreamMode.Write))
            using (var swEncrypt = new StreamWriter(csEncrypt))
            {
                swEncrypt.Write(privateKey);
            }
        }
    }

    /// <summary>
    /// Read PrivateKey from File
    /// </summary>
    /// <param name="path"></param>
    /// <param name="password"></param>
    /// <param name="symmetricSalt"></param>
    /// <returns></returns>
    public static string ReadPrivateKey(string path, string password, string symmetricSalt = null)
    {
        if (string.IsNullOrEmpty(symmetricSalt))
        {
            symmetricSalt = "HandyControls";
        }

        var salt = Encoding.UTF8.GetBytes(symmetricSalt);
        var cypherText = File.ReadAllBytes(path);

        using (var cypher = new AesManaged())
        {
            var pdb = new Rfc2898DeriveBytes(password, salt);
            var key = pdb.GetBytes(cypher.KeySize / 8);
            var iv = pdb.GetBytes(cypher.BlockSize / 8);

            using (var decryptor = cypher.CreateDecryptor(key, iv))
            using (var msDecrypt = new MemoryStream(cypherText))
            using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
            using (var srDecrypt = new StreamReader(csDecrypt))
            {
                return srDecrypt.ReadToEnd();
            }
        }
    }
    
    public static void ExportPublicAndPrivateKeyToFile(string publicKeyPath, string privateKeyPath, RSAKey rsaKey, string password, string symmetricSalt = null)
    {
        ExportPublicKeyToFile(publicKeyPath, rsaKey.PublicKey);
        ExportPrivateKeyToFile(privateKeyPath, rsaKey.PrivateKey, password, symmetricSalt);
    }

    public static RSAKey ReadPublicAndPrivateKey(string publicKeyPath, string privateKeyPath, string password, string symmetricSalt = null)
    {
        var pub = ReadPublicKey(publicKeyPath);
        var priv = ReadPrivateKey(privateKeyPath, password, symmetricSalt);
        return new RSAKey { PublicKey = pub, PrivateKey = priv };
    }

    #endregion
}

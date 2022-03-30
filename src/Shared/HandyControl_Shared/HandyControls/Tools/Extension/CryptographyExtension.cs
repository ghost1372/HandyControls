namespace HandyControl.Tools.Extension;

public static class CryptographyExtension
{
    /// <summary>
    /// Generate MD5 Hash 
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string GenerateMD5(this string input) => CryptographyHelper.GenerateMD5(input);

    /// <summary>
    /// Generate SHA256 Hash
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string GenerateSHA256(this string input) => CryptographyHelper.GenerateSHA256(input);

    /// <summary>
    /// Encrypt string with AES
    /// </summary>
    /// <param name="input"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public static string EncryptStringAES(this string input, string password) => CryptographyHelper.EncryptStringAES(input, password);

    /// <summary>
    /// Decrypt string with AES
    /// </summary>
    /// <param name="input"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public static string DecryptStringAES(this string input, string password) => CryptographyHelper.DecryptStringAES(input, password);

    /// <summary>
    /// Encrypt string with Base64
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string EncryptStringBase64(this string input) => CryptographyHelper.EncryptStringBase64(input);

    /// <summary>
    /// Decrypt string with Base64
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string DecryptStringBase64(this string input) => CryptographyHelper.DecryptStringBase64(input);

    /// <summary>
    /// Encrypt string with RSA
    /// </summary>
    /// <param name="input"></param>
    /// <param name="publicKey"></param>
    /// <returns></returns>
    public static string EncryptStringRSA(this string input, string publicKey) => CryptographyHelper.EncryptStringRSA(input, publicKey);

    /// <summary>
    /// Decrypt string with RSA
    /// </summary>
    /// <param name="input"></param>
    /// <param name="privateKey"></param>
    /// <returns></returns>
    public static string DecryptStringRSA(this string input, string privateKey) => CryptographyHelper.DecryptStringRSA(input, privateKey);

}

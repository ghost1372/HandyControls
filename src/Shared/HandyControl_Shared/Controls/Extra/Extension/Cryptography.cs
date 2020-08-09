using HandyControl.Controls;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandyControl.Tools.Extension
{
    public static class Cryptography
    {
        public static string GenerateMD5(this string input) => CryptographyHelper.GenerateMD5(input);

        public static string GenerateSHA256(this string input) => CryptographyHelper.GenerateSHA256(input);
    }
}

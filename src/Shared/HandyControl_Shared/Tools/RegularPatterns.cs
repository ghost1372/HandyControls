﻿namespace HandyControl.Tools
{
    /// <summary>
    ///     包含一些正则验证所需要的字符串
    /// </summary>
    public sealed class RegularPatterns
    {
        /// <summary>
        ///     邮件正则匹配表达式
        /// </summary>
        public const string MailPattern =
            @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";

        /// <summary>
        ///     手机号正则匹配表达式
        /// </summary>
        public const string PhonePattern = @"^((13[0-9])|(15[^4,\d])|(18[0,5-9]))\d{8}$";

        /// <summary>
        ///     IP正则匹配
        /// </summary>
        public const string IpPattern =
            @"^(25[0-5]|2[0-4]\d|1\d{2}|[1-9]?\d)\."
            + @"(25[0-5]|2[0-4]\d|1\d{2}|[1-9]?\d)\."
            + @"(25[0-5]|2[0-4]\d|1\d{2}|[1-9]?\d)\."
            + @"(25[0-5]|2[0-4]\d|1\d{2}|[1-9]?\d)$";

        /// <summary>
        ///     A类IP正则匹配
        /// </summary>
        public const string IpAPattern =
            @"^(12[0-6]|1[0-1]\d|[1-9]?\d)\."
            + @"(25[0-5]|2[0-4]\d|1\d{2}|[1-9]?\d)\."
            + @"(25[0-5]|2[0-4]\d|1\d{2}|[1-9]?\d)\."
            + @"(25[0-5]|2[0-4]\d|1\d{2}|[1-9]?\d)$";

        /// <summary>
        ///     B类IP正则匹配
        /// </summary>
        public const string IpBPattern =
            @"^(19[0-1]|12[8-9]|1[3-8]\d)\."
            + @"(25[0-5]|2[0-4]\d|1\d{2}|[1-9]?\d)\."
            + @"(25[0-5]|2[0-4]\d|1\d{2}|[1-9]?\d)\."
            + @"(25[0-5]|2[0-4]\d|1\d{2}|[1-9]?\d)$";

        /// <summary>
        ///     C类IP正则匹配
        /// </summary>
        public const string IpCPattern =
            @"^(19[2-9]|22[0-3]|2[0-1]\d)\."
            + @"(25[0-5]|2[0-4]\d|1\d{2}|[1-9]?\d)\."
            + @"(25[0-5]|2[0-4]\d|1\d{2}|[1-9]?\d)\."
            + @"(25[0-5]|2[0-4]\d|1\d{2}|[1-9]?\d)$";

        /// <summary>
        ///     D类IP正则匹配
        /// </summary>
        public const string IpDPattern =
            @"^(22[4-9]|23\d\."
            + @"(25[0-5]|2[0-4]\d|1\d{2}|[1-9]?\d)\."
            + @"(25[0-5]|2[0-4]\d|1\d{2}|[1-9]?\d)\."
            + @"(25[0-5]|2[0-4]\d|1\d{2}|[1-9]?\d)$";

        /// <summary>
        ///     E类IP正则匹配
        /// </summary>
        public const string IpEPattern =
            @"^(25[0-5]|24\d\."
            + @"(25[0-5]|2[0-4]\d|1\d{2}|[1-9]?\d)\."
            + @"(25[0-5]|2[0-4]\d|1\d{2}|[1-9]?\d)\."
            + @"(25[0-5]|2[0-4]\d|1\d{2}|[1-9]?\d)$";

        /// <summary>
        ///     汉字正则匹配
        /// </summary>
        public const string ChinesePattern = @"^[\u4e00-\u9fa5]$";

        /// <summary>
        ///     Url正则匹配
        /// </summary>
        public const string UrlPattern =
            @"((http|ftp|https)://)(([a-zA-Z0-9\._-]+\.[a-zA-Z]{2,6})|([0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}))(:[0-9]{1,4})*(/[a-zA-Z0-9\&%_\./-~-]*)?";

        /// <summary>
        ///     数字正则匹配
        /// </summary>
        public const string NumberPattern = @"^\d$";

        /// <summary>
        ///     计算性质数字正则匹配
        /// </summary>
        public const string DigitsPattern = @"^[+-]?([0-9]*\.?[0-9]+|[0-9]+\.?[0-9]*)([eE][+-]?[0-9]+)?$";

        /// <summary>
        ///     正整数正则匹配
        /// </summary>
        public const string PIntPattern = @"^[1-9]\d*$";

        /// <summary>
        ///     负整数正则匹配
        /// </summary>
        public const string NIntPattern = @"^-[1-9]\d*$ ";

        /// <summary>
        ///     整数正则匹配
        /// </summary>
        public const string IntPattern = @"^-?[1-9]\d*$";

        /// <summary>
        ///     非负整数正则匹配
        /// </summary>
        public const string NnIntPattern = @"^[1-9]\d*|0$";

        /// <summary>
        ///     非正整数正则匹配
        /// </summary>
        public const string NpIntPattern = @"^-[1-9]\d*|0$";

        /// <summary>
        ///     正浮点数正则匹配
        /// </summary>
        public const string PDoublePattern = @"^[1-9]\d*\.\d*|0\.\d*[1-9]\d*$";

        /// <summary>
        ///     负浮点数正则匹配
        /// </summary>
        public const string NDoublePattern = @"^-([1-9]\d*\.\d*|0\.\d*[1-9]\d*)$";

        /// <summary>
        ///     浮点数正则匹配
        /// </summary>
        public const string DoublePattern = @"^-?([1-9]\d*\.\d*|0\.\d*[1-9]\d*|0?\.0+|0)$";

        /// <summary>
        ///     非负浮点数正则匹配
        /// </summary>
        public const string NnDoublePattern = @"^[1-9]\d*\.\d*|0\.\d*[1-9]\d*|0?\.0+|0$";

        /// <summary>
        ///     非正浮点数正则匹配
        /// </summary>
        public const string NpDoublePattern = @"^(-([1-9]\d*\.\d*|0\.\d*[1-9]\d*))|0?\.0+|0$";

        /// <summary>
        ///     شناسایی حروف فارسی
        /// </summary>
        public const string PersianPattern = @"^([\u0600-\u0605 ؐ-ؚ\u061Cـ ۖ-\u06DD ۟-ۤ ۧ ۨ ۪-ۭ ً-ٕ ٟ ٖ-ٞ ٰ ، ؍ ٫ ٬ ؛ ؞ ؟ ۔ ٭ ٪ ؉ ؊ ؈ ؎ ؏
۞ ۩ ؆ ؇ ؋  0 1 2 3 4 5 6 7 8 9 ٠ ۰ ١ ۱ ٢ ۲ ٣ ۳ ٤ ۴ ٥ ۵ ٦ ۶ ٧ ۷ ٨ ۸ ٩ ۹ ءٴ۽ آ أ ٲ ٱ ؤ إ ٳ ئ ا ٵ ٮ ب ٻ پ ڀ
ة-ث ٹ ٺ ټ ٽ ٿ ج ڃ ڄ چ ڿ ڇ ح خ ځ ڂ څ د ذ ڈ-ڐ ۮ ر ز ڑ-ڙ ۯ س ش ښ-ڜ ۺ ص ض ڝ ڞ
ۻ ط ظ ڟ ع غ ڠ ۼ ف ڡ-ڦ ٯ ق ڧ ڨ ك ک-ڴ ػ ؼ ل ڵ-ڸ م۾ ن ں-ڽ ڹ ه ھ ہ-ۃ ۿ ەۀ وۥ ٶ
ۄ-ۇ ٷ ۈ-ۋ ۏ ى يۦ ٸ ی-ێ ې ۑ ؽ-ؿ ؠ ے ۓ \u061D])+$";

        /// <summary>
        ///     شناسایی کد ملی
        /// </summary>
        public const string IranNationalCodePattern = @"^(?!(\d)\1{9})\d{10}$";

        /// <summary>
        ///     根据属性名称使用反射来获取值
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public object GetValue(string propertyName) => GetType().GetField(propertyName).GetValue(null);
    }
}
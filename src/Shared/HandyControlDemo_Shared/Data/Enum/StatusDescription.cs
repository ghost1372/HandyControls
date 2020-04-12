using HandyControl.Tools.Converter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace HandyControlDemo.Data
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum StatusDescription
    {
        [Description("This is horrible")]
        Horrible,
        [Description("This is bad")]
        Bad,
        [Description("This is so so")]
        SoSo,
        [Description("This is good")]
        Good,
        [Description("This is better")]
        Better,
        [Description("This is best")]
        Best
    }
}

// https://github.com/DingpingZhang/WpfExtensions

using System;
using System.Windows.Data;

namespace HandyControl.Tools
{
    public abstract class MultiBindingExtensionBase : MultiBinding
    {
        public new IMultiValueConverter Converter
        {
            get => base.Converter;
            protected set
            {
                if (base.Converter != null)
                {
                    throw new InvalidOperationException($"The {GetType().Name}.Converter property is readonly.");
                }

                base.Converter = value;
            }
        }
    }
}

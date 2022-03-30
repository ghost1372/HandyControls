using System;
using System.Collections.Generic;
using System.Windows.Markup;

namespace HandyControl.Controls;

public class EnumBindingSourceExtension : MarkupExtension
{
    bool _isDescription = false;
    private Type _enumType;
    private string[] _description;

    public EnumBindingSourceExtension() { }

    public EnumBindingSourceExtension(Type enumType)
    {
        EnumType = enumType;
        _isDescription = false;
    }

    public EnumBindingSourceExtension(Type enumType, string[] description)
    {
        EnumType = enumType;
        Description = description;
        _isDescription = true;
    }

    public Type EnumType
    {
        get => _enumType;
        set
        {
            if (value != _enumType)
            {
                if (null != value)
                {
                    Type enumType = Nullable.GetUnderlyingType(value) ?? value;

                    if (!enumType.IsEnum)
                    {
                        throw new ArgumentException("Type must be for an Enum.");
                    }
                }

                _enumType = value;
            }
        }
    }

    public string[] Description
    {
        get => _description;
        set
        {
            if (value != _description)
            {
                _description = value;
            }
        }
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        if (null == _enumType)
        {
            throw new InvalidOperationException("The EnumType must be specified.");
        }

        Type actualEnumType = Nullable.GetUnderlyingType(_enumType) ?? _enumType;
        Array enumValues = Enum.GetValues(actualEnumType);


        if (_isDescription)
        {
            Dictionary<object, string> dic = new Dictionary<object, string>();
            int index = 0;
            foreach (var item in enumValues)
            {
                if (index < Description.Length)
                {
                    dic.Add(item, Description[index]);
                }
                else
                {
                    dic.Add(item, item.ToString());

                }
                index += 1;
            }

            if (actualEnumType == _enumType)
            {
                return dic;
            }
        }
        else
        {
            if (actualEnumType == _enumType)
            {
                return enumValues;
            }

            Array tempArray = Array.CreateInstance(actualEnumType, enumValues.Length + 1);
            enumValues.CopyTo(tempArray, 1);
            return tempArray;
        }

        return null;
    }
}

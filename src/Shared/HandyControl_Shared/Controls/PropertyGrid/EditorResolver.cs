using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace HandyControl.Controls
{
    public static class EditorResolver
    {
        public static readonly Dictionary<Type, PropertyEditorBase> TypeEditorsDictionary = new()
        {
            [typeof(string)] = new PlainTextPropertyEditor(),
            [typeof(sbyte)] = new NumberPropertyEditor(sbyte.MinValue, sbyte.MaxValue),
            [typeof(byte)] = new NumberPropertyEditor(byte.MinValue, byte.MaxValue),
            [typeof(short)] = new NumberPropertyEditor(short.MinValue, short.MaxValue),
            [typeof(ushort)] = new NumberPropertyEditor(ushort.MinValue, ushort.MaxValue),
            [typeof(int)] = new NumberPropertyEditor(int.MinValue, int.MaxValue),
            [typeof(uint)] = new NumberPropertyEditor(uint.MinValue, uint.MaxValue),
            [typeof(long)] = new NumberPropertyEditor(long.MinValue, long.MaxValue),
            [typeof(ulong)] = new NumberPropertyEditor(ulong.MinValue, ulong.MaxValue),
            [typeof(float)] = new NumberPropertyEditor(float.MinValue, float.MaxValue),
            [typeof(double)] = new NumberPropertyEditor(double.MinValue, double.MaxValue),
            [typeof(bool)] = new SwitchPropertyEditor(),
            [typeof(DateTime)] = new DateTimePropertyEditor(),
            [typeof(HorizontalAlignment)] = new HorizontalAlignmentPropertyEditor(),
            [typeof(VerticalAlignment)] = new VerticalAlignmentPropertyEditor(),
            [typeof(ImageSource)] = new ImagePropertyEditor(),
            [typeof(Brush)] = new ColorPropertyEditor(),
            [typeof(object)] = new PlainTextPropertyEditor(),
            [typeof(Thickness)] = new PlainTextPropertyEditor(),
            [typeof(CornerRadius)] = new PlainTextPropertyEditor(),
        };
    }
}


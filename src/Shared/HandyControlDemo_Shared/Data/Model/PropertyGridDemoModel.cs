using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace HandyControlDemo.Data;

public class PropertyGridDemoModel
{
    [Category("Category1")]
    public string String { get; set; }

    [Category("Category2")]
    public int Integer { get; set; }

    [Category("Category2")]
    public bool Boolean { get; set; }

    [Category("Category1")]
    public Gender Enum { get; set; }

    [Category("Category1")]
    public Brush Brush { get; set; }
    [Category("Category1")]
    public Brush Brush1 { get; set; }

    [Category("Category3")]
    public object Object { get; set; }
    [Category("Category3")]
    public Thickness Thickness { get; set; }
    [Category("Category3")]
    public CornerRadius CornerRadius { get; set; }


    public HorizontalAlignment HorizontalAlignment { get; set; }

    public VerticalAlignment VerticalAlignment { get; set; }

    public ImageSource ImageSource { get; set; }
}

public enum Gender
{
    Male,
    Female
}

using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace HandyControl.Controls;

public partial class DataGridAttach
{
    internal static readonly DependencyProperty IsSelectAllProperty =
        DependencyProperty.RegisterAttached("IsSelectAll", typeof(bool), typeof(DataGridAttach), new PropertyMetadata(false,
            (o, e) =>
            {
                var dg = GetCurrentDataGrid(o);
                ToggleButton tg = o as ToggleButton;

                if (dg.SelectionMode != DataGridSelectionMode.Single)
                {
                    if (dg.SelectedItems.Count < 2)
                    {
                        tg.IsChecked = true;
                    }

                    if (tg.IsChecked == true)
                    {
                        dg.SelectAll();
                    }
                    else
                    {
                        dg.UnselectAll();
                    }
                }

            }));
    internal static bool GetIsSelectAll(DependencyObject obj)
    {
        return (bool) obj.GetValue(IsSelectAllProperty);
    }

    internal static void SetIsSelectAll(DependencyObject obj, bool value)
    {
        obj.SetValue(IsSelectAllProperty, value);
    }

    internal static readonly DependencyProperty CurrentDataGridProperty =
        DependencyProperty.RegisterAttached("CurrentDataGrid", typeof(DataGrid), typeof(DataGridAttach), new PropertyMetadata(null));

    internal static DataGrid GetCurrentDataGrid(DependencyObject obj)
    {
        return (DataGrid) obj.GetValue(CurrentDataGridProperty);
    }

    internal static void SetCurrentDataGrid(DependencyObject obj, DataGrid value)
    {
        obj.SetValue(CurrentDataGridProperty, value);
    }
}

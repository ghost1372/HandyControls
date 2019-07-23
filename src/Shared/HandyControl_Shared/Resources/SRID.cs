//---------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation.  All rights reserved.
//
//---------------------------------------------------------------------------

namespace Microsoft.Windows.Controls
{
    // A wrapper around string identifiers.
    internal struct SRID
    {
        private readonly string _string;

        public string String => _string;

        private SRID(string s)
        {
            _string = s;
        }

        public static SRID DataGrid_SelectAllCommandText => new SRID("DataGrid_SelectAllCommandText");

        public static SRID DataGrid_SelectAllKey => new SRID("DataGrid_SelectAllKey");

        public static SRID DataGrid_SelectAllKeyDisplayString => new SRID("DataGrid_SelectAllKeyDisplayString");

        public static SRID DataGrid_BeginEditCommandText => new SRID("DataGrid_BeginEditCommandText");

        public static SRID DataGrid_CommitEditCommandText => new SRID("DataGrid_CommitEditCommandText");

        public static SRID DataGrid_CancelEditCommandText => new SRID("DataGrid_CancelEditCommandText");

        public static SRID DataGrid_DeleteCommandText => new SRID("DataGrid_DeleteCommandText");

        public static SRID DataGridCellItemAutomationPeer_NameCoreFormat => new SRID("DataGridCellItemAutomationPeer_NameCoreFormat");

        public static SRID CalendarAutomationPeer_CalendarButtonLocalizedControlType => new SRID("CalendarAutomationPeer_CalendarButtonLocalizedControlType");

        public static SRID CalendarAutomationPeer_DayButtonLocalizedControlType => new SRID("CalendarAutomationPeer_DayButtonLocalizedControlType");

        public static SRID CalendarAutomationPeer_BlackoutDayHelpText => new SRID("CalendarAutomationPeer_BlackoutDayHelpText");

        public static SRID Calendar_NextButtonName => new SRID("Calendar_NextButtonName");

        public static SRID Calendar_PreviousButtonName => new SRID("Calendar_PreviousButtonName");

        public static SRID DatePickerAutomationPeer_LocalizedControlType => new SRID("DatePickerAutomationPeer_LocalizedControlType");

        public static SRID DatePickerTextBox_DefaultWatermarkText => new SRID("DatePickerTextBox_DefaultWatermarkText");

        public static SRID DatePicker_DropDownButtonName => new SRID("DatePicker_DropDownButtonName");

        public static SRID DataGrid_ColumnIndexOutOfRange => new SRID("DataGrid_ColumnIndexOutOfRange");

        public static SRID DataGrid_ColumnDisplayIndexOutOfRange => new SRID("DataGrid_ColumnDisplayIndexOutOfRange");

        public static SRID DataGrid_DisplayIndexOutOfRange => new SRID("DataGrid_DisplayIndexOutOfRange");

        public static SRID DataGrid_InvalidColumnReuse => new SRID("DataGrid_InvalidColumnReuse");

        public static SRID DataGrid_DuplicateDisplayIndex => new SRID("DataGrid_DuplicateDisplayIndex");

        public static SRID DataGrid_NewColumnInvalidDisplayIndex => new SRID("DataGrid_NewColumnInvalidDisplayIndex");

        public static SRID DataGrid_NullColumn => new SRID("DataGrid_NullColumn");

        public static SRID DataGrid_ReadonlyCellsItemsSource => new SRID("DataGrid_ReadonlyCellsItemsSource");

        public static SRID DataGrid_InvalidSortDescription => new SRID("DataGrid_InvalidSortDescription");

        public static SRID DataGrid_ProbableInvalidSortDescription => new SRID("DataGrid_ProbableInvalidSortDescription");

        public static SRID DataGridLength_InvalidType => new SRID("DataGridLength_InvalidType");

        public static SRID DataGridLength_Infinity => new SRID("DataGridLength_Infinity");

        public static SRID DataGrid_CannotSelectCell => new SRID("DataGrid_CannotSelectCell");

        public static SRID DataGridRow_CannotSelectRowWhenCells => new SRID("DataGridRow_CannotSelectRowWhenCells");

        public static SRID DataGrid_AutomationInvokeFailed => new SRID("DataGrid_AutomationInvokeFailed");

        public static SRID SelectedCellsCollection_InvalidItem => new SRID("SelectedCellsCollection_InvalidItem");

        public static SRID SelectedCellsCollection_DuplicateItem => new SRID("SelectedCellsCollection_DuplicateItem");

        public static SRID VirtualizedCellInfoCollection_IsReadOnly => new SRID("VirtualizedCellInfoCollection_IsReadOnly");

        public static SRID VirtualizedCellInfoCollection_DoesNotSupportIndexChanges => new SRID("VirtualizedCellInfoCollection_DoesNotSupportIndexChanges");

        public static SRID ClipboardCopyMode_Disabled => new SRID("ClipboardCopyMode_Disabled");

        public static SRID Calendar_OnDisplayModePropertyChanged_InvalidValue => new SRID("Calendar_OnDisplayModePropertyChanged_InvalidValue");

        public static SRID Calendar_OnFirstDayOfWeekChanged_InvalidValue => new SRID("Calendar_OnFirstDayOfWeekChanged_InvalidValue");

        public static SRID Calendar_OnSelectedDateChanged_InvalidValue => new SRID("Calendar_OnSelectedDateChanged_InvalidValue");

        public static SRID Calendar_OnSelectedDateChanged_InvalidOperation => new SRID("Calendar_OnSelectedDateChanged_InvalidOperation");

        public static SRID CalendarCollection_MultiThreadedCollectionChangeNotSupported => new SRID("CalendarCollection_MultiThreadedCollectionChangeNotSupported");

        public static SRID Calendar_CheckSelectionMode_InvalidOperation => new SRID("Calendar_CheckSelectionMode_InvalidOperation");

        public static SRID Calendar_OnSelectionModeChanged_InvalidValue => new SRID("Calendar_OnSelectionModeChanged_InvalidValue");

        public static SRID Calendar_UnSelectableDates => new SRID("Calendar_UnSelectableDates");

        public static SRID DatePickerTextBox_TemplatePartIsOfIncorrectType => new SRID("DatePickerTextBox_TemplatePartIsOfIncorrectType");

        public static SRID DatePicker_OnSelectedDateFormatChanged_InvalidValue => new SRID("DatePicker_OnSelectedDateFormatChanged_InvalidValue");

        public static SRID DatePicker_WatermarkText => new SRID("DatePicker_WatermarkText");

        public static SRID CalendarAutomationPeer_MonthMode => new SRID("CalendarAutomationPeer_MonthMode");

        public static SRID CalendarAutomationPeer_YearMode => new SRID("CalendarAutomationPeer_YearMode");

        public static SRID CalendarAutomationPeer_DecadeMode => new SRID("CalendarAutomationPeer_DecadeMode");
    }
}

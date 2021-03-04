//---------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation.  All rights reserved.
//
//---------------------------------------------------------------------------

namespace Microsoft.Windows.Controls
{
    /// <summary>
    /// Specifies values for the different modes of operation of a PersianCalendar. 
    /// </summary>
    public enum CalendarMode
    {
        /// <summary>
        /// The PersianCalendar displays a month at a time.
        /// </summary>
        Month = 0,

        /// <summary>
        ///  The PersianCalendar displays a year at a time.
        /// </summary>
        Year = 1,
        
        /// <summary>
        /// The PersianCalendar displays a decade at a time.
        /// </summary>
        Decade = 2,
    }
}
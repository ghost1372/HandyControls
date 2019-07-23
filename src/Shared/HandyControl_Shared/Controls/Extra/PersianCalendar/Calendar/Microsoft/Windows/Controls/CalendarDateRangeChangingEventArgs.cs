//---------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation.  All rights reserved.
//
//---------------------------------------------------------------------------

using System;

namespace Microsoft.Windows.Controls
{
    /// <summary>
    /// Event arguments to notify clients that the range is changing and what the new range will be
    /// </summary>
    internal class CalendarDateRangeChangingEventArgs : EventArgs
    {
        public CalendarDateRangeChangingEventArgs(DateTime start, DateTime end)
        {
            _start = start;
            _end = end;
        }

        public DateTime Start => _start;

        public DateTime End => _end;

        private readonly DateTime _start;
        private readonly DateTime _end;
    }
}
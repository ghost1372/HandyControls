//---------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation.  All rights reserved.
//
//---------------------------------------------------------------------------

using System;

namespace Microsoft.Windows.Controls
{
    /// <summary>
    /// Provides data for the DateValidationError event.
    /// </summary>
    public class DatePickerDateValidationErrorEventArgs : EventArgs
    {
        private bool _throwException;

        /// <summary>
        /// Initializes a new instance of the DatePickerDateValidationErrorEventArgs class. 
        /// </summary>
        /// <param name="exception">The exception that initially triggered the DateValidationError event.</param>
        /// <param name="text">The text being parsed.</param>
        public DatePickerDateValidationErrorEventArgs(Exception exception, string text)
        {
            Text = text;
            Exception = exception;
        }

        /// <summary>
        /// Gets the exception that initially triggered the DateValidationError event.
        /// </summary>
        public Exception Exception
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the text being parsed when the DateValidationError event was raised.
        /// </summary>
        public string Text
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets a value that indicates whether Exception should be thrown.
        /// </summary>
        public bool ThrowException
        {
            get => _throwException;

            set => _throwException = value;
        }
    }
}

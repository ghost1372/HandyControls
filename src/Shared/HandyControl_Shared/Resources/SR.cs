//---------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation.  All rights reserved.
//
//---------------------------------------------------------------------------

using System.Globalization;
using System.Resources;

namespace Microsoft.Windows.Controls
{
    /// <summary>
    ///     Retrieves exception strings and other localized strings.
    /// </summary>
    internal static class SR
    {
        internal static string Get(SRID id)
        {
            return _resourceManager.GetString(id.String);
        }

        internal static string Get(SRID id, params object[] args)
        {
            string message = _resourceManager.GetString(id.String);
            if (message != null)
            {
                // Apply arguments to formatted string (if applicable)
                if (args != null && args.Length > 0)
                {
                    message = string.Format(CultureInfo.CurrentCulture, message, args);
                }
            }

            return message;
        }

        // Get exception string resources for current locale
        private static readonly ResourceManager _resourceManager = new ResourceManager("ExceptionStringTable", typeof(SR).Assembly);
    }
}

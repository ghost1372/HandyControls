#region Copyright information
// <copyright file="INestedMarkupExtension.cs">
//     Licensed under Microsoft Public License (Ms-PL)
//     https://github.com/XAMLMarkupExtensions/XAMLMarkupExtensions/blob/master/LICENSE
// </copyright>
// <author>Uwe Mayer</author>
#endregion

namespace HandyControl.Tools.DynamicLanguage
{
    #region Uses
    using System.Collections.Generic;
    #endregion

    /// <summary>
    /// Markup extensions that implement this interface shall be able to return their target objects.
    /// They should also implement a SetNewValue function that properly set the value to all their targets with their own modification of the value.
    /// </summary>
    public interface INestedMarkupExtension
    {
        /// <summary>
        /// Get the paths to all target properties through the nesting hierarchy.
        /// </summary>
        /// <returns>A list of combinations of property types and the corresponsing stacks that resemble the paths to the properties.</returns>
        List<TargetPath> GetTargetPropertyPaths();

        /// <summary>
        /// Trigger the update of the target(s).
        /// </summary>
        /// <param name="targetPath">A specific path to follow or null for all targets.</param>
        /// <returns>The output of the path at the endpoint.</returns>
        object UpdateNewValue(TargetPath targetPath);

        /// <summary>
        /// Format the output of the markup extension.
        /// </summary>
        /// <param name="endpoint">Information about the endpoint.</param>
        /// <param name="info">Information about the target.</param>
        /// <returns>The output of this extension for the given endpoint and target.</returns>
        object FormatOutput(TargetInfo endpoint, TargetInfo info);

        /// <summary>
        /// Check, if the given target is connected to this markup extension.
        /// </summary>
        /// <param name="info">Information about the target.</param>
        /// <returns>True, if a connection exits.</returns>
        bool IsConnected(TargetInfo info);
    }
}

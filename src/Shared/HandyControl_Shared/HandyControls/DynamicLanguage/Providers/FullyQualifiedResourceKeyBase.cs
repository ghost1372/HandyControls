#region Copyright information
// <copyright file="FullyQualifiedResourceKeyBase.cs">
//     Licensed under Microsoft Public License (Ms-PL)
//     https://github.com/XAMLMarkupExtensions/WPFLocalizationExtension/blob/master/LICENSE
// </copyright>
// <author>Justin Pihony</author>
// <author>Uwe Mayer</author>
#endregion

namespace HandyControl.Tools.DynamicLanguage
{
    /// <summary>
    /// An abstract class for key identification.
    /// </summary>
    public abstract class FullyQualifiedResourceKeyBase
    {
        /// <summary>
        /// Implicit string operator.
        /// </summary>
        /// <param name="fullyQualifiedResourceKey">The object.</param>
        /// <returns>The joined version of the assembly, dictionary and key.</returns>
        public static implicit operator string(FullyQualifiedResourceKeyBase fullyQualifiedResourceKey)
        {
            return fullyQualifiedResourceKey?.ToString();
        }
    }
}
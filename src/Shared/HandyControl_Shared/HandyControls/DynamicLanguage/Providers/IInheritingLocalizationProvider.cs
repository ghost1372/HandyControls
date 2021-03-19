#region Copyright information
// <copyright file="ILocalizationProvider.cs">
//     Licensed under Microsoft Public License (Ms-PL)
//     https://github.com/XAMLMarkupExtensions/WPFLocalizationExtension/blob/master/LICENSE
// </copyright>
// <author>Uwe Mayer</author>
#endregion

namespace HandyControl.Tools.DynamicLanguage
{
    /// <summary>
    /// An interface describing classes that provide localized values based on a source/dictionary/key combination.
    /// and used for a localization provider that uses Inheriting Dependency Properties
    /// </summary>
    internal interface IInheritingLocalizationProvider: ILocalizationProvider
    {
    
    }
}

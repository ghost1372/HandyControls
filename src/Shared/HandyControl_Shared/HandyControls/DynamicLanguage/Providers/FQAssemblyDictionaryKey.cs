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
    #region Usings
    using System.Linq;
    #endregion

    /// <summary>
    /// A class that bundles the key, assembly and dictionary information.
    /// </summary>
    public class FQAssemblyDictionaryKey : FullyQualifiedResourceKeyBase
    {
        private readonly string _key;
        /// <summary>
        /// The key.
        /// </summary>
        public string Key => _key;

        private readonly string _assembly;
        /// <summary>
        /// The assembly of the dictionary.
        /// </summary>
        public string Assembly => _assembly;

        private readonly string _dictionary;
        /// <summary>
        /// The resource dictionary.
        /// </summary>
        public string Dictionary => _dictionary;

        /// <summary>
        /// Creates a new instance of <see cref="FullyQualifiedResourceKeyBase"/>.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="assembly">The assembly of the dictionary.</param>
        /// <param name="dictionary">The resource dictionary.</param>
        public FQAssemblyDictionaryKey(string key, string assembly = null, string dictionary = null)
        {
            _key = key;
            _assembly = assembly;
            _dictionary = dictionary;
        }

        /// <summary>
        /// Converts the object to a string.
        /// </summary>
        /// <returns>The joined version of the assembly, dictionary and key.</returns>
        public 
            override string ToString()
        {
            return string.Join(":", new[] { Assembly, Dictionary, Key }.Where(x => !string.IsNullOrEmpty(x)).ToArray());
        }
    }
}

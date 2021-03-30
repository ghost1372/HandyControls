using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace HandyControl.Tools
{
    public sealed class ResxLocalizationProvider : ILocalizationProvider
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly ResourceManager resourceManager;

        public ResxLocalizationProvider(string baseName, Assembly assembly)
        {
            this.resourceManager = new ResourceManager(baseName, assembly);

            try
            {
                this.resourceManager.GetString(string.Empty);
            }
            catch (MissingManifestResourceException)
            {
                throw new MissingManifestResourceException(string.Format("Could not find any resources. Make sure \"{0}.resources\" was correctly embedded or linked into assembly \"{1}\" at compile time, or that all the satellite assemblies required are loadable and fully signed.", baseName, assembly.GetName().Name));
            }
        }

        public string Localize(string key, CultureInfo culture)
        {
            return this.resourceManager.GetString(key, culture);
        }
    }
}

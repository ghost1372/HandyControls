using HandyControl.Tools.Extension;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace HandyControl.Tools;

/// <summary>
/// Implementing a localized string provider through application resources
/// </summary>
public class ResxLocalizationProvider : ILocalizationProvider
{

    private readonly ResourceManager resourceManager;

    public ResxLocalizationProvider(string baseName, Assembly assembly)
    {
        this.resourceManager = new ResourceManager(baseName, assembly);
        LocalizationManager.AvailableResourceManager.AddIfNotExists(baseName, resourceManager);

        try
        {
            // we only do this call to validate the given baseName
            this.resourceManager.GetString(string.Empty);
        }
        catch (MissingManifestResourceException)
        {
            // this exception is thrown if the path is completely wrong
            // let's improve the exception message a bit
            throw new MissingManifestResourceException(string.Format("Could not find any resources. Make sure \"{0}.resources\" was correctly embedded or linked into assembly \"{1}\" at compile time, or that all the satellite assemblies required are loadable and fully signed.", baseName, assembly.GetName().Name));
        }
    }
    
    public object Localize(string key, CultureInfo cultureInfo)
{
        return this.resourceManager.GetString(key, cultureInfo);
    }
}

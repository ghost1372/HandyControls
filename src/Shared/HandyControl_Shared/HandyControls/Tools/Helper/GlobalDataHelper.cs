#if NETCOREAPP
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HandyControl.Tools;

public abstract class GlobalDataHelper
{
    private const string FileVersionKey = "FileVersion";

    [JsonIgnore]
    public abstract string FileName { get; set; }

    [JsonIgnore]
    public abstract JsonSerializerOptions JsonSerializerOptions { get; set; }

    [JsonIgnore]
    public abstract int FileVersion { get; set; }

    public GlobalDataHelper()
    {
        if (string.IsNullOrEmpty(FileName))
        {
            FileName = "AppConfig.json";
        }
        else
        {
            CreateDirectory(FileName);
        }

        if (FileVersion != 0)
        {
            if (!FileVersion.Equals(RegistryHelper.GetValue<int>(FileVersionKey, Path.GetFileNameWithoutExtension(ApplicationHelper.GetExecutablePathNative()))))
            {
                if (File.Exists(FileName))
                {
                    File.Delete(FileName);
                }
                RegistryHelper.AddOrUpdateKey(FileVersionKey, Path.GetFileNameWithoutExtension(ApplicationHelper.GetExecutablePathNative()), FileVersion);
            }
        }
    }
    public static T Load<T>() where T : GlobalDataHelper, new()
    {
        T result = new T();
        result = JsonFile.Load<T>(result.FileName) ?? result;
        return result;
    }
    public async static Task<T> LoadAsync<T>() where T : GlobalDataHelper, new()
    {
        T result = new T();
        result = await JsonFile.LoadAsync<T>(result.FileName) ?? result;
        return result;
    }
    public void Save()
    {
        JsonFile.Save(FileName, this, JsonSerializerOptions);
    }
    public async Task SaveAsync()
    {
        await JsonFile.SaveAsync(FileName, this, JsonSerializerOptions);
    }

    public void CreateDirectory(string path)
    {
        var dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
    }
}
internal static class JsonFile
{
    public static void Save<T>(string fileName, T @object, JsonSerializerOptions options = null)
    {
        using (StreamWriter writer = File.CreateText(fileName))
        {
            if (options == null)
            {
                options = new JsonSerializerOptions();
                options.WriteIndented = true;
                options.IgnoreNullValues = true;
            }
            options.Converters.Add(new PolymorphicJsonConverter<T>());
            string json = JsonSerializer.Serialize(@object, options);
            writer.Write(json);
        }
    }

    public async static Task SaveAsync<T>(string fileName, T @object, JsonSerializerOptions options = null)
    {
        if (options == null)
        {
            options = new JsonSerializerOptions();
            options.WriteIndented = true;
            options.IgnoreNullValues = true;
        }
        options.Converters.Add(new PolymorphicJsonConverter<T>());
        using FileStream createStream = File.Create(fileName);
        await JsonSerializer.SerializeAsync(createStream, @object, options);
    }

    public static T Load<T>(string fileName)
    {
        if (File.Exists(fileName))
        {
            using (StreamReader reader = File.OpenText(fileName))
            {
                string json = reader.ReadToEnd();
                if (!string.IsNullOrEmpty(json))
                {
                    return JsonSerializer.Deserialize<T>(json);
                }
                else
                {
                    return default(T);
                }
            }
        }
        else
        {
            return default(T);
        }
    }

    public async static Task<T> LoadAsync<T>(string fileName)
    {
        if (File.Exists(fileName))
        {
            using FileStream openStream = File.OpenRead(fileName);
            if (openStream.Length > 0)
            {
                return await JsonSerializer.DeserializeAsync<T>(openStream);
            }
            else
            {
                return default(T);
            }
        }
        else
        {
            return default(T);
        }
    }
}
#endif

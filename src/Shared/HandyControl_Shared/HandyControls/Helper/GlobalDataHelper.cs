#if NETCOREAPP
using System.IO;
using System.Text.Json;

namespace HandyControl.Tools
{
    public abstract class GlobalDataHelper<T> where T : GlobalDataHelper<T>, new()
    {
        public static T Config { get; set; }
        private static string _filename { get; set; }

        /// <summary>
        /// This function will load settings object.
        /// </summary>
        /// <param name="FileName">config file location with filename</param>
        public static void Init(string FileName = "AppConfig.json")
        {
            _filename = FileName;
            if (File.Exists(FileName))
            {
                string json = File.ReadAllText(FileName);

                Config = (string.IsNullOrEmpty(json) ? new T() : JsonSerializer.Deserialize<T>(json)) ?? new T();
            }
            else
            {
                Config = new T();
            }
        }

        public static void Save()
        {
            JsonSerializerOptions options = new JsonSerializerOptions { WriteIndented = true, IgnoreNullValues = true };

            string json = JsonSerializer.Serialize(Config, options);
            File.WriteAllText(_filename, json);
        }
    }
}
#endif

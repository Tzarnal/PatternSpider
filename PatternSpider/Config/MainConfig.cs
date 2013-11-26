using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace PatternSpider.Config
{
    class MainConfig
    {
        public static string ConfigPath = "Configuration/";
        public static string ConfigFileName = "MainConfig.json";
        public static string FullPath
        {
            get { return ConfigPath + ConfigFileName; }

        }

        public List<ServerConfig> Servers;
        public string CommandSymbol = "!";
        
        public MainConfig()
        {
            Servers = new List<ServerConfig>();
        }

        public void Save()
        {
            var data = JsonConvert.SerializeObject(this, Formatting.Indented);
            
            if (!Directory.Exists(ConfigPath))
            {
                Directory.CreateDirectory(ConfigPath);
            }

            File.WriteAllText(FullPath, data);
        }

        public static MainConfig Load()
        {
            var data = File.ReadAllText(FullPath);
            return JsonConvert.DeserializeObject<MainConfig>(data);
        }
    }
}

using System.IO;
using Newtonsoft.Json;

namespace Plugin_Sentience
{
    class Settings
    {
        public static string DataPath = "Plugins/Sentience/";
        public static string DataFileName = "Settings.json";
        public static string FullPath
        {
            get { return DataPath + DataFileName; }

        }

        public int WindowSize;
        public int LogSize;

        public Settings()
        {
            WindowSize = 3;
            LogSize = 6000000;
        }

        public void Save()
        {
            var data = JsonConvert.SerializeObject(this, Formatting.Indented);

            if (!Directory.Exists(DataPath))
            {
                Directory.CreateDirectory(DataPath);
            }

            File.WriteAllText(FullPath, data);
        }

        public static Settings Load()
        {
            var data = File.ReadAllText(FullPath);
            return JsonConvert.DeserializeObject<Settings>(data);
        }
    }
}

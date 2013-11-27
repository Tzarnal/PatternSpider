using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Plugin_Weather
{
    class UsersLocations
    {
        public static string DataPath = "Plugins/Weather/";
        public static string DataFileName = "UserLocations.json";
        public static string FullPath
        {
            get { return DataPath + DataFileName; }

        }

        public Dictionary<string, string> UserLocations;

        public UsersLocations()
        {
            UserLocations = new Dictionary<string, string>();
        }

        public void Save()
        {
            var data = JsonConvert.SerializeObject(this);

            if (!Directory.Exists(DataPath))
            {
                Directory.CreateDirectory(DataPath);
            }

            File.WriteAllText(FullPath, data);
        }

        public static UsersLocations Load()
        {
            var data = File.ReadAllText(FullPath);
            return JsonConvert.DeserializeObject<UsersLocations>(data);
        }
    }
}

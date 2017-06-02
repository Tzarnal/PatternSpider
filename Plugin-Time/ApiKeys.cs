using System.IO;
using Newtonsoft.Json;

namespace Plugin_Time
{
    class ApiKeys
    {
        public static string DataPath = "Plugins/Time/";
        public static string DataFileName = "APIKeys.json";
        public static string FullPath
        {
            get { return DataPath + DataFileName; }

        }

        public string TimeZoneKey;        
        public string MapQuestKey;

        public ApiKeys ()
        {
            TimeZoneKey = "Dummy - Key - 0123456";
            MapQuestKey = "Dummy-Key-0123456";
        }

        public void Save()
        {            
            var data = JsonConvert.SerializeObject(this,Formatting.Indented);

            if (!Directory.Exists(DataPath))
            {
                Directory.CreateDirectory(DataPath);
            }

            File.WriteAllText(FullPath, data);
        }

        public static ApiKeys Load()
        {
            var data = File.ReadAllText(FullPath);
            return JsonConvert.DeserializeObject<ApiKeys>(data);
        }
    }
}

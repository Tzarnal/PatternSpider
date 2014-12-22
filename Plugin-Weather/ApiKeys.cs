using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Plugin_Weather
{
    class ApiKeys
    {
        public static string DataPath = "Plugins/Weather/";
        public static string DataFileName = "APIKeys.json";
        public static string FullPath
        {
            get { return DataPath + DataFileName; }

        }

        public string ForecastIoKey;
        public string MapQuestKey;

        public ApiKeys ()
        {
            ForecastIoKey = "Dummy-Key-0123456";
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

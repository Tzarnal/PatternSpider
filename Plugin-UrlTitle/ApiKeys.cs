using System;
using System.IO;
using Newtonsoft.Json;


namespace Plugin_UrlTitle
{
    class ApiKeys
    {
        public static string DataPath = "Plugins/UrlTitle/";
        public static string DataFileName = "APIKeys.json";
        public static string FullPath
        {
            get { return DataPath + DataFileName; }

        }

        public string TwitterConsumerKey;
        public string TwitterConsumerSecretKey;

        public ApiKeys ()
        {
            TwitterConsumerKey = "Dummy-Key-0123456";
            TwitterConsumerSecretKey = "Dummy-Key-0123456";
        }

        public void Save()
        {            
            var data = JsonConvert.SerializeObject(this,Formatting.Indented);

            if (!Directory.Exists(DataPath))
            {
                Directory.CreateDirectory(DataPath);
            }

            try
            {
                File.WriteAllText(FullPath, data);
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not create Apikey.json for UrlTitle: " + e.Message);
            }
            
        }

        public static ApiKeys Load()
        {
            var data = File.ReadAllText(FullPath);
            return JsonConvert.DeserializeObject<ApiKeys>(data);
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Plugin_Seen
{
    class NickHistory
    {
        public static string DataPath = "Plugins/Seen/";
        public static string DataFileName = "Seen.json";
        private int _saveHeartbeat;

        public static string FullPath
        {
            get { return DataPath + DataFileName; }

        }

        public Dictionary<string, Dictionary<string,HistoryEntry>> HistoryByServer;

        public NickHistory()
        {
            HistoryByServer = new Dictionary<string, Dictionary<string, HistoryEntry>>();
        }

        public void Save()
        {
            var data = JsonConvert.SerializeObject(this);

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
                Console.WriteLine("Failed to save Nick History: " + e.Message);
            }            
        }

        public void SaveHeartbeat()
        {
            _saveHeartbeat++;
            if (_saveHeartbeat > 20)
            {
                Save();
                _saveHeartbeat = 0;
            }
        }

        public static NickHistory Load()
        {
            var data = File.ReadAllText(FullPath);
            return JsonConvert.DeserializeObject<NickHistory>(data);
        }
    }
}


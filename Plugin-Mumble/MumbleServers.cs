using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Plugin_Mumble
{
    class MumbleServers
    {
        public static string DataPath = "Plugins/Mumble/";
        public static string DataFileName = "Servers.json";

        public List<ServerEntry> Servers;

        public static string FullPath
        {
            get { return DataPath + DataFileName; }

        }

        public MumbleServers()
        {
            Servers = new List<ServerEntry>();
            Servers.Add(new ServerEntry {IRCServer = "irc.example.net",IrcChannel = "#xamplez",MumbleCVP = "https://example.com/servers/cvp.json"});
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
                Console.WriteLine("Failed to save Mumble Servers file: " + e.Message);
            }
        }

        public static MumbleServers Load()
        {
            var data = File.ReadAllText(FullPath);
            return JsonConvert.DeserializeObject<MumbleServers>(data);
        }
    }
}

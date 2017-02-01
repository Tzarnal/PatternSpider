using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Net;
using PatternSpider.Irc;
using PatternSpider.Plugins;


namespace Plugin_Mumble
{
    [Export(typeof(IPlugin))]
    public class Mumble : IPlugin
    {
        public string Name { get { return "Mumble"; } }
        public string Description { get { return "Gives information on mumble servers associated with a channel"; } }

        public List<string> Commands { get { return new List<string> { "mumble" }; } }

        private MumbleServers _servers;

        public Mumble()
        {
            if (File.Exists(MumbleServers.FullPath))
            {
                _servers = MumbleServers.Load();
            }
            else
            {
                _servers = new MumbleServers();
                _servers.Save();
            }
        }

        public List<string> IrcCommand(IrcBot ircBot, string server, IrcMessage m)
        {
            string channel = m.Channel;

            if (string.IsNullOrWhiteSpace(server))
            {
                return null;
            }

            var candidateServer =
                _servers.Servers.Where(entry => entry.IRCServer == server && entry.IrcChannel == channel).FirstOrDefault();

            if (candidateServer != null)
            {
                return GetMumbleData(candidateServer.MumbleCVP);                
            }

            return null;
        }

        public List<string> OnChannelMessage(IrcBot ircBot, string server, string channel, IrcMessage m)
        {
            return null;
        }

        public List<string> OnUserMessage(IrcBot ircBot, string server, IrcMessage m)
        {
            return null;
        }

        private String GetMumbleChannelUsers(List<User> users)
        {
            var userNames = new List<string>();

            foreach (var user in users)
            {
                userNames.Add(user.Name);
            }

            return String.Join(",", userNames);
        }

        private List<string> GetMumbleData(string cvpUrl)
        {
            string json;
            MumbleCVP mumbleData = new MumbleCVP();
            var message = new List<string>();

            try
            {
                using (WebClient wc = new WebClient())
                {
                    json = wc.DownloadString(cvpUrl);
                    mumbleData = MumbleCVP.Load(json);
                    message.Add($"{mumbleData.name} - {mumbleData.x_connecturl}");                    
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("Mumble Plugin Download Error: " + e.Message);
            }

            if (mumbleData.root.users.Count != 0)
            {
                message.Add($"{mumbleData.root.name} {GetMumbleChannelUsers(mumbleData.root.users)}");   
            }

            foreach (var channel in mumbleData.root.channels)
            {
                if (channel.users.Count != 0)
                {
                    message.Add($"{channel.name} - {GetMumbleChannelUsers(channel.users)}");
                }

                if (channel.channels.Count != 0)
                {
                    foreach (var subChannel in channel.channels)
                    {
                        if (subChannel.users.Count != 0)
                        {
                            message.Add($"{subChannel.name} - {GetMumbleChannelUsers(subChannel.users)}");
                        }
                    }
                }
            }

            return message;
        }
    }
}

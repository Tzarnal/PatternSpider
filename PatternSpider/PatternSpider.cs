using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using PatternSpider.Config;
using PatternSpider.Irc;
using PatternSpider.Plugins;

namespace PatternSpider
{
    class PatternSpider
    {
        private MainConfig _configuration;
        private Dictionary<IrcBot, ServerConfig> _connections;
        private PluginManager _pluginManager;

        public PatternSpider()
        {
            _connections = new Dictionary<IrcBot, ServerConfig>();
            _pluginManager = new PluginManager();
            LoadConfiguration();
        }

        public void Run()
        {                       
            Connect(_configuration.Servers);
        }

        public void Quit(bool saveConfig = false)
        {
            if(saveConfig)
                _configuration.Save();

            foreach (KeyValuePair<IrcBot, ServerConfig> pair in _connections)
            {
                pair.Key.Disconnect();
                pair.Key.Stop();                
            }           
        }

        private void Connect(IEnumerable<ServerConfig> serverConfigs)
        {
            foreach (var serverConfig in serverConfigs)
            {
                Connect(serverConfig);
            }
        }

        private void Connect(ServerConfig serverConfig)
        {
            var ircUser = new IrcUser
                {
                    Nickname = serverConfig.NickName,
                    Username = serverConfig.NickName,
                    Realname = serverConfig.RealName,
                    NickServPassword = serverConfig.NickservPassword
                };

            var connection = new IrcBot(ircUser);

            connection.OnChannelMessage += ChannelMessage;
            connection.OnUserMessage += UserMessage;

            connection.Connect(serverConfig.Address);
            connection.Join(serverConfig.Channels);

            _connections.Add(connection, serverConfig);

            connection.Run();
        }

        private void SendReplies(IEnumerable<String> replies, IrcBot server,string user, object source)
        {
            if (replies != null)
            {
                foreach (var reply in replies)
                {
                    SendReply(reply, server, user, source);
                }
            }
        }

        private void SendReply(string reply, IrcBot server,string user, object source)
        {
            server.SendMessage(user, reply);
            
        }

        private void ChannelMessage(object source, IrcBot ircBot, IrcMessage m)
        {                                          
            if(string.IsNullOrWhiteSpace(m.Text) )
            {
                return;
            }

            var serverConfig = _connections[ircBot];
            var servername = serverConfig.Address;
            var firstWord = m.TextArray[0].ToLower();
            var channelName = m.Channel;
            
            if(serverConfig.ActivePlugins == null)
            {
                return;
            }

            var relevantPlugins = _pluginManager.Plugins.Where(plugin => serverConfig.ActivePlugins.Contains(plugin.Name));

            foreach (var plugin in relevantPlugins)
            {

                SendReplies(plugin.OnChannelMessage(ircBot, servername, channelName, m), ircBot, m.Channel, source);

                if (firstWord[0].ToString(CultureInfo.InvariantCulture) == _configuration.CommandSymbol)
                {
                    var command = firstWord.Substring(1);
                    if (plugin.Commands.Contains(command))
                    {
                        SendReplies(plugin.IrcCommand(ircBot, servername, m), ircBot, m.Channel, source);
                    }
                }
            }          
        }

        private void UserMessage(object source, IrcBot ircBot, IrcMessage m)
        {
            
            var serverConfig = _connections[ircBot];
            var servername = serverConfig.Address;
            var firstWord = m.TextArray[0].ToLower();

 
            if (serverConfig.ActivePlugins == null)
            {
                return;
            }

            var relevantPlugins = _pluginManager.Plugins.Where(plugin => serverConfig.ActivePlugins.Contains(plugin.Name));

            foreach (var plugin in relevantPlugins)
            {
                SendReplies(plugin.OnUserMessage(ircBot, servername, m), ircBot, m.Sender, source);

                if (firstWord[0].ToString(CultureInfo.InvariantCulture) == _configuration.CommandSymbol)
                {
                    var command = firstWord.Substring(1);
                    if (plugin.Commands.Contains(command))
                    {
                        SendReplies(plugin.IrcCommand(ircBot, servername, m), ircBot, m.Sender, source);
                    }
                }
            }        
        }

        private void LoadConfiguration()
        {

            if (File.Exists(MainConfig.FullPath))
            {
                _configuration = MainConfig.Load();
            }
            else
            {
                Console.WriteLine("Could not load {0}, creating new config file.", MainConfig.FullPath);                
                _configuration = new MainConfig();

                _configuration.Servers.Add(new ServerConfig
                {
                    Address = "irc.mmoirc.com",
                    Channels = new List<string>
                        {
                            "#bot",
                        },
                    NickName = "BetaSpider",
                    RealName = "PatternSpider"
                });

                _configuration.Save();
            }
        }
    }
}

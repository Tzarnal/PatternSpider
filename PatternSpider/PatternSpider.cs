using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using IrcDotNet;
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

        public void Quit()
        {
            _configuration.Save();

            foreach (KeyValuePair<IrcBot, ServerConfig> pair in _connections)
            {
                pair.Key.Disconnect();
                pair.Key.Stop();                
            }           
        }

        public void ReloadPlugins()
        {
            _pluginManager.ReloadPlugins();
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
            var connection = new IrcBot();
            var regInfo = new IrcUserRegistrationInfo()
                {
                    NickName = serverConfig.NickName,
                    RealName = serverConfig.RealName,
                    UserModes = new List<char> {'i'},
                    UserName = serverConfig.NickName
                };

            connection.OnChannelMessage += ChannelMessage;
            connection.OnUserMessage += UserMessage;

            connection.Connect(serverConfig.Address,regInfo);
            connection.Join(serverConfig.Channels);

            _connections.Add(connection, serverConfig);
        }

        private void ChannelMessage(object source, IrcBot ircBot, IrcMessageEventArgs e)
        {
            var serverConfig = _connections[ircBot];
            var firstWord = e.Text.Split(' ')[0];
            
            if(serverConfig.ActivePlugins == null)
            {
                return;
            }

            var relevantPlugins = _pluginManager.Plugins.Where(plugin => serverConfig.ActivePlugins.Contains(plugin.Name));

            foreach (var plugin in relevantPlugins)
            {
                plugin.OnChannelMessage(ircBot, e);

                if (firstWord[0].ToString(CultureInfo.InvariantCulture) == _configuration.CommandSymbol)
                {
                    var command = firstWord.Substring(1);
                    if (plugin.Commands.Contains(command))
                    {
                        plugin.IrcCommand(ircBot, e);
                    }
                }
            }           
        }

        private void UserMessage(object source, IrcBot ircBot, IrcMessageEventArgs e)
        {            
            var serverConfig = _connections[ircBot];
            var firstWord = e.Text.Split(' ')[0];

            if (serverConfig.ActivePlugins == null)
            {
                return;
            }

            var relevantPlugins = _pluginManager.Plugins.Where(plugin => serverConfig.ActivePlugins.Contains(plugin.Name));

            foreach (var plugin in relevantPlugins)
            {
                plugin.OnUserMessage(ircBot, e);

                if (firstWord[0].ToString(CultureInfo.InvariantCulture) == _configuration.CommandSymbol)
                {
                    var command = firstWord.Substring(1);
                    if (plugin.Commands.Contains(command))
                    {
                        plugin.IrcCommand(ircBot, e);
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

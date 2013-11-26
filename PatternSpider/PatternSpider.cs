using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using IrcDotNet;
using PatternSpider.Config;
using PatternSpider.Irc;
using PatternSpider.Plugins;

namespace PatternSpider
{
    class PatternSpider
    {
        private MainConfig _configuration;
        private List<IrcBot> _connections;

        public PatternSpider()
        {
            _connections = new List<IrcBot>();
            LoadConfiguration();
        }

        public void Run()
        {
            var pluginManager = new PluginManager();
            pluginManager.LoadPlugins();

            Connect(_configuration.Servers);

        }

        public void Quit()
        {
            _configuration.Save();

            foreach (var connection in _connections)
            {
                connection.Disconnect(_configuration.QuitMessage);
                connection.Stop();
            }            
        }

        private void Connect(IEnumerable<ServerConfig> servers)
        {
            foreach (var server in servers)
            {
                Connect(server);
            }
        }

        private void Connect(ServerConfig server)
        {
            var connection = new IrcBot();
            var regInfo = new IrcUserRegistrationInfo()
                {
                    NickName = server.NickName,
                    RealName = server.RealName,
                    UserModes = new List<char> {'i'},
                    UserName = server.NickName
                };
            
            connection.Connect(server.Address,regInfo);
            connection.Join(server.Channels);

            _connections.Add(connection);
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

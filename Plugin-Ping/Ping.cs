using System.Collections.Generic;
using System.ComponentModel.Composition;
using IrcDotNet;
using PatternSpider.Irc;
using PatternSpider.Plugins;

namespace Plugin_Ping
{
    [Export(typeof(IPlugin))]
    class Ping : IPlugin
    {
        public string Name { get { return "Ping"; } }
        public string Description { get { return "Returns a pong, when invoked with !ping"; } }

        public List<string> Commands { get { return new List<string> { "ping" }; } }

        
        public List<string> IrcCommand(IrcBot ircBot, string server, IrcMessageEventArgs e)
        {
            return new List<string>{"Pong."};
        }

        public List<string> OnChannelMessage(IrcBot ircBot, string server, string channel, IrcMessageEventArgs e)
        {
            return null;
        }

        public List<string> OnUserMessage(IrcBot ircBot, string server, IrcMessageEventArgs e)
        {
            return null;
        }
    }
}

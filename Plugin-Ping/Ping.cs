using System.Collections.Generic;
using System.ComponentModel.Composition;
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


        public List<string> IrcCommand(IrcBot ircBot, string server, IrcMessage m)
        {            
            return new List<string>{"Pong"};
        }

        public List<string> OnChannelMessage(IrcBot ircBot, string server, string channel, IrcMessage m)
        {
            return null;
        }

        public List<string> OnUserMessage(IrcBot ircBot, string server, IrcMessage m)
        {
            return null;
        }
    }
}

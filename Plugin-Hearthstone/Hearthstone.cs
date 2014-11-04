using System.Collections.Generic;
using System.ComponentModel.Composition;
using IrcDotNet;
using PatternSpider.Irc;
using PatternSpider.Plugins;

namespace Plugin_Hearthstone
{
    [Export(typeof(IPlugin))]
    public class Hearthstone : IPlugin
    {
        public string Name { get { return "Hearthstone"; } }
        public string Description { get { return "Gives a link to a hearhstone card when invoked."; } }

        public List<string> Commands { get { return new List<string> { "hs" }; } }


        public List<string> IrcCommand(IrcBot ircBot, string server, IrcMessageEventArgs e)
        {
            return new List<string> { "Card." };
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

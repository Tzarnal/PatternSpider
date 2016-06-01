using System.Collections.Generic;
using System.ComponentModel.Composition;
using PatternSpider.Irc;
using PatternSpider.Plugins;

namespace Plugin_ChatReactions
{
    [Export(typeof(IPlugin))]
    public class ChatReactions : IPlugin
    {
        public string Name => "ChatReactions";
        public string Description => "Automated reactions to certain things being said.";
        public List<string> Commands => new List<string>();

        public List<string> IrcCommand(IrcBot ircBot, string server, IrcMessage m)
        {
            return null;
        }

        public List<string> OnChannelMessage(IrcBot ircBot, string server, string channel, IrcMessage m)
        {
            if(m.Text.Contains("(╯°□°)╯︵ ┻━┻"))
                return new List<string> { "┬──┬◡ﾉ(° -°ﾉ)" };

            return null;
        }
                        
        public List<string> OnUserMessage(IrcBot ircBot, string server, IrcMessage m)
        {
            return null;
        }
    }
}

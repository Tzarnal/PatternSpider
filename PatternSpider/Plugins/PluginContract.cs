using System.Collections.Generic;
using PatternSpider.Irc;

namespace PatternSpider.Plugins
{
    public interface IPlugin
    {
        string Name { get; }
        string Description { get; }
        
        List<string> Commands { get; }

        List<string> IrcCommand(IrcBot ircBot, string server, IrcMessage m); //Fired when one of the defined commands for this plugin is used
        List<string> OnChannelMessage(IrcBot ircBot, string server, string channel, IrcMessage m); //Fires on any channel message
        List<string> OnUserMessage(IrcBot ircBot, string server, IrcMessage m);//Fires on any query (PM) message
    }
}

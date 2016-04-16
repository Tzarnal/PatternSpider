using System.Collections.Generic;
using System.ComponentModel.Composition;
using PatternSpider.Irc;
using PatternSpider.Plugins;
using PatternSpider.Utility;

namespace Plugin_Coin
{
    [Export(typeof (IPlugin))]
    public class Coin : IPlugin
    {
        public string Name { get { return "Coin"; } }
        public string Description { get { return "Throws a coin."; } }
        public List<string> Commands { get { return new List<string> { "coin" }; } }

        private DiceRoller _genie;

        public Coin()
        {
            _genie = new DiceRoller();
        }

        public List<string> IrcCommand(IrcBot ircBot, string server, IrcMessage m)
        {            
            var roll = _genie.RollDice(2);

            if (roll == 1)
            {
                return new List<string> {"Heads."};
            }
            else
            {
                return new List<string> { "Tails." };
            }            
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



using System.Collections.Generic;
using System.ComponentModel.Composition;
using PatternSpider.Irc;
using PatternSpider.Plugins;
using PatternSpider.Utility;

namespace Plugin_Fudge
{
    [Export(typeof(IPlugin))]
    class FudgeDice : IPlugin
    {
        public string Name { get { return "Fudge-Dice"; } }
        public string Description { get { return "Rolls fudge dice for systems like fudge or fate."; } }

        public List<string> Commands { get { return new List<string> { "fudge" }; } }
        private DiceRoller _genie;

        public FudgeDice()
        {
            _genie = new DiceRoller();
        }

        public List<string> IrcCommand(IrcBot ircBot, string server, IrcMessage m)
        {
            var message = m.Text;
            var name = m.Sender;
            var messageParts = message.Split(' ');
            var diceNumber = 4;

            if (messageParts.Length > 1 && !int.TryParse(messageParts[1], out diceNumber))
            {
                return new List<string> { "Sorry, the argument must be a number" };
            }


            var rollTotal = 0;
            var results = new string[diceNumber];

            for (var i = diceNumber; i > 0; i--)
            {
                var roll = _genie.RollDice(3);

                if (roll == 1)
                {
                    rollTotal--;
                    results[i - 1] = "-";
                }
                else if (roll == 2)
                {
                    results[i - 1] = "0";
                }
                else
                {
                    rollTotal++;
                    results[i - 1] = "+";
                }
            }

            return new List<string> { name + " -- [" + string.Join(" ", results) + "] -- Sum: " + rollTotal };
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

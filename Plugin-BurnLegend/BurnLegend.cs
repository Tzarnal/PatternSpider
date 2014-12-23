using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using PatternSpider.Irc;
using PatternSpider.Plugins;

namespace Plugin_BurnLegend
{
    [Export(typeof(IPlugin))]
    public class BurnLegend : IPlugin
    {
        public string Name { get { return "BurnLegend"; } }
        public string Description { get { return "Manages Burn Legend rounds."; } }

        public List<string> Commands { get { return new List<string> { "burn","burnlegend" }; } }

        private Dictionary<string, BurnLegendRound> _activeRounds;

        public BurnLegend()
        {
            _activeRounds = new Dictionary<string, BurnLegendRound>();
        }


        public List<string> IrcCommand(IrcBot ircBot, string server, IrcMessage m)
        {
            var messageParts = m.Text.Split(' ');

            if (messageParts.Length < 2)
            {
                return new List<string> { "Please ask for help in a PM." };
            }

            var subcommand = messageParts[1].ToLower();
            var nick = m.Sender;

            switch (subcommand)
            {
                case "help":
                    return new List<string> {"Please ask for help in a PM."};
                case "start":
                    {
                        if (messageParts.Length < 3)
                        {
                            return new List<string> { "Please provide a roundname" };
                        }

                        var roundName = messageParts[2];
                        if (_activeRounds.ContainsKey(roundName))
                        {
                            return new List<string>{"A Round with that name is already active."};
                        }
                        else
                        {
                            _activeRounds.Add(roundName, new BurnLegendRound());
                            return new List<string> { "New round with the name " + roundName + " started." };
                        }
                    }
                case "reveal":
                    {

                        if (messageParts.Length < 3)
                        {
                            return new List<string> { "Please provide a roundname" };
                        }                
                
                        var roundName = messageParts[2];
                
                        if (!_activeRounds.ContainsKey(roundName))
                        {
                            return new List<string>{"No round active by that name."};
                        }
                
                        var response = _activeRounds[roundName].Reveal();
                        _activeRounds.Remove(roundName);
                        return response;
                    }
                case "status":
                    {

                        if (messageParts.Length < 3)
                        {
                            return new List<string> { "Please provide a roundname" };
                        }

                        var roundName = messageParts[2];

                        if (!_activeRounds.ContainsKey(roundName))
                        {
                            return new List<string> { "No round active by that name." };
                        }

                        var response = _activeRounds[roundName].Status();
                        return new List<string> {response};
                    }
                case "action":
                    {                
                        if (messageParts.Length < 4)
                        {
                            return new List<string> { "Usage: Burnlegend BurnLegendAction <roundname> <action description>" };
                        }

                        var roundName = messageParts[2];

                        var actionDescription = string.Join(" ", messageParts.Skip(3));
                
                        if (!_activeRounds.ContainsKey(roundName))
                        {
                            return new List<string> { "No round active by that name." };
                        }
                
                        _activeRounds[roundName].AddAction(m.Sender,actionDescription);

                        return new List<string> { "Action Added to Round." };
                    }
                case "clear":
                    {
                        if (messageParts.Length < 3)
                        {
                            return new List<string> { "Please provide a roundname" };
                        }
             
                        var roundName = messageParts[2];

                        if (!_activeRounds.ContainsKey(roundName))
                        {
                            return new List<string> { "No round active by that name." };
                        }

                        _activeRounds[roundName].ClearActionsBy(m.Sender);
                        return new List<string> { "Cleared your entered actions in that round." };
                    }
            }

            return null;
        }

        public List<string> OnChannelMessage(IrcBot ircBot, string server, string channel, IrcMessage m)
        {
            return null;
        }

        public List<string> OnUserMessage(IrcBot ircBot, string server, IrcMessage m)
        {                       
            var messageParts = m.Text.ToLower().Split(' ');

            if (messageParts.Length >= 2)
            {
                var command = messageParts[0].Substring(1);
                if (Commands.Contains(command) )
                {
                    if (messageParts[1] == "help")
                    {
                        return HelpText();
                    }
                }
            }
                      
            return null;
        }

        private List<string> HelpText()
        {
            var helptext = new List<string>
                {
                    "Usage:",
                    "BurnLegend Help",
                    "- Show this Help Text",
                    "BurnLegend Start <roundname>",
                    "- Starts a Round of Burn legend with the provided round name",
                    "BurnLegend Reveal <roundname>",
                    "- Finishes an ongoing Round of Burn Legend and reveals the actions",
                    "Burnlegend Status <roundname>",
                    "- Displays who have entered actions for this round and how many.",
                    "Burnlegend Action <roundname> <action description>",
                    "- How to PM this bot actions for a round in a channel",
                    "Burnlegend Clear <roundname>",
                    "- Clears any actions you set up for that channel"
                };

            return helptext;
        }
    }
}

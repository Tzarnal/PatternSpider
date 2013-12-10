using System.Collections.Generic;
using System.ComponentModel.Composition;
using IrcDotNet;
using PatternSpider.Irc;
using PatternSpider.Plugins;
using PatternSpider.Utility;

namespace Plugin_Nwod
{
    [Export(typeof(IPlugin))]
    class NWod : IPlugin
    {
        public string Name { get { return "NWoD"; } }
        public string Description { get { return "Rolls dice for the NWoD system"; } }

        public List<string> Commands { get { return new List<string> { "nwod" }; } }

        private int _successes; 

        public List<string> IrcCommand(IrcBot ircBot, string server, IrcMessageEventArgs e)
        {
            var message = e.Text;
            var name = e.Source.Name;
            var messageParts = message.Split(' ');
            string response;

            if (messageParts.Length < 2)
            {
                return new List<string> { "!nwod <Pool size> [explode number] [target number]" };
            }

            var explode = 10;
            var success = 8;
            _successes = 0;

            try
            {
                var pool = int.Parse(messageParts[1]);

                if (pool > 2000)
                {
                    return new List<string> { "No pools over 2000." };
                }

                if (messageParts.Length > 2)
                {
                    explode = int.Parse(messageParts[2]);
                    if (explode < 2)
                        explode = 2;
                }

                if (messageParts.Length > 3)
                {
                    success = int.Parse(messageParts[3]);
                    if (explode < success)
                        success = explode;
                }

                response = RollDice(pool, explode, success);
            }
            catch
            {
                return new List<string>{ "Arguments must be numbers. nwod <Pool size> [explode number] [target number]" };
            }

            if (response.Length > 140)
            {
                response = "Rolls over 140 characters, truncated to reduce spam";
            }

            if (_successes > 0)
            {
                return new List<string> { string.Format("{0} -- {1} -- {2} success(es)", name, response, _successes) };
            }

            return new List<string> { string.Format("{0} -- {1} -- Failure", name, response) };
            

        }

        public List<string> OnChannelMessage(IrcBot ircBot, string server, string channel, IrcMessageEventArgs e)
        {
            return null;
        }

        public List<string> OnUserMessage(IrcBot ircBot, string server, IrcMessageEventArgs e)
        {
            return null;
        }

        private string RollDice(int dicepool, int explode = 10, int success = 8)
        {
            var genie = new DiceRoller();
            var results = new List<int>();
            var exploded = 0;

            while (dicepool > 0)
            {
                var result = genie.RollDice(10);
                if (result >= success)
                    _successes++;

                if (result >= explode)
                    exploded++;

                results.Add(result);
                dicepool--;
            }

            results.Sort();

            var response = string.Format("[ {0} ]", string.Join(",", results));

            if (exploded > 0)
                return response + " " + RollDice(exploded, explode, success);

            return response;
        }
    }
}

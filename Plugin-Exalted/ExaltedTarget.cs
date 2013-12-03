using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.Composition;
using IrcDotNet;
using PatternSpider.Irc;
using PatternSpider.Plugins;
using PatternSpider.Utility;

namespace Plugin_Exalted
{
    [Export(typeof(IPlugin))]    
    class ExaltedTarget: IPlugin
    {
        public string Name { get { return "ExaltedTarget"; } }
        public string Description { get { return "Throws Exalted (2e) dicepools against a specific target number and counts successes."; } }

        public List<string> Commands { get { return new List<string>{"et"}; } }

        private DiceRoller _fate = new DiceRoller();

        public List<string> IrcCommand(IrcBot ircBot, string server, IrcMessageEventArgs e)
        {
            var response = new List<string>();
            var mesasge = e.Text;
            var messageParts = mesasge.Split(' ');
            int targetNumber;

            if (messageParts.Length < 3)
            {
                return new List<string> { "Usage: e <target number> <poolsize> [poolsize]..." };
            }

            if (!int.TryParse(messageParts[1], out targetNumber))
            {
                return new List<string> { "Usage: e <target number> <poolsize> [poolsize]..." };
            }

            foreach (var messagePart in messageParts.Skip(2))
            {
                int poolSize;
                if (int.TryParse(messagePart,out poolSize))
                {
                    if (poolSize > 2000)
                    {
                        response.Add("No Pools over 2000.");
                    }
                    else
                    {
                        response.Add(string.Format("<{0}> {1}", e.Source.Name, RollPool(poolSize, targetNumber)));    
                    }                    
                }
            }

            return response;
        }

        private string RollPool(int poolSize, int targetNumber)
        {
            int[] rolls = new int[poolSize];
            var successes = 0;
            var response = "";

            for (var i = 0; i < poolSize; i++)
            {
                rolls[i] = _fate.RollDice(10);
                if (rolls[i] >= targetNumber)
                {
                    successes++;
                }

                if (rolls[i] == 10)
                {
                    successes++;
                }
            }

            if (rolls.Length <= 50)
            {
                response += String.Format("Rolls: {0}", string.Join(", ", rolls.ToList().OrderBy(r => r)));
            }
            else
            {
                response += "Rolls: Over 50 rolls, truncated to reduce spam";
            }
            
            if (successes == 0 && rolls.Contains(1))
            {
                response += " -- BOTCH";
            }
            else
            {
                response += string.Format(" -- {0} success(es).",successes);
            }

            return response;
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

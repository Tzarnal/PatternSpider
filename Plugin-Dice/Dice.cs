using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using IrcDotNet;
using PatternSpider.Irc;
using PatternSpider.Plugins;
using PatternSpider.Utility;

namespace Plugin_Dice
{
    [Export(typeof(IPlugin))]
    class Dice : IPlugin
    {
        public string Name { get { return "Dice"; } }
        public string Description { get { return "Rolls generic dice expressions, capable of doing math with results."; } }

        public List<string> Commands { get { return new List<string> { "dice","d","roll","r" }; } }
        private string _diceResults;
        private DiceRoller _genie = new DiceRoller();

        public List<string> IrcCommand(IrcBot ircBot, string server, IrcMessageEventArgs e)
        {
            var messageParts = e.Text.Split(' ');
            var processedMessage = string.Join(" ", messageParts.Skip(1));
            var name = e.Source.Name;                     
            var response = new List<string>();

            do
            {
                processedMessage = RollsToNumbers(processedMessage);

                if (!string.IsNullOrWhiteSpace(_diceResults))
                {
                    response.Add(string.Format("{0} -- {2} -- {1}", name, processedMessage, _diceResults));
                }

            } while (!string.IsNullOrWhiteSpace(_diceResults));


            var total = CalculateString(processedMessage);
            if (total != 0 && total.ToString(CultureInfo.InvariantCulture) != processedMessage)
            {
                response.Add(string.Format("{0} -- Result: {1}", name, total));
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

        private string RollsToNumbers(string input)
        {
            _diceResults = "";
            var output = input;
            var diceRegex = new Regex(@"\dd\d", RegexOptions.IgnoreCase);

            foreach (Match match in diceRegex.Matches(input))
            {
                if (match.Success)
                {
                    var numbers = match.Value.ToLower().Split('d');
                    var dieSize = int.Parse(numbers[1]);
                    var amountThrown = int.Parse(numbers[0]);
                    var total = 0;
                    var diceResults = new List<int>();

                    for (var i = 0; i < amountThrown; i++)
                    {
                        var result =  _genie.RollDice(dieSize);
                        diceResults.Add(result);
                        total += result;
                    }

                    var repRegex = new Regex(match.Value, RegexOptions.IgnoreCase);
                    output = repRegex.Replace(output, total.ToString(CultureInfo.InvariantCulture), 1);
                    _diceResults = string.Format("{0}[{1}]", _diceResults, string.Join(",", diceResults));
                }
            }

            return output;
        }

        private double CalculateString(string input)
        {
            var sc = new MSScriptControl.ScriptControl {Language = "VBScript"};

            if (Regex.Match(input, @"[abcefghijklmnopqrstuvwxyz]").Success)
                return 0;

            try
            {
                return sc.Eval(input);
            }
            catch (COMException)
            {
                return 0;
            }            
        }
    }
}

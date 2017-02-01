using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
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

        public List<string> Commands { get { return new List<string> { "dice", "d", "roll", "r", "d100", "d20", "d12", "d10", "d8", "d6", "d4" }; } }
        private string _diceResults;
        private DiceRoller _genie = new DiceRoller();

        public List<string> IrcCommand(IrcBot ircBot, string server, IrcMessage m)
        {
            var messageParts = m.Text.Split(' ');
            var processedMessage = string.Join(" ", messageParts.Skip(1));
            var name = m.Sender;                     
            var response = new List<string>();

            switch (messageParts[0].Substring(1))
            {
                case "d100":
                    processedMessage = "1d100 " + processedMessage;
                    break;
                case "d20":
                    processedMessage = "1d20 " +processedMessage;
                    break;
                case "d12":
                    processedMessage = "1d12 " + processedMessage;
                    break;
                case "d10":
                    processedMessage = "1d10 " + processedMessage;
                    break;
                case "d8":
                    processedMessage = "1d8 " + processedMessage;
                    break;
                case "d6":
                    processedMessage = "1d6 " + processedMessage;
                    break;
                case "d4":
                    processedMessage = "1d4 " + processedMessage;
                    break;
            }

            do
            {
                try
                {
                    processedMessage = RollsToNumbers(processedMessage);
                }
                catch (ArgumentException ex)
                {
                    
                    return new List<string>{ string.Format("{0} -- {1}", name,  ex.Message)};
                }
                                

                if (!string.IsNullOrWhiteSpace(_diceResults))
                {
                    if (_diceResults.Length > 140)
                    {
                        _diceResults = "rolls over 140 character, truncated to reduce spam";
                    }
                    response.Add(string.Format("{0} -- {2} -- {1}", name, processedMessage, _diceResults));
                }

            } while (!string.IsNullOrWhiteSpace(_diceResults));


            double total;
            
            try
            {
                total = CalculateString(processedMessage);
            }
            catch (InvalidExpressionException e)
            {
                return new List<string> { e.Message };
            }
            

            if (total != 0 && total.ToString(CultureInfo.InvariantCulture) != processedMessage.Trim())
            {
                response.Add(string.Format("{0} -- Result: {1}", name, total));
            }

            if (response.Any())
            {
                return response;
            }
            else
            {
                return new List<string>{"No dice to roll and not a mathematical expression."};
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

        private string RollsToNumbers(string input)
        {
            _diceResults = "";
            var output = input;
            var diceRegex = new Regex(@"\d*d\d+", RegexOptions.IgnoreCase);

            foreach (Match match in diceRegex.Matches(input))
            {
                if (match.Success)
                {
                    var numbers = match.Value.ToLower().Split('d');
                    int dieSize;
                    int amountThrown;

                    if (string.IsNullOrWhiteSpace(numbers[0]))
                    {
                        numbers[0] = "1";
                    }
                   
                    try
                    {
                        dieSize = int.Parse(numbers[1]);
                        amountThrown = int.Parse(numbers[0]);
                    }
                    catch (OverflowException)
                    {
                        throw new ArgumentException("Die size or roll size exceeds " + Int32.MaxValue);
                    }
                    
                                        
                    var total = 0;
                    var diceResults = new List<int>();

                    if (amountThrown > 9999)
                    {
                        throw new ArgumentException("No Rolls with more than 9999 Dice");
                    }

                    for (var i = 0; i < amountThrown; i++)
                    {
                        var result = _genie.RollDice(dieSize);
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
            if (Regex.Match(input, @"[A-z]").Success)
                throw new InvalidExpressionException("Unsupported symbols or characters in expression.");

            double result;

            try
            {
                result = Convert.ToDouble(new DataTable().Compute(input, null));
            }
            catch (Exception e)
            {
                throw new InvalidExpressionException("Error occured while trying to calculate: " + e.Message);
            }

            return result;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using PatternSpider.Irc;
using PatternSpider.Plugins;

namespace Plugin_Math
{
    [Export(typeof(IPlugin))]
    class Math : IPlugin
    {
        public string Name { get { return "Math"; } }
        public string Description { get { return "Does basic math operations ."; } }

        public List<string> Commands { get { return new List<string> { "math","m","calculate","calc" }; } }


        public List<string> IrcCommand(IrcBot ircBot, string server, IrcMessage m)
        {
            var messageParts = m.Text.Split(' ');
            var message = string.Join(" ", messageParts.Skip(1));

            try
            {
                return new List<string> {CalculateString(message).ToString(CultureInfo.InvariantCulture)};
            }
            catch (InvalidExpressionException e)
            {
                return new List<string> {e.Message};
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

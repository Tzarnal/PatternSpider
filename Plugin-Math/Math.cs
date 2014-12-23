using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
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
            catch (InvalidExpressionException)
            {
                return new List<string> {"Not a valid mathematical Expression."};
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
            if (Regex.Match(input, @"[abcdefghijklmnopqrstuvwxyz]").Success)
                throw new InvalidExpressionException();

            var sc = new MSScriptControl.ScriptControl {Language = "VBScript"};


            try
            {
                return sc.Eval(input);
            }
            catch (COMException)
            {
                throw new InvalidExpressionException();
            }      
        }
    }
}

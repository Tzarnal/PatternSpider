using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using IrcDotNet;
using PatternSpider.Irc;
using PatternSpider.Plugins;

namespace Plugin_Math
{
    [Export(typeof(IPlugin))]
    class Math : IPlugin
    {
        public string Name { get { return "Math"; } }
        public string Description { get { return "Returns a pong, when invoked with !ping"; } }

        public List<string> Commands { get { return new List<string> { "math","m","calculate","calc" }; } }


        public List<string> IrcCommand(IrcBot ircBot, string server, IrcMessageEventArgs e)
        {
            var messageParts = e.Text.Split(' ');
            var message = string.Join(" ", messageParts.Skip(1));

            return new List<string> { CalculateString(message).ToString(CultureInfo.InvariantCulture) };
        }

        public List<string> OnChannelMessage(IrcBot ircBot, string server, string channel, IrcMessageEventArgs e)
        {
            return null;
        }

        public List<string> OnUserMessage(IrcBot ircBot, string server, IrcMessageEventArgs e)
        {
            return null;
        }

        private double CalculateString(string input)
        {
            var sc = new MSScriptControl.ScriptControl {Language = "VBScript"};
            return sc.Eval(input);        
        }
    }
}

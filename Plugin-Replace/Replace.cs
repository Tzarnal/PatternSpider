using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text.RegularExpressions;
using IrcDotNet;
using PatternSpider.Irc;
using PatternSpider.Plugins;

namespace Plugin_Replace
{
    [Export(typeof(IPlugin))]
    class Replace : IPlugin
    {
        public string Name { get { return "Replace"; } }
        public string Description { get { return "Uses the s/bla/bloop/ notation to correct line spreviously said"; } }

        public List<string> Commands { get { return new List<string>(); } }

        private Dictionary<string, LineHistory> _history;
        private LineHistory _generalHistory;

        public Replace()
        {
            _history = new Dictionary<string, LineHistory>();
            _generalHistory = new LineHistory(10);
        }

        public List<string> IrcCommand(IrcBot ircBot, string server, IrcMessageEventArgs e)
        {
            return null;
        }

        public List<string> OnChannelMessage(IrcBot ircBot, string server, string channel, IrcMessageEventArgs e)
        {
            var id = channel + e.Source.Name;
            var line = e.Text;
            var expresion = @"[sr][/\\\\](.+)[/\\\\](.+)";

            if (!_history.ContainsKey(id))
            {
                _history.Add(id, new LineHistory(5));
            }

            if (Regex.IsMatch(line, expresion))
            {
                var match = Regex.Match(line, expresion);
                var original = match.Groups[1].Value;
                var replacement = match.Groups[2].Value;
                

                var rLastChar = replacement.Length-1;
                if(replacement[rLastChar] == '\\' || replacement[rLastChar] == '/')
                {
                    replacement = replacement.Substring(0, rLastChar);
                }


                if (_history[id].HasMatch(original))
                {
                    return new List<string> { string.Format("{0} Meant: {1}", e.Source.Name, _history[id].Replace(original, replacement)) };                    
                }
                
                if (_generalHistory.HasMatch(original))
                {
                    return new List<string> { string.Format("{0} Thinks you meant: {1}", e.Source.Name, _generalHistory.Replace(original, replacement)) };                    
                }

                return null;
            }
            

            _generalHistory.AddLine(line);
            _history[id].AddLine(line);

            return null;
        }

        public List<string> OnUserMessage(IrcBot ircBot, string server, IrcMessageEventArgs e)
        {
            return null;
        }
    }
}

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
            var line = e.Text.Trim();
            var expresion = @"\A[sr][/\\\\](.+?)[/\\\\](.+?)(?:[/\\\\]g)?";

            if (Regex.IsMatch(line, expresion))
            {
                var match = Regex.Match(line, expresion);
                var original = match.Groups[1].Value;
                var replacement = match.Groups[2].Value;
                var global = false;

                var lineLastCharIndex = line.Length - 1;
                if (line.ToLower()[lineLastCharIndex] == 'g' && (line[lineLastCharIndex-1] == '\\' || line[lineLastCharIndex-1] == '/'))
                {
                    global = true;
                }

                var rLastCharIndex = replacement.Length-1;
                if(replacement[rLastCharIndex] == '\\' || replacement[rLastCharIndex] == '/')
                {
                    replacement = replacement.Substring(0, rLastCharIndex);
                }
               
                if (_history[id].HasMatch(original))
                {
                    var text = _history[id].GetLine(original);                   
                    
                    return new List<string> { string.Format("{0} Meant: {1}", e.Source.Name, ReplaceText(text, original, replacement, global)) };                                             
                }
                
                if (_generalHistory.HasMatch(original))
                {
                    var text = _generalHistory.GetLine(original);

                    return new List<string> { string.Format("{0} Thinks you meant: {1}", e.Source.Name, ReplaceText(text, original, replacement, global)) };                                        
                }

                return null;
            }else if (!_history.ContainsKey(id))
            {
                _history.Add(id, new LineHistory(5));
            }
            
            _generalHistory.AddLine(line);
            _history[id].AddLine(line);

            return null;
        }

        public List<string> OnUserMessage(IrcBot ircBot, string server, IrcMessageEventArgs e)
        {
            return null;
        }

        public string ReplaceText(string line, string original, string replacement, bool global=false)
        {
            if (global)
            {
                return ReplaceGlobal(line, original, replacement);
            }

            var regex = new Regex(Regex.Escape(original));
            return regex.Replace(line, replacement, 1).Trim();       
        }

        public string ReplaceGlobal(string line, string original, string replacement)
        {
            return line.Replace(original, replacement).Trim();   
        }
    }
}

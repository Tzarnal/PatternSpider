using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using PatternSpider.Irc;
using PatternSpider.Plugins;
using Plugin_Seen.Extensions;

namespace Plugin_Seen
{
    [Export(typeof(IPlugin))]
    public class Seen : IPlugin
    {
        public string Name { get { return "Seen"; } }
        public string Description { get { return "Gives the last time this nickcname was seen on the server. "; } }

        public List<string> Commands { get { return new List<string> { "seen" }; } }

        private NickHistory _history;

        public Seen()
        {
            if (File.Exists(NickHistory.FullPath))
            {
                _history = NickHistory.Load();
            }
            else
            {
                _history = new NickHistory();
            }
        }

        public List<string> IrcCommand(IrcBot ircBot, string server, IrcMessage m)
        {
            var targets = m.TextArray.Skip(1);
            var message = new List<string>();

            if (! _history.HistoryByServer.ContainsKey(m.Server))
            {
                return null;
            }

            var history = _history.HistoryByServer[m.Server];

            foreach (var target in targets)
            {
                if (history.ContainsKey(target))
                {
                    var entry = history[target];
                    message.Add(string.Format("{0} was last seen {1} in {2} saying: {3}", target, entry.Time.TimeSince(),
                        entry.Channel, entry.Message));
                }
                else
                {
                    message.Add(string.Format("I cannot remember seeing {0} on this server.", target));
                }
            }

            return message;
        }

        public List<string> OnChannelMessage(IrcBot ircBot, string server, string channel, IrcMessage m)
        {
            if (!_history.HistoryByServer.ContainsKey(m.Server))
            {
                _history.HistoryByServer.Add(m.Server,new Dictionary<string, HistoryEntry>());    
            }

            if (_history.HistoryByServer[m.Server].ContainsKey(m.Sender))
            {
                _history.HistoryByServer[m.Server][m.Sender] = new HistoryEntry
                {
                    Channel = m.Channel,
                    Time = DateTime.Now,
                    Message = m.Text
                };
            }
            else
            {
                _history.HistoryByServer[m.Server].Add(m.Sender, new HistoryEntry
                {
                    Channel = m.Channel,
                    Time = DateTime.Now,
                    Message = m.Text
                });
            }

            _history.SaveHeartbeat();

            return null;
        }

        public List<string> OnUserMessage(IrcBot ircBot, string server, IrcMessage m)
        {
            return null;
        }
    }
}

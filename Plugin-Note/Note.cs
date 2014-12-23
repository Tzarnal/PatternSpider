using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using PatternSpider.Irc;
using PatternSpider.Plugins;
using Plugin_Note.Extensions;


namespace Plugin_Note
{
    [Export(typeof(IPlugin))]
    class Note : IPlugin
    {
        public string Name { get { return "Note"; } }
        public string Description { get { return "Manages notes for people connected to a server"; } }

        public List<string> Commands { get { return new List<string>{ "note"};}}

        private Notes _notes;

        public Note()
        {
            if (File.Exists(Notes.FullPath))
            {
                _notes = Notes.Load();
            }
            else
            {
                _notes = new Notes();
            }
        }

        public List<string> IrcCommand(IrcBot ircBot, string serverName, IrcMessage m)
        {
            var messageParts = m.Text.Split(' ');
            var message = string.Join(" ", messageParts.Skip(2));

            if (messageParts.Length < 3)
            {
                return HelpText();
            }

            if (!_notes.NotesByServer.ContainsKey(serverName))
            {
                _notes.NotesByServer.Add(serverName, new List<NoteEntry>());
            }

            var note = new NoteEntry
                {
                    Recipient = messageParts[1],
                    Message = message,
                    Sender = m.Sender,
                    Time = DateTime.Now
                };

            _notes.NotesByServer[serverName].Add(note);
            _notes.Save();

            return new List<string> { String.Format("Your note to {0} has been added.", messageParts[1]) };           
        }

        public List<string> OnChannelMessage(IrcBot ircBot, string serverName, string channelName, IrcMessage m)
        {
            var responses = new List<string>();

            if (_notes.NotesByServer.ContainsKey(serverName))
            {
                var user = m.Sender.ToLower();
                var userNotes = _notes.NotesByServer[serverName].Where(note => note.Recipient.ToLower() == user).ToList();
                
                foreach (var noteEntry in userNotes)
                {
                    var reponse = string.Format("{0}, Note by {1} from {2} : {3}", noteEntry.Recipient, noteEntry.Sender,
                                                noteEntry.Time.TimeSince(), noteEntry.Message);
                        
                    _notes.NotesByServer[serverName].Remove(noteEntry);
                    _notes.Save();
                    responses.Add(reponse);
                }

            }
            return responses;
        }

        public List<string> OnUserMessage(IrcBot ircBot, string serverName, IrcMessage m)
        {
            return null;
        }

        private List<string> HelpText()
        {
            var helptext = new List<string>();
            
            helptext.Add("Usage:");
            helptext.Add("Note <Username> Message");
                
            return helptext;
        }
    }
}

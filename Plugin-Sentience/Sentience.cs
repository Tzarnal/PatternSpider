using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Text.RegularExpressions;
using IrcDotNet;
using PatternSpider.Irc;
using PatternSpider.Plugins;
using RelayChains;

namespace Plugin_Sentience
{
    [Export(typeof(IPlugin))]
    class Sentience : IPlugin
    {
        public string Name { get { return "Sentience"; } }
        public string Description { get { return "Implements a chatterbot through RelayChains, a Markov Chain implementation"; } }

        public List<string> Commands { get { return new List<string>(); } }

        private Dictionary<string, Chain> _brains;
        private const int Windowsize = 4;
        private readonly object writeLock = new object();

        public static string BrainPath = "Plugins/Sentience/";

        public Sentience()
        {
            _brains = new Dictionary<string, Chain>();
            if (!Directory.Exists(BrainPath))
            {
                Directory.CreateDirectory(BrainPath);
            }
            PruneBrains();
            LoadBrains();
        }

        public List<string> IrcCommand(IrcBot ircBot, string server, IrcMessageEventArgs e)
        {
            return null;
        }

        public List<string> OnChannelMessage(IrcBot ircBot, string server, string channel, IrcMessageEventArgs e)
        {
            var key = server + channel;
            var message = e.Text;
            var botName = ircBot.Nickname;
            var botNameMatch = string.Format("^{0}[:,;]", botName);

            if (!_brains.ContainsKey(key))
            {
                _brains.Add(key, new Chain(Windowsize));
            }
            var brain = _brains[key];

            if (Regex.IsMatch(message, botNameMatch))
            {
                var regex = new Regex(botNameMatch);
                
                message = regex.Replace(message, "");
                
                var response = TextSanitizer.FixInputEnds(brain.GenerateSentenceFromSentence(message));

                if (!string.IsNullOrWhiteSpace(response))
                {
                    return new List<string> { response };    
                }

                response = RandomResponse.Reponse(e.Source.Name);
                return new List<string> { response  };                                    
            }
            
            if (message.Split(' ').Length > Windowsize)
            {
                message = TextSanitizer.SanitizeInput(message);
                brain.Learn(message);

                SaveLine(key, message);    
            }
            

            return null;
        }

        private void SaveLine(string key, string message)
        {
            lock (writeLock)
            {
                var fs = File.AppendText(BrainPath + key + ".brain");
                fs.WriteLine(message);
                fs.Close();
            }
        }

        private void LoadBrains()
        {
            var brainFiles = Directory.EnumerateFiles(BrainPath,"*.brain");
            foreach (var brainfile in brainFiles)
            {
                var sr = new StreamReader(brainfile);
                var brain = new Chain(Windowsize);
                string line;

                var key = brainfile.Replace(".brain", "");
                key = key.Replace(BrainPath, "");

                while( (line = sr.ReadLine()) != null)
                {
                    brain.Learn(TextSanitizer.SanitizeInput(line));
                }

                _brains.Add(key, brain);
                sr.Close();
                Console.WriteLine("Sentience: Loaded {0}", key);
            }
        }

        private void PruneBrains()
        {
            var brainFiles = Directory.EnumerateFiles(BrainPath,"*.brain");
            foreach (var brainfile in brainFiles)
            {
                var fileInfo = new FileInfo(brainfile);
                if (fileInfo.Length > 3145728)
                {
                    File.Copy(brainfile,BrainPath+"pruning.brain");
                    File.Delete(brainfile);
                    
                    string line;
                    var sr = new StreamReader(BrainPath + "pruning.brain");
                    var sw = new StreamWriter(brainfile);
                    var rand = new Random();

                    while ((line = sr.ReadLine()) != null)
                    {
                        if (rand.Next(0, 2) == 1)
                        {
                            sw.WriteLine(line); 
                        }                        
                    }

                    sr.Close();
                    sw.Close();

                    File.Delete(BrainPath + "pruning.brain");
                    Console.WriteLine("Sentience: Pruned {0}", brainfile);
                }
            }
        }

        public List<string> OnUserMessage(IrcBot ircBot, string server, IrcMessageEventArgs e)
        {
            return null;
        }
    }
}

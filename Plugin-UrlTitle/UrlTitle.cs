using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using HtmlAgilityPack;
using IrcDotNet;
using PatternSpider.Irc;
using PatternSpider.Plugins;

namespace Plugin_UrlTitle
{
    [Export(typeof(IPlugin))]
    class UrlTitle : IPlugin
    {
        public string Name { get { return "UrlTitle"; } }
        public string Description { get { return "Shows the Title of the page associated with an url when an url is mentioned on irc"; } }
       
        public List<string> Commands {
            get { return new List<string>(); }
        }
        
        public List<string> OnUserMessage(IrcBot ircBot, string server, IrcMessageEventArgs e)
        {
            return null;
        }

        public List<string> OnChannelMessage(IrcBot ircBot, string server, string channel, IrcMessageEventArgs e)
        {
            var MatchUrlRegex = @"(?i)\b((?:[a-z][\w-]+:(?:/{1,3}|[a-z0-9%])|www\d{0,3}[.]|[a-z0-9.\-]+[.][a-z]{2,4}/)(?:[^\s()<>]+|\(([^\s()<>]+|(\([^\s()<>]+\)))*\))+(?:\(([^\s()<>]+|(\([^\s()<>]+\)))*\)|[^\s`!()\[\]{};:'"".,<>?«»“”‘’]))";
            var results = Regex.Matches(e.Text, MatchUrlRegex);

            foreach (var result in results)
            {
                var r = result;
                ThreadStart starter = () => OutputUrlTitle(r.ToString(), ircBot, channel);
                new Thread(starter).Start();
            }

            return null;
        }

        public List<string> IrcCommand(IrcBot ircBot, string server, IrcMessageEventArgs e)
        {
            return null;
        }

        private void OutputUrlTitle(string url, IrcBot ircBot, string channel)
        {
            try
            {
                const string twitterStatusRegex = @"twitter.com/\w+/status/\d+";
                const string twitterMobileStatusRegex = @"mobile.twitter.com/\w+/status/\d+";
                string message;


                if (Regex.IsMatch(url, twitterMobileStatusRegex))
                {
                    message = GetTwitterMobileMessage(url);
                }
                else if (Regex.IsMatch(url, twitterStatusRegex))
                {
                    message = GetTwitterMessage(url);
                }else
                {
                    message = GetWebPageTitle(url);
                }

                if (message != null)
                {
                    ircBot.SendChannelMessage(channel, message);
                }

            }catch(Exception e)
            {
                //Console.WriteLine(e.Message);
            }
            
        }

        private string GetTwitterMessage(string url)
        {
            var document = UrlRequest(url);
            var statusnode =
                document.DocumentNode.SelectSingleNode(
                    "//p[contains(concat(' ', normalize-space(@class), ' '), 'tweet-text')]");

            return CleanString(statusnode.InnerText);
        }

        private string GetTwitterMobileMessage(string url)
        {
            var document = UrlRequest(url);
            var statusnode =
                document.DocumentNode.SelectSingleNode(
                    "//div[contains(concat(' ', normalize-space(@class), ' '), 'tweet-text')]");

            return CleanString(statusnode.InnerText);
        }

        private string GetWebPageTitle(string url)
        {
            var document = UrlRequest(url);
            var titlenode = document.DocumentNode.SelectSingleNode("//head/title");
            if (titlenode == null)
                return null;

            return titlenode.InnerText;
        }

        private string CleanString(string input)
        {
            Regex whitespaceRegex = new Regex("\\s");
            return whitespaceRegex.Replace(input," ").Trim();
        }

        private HtmlDocument UrlRequest(string url)
        {
            var req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "GET";
            req.ContentType = "application/x-www-form-urlencoded";
            req.UserAgent = "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/535.2 (KHTML, like Gecko) Chrome/15.0.874.121 Safari/535.2";

            var responseStream = req.GetResponse().GetResponseStream();
            var document = new HtmlDocument();

            if (responseStream == null)
            {
                throw new NoNullAllowedException();
            }

            using (var reader = new StreamReader(responseStream))
            {
                using (var memoryStream = new MemoryStream())
                {
                    using (var writer = new StreamWriter(memoryStream))
                    {
                        writer.Write(reader.ReadToEnd());
                        memoryStream.Position = 0;
                        document.Load(memoryStream, new UTF8Encoding());
                    }
                }
            }

            return document;
        }
       
    }
}

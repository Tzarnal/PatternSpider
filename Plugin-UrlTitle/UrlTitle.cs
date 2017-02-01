using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using PatternSpider.Irc;
using PatternSpider.Plugins;

namespace Plugin_UrlTitle
{
    [Export(typeof (IPlugin))]
    internal class UrlTitle : IPlugin
    {
        private TwitterHandler _twitter;

        public string Name => "UrlTitle";

        public string Description => "Shows the Title of the page associated with an url when an url is mentioned on irc";

        public List<string> Commands => new List<string>();

        public UrlTitle()
        {
            _twitter = new TwitterHandler();
        }

        public List<string> OnUserMessage(IrcBot ircBot, string server, IrcMessage m)
        {
            return null;
        }

        public List<string> OnChannelMessage(IrcBot ircBot, string server, string channel, IrcMessage m)
        {
            var MatchUrlRegex =
                @"(?i)\b((?:[a-z][\w-]+:(?:/{1,3}|[a-z0-9%])|www\d{0,3}[.]|[a-z0-9.\-]+[.][a-z]{2,4}/)(?:[^\s()<>]+|\(([^\s()<>]+|(\([^\s()<>]+\)))*\))+(?:\(([^\s()<>]+|(\([^\s()<>]+\)))*\)|[^\s`!()\[\]{};:'"".,<>?«»“”‘’]))";
            var results = Regex.Matches(m.Text, MatchUrlRegex);

            foreach (var result in results)
            {
                var r = result;
                OutputUrlTitle(r.ToString(), ircBot, channel);
            }

            return null;
        }

        public List<string> IrcCommand(IrcBot ircBot, string server, IrcMessage m)
        {
            return null;
        }

        private void OutputUrlTitle(string url, IrcBot ircBot, string channel)
        {
            try
            {
                const string twitterStatusRegex = @"twitter.com/\w+/status/\d+";                
                string message;

                if (Regex.IsMatch(url, twitterStatusRegex))
                {
                    message = GetTwitterMessage(url);
                }
                else
                {
                    message = GetWebPageTitle(url);
                }

                if (message != null)
                {
                    ircBot.SendMessage(channel, message);
                }

            }
            catch 
            {
                //Console.WriteLine(e.Message);
            }

        }

        private string GetTwitterMessage(string url)
        {
            const string twitterStatusRegex = @"twitter.com/\w+/status/(\d+)";
            var result = Regex.Match(url, twitterStatusRegex);

            return CleanString(_twitter.GetTweet(result.Groups[1].Value)); 
                        
        }

        private string GetWebPageTitle(string url)
        {
            var document = UrlRequest(url);
            var titlenode = document.DocumentNode.SelectSingleNode("//head/title");
            if (titlenode == null)
                return null;

            var title = Regex.Match(titlenode.InnerText, @"\S+.+").Value;

            return string.Format("[{0}]", CleanString(title));
        }

        private string CleanString(string input)
        {
            input = WebUtility.HtmlDecode(input);

            input = input.Replace("\n", " ");
            input = input.Replace("\t", " ");
            input = input.Replace("pic.twitter", " https://pic.twitter");

            input = input.Trim();
            input = Regex.Replace(input, @"\s+", " ");

            return input;
        }

        private HtmlDocument UrlRequest(string url)
        {
            HtmlDocument document;

            var web = new HtmlWeb();
            web.OverrideEncoding = Encoding.UTF8;
            web.PreRequest = delegate(HttpWebRequest webRequest)
            {
                webRequest.Method = "GET";
                webRequest.ContentType = "application/x-www-form-urlencoded";
                webRequest.UserAgent =
                    "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/535.2 (KHTML, like Gecko) Chrome/15.0.874.121 Safari/535.2";
                webRequest.Headers.Add("Accept-Language", "en-gb, en;q=0.8)");
                return true;
            };

            try
            {
                document = web.Load(url);
            }
            catch
            {
                return null;
            }
            

            return document;
        }
    }
}

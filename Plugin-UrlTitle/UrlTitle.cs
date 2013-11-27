using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
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
            var title = GetWebPageTitle(url);

            ircBot.SendChannelMessage(channel, title);
        }

        private string GetWebPageTitle(string url)
        {
            // Create a request to the url

            HttpWebRequest request;

            //Ensure the url starts with http:// otherwise the httpwebrequest gets confused
            if (!url.Contains(@"http://") && !url.Contains(@"https://"))
            {
                url = "http://" + url;
            }

            try
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }
            catch (Exception)
            {
                return null;
            }

            // If the request wasn't an HTTP request (like a file), ignore it
            if (request == null) return null;

            // Use the user's credentials
            request.UseDefaultCredentials = true;

            // Obtain a response from the server, if there was an error, return nothing
            HttpWebResponse response;
            try
            {
                response = request.GetResponse() as HttpWebResponse;
            }
            catch (WebException)
            {
                return null;
            }

            // Regular expression for an HTML title
            const string regex = @"(?<=<title.*>)([\s\S]*?)(?=</title>)";

            // If the correct HTML header exists for HTML text, continue
            if (response != null && new List<string>(response.Headers.AllKeys).Contains("Content-Type"))

                if (response.Headers["Content-Type"].StartsWith("text/html"))
                {
                    // Download the page
                    var web = new WebClient();
                    web.UseDefaultCredentials = true;

                    string page;
                    try
                    {
                        page = web.DownloadString(url);
                    }
                    catch (Exception)
                    {
                        return null;
                    }

                    // Extract the title
                    var ex = new Regex(regex, RegexOptions.IgnoreCase);
                    string output = ex.Match(page).Value;
                    output = Regex.Replace(output, @"\s+", " ");
                    return output.Trim().Replace("\r", "").Replace("\n", "").Replace("\r\n", "");

                }

            // Not a valid HTML page
            return null;
        }

    }
}

using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using IrcDotNet;
using PatternSpider.Irc;
using PatternSpider.Plugins;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Plugin_Weather
{
    [Export(typeof(IPlugin))]
    class Weather:IPlugin
    {
        public string Name { get { return "Weather"; } }
        public string Description { get { return "Gives weather information on command."; } }

        public List<string> Commands { get { return new List<string> { "weather" }; } }

        private UsersLocations _usersLocations;

        public Weather()
        {
            if (File.Exists(UsersLocations.FullPath))
            {
                _usersLocations = UsersLocations.Load();
            }
            else
            {
                _usersLocations = new UsersLocations();
            }
        }

        public List<string> IrcCommand(IrcBot ircBot, string server, IrcMessageEventArgs e)
        {
            var text = e.Text.Trim();
            var messageParts = text.Split(' ');
            List<string> response;
            var user = e.Source.Name;

            if (messageParts.Count() == 1)
            {
                if (_usersLocations.UserLocations.ContainsKey(user))
                {
                    response = WeatherToday(_usersLocations.UserLocations[user]);
                }
                else
                {
                    response = HelpText();
                }
                
            }
            else if (messageParts.Count() == 2)
            {
                var command = messageParts[1];                
                if (command == "forecast")
                {
                    if (_usersLocations.UserLocations.ContainsKey(user))
                    {
                        response = WeatherForecast(_usersLocations.UserLocations[user]);
                    }
                    else
                    {
                        response = HelpText();
                    }
                }else if (command == "remember")
                {
                    response = HelpText();
                }
                else
                {
                    response = WeatherToday(command);
                }                
            }
            else
            {
                var command = messageParts[1].ToLower();

                if (command == "forecast")
                {
                    response = WeatherForecast(string.Join(" ", messageParts.Skip(1)));
                }
                else if (command == "remember")
                {
                    response = Remember(e.Source.Name,string.Join(" ", messageParts.Skip(2)));
                }
                else
                {
                    response = WeatherToday(string.Join(" ", messageParts.Skip(1)));
                }
            }

            return response;
        }

        public List<string> OnChannelMessage(IrcBot ircBot, string server, string channel, IrcMessageEventArgs e)
        {
            return null;
        }

        public List<string> OnUserMessage(IrcBot ircBot, string server, IrcMessageEventArgs e)
        {
            return null;
        }

        private string CleanString(string input)
        {
            Regex whitespaceRegex = new Regex("\\s");
            return whitespaceRegex.Replace(input, " ").Trim();
        }

        private List<string> WeatherToday(string location)
        {
            HtmlDocument document;
            
            try
            {
                document = UrlRequest(string.Format("https://www.google.com/search?q='Weather:{0}'", location));
            }
            catch
            {
                return new List<string> {"Error Occured trying to query for weather."};
            }
            
            //try
            //{
                var siteLocation = CleanString(document.DocumentNode.SelectSingleNode("//div[@id='wob_loc']").InnerText);
                var weatherDescription = CleanString(document.DocumentNode.SelectSingleNode("//span[@id='wob_dc']").InnerText);
                var tempC = CleanString(document.DocumentNode.SelectSingleNode("//span[@id='wob_tm']").InnerText);
                var tempF = CleanString(document.DocumentNode.SelectSingleNode("//span[@id='wob_ttm']").InnerText);
                var humidity = CleanString(document.DocumentNode.SelectSingleNode("//span[@id='wob_hm']").InnerText);
                var windkmh = CleanString(document.DocumentNode.SelectSingleNode("//div[@id='wob_wg']//span[@class='wob_t'][1]").InnerText);
                var windmph = CleanString(document.DocumentNode.SelectSingleNode("//div[@id='wob_wg']//span[@class='wob_t'][2]").InnerText);

                return new List<string> { String.Format("Weather for {6}: {1}°C ({0}°F) and {2}, {3} Humidity and {4} ({5}) Winds.",
                                                            tempC,tempF,weatherDescription,humidity,windkmh,windmph,siteLocation)};

            /*}
            catch
            {
                return new List<string> { "Could not find weather for " + location };
            } */          
        }

        private List<string> WeatherForecast(string location)
        {
            HtmlDocument document;

            try
            {
                document = UrlRequest(string.Format("https://www.google.com/search?q='Weather:{0}'", location));
            }
            catch
            {
                return new List<string> { "Error Occured trying to query for weather." };
            }

            try
            {
                var locationNode = document.DocumentNode.SelectSingleNode("//div[@id='wob_loc'] ");
                var response = new List<string>();

                response.Add(string.Format("3 Day forcast for: {0}", locationNode.InnerText));

                for(var i = 1; i < 4; i++)
                {
                    var day = CleanString(document.DocumentNode.SelectSingleNode("//div[@wob_di='" + i + "']/div[@class='vk_lgy']").InnerText);
                    var desc = CleanString(document.DocumentNode.SelectSingleNode("//div[@wob_di='" + i + "']//img").Attributes["alt"].Value);

                    var tempDayC = CleanString(document.DocumentNode.SelectSingleNode("//div[@wob_di='" + i + "']//div[@class='vk_gy']/*[1]").InnerText);
                    var tempDayF = CleanString(document.DocumentNode.SelectSingleNode("//div[@wob_di='" + i + "']//div[@class='vk_gy']/*[2]").InnerText);

                    var tempNightC = CleanString(document.DocumentNode.SelectSingleNode("//div[@wob_di='" + i + "']//div[@class='vk_lgy']/*[1]").InnerText);
                    var tempNightF = CleanString(document.DocumentNode.SelectSingleNode("//div[@wob_di='" + i + "']//div[@class='vk_lgy']/*[2]").InnerText);

                    response.Add(String.Format("{0}: {1} Day {2}°C ({3}°F), Night {4}°C ({5}°F)",
                        day, desc, tempDayC,tempDayF, tempNightC, tempNightF));
                }

                return response;
            }
            catch
            {
                return new List<string> { "Could not find weather for " + location };
            }   
        }

        private List<string> Remember(string user, string location)
        {
            if (_usersLocations.UserLocations.ContainsKey(user))
            {
                _usersLocations.UserLocations[user] = location;
                _usersLocations.Save();
                return new List<string> { "Remembering new location for: " + user };

            }            
            _usersLocations.UserLocations.Add(user,location);
            _usersLocations.Save();
            return new List<string> { "Remembering location for: " + user };
        }

        private HtmlDocument UrlRequest(string url)
        {
            var req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "GET";
            req.ContentType = "application/x-www-form-urlencoded";
            req.Headers.Add("Accept-Language", "en;q=0.8");
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

        private List<string> HelpText()
        {
            var helptext = new List<string>();

            helptext.Add("Usage:");
            helptext.Add("Weather - Gives Weather for a remembered location");
            helptext.Add("Weather <location> - Gives Weather for a specificed location");
            helptext.Add("Weather Forecast - Give Weather Forecast for a remembered location");
            helptext.Add("Weather Forecast <location> Gives Weather Forecast for a specified location");
            helptext.Add("Weather Remember <location> - Remembers a location for your nickname");

            return helptext;
        }
    }
}

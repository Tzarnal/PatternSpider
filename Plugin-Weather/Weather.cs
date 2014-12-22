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
using ForecastIO;

namespace Plugin_Weather
{
    [Export(typeof(IPlugin))]
    class Weather:IPlugin
    {
        public string Name { get { return "Weather"; } }
        public string Description { get { return "Gives weather information on command."; } }

        public List<string> Commands { get { return new List<string> { "weather" }; } }

        private UsersLocations _usersLocations;
        private ApiKeys _apiKeys;

        public Weather()
        {
            if (File.Exists(UsersLocations.FullPath))
            {
                _usersLocations = UsersLocations.Load();
            }
            else
            {
                _usersLocations = new UsersLocations();
                _usersLocations.Save();
            }

            if (File.Exists(ApiKeys.FullPath))
            {
                _apiKeys = ApiKeys.Load();
            }
            else
            {
                _apiKeys = new ApiKeys();
                _apiKeys.Save();
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
            var output = new List<string> {"No Weather"};
            return output;
        }

        private List<string> WeatherForecast(string location)
        {
            var output = new List<string> {"No Forecast"};
            return output;
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

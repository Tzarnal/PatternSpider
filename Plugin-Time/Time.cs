using System;
using System.Collections.Generic;
using PatternSpider.Irc;
using PatternSpider.Plugins;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;

namespace Plugin_Time
{
    [Export(typeof(IPlugin))]
    public class Time : IPlugin
    {
        public string Name { get { return "Time"; } }
        public string Description { get { return "Gives current time for a location when."; } }

        public List<string> Commands { get { return new List<string> { "time" }; } }

        private ApiKeys _apiKeys;
        private GeoCodeLookup _geoLookup;
        private TimeLookup _timeLookup;

        public Time()
        {
            if (File.Exists(ApiKeys.FullPath))
            {
                _apiKeys = ApiKeys.Load();
            }
            else
            {
                _apiKeys = new ApiKeys();
                _apiKeys.Save();
            }

            _geoLookup = new GeoCodeLookup(_apiKeys.MapQuestKey);
            _timeLookup = new TimeLookup(_apiKeys.TimeZoneKey);
        }

        public List<string> IrcCommand(IrcBot ircBot, string server, IrcMessage m)
        {
            var text = m.Text.Trim();
            var messageParts = text.Split(' ');
            List<string> response = new List<string> {};
            var user = m.Sender;

            if (messageParts.Length == 1)
            {
                return new List<string> { "Please supply a location." };

            }

            var location = string.Join(" ",messageParts.Skip(1));

            Coordinates coordinates;
           
            try
            {
                coordinates = _geoLookup.Lookup(location);
            }
            catch (Exception e)
            {
                Console.WriteLine("Weather Lookup failure: " + e.Message);
                if (e.InnerException != null && !string.IsNullOrWhiteSpace(e.InnerException.Message))
                    Console.WriteLine("--> " + e.InnerException.Message);

                return new List<string> { "Could not find " + location };
            }

            TimeData timeData;

            try
            {
                timeData = _timeLookup.Lookup(coordinates.Latitude, coordinates.Longitude);
            }
            catch(Exception e)
            {
                Console.WriteLine("Time Lookup failure: " + e.Message);
                if (e.InnerException != null && !string.IsNullOrWhiteSpace(e.InnerException.Message))
                    Console.WriteLine("--> " + e.InnerException.Message);

                return new List<string> { "Could not find Time for " + coordinates.Name };
            }

            response.Add($"Time for {coordinates.Name}: {TimeFromEpoch(timeData.timestamp)} {timeData.abbreviation}. GMT Offset {SecondsToHours(timeData.gmtOffset)} Hours.");

            return response;
        }

        private string TimeFromEpoch(int time)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            epoch = epoch.AddSeconds(time);

            return epoch.ToShortTimeString();
        }

        private String SecondsToHours(int seconds)
        {
            var time = TimeSpan.FromSeconds(seconds);

            return time.Hours.ToString();
        }

        public List<string> OnChannelMessage(IrcBot ircBot, string server, string channel, IrcMessage m)
        {
            return null;
        }

        public List<string> OnUserMessage(IrcBot ircBot, string server, IrcMessage m)
        {
            return null;
        }
    }
}

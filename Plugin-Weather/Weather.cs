using System;
using System.IO;
using System.Linq;
using ForecastIO;
using ForecastIO.Extensions;
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
        private ApiKeys _apiKeys;
        private GeoCodeLookup _lookup;

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

            _lookup = new GeoCodeLookup(_apiKeys.BingKey);
        }

        public List<string> IrcCommand(IrcBot ircBot, string server, IrcMessage m)
        {
            var text = m.Text.Trim();
            var messageParts = text.Split(' ');
            List<string> response;
            var user = m.Sender;

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
                if (command.ToLower() == "forecast")
                {
                    if (_usersLocations.UserLocations.ContainsKey(user))
                    {
                        response = WeatherForecast(_usersLocations.UserLocations[user]);
                    }
                    else
                    {
                        response = HelpText();
                    }
                }else if (command.ToLower() == "remember")
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

                if (command.ToLower() == "forecast")
                {
                    response = WeatherForecast(string.Join(" ", messageParts.Skip(2)));
                }
                else if (command.ToLower() == "remember")
                {
                    response = Remember(m.Sender,string.Join(" ", messageParts.Skip(2)));
                }
                else
                {
                    response = WeatherToday(string.Join(" ", messageParts.Skip(1)));
                }
            }

            return response;
        }

        public List<string> OnChannelMessage(IrcBot ircBot, string server, string channel, IrcMessage m)
        {
            return null;
        }

        public List<string> OnUserMessage(IrcBot ircBot, string server, IrcMessage m)
        {
            return null;
        }


        private List<string> WeatherToday(string location)
        {

            List<string> output;
            Coordinates coordinates;

            try
            {
                coordinates = _lookup.Lookup(location);
            }
            catch
            {                
               return new List<string> {"Could not find " + location };
            }

            //output = new List<string>{string.Format("{0} by {1}", coordinates.Latitude,coordinates.Longitude)};

            var weatherRequest = new ForecastIORequest(_apiKeys.ForecastIoKey,coordinates.Latitude, coordinates.Longitude, DateTime.Now, Unit.si);
            ForecastIOResponse weather;

            try
            {
                weather = weatherRequest.Get();
            }
            catch
            {
                return new List<string> { "Found " + location + " but could not find any weather there."};
            }
               
            var wToday = weather.currently;

            output = new List<string> { string.Format("Weather for {0}: {1} and {2}, {3}% Humidity and {4} Winds.", 
                location, Temp(wToday.temperature), wToday.summary, wToday.humidity  * 100,  Windspeed(wToday.windSpeed) )};

            return output;
        }

        private List<string> WeatherForecast(string location)
        {
            List<string> output = new List<string>();
            Coordinates coordinates;

            try
            {
                coordinates = _lookup.Lookup(location);
            }
            catch
            {
                return new List<string> { "Could not find " + location };
            }

            //output = new List<string>{string.Format("{0} by {1}", coordinates.Latitude,coordinates.Longitude)};

            var extendBlocks = new Extend[] 
            {
                Extend.hourly
            };

            var weatherRequest = new ForecastIORequest(_apiKeys.ForecastIoKey, coordinates.Latitude, coordinates.Longitude, Unit.si, extendBlocks);
            
            ForecastIOResponse weather;

            try
            {
                weather = weatherRequest.Get();
            }
            catch
            {
                return new List<string> { "Found " + location + " but could not find any weather there." };
            }

            output.Add("3 day forecast for: " + location);


            var dailyWeather = weather.daily.data.Skip(2).Take(3);

            foreach (var dayWeather in dailyWeather)
            {
                output.Add(string.Format("{0}: {1} {2} to {3}",
                                         dayWeather.time.ToDateTime().DayOfWeek,
                                         dayWeather.summary,
                                         Temp(dayWeather.temperatureMin),
                                         Temp(dayWeather.temperatureMax) ));
            }
                     
            return output;
        }

        private string Windspeed(float windSpeedKm)
        {
            var windSpeedM = windSpeedKm * 0.62137;

            return string.Format("{0} km/h ({1} mp/h)",
                Math.Round(windSpeedKm,MidpointRounding.AwayFromZero),
                Math.Round(windSpeedM,MidpointRounding.AwayFromZero) );
        }

        private string Temp(float temperatureC)
        {
            var temperatureF = temperatureC * 9/5 + 32;

            return string.Format("{0}°C ({1}°F)", 
                Math.Round(temperatureC,MidpointRounding.AwayFromZero), 
                Math.Round(temperatureF,MidpointRounding.AwayFromZero) );
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

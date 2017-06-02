using System;
using System.Globalization;
using System.Net;
using System.Threading;
using Newtonsoft.Json;

namespace Plugin_Weather
{
    class WeatherLookup
    {
        private string _key;
        private float _lat;
        private float _long;             

        public WeatherLookup(string key, float lat, float lon)
        {
            _key = key;
            _lat = lat;
            _long = lon;            
        }

        public WeatherData Get(bool extend=false)
        {
            //Change float tostring to use a decimal point instead of a comma
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-GB");

            var requestString = $"https://api.darksky.net/forecast/{_key}/{_lat},{_long}?units=si";
            if (extend)
            {
                requestString += "&extend=hourly";
            }

            var json = new WebClient().DownloadString(requestString);
            var locationData = JsonConvert.DeserializeObject<WeatherData>(json);
            
            return locationData;
        }
    }
}

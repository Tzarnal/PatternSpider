using System.Globalization;
using System.Net;
using System.Threading;
using Newtonsoft.Json;

namespace Plugin_Time
{
    class TimeLookup
    {
        private string _key;

        public TimeLookup(string key)
        {
            _key = key;
        }

        public TimeData Lookup(float lat, float lon)
        {
            //Change float tostring to use a decimal point instead of a comma
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-GB");

            var requestString = $"http://api.timezonedb.com/v2/get-time-zone?key={_key}&format=json&by=position&lat={lat}&lng={lon}";

            var json = new WebClient().DownloadString(requestString);
            var timeData = JsonConvert.DeserializeObject<TimeData>(json);

            return timeData;
        }
    }
}

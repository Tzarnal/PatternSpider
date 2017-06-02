using System.Collections.Generic;
using System.Linq;
using System.Net;
using Newtonsoft.Json;


namespace Plugin_Weather
{
    struct Coordinates
    {
        public float Latitude;
        public float Longitude;
        public string Name;
    }
   
    class GeoCodeLookup
    {
        private string _key;

        private Dictionary<string, Coordinates> _cache;        

        public GeoCodeLookup(string key)
        {
            _cache = new Dictionary<string, Coordinates>();
            _key = key;
        }

        public Coordinates Lookup(string location)
        {
            if (_cache.ContainsKey(location))
            {
                return _cache[location];
            }
            
            var coordinates = new Coordinates();

            var requestString = $"http://www.mapquestapi.com/geocoding/v1/address?key={_key}&location={location}";

            var json = new WebClient().DownloadString(requestString);
            var locationData = JsonConvert.DeserializeObject<GeoLocationData>(json);
            
            var locationResult = locationData.results.First().locations.First();
            
            coordinates.Latitude = (float) locationResult.latLng.lat;
            coordinates.Longitude = (float) locationResult.latLng.lng;
            coordinates.Name = locationResult.ToString();

            _cache.Add(location,coordinates);

            return coordinates;
        }        
    }
}

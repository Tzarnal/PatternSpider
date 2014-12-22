using System.Collections.Generic;
using System.Linq;
using Geocoding;
using Geocoding.MapQuest;

namespace Plugin_Weather
{
    struct Coordinates
    {
        public double Latitude;
        public double Longitude;       
    }
    
    class GeoCodeLookup
    {
        private Dictionary<string, Coordinates> _cache;
        private IGeocoder _gcoder;

        public GeoCodeLookup(string mapQuestKey)
        {
            _cache = new Dictionary<string, Coordinates>();
            _gcoder = new MapQuestGeocoder(mapQuestKey);
        }

        public Coordinates Lookup(string location)
        {
            if (_cache.ContainsKey(location))
            {
                return _cache[location];
            }
            
            var coordinates = new Coordinates();

            var qLocation = _gcoder.Geocode(location);
            var qCoordinates = qLocation.First().Coordinates;


            coordinates.Latitude = qCoordinates.Latitude;
            coordinates.Longitude = qCoordinates.Longitude;

            _cache.Add(location,coordinates);

            return coordinates;
        }
    }
}

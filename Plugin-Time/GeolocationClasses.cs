using System.Collections.Generic;


namespace Plugin_Time
{
    public class Copyright
    {
        public string text { get; set; }
        public string imageUrl { get; set; }
        public string imageAltText { get; set; }
    }

    public class Info
    {
        public int statuscode { get; set; }
        public Copyright copyright { get; set; }
        public IList<object> messages { get; set; }
    }

    public class Options
    {
        public int maxResults { get; set; }
        public bool thumbMaps { get; set; }
        public bool ignoreLatLngInput { get; set; }
    }

    public class ProvidedLocation
    {
        public string location { get; set; }
    }

    public class LatLng
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class DisplayLatLng
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class Location
    {
        public string street { get; set; }
        public string adminArea6 { get; set; }
        public string adminArea6Type { get; set; }
        public string adminArea5 { get; set; }
        public string adminArea5Type { get; set; }
        public string adminArea4 { get; set; }
        public string adminArea4Type { get; set; }
        public string adminArea3 { get; set; }
        public string adminArea3Type { get; set; }
        public string adminArea2 { get; set; }
        public string adminArea2Type { get; set; }
        public string adminArea1 { get; set; }
        public string adminArea1Type { get; set; }
        public string postalCode { get; set; }
        public string geocodeQualityCode { get; set; }
        public string geocodeQuality { get; set; }
        public bool dragPoint { get; set; }
        public string sideOfStreet { get; set; }
        public string linkId { get; set; }
        public string unknownInput { get; set; }
        public string type { get; set; }
        public LatLng latLng { get; set; }
        public DisplayLatLng displayLatLng { get; set; }
        public string mapUrl { get; set; }

        public override string ToString()
        {
            var locationElements = new List<string>();

            if (!string.IsNullOrWhiteSpace(adminArea1))
            {
                locationElements.Add(adminArea1);
            }

            if (!string.IsNullOrWhiteSpace(adminArea2))
            {
                locationElements.Add(adminArea2);
            }

            if (!string.IsNullOrWhiteSpace(adminArea3))
            {
                locationElements.Add(adminArea3);
            }

            if (!string.IsNullOrWhiteSpace(adminArea4))
            {
                locationElements.Add(adminArea5);
            }

            if (!string.IsNullOrWhiteSpace(adminArea5))
            {
                locationElements.Add(adminArea5);
            }

            if (!string.IsNullOrWhiteSpace(adminArea6))
            {
                locationElements.Add(adminArea6);
            }

            if (!string.IsNullOrWhiteSpace(street))
            {
                locationElements.Add(street);
            }

            return string.Join(", ",locationElements);
        }
    }

    public class Result
    {
        public ProvidedLocation providedLocation { get; set; }
        public IList<Location> locations { get; set; }
    }

    public class GeoLocationData
    {
        public Info info { get; set; }
        public Options options { get; set; }
        public IList<Result> results { get; set; }
    }
}

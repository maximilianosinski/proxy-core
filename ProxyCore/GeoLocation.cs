namespace ProxyCore;

public class GeoLocation
{
    public string City { get; }
    public string Country { get; }

    public GeoLocation(string city, string country)
    {
        City = city;
        Country = country;
    }
}
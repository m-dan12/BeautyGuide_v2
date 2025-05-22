namespace BeautyGuide_v2.Models;

public class Salon
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string PhoneNumber { get; set; }
    public bool HasParking { get; set; }
    public string WorkingHours { get; set; }
    public string Description { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}
using System;

namespace BeautyGuide_v2.Models;

public class Service
{
    public string Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public double Rating { get; set; }
    public double LeadTime { get; set; }
    public DateTime CreatedAt { get; set; }
    public int PopularityScore { get; set; }
    public bool HomeVisit { get; set; }
    public string MasterId { get; set; }
    public string SalonId { get; set; }
    public string CategoryId { get; set; }
}
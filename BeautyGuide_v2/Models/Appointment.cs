using System;

namespace BeautyGuide_v2.Models;

public class Appointment
{
    public string Id { get; set; }
    public string ClientId { get; set; }
    public string MasterId { get; set; }
    public string ServiceId { get; set; }
    public string SalonId { get; set; }
    public DateTime AppointmentDateTime { get; set; }
    public string Status { get; set; }
}
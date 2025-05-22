using System;

namespace BeautyGuide_v2.Models;

public class Appointment
{
    public string Id { get; set; }
    public string ClientId { get; set; }
    public string MasterId { get; set; }
    public string ServiceId { get; set; }
    public string SalonId { get; set; }
    public string FullName { get; set; }
    public string PhoneNumber { get; set; }
    public string AppointmentTime { get; set; }
    public DateTimeOffset AppointmentDate { get; set; }
    public string Status { get; set; }
}
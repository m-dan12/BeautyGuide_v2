using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using BeautyGuide_v2.Interfaces;
using BeautyGuide_v2.Models;

namespace BeautyGuide_v2.Services;

public class JsonDataService : IDataService
{
    private readonly string _dataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");

    public async Task<Master> GetMasterByIdAsync(string id)
    {
        var masters = await LoadMastersAsync();
        return masters.FirstOrDefault(m => m.Id == id);
    }
    
    public async Task<Salon> GetSalonByIdAsync(string id)
    {
        var salons = await LoadSalonsAsync();
        return salons.FirstOrDefault(s => s.Id == id);
    }
    
    public async Task<List<Service>> GetServicesByMasterIdAsync(string masterId)
    {
        var services = await LoadServicesAsync();
        return services.Where(s => s.MasterId == masterId).ToList();
    }
    
    private async Task<List<Master>> LoadMastersAsync()
    {
        var filePath = Path.Combine(_dataPath, "masters.json");
        var json = await File.ReadAllTextAsync(filePath);
        return JsonSerializer.Deserialize<List<Master>>(json);
    }
    
    private async Task<List<Salon>> LoadSalonsAsync()
    {
        var filePath = Path.Combine(_dataPath, "salons.json");
        var json = await File.ReadAllTextAsync(filePath);
        return JsonSerializer.Deserialize<List<Salon>>(json);
    }
    
    private async Task<List<Service>> LoadServicesAsync()
    {
        var filePath = Path.Combine(_dataPath, "services.json");
        var json = await File.ReadAllTextAsync(filePath);
        return JsonSerializer.Deserialize<List<Service>>(json);
    }
    
    public async Task<List<Category>> GetCategoriesAsync()
    {
        var filePath = Path.Combine(_dataPath, "categories.json");
        var json = await File.ReadAllTextAsync(filePath);
        return JsonSerializer.Deserialize<List<Category>>(json);
    }
    
    public async Task<List<Master>> GetMastersAsync()
    {
        var filePath = Path.Combine(_dataPath, "masters.json");
        var json = await File.ReadAllTextAsync(filePath);
        return JsonSerializer.Deserialize<List<Master>>(json);
    }

    public async Task<List<Salon>> GetSalonsAsync()
    {
        var filePath = Path.Combine(_dataPath, "salons.json");
        var json = await File.ReadAllTextAsync(filePath);
        return JsonSerializer.Deserialize<List<Salon>>(json);
    }

    public async Task<List<Service>> GetServicesAsync()
    {
        var filePath = Path.Combine(_dataPath, "services.json");
        var json = await File.ReadAllTextAsync(filePath);
        return JsonSerializer.Deserialize<List<Service>>(json);
    }

    public async Task<List<SalonPhoto>> GetSalonPhotosAsync()
    {
        var filePath = Path.Combine(_dataPath, "salon_photos.json");
        var json = await File.ReadAllTextAsync(filePath);
        return JsonSerializer.Deserialize<List<SalonPhoto>>(json);
    }

    public async Task<List<ServicePhoto>> GetServicePhotosAsync()
    {
        var filePath = Path.Combine(_dataPath, "service_photos.json");
        var json = await File.ReadAllTextAsync(filePath);
        return JsonSerializer.Deserialize<List<ServicePhoto>>(json);
    }

    public async Task<List<Appointment>> GetAppointmentsAsync()
    {
        var filePath = Path.Combine(_dataPath, "appointments.json");
        if (!File.Exists(filePath)) await File.WriteAllTextAsync(filePath, "[]");
        var json = await File.ReadAllTextAsync(filePath);
        return JsonSerializer.Deserialize<List<Appointment>>(json) ?? [];
    }
    public async Task<Service> GetServiceByIdAsync(string id)
    {
        var services = await LoadServicesAsync();
        return services.FirstOrDefault(s => s.Id == id);
    }
    public async Task AddAppointmentAsync(Appointment appointment)
    {
        var appointments = await GetAppointmentsAsync();
        appointments.Add(appointment);
        var filePath = Path.Combine(_dataPath, "appointments.json");
        await File.WriteAllTextAsync(filePath, JsonSerializer.Serialize(appointments, new JsonSerializerOptions { WriteIndented = true }));
        Console.WriteLine("Теперь точно записали");
    }
}
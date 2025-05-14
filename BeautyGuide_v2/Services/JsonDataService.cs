using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using BeautyGuide_v2.Interfaces;
using BeautyGuide_v2.Models;

namespace BeautyGuide_v2.Services;

public class JsonDataService : IDataService
{
    private readonly string _dataPath;

    public JsonDataService()
    {
        _dataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
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
        var json = await File.ReadAllTextAsync(filePath);
        return JsonSerializer.Deserialize<List<Appointment>>(json);
    }
}
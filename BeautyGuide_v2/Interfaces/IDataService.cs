using System.Collections.Generic;
using System.Threading.Tasks;
using BeautyGuide_v2.Models;

namespace BeautyGuide_v2.Interfaces;

public interface IDataService
{
    Task<List<Category>> GetCategoriesAsync();
    Task<List<Master>> GetMastersAsync();
    Task<List<Salon>> GetSalonsAsync();
    Task<List<Service>> GetServicesAsync();
    Task<List<SalonPhoto>> GetSalonPhotosAsync();
    Task<List<ServicePhoto>> GetServicePhotosAsync();
    Task AddAppointmentAsync(Appointment appointment);
    Task<List<Appointment>> GetAppointmentsAsync();
    Task<Master> GetMasterByIdAsync(string id); // Мастер по ID
    Task<Salon> GetSalonByIdAsync(string id);   // Салон по ID
    Task<List<Service>> GetServicesByMasterIdAsync(string masterId); // Услуги мастера
    Task<Service> GetServiceByIdAsync(string id);
}
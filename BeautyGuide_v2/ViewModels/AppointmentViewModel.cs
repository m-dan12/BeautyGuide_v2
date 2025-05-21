using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using BeautyGuide_v2.Interfaces;
using BeautyGuide_v2.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Contexts;
using ReactiveUI.Validation.Extensions;

namespace BeautyGuide_v2.ViewModels;

public class AppointmentViewModel : ViewModelBase, IInitializableViewModel, IValidatableViewModel
{
    private readonly INavigationService _navigationService;
    private readonly IDataService _dataService;
    private readonly IImageLoaderService _imageLoaderService;
    
    private Service _service;
    private Master _master;
    private Salon _salon;
    private string _photoUrl;
    

    public string ServiceName => _service.Name;
    public decimal ServicePrice => _service.Price;
    public Bitmap ServicePhoto => _imageLoaderService.LoadImage(_photoUrl);
    public string MasterName => _master.FullName;
    public string SalonName => _salon.Name;
    public List<string> AvailableTimes { get; } = ["10:00", "11:00", "12:00", "13:00", "14:00"];
    
    [Reactive] public DateTimeOffset? SelectedDate { get; set; }
    [Reactive] public string SelectedTime { get; set; }
    [Reactive] public string FullName { get; set; }
    [Reactive] public string PhoneNumber { get; set; }
    public void CloseCommand() => _navigationService.ClosePopup();

    public ReactiveCommand<Unit, Unit> Submit { get; set; }

    public AppointmentViewModel(INavigationService navigationService, IDataService dataService, IImageLoaderService imageLoaderService)
    {
        _navigationService = navigationService;
        _dataService = dataService;
        _imageLoaderService = imageLoaderService;
        
        // Валидация
        this.ValidationRule(
            vm => vm.SelectedDate,
            date => date.HasValue,
            "Пожалуйста, выберите дату.");

        this.ValidationRule(
            vm => vm.SelectedTime,
            time => !string.IsNullOrWhiteSpace(time),
            "Пожалуйста, выберите время.");

        this.ValidationRule(
            vm => vm.FullName,
            name => !string.IsNullOrWhiteSpace(name),
            "Пожалуйста, введите ФИО.");


        this.ValidationRule(
            vm => vm.PhoneNumber,
            phone => !string.IsNullOrWhiteSpace(phone),
            "Пожалуйста, введите номер телефона.");
        
        var canSubmit = this.IsValid(); // Проверяет, что все валидации пройдены
        
        Submit = ReactiveCommand.CreateFromTask(BookAppointmentAsync, canSubmit);
    }

    public async Task LoadAsync(string parameter)
    {
        if (!string.IsNullOrEmpty(parameter))
        {
            var service = await _dataService.GetServiceByIdAsync(parameter); // Предполагаемый метод
            if (service != null)
            {
                _service = service;
                var photos = await _dataService.GetServicePhotosAsync();
                _photoUrl = photos.FirstOrDefault(p => p.ServiceId == parameter).Url;
                _salon = await _dataService.GetSalonByIdAsync(service.SalonId);
                _master = await _dataService.GetMasterByIdAsync(service.MasterId);
            }
        }
    }
    private async Task BookAppointmentAsync()
    {
        if (SelectedDate.HasValue && !string.IsNullOrWhiteSpace(SelectedTime) && !string.IsNullOrWhiteSpace(FullName) && !string.IsNullOrWhiteSpace(PhoneNumber))
        {
            var service = await _dataService.GetServiceByIdAsync(_service.Id); // Используем ServiceName как индикатор
            if (service != null)
            {
                var appointment = new Appointment
                {
                    Id = Guid.NewGuid().ToString(),
                    ClientId = Guid.NewGuid().ToString(), // Можно заменить на реальный ClientId
                    MasterId = service.MasterId,
                    ServiceId = service.Id,
                    SalonId = service.SalonId,
                    AppointmentDateTime = DateTime.Parse(SelectedDate.Value.ToString()) + TimeSpan.Parse(SelectedTime),
                    Status = "Scheduled"
                };

                await _dataService.AddAppointmentAsync(appointment);
                _navigationService.ClosePopup();
                Console.WriteLine("Записали");
            }
    
        }
        else Console.WriteLine("Не записали");
    }

    public IValidationContext ValidationContext { get; }
}
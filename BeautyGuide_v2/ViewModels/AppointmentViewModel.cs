using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using BeautyGuide_v2.Interfaces;
using BeautyGuide_v2.Models;
using Avalonia.ReactiveUI;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Contexts;
using ReactiveUI.Validation.Extensions;
using ShimSkiaSharp;

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
    
    public IValidationContext ValidationContext { get; } = new ValidationContext();
    
    public string ServiceName => _service.Name;
    public decimal ServicePrice => _service.Price;
    public Bitmap ServicePhoto => _imageLoaderService.LoadImage(_photoUrl);
    public string MasterName => _master.FullName;
    public string SalonName => _salon.Name;
    private readonly List<string> _availableTimes = [
        "09:00", "10:00", "11:00",
        "12:00", "13:00", "14:00",
        "15:00", "16:00", "17:00",
        "18:00", "19:00", "20:00"
    ];
    [Reactive] public List<string> AvailableTimes { get; private set; } = [];
    
    [Reactive] public DateTimeOffset? SelectedDate { get; set; }
    [Reactive] public string SelectedTime { get; set; }
    [Reactive] public string FullName { get; set; }
    [Reactive] public string PhoneNumber { get; set; }
    public ReactiveCommand<Unit, Unit> Close { get; set; }
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
        
        this.WhenAnyValue(vm => vm.SelectedDate)
            .Where(d => d.HasValue)
            .Subscribe(async _ => await RefreshAvailableTimesAsync());  

        
        Submit = ReactiveCommand.CreateFromTask(BookAppointmentAsync, canSubmit);
        Close = ReactiveCommand.Create(() => _navigationService.ClosePopup());
    }
    
    private async Task RefreshAvailableTimesAsync()
    {
        if (_service is null || !SelectedDate.HasValue)
        {
            AvailableTimes = new(_availableTimes);           // показываем всё
            return;
        }

        var appointments = await _dataService.GetAppointmentsAsync();

        var booked = appointments
            .Where(a => a.ServiceId == _service.Id &&
                        a.AppointmentDate.Date == SelectedDate.Value.Date)
            .Select(a => a.AppointmentTime)
            .ToHashSet();

        AvailableTimes = _availableTimes
            .Where(t => !booked.Contains(t))
            .ToList();
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
                
                await RefreshAvailableTimesAsync();
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
                    FullName = FullName,
                    PhoneNumber = PhoneNumber,
                    MasterId = service.MasterId,
                    ServiceId = service.Id,
                    SalonId = service.SalonId,
                    AppointmentTime = SelectedTime,
                    AppointmentDate = SelectedDate.Value,
                    Status = "Scheduled"
                };

                await _dataService.AddAppointmentAsync(appointment);
                
                await RefreshAvailableTimesAsync();
                _navigationService.ClosePopup();
                Console.WriteLine("Записали");
            }
    
        }
        else Console.WriteLine("Не записали");
    }

}
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Avalonia.Media.Imaging;
using BeautyGuide_v2.Interfaces;
using BeautyGuide_v2.ViewModels;
using ReactiveUI;

namespace BeautyGuide_v2.Models;

public class ServiceCard
{
    private readonly INavigationService _navigationService;   
    public Service Service { get; set; }
    public List<string> Photos { get; set; }
    public Master Master { get; set; }
    public Salon Salon { get; set; }
    public Bitmap MainPhoto { get; set; }
    public Bitmap MasterPhoto { get; set; }

    public void GoToMaster() => _navigationService.NavigateTo<MasterViewModel>(Master.Id);
    public void GoToSalon() => _navigationService.NavigateTo<SalonViewModel>(Salon.Id);
    public async void BookAppointment() {
        if (Service != null && !string.IsNullOrEmpty(Service.Id))
        {
            await _navigationService.ShowPopup<AppointmentViewModel>(Service.Id);
        }
        else
        {
            Console.WriteLine("Не удалось определить ID услуги для записи.");
        }
    }
    public ServiceCard(Service service, List<string> photos, Master master, Salon salon, INavigationService navigationService)
    {
        _navigationService = navigationService;
        Service = service;
        Photos = photos;
        Master = master;
        Salon = salon;
    }
    [JsonConstructor]
    public ServiceCard(Service service, List<string> photos, Master master, Salon salon)
    {
        Service = service;
        Photos  = photos;
        Master  = master;
        Salon   = salon;
    }

}
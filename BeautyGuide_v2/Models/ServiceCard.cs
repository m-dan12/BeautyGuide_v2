using System.Collections.Generic;
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
    public ServiceCard(Service service, List<string> photos, Master master, Salon salon, INavigationService navigationService)
    {
        _navigationService = navigationService;
        Service = service;
        Photos = photos;
        Master = master;
        Salon = salon;
    }
}
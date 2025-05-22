using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using BeautyGuide_v2.Interfaces;
using ReactiveUI;

namespace BeautyGuide_v2.ViewModels;

public class SalonViewModel : ViewModelBase, IInitializableViewModel
{
    private readonly IDataService _dataService;
    private readonly INavigationService _navigationService;
    private readonly IImageLoaderService _imageLoaderService;

    public string Name { get; set; }
    public string Description { get; set; }
    public string PhoneNumber { get; set; }
    public string Address { get; set; }
    public Bitmap Photo { get; set; }

    public void GoBackCommand() => _navigationService.GoBack();

    public SalonViewModel(IDataService dataService, INavigationService navigationService, IImageLoaderService imageLoaderService)
    {
        _dataService = dataService;
        _navigationService = navigationService;
        _imageLoaderService = imageLoaderService;
    }

    public async Task LoadAsync(string salonId)
    {
        var salon = await _dataService.GetSalonByIdAsync(salonId);
        if (salon == null) return;
        Name = salon.Name;
        Description = salon.Description;
        PhoneNumber = salon.PhoneNumber;
        Address = salon.Address;
    }
}
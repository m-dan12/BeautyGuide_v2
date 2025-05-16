using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using BeautyGuide_v2.Interfaces;
using BeautyGuide_v2.Models;
using ReactiveUI;

namespace BeautyGuide_v2.ViewModels;

public class MasterViewModel : ViewModelBase, IInitializableViewModel
{
    private readonly IDataService _dataService;
    private readonly IImageLoaderService _imageLoaderService;
    private readonly INavigationService _navigationService;
    private string _salonId;

    public string FullName { get; set; }
    public Bitmap MasterPhoto { get; set; }
    public ObservableCollection<Service> Services { get; set; }
    public void GoToSalonCommand() => _navigationService.NavigateTo<SalonViewModel>(_salonId);
    public void GoBackCommand() => _navigationService.GoBack();

    public MasterViewModel(IDataService dataService, IImageLoaderService imageLoaderService, INavigationService navigationService)
    {
        _dataService = dataService;
        _imageLoaderService = imageLoaderService;
        _navigationService = navigationService;
    }

    public async Task LoadAsync(string masterId)
    {
        var master = await _dataService.GetMasterByIdAsync(masterId);
        if (master != null)
        {
            FullName = master.FullName;
            MasterPhoto = _imageLoaderService.LoadImage(master.Photo);
            Services = new ObservableCollection<Service>(await _dataService.GetServicesByMasterIdAsync(masterId));
            _salonId = master.SalonId;
        }
    }
}
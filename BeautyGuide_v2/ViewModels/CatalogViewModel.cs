using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ReactiveUI;
using BeautyGuide_v2.Interfaces;
using BeautyGuide_v2.Models;
using Newtonsoft.Json;

namespace BeautyGuide_v2.ViewModels;

public class CatalogViewModel : ViewModelBase, IInitializableViewModel
{
    private readonly IDataService _dataService;
    private readonly IImageLoaderService _imageLoaderService;
    private readonly INavigationService _navigationService;

    private ObservableCollection<ServiceCard> _allServices;
    private ObservableCollection<ServiceCard> _filteredServices;
    private List<Category> _categories;
    private Category _selectedCategory;
    private decimal _minPrice;
    private decimal _maxPrice;
    private int _sortBy;
    private bool _isDataLoaded; // Флаг для отслеживания загрузки данных
    private int _selectedGender; // пол: 1: "Муж", 2: "Жен", или 0: null
    private bool _hasHomeVisit;   // вызов на дом: true/false/null
    private bool _hasParking;     // парковка: true/false/null
    private bool _isAllCategoriesSelected = true;
    private string _searchQuery;
    private string _salonsJson;
    private ObservableCollection<string> _salonNames = new();
    private string _selectedSalonName;
    public List<Salon> Salons { get; private set; } = [];

    public string CurrentAddress { get; private set; } =
        "https://yandex.ru/maps/51/samara/search/%D1%81%D0%B0%D0%BB%D0%BE%D0%BD%D1%8B%20%D0%BA%D1%80%D0%B0%D1%81%D0%BE%D1%82%D1%8B/?ll=50.133072%2C53.183910&sll=50.100199%2C53.195876&sspn=0.474129%2C0.178961&z=13";
    public CatalogViewModel(IDataService dataService, IImageLoaderService imageLoaderService, INavigationService тavigationService)
    {
        _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
        _imageLoaderService = imageLoaderService ?? throw new ArgumentNullException(nameof(imageLoaderService));
        _navigationService = тavigationService ?? throw new ArgumentNullException(nameof(imageLoaderService));
        _allServices = [];
        _filteredServices = [];
        _categories = [];
        _isDataLoaded = false;

        // Реактивное обновление при изменении фильтров
        this.WhenAnyValue(
                x => x.SelectedCategory, x => x.MinPrice, x => x.MaxPrice,
                x => x.SelectedGender, x => x.HasHomeVisit, x => x.HasParking,
                x => x.IsAllCategoriesSelected, x => x.SelectedSalonName // Добавляем фильтр по салонам
            )
            .Throttle(TimeSpan.FromMilliseconds(300))
            .Where(_ => _isDataLoaded) // Выполнять только после загрузки данных
            .Subscribe(_ => ApplyFilters());
        
        this.WhenAnyValue(x => x.SearchQuery)
            .Throttle(TimeSpan.FromMilliseconds(300))
            .DistinctUntilChanged()          // лишние вызовы не нужны
            .Where(_ => _isDataLoaded)
            .Subscribe(_ => ApplyFilters());


        this.WhenAnyValue(x => x.SortBy)
            .Where(_ => _isDataLoaded) // Выполнять только после загрузки данных
            .Subscribe(_ => ApplySorting());
        
        this.WhenAnyValue(x => x.SelectedCategory)
            .Subscribe((x) =>
            {
                if (x != null) IsAllCategoriesSelected = false;
            });

    }
    public string SalonsJson
    {
        get => _salonsJson;
        set => this.RaiseAndSetIfChanged(ref _salonsJson, value);
    }
    public ObservableCollection<ServiceCard> Services
    {
        get => _filteredServices;
        set => this.RaiseAndSetIfChanged(ref _filteredServices, value);
    }

    public List<Category> Categories
    {
        get => _categories;
        set => this.RaiseAndSetIfChanged(ref _categories, value);
    }

    public Category SelectedCategory
    {
        get => _selectedCategory;
        set => this.RaiseAndSetIfChanged(ref _selectedCategory, value);
    }

    public decimal MinPrice
    {
        get => _minPrice;
        set => this.RaiseAndSetIfChanged(ref _minPrice, value);
    }

    public decimal MaxPrice
    {
        get => _maxPrice;
        set => this.RaiseAndSetIfChanged(ref _maxPrice, value);
    }

    public int SortBy
    {
        get => _sortBy;
        set => this.RaiseAndSetIfChanged(ref _sortBy, value);
    }
    public int SelectedGender
    {
        get => _selectedGender;
        set => this.RaiseAndSetIfChanged(ref _selectedGender, value);
    }
    public bool HasHomeVisit
    {
        get => _hasHomeVisit;
        set => this.RaiseAndSetIfChanged(ref _hasHomeVisit, value);
    }
    public bool HasParking
    {
        get => _hasParking;
        set => this.RaiseAndSetIfChanged(ref _hasParking, value);
    }
    public bool IsAllCategoriesSelected
    {
        get => _isAllCategoriesSelected;
        set => this.RaiseAndSetIfChanged(ref _isAllCategoriesSelected, value);
    }
    public string SearchQuery
    {
        get => _searchQuery;
        set => this.RaiseAndSetIfChanged(ref _searchQuery, value);
    }

    public ObservableCollection<string> SalonNames
    {
        get => _salonNames;
        private set => this.RaiseAndSetIfChanged(ref _salonNames, value);
    }

    public string SelectedSalonName
    {
        get => _selectedSalonName;
        set => this.RaiseAndSetIfChanged(ref _selectedSalonName, value);
    }
    
    
    public void ResetCategoryFilter()
    {
        if (IsAllCategoriesSelected)
        {
            SelectedCategory = null; // Сбрасываем выбранную категорию
        }
        ApplyFilters(); // Применяем фильтры заново
    }


    public async Task LoadAsync(string parameter)
    {
        try
        {
            var services = await _dataService.GetServicesAsync();
            var masters = await _dataService.GetMastersAsync();
            var salons = await _dataService.GetSalonsAsync();
            var servicePhotos = await _dataService.GetServicePhotosAsync();
            var categories = await _dataService.GetCategoriesAsync();

            _allServices = new ObservableCollection<ServiceCard>(
                services.Select(s =>
                {
                    var master = masters.FirstOrDefault(m => m.Id == s.MasterId);
                    var salon = salons.FirstOrDefault(sal => sal.Id == s.SalonId);
                    Salons.Add(salon);
                    var photoUrl = servicePhotos.FirstOrDefault(p => p.ServiceId == s.Id)?.Url;
                    return new ServiceCard(s, servicePhotos.Where(p => p.ServiceId == s.Id).Select(p => p.Url).ToList(), master, salon, _navigationService)
                    {
                        MainPhoto = _imageLoaderService.LoadImage(photoUrl),
                        MasterPhoto = master != null ? _imageLoaderService.LoadImage(master.Photo) : null
                    };
                }));

            Services = new ObservableCollection<ServiceCard>(_allServices);
            Categories = categories;

            // Заполняем список салонов для фильтра
            SalonNames = new ObservableCollection<string>(
                _allServices
                    .Select(s => s.Salon.Name)
                    .Where(name => !string.IsNullOrEmpty(name))
                    .Distinct()
                    .OrderBy(n => n));

            _isDataLoaded = true;
            
            SalonsJson = JsonConvert.SerializeObject(Salons.Select(s => new
            {
                s.Name,
                s.Latitude,
                s.Longitude
            }));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка загрузки данных: {ex.Message}");
        }
    }

    private void ApplyFilters()
    {
        if (!_isDataLoaded) return; // Не применять фильтры, если данные не загружены

        var filtered = _allServices.AsEnumerable();

        if (SelectedCategory != null)
        {
            var categoryIds = GetAllSubCategoryIds(SelectedCategory);
            filtered = filtered.Where(s => GetAllSubCategoryIds(SelectedCategory).Contains(s.Service.CategoryId));
        }
        
        if (!string.IsNullOrWhiteSpace(SearchQuery))
            filtered = filtered.Where(s =>
                s.Service.Name.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase));

        if (MinPrice > 0)
            filtered = filtered.Where(s => s.Service.Price >= MinPrice);

        if (MaxPrice > 0)
            filtered = filtered.Where(s => s.Service.Price <= MaxPrice);
        
        if (SelectedGender != 0)
        {
            filtered = filtered.Where(s =>
                string.Equals(s.Master.Gender, SelectedGender == 1 ? "Муж" : "Жен", StringComparison.OrdinalIgnoreCase));
        }
        if (HasHomeVisit)
            filtered = filtered.Where(s => s.Service.HomeVisit);

        if (HasParking)
            filtered = filtered.Where(s => s.Salon.HasParking);

        // Фильтрация по названию салона
        if (!string.IsNullOrWhiteSpace(SelectedSalonName))
            filtered = filtered.Where(s => s.Salon.Name == SelectedSalonName);

        // Убедимся, что filtered не null
        Services = new ObservableCollection<ServiceCard>(filtered?.ToList() ?? []);
        ApplySorting();
    }

    private void ApplySorting()
    {
        if (!_isDataLoaded || Services == null) return; // Не сортировать, если данные не загружены или Services null

        var sorted = Services.AsEnumerable();
        
        sorted = SortBy switch
        {
            0 => sorted.OrderByDescending(s => s.Service.PopularityScore),
            1 => sorted.OrderByDescending(s => s.Service.CreatedAt),
            2 => sorted.OrderBy(s => s.Service.Price),
            3 => sorted.OrderByDescending(s => s.Service.Price),
            4 => sorted.OrderByDescending(s => s.Service.Rating),
            _ => sorted
        };

        // Убедимся, что sorted не null
        Services = new ObservableCollection<ServiceCard>(sorted?.ToList() ?? []);
    }

    private List<string> GetAllSubCategoryIds(Category category)
    {
        var ids = new List<string> { category.Id };
        foreach (var sub in category.SubCategories)
        {
            ids.AddRange(GetAllSubCategoryIds(sub));
        }
        return ids;
    }
}
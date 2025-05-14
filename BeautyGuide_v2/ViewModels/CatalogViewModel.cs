using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using Avalonia.Media.Imaging;
using ReactiveUI;
using BeautyGuide_v2.Interfaces;
using BeautyGuide_v2.Models;

namespace BeautyGuide_v2.ViewModels;

public class CatalogViewModel : ViewModelBase
{
    private readonly IDataService _dataService;

    private ObservableCollection<ServiceCard> _allServices;
    private ObservableCollection<ServiceCard> _filteredServices;
    private List<Category> _categories;
    private Category _selectedCategory;
    private decimal _minPrice;
    private decimal _maxPrice;
    private string _sortBy;
    private bool _isDataLoaded; // Флаг для отслеживания загрузки данных

    public CatalogViewModel(IDataService dataService)
    {
        _dataService = dataService;
        _allServices = new ObservableCollection<ServiceCard>();
        _filteredServices = new ObservableCollection<ServiceCard>();
        _categories = new List<Category>();
        _isDataLoaded = false;

        // Реактивное обновление при изменении фильтров
        this.WhenAnyValue(
                x => x.SelectedCategory,
                x => x.MinPrice,
                x => x.MaxPrice)
            .Throttle(TimeSpan.FromMilliseconds(300))
            .Where(_ => _isDataLoaded) // Выполнять только после загрузки данных
            .Subscribe(_ => ApplyFilters());

        this.WhenAnyValue(x => x.SortBy)
            .Where(_ => _isDataLoaded) // Выполнять только после загрузки данных
            .Subscribe(_ => ApplySorting());

        // Загрузка данных
        LoadDataAsync();
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

    public string SortBy
    {
        get => _sortBy;
        set => this.RaiseAndSetIfChanged(ref _sortBy, value);
    }

    private async void LoadDataAsync()
    {
        try
        {
            var services = await _dataService.GetServicesAsync();
            var masters = await _dataService.GetMastersAsync();
            var salons = await _dataService.GetSalonsAsync();
            var servicePhotos = await _dataService.GetServicePhotosAsync();
            var categories = await _dataService.GetCategoriesAsync();

            _allServices = new ObservableCollection<ServiceCard>(
                services.Select(s => new ServiceCard
                {
                    Service = s,
                    Photos = servicePhotos.Where(p => p.ServiceId == s.Id).Select(p => new Bitmap(p.Url)).ToList(),
                    Master = masters.FirstOrDefault(m => m.Id == s.MasterId),
                    Salon = salons.FirstOrDefault(sal => sal.Id == s.SalonId)
                })
            );

            Services = new ObservableCollection<ServiceCard>(_allServices);
            Categories = categories;
            _isDataLoaded = true; // Данные загружены, можно применять фильтры и сортировку
        }
        catch (Exception ex)
        {
            // Обработка ошибок загрузки данных
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
            filtered = filtered.Where(s => categoryIds.Contains(s.Service.CategoryId));
        }

        if (MinPrice > 0)
        {
            filtered = filtered.Where(s => s.Service.Price >= MinPrice);
        }

        if (MaxPrice > 0)
        {
            filtered = filtered.Where(s => s.Service.Price <= MaxPrice);
        }

        // Убедимся, что filtered не null
        Services = new ObservableCollection<ServiceCard>(filtered?.ToList() ?? new List<ServiceCard>());
        ApplySorting();
    }

    private void ApplySorting()
    {
        if (!_isDataLoaded || Services == null) return; // Не сортировать, если данные не загружены или Services null

        var sorted = Services.AsEnumerable();

        switch (SortBy)
        {
            case "Popularity":
                sorted = sorted.OrderByDescending(s => s.Service.PopularityScore);
                break;
            case "Newest":
                sorted = sorted.OrderByDescending(s => s.Service.CreatedAt);
                break;
            case "Price":
                sorted = sorted.OrderBy(s => s.Service.Price);
                break;
            case "Rating":
                sorted = sorted.OrderByDescending(s => s.Service.Rating);
                break;
            default:
                sorted = sorted; // Без сортировки
                break;
        }

        // Убедимся, что sorted не null
        Services = new ObservableCollection<ServiceCard>(sorted?.ToList() ?? new List<ServiceCard>());
    }

    private List<string> GetAllSubCategoryIds(Category category)
    {
        var ids = new List<string> { category.Id };
        foreach (var sub in category.SubCategories ?? new List<Category>())
        {
            ids.AddRange(GetAllSubCategoryIds(sub));
        }
        return ids;
    }
}
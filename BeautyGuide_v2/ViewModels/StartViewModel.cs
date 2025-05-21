using BeautyGuide_v2.Interfaces;

namespace BeautyGuide_v2.ViewModels;

public class StartViewModel(INavigationService navigationService) : ViewModelBase
{
    public void ToCatalog() => navigationService.NavigateTo<CatalogViewModel>();
}
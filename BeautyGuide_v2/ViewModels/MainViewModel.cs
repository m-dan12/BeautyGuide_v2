using BeautyGuide_v2.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using ReactiveUI.Fody.Helpers;

namespace BeautyGuide_v2.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [Reactive] public string Greeting { get; set; } = "Welcome to Avalonia!";
    public ViewModelBase CurrentViewModel { get; set; } = new CatalogViewModel(new JsonDataService());
}
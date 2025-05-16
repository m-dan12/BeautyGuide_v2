using System;
using System.Reactive;
using System.Threading;
using Avalonia.Threading;
using BeautyGuide_v2.Interfaces;
using BeautyGuide_v2.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace BeautyGuide_v2.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;
    [Reactive] public ViewModelBase CurrentViewModel { get; set; }

    #region Navigation

    public void ToStart()           => _navigationService.NavigateTo<StartViewModel>();
    public void ToCatalog()         => _navigationService.NavigateTo<CatalogViewModel>();
    public void ToTop()             => _navigationService.NavigateTo<TopViewModel>();
    public void ToAboutUs()         => _navigationService.NavigateTo<AboutUsViewModel>();
    public void ToGuide()           => _navigationService.NavigateTo<GuideViewModel>();
    public void ToContacts()        => _navigationService.NavigateTo<ContactsViewModel>();

    #endregion

    public MainViewModel(INavigationService navigationService, StartViewModel startViewModel)
    {
        _navigationService = navigationService;
        CurrentViewModel = startViewModel;
        (navigationService as NavigationService).SetMainViewModel(this);
    }
}
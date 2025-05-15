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
    [Reactive] public ViewModelBase? CurrentViewModel { get; set; }

    #region Команды

    public ReactiveCommand<Unit, Unit> ToStart           { get; }
    public ReactiveCommand<Unit, Unit> ToSalons          { get; }
    public ReactiveCommand<Unit, Unit> ToTop             { get; }
    public ReactiveCommand<Unit, Unit> ToAboutUs         { get; }
    public ReactiveCommand<Unit, Unit> ToRecommendations { get; }
    public ReactiveCommand<Unit, Unit> ToContacts        { get; }

    #endregion

    public MainViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;
        _navigationService.ViewModelChanged += viewModel => CurrentViewModel = viewModel;
        ToStart           = ReactiveCommand.Create(_navigationService.NavigateTo<StartViewModel>);
        ToSalons          = ReactiveCommand.Create(_navigationService.NavigateTo<CatalogViewModel>);
        ToTop             = ReactiveCommand.Create(_navigationService.NavigateTo<TopViewModel>);
        ToAboutUs         = ReactiveCommand.Create(_navigationService.NavigateTo<AboutUsViewModel>);
        ToRecommendations = ReactiveCommand.Create(_navigationService.NavigateTo<GuideViewModel>);
        
        _navigationService.NavigateTo<StartViewModel>();
    }
}
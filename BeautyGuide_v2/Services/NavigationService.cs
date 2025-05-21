using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;
using BeautyGuide_v2.Interfaces;
using BeautyGuide_v2.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace BeautyGuide_v2.Services;

public class NavigationService() : INavigationService
{
    private MainViewModel? _mainViewModel;
    private readonly Stack<ViewModelBase> _navigationStack = new Stack<ViewModelBase>();
    public event Action<ViewModelBase> ViewModelChanged;
    public void SetMainViewModel(MainViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
    }

    public async Task NavigateTo<TViewModel>(object parameter = null) where TViewModel : ViewModelBase
    {
        var viewModel = ServiceProvider.Services.GetService<TViewModel>();
        
        if (_mainViewModel == null)
            throw new InvalidOperationException("MainViewModel not set");
        
        if (viewModel is IInitializableViewModel initializable)
            await initializable.LoadAsync(parameter?.ToString());
        
        _navigationStack.Push(_mainViewModel.CurrentViewModel);
        _mainViewModel.CurrentViewModel = viewModel;
        ViewModelChanged?.Invoke(viewModel);
    }
    public void GoBack()
    {
        if (_navigationStack.Count <= 0) return;
        
        var previousViewModel = _navigationStack.Pop();
        _mainViewModel.CurrentViewModel = previousViewModel;
        ViewModelChanged?.Invoke(previousViewModel);
    }
    
    public async Task ShowPopup<TViewModel>(object parameter = null) where TViewModel : ViewModelBase
    {
        var viewModel = ServiceProvider.Services.GetService<TViewModel>();
        if (viewModel == null)
        {
            throw new InvalidOperationException($"Не удалось создать Popup ViewModel типа {typeof(TViewModel).Name}.");
        }

        if (viewModel is IInitializableViewModel initializable)
        {
            await initializable.LoadAsync(parameter?.ToString());
        }

        _mainViewModel.PopupViewModel = viewModel; // Устанавливаем PopupViewModel
    }

    public void ClosePopup()
    {
        _mainViewModel.PopupViewModel = null; // Сбрасываем PopupViewModel
    }
}
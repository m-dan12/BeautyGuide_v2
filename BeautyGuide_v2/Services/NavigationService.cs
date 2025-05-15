using System;
using System.Threading;
using Avalonia.Threading;
using BeautyGuide_v2.Interfaces;
using BeautyGuide_v2.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace BeautyGuide_v2.Services;

public sealed class NavigationService(IServiceProvider serviceProvider) : INavigationService
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public event Action<ViewModelBase> ViewModelChanged;

    public void NavigateTo<TViewModel>() where TViewModel : ViewModelBase
    {
        Console.WriteLine($"NavigateTo<{typeof(TViewModel).Name}> вызван в потоке {Thread.CurrentThread.ManagedThreadId} (IsThreadPoolThread: {Thread.CurrentThread.IsThreadPoolThread})");
        var viewModel = _serviceProvider.GetService<TViewModel>();
        if (viewModel != null)
        {
            ViewModelChanged?.Invoke(viewModel);
        }
    }
}
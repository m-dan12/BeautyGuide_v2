using System;
using System.Reflection;
using BeautyGuide_v2.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using BeautyGuide_v2.Services;
using BeautyGuide_v2.ViewModels;

namespace BeautyGuide_v2;

public static class ServiceProvider
{
    private static IServiceProvider? _serviceProvider;

    public static IServiceProvider Services
    {
        get => _serviceProvider ?? throw new InvalidOperationException("ServiceProvider not initialized");
        private set => _serviceProvider = value;
    }

    public static void Initialize()
    {
        var services = new ServiceCollection();

        // Регистрация сервисов
        services.AddSingleton<IDataService, JsonDataService>(); // Один экземпляр на приложение
        services.AddSingleton<IImageLoaderService, ImageLoaderService>();
        services.AddSingleton<INavigationService, NavigationService>();
        
        // Регистрация ViewModel-ей
        RegisterViewModels(services, Assembly.GetExecutingAssembly());

        // Построение провайдера сервисов
        Services = services.BuildServiceProvider();
    }
    
    private static void RegisterViewModels(IServiceCollection sc, Assembly asm)
    {
        foreach (var t in asm.GetTypes())
        {
            if (!t.Name.EndsWith("ViewModel") || t.IsAbstract) continue;

            if (t == typeof(MainViewModel))
                sc.AddSingleton(t);
            else
                sc.AddTransient(t);
        }
    }
}
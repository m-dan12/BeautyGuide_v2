using System;
using System.Reflection;
using BeautyGuide_v2.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using BeautyGuide_v2.Services;
using BeautyGuide_v2.ViewModels;

namespace BeautyGuide_v2;

public static class DependencySetup
{
    public static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        // Регистрация сервисов
        services.AddSingleton<IDataService, JsonDataService>(); // Один экземпляр на приложение
        services.AddSingleton<IImageLoaderService, ImageLoaderService>();
        services.AddSingleton<INavigationService, NavigationService>();
        
        // Регистрация ViewModel-ей
        RegisterViewModels(services, Assembly.GetExecutingAssembly());

        // Построение провайдера сервисов
        return services.BuildServiceProvider();
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
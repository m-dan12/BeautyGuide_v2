using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using BeautyGuide_v2.ViewModels;
using BeautyGuide_v2.Views;
using Microsoft.Extensions.DependencyInjection;

namespace BeautyGuide_v2;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var serviceProvider = DependencySetup.ConfigureServices();
        var mainViewModel = serviceProvider.GetService<MainViewModel>();
        switch (ApplicationLifetime)
        {
            case IClassicDesktopStyleApplicationLifetime desktop:
                
                Console.OutputEncoding = System.Text.Encoding.UTF8;
                DisableAvaloniaDataAnnotationValidation();
                
                desktop.MainWindow = new MainWindow
                {
                    DataContext = mainViewModel
                };
                
                break;
            case ISingleViewApplicationLifetime singleViewPlatform:
                
                singleViewPlatform.MainView = new MainView
                {
                    DataContext = mainViewModel
                };
                
                break;
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}
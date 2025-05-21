using System;
using System.Threading.Tasks;
using BeautyGuide_v2.ViewModels;

namespace BeautyGuide_v2.Interfaces;

public interface INavigationService
{
    Task NavigateTo<TViewModel>(object parameter = null) where TViewModel : ViewModelBase;
    void GoBack();
    Task ShowPopup<TViewModel>(object parameter = null) where TViewModel : ViewModelBase;
    void ClosePopup();
    
}
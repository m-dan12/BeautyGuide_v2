using System;
using BeautyGuide_v2.ViewModels;

namespace BeautyGuide_v2.Interfaces;

public interface INavigationService
{
    void NavigateTo<TViewModel>() where TViewModel : ViewModelBase;
    event Action<ViewModelBase> ViewModelChanged;
}
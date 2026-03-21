using System;
using AvaloniaDiApp.ViewModels;

namespace AvaloniaDiApp.Services;

public interface INavigationService
{
    event Action<ViewModelBase>? Navigated;
    void NavigateTo<T>() where T : ViewModelBase;
}

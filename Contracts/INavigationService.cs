using System;
using AvaloniaDiApp.ViewModels;

namespace AvaloniaDiApp.Contracts;

public interface INavigationService
{
    event Action<ViewModelBase>? Navigated;
    void NavigateTo<T>() where T : ViewModelBase;
}

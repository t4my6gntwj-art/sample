using System;
using Microsoft.Extensions.DependencyInjection;
using AvaloniaDiApp.ViewModels;
using AvaloniaDiApp.Services;

namespace AvaloniaDiApp.Services.Navigation;

public class NavigationService : INavigationService
{
    private readonly IServiceProvider _serviceProvider;

    public event Action<ViewModelBase>? Navigated;

    public NavigationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void NavigateTo<T>() where T : ViewModelBase
    {
        // ViewをDIコンテナから生成し、イベントを通じて通知する
        var viewModel = _serviceProvider.GetRequiredService<T>();
        Navigated?.Invoke(viewModel);
    }
}

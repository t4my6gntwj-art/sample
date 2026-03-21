using System;
using AvaloniaDiApp.ViewModels;

namespace AvaloniaDiApp.Services;

public interface IContentHolder
{
    ViewModelBase? CurrentViewModel { get; set; }
    event Action? CurrentViewModelChanged;
}

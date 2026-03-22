using System;
using AvaloniaDiApp.ViewModels;

namespace AvaloniaDiApp.Contracts;

public interface IContentHolder
{
    ViewModelBase? CurrentViewModel { get; set; }
    event Action? CurrentViewModelChanged;
}

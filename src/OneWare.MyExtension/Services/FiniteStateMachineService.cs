using OneWare.Essentials.Models;
using OneWare.Essentials.Services;
using OneWare.Essentials.Enums;
using OneWare.MyExtension.ViewModels;

namespace OneWare.MyExtension.Services;

public interface IFiniteStateMachineService
{
    Task ShowFiniteStateMachineAsync(IProjectFile jsonFile);
}

public class FiniteStateMachineService : IFiniteStateMachineService
{
    private readonly IDockService _dockService;
    private readonly IProjectExplorerService _projectExplorerService;
    private readonly IWindowService _windowService;

    public FiniteStateMachineService(IDockService dockService, IProjectExplorerService projectExplorerService, IWindowService windowService)
    {
        _dockService = dockService;
        _projectExplorerService = projectExplorerService;
        _windowService = windowService;
    }

    public Task ShowFiniteStateMachineAsync(IProjectFile jsonFile)
    {
        var viewModel = new FiniteStateMachineViewModel(jsonFile.FullPath, _projectExplorerService, _dockService, _windowService);
        _dockService.Show(viewModel, DockShowLocation.Document);
        
        return Task.CompletedTask;
    }
}
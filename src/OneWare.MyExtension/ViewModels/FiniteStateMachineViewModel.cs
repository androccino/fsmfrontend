using CommunityToolkit.Mvvm.ComponentModel;
using Dock.Model.Core;
using OneWare.Essentials.ViewModels;
using OneWare.Essentials.Services;

namespace OneWare.MyExtension.ViewModels;

public class FiniteStateMachineViewModel : ExtendedDocument, IDockable
{
    private string _filePath = string.Empty;

    public string FilePath
    {
        get => _filePath;
        set => SetProperty(ref _filePath, value);
    }


    public FiniteStateMachineViewModel(string filePath, IProjectExplorerService projectExplorerService, OneWare.Essentials.Services.IDockService dockService, IWindowService windowService) 
        : base(filePath, projectExplorerService, dockService, windowService)
    {
        FilePath = filePath;
        Title = $"Finite State Machine - {System.IO.Path.GetFileName(filePath)}";
        Id = $"FSM_{filePath}";
    }

    protected override void UpdateCurrentFile(OneWare.Essentials.Models.IFile? file)
    {
        // Implementation of abstract method
    }
}
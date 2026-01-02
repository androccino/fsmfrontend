# OneWare Studio Extension Development Guide

## Architecture Overview

This is a **plugin extension for OneWare Studio**, an IDE for hardware development. The extension follows a **modular Prism architecture** with Avalonia UI:

- **Module Entry**: [OneWareMyExtensionModule.cs](../src/OneWare.MyExtension/OneWareMyExtensionModule.cs) implements `IModule` with `RegisterTypes()` for DI and `OnInitialized()` for setup
- **MVVM Pattern**: ViewModels extend `ExtendedDocument` and implement `IDockable` for dockable panels
- **Service Layer**: Services registered as singletons in DI container (see `FiniteStateMachineService`)
- **View Binding**: Avalonia XAML views with compiled bindings (`AvaloniaUseCompiledBindingsByDefault=true`)

## Critical Dependencies

- **OneWare.Essentials** (0.11.11): Core framework providing services, models, ViewModels. All host services accessed via this package
- **Prism.Ioc/Prism.Modularity**: DI container and module system
- **Avalonia**: Cross-platform UI framework
- **CommunityToolkit.Mvvm**: Commands and observable properties

## Extension Registration Pattern

Extensions **must** be dynamically loadable (`<EnableDynamicLoading>true</EnableDynamicLoading>` in csproj). The module pattern:

```csharp
public void RegisterTypes(IContainerRegistry containerRegistry)
{
    // Register services as singletons with interface
    containerRegistry.RegisterSingleton<IYourService, YourService>();
}

public void OnInitialized(IContainerProvider containerProvider)
{
    // Register dockable views
    containerProvider.Resolve<IDockService>().RegisterLayoutExtension<YourViewModel>(DockShowLocation.Document);
    
    // Add context menu items via ProjectExplorerService
    containerProvider.Resolve<IProjectExplorerService>().RegisterConstructContextMenu((x,l) => {
        if (x is [IProjectFile { Extension: ".yourext" } file]) {
            l.Add(new MenuItemViewModel("ActionId") {
                Header = "Display Name",
                Command = new AsyncRelayCommand(async () => { /* action */ })
            });
        }
    });
}
```

## ViewModel Requirements

Documents must extend `ExtendedDocument` and implement `IDockable`:

```csharp
public class YourViewModel : ExtendedDocument, IDockable
{
    public YourViewModel(string filePath, IProjectExplorerService projectExplorerService, 
        IDockService dockService, IWindowService windowService) 
        : base(filePath, projectExplorerService, dockService, windowService)
    {
        Title = $"Your Title - {Path.GetFileName(filePath)}";
        Id = $"UniqueId_{filePath}"; // Unique ID for dock management
    }
    
    protected override void UpdateCurrentFile(IFile? file) { /* implementation */ }
}
```

## Build & Test Workflow

- **Build**: `dotnet build OneWare.MyExtension.sln --configuration Debug` (or use VS Code task "Build Solution")
- **Test**: Tests use xUnit in [tests/OneWare.MyExtension.UnitTests](../tests/OneWare.MyExtension.UnitTests/)
- **Output**: Extension DLL + `compatibility.txt` generated in `bin/Debug/net10.0/` listing dependencies
- **Target Framework**: net10.0 (preview framework)

## Extension Metadata

Configure [oneware-extension.json](../oneware-extension.json) for package manager:
- `id`: Must match project namespace
- `type`: "Plugin" for code extensions
- `category`: Determines marketplace grouping
- `versions[].targets`: Platform targeting (use "all" for cross-platform)
- `sourceUrl`: GitHub releases download path

## File Naming Conventions

- Services: `{FeatureName}Service.cs` in `Services/` with `I{FeatureName}Service` interface
- ViewModels: `{FeatureName}ViewModel.cs` in `ViewModels/` extending `ExtendedDocument`
- Views: `{FeatureName}View.axaml` + `.axaml.cs` in `Views/`
- Namespace pattern: `OneWare.MyExtension.{Services|ViewModels|Views}`

## Common Patterns

**Context Menu Registration**: File extension pattern matching uses `IProjectFile { Extension: ".ext" }` with pattern matching syntax

**Service Access**: Always resolve host services via `containerProvider.Resolve<IService>()` - never instantiate directly

**Dependency Management**: Mark host packages with `Private="false" ExcludeAssets="runtime;Native"` to avoid conflicts with host

**Document Display**: Use `IDockService.Show(viewModel)` to display dockable documents in the IDE

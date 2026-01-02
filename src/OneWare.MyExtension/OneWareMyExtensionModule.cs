using Avalonia;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using OneWare.Essentials.Enums;
using OneWare.Essentials.Models;
using OneWare.Essentials.Services;
using OneWare.Essentials.ViewModels;
using OneWare.MyExtension.Services;
using OneWare.MyExtension.ViewModels;
using Prism.Ioc;
using Prism.Modularity;

namespace OneWare.MyExtension;

public class OneWareMyExtensionModule : IModule
{
    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterSingleton<IFiniteStateMachineService, FiniteStateMachineService>();
    }

    public void OnInitialized(IContainerProvider containerProvider)
    {
        containerProvider.Resolve<IDockService>().RegisterLayoutExtension<FiniteStateMachineViewModel>(DockShowLocation.Document);
        
        //This example adds a context menu for .vhd files
                containerProvider.Resolve<IProjectExplorerService>().RegisterConstructContextMenu((x,l) =>
        {
            if (x is [IProjectFile { Extension: ".vhd" or ".vhdl" } file])
            {
                l.Add(new MenuItemViewModel("GH")
                {
                    Header = "GH",
                    Items = 
                    [
                        new MenuItemViewModel("A")
                        {
                            Header = "A"

                        },

                        new MenuItemViewModel("B")
                        {
                            Header = "B"        
                        },

                        new MenuItemViewModel("C")
                        {
                            Header = "C"
                        }
                    ]
                });
            }
        });
                       containerProvider.Resolve<IProjectExplorerService>().RegisterConstructContextMenu((x,l) =>
        {
            if (x is [IProjectFile { Extension: ".json" } file])
            {
                l.Add(new MenuItemViewModel("OpenFiniteStateMachine")
                {
                    Header = "Open Finite State Machine",
                    Command = new AsyncRelayCommand(async () =>
                    {
                        var fsmService = containerProvider.Resolve<IFiniteStateMachineService>();
                        await fsmService.ShowFiniteStateMachineAsync(file);
                    })
                });
            }
        });
    }
}
using MachineManagerApp.Models;
using MachineManagerApp.Services;
using MachineManagerApp.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;

namespace MachineManagerApp
{
    public partial class MainWindow : Window
    {
        private MainViewModel ViewModel => (MainViewModel)DataContext;

        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is ProductionLine line)
            {
                ViewModel.SelectedLine = line;
            }
            else if (e.NewValue is Machine machine)
            {
                ViewModel.SelectedMachine = machine;
                // Find and select the parent line
                var parentLine = ViewModel.ProductionLines.FirstOrDefault(l => l.Machines.Contains(machine));
                if (parentLine != null)
                {
                    ViewModel.SelectedLine = parentLine;
                }
            }
        }
    }
}
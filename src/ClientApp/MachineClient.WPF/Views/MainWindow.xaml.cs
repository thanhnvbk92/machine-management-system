using MachineClient.WPF.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace MachineClient.WPF.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
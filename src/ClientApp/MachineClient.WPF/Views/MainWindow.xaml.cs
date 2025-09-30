using MachineClient.WPF.ViewModels;
using MaterialDesignThemes.Wpf;
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

        private void DarkModeToggle_Checked(object sender, RoutedEventArgs e)
        {
            SetDarkTheme();
        }

        private void DarkModeToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            SetLightTheme();
        }

        private void LightModeButton_Click(object sender, RoutedEventArgs e)
        {
            DarkModeToggle.IsChecked = false;
            SetLightTheme();
        }

        private void DarkModeButton_Click(object sender, RoutedEventArgs e)
        {
            DarkModeToggle.IsChecked = true;
            SetDarkTheme();
        }

        private void SetDarkTheme()
        {
            var paletteHelper = new PaletteHelper();
            var theme = paletteHelper.GetTheme();
            theme.SetBaseTheme(BaseTheme.Dark);
            paletteHelper.SetTheme(theme);
        }

        private void SetLightTheme()
        {
            var paletteHelper = new PaletteHelper();
            var theme = paletteHelper.GetTheme();
            theme.SetBaseTheme(BaseTheme.Light);
            paletteHelper.SetTheme(theme);
        }
    }
}
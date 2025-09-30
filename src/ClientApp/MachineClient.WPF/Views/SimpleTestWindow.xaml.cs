using System.Windows;

namespace MachineClient.WPF.Views
{
    public partial class SimpleTestWindow : Window
    {
        public SimpleTestWindow()
        {
            InitializeComponent();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
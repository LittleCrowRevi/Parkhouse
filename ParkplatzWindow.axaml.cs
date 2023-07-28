using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Diagnostics;

namespace Parkhouse
{
    public partial class ParkplatzWindow : Window
    {
        public ParkplatzWindow()
        {
            InitializeComponent();
        }

        public void reservierParkplatz(object source, RoutedEventArgs args)
        {
            var kennzeichenN = kennzeichen.Text;
            Debug.WriteLine(kennzeichenN);
        }
    }
}

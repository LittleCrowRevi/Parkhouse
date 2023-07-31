using Avalonia.Controls;
using Avalonia.Interactivity;
using Parkhouse.Util;
using System.Diagnostics;

namespace Parkhouse.Pages
{
    public partial class Admin : Window
    {
        public Admin()
        {
            InitializeComponent();
            var etagen = MainWindow.Parkhaus().Count();
            Debug.WriteLine("Entagen Count: " +  etagen);
            var maxParkplätze = MainWindow.MaxParkplätze;
            Debug.WriteLine("Parkplatz Count: " + maxParkplätze);
            currentEtagenNum.Text = $"[{etagen}]";
            currentParkNum.Text = $"[{maxParkplätze}]";
        }

        public void ChangeMaxAdmin(object source, RoutedEventArgs args)
        {
            if (maxEtagenInput.Text != null && int.TryParse(maxEtagenInput.Text, out int s))
            {
                MainWindow.ChangeEtagen(s);
                var etagen = MainWindow.Parkhaus().Count();
                currentEtagenNum.Text = $"[{etagen}]";
            }
            if (maxParkInput.Text != null && int.TryParse(maxParkInput.Text, out int n))
            {
                MainWindow.MaxParkplätze = n;
                foreach (Etage e in MainWindow.Parkhaus().FindAll())
                {
                    e.AdjustMaxPark(n);
                }
                currentParkNum.Text = $"[{MainWindow.MaxParkplätze}]";
            }
        }

        public void CloseAdmin(object source, RoutedEventArgs args)
        {
            Close();
        }
    }
}

using Avalonia.Controls;
using Avalonia.Interactivity;
using Parkhouse.Util;
using System.Diagnostics;
using System.Linq;

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

        public void ChangeMaxValues(object source, RoutedEventArgs args)
        {
            if (maxParkInput.Text != null && int.TryParse(maxParkInput.Text, out int n))
            {
                if (MainWindow.Parkhaus().Count() > 0)
                {
                    foreach (Etage e in MainWindow.Parkhaus().FindAll())
                    {

                        e.AdjustMaxPark(n);
                    }
                }
                MainWindow.MaxParkplätze = n;
                currentParkNum.Text = $"[{MainWindow.MaxParkplätze}]";
            }
            if (maxEtagenInput.Text != null && int.TryParse(maxEtagenInput.Text, out int s))
            {
                MainWindow.ChangeEtagen(s);
                var etagen = MainWindow.Parkhaus().Count();
                currentEtagenNum.Text = $"[{etagen}]";
            }
        }

        public void CloseAdmin(object source, RoutedEventArgs args)
        {
            Close();
        }
    }
}

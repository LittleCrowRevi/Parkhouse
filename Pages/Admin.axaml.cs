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
            var maxParkpl�tze = MainWindow.MaxParkpl�tze;
            Debug.WriteLine("Parkplatz Count: " + maxParkpl�tze);
            currentEtagenNum.Text = $"[{etagen}]";
            currentParkNum.Text = $"[{maxParkpl�tze}]";
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
                MainWindow.MaxParkpl�tze = n;
                foreach (Etage e in MainWindow.Parkhaus().FindAll())
                {
                    e.AdjustMaxPark(n);
                }
                currentParkNum.Text = $"[{MainWindow.MaxParkpl�tze}]";
            }
        }

        public void CloseAdmin(object source, RoutedEventArgs args)
        {
            Close();
        }
    }
}

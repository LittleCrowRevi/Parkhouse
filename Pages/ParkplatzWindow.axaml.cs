using Avalonia.Controls;
using Avalonia.Interactivity;
using LiteDB;
using Parkhouse.Util;
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
            warnKennzeichen.IsVisible = false;
            var kennzeichen = kennzeichenInput.Text;
            Debug.WriteLine("Kennzeichen: "+ kennzeichen);
            ILiteCollection<Etage> parkhaus = MainWindow.Parkhaus();
            if (kennzeichen == null ) 
            {
                return;
            }
            foreach (Etage e in parkhaus.FindAll())
            {
                var id = e.FindeFreienPlatz(kennzeichen);
                if (id == -1)
                {
                    warnKennzeichen.IsVisible = true;
                    return;
                }
                if (id == 0)
                {
                    continue;
                }
                if (id > 0) { 
                    e.BelegPlatz(id, kennzeichen);
                    ReservierPanel.IsVisible = false;
                    InfoPanel.IsVisible = true;
                    ParkplatzInfoPanel.Text = $"Ihre Parkplatz Nummer: " + id;
                    MainWindow.NotifyUpdate();
                    return;
                }
            }

            ReservierPanel.IsVisible = false;
            InfoPanel.IsVisible = true;
            ParkplatzInfoPanel.Text = "Kein freier Parkplatz ist zuzeit verfügbar.";
        }
        public void CloseReservier(object source, RoutedEventArgs args)
        {
            Close();
        }
    }
}

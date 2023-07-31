using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using LiteDB;
using Parkhouse.Pages;
using Parkhouse.Util;
using System;
using System.Diagnostics;
using System.Linq;

namespace Parkhouse
{

    public partial class MainWindow : Window
    {
        int _freiePl�tze;
        public int FreiePl�tze
        {
            get { return _freiePl�tze; }
            set
            {
                if (value < 0)
                {
                    _freiePl�tze = 0;
                    freieParkpl�tze.Text = FreiePl�tze.ToString();
                    return;
                }
                _freiePl�tze = value;
                freieParkpl�tze.Text = FreiePl�tze.ToString();
            }
        }
        static int _maxParkPl�tze;
        public static int MaxParkpl�tze
        {
            get { return _maxParkPl�tze; }
            set
            {
                if (value < 0)
                {
                    _maxParkPl�tze = 0;
                    return;
                }
                foreach (Etage e in Parkhaus().FindAll())
                {
                    e.MaxPl�tze = value;
                    Parkhaus().Update(e);
                }
                NotifyUpdate();
                _maxParkPl�tze = value;
            }
        }

        static readonly LiteDatabase db = new(@"Parkhaus.db");
        public static ILiteCollection<Etage> Parkhaus()
        {
            return db.GetCollection<Etage>("parkhaus");
        }
        public static ILiteCollection<Parkplatz> Etage(int id)
        {
            return db.GetCollection<Parkplatz>($"etage_{id}");
        }

        public delegate void Notify();
        public static event Notify? Update;
        // Wrap event...
        public static void NotifyUpdate()
        {
            Update?.Invoke();
        }


        // on table creation also maximum number of rows are generated
        // to assign a spot it would check if a spot with a NULL fahrzeug exists and assign it
        // if maximum amount of spots are reduced...get the difference and delete all above the new value? (old - new) = diff OR for i = new; i < old; i++
        public MainWindow()
        {
            InitializeComponent();
            Update += OnParkplatzChange;
            if (Parkhaus().Count() == 0)
            {
                MaxParkpl�tze = 0;
                var a = new Admin();
                a.Show();
                a.Focus();
            } else
            {
                MaxParkpl�tze = Parkhaus().FindAll().First().MaxPl�tze;
            }
            NotifyUpdate();

        }

        public static int QueryFreiePl�tze()
        {
            if (Parkhaus().Count() == 0)
            {
                return 0;
            }
            var count = 0;
            foreach (Etage etage in Parkhaus().FindAll())
            {
                count += etage.FreiePl�tze();
            }
            return count;
        }


        // update text f�r freie parkpl�tze
        public void OnParkplatzChange()
        {
            FreiePl�tze = QueryFreiePl�tze();
        }

        public static void EntferneEtage(int id)
        {
            if (Parkhaus().Exists(x => x.ID == id))
            {
                db.DropCollection($"etage_{id}");
                if (Parkhaus().Delete(id))
                {
                    Debug.WriteLine("Deleted etage with ID: " + id);
                }
                else
                {
                    Debug.WriteLine("Deleted of etage with ID: " + id + " failed?");
                }
            }
        }

        // parkplatz fenster wird hier ge�ffnet falls es einen freien Platz gibt
        public void PlatzReservieren(object source, RoutedEventArgs args)
        {
            var p = new ParkplatzWindow();
            if (QueryFreiePl�tze() == 0)
            {
                NotifyBelegt();
                return;
            }
            p.Show();
        }

        public void OpenAdmin(object source, RoutedEventArgs args)
        {
            var a = new Admin();
            a.Show();
        }

        // f�r die "alle parkpl�tze sind belegt" warnung
        public void NotifyBelegt()
        {
            var bgColor = new SolidColorBrush
            {
                Color = Color.FromArgb((byte)0.5, 250, 5, 5)
            };
            var borderColor = new SolidColorBrush
            {
                Color = Color.FromArgb((byte)0.9, 255, 0, 0)
            };
            warnLabel.Background = bgColor;
            warnLabel.BorderBrush = borderColor;
            warnLabel.IsVisible = true;
        }

        
        public static void ChangeEtagen(int num)
        {
            var currentEtagenNum = Parkhaus().Count();
            Debug.WriteLine("Etagen:" + currentEtagenNum);
            if (num < currentEtagenNum)
            {
                // der Unterschied zwischen dem alten Etagen und Neuen wird von den Etagen in reverse abgezogen
                for (int i = currentEtagenNum; i > num; i--)
                {
                    EntferneEtage(i);
                    Update?.Invoke();
                }
                return;
            }
            for (int i = num; i > currentEtagenNum; i--)
            {
                Parkhaus().Upsert(i, new Etage(MaxParkpl�tze, i));
                Parkhaus().EnsureIndex(x => x.ID);
            }
            Update?.Invoke();
        }
    }
}
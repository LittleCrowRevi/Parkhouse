using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Diagnostics;
using Npgsql;
using System.Collections.Generic;
using LiteDB;
using Microsoft.CodeAnalysis.CSharp;

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
                int newValue = FreiePl�tze + value;
                if (newValue < 0)
                {
                    _freiePl�tze = 0;
                    freieParkpl�tze.Text = FreiePl�tze.ToString();
                    return;
                }
                _freiePl�tze = newValue;
                freieParkpl�tze.Text = FreiePl�tze.ToString();
            }
        }
        int _maxParkPl�tze;
        public int MaxParkpl�tze
        {
            get { return _maxParkPl�tze; }
            set
            {
                if (value < 0)
                {
                    _maxParkPl�tze = 0;
                    return;
                }
                _maxParkPl�tze += value;
            }
        }

        public List<Etage> etagenArray = new();
        int _etagen = 0;
        public int Etagen
        {
            get { return _etagen; }
            set
            {
                Debug.WriteLine(value);
                if (value < _etagen)
                {
                    for (int i = _etagen; i > value; i--)
                    {
                        etagenArray.RemoveAt(-1);
                    }
                    _etagen = value;
                    return;
                }
                if (value >= _etagen)
                {
                    for (int i = value; i > _etagen; i--)
                    {
                        AddEtage(i);
                    }
                    _etagen = value;
                }
            }
        }

        public static LiteDatabase db = new LiteDatabase(@"Parkhaus.db");

        // on table creation also maximum number of rows are generated
        // to assign a spot it would check if a spot with a NULL fahrzeug exists and assign it
        // if maximum amount of spots are reduced...get the difference and delete all above the new value? (old - new) = diff OR for i = new; i < old; i++
        public MainWindow()
        {
            InitializeComponent();

            MaxParkpl�tze = 10;
            PopulateDatabse();
            Etagen += 2;
            FreiePl�tze = QueryFreiePl�tze();

        }

        public static int QueryFreiePl�tze()
        {
            var parkhaus = db.GetCollection<Etage>("parkhaus");
            var count = 0;
            foreach (Etage etage in parkhaus.FindAll())
            {
                count += etage.FreiePl�tze(db);
            }
            return count;
        }

        public void PopulateDatabse()
        {
            AddEtage(1);
            AddEtage(2);

        }

        public void ParkplatzZuteilen(object source, RoutedEventArgs args)
        {
            var p = new ParkplatzWindow();
            if (QueryFreiePl�tze() == 0)
            {
                return;
            }
            p.Show();
        }


        public void AddEtage(int id)
        {
            var parkhaus = db.GetCollection<Etage>("parkhaus");
            parkhaus?.Upsert(id, new Etage(MaxParkpl�tze, id));
            parkhaus?.EnsureIndex(x => x.ID);
        }
    }
}
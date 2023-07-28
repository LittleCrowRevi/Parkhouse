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
        int _freiePlätze;
        public int FreiePlätze
        {
            get { return _freiePlätze; }
            set
            {
                int newValue = FreiePlätze + value;
                if (newValue < 0)
                {
                    _freiePlätze = 0;
                    freieParkplätze.Text = FreiePlätze.ToString();
                    return;
                }
                _freiePlätze = newValue;
                freieParkplätze.Text = FreiePlätze.ToString();
            }
        }
        int _maxParkPlätze;
        public int MaxParkplätze
        {
            get { return _maxParkPlätze; }
            set
            {
                if (value < 0)
                {
                    _maxParkPlätze = 0;
                    return;
                }
                _maxParkPlätze += value;
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

            MaxParkplätze = 10;
            PopulateDatabse();
            Etagen += 2;
            FreiePlätze = QueryFreiePlätze();

        }

        public static int QueryFreiePlätze()
        {
            var parkhaus = db.GetCollection<Etage>("parkhaus");
            var count = 0;
            foreach (Etage etage in parkhaus.FindAll())
            {
                count += etage.FreiePlätze(db);
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
            if (QueryFreiePlätze() == 0)
            {
                return;
            }
            p.Show();
        }


        public void AddEtage(int id)
        {
            var parkhaus = db.GetCollection<Etage>("parkhaus");
            parkhaus?.Upsert(id, new Etage(MaxParkplätze, id));
            parkhaus?.EnsureIndex(x => x.ID);
        }
    }
}
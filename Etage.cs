using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Diagnostics;
using LiteDB;

namespace Parkhouse
{
    public class Parkplatz
    {
        public int Nummer { get; set; }
        public string Status { get; set; } = "frei";
    }
    public class Etage
    {
        public int ID { get; set; }
        public int maxPlätze { get; set; }

        public Etage(int m, int id)
        {
            ID = id;
            maxPlätze = m;
            var etage = MainWindow.db.GetCollection<Parkplatz>($"etage_{ID}");
            for(int i = 0; i < maxPlätze; i++)
            {
                etage.Upsert(ID * 100 + i, new Parkplatz { Nummer = ID * 100 + i});
            }
            etage.EnsureIndex(x => x.Nummer);
            etage.EnsureIndex(x => x.Status);
        }


        public int FindeFreienPlatz()
        {
            var etage = MainWindow.db.GetCollection<Parkplatz>($"etage_{ID}");
            var n = etage.FindOne(x => x.Status == "frei").Nummer;
            return n;
        }

        public int FreiePlätze(LiteDatabase db)
        {
            var plätze = db.GetCollection<Parkplatz>($"etage_{ID}");
            var count = plätze.Find("$.Status = 'frei'").Count();
            Debug.WriteLine(count);
            return count;
        }
    }
}

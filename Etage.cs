using LiteDB;
using System.Diagnostics;
using System.Linq;

namespace Parkhouse
{
    public class Parkplatz
    {
        public int Nummer { get; set; }
        public string Status { get; set; } = "belegt";
    }
    public class Etage
    {
        public int ID { get; set; }
        public int MaxPlätze { get; set; }

        public Etage(int m, int id)
        {
            ID = id;
            MaxPlätze = m;
            var etage = MainWindow.db.GetCollection<Parkplatz>($"etage_{ID}");
            for (int i = 0; i < MaxPlätze; i++)
            {
                etage.Upsert(ID * 100 + i, new Parkplatz { Nummer = ID * 100 + i });
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

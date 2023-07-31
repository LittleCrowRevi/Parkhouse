using LiteDB;
using System.Diagnostics;
using System.Linq;

namespace Parkhouse.Util
{
    public class Parkplatz
    {
        public int Nummer { get; set; }
        public string Status { get; set; } = "frei";
    }
    public class Etage
    {
        public int ID { get; set; }
        public int MaxPlätze { get; set; }

        public Etage(int m, int id)
        {
            ID = id;
            MaxPlätze = m;
            var etage = MainWindow.Etage(ID);
            for (int i = 0; i < MaxPlätze; i++)
            {
                etage.Upsert(ID * 100 + i, new Parkplatz { Nummer = ID * 100 + i });
            }
            etage.EnsureIndex(x => x.Nummer);
            etage.EnsureIndex(x => x.Status);
        }


        public int FindeFreienPlatz(string kennzeichen)
        {
            var etage = MainWindow.Etage(ID);
            if (etage.Exists(x => x.Status == kennzeichen))
            {
                return -1;
            }
            var n = etage.FindOne(x => x.Status == "frei")?.Nummer;
            Debug.WriteLine("Freier Platz: " + n);
            if (n == null)
            {
                return 0;
            }
            return (int)n;
        }

        public int FreiePlätze()
        {
            var plätze = MainWindow.Etage(ID);
            var count = plätze.Find("$.Status = 'frei'").Count();
            return count;
        }

        public void BelegPlatz(int id, string kennzeichen)
        {
            var etage = MainWindow.Etage(ID);
            var p = new Parkplatz { Nummer = id, Status = kennzeichen };
            Debug.WriteLine("Updating: " + id);
            etage.Update(id, p);
        }


        public void AdjustMaxPark(int newMax)
        {
            var etage = MainWindow.Etage(ID);
            if (newMax < MaxPlätze && newMax > -1)
            {
                for (int i = newMax; i < MaxPlätze; i++)
                {
                    if (etage.Exists(x => x.Nummer == ID * 100 + i))
                    {
                        etage.Delete(ID * 100 + i);
                    }
                }
            }
            if (newMax > MaxPlätze )
            {
                for (int i = newMax; i > MaxPlätze; i--)
                {
                    etage.Upsert(ID * 100 + i, new Parkplatz { Nummer = ID * 100 + i });
                }
            }
            MaxPlätze = newMax;
            MainWindow.NotifyUpdate();
        }
    }
}

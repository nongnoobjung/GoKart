using KartExtreme.Data;
using KartExtreme.Net;

namespace KartExtreme.Kart
{
    public class Player
    {
        public KartClient Client { get; private set; }

        public int ID { get; private set; }
        public string Name { get; private set; }
        public byte Level { get; private set; }
        public int Lucci { get; private set; }

        private bool Assigned { get; set; }

        public Player(int id = 0, KartClient client = null)
        {
            this.ID = id;
            this.Client = client;
        }

        public void Load()
        {
            foreach (dynamic datum in new Datums("players").Populate("ID = '{0}'", this.ID))
            {
                this.Name = datum.Name;
                this.Level = datum.Level;
                this.Lucci = datum.Lucci;

                this.Assigned = true;
            }
        }

        public void Save()
        {
            dynamic datum = new Datum("players");

            datum.ID = this.ID;
            datum.Name = this.Name;
            datum.Level = this.Level;
            datum.Lucci = this.Lucci;

            if (this.Assigned)
            {
                datum.Update("ID = '{0}'", this.ID);
            }
            else
            {
                datum.Insert();
            }
        }

        public void Delete()
        {
            Database.Delete("players", "ID = '{0}'", this.ID);

            this.Assigned = false;
        }
    }
}

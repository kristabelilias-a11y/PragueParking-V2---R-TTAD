using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PragueParking_V2.Klasser
{
    public class Fordon
    {
        public string RegNr { get; set; }
        public string FordonsTyp { get; set; }

        public int Storlek {  get; set; }
        public DateTime Incheckningstid { get; set; } = DateTime.Now;

         public Fordon(string regNr)
        {
            RegNr = regNr.ToUpper();
            Incheckningstid = DateTime.Now; // Registrera parkeringsstart

        }
    }
}

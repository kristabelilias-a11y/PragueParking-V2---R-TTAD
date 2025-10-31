using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace PragueParking.Core
{
    [JsonDerivedType(typeof(Bil), "Bil")]
    [JsonDerivedType(typeof(MC), "MC")] // talar om för JSON filen vad underklassen är
    public class Fordon
    
    {
        public Fordon() { }
        public string RegNr { get; set; } = string.Empty;
        public string FordonsTyp { get; set; } = string.Empty;  

        public int Storlek {  get; set; }
        public DateTime Incheckningstid { get; set; } = DateTime.Now;

         public Fordon(string regNr)
        {
            RegNr = regNr.ToUpper();
            Incheckningstid = DateTime.Now; // Registrera parkeringsstart

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PragueParking_V2.Klasser
{
    public class ParkeringsPlats
    {
        public int platsNummer { get ; set; }
        public List<Fordon> ParkeradeFordon { get; set; } = new List<Fordon>();

        public bool FordonPåPlats(Fordon fordon) // Metod som söker fordon på angiven plats
        {
            if (fordon is Bil)
            {
                if (ParkeradeFordon.Count == 0)
                {
                    ParkeradeFordon.Add(fordon);
                    return true;
                }
                return false;
            }


            if (fordon is MC)
            {
                if (ParkeradeFordon.Count < 2 && ParkeradeFordon.All(f => f is MC)) // Inga bilar får redan stå här och max 1 MC får finnas om vi ska parkera
                {
                    ParkeradeFordon.Add(fordon);
                    return true;
                }
                return false;
            }

            return false; // Övriga fordonstyper ej tillåtna
        }
        
    }
}

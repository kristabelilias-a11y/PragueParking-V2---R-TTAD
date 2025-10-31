using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PragueParking.Core
{
    public class ParkeringsPlats
    {
        // I dagsläget är förutsättningarna att endast bil och mc kan parkeras, men skulle det i framtiden komma bussar/cyklar så tar en buss med strl 16 upp ex 4 platser och en cykel med storlek 1 upp 1/4 av en plats
        public int platsNummer { get ; set; }
        public int MaxStorlek { get; set; } = 4; //Vanlig parkering som passar en bil eller 2 MC
        
        public List<Fordon> ParkeradeFordon { get; set; } = new List<Fordon>();

        public int UpptagenYta => ParkeradeFordon.Sum(fordon => fordon.Storlek);

        public bool FordonPåPlats(Fordon fordon) // Metod som söker fordon på angiven plats
        {
            if (fordon is Bil)
            {
                if (ParkeradeFordon.Count == 0 && UpptagenYta + fordon.Storlek <= MaxStorlek) 
                {
                    ParkeradeFordon.Add(fordon);
                    return true;
                }
                return false;
            }


            if (fordon is MC)
            {
                if (ParkeradeFordon.Count < 2 && ParkeradeFordon.All(fordon => fordon is MC)) // Inga bilar får redan stå här och max 1 MC får finnas om vi ska parkera
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

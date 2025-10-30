using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PragueParking_V2.Klasser
{
    public class Parkeringshus
    {
        public List<ParkeringsPlats> Plats { get; set; } = new List<ParkeringsPlats>(100);

        public Parkeringshus() { }
        //internal static IEnumerable<object> OrderBy(Func<object, object> value)
        //{
        //    throw new NotImplementedException();
        //}

        public bool FörsökParkera(Fordon fordon, out string meddelande)
        {
            // Validera regnr
            var regNr = (fordon?.RegNr ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(regNr))
            {
                meddelande = "\nOgiltigt registreringsnummer.";
                return false;
            }
            if (regNr.Length > 10)
            {
                meddelande = "\nFör lång text. Max 10 tecken tillåtet. Vänligen försök igen.";
                return false;
            }

            // Förhindra dubblett: "fordonet finns redan i systemet"
            var befintligPlats = Plats.FirstOrDefault(p =>
                p.ParkeradeFordon.Any(f => f.RegNr.Equals(regNr, StringComparison.OrdinalIgnoreCase)));

            if (befintligPlats != null)
            {
                meddelande = $"\nFordon med registreringsnummer {regNr} finns redan på plats {befintligPlats.platsNummer}.";
                return false;
            }

            // Försök parkera på första plats där din ParkeringsPlats-logik säger ja
            foreach (var plats in Plats.OrderBy(p => p.platsNummer))
            {
                if (plats.FordonPåPlats(fordon))
                {
                    meddelande = $"{fordon.FordonsTyp} {regNr} parkerad på plats {plats.platsNummer}.";
                    return true;
                }
            }

            meddelande = "\nIngen lämplig plats hittades just nu.";
            return false;
        }
    }
}

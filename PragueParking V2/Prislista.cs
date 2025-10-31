using PragueParking.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PragueParking_V2
{
    public static class Prislista
    {
        private static readonly string Filnamn = "Prislista.txt";
        private static readonly Dictionary<string, int> PrisPerTyp = new();

        static Prislista()
        {
            LaddaPrislista();
        }

        private static void LaddaPrislista()
        {
            if (!File.Exists(Filnamn))
            {
                Console.WriteLine($"⚠️ Filen '{Filnamn}' saknas! Skapar en standardlista.");
                File.WriteAllText(Filnamn, "Första 10min är gratis, därefter:\n\nBIL: 20CZK/h\nMC: 10CZK/h");
            }

            string[] rader = File.ReadAllLines(Filnamn);

            // Gör raden okänlig för stora/små bokstäver, ^ betyder början av raden, (?<typ>\w+) fångar in själva fordonstypen och döper om den till typ
            // (?<pris>\d+) fångar in priset och döper om den till pris, \s*CZK?/h kräver att texten slutar med CZK/h
            Regex regex = new(@"(?i)^(?<typ>\w+):\s*(?<pris>\d+)\s*CZK?/h"); // Detta tog jag hjälp av AI med, för att programmet ska kunna tolka textfilen korrekt. 

            foreach (string rad in rader)
            {
                var match = regex.Match(rad);
                if (match.Success)
                {
                    string typ = match.Groups["typ"].Value.Trim();
                    int pris = int.Parse(match.Groups["pris"].Value.Trim());
                    PrisPerTyp[typ] = pris;
                }
            }
        }

        public static int HämtaPrisPerTimme(string fordonstyp)
        {
            // matcha oavsett versaler/gemener
            var nyckel = PrisPerTyp.Keys.FirstOrDefault(k =>
                k.Equals(fordonstyp, StringComparison.OrdinalIgnoreCase));

            if (nyckel != null)
                return PrisPerTyp[nyckel];

            throw new Exception($"Fordonstyp '{fordonstyp}' finns inte i Prislista.txt");
        }

        public static int BeräknaPris(Fordon fordon, DateTime utTid)
        {
            int prisPerTimme = fordon is Bil ? 20 : 10;

            TimeSpan tid = utTid - fordon.Incheckningstid;
            if (tid.TotalMinutes <= 10)
                return 0; // första 10 minuter gratis

            double minuter = Math.Ceiling(tid.TotalMinutes);

            double prisPerMinut = prisPerTimme / 60.0;

            int pris = (int)Math.Ceiling(minuter * prisPerMinut); // Nu räknas priset per minut efter 10 minuter

            return pris;
        }
    }
}

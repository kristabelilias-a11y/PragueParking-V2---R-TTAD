using PragueParking.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PragueParking_V2
{
    public class Konfiguration
    {
        public int  AntalPlatser{ get; set; }

        public List<FordonsTypKonfig> FordonsTyp { get; set; } = new ();

    
    }

    public class FordonsTypKonfig
    {
        public string Typ { get; set; } = "";
        public int AntalPerPlats { get; set; }
        public int PrisPerTimme { get; set; }
    }

    public static class KonfigurationsHantere
    {
        public static Konfiguration LäsInKonfig(string filnamn = "Konfiguering.json")
        {
            string json = File.ReadAllText(filnamn);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<Konfiguration>(json, options)!;
        }
    }
}

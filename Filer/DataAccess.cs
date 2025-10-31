using PragueParking_V2.Klasser;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PragueParking_V2.Filer
{

    public class DataAccess
    {

        public class Konfiguering
        {
            public int AntalPlatser { get; set; } = 100;
            public List<string> Fordonstyper { get; set; } = new List<string> { "Bil", "MC" };
        }

        private readonly string konfigFil = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "../../../Konfiguering.json");
        private readonly string dataFil = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "../../../Datafil.json");

        //public DataAccess()
        //{
        //    // Se till att filerna sparas i programmets körmapp
        //    string basePath = AppContext.BaseDirectory;
        //    konfigFil = Path.Combine(basePath, "Konfiguering.json");
        //    dataFil = Path.Combine(basePath, "Datafil.json");
        //}


        public void SparaData(Parkeringshus parkeringsHus)
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true }; // Gör json filen lätt att läsa med WriteIntended
                string jsonString = JsonSerializer.Serialize(parkeringsHus, options); // Gör om hela parkeringshuset till text i json-format

                File.WriteAllText(dataFil, jsonString); //Skriver texten till en fil på hårddisken

                Console.WriteLine("Data sparad till" + dataFil);

            }
            catch (Exception ex)
            {
                Console.WriteLine("Fel vid sparande av data." + ex.Message);
            }

        }

        public Parkeringshus LäsData()
        {
            try
            {
                if (!File.Exists(dataFil))
                {
                    Console.WriteLine("Ingen datafil hittades, skapar nytt parkeringshus\n.");
                    return new Parkeringshus();
                }

                string jsonString = File.ReadAllText(dataFil);

                // Gör om texten till ett Parkeringshus-objekt igen
                Parkeringshus? parkeringsHus = JsonSerializer.Deserialize<Parkeringshus>(jsonString);

                if (parkeringsHus != null)
                { //Konverterar generella fordon till rätt typ baserat på fordonstyp
                    foreach (var plats in parkeringsHus.Plats)
                    {
                        for (int i = 0; i < plats.ParkeradeFordon.Count; i++)
                        {
                            var f = plats.ParkeradeFordon[i];
                            if (f.FordonsTyp.Equals("BIL", StringComparison.OrdinalIgnoreCase))
                                plats.ParkeradeFordon[i] = new Bil(f.RegNr) { Incheckningstid = f.Incheckningstid };
                            else if (f.FordonsTyp.Equals("MC", StringComparison.OrdinalIgnoreCase))
                                plats.ParkeradeFordon[i] = new MC(f.RegNr) { Incheckningstid = f.Incheckningstid };
                        }
                    }
                }
                
                Console.WriteLine("Data läst in från " + dataFil);
                return parkeringsHus ?? new Parkeringshus();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fel vid inläsning av data" + ex.Message);
                return new Parkeringshus();
            }
        }



        public void SparaKonfig(Konfiguering config)
        {
            string json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(konfigFil, json);
        }

        public Konfiguering LäsKonfig()
        {
            if (!File.Exists(konfigFil))
                return new Konfiguering(); // ny standard om ingen fil finns

            string json = File.ReadAllText(konfigFil);
            return JsonSerializer.Deserialize<Konfiguering>(json);
        }


    }
}


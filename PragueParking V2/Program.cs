using PragueParking_V2.Filer;
using PragueParking_V2.Klasser;
using Spectre.Console;
using Spectre.Console.Rendering;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.IO;
using System.Linq;


namespace PragueParking
{
    class Program
    {

   
        static void Main(string[] args)
        {
            DataAccess dataAccess = new DataAccess(); // Skapa datahanterare
            Parkeringshus parkeringshus = dataAccess.LäsData(); // Läs in tidigare sparad data

            PrisLista();
            VäntaOchRensa();

            if (parkeringshus.Plats == null || parkeringshus.Plats.Count == 0)
            {
                parkeringshus.Plats = new List<ParkeringsPlats>();
                for (int i = 1; i <= 100; i++)
                {
                    parkeringshus.Plats.Add(new ParkeringsPlats { platsNummer = i });
                }
            }

            while (true)
            {

                AnsiConsole.Clear();
                var val = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[bold]Välkommen till Prague Parking\nVälj ett av följande alternativ:[/]")

                        .AddChoices(new[]
                        { "Prislista",
                          "1. Visa karta",
                          "2. Parkera fordon",
                          "3. Flytta fordon",
                          "4. Hämta fordon",
                          "5. Sök fordon",
                          "6. Avsluta",
                            }));

                switch (val)
                {
                    case "1. Visa karta":
                        UIVisaKarta(parkeringshus);
                        VäntaOchRensa();
                        break;
                    case "2. Parkera fordon":
                        UIParkera(parkeringshus);
                        dataAccess.SparaData(parkeringshus);
                        VäntaOchRensa();
                        break;
                    case "3. Flytta fordon":
                        UIFlytta(parkeringshus);
                        dataAccess.SparaData(parkeringshus);
                        VäntaOchRensa();
                        break;
                    case "4. Hämta fordon":
                        UIHämta(parkeringshus);
                        dataAccess.SparaData(parkeringshus);
                        VäntaOchRensa();
                        break;
                    case "5. Sök fordon":
                        UISök(parkeringshus);
                        VäntaOchRensa();
                        break;
                    case "6. Avsluta":
                        dataAccess.SparaData(parkeringshus);
                        AnsiConsole.MarkupLine("Tack för besöket, välkommen åter!");
                        return;

                    case "Prislista":
                        PrisLista();
                        VäntaOchRensa();
                        break;

                    default:
                        AnsiConsole.MarkupLine("[red]Ogiltigt val.[/]");
                        break;
                }

                AnsiConsole.MarkupLine("[grey](Tryck val i menyn för att fortsätta.)[/]");
            }
        }

        

        static Parkeringshus SkapaStandardGarage(int antal = 100)
        {
            var g = new Parkeringshus();
            for (int i = 1; i <= antal; i++)
                g.Plats.Add(new ParkeringsPlats { platsNummer = i });
            return g;
        }

        static string HämtaFärg(ParkeringsPlats plats)
        {
            int count = plats.ParkeradeFordon.Count;

            if (count == 0)
                return "green";

            if (count == 1 && plats.ParkeradeFordon.All(f => f is MC))
                return "yellow";

            return "red";
        }
        public static void UIVisaKarta(Parkeringshus parkeringshus, int kolumner = 10)

        
        {

            AnsiConsole.Clear();

            var table = new Table().Centered();
            table.Title("Karta över P-huset");
            table.Border(TableBorder.Rounded);

            for (int c = 0; c < kolumner; c++)
                table.AddColumn(new TableColumn($"[grey]{c + 1}[/]").Centered());

            var platser = parkeringshus.Plats.OrderBy(p => p.platsNummer).ToList();

            for (int i = 0; i < platser.Count; i += kolumner)
            {
                var row = new List<IRenderable>();

                foreach (var plats in platser.Skip(i).Take(kolumner))
                {
                    string färg = HämtaFärg(plats);
                    string cellText = plats.ParkeradeFordon.Count == 0 ? $"{plats.platsNummer:D3}\n[ ]" : $"{plats.platsNummer:D3}\n[{string.Join(",", plats.ParkeradeFordon.Select(fordon => fordon.FordonsTyp))}]";

                    row.Add(new Markup($"[{färg}]{Markup.Escape(cellText)}[/]").Centered());
                }
                table.AddRow(row.ToArray());

            }

            AnsiConsole.Write(table);

            var lediga = platser.Count(p => p.ParkeradeFordon.Count == 0 || (p.ParkeradeFordon.All(f => f is MC) && p.ParkeradeFordon.Count == 1));
            
            AnsiConsole.WriteLine();
            AnsiConsole.Write(new FigletText($"Ledigt: {lediga}").Centered().Color(Spectre.Console.Color.Green));
            AnsiConsole.WriteLine();

            return;
        }

        public static void UIParkera(Parkeringshus parkeringshus)
        {
            var typ = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Välj [green]fordonstyp[/]:")
                    .AddChoices("BIL", "MC"));
            var regNr = AnsiConsole.Ask<string>("Ange [green]regnr[/]:").Trim().ToUpper();

            Fordon fordon = typ == "BIL" ? new Bil(regNr) : new MC(regNr);

            if (parkeringshus.FörsökParkera(fordon, out var msg))
                AnsiConsole.MarkupLine($"[green]{msg}[/]");
            else
                AnsiConsole.MarkupLine($"[red]{msg}[/]");

            return;
        }

        public static void UIFlytta(Parkeringshus parkeringshus)
        {
            var regNr = AnsiConsole.Ask<string>("Ange registreringsnummer att flytta:").Trim().ToUpper();

            ParkeringsPlats? gammalPlats = null;
            Fordon? fordon = null;                  // Hitta fordonet i garaget

            foreach (var plats in parkeringshus.Plats)
            {
                fordon = plats.ParkeradeFordon.FirstOrDefault(f => f.RegNr.Equals(regNr, StringComparison.OrdinalIgnoreCase));
                if (fordon != null)
                {
                    gammalPlats = plats;
                    break;
                }
            }

            if (fordon == null || gammalPlats == null)
            {
                AnsiConsole.MarkupLine("[red]Fordonet hittades inte i garaget.[/]");
                return;
            }


            foreach (var plats in parkeringshus.Plats.OrderBy(p => p.platsNummer))
            {

                if (plats.FordonPåPlats(fordon))
                {
                    AnsiConsole.MarkupLine($"[green]{fordon.FordonsTyp}-{fordon.RegNr} tilldelades ny plats [bold]{plats.platsNummer}[/][/]");
                    gammalPlats.ParkeradeFordon.Remove(fordon);
                    return;
                }

               
            }

            gammalPlats.ParkeradeFordon.Add(fordon);
            AnsiConsole.MarkupLine("[red]Ingen ledig plats hittades för flytt. Fordonet står kvar på sin gamla plats.[/]");
        }


        public static void UIHämta(Parkeringshus parkeringshus)
        {
            var regNr = AnsiConsole.Ask<string>("Ange registreringsnummer att hämta:").Trim().ToUpper();
            DateTime uthämtning = DateTime.Now;

            foreach (var plats in parkeringshus.Plats)
            {
                var Plats = plats.ParkeradeFordon.FirstOrDefault(x => x.RegNr.Equals(regNr, StringComparison.OrdinalIgnoreCase));
                if (Plats != null)
                {
                    int pris = Prislista.BeräknaPris(Plats, DateTime.Now);
                    var tidTotalt = uthämtning - Plats.Incheckningstid;
                    string tidText = $"{(int)tidTotalt.TotalMinutes} min";
                    AnsiConsole.MarkupLine($"[green]{Plats.FordonsTyp} {regNr} utlämnad[/] från plats [bold]{plats.platsNummer}[/].");
                    plats.ParkeradeFordon.Remove(Plats);
                    AnsiConsole.MarkupLine($"Pris att betala: [bold]{pris} kr[/]");
                    AnsiConsole.MarkupLine($"Tid parkerad: {Plats.Incheckningstid} till {uthämtning}");
                    return;
                }
            }
            AnsiConsole.MarkupLine("[yellow]Fordonet hittades inte.[/]");
        }

        public static void UISök(Parkeringshus parkeringshus)
        {
            var regNr = AnsiConsole.Ask<string>("Ange registreringsnummer för fordon du söker: ").Trim().ToUpper();
            if (string.IsNullOrEmpty(regNr))
                return;

            bool hittad = false;

            foreach (var plats in parkeringshus.Plats)
            {
                var fordon = plats.ParkeradeFordon.FirstOrDefault(x => x.RegNr.Equals(regNr, StringComparison.OrdinalIgnoreCase));
                if (fordon != null)
                {
                    AnsiConsole.MarkupLine($"[green]Fordonet {fordon.FordonsTyp} med regnr {fordon.RegNr} står på plats [bold]{plats.platsNummer}[/].[/]");
                    hittad = true;
                    break;
                }
            }
            if (!hittad)
            {
                AnsiConsole.MarkupLine("[yellow]Fordonet hittades inte.[/]");
            }
        }

        public static void PrisLista()
        {

                try
                {
                string fileName = Path.Combine("Filer", "Prislista.txt");
                string filePath = Path.Combine(AppContext.BaseDirectory, fileName); // Returnerar den mapp där programmet körs ifrån

                if (File.Exists(filePath))
                {
                    string fileContents = File.ReadAllText(fileName);
                    AnsiConsole.Write(new Panel(fileContents).Header("Prislista").Border(BoxBorder.Rounded).Padding(1, 1));
                    return;

                }
                

                if (!File.Exists("Prislista.txt"))
                    {
                        AnsiConsole.MarkupLine("[red]Prislista.txt hittades inte.[/]");
                        return;
                    }


            }
            catch (Exception ex)
                {
                    AnsiConsole.MarkupLine($"[red]Fel vid läsning av prislista: {Markup.Escape(ex.Message)}[/]");
                }
            
        }


            
        
            

        public static void VäntaOchRensa()
        {
            AnsiConsole.MarkupLine("\n(Tryck på valfri tangent för att fortsätta...)");
            Console.ReadKey(true);
            AnsiConsole.Clear();
        }
    }

}


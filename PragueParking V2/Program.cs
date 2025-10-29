using PragueParking_V2.Klasser;
using Spectre.Console;
using Spectre.Console.Rendering;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;


namespace PragueParking
{
    class Program
    {

   
        static void Main(string[] args)
        {
            var garage = SkapaStandardGarage(100);

            while (true)
            {

                AnsiConsole.Clear();
                var val = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[bold]Prague Parking [/] – Välj ett alternativ:")
                        .AddChoices(new[]
                        { "1. Visa karta",
                          "2. Parkera fordon",
                          "3. Flytta fordon",
                          "4. Hämta fordon",
                          "5. Sök fordon",
                          "6. Avsluta"
                            }));

                switch (val)
                {
                    case "1. Visa karta":
                        UIVisaKarta(garage);
                        VäntaOchRensa();
                        break;
                    case "2. Parkera fordon":
                        UIParkera(garage);
                        VäntaOchRensa();
                        break;
                    case "3. Flytta fordon":
                        UIFlytta(garage);
                        VäntaOchRensa();
                        break;
                    case "4. Hämta fordon":
                        UIHämta(garage);
                        VäntaOchRensa();
                        break;
                    case "5. Sök fordon":
                        VäntaOchRensa();
                        UISök(garage);
                        break;
                    case "6. Avsluta":
                        return;

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


        public static void UIVisaKarta(Parkeringshus garage, int kolumner = 10)
        {
            var table = new Table().Centered();
            table.Title("Karta över P-huset");
            table.Border(TableBorder.Rounded);

            for (int c = 0; c < kolumner; c++)
                table.AddColumn(new TableColumn($"[grey]{c + 1}[/]").Centered());

            var platser = garage.Plats.OrderBy(p => p.platsNummer).ToList();

            for (int i = 0; i < platser.Count; i += kolumner)
            {
                var row = new List<IRenderable>();
                foreach (var p in platser.Skip(i).Take(kolumner))
                {

                    // Färglogik: tom=grå, delvis (1 MC på tom plats)=gul, full (1 bil eller 2 MC)=röd
                    var count = p.ParkeradeFordon.Count;
                    string färg = count == 0 ? "grey" : (count == 1 && p.ParkeradeFordon.All(f => f is MC) ? "yellow" : "red");

                    var text = $"{p.platsNummer:D3} [{string.Join(",", p.ParkeradeFordon.Select(f => f.FordonsTyp))}]";
                    if (count == 0) text = $"{p.platsNummer:D3} [ ]";

                    //row.Add(new Markup($"[{färg}]{text}[/]"));
                }
                table.AddRow(row.ToArray());


            }

            AnsiConsole.Clear();
            AnsiConsole.Write(table);

            var lediga = platser.Count(p => p.ParkeradeFordon.Count == 0 || (p.ParkeradeFordon.All(f => f is MC) && p.ParkeradeFordon.Count == 1));
            AnsiConsole.WriteLine();
            AnsiConsole.Write(new FigletText($"Ledigt: {lediga}").Centered().Color(Spectre.Console.Color.Green));
            AnsiConsole.WriteLine();

            return;
        }

        public static void UIParkera(Parkeringshus garage)
        {
            var typ = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Välj [green]fordonstyp[/]:")
                    .AddChoices("BIL", "MC"));
            var regNr = AnsiConsole.Ask<string>("Ange [green]regnr[/]:").Trim().ToUpper();

            Fordon fordon = typ == "BIL" ? new Bil(regNr) : new MC(regNr);

            if (garage.FörsökParkera(fordon, out var msg))
                AnsiConsole.MarkupLine($"[green]{msg}[/]");
            else
                AnsiConsole.MarkupLine($"[red]{msg}[/]");

            return;
        }

        public static void UIFlytta(Parkeringshus garage)
        {

        }

        public static void UIHämta(Parkeringshus garage)
        {
            var regNr = AnsiConsole.Ask<string>("Ange registreringsnummer att hämta:").Trim().ToUpper();
            foreach (var plats in garage.Plats)
            {
                var f = plats.ParkeradeFordon.FirstOrDefault(x => x.RegNr.Equals(regNr, StringComparison.OrdinalIgnoreCase));
                if (f != null)
                {
                    int pris = Prislista.BeräknaPris(f, DateTime.Now);
                    AnsiConsole.MarkupLine($"[green]{f.FordonsTyp} {regNr} utlämnad[/] från plats [bold]{plats.platsNummer}[/].");
                    plats.ParkeradeFordon.Remove(f);
                    AnsiConsole.MarkupLine($"Pris att betala: [bold]{pris} kr[/]");

                    return;
                }
            }
            AnsiConsole.MarkupLine("[yellow]Fordonet hittades inte.[/]");
        }

        public static void UISök(Parkeringshus garage)
        {

        }

        public static void VäntaOchRensa()
        {
            AnsiConsole.MarkupLine("\n(Tryck valfri tangent för att fortsätta...)");
            Console.ReadKey(true);
            AnsiConsole.Clear();
        }
    }

}


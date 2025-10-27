using Spectre.Console;
using Spectre;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PragueParking2._0.DataAccess;
using PragueParking2._0.Models;
using System.Runtime.CompilerServices;


namespace PragueParking2._0.UI
{

    public class UserInterface
    {
        private Garage garage;
        private Config config;

        public UserInterface(Garage garage, Config config)
        {
            this.garage = garage;
            this.config = config;
        }
        public void Run()
        {
            bool running = true;
            while (running)
            {
                AnsiConsole.Clear();
                AnsiConsole.Write(
                    new FigletText("Prague Parking")
                    .Centered()
                    .Color(Color.DarkOrange));

                //Huvudmeny
                var panel = new Panel("\[white]Huvudmeny[/]")
                    .RoundedBorder()
                    .BorderColor(Color.DarkOrange);
                AnsiConsole.Write(panel);

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                    .Title("\nVälj ett alternativ:")
                    .AddChoices(new[] {
                "Parkera fordon",
                "Checka ut fordon",
                "Visa garage",
                "Avsluta" }));

                switch (choice)
                {
                    case "Parkera fordon":
                        ParkVehicleMenu();
                        break;
                    case "Checka ut fordon":
                        CheckOutMenu();
                        break;
                    case "Visa garage":
                        ShowGarage(garage);
                        break;
                    case "Avsluta":
                        running = false;
                        break;
                }
                AnsiConsole.MarkupLine("\nTryck på valfri tangent för att fortsätta...");
                Console.ReadKey();
            }
        }

        public void ParkVehicleMenu()
        {
            var type = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                .Title("\tVälj fordonstyp:")
                .AddChoices(new[] { "\tBil", "\tMC" }));

            string regNumber = AnsiConsole.Ask<string>("Ange registreringsnummer: ");

            Vehicle vehicle = type == "Bil" ? new Car(regNumber) : new MC(regNumber);

            int spotNumber = garage.ParkVehicle(vehicle, config); //få platsnummer på parkeringen

            if (spotNumber != -1) //om fordonet parkerades 
                AnsiConsole.MarkupLine($"[green]{vehicle.Type} {vehicle.RegNumber} parkerades på plats [bold]{spotNumber}[/][/]");
            else
                AnsiConsole.MarkupLine("[red] Ingen ledig plats för detta fordon.[/]");
        }

        public void CheckOutMenu()
        {
            string regNumber = AnsiConsole.Ask<string>("Ange registreringsnummer på fordonet som ska checkas ut: ");
            string message = garage.CheckOut(regNumber, config);

            AnsiConsole.Markup($"[white]{message}[/]");
        }
        public void ShowGarage(Garage garage)
        {
            AnsiConsole.Write(
                new FigletText("Prague Parking")
                .Centered()
                .Color(Color.White));

            AnsiConsole.WriteLine();

            var table = new Table();

            table.AddColumn("P-plats");
            table.AddColumn("Fordonstyp");
            table.AddColumn("Registreringsnummer");
            table.AddColumn("Incheckad");

            foreach (var spot in garage.Spots)
            {
                if (spot.ParkedVehicles.Count == 0)
                {
                    table.AddRow(
                        spot.SpotNumber.ToString(),
                    "[green] Ledig[/]",
                    "[green] - [/]",
                    "[green] - [/]");
                }
                else
                {
                    foreach (var vehicle in spot.ParkedVehicles)
                    {
                        table.AddRow(
                            spot.SpotNumber.ToString(),
                            $"[red]{vehicle.Type}[/]",
                            $"[red]{vehicle.RegNumber}[/]",
                            $"[red]{vehicle.CheckInTime: yyyy-MM-dd HH:mm}[/]");
                    }
                }

            }
            AnsiConsole.Write(table);
            AnsiConsole.WriteLine();

        }
    }
}

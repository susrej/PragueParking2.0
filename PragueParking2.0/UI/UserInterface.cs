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

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                    .AddChoices(new[] {
                "Parkera fordon",
                "Flytta fordon",
                "Checka ut fordon",
                "Visa garage",
                "Avsluta" }));

                switch (choice)
                {
                    case "Parkera fordon":
                        ParkVehicleMenu();
                        break;
                    case "Flytta fordon":
                        ShowOnlyParkedVehicles(garage);
                        MoveVehicleMenu();
                        break;
                    case "Checka ut fordon":
                        ShowOnlyParkedVehicles(garage);
                        CheckOutMenu();
                        break;
                    case "Visa garage":
                        ShowGarage(garage);

                        break;
                    case "Avsluta":
                        running = false;
                        break;
                }
            }
        }

        public void ParkVehicleMenu()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(
                new FigletText("Prague Parking")
                .Centered()
                .Color(Color.White));
            var type = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                .Title("Välj ett alternativ:")
                .AddChoices(new[] { "Bil", "MC", "Tillbaka till huvudmenyn" }));

            if (type == "Tillbaka till huvudmenyn") { return; }

            string regNumber = AnsiConsole.Ask<string>("Ange registreringsnummer: ");

            Vehicle vehicle = type == "Bil" ? new Car(regNumber) : new MC(regNumber);

            int spotNumber = garage.ParkVehicle(vehicle, config); //få platsnummer på parkeringen
            if (spotNumber == -2)
            {
                AnsiConsole.MarkupLine("[red]Fordon med detta registreringsnummer är redan parkerad.[/]");
                AnsiConsole.MarkupLine("\nTryck på valfri tangent för att återgå till huvudmenyn...");
                Console.ReadKey();
                return;
            }
            if (spotNumber != -1) //om fordonet parkerades 
                AnsiConsole.MarkupLine($"[green]{vehicle.Type} {vehicle.RegNumber} parkerades på plats [bold]{spotNumber}[/][/]");
            else
                AnsiConsole.MarkupLine("[red] Ingen ledig plats för detta fordon.[/]");
            AnsiConsole.MarkupLine("\nTryck på valfri tangent för att återgå till huvudmenyn...");
            Console.ReadKey();
        }

        public void MoveVehicleMenu()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(
                new FigletText("Prague Parking")
                .Centered()
                .Color(Color.White));
            var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Välj ett alternativ:")
                .AddChoices(new[] { "Flytta fordon", "Tillbaka till huvudmenyn" }));

            if (choice == "Tillbaka till huvudmenyn")
                return;
            ShowOnlyParkedVehicles(garage);
            string regNumber = AnsiConsole.Ask<string>("\nAnge registreringsnummer på fordonet som ska flyttas: ");
            int newSpot = AnsiConsole.Ask<int>("Ange ny parkeringsplats: ");

            bool result = garage.MoveVehicle(regNumber, newSpot, config, out string message);

            if (result)
                AnsiConsole.MarkupLine($"[green]{message}[/]");
            else
                AnsiConsole.MarkupLine($"[red]{message}[/]");
            AnsiConsole.MarkupLine("\nTryck på valfri tangent för att återgå till huvudmenyn...");
            Console.ReadKey();
        }

        public void CheckOutMenu()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(
                new FigletText("Prague Parking")
                .Centered()
                .Color(Color.White));

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
             .Title("Välj ett alternativ:")
             .AddChoices(new[] { "Checka ut fordon", "Tillbaka till huvudmenyn" }));

            if (choice == "Tillbaka till huvudmenyn")
                return;

            if (garage == null)
            {
                AnsiConsole.MarkupLine("Inga fordon parkerade");
                AnsiConsole.MarkupLine("Tryck på valfri tangent för att återgå till huvudmenyn");
                Console.ReadKey();
            }

            ShowOnlyParkedVehicles(garage);
            string regNumber = AnsiConsole.Ask<string>("\nAnge registreringsnummer på fordonet som ska checkas ut: ");

            string message = garage.CheckOut(regNumber, config);

            AnsiConsole.Markup($"\n[white]{message}[/]");
            AnsiConsole.MarkupLine("\nTryck på valfri tangent för att återgå till huvudmenyn...");
            Console.ReadKey();
            
        }

        public void ShowOnlyParkedVehicles(Garage garage)
        {
            AnsiConsole.Clear();
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
                if (spot.ParkedVehicles.Count > 0)
                {
                    foreach (var vehicle in spot.ParkedVehicles)
                    {
                        var color = vehicle is Car ? "red" : "yellow";
                        table.AddRow(
                            spot.SpotNumber.ToString(),
                            $"[{color}]{vehicle.Type}[/]",
                            $"[{color}]{vehicle.RegNumber.ToUpper()}[/]",
                            $"[{color}]{vehicle.CheckInTime: yyyy-MM-dd HH:mm}[/]");
                    }
                }
            }
            AnsiConsole.Write(table.Centered());

           
        }
        public void ShowGarage(Garage garage)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(
                new FigletText("Prague Parking")
                .Centered()
                .Color(Color.White));

            AnsiConsole.WriteLine();

            if (garage == null)
            {
                AnsiConsole.MarkupLine("Inga fordon parkerade");
                AnsiConsole.MarkupLine("Tryck på valfri tangent för att återgå till huvudmenyn");
                Console.ReadKey();
            }
            var table = new Table();

            table.AddColumn("P-plats");
            table.AddColumn(new TableColumn("Fordonstyp").Centered());
            table.AddColumn(new TableColumn("Registreringsnummer").Centered());
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
                    var types = string.Join(" | ", spot.ParkedVehicles.Select(v => v.Type).Distinct());
                    var regNumbers = string.Join(" | ", spot.ParkedVehicles.Select(v => v.RegNumber));
                    var checkInTimes = string.Join(" | ", spot.ParkedVehicles.Select(v => v.CheckInTime.ToString("yyyy-MM-dd HH:mm")));

                    var mcCount = spot.ParkedVehicles.Count(v => v is MC);
                    var color = spot.ParkedVehicles.Any(v => v is Car) || mcCount == 2 ? "red" : "yellow";
                    table.AddRow(
                        spot.SpotNumber.ToString(),
                        $"[{color}]{types}[/]",
                        $"[{color}]{regNumbers.ToUpper()}[/]",
                        $"[{color}]{checkInTimes}[/]");
                }
            }
            AnsiConsole.Write(table.Centered());


            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("\nTryck på valfri tangent för att återgå till huvudmenyn...");
            Console.ReadKey();

        }
    }

}

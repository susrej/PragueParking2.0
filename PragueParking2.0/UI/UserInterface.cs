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
                    .Color(Color.HotPink2));

                #region UI Prislista
                var pricePanel = new Panel("\n" +
                $"[white] Bil:[/] {config.CarRatePerHour} CZK/timme\n" +
                $"[white] MC:[/] {config.MCRatePerHour} CZK/timme\n\n" +
                $"[palegreen3_1] Fri parkering: {config.FreeMinutes} minuter[/]")
                {
                    Header = new PanelHeader("[hotpink2]Parkeringsavgifter[/]"),
                    Width = 60,
                    Border = BoxBorder.Rounded,
                    Padding = new Padding(1, 1, 1, 1)
                };
                pricePanel.BorderStyle = new Style(Color.MistyRose3);
                #endregion
                #region UI Garage status
                //Visa endast lediga platser i garaget
                int availableSpots = garage.Spots.Count(spot => spot.ParkedVehicles.Count == 0);
                var infoPanel = new Panel(
                    $"\n\nLediga platser: [palegreen3_1]{availableSpots}[/]\n\n")
                {
                    Header = new PanelHeader("[hotpink2]Parkeringsstatus[/]"),
                    Width = 43,
                    Border = BoxBorder.Rounded,
                    
                    Padding = new Padding(4, 1, 1, 1)
                };
                infoPanel.BorderStyle = new Style(Color.MistyRose3);
                AnsiConsole.Write(new Columns(pricePanel, infoPanel));
                #endregion
                #region UI Panel med menyval

                var menuChoices = new List<string> {
                    "Parkera fordon",
                    "Flytta fordon",
                    "Checka ut fordon",
                    "Incheckade fordon",
                    "Parkeringsöversikt",
                    "Avsluta" };

                int selectedIndex = 0;
                bool selected = false;
                ConsoleKey key;

                var miniMatrixPanel = MiniMatrixPanel(garage, config);
                AnsiConsole.Live(new Columns(miniMatrixPanel, new Panel(""))).Start(ctx =>
                {
                    while (!selected)
                    {
                        // Skapa meny-panel från menuChoices
                        var grid = new Grid();
                        grid.AddColumn();
                        foreach (var choice in menuChoices)
                        {
                            if (menuChoices.IndexOf(choice) == selectedIndex)
                                grid.AddRow($"[black on mistyrose3]> {choice}[/]"); // markerad
                            else
                                grid.AddRow($"  {choice}");
                        }

                        var menuPanel = new Panel(grid)
                        {
                            Header = new PanelHeader("[hotpink2]Huvudmeny[/]"),
                            Border = BoxBorder.Rounded,
                            Width = 60,
                            Padding = new Padding(1, 3, 1, 1)
                        };

                        menuPanel.BorderStyle = new Style(Color.MistyRose3);

                        // Visa Columns: meny till vänster, MiniMatrix till höger 
                        ctx.UpdateTarget(new Columns(menuPanel, miniMatrixPanel));

                        // Läs tangent
                        key = Console.ReadKey(true).Key;
                        if (key == ConsoleKey.UpArrow)
                            selectedIndex = (selectedIndex - 1 + menuChoices.Count) % menuChoices.Count;
                        else if (key == ConsoleKey.DownArrow)
                            selectedIndex = (selectedIndex + 1) % menuChoices.Count;
                        else if (key == ConsoleKey.Enter)
                            selected = true;
                    }
                });

                #region Switch case för menyval
                var selectedChoice = menuChoices[selectedIndex];
                switch (selectedChoice)
                {
                    case "Parkera fordon":
                        ParkVehicleMenuUI();
                        break;
                    case "Flytta fordon":
                        ShowOnlyParkedVehicles(garage);
                        MoveVehicleMenuUI();
                        break;
                    case "Checka ut fordon":
                        ShowOnlyParkedVehicles(garage);
                        CheckOutMenuUI();
                        break;
                    case "Incheckade fordon":
                        ShowOnlyParkedVehicles(garage);
                        AnsiConsole.MarkupLine("\n\nTryck på valfri tangent för att återgå till huvudmenyn...");
                        Console.ReadKey();
                        break;
                    case "Parkeringsöversikt":
                        ParkingOverviewMatrix(garage, config);
                        break;
                    case "Avsluta":
                        running = false;
                        break;
                }
                #endregion
                #endregion
            }
        }

        #region UI Methods
        public void ParkVehicleMenuUI()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(
                new FigletText("Prague Parking")
                .Centered()
                .Color(Color.HotPink2));

            int selectedIndex = 0;
            bool selected = false;
            ConsoleKey key;

            var menuChoices = new List<string> {
                "Registrera bil", "Registrera MC", "Tillbaka till huvudmenyn" };


            AnsiConsole.Live(new Panel("")).Start(ctx =>
            {
                while (!selected)
                {
                    // Skapa meny-panel från menuChoices
                    var grid = new Grid();
                    grid.AddColumn();
                    foreach (var choice in menuChoices)
                    {
                        if (menuChoices.IndexOf(choice) == selectedIndex)
                            grid.AddRow($"[black on mistyrose3]> {choice}[/]"); // markerad
                        else
                            grid.AddRow($"  {choice}");
                    }

                    var menuPanel = new Panel(grid)
                    {
                        Header = new PanelHeader("Parkera fordon"),
                        Border = BoxBorder.Rounded,
                        Width = 50,
                        Padding = new Padding(1, 2, 1, 1)
                    };

                    ctx.UpdateTarget(menuPanel);

                    // Läs tangent
                    key = Console.ReadKey(true).Key;
                    if (key == ConsoleKey.UpArrow)
                        selectedIndex = (selectedIndex - 1 + menuChoices.Count) % menuChoices.Count;
                    else if (key == ConsoleKey.DownArrow)
                        selectedIndex = (selectedIndex + 1) % menuChoices.Count;
                    else if (key == ConsoleKey.Enter)
                        selected = true;
                }
            });

            switch (menuChoices[selectedIndex])
            {
                case "Registrera bil":
                case "Registrera MC":
                    string regNumber = AnsiConsole.Ask<string>("Ange registreringsnummer: ");

                    Vehicle vehicle = menuChoices[selectedIndex] == "Registrera bil" ? new Car(regNumber) : new MC(regNumber);

                    int spotNumber = garage.ParkVehicle(vehicle, config); //få platsnummer på parkeringen
                    if (spotNumber == -2)
                    {
                        AnsiConsole.MarkupLine("[red]Fordon med detta registreringsnummer är redan parkerad.[/]");
                        AnsiConsole.MarkupLine("\nTryck på valfri tangent för att återgå till huvudmenyn...");
                        Console.ReadKey();
                        return;
                    }
                    if (spotNumber != -1) //om fordonet parkerades 
                        AnsiConsole.MarkupLine($"[palegreen3_1]{vehicle.Type} {vehicle.RegNumber.ToUpper()} parkerades på plats [bold]{spotNumber}[/][/]");
                    else
                        AnsiConsole.MarkupLine("[red] Ingen ledig plats för detta fordon.[/]");
                    AnsiConsole.MarkupLine("\nTryck på valfri tangent för att återgå till huvudmenyn...");
                    Console.ReadKey();
                    break;
                case "Avsluta":
                    break;
            }
        }

        public void MoveVehicleMenuUI()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(
                new FigletText("Prague Parking")
                .Centered()
                .Color(Color.HotPink2));

            int selectedIndex = 0;
            bool selected = false;
            ConsoleKey key;

            var menuChoices = new List<string> {
                "Flytta fordon", "Tillbaka till huvudmenyn" };

            AnsiConsole.Live(new Panel("")).Start(ctx =>
            {
                while (!selected)
                {
                    // Skapa meny-panel från menuChoices
                    var grid = new Grid();
                    grid.AddColumn();
                    foreach (var choice in menuChoices)
                    {
                        if (menuChoices.IndexOf(choice) == selectedIndex)
                            grid.AddRow($"[black on mistyrose3]> {choice}[/]"); // markerad
                        else
                            grid.AddRow($"  {choice}");
                    }

                    var menuPanel = new Panel(grid)
                    {
                        Header = new PanelHeader("Flytta fordon"),
                        Border = BoxBorder.Rounded,
                        Width = 40,
                        Padding = new Padding(1, 2, 1, 1)
                    };

                    ctx.UpdateTarget(menuPanel);

                    // Läs tangent
                    key = Console.ReadKey(true).Key;
                    if (key == ConsoleKey.UpArrow)
                        selectedIndex = (selectedIndex - 1 + menuChoices.Count) % menuChoices.Count;
                    else if (key == ConsoleKey.DownArrow)
                        selectedIndex = (selectedIndex + 1) % menuChoices.Count;
                    else if (key == ConsoleKey.Enter)
                        selected = true;
                }
            });

            switch (menuChoices[selectedIndex])
            {
                case "Flytta fordon":
                    ShowOnlyParkedVehicles(garage);
                    string regNumber = AnsiConsole.Ask<string>("\nAnge registreringsnummer på fordonet som ska flyttas: ");
                    int newSpot = AnsiConsole.Ask<int>("Ange ny parkeringsplats: ");

                    bool result = garage.MoveVehicle(regNumber, newSpot, config, out string message);

                    if (result)
                        AnsiConsole.MarkupLine($"[palegreen3_1]{message}[/]");
                    else
                        AnsiConsole.MarkupLine($"[red]{message}[/]");
                    AnsiConsole.MarkupLine("\nTryck på valfri tangent för att återgå till huvudmenyn...");
                    Console.ReadKey();
                    break;
                case "Tillbaka till huvudmenyn":
                    break;
            }
        }

        public void CheckOutMenuUI()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(
                new FigletText("Prague Parking")
                .Centered()
                .Color(Color.HotPink2));

            if (garage == null)
            {
                AnsiConsole.MarkupLine("Inga fordon parkerade");
                AnsiConsole.MarkupLine("Tryck på valfri tangent för att återgå till huvudmenyn");
                Console.ReadKey();
                return;
            }

            int selectedIndex = 0;
            bool selected = false;
            ConsoleKey key;

            var menuChoices = new List<string> {
                "Checka ut fordon", "Tillbaka till huvudmenyn" };

            AnsiConsole.Live(new Panel("")).Start(ctx =>
            {
                if (garage == null)
                {
                    AnsiConsole.MarkupLine("Inga fordon parkerade");
                    AnsiConsole.MarkupLine("Tryck på valfri tangent för att återgå till huvudmenyn");
                    Console.ReadKey();
                    return;
                }

                while (!selected)
                {
                    // Skapa meny-panel från menuChoices
                    var grid = new Grid();
                    grid.AddColumn();
                    foreach (var choice in menuChoices)
                    {
                        if (menuChoices.IndexOf(choice) == selectedIndex)
                            grid.AddRow($"[black on mistyrose3]> {choice}[/]"); // markerad
                        else
                            grid.AddRow($"  {choice}");
                    }

                    var menuPanel = new Panel(grid)
                    {
                        Header = new PanelHeader("Flytta fordon"),
                        Border = BoxBorder.Rounded,
                        Width = 40,
                        Padding = new Padding(1, 2, 1, 1)
                    };

                    ctx.UpdateTarget(menuPanel);

                    // Läs tangent
                    key = Console.ReadKey(true).Key;
                    if (key == ConsoleKey.UpArrow)
                        selectedIndex = (selectedIndex - 1 + menuChoices.Count) % menuChoices.Count;
                    else if (key == ConsoleKey.DownArrow)
                        selectedIndex = (selectedIndex + 1) % menuChoices.Count;
                    else if (key == ConsoleKey.Enter)
                        selected = true;
                }
            });

            switch (menuChoices[selectedIndex])
            {
                case "Checka ut fordon":
                    ShowOnlyParkedVehicles(garage);
                    string regNumber = AnsiConsole.Ask<string>("\nAnge registreringsnummer på fordonet som ska checkas ut: ");

                    string message = garage.CheckOut(regNumber, config);

                    AnsiConsole.Markup($"\n[white]{message}[/]");
                    AnsiConsole.MarkupLine("\nTryck på valfri tangent för att återgå till huvudmenyn...");
                    Console.ReadKey();
                    break;
                case "Tillbaka till huvudmenyn":
                    break;
            }
        }

        public void ShowOnlyParkedVehicles(Garage garage)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(
                new FigletText("Prague Parking")
                .Centered()
                .Color(Color.HotPink2));

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
        }
        public Panel MiniMatrixPanel(Garage garage, Config config)
        {
            var matrixBuilder = new StringBuilder();
            //for (int col = 1; col <= 10; col++)
            for (int row = 0; row < 10; row++)
            {
                //var rowCells = new List<string>();
                for (int col = 1; col <= 10; col++)
                {
                    int spotNumber = row * 10 + col;
                    var spot = garage.Spots.FirstOrDefault(s => s.SpotNumber == spotNumber);

                    if (spot.ParkedVehicles.Count == 0)
                    {
                        matrixBuilder.Append($"[palegreen3_1] {spot.SpotNumber:D2}[/] ");
                        
                    }
                    else
                    {
                        var vehicle = spot.ParkedVehicles.First();
                        var color = vehicle is Car ? "red" : "yellow";
                        matrixBuilder.Append($"[{color}] {spot.SpotNumber:D2}[/] ");
                        //rowCells.Add($"[{color}]{spot.SpotNumber}\n{vehicle.RegNumber.ToUpper()}[/]");
                    }
                }
                matrixBuilder.AppendLine();
            }
            return new Panel(new Markup(matrixBuilder.ToString()))
                .Header("[hotpink2]Parkeringsöversikt[/]", Justify.Center)
                .Border(BoxBorder.Rounded)
                .BorderColor(Color.MistyRose3)
                .Padding(0, 0, 0, 0);
        }
        public void ParkingOverviewMatrix(Garage garage, Config config)
        {
            AnsiConsole.Clear();
            AnsiConsole.WriteLine();

            var matrixBuilder = new StringBuilder();
            //for (int col = 1; col <= 10; col++)
            for (int row = 0; row < 10; row++)
            {
                //var rowCells = new List<string>();
                for (int col = 1; col <= 10; col++)
                {
                    int spotNumber = row * 10 + col;
                    var spot = garage.Spots.FirstOrDefault(s => s.SpotNumber == spotNumber);
                    //if (spot == null)
                    //{
                    //    matrixBuilder.Append("  [grey]N/A[/]    ");
                    //    //rowCells.Add("  [grey]N/A[/]    ");
                    //}
                    if (spot.ParkedVehicles.Count == 0)
                    {
                        matrixBuilder.Append($"[palegreen3_1]   {spot.SpotNumber:D2}[/]   ");
                        //rowCells.Add($"[palegreen3_1]  {spot.SpotNumber}[/]    ");
                    }
                    else
                    {
                        var vehicle = spot.ParkedVehicles.First();
                        var color = vehicle is Car ? "red" : "yellow";
                        matrixBuilder.Append($"[{color}]   {spot.SpotNumber:D2}[/]   ");
                        //rowCells.Add($"[{color}]{spot.SpotNumber}\n{vehicle.RegNumber.ToUpper()}[/]");
                    }
                }
                matrixBuilder.AppendLine();
                matrixBuilder.AppendLine();
            }
            var panel = new Panel(new Markup(matrixBuilder.ToString()))
                .Header("[hotpink2]Parkeringsöversikt[/]", Justify.Center)
                .Border(BoxBorder.Rounded)
                .BorderColor(Color.MistyRose3)
                .Padding(1, 2);

            AnsiConsole.Write(new Align(panel, HorizontalAlignment.Center));
            AnsiConsole.MarkupLine("\n\n\tTryck på valfri tangent för att återgå till huvudmenyn...");
            Console.ReadKey();
        }

        #region Old Methods       
        public void MoveVehicleMenu()
        {
            var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Välj ett alternativ:")
                .AddChoices(new[] { "Flytta fordon", "Tillbaka till huvudmenyn" }));

            if (choice == "Tillbaka till huvudmenyn")
                return;

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
        public void Test()
        {
            // Skapa en garagestatus-panel till höger

            int availableSpots = 0;
            List<string> parkedVehiclesRows = new List<string>();
            foreach (var spot in garage.Spots)
            {
                if (spot.ParkedVehicles.Count == 0)
                {
                    availableSpots++; // ✅ Öka lediga platser
                }
                else
                {
                    foreach (var vehicle in spot.ParkedVehicles)
                    {
                        var color = vehicle is Car ? "red" : "yellow";
                        parkedVehiclesRows.Add($"[{color}]Plats: {spot.SpotNumber}: " +
                            $" {vehicle.Type} " +
                            $" {vehicle.RegNumber.ToUpper()} " +
                            $" {vehicle.CheckInTime})[/]");
                    }
                }
            }
            if (parkedVehiclesRows.Count == 0)
            {
                parkedVehiclesRows.Add("[palegreen3_1]Inga parkerade fordon[/]");
            }
            int pageSize = 6;
            var visibleVehicles = parkedVehiclesRows.Take(pageSize);

            var infoPanel = new Panel($"[blue]Garage status: {availableSpots} lediga platser[/]\n\n" +
                $"[bold]Incheckade fordon:\n\n[/]" +
                $"{string.Join("\n", visibleVehicles)}\n\n")
            {
                Header = new PanelHeader("Parkeringsöversikt"),
                Width = 40,
                Border = BoxBorder.Rounded,
                Padding = new Padding(1, 1, 1, 1)
            };
        }
        public void CheckOutMenu()
        {
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
                return;
            }

            ShowOnlyParkedVehicles(garage);
            string regNumber = AnsiConsole.Ask<string>("\nAnge registreringsnummer på fordonet som ska checkas ut: ");

            string message = garage.CheckOut(regNumber, config);

            AnsiConsole.Markup($"\n[white]{message}[/]");
            AnsiConsole.MarkupLine("\nTryck på valfri tangent för att återgå till huvudmenyn...");
            Console.ReadKey();
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
            table.AddColumn(new TableColumn("Fordonstyp"));
            table.AddColumn(new TableColumn("Registreringsnummer").LeftAligned());
            table.AddColumn("Ankomst");

            foreach (var spot in garage.Spots)
            {
                if (spot.ParkedVehicles.Count == 0)
                {
                    table.AddRow(
                        spot.SpotNumber.ToString(),
                    "[palegreen3_1]Ledig[/]",
                    "[palegreen3_1] - [/]",
                    "[palegreen3_1] - [/]");
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
        //public void ShowOnlyParkedV(Garage garage)
        //{
        //    AnsiConsole.Clear();
        //    AnsiConsole.Write(
        //        new FigletText("Prague Parking")
        //        .Centered()
        //        .Color(Color.White));
        //    AnsiConsole.WriteLine();

        //    var table = new Table();
        //    table.BorderColor(Color.Black);
        //    table.AddColumn("P-plats");
        //    table.AddColumn("Fordonstyp");
        //    table.AddColumn("Registreringsnummer");
        //    table.AddColumn("Incheckad");
        //    foreach (var spot in garage.Spots)
        //    {
        //        if (spot.ParkedVehicles.Count > 0)
        //        {
        //            foreach (var vehicle in spot.ParkedVehicles)
        //            {
        //                var color = vehicle is Car ? "red" : "yellow";
        //                table.AddRow(
        //                    spot.SpotNumber.ToString(),
        //                    $"[{color}]{vehicle.Type}[/]",
        //                    $"[{color}]{vehicle.RegNumber.ToUpper()}[/]",
        //                    $"[{color}]{vehicle.CheckInTime: yyyy-MM-dd HH:mm}[/]");
        //            }
        //        }
        //    }
        //    var panel = new Panel(table)
        //        .Header("Parkeringsöversikt", Justify.Center)
        //        .Border(BoxBorder.Rounded)
        //        .BorderColor(Color.OrangeRed1)
        //        .Padding(1, 2);

        //    AnsiConsole.Write(new Align(panel, HorizontalAlignment.Center));

        //    Console.ReadKey();
        //}
        //#endregionpublic void ParkVehicleMenu()
        //{
        //    AnsiConsole.Clear();
        //    AnsiConsole.Write(
        //        new FigletText("Prague Parking")
        //        .Centered()
        //        .Color(Color.White));
        //    var type = AnsiConsole.Prompt(
        //        new SelectionPrompt<string>()
        //        .Title("Välj ett alternativ:")
        //        .AddChoices(new[] { "Bil", "MC", "Tillbaka till huvudmenyn" }));

        //    if (type == "Tillbaka till huvudmenyn") { return; }

        //    string regNumber = AnsiConsole.Ask<string>("Ange registreringsnummer: ");

        //    Vehicle vehicle = type == "Bil" ? new Car(regNumber) : new MC(regNumber);

        //    int spotNumber = garage.ParkVehicle(vehicle, config); //få platsnummer på parkeringen
        //    if (spotNumber == -2)
        //    {
        //        AnsiConsole.MarkupLine("[red]Fordon med detta registreringsnummer är redan parkerad.[/]");
        //        AnsiConsole.MarkupLine("\nTryck på valfri tangent för att återgå till huvudmenyn...");
        //        Console.ReadKey();
        //        return;
        //    }
        //    if (spotNumber != -1) //om fordonet parkerades 
        //        AnsiConsole.MarkupLine($"[green]{vehicle.Type} {vehicle.RegNumber.ToUpper()} parkerades på plats [bold]{spotNumber}[/][/]");
        //    else
        //        AnsiConsole.MarkupLine("[red] Ingen ledig plats för detta fordon.[/]");
        //    AnsiConsole.MarkupLine("\nTryck på valfri tangent för att återgå till huvudmenyn...");
        //    Console.ReadKey();
        //}
        #endregion
    }
}


#endregion
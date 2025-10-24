using PragueParking2._0.DataAccess;
using PragueParking2._0.Models;

var config = Config.Load();


// TEST: skriv ut värdena för att se att det fungerar
Console.WriteLine($"Gratis minuter: {config.FreeMinutes}");
Console.WriteLine($"Bil timpris: {config.CarRatePerHour} kr/h");
Console.WriteLine($"MC timpris: {config.MCRatePerHour} kr/h");



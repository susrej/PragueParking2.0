using PragueParking2._0.DataAccess;
using PragueParking2._0.Models;
using System.Runtime.InteropServices.Marshalling;
using Spectre;
using Spectre.Console;
using PragueParking2._0.UI;

Config.SyncFromTextFile();
var config = Config.Load();
var garage = GarageData.Load(config);

UserInterface ui = new UserInterface(garage, config);
ui.Run();
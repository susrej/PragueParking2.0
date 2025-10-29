using PragueParking2._0.DataAccess;
using PragueParking2._0.Models;

namespace PragueParking2._0.Tests
{
    [TestClass]
    public sealed class ParkingTests
    {
        [TestMethod]
        public void ParkVehicle_ShouldReturnMinusOneWhenGarageIsFull()
        {
            //Arrange
            var config = new Config();
            config.TotalParkingSpots = 1;//skapar ett garage med 1 plats
            var garage = new Garage(config);
            var firstVehicle = new Car("ABC123");
            var secondVehicle = new Car("DEF456");

            //Act
            garage.ParkVehicle(firstVehicle, config);
            int result = garage.ParkVehicle(secondVehicle, config);

            //Assert
            Assert.AreEqual(-1, result, "Parkeringsplatsen är full, borde returnera -1.");
        }
        
        [TestMethod]
        public void IsAvailable_ShouldReturnFalseWhenCarIsParked()
        {
            //Arrange
            var parkingSpot = new ParkingSpot();
            var parkedCar = new Car("ABC123");
            parkingSpot.ParkedVehicles.Add(parkedCar);
            var newMC = new MC("DEF456");

            //Act
            bool isAvailable = parkingSpot.IsAvailable(newMC, new Config());

            //Assert
            Assert.IsFalse(isAvailable, "Parkeringsplatsen har redan en bil, borde returnera false för MC.");
        }
    }
}

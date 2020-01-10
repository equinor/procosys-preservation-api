using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.MainApi.Client;
using Equinor.Procosys.Preservation.MainApi.Plant;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.MainApi.Tests.Plant
{
    [TestClass]
    public class MainApiPlantServiceTests
    {
        [TestMethod]
        public async Task Get_plants_returns_correct_number_or_plants_test_async()
        {
            // Arrange
            var mainApiClient = new Mock<IMainApiClient>();
            mainApiClient
                .Setup(x => x.QueryAndDeserialize<List<ProcosysPlant>>(It.IsAny<string>()))
                .Returns(Task.FromResult(new List<ProcosysPlant>
                {
                    new ProcosysPlant { Id = "PCS$AASTA_HANSTEEN", Title = "AastaHansteen" },
                    new ProcosysPlant { Id = "PCS$ASGARD", Title = "Åsgard" },
                    new ProcosysPlant { Id = "PCS$ASGARD_A", Title = "ÅsgardA" },
                    new ProcosysPlant { Id = "PCS$ASGARD_B", Title = "ÅsgardB" },
                }));
            var dut = new MainApiPlantService(mainApiClient.Object);

            // Act
            var result = await dut.GetPlants();

            // Assert
            Assert.AreEqual(4, result.Count());
        }

        [TestMethod]
        public async Task Get_plants_sets_correct_properties_test_async()
        {
            // Arrange
            var mainApiClient = new Mock<IMainApiClient>();
            mainApiClient
                .Setup(x => x.QueryAndDeserialize<List<ProcosysPlant>>(It.IsAny<string>()))
                .Returns(Task.FromResult(new List<ProcosysPlant>
                {
                    new ProcosysPlant { Id = "PCS$AASTA_HANSTEEN", Title = "AastaHansteen" }
                }));
            var dut = new MainApiPlantService(mainApiClient.Object);

            // Act
            var result = await dut.GetPlants();

            // Assert
            var plant = result.First();
            Assert.AreEqual("PCS$AASTA_HANSTEEN", plant.Id);
            Assert.AreEqual("AastaHansteen", plant.Title);
        }
    }
}

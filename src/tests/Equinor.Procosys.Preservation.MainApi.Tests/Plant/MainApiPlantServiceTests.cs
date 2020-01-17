using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.MainApi.Client;
using Equinor.Procosys.Preservation.MainApi.Plant;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.MainApi.Tests.Plant
{
    [TestClass]
    public class MainApiPlantServiceTests
    {
        [TestMethod]
        public async Task GetPlants_ReturnsCorrectNumberOfPlants_TestAsync()
        {
            // Arrange
            var mainApiOptions = new Mock<IOptionsMonitor<MainApiOptions>>();
            mainApiOptions
                .Setup(x => x.CurrentValue)
                .Returns(new MainApiOptions { ApiVersion = "4.0", BaseAddress = "http://example.com" });
            var mainApiClient = new Mock<IBearerTokenApiClient>();
            mainApiClient
                .Setup(x => x.QueryAndDeserialize<List<ProcosysPlant>>(It.IsAny<string>()))
                .Returns(Task.FromResult(new List<ProcosysPlant>
                {
                    new ProcosysPlant { Id = "PCS$AASTA_HANSTEEN", Title = "AastaHansteen" },
                    new ProcosysPlant { Id = "PCS$ASGARD", Title = "Åsgard" },
                    new ProcosysPlant { Id = "PCS$ASGARD_A", Title = "ÅsgardA" },
                    new ProcosysPlant { Id = "PCS$ASGARD_B", Title = "ÅsgardB" },
                }));
            var dut = new MainApiPlantService(mainApiClient.Object, mainApiOptions.Object);

            // Act
            var result = await dut.GetPlants();

            // Assert
            Assert.AreEqual(4, result.Count());
        }

        [TestMethod]
        public async Task GetPlants_SetsCorrectProperties_TestAsync()
        {
            // Arrange
            var mainApiOptions = new Mock<IOptionsMonitor<MainApiOptions>>();
            mainApiOptions
                .Setup(x => x.CurrentValue)
                .Returns(new MainApiOptions { ApiVersion = "4.0", BaseAddress = "http://example.com" });
            var mainApiClient = new Mock<IBearerTokenApiClient>();
            mainApiClient
                .Setup(x => x.QueryAndDeserialize<List<ProcosysPlant>>(It.IsAny<string>()))
                .Returns(Task.FromResult(new List<ProcosysPlant>
                {
                    new ProcosysPlant { Id = "PCS$AASTA_HANSTEEN", Title = "AastaHansteen" }
                }));
            var dut = new MainApiPlantService(mainApiClient.Object, mainApiOptions.Object);

            // Act
            var result = await dut.GetPlants();

            // Assert
            var plant = result.First();
            Assert.AreEqual("PCS$AASTA_HANSTEEN", plant.Id);
            Assert.AreEqual("AastaHansteen", plant.Title);
        }
    }
}

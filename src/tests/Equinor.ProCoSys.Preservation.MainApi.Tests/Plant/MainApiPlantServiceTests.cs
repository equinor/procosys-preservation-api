using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.MainApi.Client;
using Equinor.ProCoSys.Preservation.MainApi.Plant;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.MainApi.Tests.Plant
{
    [TestClass]
    public class MainApiPlantServiceTests
    {
        private readonly Guid _azureOid = Guid.NewGuid();
        private MainApiPlantService _dut;
        private readonly string _plantId = "PCS$AASTA_HANSTEEN";
        private readonly string _plantTitle = "AastaHansteen";
        private Mock<IAuthenticator> _authenticator;

        [TestInitialize]
        public void Setup()
        {
            // Arrange
            var mainApiOptions = new Mock<IOptionsSnapshot<MainApiOptions>>();
            mainApiOptions
                .Setup(x => x.Value)
                .Returns(new MainApiOptions { ApiVersion = "4.0", BaseAddress = "http://example.com" });
            var mainApiClient = new Mock<IBearerTokenApiClient>();
            _authenticator = new Mock<IAuthenticator>();
            mainApiClient
                .Setup(x => x.QueryAndDeserializeAsync<List<PCSPlant>>(It.IsAny<string>()))
                .Returns(Task.FromResult(new List<PCSPlant>
                {
                    new PCSPlant { Id = _plantId, Title = _plantTitle },
                    new PCSPlant { Id = "PCS$ASGARD", Title = "Åsgard" },
                    new PCSPlant { Id = "PCS$ASGARD_A", Title = "ÅsgardA" },
                    new PCSPlant { Id = "PCS$ASGARD_B", Title = "ÅsgardB" },
                }));

            _dut = new MainApiPlantService(_authenticator.Object, mainApiClient.Object, mainApiOptions.Object);
        }

        [TestMethod]
        public async Task GetAllPlants_ShouldReturnCorrectNumberOfPlants()
        {
            // Act
            var result = await _dut.GetAllPlantsForUserAsync(_azureOid);

            // Assert
            Assert.AreEqual(4, result.Count);
        }

        [TestMethod]
        public async Task GetAllPlants_ShouldSetsCorrectProperties()
        {
            // Act
            var result = await _dut.GetAllPlantsForUserAsync(_azureOid);

            // Assert
            var plant = result.First();
            Assert.AreEqual(_plantId, plant.Id);
            Assert.AreEqual(_plantTitle, plant.Title);
        }

        [TestMethod]
        public async Task GetAllPlants_ShouldSetApplicationAuthentication()
        {
            // Act
            await _dut.GetAllPlantsForUserAsync(_azureOid);

            // Assert
            _authenticator.VerifySet(a => a.AuthenticationType = AuthenticationType.AsApplication);
        }

        [TestMethod]
        public async Task GetAllPlants_ShouldResetToOnBehalfOfAuthentication()
        {
            // Act
            await _dut.GetAllPlantsForUserAsync(_azureOid);

            // Assert
            _authenticator.VerifySet(a => a.AuthenticationType = AuthenticationType.OnBehalfOf);
        }
    }
}

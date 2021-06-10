using System;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.WebApi.Authorizations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.WebApi.Tests.Authorizations
{
    [TestClass]
    public class CrossPlantAccessCheckerTests
    {
        private Guid _userOidWithAccess;
        private Guid _userOidWithoutAccess;
        private Mock<ICurrentUserProvider> _currentUserProviderMock;

        private CrossPlantAccessChecker _dut;

        [TestInitialize]
        public void Setup()
        {
            _userOidWithAccess = new Guid("00000000-0000-0000-0000-00000000000A");
            _userOidWithoutAccess = new Guid("00000000-0000-0000-0000-00000000000B");
            _currentUserProviderMock = new Mock<ICurrentUserProvider>();

            var attachmentOptionsMock = new Mock<IOptionsMonitor<AuthorizationOptions>>();
            var loggerMock = new Mock<ILogger<CrossPlantAccessChecker>>();
            var options = new AuthorizationOptions
            {
                CrossPlantUserOidList =  _userOidWithAccess.ToString()
            };
            attachmentOptionsMock
                .Setup(x => x.CurrentValue)
                .Returns(options);

            _dut = new CrossPlantAccessChecker(_currentUserProviderMock.Object, attachmentOptionsMock.Object, loggerMock.Object);
        }

        [TestMethod]
        public void HasCurrentUserAccessToCrossPlant_ShouldReturnFalse_WhenCurrentUserOidNotInConfig()
        {
            // Arrange
            _currentUserProviderMock.Setup(c => c.GetCurrentUserOid()).Returns(_userOidWithoutAccess);

            // Act
            var result = _dut.HasCurrentUserAccessToCrossPlant();

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void HasCurrentUserAccessToCrossPlant_ShouldReturnTrue_WhenCurrentUserOidInConfig()
        {
            // Arrange
            _currentUserProviderMock.Setup(c => c.GetCurrentUserOid()).Returns(_userOidWithAccess);

            // Act
            var result = _dut.HasCurrentUserAccessToCrossPlant();

            // Assert
            Assert.IsTrue(result);
        }
    }
}

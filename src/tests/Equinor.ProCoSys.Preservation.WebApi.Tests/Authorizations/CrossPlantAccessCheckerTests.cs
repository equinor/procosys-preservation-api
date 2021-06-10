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
        private Guid _userOidWithAccessA;
        private Guid _userOidWithAccessB;
        private Guid _userOidWithAccessC;
        private Guid _userOidWithoutAccess;
        private Mock<ICurrentUserProvider> _currentUserProviderMock;

        private CrossPlantAccessChecker _dut;

        [TestInitialize]
        public void Setup()
        {
            _userOidWithAccessA = new Guid("00000000-0000-0000-0000-00000000000A");
            _userOidWithAccessB = new Guid("00000000-0000-0000-0000-00000000000B");
            _userOidWithAccessC = new Guid("00000000-0000-0000-0000-00000000000C");
            _userOidWithoutAccess = new Guid("00000000-0000-0000-0000-00000000000D");
            _currentUserProviderMock = new Mock<ICurrentUserProvider>();

            var attachmentOptionsMock = new Mock<IOptionsMonitor<AuthorizationOptions>>();
            var loggerMock = new Mock<ILogger<CrossPlantAccessChecker>>();
            var options = new AuthorizationOptions
            {
                CrossPlantUserOidList = $"{_userOidWithAccessA},{_userOidWithAccessB},{_userOidWithAccessC}"
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
        public void HasCurrentUserAccessToCrossPlant_ShouldReturnTrue_WhenCurrentUserOidFirstInConfig()
        {
            // Arrange
            _currentUserProviderMock.Setup(c => c.GetCurrentUserOid()).Returns(_userOidWithAccessA);

            // Act
            var result = _dut.HasCurrentUserAccessToCrossPlant();

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void HasCurrentUserAccessToCrossPlant_ShouldReturnTrue_WhenCurrentUserOidInConfig()
        {
            // Arrange
            _currentUserProviderMock.Setup(c => c.GetCurrentUserOid()).Returns(_userOidWithAccessB);

            // Act
            var result = _dut.HasCurrentUserAccessToCrossPlant();

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void HasCurrentUserAccessToCrossPlant_ShouldReturnTrue_WhenCurrentUserOidLastInConfig()
        {
            // Arrange
            _currentUserProviderMock.Setup(c => c.GetCurrentUserOid()).Returns(_userOidWithAccessC);

            // Act
            var result = _dut.HasCurrentUserAccessToCrossPlant();

            // Assert
            Assert.IsTrue(result);
        }
    }
}

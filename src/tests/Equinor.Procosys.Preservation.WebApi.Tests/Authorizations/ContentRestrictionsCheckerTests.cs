﻿using System.Security.Claims;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.WebApi.Authorizations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.WebApi.Tests.Authorizations
{
    [TestClass]
    public class ContentRestrictionsCheckerTests
    {
        public readonly string ContentRestriction = "R1";
        private ContentRestrictionsChecker _dut;
        private ClaimsIdentity _claimsIdentity;
        private Claim _normalContentRestrictionClaim;
        private Claim _explicitNoRestrictionsClaim;

        [TestInitialize]
        public void Setup()
        {
            _normalContentRestrictionClaim = new Claim(ClaimTypes.UserData, ClaimsTransformation.GetContentRestrictionClaimValue(ContentRestriction));
            _explicitNoRestrictionsClaim = new Claim(ClaimTypes.UserData, ClaimsTransformation.GetContentRestrictionClaimValue(ClaimsTransformation.NoRestrictions));

            var principal = new ClaimsPrincipal();
            _claimsIdentity = new ClaimsIdentity();
            principal.AddIdentity(_claimsIdentity);
            var currentUserProviderMock = new Mock<ICurrentUserProvider>();
            currentUserProviderMock.Setup(u => u.GetCurrentUser()).Returns(principal);
            
            _dut = new ContentRestrictionsChecker(currentUserProviderMock.Object);
        }

        [TestMethod]
        public void HasCurrentUserExplicitNoRestrictions_ShouldReturnFalse_WhenNoClaims()
        {
            // Act
            var result = _dut.HasCurrentUserExplicitNoRestrictions();

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void HasCurrentUserExplicitNoRestrictions_ShouldReturnFalse_WhenNormalRestrictionClaimOnlyExist()
        {
            // Arrange
            _claimsIdentity.AddClaim(_normalContentRestrictionClaim);

            // Act
            var result = _dut.HasCurrentUserExplicitNoRestrictions();

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void HasCurrentUserExplicitNoRestrictions_ShouldReturnTrue_WhenNoRestrictionClaimExistsAlone()
        {
            // Arrange
            _claimsIdentity.AddClaim(_explicitNoRestrictionsClaim);

            // Act
            var result = _dut.HasCurrentUserExplicitNoRestrictions();

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void HasCurrentUserExplicitNoRestrictions_ShouldReturnFalse_WhenNoRestrictionClaimExistTogetherWithNormalRestrictionClaim()
        {
            // Arrange (invalid state)
            _claimsIdentity.AddClaim(_explicitNoRestrictionsClaim);
            _claimsIdentity.AddClaim(_normalContentRestrictionClaim);

            // Act
            var result = _dut.HasCurrentUserExplicitNoRestrictions();

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void HasCurrentUserExplicitAccessToContent_ShouldReturnTrue_WhenNormalRestrictionClaimExists()
        {
            _claimsIdentity.AddClaim(_normalContentRestrictionClaim);

            // Act
            var result = _dut.HasCurrentUserExplicitAccessToContent(ContentRestriction);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void HasCurrentUserExplicitAccessToContent_ShouldReturnFalse_WhenNormalRestrictionClaimNotExists()
        {
            _claimsIdentity.AddClaim(_normalContentRestrictionClaim);

            // Act
            var result = _dut.HasCurrentUserExplicitAccessToContent("XYZ");

            // Assert
            Assert.IsFalse(result);
        }
    }
}

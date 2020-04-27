using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.MainApi.Plant;
using Equinor.Procosys.Preservation.WebApi.Authorizations;
using Equinor.Procosys.Preservation.WebApi.Caches;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.WebApi.Tests.Authorizations
{
    [TestClass]
    public class ClaimsTransformationTests
    {
        private ClaimsTransformation _dut;
        private Guid Oid = new Guid("{0B627D64-8113-40E1-9394-60282FB6BB9F}");
        private ClaimsPrincipal _principalWithOid;
        private readonly string Plant = "P";
        private readonly string Permission1 = "A";
        private readonly string Permission2 = "B";
        private readonly string Project1 = "P1";
        private readonly string Project2 = "P2";
        private readonly string Restriction1 = "R1";
        private readonly string Restriction2 = "R2";
        private Mock<IPlantProvider> _plantProviderMock;
        private Mock<IPlantCache> _plantCacheMock;

        [TestInitialize]
        public void Setup()
        {
            _plantProviderMock = new Mock<IPlantProvider>();
            _plantProviderMock.SetupGet(p => p.Plant).Returns(Plant);

            _plantCacheMock = new Mock<IPlantCache>();
            _plantCacheMock.Setup(p => p.IsValidPlantForUserAsync(Plant, Oid)).Returns(Task.FromResult(true));

            var permissionCacheMock = new Mock<IPermissionCache>();
            permissionCacheMock.Setup(p => p.GetPermissionsForUserAsync(Plant, Oid))
                .Returns(Task.FromResult<IList<string>>(new List<string> {Permission1, Permission2}));
            permissionCacheMock.Setup(p => p.GetProjectNamesForUserOidAsync(Plant, Oid))
                .Returns(Task.FromResult<IList<string>>(new List<string> {Project1, Project2}));
            permissionCacheMock.Setup(p => p.GetContentRestrictionsForUserOidAsync(Plant, Oid))
                .Returns(Task.FromResult<IList<string>>(new List<string> {Restriction1, Restriction2}));

            _principalWithOid = new ClaimsPrincipal();
            var claimsIdentity = new ClaimsIdentity();
            claimsIdentity.AddClaim(new Claim(ClaimsExtensions.OidType, Oid.ToString()));
            _principalWithOid.AddIdentity(claimsIdentity);
            
            _dut = new ClaimsTransformation(_plantProviderMock.Object, _plantCacheMock.Object, permissionCacheMock.Object);
        }

        [TestMethod]
        public async Task TransformAsync_ShouldAddRoleClaimsForPermissions()
        {
            var result = await _dut.TransformAsync(_principalWithOid);

            AssertRoleClaims(result.Claims);
        }

        [TestMethod]
        public async Task TransformAsync_ShouldAddUserDataClaimsForProjects()
        {
            var result = await _dut.TransformAsync(_principalWithOid);

            AssertProjectClaims(result.Claims);
        }

        [TestMethod]
        public async Task TransformAsync_ShouldAddUserDataClaimsForContentRestriction()
        {
            var result = await _dut.TransformAsync(_principalWithOid);

            AssertContentRestriction(result.Claims);
        }

        [TestMethod]
        public async Task TransformAsync_ShouldNotAddAnyClaims_ForPrincipalWithoutOid()
        {
            var result = await _dut.TransformAsync(new ClaimsPrincipal());

            Assert.AreEqual(0, GetProjectClaims(result.Claims).Count);
            Assert.AreEqual(0, GetRoleClaims(result.Claims).Count);
            Assert.AreEqual(0, GetContentRestrictionClaims(result.Claims).Count);
        }

        [TestMethod]
        public async Task TransformAsync_ShouldNotAddAnyClaims_WhenNoPlantGiven()
        {
            _plantProviderMock.SetupGet(p => p.Plant).Returns((string)null);

            var result = await _dut.TransformAsync(_principalWithOid);

            Assert.AreEqual(0, GetProjectClaims(result.Claims).Count);
            Assert.AreEqual(0, GetRoleClaims(result.Claims).Count);
            Assert.AreEqual(0, GetContentRestrictionClaims(result.Claims).Count);
        }

        private void AssertRoleClaims(IEnumerable<Claim> claims)
        {
            var roleClaims = GetRoleClaims(claims);
            Assert.AreEqual(2, roleClaims.Count);
            Assert.IsTrue(roleClaims.Any(r => r.Value == Permission1));
            Assert.IsTrue(roleClaims.Any(r => r.Value == Permission2));
        }

        private void AssertProjectClaims(IEnumerable<Claim> claims)
        {
            var projectClaims = GetProjectClaims(claims);
            Assert.AreEqual(2, projectClaims.Count);
            Assert.IsTrue(projectClaims.Any(r => r.Value == ClaimsTransformation.GetProjectClaimValue(Project1)));
            Assert.IsTrue(projectClaims.Any(r => r.Value == ClaimsTransformation.GetProjectClaimValue(Project2)));
        }

        private void AssertContentRestriction(IEnumerable<Claim> claims)
        {
            var contentRestrictionClaims = GetContentRestrictionClaims(claims);
            Assert.AreEqual(2, contentRestrictionClaims.Count);
            Assert.IsTrue(contentRestrictionClaims.Any(r => r.Value == ClaimsTransformation.GetContentRestrictionClaimValue(Restriction1)));
            Assert.IsTrue(contentRestrictionClaims.Any(r => r.Value == ClaimsTransformation.GetContentRestrictionClaimValue(Restriction2)));
        }

        private static List<Claim> GetContentRestrictionClaims(IEnumerable<Claim> claims)
            => claims
                .Where(c => c.Type == ClaimTypes.UserData &&
                            c.Value.StartsWith(ClaimsTransformation.ContentRestrictionPrefix))
                .ToList();

        private static List<Claim> GetRoleClaims(IEnumerable<Claim> claims)
            => claims.Where(c => c.Type == ClaimTypes.Role).ToList();

        private static List<Claim> GetProjectClaims(IEnumerable<Claim> claims)
            => claims.Where(
                    c => c.Type == ClaimTypes.UserData && c.Value.StartsWith(ClaimsTransformation.ProjectPrefix))
                .ToList();
    }
}

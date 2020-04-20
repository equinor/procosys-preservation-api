using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.WebApi.Authorizations;
using Equinor.Procosys.Preservation.WebApi.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.WebApi.Tests.Authorizations
{
    [TestClass]
    public class ClaimsTransformationTests
    {
        private ClaimsTransformation _dut;
        private Guid Oid = new Guid("{0B627D64-8113-40E1-9394-60282FB6BB9F}");
        private ClaimsPrincipal _cp;
        private readonly string Plant1 = "X";
        private readonly string Plant2 = "Y";
        private readonly string Permission1 = "A";
        private readonly string Permission2 = "B";
        private readonly string Project1 = "P1";
        private readonly string Project2 = "P2";
        private readonly string Restriction1 = "R1";
        private readonly string Restriction2 = "R2";

        [TestInitialize]
        public void Setup()
        {
            var permissionServiceMock = new Mock<IPermissionService>();

            permissionServiceMock.Setup(p => p.GetPlantsForUserOidAsync(Oid))
                .Returns(Task.FromResult<IList<string>>(new List<string> {Plant1, Plant2}));
            permissionServiceMock.Setup(p => p.GetPermissionsForUserOidAsync(Oid))
                .Returns(Task.FromResult<IList<string>>(new List<string> {Permission1, Permission2}));
            permissionServiceMock.Setup(p => p.GetProjectsForUserOidAsync(Oid))
                .Returns(Task.FromResult<IList<string>>(new List<string> {Project1, Project2}));
            permissionServiceMock.Setup(p => p.GetContentRestrictionsForUserOidAsync(Oid))
                .Returns(Task.FromResult<IList<string>>(new List<string> {Restriction1, Restriction2}));

            _cp = new ClaimsPrincipal();
            var claimsIdentity = new ClaimsIdentity();
            claimsIdentity.AddClaim(new Claim(ClaimsExtensions.OidType, Oid.ToString()));
            _cp.AddIdentity(claimsIdentity);
            _dut = new ClaimsTransformation(permissionServiceMock.Object);
        }

        [TestMethod]
        public async Task TransformAsync_ShouldAddRoleClaimsForPermissions()
        {
            var result = await _dut.TransformAsync(_cp);

            AssertRoleClaims(result.Claims);
        }

        [TestMethod]
        public async Task TransformAsync_ShouldAddUserDataClaimsForPlants()
        {
            var result = await _dut.TransformAsync(_cp);

            AssertPlantClaims(result.Claims);
        }

        [TestMethod]
        public async Task TransformAsync_ShouldAddUserDataClaimsForProjects()
        {
            var result = await _dut.TransformAsync(_cp);

            AssertProjectClaims(result.Claims);
        }

        [TestMethod]
        public async Task TransformAsync_ShouldAddUserDataClaimsForContentRestriction()
        {
            var result = await _dut.TransformAsync(_cp);

            AssertContentRestriction(result.Claims);
        }

        private void AssertRoleClaims(IEnumerable<Claim> claims)
        {
            var roleClaims = claims.Where(c => c.Type == ClaimTypes.Role).ToList();
            Assert.AreEqual(2, roleClaims.Count);
            Assert.IsTrue(roleClaims.Any(r => r.Value == Permission1));
            Assert.IsTrue(roleClaims.Any(r => r.Value == Permission2));
        }

        private void AssertPlantClaims(IEnumerable<Claim> claims)
        {
            var plantClaims = claims
                .Where(c => c.Type == ClaimTypes.UserData && c.Value.StartsWith(ClaimsTransformation.PlantPrefix))
                .ToList();
            Assert.AreEqual(2, plantClaims.Count);
            Assert.IsTrue(plantClaims.Any(r => r.Value == ClaimsTransformation.GetPlantClaimValue(Plant1)));
            Assert.IsTrue(plantClaims.Any(r => r.Value == ClaimsTransformation.GetPlantClaimValue(Plant2)));
        }

        private void AssertProjectClaims(IEnumerable<Claim> claims)
        {
            var projectClaims = claims
                .Where(c => c.Type == ClaimTypes.UserData && c.Value.StartsWith(ClaimsTransformation.ProjectPrefix))
                .ToList();
            Assert.AreEqual(2, projectClaims.Count);
            Assert.IsTrue(projectClaims.Any(r => r.Value == ClaimsTransformation.GetProjectClaimValue(Project1)));
            Assert.IsTrue(projectClaims.Any(r => r.Value == ClaimsTransformation.GetProjectClaimValue(Project2)));
        }

        private void AssertContentRestriction(IEnumerable<Claim> claims)
        {
            var contentRestrictionClaims = claims
                .Where(c => c.Type == ClaimTypes.UserData &&
                            c.Value.StartsWith(ClaimsTransformation.ContentRestrictionPrefix))
                .ToList();
            Assert.AreEqual(2, contentRestrictionClaims.Count);
            Assert.IsTrue(contentRestrictionClaims.Any(r => r.Value == ClaimsTransformation.GetContentRestrictionClaimValue(Restriction1)));
            Assert.IsTrue(contentRestrictionClaims.Any(r => r.Value == ClaimsTransformation.GetContentRestrictionClaimValue(Restriction2)));
        }
    }
}

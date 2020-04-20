using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.WebApi.Authorizations;
using Equinor.Procosys.Preservation.WebApi.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.WebApi.Tests.Misc
{
    [TestClass]
    public class ClaimsTransformationTests
    {
        private ClaimsTransformation _dut;
        private Guid Oid = new Guid("{0B627D64-8113-40E1-9394-60282FB6BB9F}");
        private ClaimsPrincipal _cp;

        [TestInitialize]
        public void Setup()
        {
            var permissionServiceMock = new Mock<IPermissionService>();
            permissionServiceMock.Setup(p => p.GetPermissionsForUserOidAsync(Oid))
                .Returns(Task.FromResult<IList<string>>(new List<string> {"A", "B"}));
            permissionServiceMock.Setup(p => p.GetProjectsForUserOidAsync(Oid))
                .Returns(Task.FromResult<IList<string>>(new List<string> {"P1", "P2"}));
            permissionServiceMock.Setup(p => p.GetContentRestrictionsForUserOidAsync(Oid))
                .Returns(Task.FromResult<IList<string>>(new List<string> {"R1", "R2"}));

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

            Assert.AreEqual(2, result.Claims.Count(c => c.Type == ClaimTypes.Role));
        }

        [TestMethod]
        public async Task TransformAsync_ShouldAddUserDataClaimsForProjects()
        {
            var result = await _dut.TransformAsync(_cp);

            Assert.AreEqual(2, result.Claims.Count(c => c.Type == ClaimTypes.UserData && c.Value.StartsWith(ClaimsTransformation.ProjectPrefix)));
        }

        [TestMethod]
        public async Task TransformAsync_ShouldAddUserDataClaimsForContentRestriction()
        {
            var result = await _dut.TransformAsync(_cp);

            Assert.AreEqual(2, result.Claims.Count(c => c.Type == ClaimTypes.UserData && c.Value.StartsWith(ClaimsTransformation.ContentRestrictionPrefix)));
        }
    }
}

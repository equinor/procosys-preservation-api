using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators.Project;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.Validators
{
    [TestClass]
    public class ProjectValidatorTests
    {
        private const string ProjectNameNotClosed = "P";
        private const string ProjectNameClosed = "PClosed";
        private ProjectValidator _dut;

        [TestInitialize]
        public void Setup()
        {
            var projectRepositoryMock = new Mock<IProjectRepository>();

            var project = new Project("S", "P", "D");
            var projectClosed = new Project("S", "P", "D");
            projectClosed.Close();

            projectRepositoryMock.Setup(r => r.GetByNameAsync(ProjectNameNotClosed)).Returns(Task.FromResult(project));
            projectRepositoryMock.Setup(r => r.GetByNameAsync(ProjectNameClosed)).Returns(Task.FromResult(projectClosed));

            _dut = new ProjectValidator(projectRepositoryMock.Object);
        }

        [TestMethod]
        public void ValidateExists_KnownName_ReturnsTrue()
        {
            var result = _dut.Exists(ProjectNameNotClosed);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ValidateExists_UnknownName_ReturnsFalse()
        {
            var result = _dut.Exists("ASDR");
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ValidateIsClosed_KnownClosed_ReturnsTrue()
        {
            var result = _dut.IsClosed(ProjectNameClosed);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ValidateIsClosed_KnownNotClosed_ReturnsFalse()
        {
            var result = _dut.IsClosed(ProjectNameNotClosed);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ValidateIsClosed_UnknownName_ReturnsFalse()
        {
            var result = _dut.IsClosed("ASD");
            Assert.IsFalse(result);
        }
    }
}

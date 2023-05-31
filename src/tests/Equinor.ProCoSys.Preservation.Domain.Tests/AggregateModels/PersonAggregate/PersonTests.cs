using System;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.Domain.Tests.AggregateModels.PersonAggregate
{
    [TestClass]
    public class PersonTests
    {
        private const string TestPlant = "PCS$PlantA";
        private Guid Oid = Guid.NewGuid();

        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new Person(Oid, "Espen", "Askeladd");

            Assert.AreEqual(Oid, dut.Guid);
            Assert.AreEqual("Espen", dut.FirstName);
            Assert.AreEqual("Askeladd", dut.LastName);
        }

        [TestMethod]
        public void GetDefaultFilter_ShouldGetDefaultFilterWhenExists()
        {
            var dut = new Person(Oid, "firstName", "lastName");

            var project = new Project(TestPlant, "Project", "", new Guid("aec8297b-b010-4c5d-91e0-7b1c8664ced8"));
            var savedFilter = new SavedFilter(TestPlant, project, "title", "criteria")
            {
                DefaultFilter = true
            };

            dut.AddSavedFilter(savedFilter);

            // Act
            var result = dut.GetDefaultFilter(project.Id);

            // Arrange
            Assert.AreEqual(savedFilter, result);
        }
    }
}

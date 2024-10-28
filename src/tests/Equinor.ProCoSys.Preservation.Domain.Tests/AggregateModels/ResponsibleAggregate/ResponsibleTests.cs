using System;
using System.Linq;
using Equinor.ProCoSys.Common.Time;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.ProCoSys.Preservation.Domain.Events;
using Equinor.ProCoSys.Preservation.Test.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.Domain.Tests.AggregateModels.ResponsibleAggregate
{
    [TestClass]
    public class ResponsibleTests
    {
        [TestInitialize]
        public void Setup()
        {
            var utcNow = new DateTime(2020, 1, 1, 1, 1, 1, DateTimeKind.Utc);
            var timeProvider = new ManualTimeProvider(utcNow);
            TimeService.SetProvider(timeProvider);
        }
        
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new Responsible("PlantA", "CodeA", "DescA");

            Assert.AreEqual("PlantA", dut.Plant);
            Assert.AreEqual("CodeA", dut.Code);
            Assert.AreEqual("DescA", dut.Description);
        }

        [TestMethod]
        public void RenameResponsible_ShouldSetNewCode()
        {
            var newCode = "Code9";
            var dut = new Responsible("PlantA", "CodeA", "DescA");
            Assert.AreNotEqual(newCode, dut.Code);

            dut.RenameResponsible(newCode);

            Assert.AreEqual(newCode, dut.Code);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RenameResponsible_ShouldOnNullCode()
        {
            var dut = new Responsible("PlantA", "CodeA", "DescA");

            dut.RenameResponsible(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RenameResponsible_ShouldOnEmptyCode()
        {
            var dut = new Responsible("PlantA", "CodeA", "DescA");

            dut.RenameResponsible(" ");
        }
        
        [TestMethod]
        public void SetCreated_ShouldAddPlantEntityCreatedEvent()
        {
            var dut = new Responsible("PlantA", "CodeA", "DescA");
            var person = new Person(Guid.Empty, "Espen", "Askeladd");
            
            dut.SetCreated(person);
            var eventTypes = dut.DomainEvents.Select(e => e.GetType()).ToList();

            // Assert
            CollectionAssert.Contains(eventTypes, typeof(PlantEntityCreatedEvent<Responsible>));
        }
    }
}

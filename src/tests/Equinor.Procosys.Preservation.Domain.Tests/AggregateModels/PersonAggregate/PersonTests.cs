using System;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Domain.Tests.AggregateModels.PersonAggregate
{
    [TestClass]
    public class PersonTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var oid = Guid.NewGuid();

            var dut = new Person(oid, "Espen", "Askeladd");

            Assert.AreEqual(oid, dut.Oid);
            Assert.AreEqual("Espen", dut.FirstName);
            Assert.AreEqual("Askeladd", dut.LastName);
        }
    }
}

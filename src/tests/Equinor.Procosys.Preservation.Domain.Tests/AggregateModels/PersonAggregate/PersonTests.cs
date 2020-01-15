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
            var person = new Person(oid, "Espen", "Askeladd");

            Assert.AreEqual(oid, person.Oid);
            Assert.AreEqual("Espen", person.FirstName);
            Assert.AreEqual("Askeladd", person.LastName);
        }
    }
}
